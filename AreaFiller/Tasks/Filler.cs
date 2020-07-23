using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AreaFiller.Helpers;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;

namespace AreaFiller.Tasks
{
    public class Filler : ITask, ITickListener
    {
        public static List<ILocation> CurrentLayer;
        public static List<string> DoingStuff;
        public static int CurrentLayerY;
        public static int MaxLayerY;

        public static readonly object CURRENTLAYER_LOCK_ONJ = new object();
        private readonly ushort[] _buildingBlocks;
        private readonly ILocation _endLoc;
        private readonly int _fillerId;
        private readonly MacroSync _fillerMacro;

        private readonly ILocation _startLoc;

        private HelperFunctions _helperFunctions;

        public Filler(ILocation startLoc, ILocation endLoc, string fillerId, MacroSync fillerMacro)
        {
            _startLoc = startLoc;
            _endLoc = endLoc;
            _fillerId = int.Parse(fillerId);
            _fillerMacro = fillerMacro;

            _buildingBlocks = BlocksGlobal.BUILDING_BLOCKS;

            DoingStuff ??= new List<string>();

            if (_startLoc.Y <= _endLoc.Y)
            {
                CurrentLayerY = (int) startLoc.Y;
                MaxLayerY = (int) endLoc.Y;
            }
            else
            {
                CurrentLayerY = (int) _endLoc.Y;
                MaxLayerY = (int) startLoc.Y;
            }
        }

        public async Task OnTick()
        {
            try
            {
                _helperFunctions ??= new HelperFunctions(Context, Inventory);

                if (CurrentLayer != null && CurrentLayerCount() < 1 && CurrentLayerY > MaxLayerY) return;

                if (CurrentLayer == null || DoingStuff?.Count < 1 && CurrentLayerCount() < 1)
                    GetNewLayer();
                if (CurrentLayer == null) return;

                if (CurrentLayer != null && CurrentLayerCount() > 0)
                {
                    DoingStuff?.Add(Context.Player.GetUsername());

                    var currentBlock = GetClosestBlock();
                    if (currentBlock == null) return;

                    if (!await _helperFunctions.GoToLocation(currentBlock, HelperFunctions.MAP_OPTIONS_MINE_BUILD))
                    {
                        DoingStuff?.Remove(Context.Player.GetUsername());
                        return;
                    }

                    await PlaceBlocksAround(currentBlock, _fillerId);
                    DoingStuff?.Remove(Context.Player.GetUsername());
                }
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
            }
        }

        public override async Task Start()
        {
            BlocksGlobal.BUILDING_BLOCKS = new[] {(ushort) _fillerId};

            await Context.TickManager.Sleep(1);
        }

        public override async Task Stop()
        {
            BlocksGlobal.BUILDING_BLOCKS = _buildingBlocks;

            await Context.TickManager.Sleep(1);
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_fillerMacro.IsMacroRunning() &&
                   Inventory.GetAmountOfItem((ushort) _fillerId) > 0;
        }

        #region Functions

        private ILocation GetClosestBlock()
        {
            ILocation currentBlock;
            lock (CURRENTLAYER_LOCK_ONJ)
            {
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Getting Location");
                currentBlock = CurrentLayer.Where(location => World.GetBlockId(location) == 0).OrderBy(location => Context.Player.GetLocation().Distance(location))
                    .FirstOrDefault();
                if (currentBlock != null)
                {
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"currentBlock: {currentBlock}");
                    CurrentLayerRemove(currentBlock);
                }
                else
                {
                    DoingStuff?.Remove(Context.Player.GetUsername());
                    return null;
                }
            }

            return currentBlock;
        }

        private void GetNewLayer()
        {
            lock (CURRENTLAYER_LOCK_ONJ)
            {
                if (CurrentLayer != null && (!(DoingStuff?.Count < 1) || CurrentLayerCount() >= 1)) return;

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Getting new layer locations!");
                CurrentLayer = _helperFunctions.LayerOpenSpots(_startLoc, _endLoc, CurrentLayerY);
                CurrentLayerY++;
            }
        }

        private async Task PlaceBlocksAround(ILocation location, int blockId)
        {
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"PlaceBlocksAround: {location}");
            foreach (var blockAround in _helperFunctions.CheckBlocksAroundLoc(location, 3))
            {
                ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                    $"blockAround: {blockAround} | CanPlaceAt: {Context.World.GetBlock(blockAround).CanPlaceAt()}");

                if (!CurrentLayerContains(blockAround) && !location.Compare(blockAround)) continue;

                if (!Context.World.GetBlock(blockAround).CanPlaceAt())
                    continue;

                CurrentLayerRemove(blockAround);

                if (await _helperFunctions.PlaceBlockAt(blockAround, blockId, 0)) continue;

                CurrentLayerAdd(blockAround);
                return;
            }
        }

        #endregion

        #region Helper Functions

        #region CurrentLayer Lock Stuff

        private void CurrentLayerAdd(ILocation location)
        {
            if (CurrentLayer == null) return;

            lock (CURRENTLAYER_LOCK_ONJ)
            {
                CurrentLayer.Add(location);
            }
        }

        private void CurrentLayerRemove(ILocation location)
        {
            if (CurrentLayer == null) return;

            lock (CURRENTLAYER_LOCK_ONJ)
            {
                CurrentLayer.Remove(location);
            }
        }

        private bool CurrentLayerContains(ILocation location)
        {
            if (CurrentLayer == null) return false;

            bool contains;
            lock (CURRENTLAYER_LOCK_ONJ)
            {
                contains = CurrentLayer.Contains(location);
            }

            return contains;
        }

        private int CurrentLayerCount()
        {
            if (CurrentLayer == null) return 0;

            int count;
            lock (CURRENTLAYER_LOCK_ONJ)
            {
                count = CurrentLayer.Count;
            }

            return count;
        }

        #endregion

        private double DistancePos(IPosition startPos, IPosition endPos)
        {
            return startPos.Distance(endPos);
        }

        private IPosition GetEyePos()
        {
            return Context.Player.GetPosition().Offset(1.62);
        }

        #endregion
    }
}