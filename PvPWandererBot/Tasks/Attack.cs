using System;
using System.Collections.Generic;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
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
        private bool _wanderingBusy;
        private IPosition _prevPos;

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
            player.events.onPlayerUpdate += EventsOnOnPlayerUpdate;
        }

        private void EventsOnOnPlayerUpdate(IStopToken cancel)
        {
            if (_sharedTarget == null)
            {
                Console.WriteLine("Searching for target!");
                _sharedTarget = SearchTarget();
            }
            
            Hit(_sharedTarget);
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
                else
                {
                    /*if (!_wanderingBusy)
                    {
                        var limit = 50000;
                        var xRand = new Random().Next(limit / 2, limit);
                        var zRand = new Random().Next(limit / 2, limit);
                    
                        MoveToTarget(new Location(xRand, player.status.entity.location.ToLocation().y, zRand), _wanderingBusy,
                            () =>
                        {
                        
                        });
                    }*/
                }
            }
            
            //Hit();
        }

        private ILiving SearchTarget()
        {
            var currentLoc = player.status.entity.location.ToLocation();

            return player.entities.FindClosestPlayer(currentLoc.x, currentLoc.y, currentLoc.z);
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
            var targetLocationList = new List<ILocation>();
            for (var y = -3; y < 3; y++)
            {
                for (var z = -3; z < 3; z++)
                {
                    for (var x = -3; x < 3; x++)
                    {
                        var tempLocation = targetLocation.Offset(x, y, z);
                        if (player.world.IsWalkable(tempLocation) &&
                            targetLocation.Distance(tempLocation) < 4 &&
                            IsLocationsDifferent(targetLocation, tempLocation) &&
                            player.world.IsVisible(tempLocation.ToPosition(), targetLocation))
                        {
                            targetLocationList.Add(tempLocation);
                        }
                    }
                }
            }

            /*foreach (var possibleLoc in targetLocationList)
            {
                Console.WriteLine($"Possible Target Offset Loc: {possibleLoc}");
            }*/

            return targetLocationList;
        }

        private bool IsLocationsDifferent(ILocation loc1, ILocation loc2)
        {
            return (loc1.x != loc2.x && Math.Abs(loc1.y - loc2.y) > 0 && loc1.z != loc2.z);
        }
        
        private bool IsPostionsDifferent(IPosition pos1, IPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) > 0 && Math.Abs(pos1.Y - pos2.Y) > 0 && Math.Abs(pos1.Z - pos2.Z) > 0;
        }
        
        private bool HasValue(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private IPosition CalculateVelocity(IPosition currentPos, IPosition prevPos)
        {
            if (prevPos == null)
            {
                //Console.WriteLine("Test");
                _prevPos = new Position(currentPos.X, currentPos.Y, currentPos.Z);
                return null;
            }
            //.WriteLine($"Curr {currentPos} | Prev {prevPos}");
            
            var posDiffX = currentPos.X - prevPos.X;
            var posDiffY = currentPos.Y - prevPos.Y;
            var posDiffZ = currentPos.Z - prevPos.Z;
            //Console.WriteLine($"DiffX: {posDiffX} | DiffY: {posDiffY} | DiffZ: {posDiffZ}");
            
            var magicValue = Math.Sqrt(posDiffX * posDiffX + posDiffY * posDiffY + posDiffZ * posDiffZ);

            var veloX = posDiffX / magicValue;
            var veloY = posDiffY / magicValue;
            var veloZ = posDiffZ / magicValue;
            
            if (HasValue(veloX) || HasValue(veloY) || HasValue(veloZ))
                Console.WriteLine($"VelX: {veloX} | VelY: {veloY} | VelZ: {veloZ} | test: {magicValue}");
                
            _prevPos = new Position(currentPos.X, currentPos.Y, currentPos.Z);
            
            return new Position(veloX, veloY, veloZ);
        }

        private void Hit(IEntity target)
        {
            var currentLoc = player.status.entity.location.ToLocation();

            if (target == null) return;
            
            actions.LookAt(target.location.Offset(new Position(0, .65, 0)), true);

            if (_mode == Mode.Passive)
            {
                //Visibility Check
                if (!player.world.IsVisible(currentLoc.ToPosition(), target.location.ToLocation(1))) return;

                if (currentLoc.Distance(target.location.ToLocation()) > 4) return;
            }
                
            if(_autoWeapon) actions.EquipWeapon();
            // 1 hit tick is about 50 ms.
            _hitTicks++;
            int ms = _hitTicks * 50;

            if (ms >= (1000 / _cps)) {
                    
                _hitTicks = 0; //Hitting, reset tick counter.
                if (Rnd.Next(1, 101) < _ms) actions.PerformSwing(); //Miss.
                else actions.EntityAttack(target.entityId); //Hit.
            }
        }
    }
}