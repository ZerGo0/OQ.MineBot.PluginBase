using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace PvPWandererPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();
        private static IPlayerEntity _sharedTarget;
        private static List<ILocation> _sharedTargetLocations = new List<ILocation>();
        
        private static readonly MapOptions WanderingMapOptions = new MapOptions
        {
            Swim = true,
            SafeMove = true,
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.HIGH,
            Mine = false
        };

        private static readonly MapOptions TargetingMapOptions = new MapOptions
        {
            Swim = true,
            SafeMove = true,
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Segment = true
        };

        private readonly bool _autoWeapon;
        private readonly int _cps;

        private readonly ConcurrentDictionary<ILocation, DateTime> _invalidLocations =
            new ConcurrentDictionary<ILocation, DateTime>();

        private readonly Mode _mode;
        private readonly int _ms;
        private bool _busy;

        private int _hitTicks;
        private IPlayerEntity _localTarget;
        private bool _movingBusy;
        private List<ILocation> _rndWanderingLocations = new List<ILocation>();
        private bool _scanningRndLocations;
        private IStopToken _stopToken = new CancelToken();
        private bool _wanderingBusy;
        private readonly Stopwatch _sharedInvalidTimer = new Stopwatch();

        public Attack(Mode mode, int cps, int ms, bool autoWeapon)
        {
            _mode = mode;
            _cps = cps;
            _ms = ms;
            _autoWeapon = autoWeapon;
        }
        
        public override void Start()
        {
            _sharedTarget = null;
            player.events.onPlayerUpdate += EventsOnOnPlayerUpdate;

            CleanInvalidLocations();
            _sharedInvalidTimer.Start();
        }

        public override void Stop()
        {
            //Not really needed since the plugin gets unloaded anyway,
            //but yea let's pretend that I'm a good dev :laughf:
            player.events.onPlayerUpdate -= EventsOnOnPlayerUpdate;

            if (_stopToken.stopped) return;
            _stopToken?.Stop();
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy;
        }
        
        //This runs before OnTick()
        private void EventsOnOnPlayerUpdate(IStopToken cancel)
        {
            if (_sharedInvalidTimer.ElapsedMilliseconds >= 1000)
            {
                CleanInvalidLocations();
                _sharedInvalidTimer.Restart();
            }

            if (player.entities.IsBot(_sharedTarget?.uuid) || player.entities.IsBot(_localTarget?.uuid))
            {
                _localTarget = null;
                _sharedTarget = null;
            }

            if (_localTarget != null)
            {
                if (player.status.entity.location.Distance(_localTarget.location) >= 4) return;

                Hit(_localTarget);
            }
        }

        private IAsyncMap _test;
        private ILiving _testTarget;
        private IPosition _testTargetSavedLoc;
        private IPosition _savedBotPos;

        public void OnTick()
        {
            if (_test == null)
            {
                var currentLoc = player.status.entity.location.ToLocation();
                _testTarget = player.entities.FindClosestPlayer(currentLoc.x, currentLoc.y, currentLoc.z);

                if (_testTarget != null)
                {
                    Console.WriteLine("_test == null | _testTarget != null");
                    _testTargetSavedLoc = _testTarget.location;
                    _savedBotPos = player.status.entity.location;
                    _test = TestMovement(_testTarget, TargetingMapOptions, b => { },
                        () => { _test = null; });
                }
            }
            else
            {
                /*if (_testTarget != null && 
                    (_testTarget.location.Distance(_test.Target.ToPosition()) > 1))
                {
                    _test.Token.Stop();
                    _test = TestMovement(_testTarget, TargetingMapOptions, b => { },
                        () => { _test = null; });
                    Console.WriteLine($"Updated Path");

                    _testTargetSavedLoc = _testTarget.location;
                    _savedBotPos = player.status.entity.location;
                }*/
            }


            //if (_localTarget == null && _sharedTarget != null) _localTarget = _sharedTarget;
            
            /*var rndLocList = new List<ILocation>();

            #region Target Selector and Validator
            
            if (_sharedTarget == null || _localTarget == null)
            {
                //We already have a _sharedTarget, copying it to _localTarget and checking reachable block nearby
                if (_sharedTarget != null && _sharedTarget != _localTarget)
                {
                    _localTarget = _sharedTarget;

                    //Console.WriteLine($"{player.status.username}: Test");
                    
                    if (_localTarget != null)
                    {
                        //We got a new target, let's check if it it has any possible blocks around it
                        rndLocList = _sharedTargetLocations;
                        
                        //If the target is out of render distance then this will be 0...
                        //target has no possible blocks around it, resseting everything
                        if (rndLocList?.Count < 1)
                        {
                            _localTarget = null;
                            //_sharedTarget = null;
                            _movingBusy = false;
                            _wanderingBusy = false;
                            _scanningRndLocations = false;
                            
                            _stopToken?.Stop();
                            
                            rndLocList = null;
                            //TODO: this is probably causing issues, needs to be tested
                            //_rndWanderingLocations = null;
                        }
                    }
                }
                else
                {
                    //Array of all possible enemy in render distance
                    var closestPlayerkeyValuePairs = SearchClosestTarget(player.status.entity.location, 3);

                    var closestPlayerList = closestPlayerkeyValuePairs?.ToList();

                    //Just choosing the first player in render distance right now
                    if (closestPlayerList?.First().Value != null && 
                        closestPlayerList.First().Value != _sharedTarget)
                    {
                        //Using the first possible enemy for now
                        if (closestPlayerList.Any()) _sharedTarget = closestPlayerList.First().Value;

                        if (_sharedTarget != null)
                        {
                            //We got a new target, let's check if it it has any possible blocks around it
                            _sharedTargetLocations = RandomLocationNearTarget(_sharedTarget.location.ToLocation());
                            
                            //target has no possible blocks around it, resseting everything
                            if (_sharedTargetLocations.Count < 1)
                            {
                                _localTarget = null;
                                //_sharedTarget = null;
                                _movingBusy = false;
                                _wanderingBusy = false;
                                _scanningRndLocations = false;
                            
                                _stopToken?.Stop();
                            
                                _sharedTargetLocations = new List<ILocation>();
                            }
                            else
                            {
                                _localTarget = _sharedTarget;
                                rndLocList = _sharedTargetLocations;
                            }
                        }
                    }
                }
            }

            #endregion

            //The target is either not reachable or out of renderdistance
            if (_localTarget != null)
            {
                //if (player.entities.FindNameByUuid(_localTarget.uuid) != null) Console.WriteLine($"{player.status.username}: target {player.entities.FindNameByUuid(_localTarget.uuid).Name}");

                //We got a new target, let's check if it it has any possible blocks around it
                if (rndLocList != null && rndLocList.Count < 1 && _sharedTargetLocations.Count < 1) 
                    rndLocList = RandomLocationNearTarget(_localTarget.location.ToLocation());

                if (_sharedTargetLocations.Count > 1) rndLocList = _sharedTargetLocations;
                
                if (rndLocList?.Count < 1)
                {
                    _localTarget = null;
                    //_sharedTarget = null;
                    _movingBusy = false;
                    _wanderingBusy = false;
                    _scanningRndLocations = false;
                    
                    _stopToken?.Stop();
                    
                    rndLocList = null;
                    //TODO: this is probably causing issues, needs to be tested
                    //_rndWanderingLocations = null;
                }

                if (rndLocList?.Count > 1 && !_movingBusy)
                {
                    //Pick a random Loc around the target
                    var rndLocationCount = new Random().Next(0, rndLocList.Count);

                    if (!_invalidLocations.ContainsKey(rndLocList[rndLocationCount]))
                    {
                        if (_wanderingBusy || _scanningRndLocations || _rndWanderingLocations?.Count > 0)
                        {
                            _stopToken?.Stop();
                            _wanderingBusy = false;
                            _scanningRndLocations = false;
                            
                            //Reset _rndWanderingLocations since we trying to path to the target
                            //maybe not a good idea
                            _rndWanderingLocations = new List<ILocation>();
                        }

                        _movingBusy = true;
                        
                        //Move to the target
                        MoveToTarget(rndLocList[rndLocationCount], TargetingMapOptions,
                            busyState => _movingBusy = busyState, () =>
                            {
                                {
                                    _movingBusy = false;
                                }
                            });

                        return;
                    }
                }
                
                if (_movingBusy) return;
            }

            if (!_wanderingBusy && rndLocList?.Count < 1 && !_movingBusy)
            {
                if (_rndWanderingLocations == null || _rndWanderingLocations.Count < 1 && !_scanningRndLocations)
                {
                    _scanningRndLocations = true;
                    var currentLoc = player.status.entity.location.ToLocation();
                    var rnd = new Random();
                    var rndArea = rnd.Next(25, 50);
                    var rndHeight = rnd.Next(5, 10);

                    /*var timer = new Stopwatch();
                    timer.Start();#1#
                    FindRandomWanderingLocation(currentLoc, rndArea, rndHeight, locations =>
                    {
                        /*timer.Stop();
                        Console.WriteLine(
                            $"{player.status.username}: {timer.ElapsedMilliseconds}ms | {locations.Count} locs");#1#
                        if (locations.Count < 1)
                        {
                            _wanderingBusy = false;
                            _scanningRndLocations = false;
                            _rndWanderingLocations = new List<ILocation>();
                            return;
                        }

                        _rndWanderingLocations = locations;
                        _scanningRndLocations = false;
                    });
                }

                if (_rndWanderingLocations != null) 
                {
                    if (_rndWanderingLocations != null && (_rndWanderingLocations.Count > 0 && !_scanningRndLocations))
                    {
                        var rndLocInt = new Random().Next(0, _rndWanderingLocations.Count);
                        var rndLoc = _rndWanderingLocations[rndLocInt];

                        if (!player.world.IsWalkable(rndLoc) || _invalidLocations.ContainsKey(rndLoc))
                        {
                            _wanderingBusy = false;
                            if (_invalidLocations.ContainsKey(rndLoc) && !_rndWanderingLocations.Contains(rndLoc))
                                return;

                            _rndWanderingLocations.Remove(rndLoc);
                            InvalidateLocation(rndLoc);
                            return;
                        }

                        _wanderingBusy = true;
                        _rndWanderingLocations.Remove(rndLoc);

                        MoveToTarget(rndLoc, WanderingMapOptions, busyState => _wanderingBusy = busyState,
                            () => { _wanderingBusy = false; });
                    }
                }
            }*/
        }

        private void FindRandomWanderingLocation(ILocation currentLoc, int scanAreaSize, int scanAreaHeight,
            Action<List<ILocation>> callback)
        {
            var locations = new List<ILocation>();
            const int maxMs = 50;

            var currentHeight = (int) currentLoc.y - 1;
            var heightOffset = 0;
            var stopwatch = Stopwatch.StartNew();

            _stopToken = null;
            _stopToken = player.tickManager.RegisterReocurring(1, () =>
            {
                stopwatch.Restart();
                while (heightOffset < scanAreaHeight)
                {
                    var yPlus = currentHeight + heightOffset;
                    if (yPlus <= 256)
                        for (var x = -scanAreaSize; x < scanAreaSize; x++)
                        for (var z = -scanAreaSize; z < scanAreaSize; z++)
                        {
                            var yPlusTempLoc = new Location(currentLoc.x + x, yPlus, currentLoc.z + z);
                            if (world.IsWalkable(yPlusTempLoc) && !_invalidLocations.ContainsKey(yPlusTempLoc))
                                locations.Add(yPlusTempLoc);
                        }

                    var yMinus = currentHeight - heightOffset;
                    if (yMinus > 0 && heightOffset != 0)
                        for (var x = -scanAreaSize; x < scanAreaSize; x++)
                        for (var z = -scanAreaSize; z < scanAreaSize; z++)
                        {
                            var yMinusTempLoc = new Location(currentLoc.x + x, yMinus, currentLoc.z + z);
                            if (world.IsWalkable(yMinusTempLoc) && !_invalidLocations.ContainsKey(yMinusTempLoc))
                                locations.Add(yMinusTempLoc);
                        }

                    //Console.WriteLine($"Curr Height: {currentHeight} | yPlus: {yPlus} | yMinus: {yMinus} | HeightOff: {heightOffset}");

                    heightOffset++;

                    if (stopwatch.ElapsedMilliseconds >= maxMs) break;
                }

                if (heightOffset < scanAreaHeight) return;
                
                _stopToken?.Stop();
                callback(locations);
            });
        }

        private IEnumerable<KeyValuePair<double, IPlayerEntity>> SearchClosestTarget(IPosition currentPos,
            int areaAroundTarget)
        {
            var targetDic = new Dictionary<double, IPlayerEntity>();
            foreach (var target in player.entities.playerList)
            {
                if (target.Value == null) continue;

                var tempTarget = new KeyValuePair<int, IPlayerEntity>(target.Key, (IPlayerEntity) target.Value);
                if (player.entities.IsBot(tempTarget.Value.uuid) || tempTarget.Value == null) continue;

                var targetLocation = tempTarget.Value.location.ToLocation();

                var possibleToPath = false;
                for (var y = -areaAroundTarget; y < areaAroundTarget; y++)
                for (var z = -areaAroundTarget; z < areaAroundTarget; z++)
                for (var x = -areaAroundTarget; x < areaAroundTarget; x++)
                {
                    if (!player.world.IsWalkable(targetLocation.Offset(x, y, z)) ||
                        !(targetLocation.Distance(targetLocation.Offset(x, y, z)) <= areaAroundTarget) ||
                        !player.world.IsVisible(tempTarget.Value.location.Offset(new Position(x, y, z)),
                            targetLocation) ||
                        _invalidLocations.ContainsKey(targetLocation.Offset(x, y, z)))
                        continue;

                    possibleToPath = true;
                    break;
                }

                if (possibleToPath) targetDic.Add(currentPos.Distance(tempTarget.Value.location), tempTarget.Value);
            }

            return targetDic.Count <= 0 ? null : targetDic.OrderBy(key => key.Key);
        }

        private void MoveToTarget(ILocation targetLocation, MapOptions mapOptions, Action<bool> busyState,
            Action callback)
        {
            if (_invalidLocations.ContainsKey(targetLocation))
            {
                busyState(false);
                if (_stopToken.stopped) return;
                _stopToken?.Stop();
                return;
            }
            
            busyState(true);
            
            _stopToken = new CancelToken();

            var map = actions.AsyncMoveToLocation(targetLocation, _stopToken, mapOptions);
            map.Completed += areaMap =>
            {
                callback();

                if (_invalidLocations.ContainsKey(map.Target)) _invalidLocations.TryRemove(map.Target, out _);
            };

            map.Cancelled += (areaMap, cuboid) =>
            {
                busyState(false);

                if (_stopToken.stopped) return;

                //Only invalidate a location if we can't actually path to it
                //Console.WriteLine($"Invalided Loc: {map.Target}");
                if (!_invalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target);
                _stopToken?.Stop();
            };

            if (!map.Start())
            {
                busyState(false);

                if (_stopToken.stopped) return;

                //Only invalidate a location if we can't actually path to it
                if (!_invalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target);
                _stopToken?.Stop();
            }
            else
            {
                busyState(true);
            }

            //Console.WriteLine($"Target Loc: {map.Target}");
        }
        
        private IAsyncMap TestMovement(IEntity target, MapOptions mapOptions, Action<bool> busyState,
            Action callback)
        {
            busyState(true);
            
            _stopToken = new CancelToken();

            var map = actions.AsyncMoveToEntity(target, _stopToken, mapOptions);
            map.Completed += areaMap =>
            {
                callback();

                if (_invalidLocations.ContainsKey(map.Target)) _invalidLocations.TryRemove(map.Target, out _);
            };

            map.WaypointReached += areaMap =>
            {
                if (map.Target != target.location)
                {
                    map.Token.Stop();
                    TestMovement(target, mapOptions, busyState, callback);
                }
            };

            map.Cancelled += (areaMap, cuboid) =>
            {
                /*if (map.Target != target.location)
                {
                    map.Token.Stop();
                    TestMovement(target, mapOptions, busyState, callback);
                }*/
            };

            map.ChangeDetected += (areaMap, cuboid) => { Console.WriteLine("map.ChangeDetected"); };

            map.Recalculated += (areaMap, found) => { Console.WriteLine("map.Recalculated"); };
            
            if (!map.Start())
            {
                busyState(false);

                if (_stopToken.stopped) return null;;

                //Only invalidate a location if we can't actually path to it
                //if (!_invalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target);
                _stopToken?.Stop();
            }
            else
            {
                busyState(true);
            }

            //Console.WriteLine($"Target Loc: {map.Target}");
            return map;
        }

        private List<ILocation> RandomLocationNearTarget(ILocation targetLocation)
        {
            var targetLocationList = new List<ILocation>();
            for (var y = -3; y <= 3; y++)
            for (var z = -3; z <= 3; z++)
            for (var x = -3; x <= 3; x++)
            {
                var tempLocation = targetLocation.Offset(x, y, z);
                if (player.world.IsWalkable(tempLocation) &&
                    targetLocation.Distance(tempLocation) <= 4 &&
                    IsLocationsDifferent(targetLocation, tempLocation) &&
                    player.world.IsVisible(tempLocation.ToPosition(), targetLocation)/* &&
                    !_invalidLocations.ContainsKey(tempLocation)*/)
                    targetLocationList.Add(tempLocation);
            }

            return targetLocationList;
        }

        private bool IsLocationsDifferent(ILocation loc1, ILocation loc2)
        {
            return loc1.x != loc2.x && Math.Abs(loc1.y - loc2.y) > 0 && loc1.z != loc2.z;
        }

        private void Hit(IEntity target)
        {
            if (target == null) return;

            if (_autoWeapon) actions.EquipWeapon();
            actions.LookAt(target.location.Offset(new Position(0, .65, 0)), true);

            if (_autoWeapon) actions.EquipWeapon();
            // 1 hit tick is about 50 ms.
            _hitTicks++;
            var ms = _hitTicks * 50;

            if (ms >= 1000 / _cps)
            {
                _hitTicks = 0; //Hitting, reset tick counter.
                if (Rnd.Next(1, 101) < _ms) actions.PerformSwing(); //Miss.
                else actions.EntityAttack(target.entityId); //Hit.
            }
        }

        //Add a cooldown to the current location if we can't path to it.
        private void InvalidateLocation(ILocation location)
        {
            //Add location as invalid for 5 Seconds
            if (location != null && !_invalidLocations.ContainsKey(location))
                _invalidLocations.TryAdd(location, DateTime.Now.AddSeconds(5));
        }

        //Remove the locations if they expire so that we don't memory leak
        private void CleanInvalidLocations()
        {
            var deleteList =
                (from location in _invalidLocations where DateTime.Now >= location.Value select location.Key).ToList();

            foreach (var deleteableLoc in deleteList) _invalidLocations.TryRemove(deleteableLoc, out _);
        }
    }
}