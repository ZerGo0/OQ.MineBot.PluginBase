using System;
using System.Collections.Concurrent;
using System.Linq;
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

        #endregion

        private readonly MacroSync _macro;
        private readonly MacroSync _fillermacro;
        private readonly IRadius _radius;

        private bool _busy;
        private readonly string _blockId;
        private ILocation _target;
        private IAsyncMap _targetblock;

        public FillerArea(string blockId, ILocation startLocation, ILocation endLocation, MacroSync macro, MacroSync fillerMacro)
        {
            _blockId = blockId;
            _macro = macro;
            _fillermacro = fillerMacro;

            var split = new[]
                {_blockId};
            var blocks = split.Select(ushort.Parse).ToArray();
            BlocksGlobal.BUILDING_BLOCKS = blocks;

            _radius = new IRadius(startLocation, endLocation);
        }

        public void OnTick()
        {
#if (DEBUG)
            Console.WriteLine("Tick Start");
#endif

            if (inventory.FindId(int.Parse(_blockId)) < 1)
                return;

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