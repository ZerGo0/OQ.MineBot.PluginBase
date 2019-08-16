using System;
using System.Collections.Generic;
using System.Diagnostics;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.Protocols.Classes.Base;

namespace PvPWandererPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();
        private static ILiving _sharedTarget;
        

        private static readonly MapOptions Mo = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.HIGHEST,
            Mine = false
        };
        
        private int _hitTicks;
        private bool _busy;
        private bool _movingBusy;

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

        public override void Start()
        {
        }

        public override void Stop()
        {
            _sharedTarget = null;
        }

        public override bool Exec() {
            return !status.entity.isDead && !status.eating &&!_busy;
        }

        public void OnTick()
        {
            if (_sharedTarget == null)
            {
                Console.WriteLine("Searching for target!");
                _sharedTarget = SearchTarget();
            }
            
            if (!_movingBusy)
            {
                if (_sharedTarget != null)
                {
                    var rndLocList = RandomLocationNearTarget(_sharedTarget.location.ToLocation());
                    if (rndLocList.Count < 1)
                    {
                        _sharedTarget = null;
                        _movingBusy = false;
                        return;
                    }
                
                    var rndLocationCount = new Random().Next(0, rndLocList.Count);
                
                    MoveToTarget(rndLocList[rndLocationCount], () => 
                    {
                        _movingBusy = false;
                    });
                }
            }
            
            Hit();
        }

        private ILiving SearchTarget()
        {
            var currentLoc = player.status.entity.location.ToLocation();

            return player.entities.FindClosestPlayer(currentLoc.x, currentLoc.y, currentLoc.z);;
        }

        private void MoveToTarget(ILocation targetLocation, Action callback)
        {
            var cancelToken = new CancelToken();

            _movingBusy = true;

            var targetMoveToLocation = actions.AsyncMoveToLocation(targetLocation, cancelToken, Mo);

            targetMoveToLocation.Completed += areaMap => { callback(); };
            targetMoveToLocation.Cancelled += (areaMap, cuboid) =>
            {
                _movingBusy = false;
                cancelToken.Stop();
            };

            if (!targetMoveToLocation.Start())
            {
                _movingBusy = false;
                cancelToken.Stop();
            }
            else
            {
                _movingBusy = true;
            }
        }

        private List<ILocation> RandomLocationNearTarget(ILocation targetLocation)
        {
            //Console.WriteLine($"Target Loc: {targetLocation}");
            var targetLocationList = new List<ILocation>();
            for (var y = -3; y < 3; y++)
            {
                for (var z = -3; z < 3; z++)
                {
                    for (var x = -3; x < 3; x++)
                    {
                        //Console.WriteLine($"Target Offset Loc: {targetLocation.Offset(x, y, z)}");
                        if (player.world.IsWalkable(targetLocation.Offset(x, y, z)) &&
                            targetLocation.Distance(targetLocation.Offset(x, y, z)) <= 4 &&
                            IsLocationsDifferent(targetLocation, targetLocation.Offset(x, y, z)) &&
                            player.world.IsVisible(targetLocation.Offset(x, y, z).ToPosition(), targetLocation))
                        {
                            targetLocationList.Add(targetLocation.Offset(x, y, z));
                        }
                    }
                }
            }

            foreach (var possibleLoc in targetLocationList)
            {
                Console.WriteLine($"Possible Target Offset Loc: {possibleLoc}");
            }

            return targetLocationList;
        }

        private bool IsLocationsDifferent(ILocation loc1, ILocation loc2)
        {
            return (loc1.x != loc2.x && loc1.y != loc2.y && loc1.z != loc2.z);
        }

        private void Hit()
        {
            var currentLoc = player.status.entity.location.ToLocation();
            
            if (_sharedTarget != null) {
                
                if (_mode == Mode.Passive)
                {
                    //Visibility Check
                    if (!player.world.IsVisible(currentLoc.ToPosition(), _sharedTarget.location.ToLocation(1))) return;

                    if (currentLoc.Distance(_sharedTarget.location.ToLocation()) > 4) return;
                }

                if(_autoWeapon) actions.EquipWeapon();
                actions.LookAt(_sharedTarget.location.Offset(new Position(0, 1.65, 0)), true);
                
                // 1 hit tick is about 50 ms.
                _hitTicks++;
                int ms = _hitTicks * 50;

                if (ms >= (1000 / _cps)) {
                    
                    _hitTicks = 0; //Hitting, reset tick counter.
                    
                    actions.EntityAttack(_sharedTarget.entityId);
                }
            }
        }
    }
}