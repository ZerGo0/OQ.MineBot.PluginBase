using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Classes.World;
using OQ.MineBot.Protocols.Classes.Base;

using TreeFarmerPlugin.Helpers;

namespace TreeFarmerPlugin.Tasks
{
    public class MineArea : ITask, ITickListener
    {
        public static Dictionary<List<ILocation>, bool> SharedTrees = new Dictionary<List<ILocation>, bool>();
        public static readonly object SHAREDTREES_LOCK_ONJ = new object();
        public static Dictionary<ILocation, bool> ReplantList = new Dictionary<ILocation, bool>();
        public static readonly object REPLANTLIST_LOCK_ONJ = new object();
        public static readonly object SCANNING_LOCK_ONJ = new object();
        
        private readonly ILocation _startLoc;
        private readonly ILocation _endLoc;
        private readonly bool _replant;
        private readonly MacroSync _fullInvMacro;
        private readonly List<ILocation> _areaList;

        private bool _debugBool;
        private bool _stopped;
        private List<ILocation> _localTreeLocs;
        private List<ILocation> _tempTreeLocs;
        private HelperFunctions _helperFunctions;
        public MineArea(ILocation startLoc, ILocation endLoc, bool replant, MacroSync fullInvMacro)
        {
            _startLoc = startLoc;
            _endLoc = endLoc;
            _replant = replant;
            _fullInvMacro = fullInvMacro;
            _areaList = CreateAreaList(startLoc, endLoc).ToList();

            var buildingBlocks = BlocksGlobal.BUILDING_BLOCKS.ToList();
            buildingBlocks.Remove(17);
            BlocksGlobal.BUILDING_BLOCKS = buildingBlocks.ToArray();
        }

        public override Task Stop()
        {
            SharedTreeListReset();
            ReplantListReset();
            _stopped = true;

            return null;
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_fullInvMacro.IsMacroRunning() &&
                   !_stopped;
        }

        public async Task OnTick()
        {
            try
            {
                if (_helperFunctions == null) _helperFunctions = new HelperFunctions(Context, Inventory);
                
                if (!_debugBool)
                {
                    if (_localTreeLocs == null)
                    {
                        ZerGo0Debugger.Debug(Context.Player.GetUsername(), "_localTreeLocs == null");
                        var tree = GetClosestTree();
                        if (tree == null)
                        {
                            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "tree == null");
                            var tempTree = await SearchClosestTree();
                            
                            if (tempTree == null || SharedTreeListContains(tempTree))
                            {
                                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "tempTree == null || SharedTreeListContains(tempTree)");
                                if (!await ReplantSapling()) await GoToSafeSpot();
                            }
                            else
                                AddTree(tempTree);
                            
                            return;
                        }
                        _localTreeLocs = tree;
                        _tempTreeLocs = tree;
                    }

                    if (_localTreeLocs.Count < 1)
                    {
                        _localTreeLocs = null;
                        if (_tempTreeLocs == null) SharedTreeListRemove(_tempTreeLocs);
                        _tempTreeLocs = null;
                        return;
                    }

                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), "woodBlock = _localTreeLocs.FirstOrDefault();");
                    var woodBlock = _localTreeLocs.FirstOrDefault();
                    if (woodBlock == null) return;
                    if (Context.World.GetBlockId(woodBlock) == 0)
                    {
                        _localTreeLocs.Remove(woodBlock);
                        return;
                    }

                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), "BreakBlock");
                    if (!await _helperFunctions.GoToLocation(woodBlock.Offset(-1), HelperFunctions.MAP_OPTIONS_MINE_BUILD)) return;
                    if (World.GetBlockId(woodBlock) != 0) 
                        if (!await _helperFunctions.BreakBlock(woodBlock)) return;
                    _localTreeLocs.Remove(woodBlock);
                    
//                    _debugBool = true;
                }
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
            }
        }

        private async Task GoToSafeSpot()
        {
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"GoToSafeSpot START");
            if (ReplantList == null || ReplantListCount() < 1 || 
                Context.World.GetBlockId(CurrentLoc()) != 6) return;
            
            var replantLoc = ReplantListClosest(true);
            if (replantLoc == null) return;

            var freeSpot = FreeSpot(replantLoc);
            if (freeSpot == null) return;

            await _helperFunctions.GoToLocation(freeSpot);
        }

        private ILocation FreeSpot(ILocation location)
        {
            if (location == null) return null;

            for (var x = location.X - 7; x <= location.X + 7; x++)
                for (var z = location.Z - 7; z <= location.Z + 7; z++)
                    for (var y = location.Y - 3; y <= location.Y + 3; y++)
                    {
                        var tempLoc = new Location(x, y, z);
                        if (tempLoc.Distance(CurrentLoc()) > 3 && Context.World.GetBlockId(tempLoc) == 0 &&
                            Context.World.GetBlockId(tempLoc.Offset(-1)) != 0)
                            return tempLoc;
                    }

            return null;
        }

        private async Task<bool> ReplantSapling()
        {
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"ReplantSapling START");
            if (ReplantList == null || ReplantListCount() < 1) return false;
            
            
            var replantLoc = ReplantListClosest();
            if (replantLoc == null || Context.World.GetBlockId(replantLoc) != 0) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"replantLoc == null || Context.World.GetBlockId(replantLoc) != 0");
            ReplantListSet(replantLoc, true);
            if (!await _helperFunctions.GoToLocation(replantLoc.Offset(-1), HelperFunctions.MAP_OPTIONS_MINE_BUILD))
            {
                ReplantListSet(replantLoc, false);
                return false;
            }
            if (!await _helperFunctions.PlaceBlockAt(replantLoc, 6, 0))
            {
                ReplantListSet(replantLoc, false);
                return false;
            }
            ReplantListSet(replantLoc, false);
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"ReplantSapling END");

            return true;
        }

        private List<ILocation> SortByReplantLoc(List<ILocation> woodBlocks)
        {
            var replantLoc = GetReplantLoc(woodBlocks).FirstOrDefault();

            return replantLoc == null ? null : woodBlocks.OrderByDescending(location => 
                replantLoc.Distance(location)).ToList();
        }

        private List<ILocation> GetReplantLoc(List<ILocation> woodBlocks)
        {
            var replantLocs = woodBlocks?.FindAll(location => location != null &&
                                                   Context.World.GetBlockId(location) == 17 &&
                                                   (Context.World.GetBlockId(location.Offset(-1)) == 2 ||
                                                    Context.World.GetBlockId(location.Offset(-1)) == 3));

            if (replantLocs == null) return null;
            
            foreach (var replantLoc in replantLocs.Where(replantLoc => !ReplantListContains(replantLoc))) 
                ReplantListAdd(replantLoc);

            return replantLocs;
        }

        private async Task<List<ILocation>> SearchClosestTree()
        {
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "SearchClosestTree START");
            var closestWoodBlock = await Context.World.FindClosest(50, 50, 17, 
                CpuMode.Medium_Usage, block => _areaList.Contains(block.GetLocation()) &&
                                               !SharedTreeListContains(block.GetLocation()));

            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "SearchClosestTree END");
            return closestWoodBlock == null ? null : _helperFunctions.GetWoodBlocks(closestWoodBlock.GetLocation()).ToList();
        }

        private void AddTree(List<ILocation> woodBlockList)
        {
            if (woodBlockList == null || woodBlockList.Count < 1) return;
            
            SharedTreeListAdd(woodBlockList);
        }
        
        private List<int> GetCaneSlots(bool enchanted)
        {
            var window = Context.Containers.GetOpenWindow();
            var sugarSlots = (from slot in window.GetSlots() where slot.Id == 338 && slot.IsStackFull() where (enchanted && slot.GetName().Contains("Enchanted")) || (!enchanted && !slot.GetName().Contains("Enchanted")) select slot.Index).ToList();
            return sugarSlots;
        }

        private IEnumerable<ILocation> CreateAreaList(ILocation startLoc, ILocation endLoc)
        {
            var tempList = new List<ILocation>();

            var xCoords = new[] {startLoc.X, endLoc.X};
            var yCoords = new[] {startLoc.Y, endLoc.Y};
            var zCoords = new[] {startLoc.Z, endLoc.Z};
            
            for (var y = yCoords.Min(); y <= yCoords.Max(); y++)
                for (var x = xCoords.Min(); x <= xCoords.Max(); x++)
                    for (var z = zCoords.Min(); z <= zCoords.Max(); z++)
                        tempList.Add(new Location(x, y, z));

            return tempList;
        }

        private ILocation CurrentLoc()
        {
            return Context.Player.GetLocation();
        }

#region SharedBlockList Lock Stuff

        private List<ILocation> GetClosestTree()
        {
            if (SharedTrees == null) return null;

            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "GetClosestTree START");
            List<ILocation> treeClosest;
            lock (SHAREDTREES_LOCK_ONJ)
            {
                var unusedTrees = SharedTrees.Where(pair => !pair.Value).Select(pair => pair.Key).ToList();
                if (!unusedTrees.Any()) return null;

                treeClosest = unusedTrees.OrderBy(list =>
                    list.OrderBy(location => location.Distance(CurrentLoc()))).FirstOrDefault();

                if (treeClosest == null) return null;
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "GetClosestTree treeClosest == null");
                
                SharedTrees[treeClosest] = true;
                
                var sortedByReplantLoc = SortByReplantLoc(treeClosest);

                if (sortedByReplantLoc != null)
                    treeClosest = sortedByReplantLoc;
            }
            
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "GetClosestTree END");

            return treeClosest;
        }

        private void SharedTreeListAdd(List<ILocation> locations)
        {
            if (SharedTrees == null) return;

            lock (SHAREDTREES_LOCK_ONJ)
            {
                SharedTrees.Add(locations, false);
            }
        }

        private void SharedTreeListRemove(List<ILocation> locations)
        {
            if (SharedTrees == null) return;

            lock (SHAREDTREES_LOCK_ONJ)
            {
                SharedTrees.Remove(locations);
            }
        }

        private bool SharedTreeListContains(ILocation location)
        {
            if (SharedTrees == null) return false;

            bool contains;
            lock (SHAREDTREES_LOCK_ONJ)
            {
                contains = SharedTrees.Keys.Any(list => list.Contains(location));
            }

            return contains;
        }
        
        private bool SharedTreeListContains(List<ILocation> locations)
        {
            if (SharedTrees == null) return false;

            lock (SHAREDTREES_LOCK_ONJ)
            {
                if (locations.Select(location => SharedTrees.Keys.Any(list => 
                    list.Contains(location))).Any(contains => contains))
                    return true;
            }

            return false;
        }

        private int SharedTreeListCount()
        {
            if (SharedTrees == null) return 0;

            int count;
            lock (SHAREDTREES_LOCK_ONJ)
            {
                count = SharedTrees.Count;
            }

            return count;
        }

        private void SharedTreeListReset()
        {
            if (SharedTrees == null) return;

            lock (SHAREDTREES_LOCK_ONJ)
            {
                SharedTrees = new Dictionary<List<ILocation>, bool>();
            }
        }

#endregion
        
#region ReplantList Lock Stuff

        private void ReplantListAdd(ILocation location)
        {
            if (ReplantList == null) return;

            lock (REPLANTLIST_LOCK_ONJ)
            {
                ReplantList.Add(location, false);
            }
        }

        private void ReplantListRemove(ILocation location)
        {
            if (ReplantList == null) return;

            lock (REPLANTLIST_LOCK_ONJ)
            {
                ReplantList.Remove(location);
            }
        }

        private ILocation ReplantListClosest(bool value = false)
        {
            if (ReplantList == null) return null;

            ILocation closestLoc;
            lock (REPLANTLIST_LOCK_ONJ)
            {
                if (value)
                    closestLoc = ReplantList.OrderBy(pair => pair.Key.Distance(CurrentLoc()))
                        .FirstOrDefault().Key;
                else
                    closestLoc = ReplantList.OrderBy(pair => pair.Key.Distance(CurrentLoc()))
                        .FirstOrDefault(pair => !pair.Value && Context.World.GetBlockId(pair.Key) == 0).Key;
            
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"closestLoc: {closestLoc}");
            }

            return closestLoc;
        }

        private bool ReplantListContains(ILocation location)
        {
            if (ReplantList == null) return false;

            bool contains;
            lock (REPLANTLIST_LOCK_ONJ)
            {
                contains = ReplantList.ContainsKey(location);
            }

            return contains;
        }

        private bool ReplantListSet(ILocation location, bool value)
        {
            if (ReplantList == null) return false;
            
            lock (REPLANTLIST_LOCK_ONJ)
            {
                var replantLoc= ReplantList.FirstOrDefault(pair => pair.Key.Compare(location)).Key;
                if (replantLoc == null) return false;
                ReplantList[replantLoc] = value;
                return true;
            }
        }
        

        private int ReplantListCount()
        {
            if (ReplantList == null) return 0;

            int count;
            lock (REPLANTLIST_LOCK_ONJ)
            {
                count = ReplantList.Count;
            }

            return count;
        }

        private void ReplantListReset()
        {
            if (SharedTrees == null) return;

            lock (ReplantList)
            {
                ReplantList = new Dictionary<ILocation, bool>();
            }
        }

#endregion
    }
}