using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace AreaFiller.Tasks
{
    public class FillerArea : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.MEDIUM,
            Mine = true
        };

        #region Shared work.

        /// <summary>
        ///     Blocks that are taken already.
        /// </summary>
        public static readonly ConcurrentDictionary<ILocation, DateTime> Broken =
            new ConcurrentDictionary<ILocation, DateTime>();

        public static IRadius SharedArea;
        public static readonly ConcurrentDictionary<string, int> BotNumber =
            new ConcurrentDictionary<string, int>();
        public static readonly ConcurrentDictionary<string, IRadius> BotWork =
            new ConcurrentDictionary<string, IRadius>();

        public static int openBotNumber = 0;
        private static int _botCount = 0;
        public static int BotCount
        {
            get => _botCount;
            set
            {
                _botCount = value;
                UpdateSharedArea(value);
            }
        }

        public static void UpdateSharedArea(int botCount)
        {
            //Console.WriteLine($"Botcount: {botCount} | OpenNumber: {openBotNumber} | BotNumber.Count: {BotNumber.Count}");

            //Update assigned botnumber if a bot stops the plugin
            if (BotNumber.Count >= botCount)
            {
                var updateBot = new List<string>(); 
                foreach (var bot in BotNumber)
                {
                    if (bot.Value < botCount) continue;
                    
                    updateBot.Add(bot.Key);
                    updateBot.Add(bot.Value.ToString());
                }

                if (updateBot.Count > 0)
                {
                    BotNumber.TryUpdate(updateBot[0], openBotNumber, int.Parse(updateBot[1]));
                }
                openBotNumber = 0;
            }
            
            //No need to split the area if there is no bot or if there is only 1 bot
            if (botCount < 2) return;

            if (botCount % 2 == 0)
            {
                var distanceX = Math.Abs(SharedArea.xSize);
                var distanceZ = Math.Abs(SharedArea.zSize);

                if (distanceX > distanceZ)
                {
                    var splittedX = Math.Ceiling(distanceX / ((double) botCount / 2));
                    var splittedZ = Math.Ceiling((double) distanceZ / 2);
                    //TODO: Check if distanceX or distanceZ is bigger and split area accordingly
                    var areaList = new List<IRadius>();
                    for (int i = 0; i < botCount / 2; i++)
                    {
                        var startX = SharedArea.start.x + splittedX * i;
                        var startZ = SharedArea.start.z;

                        //TODO: Check for overlapping
                        var endX = startX + splittedX;
                        var endZ = startZ + splittedZ;

                        if (startX >= SharedArea.start.x + distanceX) continue;

                        if (endX > SharedArea.start.x + distanceX) endX = SharedArea.start.x + distanceX;

                        var startLocation = new Location((int) startX, SharedArea.start.y, startZ);
                        var endLocation = new Location((int) endX, SharedArea.start.y + SharedArea.height, (int) endZ);

                        areaList.Add(new IRadius(startLocation, endLocation));
                        //Console.WriteLine($"Start: {startLocation} | End: {endLocation}");

                        startZ = SharedArea.start.z + (int) splittedZ;

                        //TODO: Check for overlapping
                        endZ = startZ + splittedZ;

                        if (startX >= SharedArea.start.x + distanceX) continue;

                        if (endZ > SharedArea.start.z + distanceZ) endZ = SharedArea.start.z + distanceZ;

                        startLocation = new Location((int) startX, SharedArea.start.y, startZ);
                        endLocation = new Location((int) endX, SharedArea.start.y + SharedArea.height, (int) endZ);

                        areaList.Add(new IRadius(startLocation, endLocation));
                    }

                    var count = 0;
                    foreach (var bot in BotNumber)
                    {
                        BotWork.TryRemove(bot.Key, out _);
                        if (count > areaList.Count) continue;

                        BotWork.TryAdd(bot.Key, areaList[count]);
                        count++;
                    }

                    foreach (var botworker in BotWork)
                    {
                        Console.WriteLine($"{botworker.Key} | {botworker.Value}");
                    }
                }
                else
                {
                    var splittedX = Math.Ceiling((double) distanceX / 2);
                    var splittedZ = Math.Ceiling(distanceZ / ((double) botCount / 2));
                    //TODO: Check if distanceX or distanceZ is bigger and split area accordingly
                    var areaList = new List<IRadius>();
                    for (int i = 0; i < botCount / 2; i++)
                    {
                        var startX = SharedArea.start.x;
                        var startZ = SharedArea.start.z + splittedZ * i;

                        //TODO: Check for overlapping
                        var endX = startX + splittedX;
                        var endZ = startZ + splittedZ;

                        if (startZ >= SharedArea.start.z + distanceZ) continue;

                        if (endZ > SharedArea.start.z + distanceZ) endZ = SharedArea.start.z + distanceZ;

                        var startLocation = new Location(startX, SharedArea.start.y, (int) startZ);
                        var endLocation = new Location((int) endX, SharedArea.start.y + SharedArea.height, (int) endZ);

                        areaList.Add(new IRadius(startLocation, endLocation));
                        //Console.WriteLine($"Start: {startLocation} | End: {endLocation}");

                        startX = SharedArea.start.x + (int) splittedX;

                        //TODO: Check for overlapping
                        endX = startX + splittedX;

                        if (startZ >= SharedArea.start.z + distanceZ) continue;

                        if (endX > SharedArea.start.x + distanceX) endX = SharedArea.start.x + distanceX;

                        startLocation = new Location(startX, SharedArea.start.y,(int) startZ);
                        endLocation = new Location((int) endX, SharedArea.start.y + SharedArea.height, (int) endZ);

                        areaList.Add(new IRadius(startLocation, endLocation));
                    }

                    var count = 0;
                    foreach (var bot in BotNumber)
                    {
                        BotWork.TryRemove(bot.Key, out _);
                        if (count > areaList.Count) continue;

                        BotWork.TryAdd(bot.Key, areaList[count]);
                        count++;
                    }

                    foreach (var botworker in BotWork)
                    {
                        Console.WriteLine($"{botworker.Key} | {botworker.Value}");
                    }
                }
            }
            else
            {
                //TODO: Split Area when bot amount is uneven
            }
        }
        
        
        private int _botNumber;

        #endregion

        private readonly MacroSync _macro;
        private readonly MacroSync _fillermacro;
        private IRadius _radius;
        private IRadius _prevRadius;

        private bool _busy;
        private readonly string _blockId;
        private ILocation _target;
        private IAsyncMap _targetblock;

        public FillerArea(string blockId, ILocation startLocation, ILocation endLocation, MacroSync macro, MacroSync fillerMacro)
        {
            _blockId = blockId;
            _macro = macro;
            _fillermacro = fillerMacro;

            var split = new[] {_blockId};
            var blocks = split.Select(ushort.Parse).ToArray();
            BlocksGlobal.BUILDING_BLOCKS = blocks;

            SharedArea = new IRadius(startLocation, endLocation);
        }

        public override void Start()
        {
            BotNumber.TryAdd(player.status.uuid, BotCount);
            BotCount++;
        }

        public override void Stop()
        {
            BotNumber.TryRemove(player.status.uuid, out openBotNumber);
            BotWork.TryRemove(player.status.uuid, out _);
            BotCount--;
        }

        public void OnTick()
        {
            BotWork.TryGetValue(player.status.uuid, out _radius);
            
#if (DEBUG)
            Console.WriteLine("Tick Start");
#endif

            if (inventory.FindId(int.Parse(_blockId)) < 1) return;

            if (_target == null)
            {
                _target = FindNext();
#if (DEBUG)
                Console.WriteLine("Target:" + _target);
#endif
            }

            if (_target == null) return;

            Broken.TryAdd(_target, DateTime.Now);

            var cancelToken = new CancelToken();
            _targetblock = actions.AsyncMoveToLocation(_target, cancelToken, Mo);
            
#if (DEBUG)
            Console.WriteLine(_targetblock.Target);
#endif

            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                Broken.TryRemove(_targetblock.Target, out _);
#if (DEBUG)
                Console.WriteLine("TaskCompleted();");
#endif
                TaskCompleted();
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                Broken.TryRemove(_targetblock.Target, out _);
                //InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                Broken.TryRemove(_targetblock.Target, out _);
                //InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
#if (DEBUG)
            Console.WriteLine("Tick End");
#endif
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && !_macro.IsMacroRunning() && !_fillermacro.IsMacroRunning() && (!inventory.IsFull() || !(inventory.FindId(int.Parse(_blockId)) < 1));
        }

        private ILocation FindNext()
        {
#if (DEBUG)
            Console.WriteLine("FindNext Start");
#endif
            //Get the area from the radius.
            if (_radius == null) return null;
#if (DEBUG)
            Console.WriteLine("_radius != null");
#endif

            //Search from top to bottom.
            //(As that is easier to manager)
            for (var y = (int) _radius.start.y; y <= (int) _radius.start.y + _radius.height; y++)
                for (var x = _radius.start.x; x <= _radius.start.x + _radius.xSize; x++)
                for (var z = _radius.start.z; z <= _radius.start.z + _radius.zSize; z++)
                {
                    var tempLocation = new Location(x, y, z);

                    if (ScanBlocks(tempLocation)) continue;

                    if (player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 0)
                        continue;

                    if (Broken.ContainsKey(tempLocation) && 
                        Broken[tempLocation].Subtract(DateTime.Now).TotalSeconds >= -5) continue;

                    if (Broken.ContainsKey(tempLocation) && 
                        Broken[tempLocation].Subtract(DateTime.Now).TotalSeconds <= -5)
                        Broken.TryRemove(tempLocation, out _);

                    // Check if this block is safe to mine.
                    if (!IsSafe(tempLocation)) continue;
                    
                    ILocation closest = tempLocation;
                    
                    var targetblockid = player.world.GetBlockId(closest);
#if (DEBUG)
                        Console.WriteLine("TargetBlockId: " + targetblockid);
                        Console.WriteLine(closest);
#endif
                    return closest;
                }

#if (DEBUG)
            Console.WriteLine("FindNext End");
#endif
            return null;
        }

        private void TaskCompleted()
        {
            _target = null;
            _busy = false;
        }

        private static void InvalidateBlock(ILocation location)
        {
            if (location != null) Broken.TryAdd(location, DateTime.Now);
        }

        private bool ScanBlocks(ILocation tempLocation)
        {
            var block = short.Parse(_blockId);
            return player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) == block;
        }

        private bool IsSafe(ILocation location)
        {
            if (player.world.IsStandingOn(location, player.status.entity.location))
            {
                if (!BlocksGlobal.blockHolder.IsSafeToMine(player.world, location, true))
                    return false;
            }
            else if (!BlocksGlobal.blockHolder.IsSafeToMine(player.world, location))
            {
                return false;
            }

            return true;
        }
    }
}