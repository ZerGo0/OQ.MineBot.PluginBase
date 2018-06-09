using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Blocks;

namespace OreMinerPlugin.Tasks
{
    public class Mine : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.HIGHEST,
            Mine = true
        };

        private readonly ushort[] _ids;
        private readonly MacroSync _macro;

        private bool _busy;
        private ILocation _location;

        public Mine(bool diamondOre, bool emeraldOre, bool ironOre, bool goldOre, bool redstoneOre, bool lapisOre,
            bool coalOre, IEnumerable<string> customids, MacroSync macro)
        {
            _macro = macro;

            var list = new List<ushort>();
            if (diamondOre) list.Add(56);
            if (emeraldOre) list.Add(129);
            if (ironOre) list.Add(15);
            if (goldOre) list.Add(14);
            if (redstoneOre)
            {
                list.Add(73);
                list.Add(74);
            }

            if (lapisOre) list.Add(21);
            if (coalOre) list.Add(16);

            list.AddRange(from customid in customids
                where !string.IsNullOrWhiteSpace(customid)
                select Convert.ToUInt16(customid));

            _ids = list.ToArray();
        }

        public void OnTick()
        {
            if (_location == null)
            {
                FindClosestOre();
                return;
            }

            var cancelToken = new CancelToken();
            var map = actions.AsyncMoveToLocation(_location, cancelToken, Mo);
            map.Completed += areaMap => { TaskCompleted(); };
            map.Cancelled += (areaMap, cuboid) =>
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_location);
                TaskCompleted();
            };

            if (!map.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_location);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && !_macro.IsMacroRunning() && !inventory.IsFull();
        }

        private void FindClosestOre()
        {
            _busy = true;
            world.FindFirstAsync(player, player.status.entity.location.ToLocation(), 64, 256, _ids, IsSafe, loc =>
            {
                BeingMined.TryAdd(loc, null);
                _location = loc.Offset(-1);
                _busy = false;
            });
        }

        private bool IsSafe(ILocation location)
        {
            if (BeingMined.ContainsKey(location) || _personalBlocks.ContainsKey(location)) return false;
            return IsTunnelable(location);
        }

        private bool IsTunnelable(ILocation pos)
        {
            return !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(2))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(3))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1, 1, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1, 2, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 1, 1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 2, 1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(-1, 1, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(-1, 2, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 1, -1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 2, -1)))
                ;
        }

        private void TaskCompleted()
        {
            _location = null;
            _busy = false;
        }

        private void InvalidateBlock(ILocation location)
        {
            if (location != null) _personalBlocks.TryAdd(location, null);
        }

        #region Shared work.

        /// <summary>
        ///     Blocks that are taken already.
        /// </summary>
        public static readonly ConcurrentDictionary<ILocation, object>
            BeingMined = new ConcurrentDictionary<ILocation, object>();

        private readonly ConcurrentDictionary<ILocation, object> _personalBlocks =
            new ConcurrentDictionary<ILocation, object>();

        #endregion
    }
}