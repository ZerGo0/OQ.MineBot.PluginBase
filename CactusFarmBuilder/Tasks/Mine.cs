using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace CactusFarmBuilder.Tasks
{
    public class CactusFarmBuilder : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.MEDIUM,
            Mine = true
        };

        #region Shared work.

        /// <summary>
        ///     Blocks that are taken already.
        /// </summary>
        public static readonly ConcurrentDictionary<ILocation, DateTime> Broken =
            new ConcurrentDictionary<ILocation, DateTime>();

        #endregion

        private readonly MacroSync _fillermacro;

        private readonly MacroSync _macro;
        private readonly int _speedmode;
        private readonly bool _gamemode;
        private readonly int _maxlayers;
        private readonly int _tickDelay;
        private readonly int _direction;

        private bool _busy;

        private ILocation _currentbuildingloc;

        private bool _firstLayer;
        private bool _firstLayerCactus;

        private bool _firstLayerSand;
        private bool _firstLayerString;
        private bool _firstLayerStringEast;
        private bool _firstLayerStringNorth;
        private bool _firstLayerStringSouth;
        private bool _firstLayerStringWest;



        private bool _secondLayerStringEast;
        private bool _secondLayerStringNorth;
        private bool _secondLayerStringSouth;
        private bool _secondLayerStringWest;
        private bool _needToMoveToMiddle;
        private bool _needToMoveToEast;
        private bool _needToMoveToSouth;
        private bool _needToMoveToWest;
        private bool _needToMoveToNorth;



        private bool _secondLayer;
        private bool _secondLayerCactus;

        private bool _secondLayerSand;
        private bool _secondLayerSandEast;
        private bool _secondLayerSandNorth;
        private bool _secondLayerSandSouth;
        private bool _secondLayerSandWest;
        private bool _secondLayerString;

        private int _layerCount;
        private bool _startloc;
        private ILocation _startLocation;
        private ILocation _nextLocation;

        private ILocation _target;
        private IAsyncMap _targetblock;

        public CactusFarmBuilder(int speedmode, bool gamemode, int maxlayers, int direction, MacroSync macro)
        {

            _macro = macro;
            _speedmode = speedmode;
            _gamemode = gamemode;
            _maxlayers = maxlayers;
            _direction = direction;

            //_fillermacro = fillerMacro;

            var split = new[] {"12"};

            var blocks = split.Select(ushort.Parse).ToArray();

            BlocksGlobal.BUILDING_BLOCKS = blocks;

            switch (_speedmode)
            {
                case 0:
                    _tickDelay = 12;
                    break;
                case 1:
                    _tickDelay = 8;
                    break;
                case 2:
                    _tickDelay = 4;
                    break;
                case 3:
                    _tickDelay = 2;
                    break;
                case 4:
                    _tickDelay = 1;
                    break;
            }

            //_radius = new IRadius(startLocation, endLocation);
        }

        public void OnTick()
        {
#if (DEBUG)
            Console.WriteLine("Tick Start");
#endif

            if (_layerCount >= _maxlayers)
            {
                var cancelToken = new CancelToken();

                var currentloc = player.status.entity.location.ToLocation();


                if (_startLocation.Offset(-1).Distance(currentloc.Offset(-1)) < 1)
                {
                    _busy = true;
                    _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 8);
                    if (_gamemode)
                    {
                        switch (_direction)
                        {
                            case 0:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 8);
                                break;
                            case 1:
                                _nextLocation = new Location(currentloc.x + 8, currentloc.y - 1, currentloc.z);
                                break;
                            case 2:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z + 8);
                                break;
                            case 3:
                                _nextLocation = new Location(currentloc.x - 8, currentloc.y - 1, currentloc.z);
                                break;
                        }
                    }
                    else
                    {
                        switch (_direction)
                        {
                            case 0:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 6);
                                break;
                            case 1:
                                _nextLocation = new Location(currentloc.x + 6, currentloc.y - 1, currentloc.z);
                                break;
                            case 2:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z + 6);
                                break;
                            case 3:
                                _nextLocation = new Location(currentloc.x - 6, currentloc.y - 1, currentloc.z);
                                break;
                        }
                    }
                    _targetblock = actions.AsyncMoveToLocation(_nextLocation, cancelToken, Mo);

#if (DEBUG)
                    Console.WriteLine("_targetblock: " + _targetblock.Target);
#endif
                    _targetblock.Completed += areaMap =>
                    {
                        TaskCompleted();

                        _secondLayerString = false;

                        _secondLayer = true;
                        _firstLayer = false;

                        _secondLayerStringEast = false;
                        _secondLayerStringSouth = false;
                        _secondLayerStringWest = false;
                        _secondLayerStringNorth = false;

                        _firstLayerSand = false;
                        _firstLayerCactus = false;
                        _firstLayerString = false;
                        _secondLayerSand = false;

                        _secondLayerSandEast = false;
                        _firstLayerStringEast = false;
                        _secondLayerSandSouth = false;
                        _firstLayerStringSouth = false;
                        _secondLayerSandWest = false;
                        _firstLayerStringWest = false;
                        _secondLayerSandNorth = false;
                        _firstLayerStringNorth = false;

                        _layerCount = 0;
                    };
                    _targetblock.Cancelled += (areaMap, cuboid) =>
                    {
#if (DEBUG)
                        Console.WriteLine("Cancelled");
#endif
                        if (cancelToken.stopped) return;
                        cancelToken.Stop();
                        InvalidateBlock(_targetblock.Target);
                        TaskCompleted();
                    };

                    if (!_targetblock.Start())
                    {
                        if (cancelToken.stopped) return;
                        cancelToken.Stop();
                        InvalidateBlock(_targetblock.Target);
                        TaskCompleted();
                    }
                    else
                    {
                        _busy = true;
                        return;
                    }
                }

                if (Sandblocks(currentloc))
                {
#if DEBUG
                    Console.WriteLine("Sandblocks(currentloc)");
#endif
                    MineBlock(currentloc.Offset(-1));
                }
                return;
            }
#if DEBUG
            Console.WriteLine("Layercount: " + _layerCount);
#endif

            if (!_startloc)
            {
                _startLocation = player.status.entity.location.ToLocation();
                _startloc = true;
            }

            //TODO: Add wait for world here
            if (!player.physicsEngine.isGrounded) return;

            if (inventory.FindId(12) < 1)
            {
                Console.WriteLine("[Bot: " + player.status.username + "] No sand left");
                return;
            }

            if (inventory.FindId(287) < 1)
            {
                Console.WriteLine("[Bot: " + player.status.username + "] No strings left");
                return;
            }

            if (inventory.FindId(81) < 1)
            {
                Console.WriteLine("[Bot: " + player.status.username + "] No cactus left");
                return;
            }

            if (!_firstLayer)
            {
                if (!_firstLayerSand)
                {
                    MakeFirstLayerSand();
                    return;
                }

                if (!_gamemode)
                {
                    if (_needToMoveToEast)
                    {
                        if (!_secondLayerStringEast)
                        {
                            MakeFirstLayerStringEast();
                            return;
                        }

                        MakeSecondLayerSandEast();
                        return;
                    }

                    if (_needToMoveToSouth)
                    {
                        if (!_secondLayerStringSouth)
                        {
                            MakeFirstLayerStringSouth();
                            return;
                        }

                        MakeSecondLayerSandSouth();
                        return;
                    }

                    if (_needToMoveToWest)
                    {
                        if (!_secondLayerStringWest)
                        {
                            MakeFirstLayerStringWest();
                            return;
                        }
#if DEBUG
                        Console.WriteLine("MakeFirstLayerStringWestGamemode");
#endif

                        MakeSecondLayerSandWest();
                        return;
                    }

                    if (_needToMoveToMiddle)
                    {
                        MoveToMiddleSecondLayer();
                        return;
                    }
                }

                if (!_firstLayerCactus)
                {
                    MakeFirstLayerCactus();
                    return;
                }

                if (!_firstLayerString)
                {
                    MakeFirstLayerString();
                    return;
                }

                if (!_secondLayerSand)
                {
                    MakeSecondLayerSand();
                    return;
                }

                if (_gamemode)
                {
                    if (!_secondLayerSandEast)
                    {
                        if (!_firstLayerStringEast)
                        {
                            MakeFirstLayerStringEastGamemode();
#if DEBUG
                            Console.WriteLine("MakeFirstLayerStringEastGamemode();");
#endif
                            return;
                        }

                        MakeSecondLayerSandEastGamemode();
#if DEBUG
                        Console.WriteLine("MakeSecondLayerSandEastGamemode();");
#endif
                        return;
                    }

                    if (!_secondLayerSandSouth)
                    {
                        if (!_firstLayerStringSouth)
                        {
#if DEBUG
                            Console.WriteLine("MakeFirstLayerStringSouthGamemode();");
#endif
                            MakeFirstLayerStringSouthGamemode();
                            return;
                        }

                        MakeSecondLayerSandSouthGamemode();
#if DEBUG
                        Console.WriteLine("MakeSecondLayerSandSouthGamemode();");
#endif
                        return;
                    }

                    if (!_secondLayerSandWest)
                    {
                        if (!_firstLayerStringWest)
                        {
                            MakeFirstLayerStringWestGamemode();
#if DEBUG
                            Console.WriteLine("MakeFirstLayerStringWestGamemode();");
#endif
                            return;
                        }
                        
                        MakeSecondLayerSandWestGamemode();
#if DEBUG
                        Console.WriteLine("MakeSecondLayerSandWestGamemode");
#endif
                        return;
                    }
#if DEBUG
                    Console.WriteLine("MakeSecondLayerSandWestGamemode");
#endif

                    if (!_secondLayerSandNorth)
                    {
                        if (!_firstLayerStringNorth)
                        {
                            MakeFirstLayerStringNorthGamemode();
                            return;
                        }
#if DEBUG
                        Console.WriteLine("MakeFirstLayerStringNorthGamemode");
#endif

                        MakeSecondLayerSandNorthGamemode();
                        return;
                    }
#if DEBUG
                    Console.WriteLine("MakeSecondLayerSandNorthGamemode");
#endif

                    MoveToMiddleFirstLayerGamemode();
                    return;
                }

                _firstLayer = true;
                _secondLayer = false;

                _secondLayerCactus = false;
                _secondLayerString = false;
            }
            
            if (!_secondLayer)
            {
                if (!_secondLayerCactus)
                {
                    MakeSecondLayerCactus();
                    return;
                }

                if (!_secondLayerString)
                {
                    MakeSecondLayerString();
                    return;
                }
            }

#if (DEBUG)
            Console.WriteLine("Tick End");
#endif
        }

        public override bool Exec()
        {
            //TODO: Add all block type here
            return !status.entity.isDead && !status.eating && !_busy;
        }

        #region First Layer

        #region MakeFirstLayerSand

        private void MakeFirstLayerSand(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (locations.Count == 0 || inventory.FindId(12) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
                    });
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;

                inventory.Select(12); //Select Sand

                player.functions.LookAtBlock(location);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 1);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;
                                
                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeFirstLayerSand()
        {//might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc, cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                
                if (_gamemode)
                {
                    MakeFirstLayerSand(new[]
                        {
                            CheckForBlock(3, -2, 1), CheckForBlock(2, -2, 2), CheckForBlock(1, -2, 3),
                            CheckForBlock(-1, -2, 3), CheckForBlock(-2, -2, 2), CheckForBlock(-3, -2, 1),
                            CheckForBlock(-3, -2, -1), CheckForBlock(-2, -2, -2),
                            CheckForBlock(-1, -2, -3), CheckForBlock(1, -2, -3), CheckForBlock(2, -2, -2),
                            CheckForBlock(3, -2, -1),
                            CheckForBlock(2, -2, 0), CheckForBlock(1, -2, 1), CheckForBlock(0, -2, 2),
                            CheckForBlock(-1, -2, 1),
                            CheckForBlock(-2, -2, 0), CheckForBlock(-1, -2, -1), CheckForBlock(0, -2, -2),
                            CheckForBlock(1, -2, -1)
                        },
                        () =>
                        {
                            _firstLayerSand = true;
                            _secondLayer = true;
                            
#if (DEBUG)
                            Console.WriteLine("MakeFirstLayerSand TaskCompleted();");
#endif
                            TaskCompleted();
                        });
                    return;
                }

                MakeFirstLayerSand(new[]
                    {
                        //CheckForBlock(2, -2, 2), CheckForBlock(-2, -2, 2), CheckForBlock(-2, -2, -2),
                        //CheckForBlock(2, -2, -2),

                        CheckForBlock(2, -2, 0), CheckForBlock(1, -2, 1),
                        CheckForBlock(0, -2, 2), CheckForBlock(-1, -2, 1), CheckForBlock(-2, -2, 0),
                        CheckForBlock(-1, -2, -1), CheckForBlock(0, -2, -2), CheckForBlock(1, -2, -1)
                    },
                    () =>
                    {
                        _firstLayerSand = true;
                        _needToMoveToEast = true;

#if (DEBUG)
                        Console.WriteLine("TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        #endregion

        #region MakeFirstLayerCactus

        private void MakeFirstLayerCactus(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (locations.Count == 0 || inventory.FindId(81) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
                        
                    });
                    return;
                }

                var location = locations.Dequeue();

                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;

                inventory.Select(81); //Select Cactus

                player.functions.LookAtBlock(location);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 1);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;
                                
                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeFirstLayerCactus()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc, cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                
                if (_gamemode)
                {
                    MakeFirstLayerCactus(new[]
                        {
                            CheckForBlock(3, -2, 1), CheckForBlock(2, -2, 2), CheckForBlock(1, -2, 3),
                            CheckForBlock(-1, -2, 3), CheckForBlock(-2, -2, 2), CheckForBlock(-3, -2, 1),
                            CheckForBlock(-3, -2, -1), CheckForBlock(-2, -2, -2),
                            CheckForBlock(-1, -2, -3), CheckForBlock(1, -2, -3), CheckForBlock(2, -2, -2),
                            CheckForBlock(3, -2, -1),
                            CheckForBlock(2, -2, 0), CheckForBlock(1, -2, 1), CheckForBlock(0, -2, 2),
                            CheckForBlock(-1, -2, 1),
                            CheckForBlock(-2, -2, 0), CheckForBlock(-1, -2, -1), CheckForBlock(0, -2, -2),
                            CheckForBlock(1, -2, -1)
                        },
                        () =>
                        {
                            _firstLayerCactus = true;
                            _layerCount++;

#if (DEBUG)
                            Console.WriteLine("MakeFirstLayerCactus TaskCompleted();");
#endif
                            TaskCompleted();
                        });
                    return;
                }

                MakeFirstLayerCactus(new[]
                    {
                        CheckForBlock(2, -2, 2), CheckForBlock(-2, -2, 2), CheckForBlock(-2, -2, -2),
                        CheckForBlock(2, -2, -2), CheckForBlock(2, -2, 0), CheckForBlock(1, -2, 1),
                        CheckForBlock(0, -2, 2), CheckForBlock(-1, -2, 1), CheckForBlock(-2, -2, 0),
                        CheckForBlock(-1, -2, -1), CheckForBlock(0, -2, -2), CheckForBlock(1, -2, -1)
                    },
                    () =>
                    {
                        _firstLayerCactus = true;
                        _layerCount++;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerCactus TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        #endregion

        #region MakeFirstLayerString

        private void MakeFirstLayerString(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(287) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
                    });
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;
                
                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;

                inventory.Select(287); //Select String


                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                player.functions.LookAtBlock(data.location, true, data.face);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(data.location, data.face);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;
                                
                                locations.Enqueue(location);
                                
                            });
                    });
            });
        }

        private void MakeFirstLayerString()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();
            
            if (_gamemode)
            {
                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(3, -1, 0),
                        CheckForBlock(2, -1, -1), CheckForBlock(2, -1, 1), CheckForBlock(1, -1, 0),

                        CheckForBlock(0, -1, 3),
                        CheckForBlock(1, -1, 2), CheckForBlock(-1, -1, 2), CheckForBlock(0, -1, 1),

                        CheckForBlock(-3, -1, 0),
                        CheckForBlock(-2, -1, 1), CheckForBlock(-2, -1, -1), CheckForBlock(-1, -1, 0),

                        CheckForBlock(0, -1, -3),
                        CheckForBlock(-1, -1, -2), CheckForBlock(1, -1, -2), CheckForBlock(0, -1, -1)
                    },
                    () =>
                    {
                        _firstLayerString = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerString TaskCompleted();");
#endif
                        TaskCompleted();
                    });
                return;
            }

            MakeFirstLayerString(new[]
                {
                    CheckForBlock(2, -1, -1), CheckForBlock(2, -1, 1), CheckForBlock(1, -1, 0),
                    
                    CheckForBlock(1, -1, 2), CheckForBlock(-1, -1, 2), CheckForBlock(0, -1, 1),
                    
                    CheckForBlock(-2, -1, 1), CheckForBlock(-2, -1, -1), CheckForBlock(-1, -1, 0),
                    
                    CheckForBlock(-1, -1, -2), CheckForBlock(1, -1, -2), CheckForBlock(0, -1, -1)
                },
                () =>
                {
                    _firstLayerString = true;

#if (DEBUG)
                    Console.WriteLine("MakeFirstLayerString TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSand

        private void MakeSecondLayerSand(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(12) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
#if DEBUG
                        Console.WriteLine("No Sand left");
#endif
                    });
                    return;
                }

                var location = locations.Dequeue();
                
                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;

                inventory.Select(12); //Select String

                player.functions.LookAtBlock(location);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 1);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;
                                
                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeSecondLayerSand()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc, cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                
                if (_gamemode)
                {
                    MakeSecondLayerSand(new[]
                        {
                            CheckForBlock(3, -2, 0), CheckForBlock(2, -2, -1), CheckForBlock(2, -2, 1),
                            CheckForBlock(1, -2, 0),

                            CheckForBlock(0, -2, 3), CheckForBlock(1, -2, 2), CheckForBlock(-1, -2, 2),
                            CheckForBlock(0, -2, 1),

                            CheckForBlock(-3, -2, 0), CheckForBlock(-2, -2, 1), CheckForBlock(-2, -2, -1),
                            CheckForBlock(-1, -2, 0),

                            CheckForBlock(0, -2, -3), CheckForBlock(-1, -2, -2), CheckForBlock(1, -2, -2),
                            CheckForBlock(0, -2, -1)
                        },
                        () =>
                        {
                            _secondLayerSand = true;

#if (DEBUG)
                            Console.WriteLine("MakeSecondLayerSand TaskCompleted();");
#endif
                            TaskCompleted();
                        });
                    return;
                }

                MakeSecondLayerSand(new[]
                    {
                        CheckForBlock(2, -2, -1), CheckForBlock(2, -2, 1), CheckForBlock(1, -2, 0),

                        CheckForBlock(1, -2, 2), CheckForBlock(-1, -2, 2), CheckForBlock(0, -2, 1),

                        CheckForBlock(-2, -2, 1), CheckForBlock(-2, -2, -1), CheckForBlock(-1, -2, 0),

                        CheckForBlock(-1, -2, -2), CheckForBlock(1, -2, -2), CheckForBlock(0, -2, -1)
                    },
                    () =>
                    {
                        _secondLayerSand = true;

#if (DEBUG)
                        Console.WriteLine("MakeSecondLayerSand TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        #endregion

        #region MakeSecondLayerSandEastGamemode

        private void MakeFirstLayerStringEastGamemode()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(3, -1, 0), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("_targetblock: " + _targetblock.Target);
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                    },
                    () =>
                    {
                        _firstLayerStringEast = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringEastGamemode TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                //InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                //InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {

                _busy = true;
            }
        }

        private void MakeSecondLayerSandEastGamemode()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                },
                () =>
                {
                    _secondLayerSandEast = true;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandSouthGamemode

        private void MakeFirstLayerStringSouthGamemode()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(-3, -1, 3), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                    },
                    () =>
                    {
                        _firstLayerStringSouth = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringSouthGamemode TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandSouthGamemode()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                },
                () =>
                {
                    _secondLayerSandSouth = true;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandWestGamemode

        private void MakeFirstLayerStringWestGamemode()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(-3, -1, -3), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                    },
                    () =>
                    {
                        _firstLayerStringWest = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringWestGamemode TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandWestGamemode()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                },
                () =>
                {
                    _secondLayerSandWest = true;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandWestGamemode

        private void MakeFirstLayerStringNorthGamemode()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(3, -1, -3), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(-2, -2, 0), CheckForBlock(2, -2, 0)
                    },
                    () =>
                    {
                        _firstLayerStringNorth = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringNorthGamemode TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandNorthGamemode()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(-2, -2, 0), CheckForBlock(2, -2, 0)
                },
                () =>
                {
                    _secondLayerSandNorth = true;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        #endregion

        #region Move To Middle Gamemode

        private void MoveToMiddleFirstLayerGamemode()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(0, -1, 3), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _firstLayer = true;
                _secondLayer = false;

                _secondLayerCactus = false;
                _secondLayerString = false;

#if DEBUG
                Console.WriteLine("MoveToMiddleFirstLayerGamemode TaskCompleted");
#endif

                TaskCompleted();
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

#endregion
        
#region MakeSecondLayerSandEast

        private void MakeFirstLayerStringEast()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(2, -1, 0), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                    },
                    () =>
                    {
                        _secondLayerStringEast = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringEast TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandEast()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                },
                () =>
                {
                    _needToMoveToSouth = true;
                    _needToMoveToEast = false;

#if (DEBUG)
                    Console.WriteLine("MakeSecondLayerSandEast TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

#endregion

#region MakeSecondLayerSandSouth

        private void MakeFirstLayerStringSouth()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(-2, -1, 2), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                    },
                    () =>
                    {
                        _secondLayerStringSouth = true;
                        
#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringSouth TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandSouth()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                },
                () =>
                {
                    _needToMoveToWest = true;
                    _needToMoveToSouth = false;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

#endregion

#region MakeSecondLayerSandWest

        private void MakeFirstLayerStringWest()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(-2, -1, -2), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                    },
                    () =>
                    {
                        _secondLayerStringWest = true;

#if (DEBUG)
                        Console.WriteLine("MakeFirstLayerStringWest TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

        private void MakeSecondLayerSandWest()
        {
            _busy = true;

            MakeSecondLayerSand(new[]
                {
                    CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                },
                () =>
                {
                    _needToMoveToWest = false;
                    _needToMoveToMiddle = true;

#if (DEBUG)
                    Console.WriteLine("TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

#endregion
        
#region Move To Middle Gamemode

        private void MoveToMiddleSecondLayer()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc.Offset(2, -1, 0), cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _needToMoveToMiddle = false;
#if DEBUG
                Console.WriteLine("MoveToMiddleSecondLayer TaskCompleted");
#endif

                TaskCompleted();
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

#endregion

#endregion
        

        
#region Second Layer

        private void MakeSecondLayerCactus(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(81) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
#if DEBUG
                        Console.WriteLine("No Sand left");
#endif
                    });
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;


                inventory.Select(81); //Select String
                
                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                player.functions.LookAtBlock(data.location, true, data.face);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(data.location, data.face);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;

                                locations.Enqueue(location);

                            });
                    });
            });
        }

        private void MakeSecondLayerCactus()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            MakeSecondLayerCactus(new[]
                {
                    CheckForBlock(3, 0, 0), CheckForBlock(3, 0, 2), CheckForBlock(2, 0, 3),
                    CheckForBlock(0, 0, 3), CheckForBlock(-2, 0, 3), CheckForBlock(-3, 0, 2),
                    CheckForBlock(-3, 0, 0), CheckForBlock(-3, 0, -2), CheckForBlock(-2, 0, -3),
                    CheckForBlock(0, 0, -3), CheckForBlock(2, 0, -3), CheckForBlock(3, 0, -2),

                    CheckForBlock(1, 0, 2), CheckForBlock(2, 0, 1),
                    CheckForBlock(-1, 0, 2), CheckForBlock(-2, 0, 1),
                    CheckForBlock(-2, 0, -1), CheckForBlock(-1, 0, -2),
                    CheckForBlock(1, 0, -2), CheckForBlock(2, 0, -1)
                },
                () =>
                {
                    _secondLayerCactus = true;
                    _layerCount++;

#if (DEBUG)
                    Console.WriteLine("MakeSecondLayerCactus TaskCompleted();");
#endif
                    TaskCompleted();
                });
        }

        private void MakeSecondLayerString(ILocation[] precomputed, Action callback)
        {
            foreach (var location in precomputed)
            {
                if (location == null)
                {
                }
            }

            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(81) < 1)
                {
                    stopToken.Stop();

                    player.tickManager.Register(4, () =>
                    {
                        // Wait 4 more ticks to fire callback, as it could be possible that we are waiting for the look event.
                        callback();
#if DEBUG
                        Console.WriteLine("No Sand left");
#endif
                    });
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;

                inventory.Select(287); //Select String
                
                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                player.functions.LookAtBlock(data.location, true, data.face);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(data.location, data.face);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;

                                locations.Enqueue(location);

                            });
                    });
            });
        }

        private void MakeSecondLayerString()
        {
            //might need to change it to a stop token
            var cancelToken = new CancelToken();

            var currentloc = player.status.entity.location.ToLocation();

            _busy = true;

            _targetblock = actions.AsyncMoveToLocation(currentloc, cancelToken, Mo);

#if (DEBUG)
            Console.WriteLine("success");
#endif
            _targetblock.Completed += areaMap =>
            {
#if (DEBUG)
                Console.WriteLine("map complete");
#endif
                _currentbuildingloc = player.status.entity.location.ToLocation();
                
                if (_gamemode)
                {
                    MakeSecondLayerString(new[]
                    {
                        CheckForBlock(3, -1, 1), CheckForBlock(2, -1, 2), CheckForBlock(1, -1, 3),

                        CheckForBlock(-1, -1, 3), CheckForBlock(-2, -1, 2), CheckForBlock(-3, -1, 1),

                        CheckForBlock(-3, -1, -1), CheckForBlock(-2, -1, -2), CheckForBlock(-1, -1, -3),

                        CheckForBlock(1, -1, -3), CheckForBlock(2, -1, -2), CheckForBlock(3, -1, -1),

                        CheckForBlock(2, -1, 0), CheckForBlock(1, -1, 1),
                        CheckForBlock(0, -1, 2), CheckForBlock(-1, -1, 1),
                        CheckForBlock(-2, -1, 0), CheckForBlock(-1, -1, -1),
                        CheckForBlock(0, -1, -2), CheckForBlock(1, -1, -1)
                    },
                    () =>
                    {
                        _secondLayerString = false;

                        _secondLayer = true;
                        _firstLayer = false;

                        _secondLayerStringEast = false;
                        _secondLayerStringSouth = false;
                        _secondLayerStringWest = false;
                        _secondLayerStringNorth = false;

                        _firstLayerSand = false;
                        _firstLayerCactus = false;
                        _firstLayerString = false;
                        _secondLayerSand = false;

                        _secondLayerSandEast = false;
                        _firstLayerStringEast = false;
                        _secondLayerSandSouth = false;
                        _firstLayerStringSouth = false;
                        _secondLayerSandWest = false;
                        _firstLayerStringWest = false;
                        _secondLayerSandNorth = false;
                        _firstLayerStringNorth = false;

#if (DEBUG)
                        Console.WriteLine("MakeSecondLayerString TaskCompleted();");
#endif
                        TaskCompleted();
                    });
                    return;
                }
                
                MakeSecondLayerString(new[]
                    {
                        CheckForBlock(2, -1, 0), CheckForBlock(1, -1, 1), CheckForBlock(0, -1, 2),
                        CheckForBlock(-1, -1, 1), CheckForBlock(-2, -1, 0), CheckForBlock(-1, -1, -1),
                        CheckForBlock(0, -1, -2), CheckForBlock(1, -1, -1)
                    },
                    () =>
                    {
                        _secondLayerString = false;

                        _secondLayer = true;
                        _firstLayer = false;

                        _secondLayerStringEast = false;
                        _secondLayerStringSouth = false;
                        _secondLayerStringWest = false;
                        _secondLayerStringNorth = false;

                        _firstLayerSand = false;
                        _firstLayerCactus = false;
                        _firstLayerString = false;
                        _secondLayerSand = false;

                        _secondLayerSandEast = false;
                        _firstLayerStringEast = false;
                        _secondLayerSandSouth = false;
                        _firstLayerStringSouth = false;
                        _secondLayerSandWest = false;
                        _firstLayerStringWest = false;
                        _secondLayerSandNorth = false;
                        _firstLayerStringNorth = false;

#if (DEBUG)
                        Console.WriteLine("TaskCompleted();");
#endif
                        TaskCompleted();
                    });
            };
            _targetblock.Cancelled += (areaMap, cuboid) =>
            {
#if (DEBUG)
                Console.WriteLine("Cancelled");
#endif
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            };

            if (!_targetblock.Start())
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(_targetblock.Target);
                TaskCompleted();
            }
            else
            {
                _busy = true;
            }
        }

#endregion



#region Functions

        private IStopToken RegisterReoccuring(int ticks, Action callback, IStopToken token = null)
        {
            IStopToken tempToken = token ?? new CancelToken(); // Or CancelToken();, not sure which one you can access.
            player.tickManager.Register(ticks, () => {
                if (tempToken.stopped) return;

                callback();
                RegisterReoccuring(ticks, callback, tempToken);
            });
            return tempToken;
        }

        private bool Sandblocks(ILocation current)
        {
            return player.world.GetBlockId(current.Offset(-1)) == 12;
        }

        private void MineBlock(ILocation location)
        {
            _busy = true;
            player.tickManager.Register(3, () =>
            {
                actions.LookAtBlock(location, true);
                actions.SelectBestTool(location);
                player.tickManager.Register(1, () => { actions.BlockDig(location, action => { _busy = false; }); });
            });
        }

        private ILocation CheckForBlock(int x, float y, int z)
        {
            return player.world.GetBlockId(_currentbuildingloc.Offset(x, y + 1, z)) == 0 ? _currentbuildingloc.Offset(x, y, z) : null;
        }

        private void TaskCompleted()
        {
            _busy = false;
        }

        private static void InvalidateBlock(ILocation location)
        {
            if (location != null) Broken.TryAdd(location, DateTime.Now);
        }

#endregion
    }
}