using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;

namespace AreaMiner.Tasks
{
    public class Path : ITask, ITickListener
    {
        private static readonly MapOptions
            Zmo = new MapOptions {Look = true, Quality = SearchQuality.HIGH, Mine = true};

        private readonly MacroSync _macro;
        private readonly PathMode _pathMode;

        private readonly IRadius _radius;
        private readonly ShareManager _shareManager;

        private bool _busy;

        public Path(ShareManager shareManager, ILocation start, ILocation end, PathMode pathMode, MacroSync macro)
        {
            _shareManager = shareManager;
            _pathMode = pathMode;
            _macro = macro;

            Zmo.Mine = pathMode == PathMode.Advanced;
            _radius = new IRadius(start, end);
            _shareManager.SetArea(_radius);
        }

        public void OnTick()
        {
            var zone = _shareManager.Get(player);
            var center = zone?.GetClosestWalkable(player.world, player.status.entity.location.ToLocation(), true);
            if (center == null) return;

            var map = player.functions.AsyncMoveToLocation(center, token, Zmo);
            map.Completed += areaMap =>
            {
                _shareManager.RegisterReached(player);
                _busy = false;
            };
            map.Cancelled += (areaMap, cuboid) => { _busy = false; };
            map.Start();
            _busy = true;
        }

        public override void Start()
        {
            _shareManager.Add(player, _radius);
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && !_shareManager.MeReached(player) &&
                   !_macro.IsMacroRunning();
        }
    }
}