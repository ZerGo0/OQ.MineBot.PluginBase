using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace AutoSplashPotion.Tasks
{
    public class Equipment : ITask, IHealthListener
    {
        private readonly int _healthThreshold;
        private readonly bool _instantHealth;
        private readonly bool _regeneration;
        private bool _stopped;

        public Equipment(int healthThreshold, bool instantHealth, bool regeneration) {
            _healthThreshold = healthThreshold;
            _instantHealth = instantHealth;
            _regeneration = regeneration;
        }
        
        public override bool Exec() {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_stopped;
        }

        public override async Task Start()
        {
            try
            {
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Test");
                await OnHealthChanged();
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
                _stopped = true;
            }
        }
        
        public async Task OnHealthChanged()
        {
            try
            {
                if (Context.Player.GetHealth() >= _healthThreshold) return;
                
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "< threshold");

                var potList = new Dictionary<short, ushort>();
                
                if (_instantHealth && !_regeneration)
                {
                    potList.Add(16421, 373);
                    potList.Add(16453, 373);
                }
                else if (_regeneration && !_instantHealth)
                {
                    potList.Add(16449, 373);
                    potList.Add(16417, 373);
                    potList.Add(16385, 373);
                }
                else
                {
                    potList.Add(16421, 373);
                    potList.Add(16449, 373);
                    potList.Add(16417, 373);
                    potList.Add(16453, 373);
                    potList.Add(16385, 373);
                }

                foreach (var pot in potList)
                {
                    if (await ThrowPotion(pot.Value, pot.Key)) return;
                }
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
                _stopped = true;
            }
        }

        private async Task<bool> ThrowPotion(ushort id, short meta)
        {
            if (Inventory.FindFirst(id, meta) == null) return false;
            
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Found Item!");
            
            await Inventory.Select(id, new[] {meta});
            await Context.Player.LookAt(Context.Player.GetLocation().Offset(-1));
            await Context.Player.UseHeld();
            await Context.TickManager.Sleep(2);

            return true;
        }
    }
}