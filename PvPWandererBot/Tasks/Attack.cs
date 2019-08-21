using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace PvPWandererPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();
        private static ILiving _sharedTarget;
        

        private static readonly MapOptions Mo = new MapOptions
        {
            AntiStuck = true,
            Look = false,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Strict = true
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
        }

        public override bool Exec() {
            return !status.entity.isDead && !status.eating &&!_busy;
        }

        public void OnTick()
        {
            if (_sharedTarget == null)
            {
                //Console.WriteLine("Searching for target!");
                var test = SearchClosestTarget(player.status.entity.location.ToLocation(), 3);

                var keyValuePairs = test.ToList();
                if (keyValuePairs.Any()) _sharedTarget = keyValuePairs.First().Value;
            }
            
            if (!_movingBusy)
            {
                if (_sharedTarget != null)
                {
                    var rndLocList = RandomLocationNearTarget(_sharedTarget.location.ToLocation());
                    if (rndLocList.Count < 1)
                    {
                        _sharedTarget = null;
                        //MoveToTarget(_sharedTarget.location.ToLocation(), (b, map) => { _movingBusy = false; });
                        return;
                    }
                
                    var rndLocationCount = new Random().Next(0, rndLocList.Count);
                
                    MoveToTarget(rndLocList[rndLocationCount], (b, map) => { _movingBusy = false; });
                }
                else
                {
                    //No target found, wandering stuff here
                    var wanderLimit = 25; //4096*4
                    var rnd = new Random();
                    var rndX = rnd.Next(-wanderLimit, wanderLimit);
                    var rndY = rnd.Next(-wanderLimit, wanderLimit);

                    var currentloc = player.status.entity.location.ToLocation();
                    
                    Console.WriteLine($"{rndX} | {rndY}");

                    MoveToTarget(new Location(currentloc.x + rndX, currentloc.y - 1, currentloc.z + rndY),
                        (b, map) => { _movingBusy = false; });
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

        private IEnumerable<KeyValuePair<double, ILiving>> SearchClosestTarget(ILocation currentLoc, int areaAroundTarget)
        {
            var targetDic = new Dictionary<double, ILiving>();
            foreach (var target in player.entities.playerList)
            {
                var targetLocation = target.Value.location.ToLocation();
                var distanceToEnemy = player.status.entity.location.Distance(target.Value.location);

                var possibleToPath = false;
                for (var y = -areaAroundTarget; y < areaAroundTarget; y++)
                {
                    for (var z = -areaAroundTarget; z < areaAroundTarget; z++)
                    {
                        for (var x = -areaAroundTarget; x < areaAroundTarget; x++)
                        {
                            //Console.WriteLine($"Target Offset Loc: {targetLocation.Offset(x, y, z)}");
                            if (!player.world.IsWalkable(targetLocation.Offset(x, y, z)) ||
                                !(targetLocation.Distance(targetLocation.Offset(x, y, z)) <= 4) ||
                                !player.world.IsVisible(targetLocation.Offset(x, y, z).ToPosition(), targetLocation))
                                continue;
                            
                            possibleToPath = true;
                            break;
                        }
                    }
                }
                
                if (possibleToPath) targetDic.Add(distanceToEnemy, target.Value);
            }

            //var closestTarget = targetDic.Aggregate((l, r) => l.Key < r.Key ? l : r);

            return targetDic.OrderBy(key => key.Key);
        }

        private void MoveToTarget(ILocation targetLocation, Action<bool, IAreaMap> callback)
        {
            var map = actions.AsyncMoveToLocation(targetLocation, token, Mo);
            _movingBusy = map.Start();
            map.Completed += areaMap => {
                callback(true, areaMap);
            };
            map.Cancelled += (areaMap, cuboid) => { _movingBusy = false; };
            if (!map.Valid) /*actions.LookAtBlock(location)*/;
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

                if(_autoWeapon) actions.EquipWeapon();
                actions.LookAt(_sharedTarget.location.Offset(new Position(0, .65, 0)), true);
                
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