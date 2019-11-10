using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.Protocols.Classes.Base;

namespace PvPBotPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random _RND = new Random();
        public static IPlayerEntity SharedTarget;
        public static List<ILocation> SharedTargetLocations = new List<ILocation>();

        public static ConcurrentDictionary<string, bool> SharedTargetLoaded =
            new ConcurrentDictionary<string, bool>();

        public static ConcurrentDictionary<string, bool> SharedTargetReachable =
            new ConcurrentDictionary<string, bool>();

        private static readonly MapOptions _WANDERING_MAP_OPTIONS = new MapOptions
        {
            Swim = true,
            SafeMove = true,
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.MEDIUM,
            Mine = false
        };

        private static readonly MapOptions _TARGETING_SEG_MAP_OPTIONS = new MapOptions
        {
            Swim = true,
            SafeMove = true,
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Segment = true
        };

        private static readonly MapOptions _TARGETING_MAP_OPTIONS = new MapOptions
        {
            Swim = true,
            SafeMove = true,
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Segment = false
        };


        private readonly ConcurrentDictionary<ILocation, DateTime> _invalidLocations =
            new ConcurrentDictionary<ILocation, DateTime>();

        private bool _busy;

        private int _hitTicks;
        private IPlayerEntity _localTarget;
        private bool _movingBusy;
        private List<ILocation> _rndWanderingLocations = new List<ILocation>();
        private bool _scanningRndLocations;
        private IStopToken _stopToken = new CancelToken();
        private bool _wanderingBusy;
        private readonly Stopwatch _sharedInvalidTimer = new Stopwatch();

        private readonly int _mode;
        private readonly int _cps;
        private readonly int _missRate;
        private readonly List<string> _whitelistedTargets;
        private readonly bool _autoWeapon;

        public Attack(int mode, int cps, int missRate, List<string> whitelistedTargets, bool autoWeapon)
        {
            _mode = mode;
            _cps = cps;
            _missRate = missRate;
            _whitelistedTargets = whitelistedTargets;
            _autoWeapon = autoWeapon;
        }

        public override void Start()
        {
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
            try
            {
                UpdateTarget();

                if (_sharedInvalidTimer.ElapsedMilliseconds >= 100)
                {
                    CleanInvalidLocations();
                    _sharedInvalidTimer.Restart();
                }

                if (player.entities.IsBot(SharedTarget?.uuid) || player.entities.IsBot(_localTarget?.uuid))
                {
                    _localTarget = null;
                    SharedTarget = null;
                }

                if (_localTarget != null)
                {
                    if (player.status.entity.location.Distance(_localTarget.location) > 4)
                    {
                        var currentPos = player.status.entity.location;
                        var tempTarget = player.entities.FindClosestPlayer(currentPos.X, currentPos.Y, currentPos.Z);

                        if (tempTarget != null && currentPos.Distance(tempTarget.location) <= 4 /* &&
                            world.IsVisible(currentPos.Offset(new Position(0, .65, 0)), 
                                tempTarget.location.ToLocation())*/)
                            Hit(tempTarget);
                        return;
                    }

                    Hit(_localTarget);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }

        public void OnTick()
        {
            try
            {
                var rndLocList = new List<ILocation>();

#region Target Selector and Validator

                if (SharedTarget == null || _localTarget == null)
                {
                    //We already have a _sharedTarget, copying it to _localTarget and checking reachable block nearby
                    if (SharedTarget != null && SharedTarget != _localTarget)
                    {
                        //NOTE: Also returns "null" if the entity isn't rendered.
                        _localTarget = (IPlayerEntity) player.entities.GetEntity(SharedTarget.entityId);

                        if (_localTarget != null)
                        {
                            //target has no possible blocks around it, resseting everything
                            if (SharedTargetLocations?.Count < 1)
                            {
                                SharedTargetReachable?.TryRemove(player.status.uuid, out _);

                                _localTarget = null;
                                SharedTarget = null;
                                SharedTargetLocations = new List<ILocation>();
                                _movingBusy = false;
                            }
                        }
                        else
                        {
                            _localTarget = SharedTarget;
                        }
                    }
                    else
                    {
                        //Array of all possible enemy in render distance
                        var closestPlayerList = SearchClosestTarget(player.status.entity.location, 3)?.ToList();

                        //Just choosing the first player in render distance right now
                        if (closestPlayerList != null && closestPlayerList.Count > 0)
                        {
                            IPlayerEntity tempSharedTarget = null;
                            //Using the first possible enemy for now
                            if (closestPlayerList.Any()) tempSharedTarget = closestPlayerList.First().Key;

                            if (tempSharedTarget != null)
                            {
                                //We got a new target, let's check if it it has any possible blocks around it
                                SharedTargetLocations = RandomLocationNearTarget(tempSharedTarget);

                                //target has no possible blocks around it, resseting everything
                                if (SharedTargetLocations.Count < 1)
                                {
                                    _localTarget = null;
                                    SharedTarget = null;
                                    SharedTargetLocations = new List<ILocation>();
                                    SharedTargetReachable = new ConcurrentDictionary<string, bool>();
                                    SharedTargetLoaded = new ConcurrentDictionary<string, bool>();
                                    _movingBusy = false;
                                }
                                else
                                {
                                    SharedTarget = tempSharedTarget;

                                    SharedTargetLoaded.TryAdd(player.status.uuid, true);
                                    SharedTargetReachable.TryAdd(player.status.uuid, true);

                                    _localTarget = SharedTarget;
                                }
                            }
                        }
                    }
                }

#endregion

                //The target is either not reachable or out of renderdistance
                if (_localTarget != null)
                {
                    var targetingMapOptions = _TARGETING_MAP_OPTIONS;
                    //We got a new target, let's check if it it has any possible blocks around it
                    if (SharedTargetLocations != null && SharedTargetLocations.Count < 1)
                        rndLocList = RandomLocationNearTarget(_localTarget);

                    if (SharedTargetLocations != null && SharedTargetLocations.Count > 1)
                    {
                        var tempLocalPlayer = player.entities.GetPlayer(_localTarget.entityId);

                        if (tempLocalPlayer == null ||
                            RandomLocationNearTarget(_localTarget).Count < 1 && tempLocalPlayer.unloaded)
                        {
                            rndLocList = SharedTargetLocations;
                            targetingMapOptions = _TARGETING_SEG_MAP_OPTIONS;
                        }
                        else
                        {
                            rndLocList = RandomLocationNearTarget(_localTarget);
                        }
                    }

                    if (rndLocList?.Count < 1)
                    {
                        SharedTargetReachable?.TryRemove(player.status.uuid, out _);

                        _localTarget = null;
                        /*SharedTarget = null;
                        SharedTargetLocations = new List<ILocation>();*/
                        _movingBusy = false;
                        rndLocList = null;
                    }

                    if (rndLocList?.Count > 0 && !_movingBusy)
                    {
                        //Pick a random Loc around the target
                        var rndLocationCount = new Random().Next(0, rndLocList.Count);

                        if (!_invalidLocations.ContainsKey(rndLocList[rndLocationCount]))
                        {
                            if (_wanderingBusy || _scanningRndLocations || _rndWanderingLocations?.Count > 0)
                            {
                                //Stop pathing if we are actually pathing somewhere
                                _stopToken?.Stop();

                                _wanderingBusy = false;
                                _scanningRndLocations = false;
                                _rndWanderingLocations = new List<ILocation>();
                            }

                            if (SharedTarget != null && !SharedTarget.unloaded)
                                SharedTargetLoaded.TryAdd(player.status.uuid, !SharedTarget.unloaded);

                            SharedTargetReachable?.TryAdd(player.status.uuid, true);

                            _movingBusy = true;

                            //Move to the target
                            MoveToTarget(rndLocList[rndLocationCount], targetingMapOptions, 30,
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

                IPlayerEntity testingshit = null;

                Console.WriteLine($"{testingshit.isDead}");

                if (!_wanderingBusy && rndLocList?.Count < 1 && !_movingBusy)
                {
                    if (!_scanningRndLocations && (_rndWanderingLocations == null || _rndWanderingLocations?.Count < 1))
                    {
                        _scanningRndLocations = true;
                        var currentLoc = player.status.entity.location.ToLocation();
                        var rnd = new Random();
                        var rndArea = rnd.Next(25, 50);
                        var rndHeight = rnd.Next(5, 10);

                        FindRandomWanderingLocation(currentLoc, rndArea, rndHeight, locations =>
                        {
                            _scanningRndLocations = false;

                            if (locations.Count < 1)
                            {
                                _wanderingBusy = false;
                                _rndWanderingLocations = new List<ILocation>();
                                return;
                            }

                            _rndWanderingLocations = locations;
                        });
                    }

                    if (_rndWanderingLocations != null && _rndWanderingLocations.Count > 0 && !_scanningRndLocations)
                    {
                        var rndLocInt = new Random().Next(0, _rndWanderingLocations.Count);
                        var rndLoc = _rndWanderingLocations[rndLocInt];
                        _rndWanderingLocations.Remove(rndLoc);

                        //NOTE: Kinda redutent I think ^^
                        if (!player.world.IsWalkable(rndLoc) || _invalidLocations.ContainsKey(rndLoc))
                        {
                            _wanderingBusy = false;
                            if (!_invalidLocations.ContainsKey(rndLoc)) InvalidateLocation(rndLoc, 5);
                            return;
                        }

                        _wanderingBusy = true;

                        MoveToTarget(rndLoc, _WANDERING_MAP_OPTIONS, 5, busyState => _wanderingBusy = busyState,
                            () => { _wanderingBusy = false; });
                    }
                }
            }
            catch (Exception ex)
            {
                ZerGo0Debugger.ErrorMessage(player.status.username, ex);
            }
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

                    heightOffset++;

                    if (stopwatch.ElapsedMilliseconds >= maxMs) break;
                }

                if (heightOffset < scanAreaHeight) return;

                _stopToken?.Stop();
                callback(locations);
            });
        }

        private IEnumerable<KeyValuePair<IPlayerEntity, double>> SearchClosestTarget(IPosition currentPos,
            int areaAroundTarget)
        {
            var targetDic = new Dictionary<IPlayerEntity, double>();
            foreach (var target in player.entities.playerList)
            {
                if (target.Value == null) continue;

                var tempTarget = new KeyValuePair<int, IPlayerEntity>(target.Key, (IPlayerEntity) target.Value);

                /*Console.WriteLine($"Found Target {player.entities.FindNameByUuid(tempTarget.Value.uuid).Name} " +
                                  $"H: {tempTarget.Value.health} D: {tempTarget.Value.isDead}");*/

                if (player.entities.IsBot(tempTarget.Value.uuid) || tempTarget.Value == null ||
                    tempTarget.Value.isDead) continue;

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

                if (possibleToPath && !targetDic.ContainsKey(tempTarget.Value))
                    targetDic.Add(tempTarget.Value, currentPos.Distance(tempTarget.Value.location));
            }

            return targetDic.Count <= 0 ? null : targetDic.OrderBy(key => key.Value);
        }

        private void MoveToTarget(ILocation targetLocation, MapOptions mapOptions, int timeOut, Action<bool> busyState,
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

                //Only invaliddate a location if we can't actually path to it
                if (!_invalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target, timeOut);
                _stopToken?.Stop();
            };

            if (!map.Start())
            {
                busyState(false);

                if (_stopToken.stopped) return;

                //Only invalidate a location if we can't actually path to it
                if (!_invalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target, timeOut);
                _stopToken?.Stop();
            }
            else
            {
                busyState(true);
            }
        }

        private List<ILocation> RandomLocationNearTarget(IPlayerEntity target)
        {
            var targetLocationList = new List<ILocation>();
            var targetLocation = target.location.ToLocation();
            for (var y = -3; y <= 3; y++)
            for (var z = -3; z <= 3; z++)
            for (var x = -3; x <= 3; x++)
            {
                var tempLocation = targetLocation.Offset(x, y, z);
                if (player.world.IsWalkable(tempLocation) &&
                    targetLocation.Distance(tempLocation) < 4 &&
                    IsLocationsDifferent(targetLocation, tempLocation) &&
                    player.world.IsVisible(tempLocation.ToPosition(), targetLocation) &&
                    !_invalidLocations.ContainsKey(tempLocation) &&
                    !_whitelistedTargets.Contains(player.entities.FindNameByUuid(target.uuid).Name))
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

            //NOTE: idk if I want this or not
            /*if (!world.IsVisible(player.status.entity.location.Offset(new Position(0, .65, 0)), 
                target.location.ToLocation((float)0.65))) return;*/

            if (_autoWeapon) actions.EquipWeapon();
            actions.LookAt(target.location.Offset(new Position(0, .65, 0)), true);

            // 1 hit tick is about 50 ms.
            _hitTicks++;
            var ms = _hitTicks * 50;

            if (ms >= 1000 / _cps)
            {
                _hitTicks = 0; //Hitting, reset tick counter.
                if (_RND.Next(1, 101) < _missRate) actions.PerformSwing(); //Miss.
                else actions.EntityAttack(target.entityId); //Hit.
            }
        }

        //Add a cooldown to the current location if we can't path to it.
        private void InvalidateLocation(ILocation location, int timeOut)
        {
            //Add location as invalid for "timeOut" Seconds
            if (location != null && !_invalidLocations.ContainsKey(location))
                _invalidLocations.TryAdd(location, DateTime.Now.AddSeconds(timeOut));
        }

        //Remove the locations if they expire so that we don't memory leak
        private void CleanInvalidLocations()
        {
            var deleteList =
                (from location in _invalidLocations where DateTime.Now >= location.Value select location.Key).ToList();

            foreach (var deleteableLoc in deleteList) _invalidLocations.TryRemove(deleteableLoc, out _);
        }

        private void UpdateTarget()
        {
            if (SharedTarget == null || SharedTarget != null && SharedTarget.unloaded)
            {
                SharedTargetLoaded.TryRemove(player.status.uuid, out _);
                SharedTargetReachable.TryRemove(player.status.uuid, out _);
            }

            if (SharedTargetLoaded?.Count < 1 || SharedTargetReachable?.Count < 1 || SharedTarget == null)
            {
                SharedTarget = null;
                _localTarget = null;
                SharedTargetLocations = new List<ILocation>();
                SharedTargetLoaded = new ConcurrentDictionary<string, bool>();
                SharedTargetReachable = new ConcurrentDictionary<string, bool>();

                if (_movingBusy)
                {
                    _movingBusy = false;
                    _stopToken.Stop();
                }
            }

            if (SharedTarget != null)
            {
                if (RandomLocationNearTarget(SharedTarget).Count < 1)
                    SharedTargetReachable?.TryRemove(player.status.uuid, out _);
                else
                    SharedTargetReachable?.TryAdd(player.status.uuid, true);

                if (SharedTarget != null && SharedTarget.isDead)
                {
                    SharedTarget = null;
                    _localTarget = null;
                    SharedTargetLocations = new List<ILocation>();
                    SharedTargetLoaded = new ConcurrentDictionary<string, bool>();
                    SharedTargetReachable = new ConcurrentDictionary<string, bool>();

                    if (_movingBusy)
                    {
                        _movingBusy = false;
                        _stopToken.Stop();
                    }
                }
            }
        }
    }
}