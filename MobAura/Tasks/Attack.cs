using System;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using ShieldPlugin;

namespace MobAuraPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();

        private int _hitTicks;

        private readonly Mode _mode;
        private readonly int  _cps;
        private readonly int  _ms;
        private readonly bool _autoWeapon;

        public Attack(Mode mode, int cps, int ms, bool autoWeapon) {
            _mode = mode;
            _cps        = cps;
            _ms         = ms;
            _autoWeapon = autoWeapon;
        }

        public override bool Exec() {
            return !status.entity.isDead && !status.eating;
        }

        public void OnTick() {
            Hit();
        }

        private void Hit()
        {
            var currentLoc = player.status.entity.location.ToLocation();

            var closestMob = player.entities.FindClosestMob(currentLoc.x, currentLoc.y, currentLoc.z);
            if (closestMob != null) {
                
                if (_mode == Mode.Passive)
                {
                    //Visibility Check
                    if (!player.world.IsVisible(currentLoc.ToPosition(), closestMob.location.ToLocation().Offset(1))) return;

                    if (currentLoc.Distance(closestMob.location.ToLocation()) > 4) return;
                }

                if(_autoWeapon) actions.EquipWeapon();
                actions.LookAt(closestMob.location, true);
                
                // 1 hit tick is about 50 ms.
                _hitTicks++;
                int ms = _hitTicks * 50;

                if (ms >= (1000 / _cps)) {
                    
                    _hitTicks = 0; //Hitting, reset tick counter.
                    if (Rnd.Next(1, 101) < _ms) actions.PerformSwing(); //Miss.
                    else actions.EntityAttack(closestMob.entityId); //Hit.
                }
            }
        }
    }
}