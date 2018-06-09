using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.Protocols.Classes.Base;

namespace AreaMiner.Tasks
{
    public class Mine : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.MEDIUM,
            Mine = true
        };

        #region Shared work.

        /// <summary>
        ///     Blocks that are having a hard time being mined.
        /// </summary>
        private static readonly ConcurrentDictionary<ILocation, DateTime> Broken =
            new ConcurrentDictionary<ILocation, DateTime>();

        #endregion

        private readonly ushort[] _ignore, _block;
        private readonly MacroSync _macro;
        private readonly Mode _mode;
        private readonly PathMode _pathMode;

        private readonly ShareManager _shareManager;

        private bool _busy;

        public Mine(ShareManager shareManager, Mode mode, PathMode pathMode, ushort[] ignore, ushort[] block,
            MacroSync macro)
        {
            _shareManager = shareManager;
            _mode = mode;
            _pathMode = pathMode;
            _ignore = ignore;
            _block = block;
            _macro = macro;

            Mo.Mine = pathMode == PathMode.Advanced;
        }

        public void OnTick()
        {
            var target = FindNext();
            if (target == null) return;

            _busy = true;

            ThreadPool.QueueUserWorkItem(state =>
            {
                var success = actions.WaitMoveToRange(target, token, Mo);
                if (!success)
                {
                    Broken.TryAdd(target, DateTime.Now);
                    _busy = false;
                }
                else
                {
                    if (!IsSafe(target))
                    {
                        Broken.TryAdd(target, DateTime.Now);
                        _busy = false;
                    }
                    else
                    {
                        actions.SelectBestTool(target);
                        actions.LookAtBlock(target, true);
                        player.tickManager.Register(2, () =>
                        {
                            if (_pathMode == PathMode.Advanced) Broken.TryAdd(target, DateTime.Now);
                            actions.BlockDig(target, action => { _busy = false; });
                        });
                    }
                }
            });
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && _shareManager.AllReached() &&
                   !_macro.IsMacroRunning();
        }

        private ILocation FindNext()
        {
            //Get the area from the radius.
            var playerRadius = _shareManager.Get(player);
            if (playerRadius == null) return null;

            //Search from top to bottom.
            //(As that is easier to manager)
            ILocation closest = null;
            double distance = int.MaxValue;
            for (var y = (int) playerRadius.start.y + playerRadius.height; y >= (int) playerRadius.start.y; y--)
                if (closest == null)
                    for (var x = playerRadius.start.x; x <= playerRadius.start.x + playerRadius.xSize; x++)
                    for (var z = playerRadius.start.z; z <= playerRadius.start.z + playerRadius.zSize; z++)
                    {
                        var tempLocation = new Location(x, y, z);
                        //Check if the block is valid for mining.
                        if (player.world.GetBlockId(x, y, z) == 0) continue;

                        if (Broken.ContainsKey(tempLocation) &&
                            Broken[tempLocation].Subtract(DateTime.Now).TotalSeconds < -15)
                            continue;

                        if (_ignore?.Contains(player.world.GetBlockId(x, y, z)) == true)
                            continue;

                        if (_block.Length > 0)
                            if (!_block?.Contains(player.world.GetBlockId(x, y, z)) == true)
                                continue;

                        // Check if this block is safe to mine.
                        if (!IsSafe(tempLocation)) continue;

                        if (closest == null)
                        {
                            distance = tempLocation.Distance(player.status.entity.location.ToLocation());
                            closest = new Location(x, y, z);
                        }
                        else if (tempLocation.Distance(player.status.entity.location.ToLocation()) < distance)
                        {
                            distance = tempLocation.Distance(player.status.entity.location.ToLocation());
                            closest = tempLocation;
                        }
                    }

            return closest;
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

    public enum Mode
    {
        Accuare,
        Fast
    }

    public enum PathMode
    {
        Advanced,
        Basic
    }
}