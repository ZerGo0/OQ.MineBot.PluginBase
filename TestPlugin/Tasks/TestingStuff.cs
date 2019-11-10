using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Entity.Mob;
using OQ.MineBot.PluginBase.Movement.Events;

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
            try
            {
                Testing();
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(Context.Player.GetUsername(), e, this);
            }
        }

        public override async Task Stop()
        {
            
        }

        public async Task OnTick()
        {
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Tick");
            
        }

        public override bool Exec()
        {
            return true;
        }

        private void Testing()
        {
            Testing2();
        }
        
        private void Testing2()
        {
            Testing3();
        }
        
        private void Testing3()
        {
            throw new Exception("Test");
        }
    }
}