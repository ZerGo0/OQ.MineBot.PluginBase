using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace TreeFarmerPlugin.Tasks
{
    public class Mine : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions
        {
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = true,
            Unwalkable = new ushort[] {6}
        };

        private readonly ushort[] _ids;
        private readonly MacroSync _macro;

        private readonly ushort[] _plantable = {6};
        private readonly bool _replant;

        private bool _busy;
        private bool _currentlyMining;
        private bool _gotTargetBlock;
        private ILocation _location;

        public Mine(bool replant, MacroSync macro)
        {
            _replant = replant;
            _macro = macro;

            var list = new List<ushort> {17};
            _ids = list.ToArray();

            BlocksGlobal.BUILDING_BLOCKS = new ushort[] { 1, 4, 5, 24, 35, 45, 87, 98, 121, 125, 159, 155, 162, 172, 174, 179 };
        }

        public void OnTick()
        {
            var current = player.status.entity.location.ToLocation();

            if (_location == null)
            {
                _gotTargetBlock = false;
                FindClosestBlock();
                return;
            }

#if (DEBUG)
            Console.WriteLine("before _gotTargetBlock");
#endif
            if (_gotTargetBlock)
            {
#if (DEBUG)
                Console.WriteLine("_gotTargetBlock");
#endif
                if (!player.physicsEngine.isGrounded)
                {
                    _currentlyMining = false;
                    return;
                }

                if (player.physicsEngine.isGrounded && _currentlyMining)
                {
#if (DEBUG)
                    Console.WriteLine("Currently mining");
#endif
                    return;
                }

                if (Buildingblocks(current))
                {
                    _location = current.Offset(-1);
#if (DEBUG)
                    Console.WriteLine("Stone Loc: " + _location);
#endif
                    MineBlock(_location);
                    _currentlyMining = true;
                    return;
                }

                if (Stopblocks(current))
                {
                    TaskCompleted();
                    return;
                }

                TaskCompleted();
            }

            _currentlyMining = false;

            var cancelToken = new CancelToken();
            var map = actions.AsyncMoveToLocation(_location, cancelToken, Mo);
            map.Completed += areaMap =>
            {
                if (_replant)
                {
                    current = player.status.entity.location.ToLocation();
                    if (inventory.FindId(6) > 0 && player.world.GetBlockId(current.Offset(-1)) == 3 ||
                        inventory.FindId(6) > 0 && player.world.GetBlockId(current.Offset(-1)) == 2)
                        Replant(current);
                }

                _gotTargetBlock = true;
                _busy = false;
#if (DEBUG)
                Console.WriteLine("map complete end");
#endif
            };
            map.Cancelled += (areaMap, cuboid) =>
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_location);
                TaskCompleted();
            };

            if (!map.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_location);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }

#if (DEBUG)
            Console.WriteLine("Tick END");
#endif
        }

        private bool Stopblocks(ILocation current)
        {
            return player.world.GetBlockId(current.Offset(-1)) == 17 ||
                   player.world.GetBlockId(current.Offset(-1)) == 3 ||
                   player.world.GetBlockId(current.Offset(-1)) == 2;
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && !_macro.IsMacroRunning() && !inventory.IsFull();
        }

        private bool Buildingblocks(ILocation current)
        {
            return player.world.GetBlockId(current.Offset(-1)) == 1 ||
                   player.world.GetBlockId(current.Offset(-1)) == 4 ||
                   player.world.GetBlockId(current.Offset(-1)) == 5 ||
                   player.world.GetBlockId(current.Offset(-1)) == 24 ||
                   player.world.GetBlockId(current.Offset(-1)) == 35 ||
                   player.world.GetBlockId(current.Offset(-1)) == 45 ||
                   player.world.GetBlockId(current.Offset(-1)) == 87 ||
                   player.world.GetBlockId(current.Offset(-1)) == 98 ||
                   player.world.GetBlockId(current.Offset(-1)) == 121 ||
                   player.world.GetBlockId(current.Offset(-1)) == 159 ||
                   player.world.GetBlockId(current.Offset(-1)) == 155 ||
                   player.world.GetBlockId(current.Offset(-1)) == 162 ||
                   player.world.GetBlockId(current.Offset(-1)) == 172 ||
                   player.world.GetBlockId(current.Offset(-1)) == 174 ||
                   player.world.GetBlockId(current.Offset(-1)) == 179;
        }

        private void MineBlock(ILocation location)
        {
            // Break the block.
            player.tickManager.Register(3, () =>
            {
                actions.LookAtBlock(location, true);
                actions.SelectBestTool(location);
                player.tickManager.Register(1, () => { actions.BlockDig(location, action => { }); });
            });
        }

        private void FindClosestBlock()
        {
            _busy = true;
            world.FindFirstAsync(player, player.status.entity.location.ToLocation(), 64, 256, _ids, IsSafe, loc =>
            {
                for (var y = 0; y < 256; ++y)
                {
                    if (player.world.GetBlockId(loc.Offset(y)) == 17) continue;
                    var topblock = loc.Offset(y - 1);

#if (DEBUG)
                    Console.WriteLine("Loop count: " + y.ToString());
                    Console.WriteLine("Block Loc: " + topblock);
#endif

                    _location = topblock.Offset(-1);

                    _busy = false;
                    break;
                }
            });
        }

        private bool IsSafe(ILocation location)
        {
            if (BeingMined.ContainsKey(location) || _personalBlocks.ContainsKey(location)) return false;
            return IsTunnelable(location);
        }

        private void Replant(ILocation location)
        {
            var prioritizedList = _plantable.ToList();
            var prioritizedArray = prioritizedList.ToArray();

            if (inventory.Select(prioritizedArray) == -1) return;

#if (DEBUG)
            Console.WriteLine("Replant-loc: " + location);
#endif
            var face = player.functions.FindValidNeighbour(location);
            player.functions.LookAtBlock(face.location, true, face.face);

            player.tickManager.Register(2, () =>
            {
#if (DEBUG)
                Console.WriteLine("placing block");
#endif
                player.functions.LookAtBlock(face.location, true, face.face);

                player.functions.BlockPlaceOnBlockFace(face.location, face.face);
            });
        }

        private bool IsTunnelable(ILocation pos)
        {
            return !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(2))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(3))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1, 1, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(1, 2, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 1, 1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 2, 1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(-1, 1, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(-1, 2, 0))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 1, -1))) &&
                   !BlocksGlobal.blockHolder.IsDanger(player.world.GetBlockId(pos.Offset(0, 2, -1)))
                ;
        }

        private void TaskCompleted()
        {
            _location = null;
            _busy = false;
            _gotTargetBlock = false;
        }

        private void InvalidateBlock(ILocation location)
        {
            if (location != null) _personalBlocks.TryAdd(location, null);
        }

        #region Shared work.

        /// <summary>
        ///     Blocks that are taken already.
        /// </summary>
        public static readonly ConcurrentDictionary<ILocation, object>
            BeingMined = new ConcurrentDictionary<ILocation, object>();

        private readonly ConcurrentDictionary<ILocation, object> _personalBlocks =
            new ConcurrentDictionary<ILocation, object>();

        #endregion
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                      //
    //                                                     Area Part                                                       //
    //                                                                                                                    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public class MineArea : ITask, ITickListener
    {
        private static readonly MapOptions MoNoSapling = new MapOptions
        {
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = true,
            Unwalkable = new []{(ushort)6}
        };

        #region Shared work.

        /// <summary>
        ///     Blocks that are taken already.
        /// </summary>
        public static readonly ConcurrentDictionary<ILocation, DateTime> Broken =
            new ConcurrentDictionary<ILocation, DateTime>();

        #endregion

        private readonly MacroSync _macro;

        private readonly ushort[] _plantable = {6};
        private readonly IRadius _radius;

        private readonly bool _replant;
        private bool _busy;
        private ILocation _target;
        private IAsyncMap _targetblock;
        private int _targetblockid;
        private ILocation _safeLocation;
        private BackgroundWorker _scanWorker;
        private ILocation _prevLocation;

        public MineArea(bool replant, ILocation startLocation, ILocation endLocation, MacroSync macro)
        {
            //if (startLocation == null || endLocation == null) return;

            _replant = replant;
            _macro = macro;
            BlocksGlobal.BUILDING_BLOCKS = new ushort[] { 1, 4, 5, 24, 35, 45, 87, 98, 121, 125, 159, 155, 162, 172, 174, 179};

            _radius = new IRadius(startLocation, endLocation);
        }

        public override void Start()
        {
            _safeLocation = player.status.entity.location.ToLocation().Offset(-1);
        }

        public void OnTick()
        {
#if (DEBUG)
            //Console.WriteLine("Tick Start");
#endif

            if (_target == null)
            {
                _target = FindNext(_prevLocation);
#if (DEBUG)
                //Console.WriteLine("Target:" + _target);
#endif
            }

            if (_target == null) return;

            _busy = true;

            var mainCancelToken = new CancelToken();
            _targetblock = actions.AsyncMoveToLocation(_target.Offset(-1), mainCancelToken, MoNoSapling);

#if (DEBUG)
            //Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
                WaitGrounded(() => {
#if (DEBUG)
                    //Console.WriteLine("map complete");
#endif
                    if (_replant)
                    {
                        var current = player.status.entity.location.ToLocation();
                        if (_targetblockid == 17 && inventory.FindId(6) > 0 &&
                            player.world.GetBlockId(current.Offset(-1)) == 3 ||
                            _targetblockid == 17 && inventory.FindId(6) > 0 &&
                            player.world.GetBlockId(current.Offset(-1)) == 2)
                        {
#if (DEBUG)
                            //Console.WriteLine("Replant(current);");
#endif
                            Replant(current);

                            player.tickManager.Register(10, () =>
                            {
                                _target = null;
                                _target = FindNext(null);
                                if (_target == null)
                                {
                                    _prevLocation = null;
                                    GoToLocation(_safeLocation, TaskCompleted);
                                }
                                else
                                {
                                    TaskCompleted();
                                }
                            });
                        }
                        else
                        {
                            _target = null;
                            _target = FindNext(_targetblock.Target.Offset(1));
                            if (_target == null)
                            {
                                _prevLocation = null;
                                GoToLocation(_safeLocation, TaskCompleted);
                            }
                            else
                            {
                                _prevLocation = _targetblock.Target;
                                TaskCompleted();
                            }
                        }
                    }
                    else
                    {
                        _target = null;
                        _target = FindNext(_targetblock.Target.Offset(1));
                        if (_target == null)
                        {
                            _prevLocation = null;
                            GoToLocation(_safeLocation, TaskCompleted);
                        }
                        else
                        {
                            _prevLocation = _targetblock.Target;
                            TaskCompleted();
                        }
                    }
                });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (_targetblock == null)
                {
                    Console.WriteLine("Cancelled _targetblock null");
                    TaskCompleted();
                    return;
                }

                if (mainCancelToken.stopped) return;

                try
                {
                    mainCancelToken.Stop();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error in CANCELLED: \n" + exception);
                }
                InvalidateBlock(_target.Offset(-1));
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (_targetblock == null)
                {
                    Console.WriteLine("Start _targetblock null");
                    TaskCompleted();
                    return;
                }

                if (mainCancelToken.stopped) return;

                try
                {
                    mainCancelToken.Stop();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error in !START: \n" + exception);
                }
                InvalidateBlock(_target.Offset(-1));
                TaskCompleted();
            }
            else
            {
                _busy = true;

                _scanWorker = null;

                if (_targetblock == null)
                {
                    Console.WriteLine("Else _targetblock null");
                    TaskCompleted();
                    return;
                }

                if (player.world.GetBlockId(_targetblock.Target.Offset(1)) == 17 &&
                    player.world.GetBlockId(_targetblock.Target) == 2) return;

                if (player.world.GetBlockId(_targetblock.Target.Offset(1)) == 17 &&
                    player.world.GetBlockId(_targetblock.Target) == 3) return;
                
                _scanWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
                _scanWorker.DoWork += ScanWorker_DoWork;
                _scanWorker.RunWorkerAsync(mainCancelToken);
            }
#if (DEBUG)
            //Console.WriteLine("Tick End");
#endif
        }

        private void ScanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_targetblock != null)
            {
                while (player.world.GetBlockId(_targetblock.Target.Offset(1)) == _targetblockid)
                {
                    //Console.WriteLine("BlockID: " + player.world.GetBlockId(_targetblock.Target.Offset(1)) + " | Cur Y: " + player.status.entity.location.ToLocation().Offset(-1).y +
                    //                  " | Target Y: " + _targetblock.Target.Offset(1).y);
                    //To prevent getting the bot stuck


                    if (_targetblockid == 17)
                    {
                        if (player.status.entity.location.ToLocation().Offset(-1).y >=
                            _targetblock.Target.Offset(1).y) return;
                    }

                    //Wait till block is not wood anymore
                    Thread.Sleep(1);
                }

                break;
            }

            if (_targetblock == null)
            {
                Console.WriteLine("Worker _targetblock null");
                TaskCompleted();
                return;
            }

            Console.WriteLine("Worker while loop passed");

            _target = null;
            _target = FindNext(_targetblock.Target.Offset(1));

            _prevLocation = _targetblock.Target.Offset(1);
            var cancelToken = (CancelToken) e.Argument;

            try
            {
                cancelToken.Stop();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error in Worker: \n" +exception);
            }

            if (_target == null)
                GoToLocation(_safeLocation, TaskCompleted);
            else
            {
                TaskCompleted();
            }
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_busy && !_macro.IsMacroRunning() && !inventory.IsFull();
        }

        private void WaitGrounded(Action callback)
        {
            IStopToken stopToken = null;
            stopToken = player.tickManager.RegisterReocurring(5, () =>
            {
                if (!player.physicsEngine.isGrounded) return;

                stopToken?.Stop();

                callback();
            });
        }

        private void GoToLocation(ILocation targetLocation, Action callback)
        {
            var cancelToken = new CancelToken();

            _busy = true;

            var targetMoveToLocation = actions.AsyncMoveToLocation(targetLocation, cancelToken, MoNoSapling);
            
            targetMoveToLocation.Completed += areaMap => WaitGrounded(callback);
            targetMoveToLocation.Cancelled += (areaMap, cuboid) =>
            {
                Console.WriteLine("Go to safelocation CANCELLED!");
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(targetMoveToLocation.Target);
                TaskCompleted();
            };

            if (!targetMoveToLocation.Start())
            {
                Console.WriteLine("Go to safelocation !Start");
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(targetMoveToLocation.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private ILocation FindNext(ILocation prevTargetLocation)
        {
#if (DEBUG)
            //Console.WriteLine("FindNext Start");
#endif
            //Get the area from the radius.
            if (_radius == null) return null;
#if (DEBUG)
            //Console.WriteLine("_radius != null");
#endif
            ILocation closest = null;
            
            if (prevTargetLocation != null)
            {
                if (_targetblockid == 17)
                {
                    for (int i = 0; i > -25; i--)
                    {
                        if (player.world.GetBlockId(prevTargetLocation.Offset(i)) == 17)
                        {
                            closest = prevTargetLocation.Offset(i);
                            _targetblockid = player.world.GetBlockId(prevTargetLocation.Offset(i));
                            return closest;
                        }

                        if (player.world.GetBlockId(prevTargetLocation.Offset(i)) == 2 ||
                            player.world.GetBlockId(prevTargetLocation.Offset(i)) == 3) break;
                    }
                }
            }
            double distance = int.MaxValue;
            for (var y = (int) _radius.start.y + _radius.height; y >= (int) _radius.start.y; y--)
                if (closest == null)
                    for (var x = _radius.start.x; x <= _radius.start.x + _radius.xSize; x++)
                    for (var z = _radius.start.z; z <= _radius.start.z + _radius.zSize; z++)
                    {
                        var tempLocation = new Location(x, y, z);
                        //Check if the block is valid for mining.
                        if (player.world.GetBlockId(x, y, z) == 0) continue;

                        if (Broken.ContainsKey(tempLocation) &&
                            Broken[tempLocation].Subtract(DateTime.Now).TotalSeconds < -15)
                            continue;

                        if (ScanBlocks(tempLocation))
                            continue;

                        // Check if this block is safe to mine.
                        if (!IsSafe(tempLocation)) continue;

                        if (closest == null)
                        {
                            distance = tempLocation.Distance(player.status.entity.location.ToLocation());
                            closest = new Location(x, y, z);
                        }
                        else if (tempLocation.Distance(player.status.entity.location.ToLocation()) < distance)
                        {
                            distance = tempLocation.Distance(player.status.entity.location.ToLocation());
                            closest = tempLocation;
                        }

                        _targetblockid = player.world.GetBlockId(closest);
#if (DEBUG)
                    //Console.WriteLine("TargetBlockId: " + _targetblockid);
                    //Console.WriteLine(closest);
#endif
                    }

#if (DEBUG)
            //Console.WriteLine("FindNext End");
#endif
            return closest;
        }

        private void Replant(ILocation location)
        {
            var prioritizedList = _plantable.ToList();
            var prioritizedArray = prioritizedList.ToArray();

            if (inventory.Select(prioritizedArray) == -1) return;

#if (DEBUG)
            //Console.WriteLine("Replant-loc: " + location);
#endif
            var face = player.functions.FindValidNeighbour(location);
            player.functions.LookAtBlock(face.location, true, face.face);

            player.tickManager.Register(2, () =>
            {
#if (DEBUG)
                //Console.WriteLine("placing block");
#endif
                player.functions.BlockPlaceOnBlockFace(face.location, face.face);
            });
        }

        private void TaskCompleted()
        {
            _target = null;
            _busy = false;

            if (_scanWorker != null)
            {
                _scanWorker.CancelAsync();
                _scanWorker.Dispose();
                _scanWorker = null;
            }
        }

        private static void InvalidateBlock(ILocation location)
        {
            if (location != null) Broken.TryAdd(location, DateTime.Now);
        }

        private bool ScanBlocks(ILocation tempLocation)
        {
            return player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 17 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 1 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 4 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 5 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 24 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 35 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 45 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 87 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 98 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 121 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 125 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 155 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 159 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 162 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 172 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 174 &&
                   player.world.GetBlockId(tempLocation.x, (int) tempLocation.y, tempLocation.z) != 179;
        }

        private bool IsSafe(ILocation location)
        {
            if (player.world.IsStandingOn(location, player.status.entity.location))
            {
                if (!BlocksGlobal.blockHolder.IsSafeToMine(player.world, location, true))
                    return false;
            }
            else if (!BlocksGlobal.blockHolder.IsSafeToMine(player.world, location))
            {
                return false;
            }

            return true;
        }
    }
}