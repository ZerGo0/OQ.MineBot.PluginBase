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
        }

        public override async Task Stop()
        {
            
        }

        public async Task OnTick()
        {
            try
            {
                var helperFunctions = new HelperFunctions(Context, Inventory);
                var closestGate = Context.World.FindClosest(50, 50, 107).Result;
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