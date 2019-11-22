using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Classes.Entity.Mob;
using OQ.MineBot.PluginBase.Movement.Events;
using OQ.MineBot.Protocols.Classes.Base;

using TestPlugin.Helpers;

namespace TestPlugin.Tasks
{
    public class TestingStuff : ITask, ITickListener
    {
        private IMobEntity _currentTarget;
        private bool _currentlyPathing;
        private IMoveTask _moveToMob;
        private string _botName;

        public TestingStuff()
        {
        }

        public override async Task Start()
        {
            _botName = Context.Player.GetUsername();
            BlocksGlobal.BUILDING_BLOCKS = new[] {(ushort) 12};
        }

        public override async Task Stop()
        {
            
        }

        public async Task OnTick()
        {
            try
            {
                var helperFunctions = new HelperFunctions(Context, Inventory);
                await helperFunctions.GoToLocation(Context.Player.GetLocation().Offset(1), HelperFunctions.MAP_OPTIONS_BUILD);

                var currentLoc = Context.Player.GetLocation();
                await Context.Player.LookAtSmooth(new Location(currentLoc.X, currentLoc.Y + 2, currentLoc.Z + 1));
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(Context.Player.GetUsername(), e, this);
            }
        }

        public override bool Exec()
        {
            return true;
        }
    }
}