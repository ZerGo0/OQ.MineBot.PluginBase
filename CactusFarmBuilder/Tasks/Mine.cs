using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
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

        private readonly int _direction;

        private readonly int _maxlayers;
        private readonly bool _gamemode;
        private readonly int _tickDelay;
        private readonly bool _linearmode;
        private readonly bool _vanillamode;
        private bool _linearmodepos;

        private bool _busy;

        private ILocation _currentbuildingloc;

        private bool _vanillaFirstLayerSand;
        private bool _vanillaFirstLayerCactus;
        private bool _vanillaFirstLayerString;

        private bool _vanillaSecondLayerSand;
        private bool _vanillaSecondLayerString;
        private bool _vanillaSecondLayerGoToFirst;

        private bool _firstLayer;

        private bool _firstLayerCactus;
        private bool _firstLayerSand;
        private bool _firstLayerString;
        private bool _firstLayerStringEast;
        private bool _firstLayerStringNorth;
        private bool _firstLayerStringSouth;
        private bool _firstLayerStringWest;
        private bool _firstLayerCactusOutside;
        private bool _firstLayerStringOutside;
        private bool _firstLayerCactusInside;
        private bool _firstLayerStringInside;

        private bool _activateLayerCount;
        private int _layerCount;
        private bool _needToMoveToEast;
        private bool _needToMoveToMiddle;
        private bool _needToMoveToSouth;
        private bool _needToMoveToWest;
        private ILocation _nextLocation;
        
        private bool _secondLayer;

        private bool _secondLayerCactus;
        private bool _secondLayerSand;
        private bool _secondLayerSandEast;
        private bool _secondLayerSandNorth;
        private bool _secondLayerSandSouth;
        private bool _secondLayerSandWest;
        private bool _secondLayerString;


        private bool _secondLayerStringEast;
        private bool _secondLayerStringSouth;
        private bool _secondLayerStringWest;
        private bool _startloc;
        private ILocation _startLocation;

        private bool _stopped;

        public CactusFarmBuilder(int speedmode, bool gamemode, int maxlayers, int direction, bool linearmode, bool vanillamode)
        {
            _gamemode = gamemode;
            _maxlayers = maxlayers;
            _direction = direction;
            _linearmode = linearmode;
            _vanillamode = vanillamode;

            //_fillermacro = fillerMacro;

            var blocks = new[] {(ushort)12};

            BlocksGlobal.BUILDING_BLOCKS = blocks;

            switch (speedmode)
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
        }

        public override void Stop()
        {
            _stopped = true;
        }

        public void OnTick()
        {
            DebugMessage("Tick START");
            
            //TODO: Add Inv-Management
            //TODO: When height is an uneven number then don't build cactus on the top layer
            //TODO: Add wait for world here

            if (!player.physicsEngine.isGrounded) return;
            if (_stopped) return;


            if (inventory.FindId(12) < 1)
            {
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(36, SlotType.Create(12, 64)); 
                }
                else
                {
                    Console.WriteLine("[Bot: " + player.status.username + "] No sand left");
                    return;
                }
            }

            if (inventory.FindId(287) < 1)
            {
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(37, SlotType.Create(287, 64)); 
                }
                else
                {
                    Console.WriteLine("[Bot: " + player.status.username + "] No strings left");
                    return;
                }
            }

            if (inventory.FindId(81) < 1)
            {
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create(81, 64)); 
                }
                else
                {
                    Console.WriteLine("[Bot: " + player.status.username + "] No cactus left");
                    return;
                }
            }

            DebugMessage("Layercount: " + _layerCount);

            if (_layerCount >= _maxlayers)
            {
                if (_vanillamode)
                {
                    var currentloc = player.status.entity.location.ToLocation();

                    if (_startLocation.Offset(-1).Distance(currentloc.Offset(-1)) < 1)
                    {
                        _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 8);

                        switch (_direction)
                        {
                            case 0:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 2);
                                break;
                            case 1:
                                _nextLocation = new Location(currentloc.x + 2, currentloc.y - 1, currentloc.z);
                                break;
                            case 2:
                                _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z + 2);
                                break;
                            case 3:
                                _nextLocation = new Location(currentloc.x - 2, currentloc.y - 1, currentloc.z);
                                break;
                        }

                        GoToLocation(_nextLocation, () =>
                        {
                            WaitGrounded(() =>
                            {
                                TaskCompleted();

                                _firstLayer = false;
                                _secondLayer = true;

                                _vanillaFirstLayerSand = false;
                                _vanillaFirstLayerCactus = false;
                                _vanillaFirstLayerString = false;

                                _layerCount = 0;
                                _startloc = false;
                                _activateLayerCount = false;
                            });
                        });
                    }

                    WaitGrounded(() =>
                    {
                        if (!Sandblocks(currentloc)) return;

                        DebugMessage("Sandblocks(currentloc)");

                        var location = player.status.entity.location.ToLocation();

                        if (player.world.GetBlockId(location.x, (int)location.y - 1, location.z + 1) == 12 &&
                            player.world.GetBlockId(location.x, (int)location.y, location.z + 1) == 0 ||
                            player.world.GetBlockId(location.x, (int)location.y - 1, location.z - 1) == 12 &&
                            player.world.GetBlockId(location.x, (int)location.y, location.z - 1) == 0)
                        {
                            _currentbuildingloc = player.status.entity.location.ToLocation();

                            PlaceBlockData(81, new[]
                                {
                                    CheckForBlock(0, 0, 1),
                                    CheckForBlock(0, 0, -1)
                                },
                                () =>
                                {
                                    DebugMessage("Place Cactus TaskCompleted();");

                                    TaskCompleted();
                                });
                            return;
                        }

                        MineBlock(currentloc.Offset(-1));
                    });
                    return;
                }
                else
                {
                    var currentloc = player.status.entity.location.ToLocation();

                    if (_startLocation.Offset(-1).Distance(currentloc.Offset(-1)) < 1)
                    {
                        _nextLocation = new Location(currentloc.x, currentloc.y - 1, currentloc.z - 8);
                        if (_gamemode)
                        {
                            if (_linearmode)
                            {
                                if (!_linearmodepos)
                                {
                                    switch (_direction)
                                    {
                                        case 0:
                                            _nextLocation = new Location(currentloc.x + 1, currentloc.y - 1,
                                                currentloc.z - 7);
                                            break;
                                        case 1:
                                            _nextLocation = new Location(currentloc.x + 7, currentloc.y - 1,
                                                currentloc.z - 1);
                                            break;
                                        case 2:
                                            _nextLocation = new Location(currentloc.x - 1, currentloc.y - 1,
                                                currentloc.z + 7);
                                            break;
                                        case 3:
                                            _nextLocation = new Location(currentloc.x - 7, currentloc.y - 1,
                                                currentloc.z + 1);
                                            break;
                                    }

                                    _linearmodepos = true;
                                }
                                else
                                {
                                    switch (_direction)
                                    {
                                        case 0:
                                            _nextLocation = new Location(currentloc.x - 1, currentloc.y - 1,
                                                currentloc.z - 7);
                                            break;
                                        case 1:
                                            _nextLocation = new Location(currentloc.x + 7, currentloc.y - 1,
                                                currentloc.z + 1);
                                            break;
                                        case 2:
                                            _nextLocation = new Location(currentloc.x + 1, currentloc.y - 1,
                                                currentloc.z + 7);
                                            break;
                                        case 3:
                                            _nextLocation = new Location(currentloc.x - 7, currentloc.y - 1,
                                                currentloc.z - 1);
                                            break;
                                    }

                                    _linearmodepos = false;
                                }
                            }
                            else
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
                        }
                        else
                        {
                            if (_linearmode)
                            {
                                switch (_direction)
                                {
                                    case 0:
                                        _nextLocation = new Location(currentloc.x, currentloc.y - 1,
                                            currentloc.z - 4);
                                        break;
                                    case 1:
                                        _nextLocation = new Location(currentloc.x + 4, currentloc.y - 1,
                                            currentloc.z);
                                        break;
                                    case 2:
                                        _nextLocation = new Location(currentloc.x, currentloc.y - 1,
                                            currentloc.z + 4);
                                        break;
                                    case 3:
                                        _nextLocation = new Location(currentloc.x - 4, currentloc.y - 1,
                                            currentloc.z);
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
                        }

                        GoToLocation(_nextLocation, () =>
                        {
                            WaitGrounded(() =>
                            {
                                TaskCompleted();

                                _secondLayerString = false;

                                _secondLayer = true;
                                _firstLayer = false;

                                _secondLayerStringEast = false;
                                _secondLayerStringSouth = false;
                                _secondLayerStringWest = false;

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

                                _firstLayerCactusOutside = false;
                                _firstLayerStringOutside = false;
                                _firstLayerCactusInside = false;
                                _firstLayerStringInside = false;

                                _layerCount = 0;
                                _startloc = false;
                                _activateLayerCount = false;
                            });
                        });
                    }
                    
                    if (currentloc.x != _startLocation.x || currentloc.z != _startLocation.z)
                    {
                        GoToLocation(new Location(_startLocation.x, currentloc.y - 1, _startLocation.z), TaskCompleted);
                        return;
                    }

                    WaitGrounded(() =>
                    {
                        if (!Sandblocks(currentloc)) return;

                        DebugMessage("Sandblocks(currentloc)");

                        var location = player.status.entity.location.ToLocation();

                        if (player.world.GetBlockId(location.x, (int) location.y - 1, location.z + 1) == 12 &&
                            player.world.GetBlockId(location.x, (int) location.y, location.z + 1) == 0 ||
                            player.world.GetBlockId(location.x, (int) location.y - 1, location.z - 1) == 12 &&
                            player.world.GetBlockId(location.x, (int) location.y, location.z - 1) == 0 ||
                            player.world.GetBlockId(location.x + 1, (int) location.y - 1, location.z) == 12 &&
                            player.world.GetBlockId(location.x + 1, (int) location.y, location.z) == 0 ||
                            player.world.GetBlockId(location.x - 1, (int) location.y - 1, location.z) == 12 &&
                            player.world.GetBlockId(location.x - 1, (int) location.y, location.z) == 0)
                        {
                            _currentbuildingloc = player.status.entity.location.ToLocation();

                            PlaceBlockData(81, new[]
                                {
                                    CheckForBlock(0, 0, 1),
                                    CheckForBlock(0, 0, -1),
                                    CheckForBlock(1, 0, 0),
                                    CheckForBlock(-1, 0, 0)
                                },
                                () =>
                                {
                                    _firstLayerCactusOutside = true;

                                    DebugMessage("Place Cactus TaskCompleted();");

                                    TaskCompleted();
                                });
                            return;
                        }

                        MineBlock(currentloc.Offset(-1));
                    });
                    return;
                }
            }

            if (!_startloc)
            {
                _startLocation = player.status.entity.location.ToLocation();
                _startloc = true;
            }

            if (_vanillamode)
            {
                if (!_firstLayer)
                {
                    if (!_vanillaFirstLayerSand)
                    {
                        MakeVanillaFirstLayerSand();
                        return;
                    }
                    
                    if (!_vanillaFirstLayerCactus)
                    {
                        MakeVanillaFirstLayerCactus();
                        return;
                    }
                    
                    if (!_vanillaFirstLayerString)
                    {
                        MakeVanillaFirstLayerString();
                        return;
                    }

                    _firstLayer = true;
                    _secondLayer = false;

                    _vanillaSecondLayerSand = false;
                    _vanillaSecondLayerString = false;
                    _vanillaSecondLayerGoToFirst = false;
                    _layerCount++;
                }

                if (!_secondLayer)
                {
                    if (!_vanillaSecondLayerSand)
                    {
                        MakeVanillaSecondLayerSand();
                        return;
                    }
                
                    if (!_vanillaSecondLayerString)
                    {
                        MakeVanillaSecondLayerString();
                        return;
                    }

                    if (!_vanillaSecondLayerGoToFirst)
                    {
                        VanillaGoToFirstLayer();
                        return;
                    }

                    _firstLayer = false;
                    _secondLayer = true;

                    _vanillaFirstLayerSand = false;
                    _vanillaFirstLayerCactus = false;
                    _vanillaFirstLayerString = false;
                    _layerCount++;
                }
            }
            else
            {
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

                            MakeSecondLayerSandWest();
                            return;
                        }

                        if (_needToMoveToMiddle)
                        {
                            MoveToMiddleSecondLayer();
                            return;
                        }
                    }
                    
                    

                    if (!_firstLayerCactus && _gamemode)
                    {
                        MakeFirstLayerCactusGamemode();
                        return;
                    }

                    if (!_firstLayerCactus && !_gamemode)
                    {
                        if (!_firstLayerCactusOutside)
                        {
                            MakeFirstLayerCactusOutside();
                            return;
                        }

                        if (!_firstLayerStringOutside)
                        {
                            MakeFirstLayerStringOutside();
                            return;
                        }

                        if (!_firstLayerCactusInside)
                        {
                            MakeFirstLayerCactusInside();
                            return;
                        }

                        if (!_firstLayerStringInside)
                        {
                            MakeFirstLayerStringInside();
                            return;
                        }
                    }

                    if (!_firstLayerString && _gamemode)
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
                                DebugMessage("MakeFirstLayerStringEastGamemode();");
                                return;
                            }

                            MakeSecondLayerSandEastGamemode();
                            DebugMessage("MakeSecondLayerSandEastGamemode();");
                            return;
                        }

                        if (!_secondLayerSandSouth)
                        {
                            if (!_firstLayerStringSouth)
                            {
                                DebugMessage("MakeFirstLayerStringSouthGamemode();");
                                MakeFirstLayerStringSouthGamemode();
                                return;
                            }

                            MakeSecondLayerSandSouthGamemode();
                            DebugMessage("MakeSecondLayerSandSouthGamemode();");
                            return;
                        }

                        if (!_secondLayerSandWest)
                        {
                            if (!_firstLayerStringWest)
                            {
                                MakeFirstLayerStringWestGamemode();
                                DebugMessage("MakeFirstLayerStringWestGamemode();");
                                return;
                            }

                            MakeSecondLayerSandWestGamemode();
                            DebugMessage("MakeSecondLayerSandWestGamemode");
                            return;
                        }

                        DebugMessage("MakeSecondLayerSandWestGamemode");

                        if (!_secondLayerSandNorth)
                        {
                            if (!_firstLayerStringNorth)
                            {
                                MakeFirstLayerStringNorthGamemode();
                                return;
                            }

                            DebugMessage("MakeFirstLayerStringNorthGamemode");

                            MakeSecondLayerSandNorthGamemode();
                            return;
                        }

                        DebugMessage("MakeSecondLayerSandNorthGamemode");

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
            }

            DebugMessage("Tick END");
        }

        public override bool Exec()
        {
            //TODO: Add all block type here
            return !status.entity.isDead && !status.eating && !_busy;
        }

        #region Vanilla Mode

        #region Vanilla First Layer

        private void MakeVanillaFirstLayerSand()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(1, -1, 1), CheckForBlock(-1, -1, 1), CheckForBlock(-1, -1, -1),
                    CheckForBlock(1, -1, -1)
                },
                () =>
                {
                    _vanillaFirstLayerSand = true;

                    DebugMessage("MakeVanillaFirstLayerSand TaskCompleted();");

                    TaskCompleted();
                });
        }

        private void MakeVanillaFirstLayerCactus()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockData(81, new[]
                {
                    CheckForBlock(1, 1, 1), CheckForBlock(-1, 1, 1), CheckForBlock(-1, 1, -1),
                    CheckForBlock(1, 1, -1),
                },
                () =>
                {
                    _vanillaFirstLayerCactus = true;

                    DebugMessage("MakeVanillaFirstLayerCactus TaskCompleted();");

                    TaskCompleted();
                });
        }

        private void MakeVanillaFirstLayerString()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockData(287, new[]
                {
                    CheckForBlock(0, 1, 1), CheckForBlock(0, 1, -1)
                },
                () =>
                {
                    _vanillaFirstLayerString = true;

                    DebugMessage("MakeVanillaFirstLayerString TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region Vanilla Second Layer

        private void MakeVanillaSecondLayerSand()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(1), () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    PlaceBlockLocation(12, new[]
                        {
                            CheckForBlock(0, -1, 1),CheckForBlock(0, -1, -1)
                        },
                        () =>
                        {
                            _vanillaSecondLayerSand = true;

                            DebugMessage("MakeVanillaSecondLayerSand TaskCompleted();");

                            TaskCompleted();
                        });
                });
            });
        }

        private void MakeVanillaSecondLayerString()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockLocation(287, new[]
                {
                    CheckForBlock(1, -1, 1), CheckForBlock(1, 0, 1),
                    CheckForBlock(-1, -1, 1), CheckForBlock(-1, 0, 1),
                    CheckForBlock(-1, -1, -1), CheckForBlock(-1, 0, -1),
                    CheckForBlock(1, -1, -1), CheckForBlock(1, 0, -1)
                },
                () =>
                {
                    MineBlockNoBusy(_currentbuildingloc.Offset(1, 0, 1));
                    player.tickManager.Register(_tickDelay, () =>
                    {
                        MineBlockNoBusy(_currentbuildingloc.Offset(-1, 0, 1));
                        player.tickManager.Register(_tickDelay, () =>
                        {
                            MineBlockNoBusy(_currentbuildingloc.Offset(-1, 0, -1));
                            player.tickManager.Register(_tickDelay, () =>
                            {
                                MineBlockNoBusy(_currentbuildingloc.Offset(1, 0, -1));

                                _vanillaSecondLayerString = true;

                                DebugMessage("MakeVanillaSecondLayerString TaskCompleted();");

                                TaskCompleted();
                            });
                        });
                    });
                });
        }

        private void VanillaGoToFirstLayer()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(1), () =>
            {
                WaitGrounded(() =>
                {
                    _vanillaSecondLayerGoToFirst = true;

                    TaskCompleted();
                });
            });
        }

        #endregion

        #endregion

        #region First Layer

        private void PlaceBlockData(ushort blockid, IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(39, SlotType.Create((short)blockid, 64)); 
                }
                
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(blockid) < 1 && player.status.entity.gamemode != Gamemodes.creative || _stopped)
                {
                    stopToken.Stop();

                    player.tickManager.Register(2, callback);
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y, location.z) != 0) return;
                
                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                inventory.Select(blockid);

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

        private void PlaceBlockLocation(ushort blockid, IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(39, SlotType.Create((short)blockid, 64)); 
                }
                
                if (locations.Count == 0 || inventory.FindId(blockid) < 1 && player.status.entity.gamemode != Gamemodes.creative || _stopped)
                {
                    stopToken.Stop();

                    player.tickManager.Register(2, callback);
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int)location.y + 1, location.z) != 0) return;

                inventory.Select(blockid);
                
                player.functions.LookAtBlock(location, true, 1);

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

        #region MakeFirstLayerSand

        private void MakeFirstLayerSand()
        {
            var currentloc = player.status.entity.location.ToLocation();
            
            GoToLocation(currentloc, () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();
                    
                    if (_gamemode)
                    {
                        PlaceBlockLocation(12, new[]
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

                                if (_activateLayerCount) _layerCount++;

                                DebugMessage("MakeFirstLayerSand TaskCompleted();");

                                TaskCompleted();
                            });
                        return;
                    }

                    PlaceBlockLocation(12, new[]
                        {
                            //CheckForBlock(2, -2, 2), CheckForBlock(-2, -2, 2), CheckForBlock(-2, -2, -2),
                            //CheckForBlock(2, -2, -2),

                            CheckForBlock(1, -2, 1),
                            CheckForBlock(0, -2, 2), CheckForBlock(-1, -2, 1), CheckForBlock(-2, -2, 0),
                            CheckForBlock(-1, -2, -1), CheckForBlock(0, -2, -2), CheckForBlock(1, -2, -1),
                            CheckForBlock(2, -2, 0)
                        },
                        () =>
                        {
                            _firstLayerSand = true;
                            _needToMoveToEast = true;

                            DebugMessage("TaskCompleted();");
                            
                            TaskCompleted();
                        });
                });
            });
        }

        #endregion

        #region MakeFirstLayerCactus

        private void MakeFirstLayerCactusGamemode()
        {
            var currentloc = player.status.entity.location.ToLocation();
            
            GoToLocation(currentloc, () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    PlaceBlockData(81, new[]
                    {
                        CheckForBlock(3, -1, 1), CheckForBlock(2, -1, 2), CheckForBlock(1, -1, 3),
                        CheckForBlock(-1, -1, 3), CheckForBlock(-2, -1, 2), CheckForBlock(-3, -1, 1),
                        CheckForBlock(-3, -1, -1), CheckForBlock(-2, -1, -2),
                        CheckForBlock(-1, -1, -3), CheckForBlock(1, -1, -3), CheckForBlock(2, -1, -2),
                        CheckForBlock(3, -1, -1),
                        CheckForBlock(2, -1, 0), CheckForBlock(1, -1, 1), CheckForBlock(0, -1, 2),
                        CheckForBlock(-1, -1, 1),
                        CheckForBlock(-2, -1, 0), CheckForBlock(-1, -1, -1), CheckForBlock(0, -1, -2),
                        CheckForBlock(1, -1, -1)
                    },
                    () =>
                    {
                        _firstLayerCactus = true;

                        DebugMessage("MakeFirstLayerCactusGamemode TaskCompleted();");

                        TaskCompleted();
                    });
                });
            });
        }

        private void MakeFirstLayerCactusOutside()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc, () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    PlaceBlockData(81, new[]
                        {
                            CheckForBlock(2, -1, 2), CheckForBlock(-2, -1, 2), CheckForBlock(-2, -1, -2),
                            CheckForBlock(2, -1, -2)
                        },
                        () =>
                        {
                            _firstLayerCactusOutside = true;

                            DebugMessage("MakeFirstLayerCactusOutside TaskCompleted();");

                            TaskCompleted();
                        });
                });
            });
        }
        
        private void MakeFirstLayerCactusInside()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockData(81, new[]
            {
                CheckForBlock(2, -1, 0), CheckForBlock(1, -1, 1),
                CheckForBlock(0, -1, 2), CheckForBlock(-1, -1, 1), CheckForBlock(-2, -1, 0),
                CheckForBlock(-1, -1, -1), CheckForBlock(0, -1, -2), CheckForBlock(1, -1, -1)
            },
            () =>
            {
                _firstLayerCactusInside = true;

                DebugMessage("MakeFirstLayerCactusInside TaskCompleted();");

                TaskCompleted();
            });
        }

        #endregion

        #region MakeFirstLayerString

        private void MakeFirstLayerString(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(37, SlotType.Create(287, 64)); 
                }
                
                if (locations.Count == 0 || inventory.FindId(287) < 1 || _stopped)
                {
                    stopToken.Stop();

                    player.tickManager.Register(2, callback);
                    return;
                }

                var location = locations.Dequeue();

                if (location == null) return;

                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                inventory.Select(287); //Select String

                player.functions.LookAtBlock(data.location, true, data.face);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(data.location, data.face);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeFirstLayerString()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();
            
            PlaceBlockData(287, new[]
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

                DebugMessage("MakeFirstLayerString TaskCompleted();");

                TaskCompleted();
            });
        }

        private void MakeFirstLayerStringOutside()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();
            
            PlaceBlockData(287, new[]
                {
                    CheckForBlock(2, -1, -1), CheckForBlock(2, -1, 1),

                    CheckForBlock(1, -1, 2), CheckForBlock(-1, -1, 2),

                    CheckForBlock(-2, -1, 1), CheckForBlock(-2, -1, -1),

                    CheckForBlock(-1, -1, -2), CheckForBlock(1, -1, -2)
                },
                () =>
                {
                    _firstLayerStringOutside = true;

                    DebugMessage("MakeFirstLayerStringOutside TaskCompleted();");

                    TaskCompleted();
                });
        }

        private void MakeFirstLayerStringInside()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            PlaceBlockData(287, new[]
                {
                    CheckForBlock(1, -1, 0), CheckForBlock(0, -1, 1), CheckForBlock(-1, -1, 0),
                    CheckForBlock(0, -1, -1)
                },
                () =>
                {
                    _firstLayerStringInside = true;
                    _firstLayerCactus = true;

                    DebugMessage("MakeFirstLayerStringInside TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSand

        private void MakeSecondLayerSand(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(36, SlotType.Create((short)12, 64)); 
                }
                
                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(12) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x, (int) location.y + 1, location.z) != 0) return;

                inventory.Select(12); //Select String

                player.functions.LookAtBlock(location);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 1);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int) location.y + 1, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeSecondLayerSand()
        {
            var currentloc = player.status.entity.location.ToLocation();
            
            GoToLocation(currentloc, () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();
                    
                    if (_gamemode)
                    {
                        PlaceBlockLocation(12, new[]
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

                                _layerCount++;

                                if (!_activateLayerCount) _activateLayerCount = true;

                                DebugMessage("MakeSecondLayerSand TaskCompleted();");

                                TaskCompleted();
                            });
                        return;
                    }

                    PlaceBlockLocation(12, new[]
                        {
                            CheckForBlock(2, -2, -1), CheckForBlock(2, -2, 1), CheckForBlock(1, -2, 0),

                            CheckForBlock(1, -2, 2), CheckForBlock(-1, -2, 2), CheckForBlock(0, -2, 1),

                            CheckForBlock(-2, -2, 1), CheckForBlock(-2, -2, -1), CheckForBlock(-1, -2, 0),

                            CheckForBlock(-1, -2, -2), CheckForBlock(1, -2, -2), CheckForBlock(0, -2, -1)
                        },
                        () =>
                        {
                            _secondLayerSand = true;

                            _layerCount++;

                            if (!_activateLayerCount) _activateLayerCount = true;
                            
                            DebugMessage("MakeSecondLayerSand TaskCompleted();");
                            
                            TaskCompleted();
                        });
                });
            });
        }

        #endregion

        #region MakeSecondLayerSandEastGamemode

        private void MakeFirstLayerStringEastGamemode()
        {
            var currentloc = player.status.entity.location.ToLocation();
            
            GoToLocation(currentloc.Offset(3, -1, 0), () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    PlaceBlockData(287, new[]
                        {
                            CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                        },
                        () =>
                        {
                            _firstLayerStringEast = true;

                            DebugMessage("MakeFirstLayerStringEastGamemode TaskCompleted();");

                            TaskCompleted();
                        });
                });
            });
        }

        private void MakeSecondLayerSandEastGamemode()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(0, -2, -2), CheckForBlock(0, -2, 2)
                },
                () =>
                {
                    _secondLayerSandEast = true;

                    DebugMessage("MakeSecondLayerSandEastGamemode TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandSouthGamemode

        private void MakeFirstLayerStringSouthGamemode()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(-3, -1, 3), () =>
            {
                _currentbuildingloc = player.status.entity.location.ToLocation();

                PlaceBlockData(287, new[]
                    {
                        CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                    },
                    () =>
                    {
                        _firstLayerStringSouth = true;

                        DebugMessage("MakeFirstLayerStringSouthGamemode TaskCompleted();");

                        TaskCompleted();
                    });
            });
        }

        private void MakeSecondLayerSandSouthGamemode()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                },
                () =>
                {
                    _secondLayerSandSouth = true;

                    DebugMessage("MakeSecondLayerSandSouthGamemode TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandWestGamemode

        private void MakeFirstLayerStringWestGamemode()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(-3, -1, -3), () =>
            {
                _currentbuildingloc = player.status.entity.location.ToLocation();

                MakeFirstLayerString(new[]
                    {
                        CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                    },
                    () =>
                    {
                        _firstLayerStringWest = true;

                        DebugMessage("MakeFirstLayerStringWestGamemode TaskCompleted();");

                        TaskCompleted();
                    });
            });
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

                    DebugMessage("MakeSecondLayerSandWestGamemode TaskCompleted();");

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

            //actions.AsyncMoveToLocation(currentloc.Offset(3, -1, -3), cancelToken, Mo);

            GoToLocation(currentloc.Offset(3, -1, -3), () =>
            {
                _currentbuildingloc = player.status.entity.location.ToLocation();

                PlaceBlockData(287, new[]
                    {
                        CheckForBlock(-2, -2, 0), CheckForBlock(2, -2, 0)
                    },
                    () =>
                    {
                        _firstLayerStringNorth = true;

                        DebugMessage("MakeFirstLayerStringNorthGamemode TaskCompleted();");

                        TaskCompleted();
                    });
            });
        }

        private void MakeSecondLayerSandNorthGamemode()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(-2, -2, 0), CheckForBlock(2, -2, 0)
                },
                () =>
                {
                    _secondLayerSandNorth = true;

                    DebugMessage("MakeSecondLayerSandNorthGamemode TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region Move To Middle Gamemode

        private void MoveToMiddleFirstLayerGamemode()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(0, -1, 3), () =>
            {
                _firstLayer = true;
                _secondLayer = false;

                _secondLayerCactus = false;
                _secondLayerString = false;

                DebugMessage("MoveToMiddleFirstLayerGamemode TaskCompleted");

                TaskCompleted();
            });
        }

        #endregion

        #region First Layer Strings

        private void MakeFirstLayerStringEast(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create(81, 64)); 
                }
                
                if (locations.Count == 0 || inventory.FindId(81) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x + 1, (int)location.y, location.z) != 0) return;

                inventory.Select(287);

                player.functions.LookAtBlock(location, true, 5);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 5);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x + 1, (int)location.y, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeFirstLayerStringSouth(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create((short)81, 64)); 
                }
                
                if (locations.Count == 0 || inventory.FindId(81) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x, (int)location.y, location.z + 1) != 0) return;

                inventory.Select(287); //Select Cactus

                player.functions.LookAtBlock(location, true, 3);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 3);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int)location.y, location.z + 1) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeFirstLayerStringWest(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create((short)81, 64)); 
                }

                if (locations.Count == 0 || inventory.FindId(81) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x - 1, (int)location.y, location.z) != 0) return;

                inventory.Select(287); //Select Cactus

                player.functions.LookAtBlock(location, true, 4);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(location, 4);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x - 1, (int)location.y + 1, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        #endregion

        #region MakeSecondLayerSandEast

        private void MakeFirstLayerStringEast()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(2, -1, 0), () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    MakeFirstLayerStringEast(new[]
                    {
                        CheckForBlock(-1, -2, -2), CheckForBlock(-1, -2, 2)
                    }, () =>
                    {
                        _secondLayerStringEast = true;

                        DebugMessage("MakeFirstLayerStringEast TaskCompleted();");

                        TaskCompleted();
                    });
                });
            });
        }

        private void MakeSecondLayerSandEast()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                     CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                },
                () =>
                {
                    _needToMoveToSouth = true;
                    _needToMoveToEast = false;

                    DebugMessage("MakeSecondLayerSandEast TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandSouth

        private void MakeFirstLayerStringSouth()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(-2, -1, 2), () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    MakeFirstLayerStringSouth(new[]
                        {
                            CheckForBlock(2, -2, -1), CheckForBlock(-2, -2, -1)
                        },
                        () =>
                        {
                            _secondLayerStringSouth = true;

                            DebugMessage("MakeFirstLayerStringSouth TaskCompleted();");

                            TaskCompleted();
                        });
                });
            });
        }

        private void MakeSecondLayerSandSouth()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(2, -2, 0), CheckForBlock(-2, -2, 0)
                },
                () =>
                {
                    _needToMoveToWest = true;
                    _needToMoveToSouth = false;

                    DebugMessage("MakeSecondLayerSandSouth TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region MakeSecondLayerSandWest

        private void MakeFirstLayerStringWest()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(-2, -1, -2), () =>
            {
                WaitGrounded(() =>
                {
                    _currentbuildingloc = player.status.entity.location.ToLocation();

                    MakeFirstLayerStringWest(new[]
                        {
                            CheckForBlock(1, -2, 2), CheckForBlock(1, -2, -2)
                        },
                        () =>
                        {
                            _secondLayerStringWest = true;

                            DebugMessage("MakeFirstLayerStringWest TaskCompleted();");

                            TaskCompleted();
                        });
                });
            });
        }

        private void MakeSecondLayerSandWest()
        {
            _busy = true;

            PlaceBlockLocation(12, new[]
                {
                    CheckForBlock(0, -2, 2), CheckForBlock(0, -2, -2)
                },
                () =>
                {
                    _needToMoveToWest = false;
                    _needToMoveToMiddle = true;

                    if (_activateLayerCount) _layerCount++;

                    DebugMessage("MakeSecondLayerSandWest TaskCompleted();");

                    TaskCompleted();
                });
        }

        #endregion

        #region Move To Middle Gamemode

        private void MoveToMiddleSecondLayer()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc.Offset(2, -1, 0), () =>
            {
                WaitGrounded(() =>
                {
                    _needToMoveToMiddle = false;
                    
                    DebugMessage("MoveToMiddleSecondLayer TaskCompleted");

                    TaskCompleted();
                });
            });
        }

        #endregion

        #endregion



        #region Second Layer

        private void MakeSecondLayerCactus(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create((short)81, 64)); 
                }

                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(81) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

                var data = player.functions.FindValidNeighbour(location);

                if (data == null) return;

                inventory.Select(81);

                player.functions.LookAtBlock(data.location, true, data.face);

                player.tickManager.Register(_tickDelay / 2,
                    () =>
                    {
                        player.functions.BlockPlaceOnBlockFace(data.location, data.face);
                        player.tickManager.Register(_tickDelay / 4,
                            () =>
                            {
                                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeSecondLayerCactus()
        {
            _busy = true;

            _currentbuildingloc = player.status.entity.location.ToLocation();

            if (_gamemode)
            {
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

                        DebugMessage("MakeSecondLayerCactus TaskCompleted();");

                        TaskCompleted();
                    });
            }
            else
            {
                MakeSecondLayerCactus(new[]
                    {
                        CheckForBlock(1, 0, 2), CheckForBlock(2, 0, 1),
                        CheckForBlock(-1, 0, 2), CheckForBlock(-2, 0, 1),
                        CheckForBlock(-2, 0, -1), CheckForBlock(-1, 0, -2),
                        CheckForBlock(1, 0, -2), CheckForBlock(2, 0, -1)
                    },
                    () =>
                    {
                        _secondLayerCactus = true;

                        DebugMessage("MakeSecondLayerCactus TaskCompleted();");

                        TaskCompleted();
                    });
            }
        }

        private void MakeSecondLayerString(IEnumerable<ILocation> precomputed, Action callback)
        {
            var locations = new Queue<ILocation>(precomputed);

            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (_stopped) return;
                
                if (player.status.entity.gamemode == Gamemodes.creative)
                {
                    actions.CreativeSetSlot(38, SlotType.Create((short)81, 64)); 
                }

                //TODO: Stop the plugin when we don't have a block left
                if (locations.Count == 0 || inventory.FindId(81) < 1 || _stopped)
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

                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

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
                                if (player.world.GetBlockId(location.x, (int) location.y, location.z) != 0) return;

                                locations.Enqueue(location);
                            });
                    });
            });
        }

        private void MakeSecondLayerString()
        {
            var currentloc = player.status.entity.location.ToLocation();

            GoToLocation(currentloc, () =>
            {
                WaitGrounded(() =>
                {
                    DebugMessage("MakeSecondLayerString map complete");

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

                                _firstLayerCactusOutside = false;
                                _firstLayerStringOutside = false;
                                _firstLayerCactusInside = false;
                                _firstLayerStringInside = false;

                                DebugMessage("MakeSecondLayerString TaskCompleted();");

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

                            _firstLayerCactusOutside = false;
                            _firstLayerStringOutside = false;
                            _firstLayerCactusInside = false;
                            _firstLayerStringInside = false;

                            DebugMessage("TaskCompleted();");
                            
                            TaskCompleted();
                        });
                });
            });
        }

        #endregion



        #region Functions

        private IStopToken RegisterReoccuring(int ticks, Action callback, IStopToken token = null)
        {
            var tempToken = token ?? new CancelToken(); // Or CancelToken();, not sure which one you can access.
            player.tickManager.Register(ticks, () =>
            {
                if (tempToken.stopped) return;

                callback();
                RegisterReoccuring(ticks, callback, tempToken);
            });
            return tempToken;
        }

        private void GoToLocation(ILocation targetLocation, Action callback)
        {
            inventory.Select(12);
			
            var cancelToken = new CancelToken();

            _busy = true;

            var targetMoveToLocation = actions.AsyncMoveToLocation(targetLocation, cancelToken, Mo);

            DebugMessage("GoToLocation Target: " + targetMoveToLocation.Target);

            targetMoveToLocation.Completed += areaMap => { callback(); };
            targetMoveToLocation.Cancelled += (areaMap, cuboid) =>
            {
                if (cancelToken.stopped) return;
                cancelToken.Stop();
                InvalidateBlock(targetMoveToLocation.Target);
                TaskCompleted();
            };

            if (!targetMoveToLocation.Start())
            {
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

        private void WaitGrounded(Action callback)
        {
            IStopToken stopToken = null;
            stopToken = RegisterReoccuring(_tickDelay, () =>
            {
                if (!player.physicsEngine.isGrounded) return;

                stopToken?.Stop();

                callback();
                
                DebugMessage("No Sand left");
            });
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

        private void MineBlockNoBusy(ILocation location)
        {
            player.tickManager.Register(3, () =>
            {
                actions.LookAtBlock(location, true);
                actions.SelectBestTool(location);
                player.tickManager.Register(1, () => { actions.BlockDig(location, action => { }); });
            });
        }

        private ILocation CheckForBlock(int x, float y, int z)
        {
            return _currentbuildingloc.Offset(x, y, z);
        }

        private void DebugMessage(string debugmessage)
        {
#if DEBUG
            Console.WriteLine(debugmessage);
#endif
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