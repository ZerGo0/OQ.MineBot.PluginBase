using System;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;

namespace MobAuraPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();

        private int hitTicks;

        private readonly Mode mode;
        private readonly int  cps;
        private readonly int  ms;
        private readonly bool autoWeapon;

        private bool currentlyPathing;

        public Attack(Mode mode, int cps, int ms, bool autoWeapon) {
            this.mode = mode;
            this.cps        = cps;
            this.ms         = ms;
            this.autoWeapon = autoWeapon;
        }

        public override bool Exec() {
            return !status.entity.isDead && !status.eating;
        }

        public void OnTick() {
            var currentLoc = player.status.entity.location.ToLocation();

            var closestMob = player.entities.FindClosestMob(currentLoc.x, currentLoc.y, currentLoc.z);
            if (closestMob != null) {

                if (mode == Mode.MovingPassive || mode == Mode.MovingAggresive)
                {
                    if (!currentlyPathing) GoToLocation(closestMob.location.ToLocation().Offset(-1));
                    
                    if (mode == Mode.MovingPassive)
                    {
                        //Visibility Check
                        if (!player.world.IsVisible(currentLoc.ToPosition(), closestMob.location.ToLocation().Offset(1))) return;

                        if (currentLoc.Distance(closestMob.location.ToLocation()) > 4) return;
                    }

                    if(autoWeapon) actions.EquipWeapon();
                    actions.LookAt(closestMob.location, true);
                
                    // 1 hit tick is about 50 ms.
                    hitTicks++;
                    int ms = hitTicks * 50;

                    if (ms >= (1000 / cps)) {
                    
                        hitTicks = 0; //Hitting, reset tick counter.
                        if (Rnd.Next(1, 101) < this.ms) actions.PerformSwing(); //Miss.
                        else actions.EntityAttack(closestMob.entityId); //Hit.
                    }
                }
                else
                {
                    if (mode == Mode.Passive)
                    {
                        //Visibility Check
                        if (!player.world.IsVisible(currentLoc.ToPosition(), closestMob.location.ToLocation().Offset(1))) return;

                        if (currentLoc.Distance(closestMob.location.ToLocation()) > 4) return;
                    }

                    if(autoWeapon) actions.EquipWeapon();
                    actions.LookAt(closestMob.location, true);
                
                    // 1 hit tick is about 50 ms.
                    hitTicks++;
                    int ms = hitTicks * 50;

                    if (ms >= (1000 / cps)) {
                    
                        hitTicks = 0; //Hitting, reset tick counter.
                        if (Rnd.Next(1, 101) < this.ms) actions.PerformSwing(); //Miss.
                        else actions.EntityAttack(closestMob.entityId); //Hit.
                    }
                }
            }
        }
        
        private void GoToLocation(ILocation targetLocation)
        {
            var cancelToken = new CancelToken();

            currentlyPathing = true;

            var targetMoveToLocation = actions.AsyncMoveToLocation(targetLocation, cancelToken, new MapOptions{
                Look = true,
                Quality = SearchQuality.MEDIUM,
                Mine = false
            });

            targetMoveToLocation.Completed += areaMap => { currentlyPathing = false; };
            targetMoveToLocation.Cancelled += (areaMap, cuboid) =>
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                currentlyPathing = false;
            };

            if (!targetMoveToLocation.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                currentlyPathing = false;
            }
            else
            {
                currentlyPathing = true;
            }
        }
    }
}