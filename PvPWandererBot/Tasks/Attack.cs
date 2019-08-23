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
using OQ.MineBot.PluginBase.Movement.Geometry;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace PvPWandererPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly Random Rnd = new Random();
        private static IPlayerEntity _sharedTarget;
        private static readonly ConcurrentDictionary<ILocation, DateTime> InvalidLocations =
            new ConcurrentDictionary<ILocation, DateTime>();

        private static readonly Stopwatch SharedInvalidTimer = new Stopwatch();
        private static bool SharedInvalidTimerStart = false;
        

        private static readonly MapOptions Mo = new MapOptions
        {
            AntiStuck = true,
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = false
        };
        
        private int _hitTicks;
        private bool _busy;
        private bool _movingBusy;
        private bool _wanderingBusy;
        private IPlayerEntity _localTarget;
        private CancelToken _stopToken = new CancelToken();

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
            
            if (!SharedInvalidTimerStart)
            {
                CleanInvalidLocations();
                SharedInvalidTimer.Start();
                SharedInvalidTimerStart = true;
            }
        }

        private void EventsOnOnPlayerUpdate(IStopToken cancel)
        {
            if (SharedInvalidTimer.ElapsedMilliseconds >= 1000)
            {
                CleanInvalidLocations();
                SharedInvalidTimer.Restart();
            }
            
            if (_sharedTarget != null && player.entities.IsBot(_sharedTarget.uuid) ||
                _localTarget != null && player.entities.IsBot(_localTarget.uuid))
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

        public override void Stop()
        {
            //Not really needed since the plugin gets unloaded anyway,
            //but yea let's pretend that I'm a good dev :laughf:
            player.events.onPlayerUpdate -= EventsOnOnPlayerUpdate;
            
            if (_stopToken.stopped) return;
            _stopToken.Stop();
        }

        public override bool Exec() {
            return !status.entity.isDead && !status.eating &&!_busy;
        }

        public void OnTick()
        {
            if (_localTarget == null && _sharedTarget != null) _localTarget = _sharedTarget;
            
            var rndLocList = new List<ILocation>();
            
            if (_sharedTarget == null || _localTarget == null)
            {
                //Array of all possible enemy in render distance
                var closestPlayerkeyValuePairs = SearchClosestTarget(player.status.entity.location, 3);

                var closestPlayerList = closestPlayerkeyValuePairs?.ToList();

                if (closestPlayerList?.First().Value != null)
                {
                    //Using the first possible enemy for now
                    if (closestPlayerList.Any()) _sharedTarget = closestPlayerList.First().Value;

                    //Local target so that it doesn't get nulled while doing something
                    if (_localTarget == null || _sharedTarget != _localTarget) _localTarget = _sharedTarget;

                    if (_localTarget != null)
                    {
                        if (rndLocList.Count < 1)
                            rndLocList = RandomLocationNearTarget(_localTarget.location.ToLocation());
                        if (rndLocList.Count < 1)
                        {
                            //Enemy has no possible location around it
                            _localTarget = null;
                            _sharedTarget = null;
                        }
                    }
                }
            }
            
            if (_localTarget != null)
            {
                //if (player.entities.FindNameByUuid(_localTarget.uuid) != null) Console.WriteLine($"{player.status.username}: target {player.entities.FindNameByUuid(_localTarget.uuid).Name}");

                if (rndLocList.Count < 1)
                    rndLocList = RandomLocationNearTarget(_localTarget.location.ToLocation());
                if (rndLocList.Count < 1)
                {
                    //Enemy has no possible location around it
                    _localTarget = null;
                    _sharedTarget = null;
                    if (_stopToken.stopped) return;
                    _stopToken?.Stop();
                    return;
                }
                
                if (!_movingBusy)
                {
                    //Pick a random Loc around the target
                    var rndLocationCount = new Random().Next(0, rndLocList.Count);

                    if (_wanderingBusy)
                    {
                        _stopToken?.Stop();
                        _wanderingBusy = false;
                    }
                
                    if (InvalidLocations.ContainsKey(rndLocList[rndLocationCount])) return;

                    _movingBusy = true;
                    
                    //TODO: Add a check to see if we need to enable segmented paths 
                    //Move to the target
                    MoveToTarget(rndLocList[rndLocationCount],
                        busyState => _movingBusy = busyState, () => { _movingBusy = false; });
                }
            }
            else
            {
                if (!_wanderingBusy)
                {
                    //No target found, wandering stuff here
                    var wanderLimit = 10; //4096*4
                    var rnd = new Random();
                    var rndX = rnd.Next(-wanderLimit, wanderLimit);
                    var rndZ = rnd.Next(-wanderLimit, wanderLimit);

                    var currentLoc = player.status.entity.location.ToLocation();
                    var randomLoc = new Location(currentLoc.x + rndX, currentLoc.y - 1, currentLoc.z + rndZ);
                    
                    if (world.GetBlockId(randomLoc) == 0 || !player.world.IsWalkable(randomLoc) || InvalidLocations.ContainsKey(randomLoc))
                    {
                        if (!InvalidLocations.ContainsKey(randomLoc)) InvalidateLocation(randomLoc);
                        return;
                    };
                    
                    _wanderingBusy = true;
                    
                    MoveToTarget(randomLoc, busyState => _wanderingBusy = busyState,
                        () => { _wanderingBusy = false; });

                    /*//Create a list.
                    List<ILocation> locations = new List<ILocation>();
                    const int maxMs = 50;

                    int currentHeight = (int) location.y;
                    int heightOffset = 0;
                    var stopwatch = Stopwatch.StartNew();

                    IStopToken token = null;
                    token = player.tickManager.RegisterReocurring(1, () =>
                    {
                        stopwatch.Restart();
                        while (heightOffset < height)
                        {
                            var yPlus = currentHeight + heightOffset;
                            if (yPlus <= 256)
                                for (int x = -width; x < width; x++)
                                for (int z = -width; z < width; z++)
                                {
                                    if (GetBlockId((int) location.x + x, (int) yPlus, (int) location.z + z) == id)
                                        locations.Add(new Location((int) location.x + x, (int) yPlus,
                                            (int) location.z + z));
                                }

                            var yMinus = currentHeight - heightOffset;
                            if (yMinus > 0 && heightOffset != 0)
                                for (int x = -width; x < width; x++)
                                for (int z = -width; z < width; z++)
                                {
                                    //Check if the block id match.
                                    if (GetBlockId((int) location.x + x, (int) yMinus, (int) location.z + z) == id)
                                        locations.Add(new Location((int) location.x + x, (int) yMinus,
                                            (int) location.z + z));
                                }

                            heightOffset++;

                            if (stopwatch.ElapsedMilliseconds >= maxMs) break;
                        }

                        var end = heightOffset >= height;
                        if (end)
                        {
                            token.Stop();
                            callback(locations.ToArray());
                        }
                    });*/
                }
            }
        }

        private IEnumerable<KeyValuePair<double, IPlayerEntity>> SearchClosestTarget(IPosition currentPos, int areaAroundTarget)
        {
            var targetDic = new Dictionary<double, IPlayerEntity>();
            foreach (var target in player.entities.playerList)
            {
                if (target.Value == null) continue; 
                    
                var tempTarget = new KeyValuePair<int, IPlayerEntity>(target.Key, (IPlayerEntity) target.Value);
                if (player.entities.IsBot(tempTarget.Value.uuid)) continue;
                
                var targetLocation = tempTarget.Value.location.ToLocation();

                var possibleToPath = false;
                for (var y = -areaAroundTarget; y < areaAroundTarget; y++)
                {
                    for (var z = -areaAroundTarget; z < areaAroundTarget; z++)
                    {
                        for (var x = -areaAroundTarget; x < areaAroundTarget; x++)
                        {
                            if (!player.world.IsWalkable(targetLocation.Offset(x, y, z)) ||
                                !(targetLocation.Distance(targetLocation.Offset(x, y, z)) <= areaAroundTarget) ||
                                !player.world.IsVisible(tempTarget.Value.location.Offset(new Position(x, y, z)), targetLocation) ||
                                InvalidLocations.ContainsKey(targetLocation.Offset(x, y, z)))
                                continue;
                            
                            possibleToPath = true;
                            break;
                        }
                    }
                }
                
                if (possibleToPath) targetDic.Add(currentPos.Distance(tempTarget.Value.location), tempTarget.Value);
            }

            return targetDic.Count <= 0 ? null : targetDic.OrderBy(key => key.Key);
        }

        private void MoveToTarget(ILocation targetLocation, Action<bool> busyState, Action callback)
        {
            _stopToken = new CancelToken();
            
            var map = actions.AsyncMoveToLocation(targetLocation, _stopToken, Mo);
            
            map.Completed += areaMap => {
                callback();

                if (InvalidLocations.ContainsKey(map.Target)) InvalidLocations.TryRemove(map.Target, out _);
            };
            
            map.Cancelled += (areaMap, cuboid) =>
            {
                busyState(false);

                if (_stopToken.stopped) return;
                
                
                //Only invalidate a location if we can't actually path to it
                Console.WriteLine($"Invalided Loc: {map.Target}");
                if (!InvalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target);
                _stopToken.Stop();
            };

            if (!map.Start())
            {
                busyState(false);
                
                if (_stopToken.stopped) return;
                
                //Only invalidate a location if we can't actually path to it
                if (!InvalidLocations.ContainsKey(map.Target)) InvalidateLocation(map.Target);
                _stopToken.Stop();
            }
            else
            {
                busyState(true);
            }
            
            //Console.WriteLine($"Target Loc: {map.Target}");
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
                            targetLocation.Distance(tempLocation) <= 4 &&
                            IsLocationsDifferent(targetLocation, tempLocation) &&
                            player.world.IsVisible(tempLocation.ToPosition(), targetLocation) &&
                            !InvalidLocations.ContainsKey(tempLocation))
                        {
                            targetLocationList.Add(tempLocation);
                        }
                    }
                }
            }

            return targetLocationList;
        }

        private bool IsLocationsDifferent(ILocation loc1, ILocation loc2)
        {
            return (loc1.x != loc2.x && Math.Abs(loc1.y - loc2.y) > 0 && loc1.z != loc2.z);
        }

        private void Hit(IEntity target)
        {
            if (target == null) return;

            if(_autoWeapon) actions.EquipWeapon();
            actions.LookAt(target.location.Offset(new Position(0, .65, 0)), true);
                
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
        
        //Add a cooldown to the current location if we can't path to it.
        private static void InvalidateLocation(ILocation location)
        {
            //Add location as invalid for 5 Seconds
            if (location != null && !InvalidLocations.ContainsKey(location)) 
                InvalidLocations.TryAdd(location, DateTime.Now.AddSeconds(5));
        }

        //Remove the locations if they expire so that we don't memory leak
        private void CleanInvalidLocations()
        {
            //var tempCount = InvalidLocations.Count;
            var deleteList = (from location in InvalidLocations where DateTime.Now >= location.Value select location.Key).ToList();

            foreach (var deleteableLoc in deleteList)
            {
                InvalidLocations.TryRemove(deleteableLoc, out _);
            }
            
            //if (tempCount > InvalidLocations.Count) Console.WriteLine($"Before: {tempCount} | After: {InvalidLocations.Count}");
        }
    }
}