using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.PluginBase.Classes.Window;
using OQ.MineBot.PluginBase.Classes.Window.Containers;
using OQ.MineBot.PluginBase.Movement.Events;

namespace CactusFarmBuilder.Helpers
{
    public class HelperFunctions
    {
        public static readonly MapOptions MAP_OPTIONS_MINE = new MapOptions
        {
            Look = true,
            Build = false,
            Mine = true
        };

        public static readonly MapOptions MAP_OPTIONS_NOMINE = new MapOptions
        {
            Look = true,
            Build = false,
            Mine = false
        };

        public static readonly MapOptions MAP_OPTIONS_BUILD = new MapOptions
        {
            Look = true,
            Build = true,
            Mine = false
        };

        public static readonly MapOptions MAP_OPTIONS_NOBUILD = new MapOptions
        {
            Look = true,
            Build = false,
            Mine = false
        };

        private readonly IBotContext _context;
        private readonly IInventory _inventory;
        private readonly bool _ignoreFailSafe;

        public bool Stopped = false;

        public HelperFunctions(IBotContext context, IInventory inventory, bool ignoreFailSafe = false)
        {
            _context = context;
            _inventory = inventory;
            _ignoreFailSafe = ignoreFailSafe;
        }

        public bool CheckItemCount(string blockName, bool creativeRefill = false)
        {
            if (Stopped) return false;

            var blockIdNullable = Blocks.Instance.GetId(blockName);
            if (blockName == "string")
                blockIdNullable = 287;

            if (blockIdNullable != null)
            {
                var blockId = blockIdNullable.Value;
                ZerGo0Debugger.Debug(_context.Player.GetUsername(), blockId.ToString());

                if (_inventory.GetAmountOfItem(blockId) >= 1) return true;
                if (_context.Player.GetGamemode() == Gamemodes.creative && creativeRefill)
                {
                    ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"CreativeSetSlot {blockId.ToString()}");
                    switch (blockId)
                    {
                        case 12:
                            _context.Functions.CreativeSetSlot(36, SlotType.Create(_context, blockId, 64));
                            break;
                        case 81:
                            _context.Functions.CreativeSetSlot(37, SlotType.Create(_context, blockId, 64));
                            break;
                        case 287:
                            _context.Functions.CreativeSetSlot(38, SlotType.Create(_context, blockId, 64));
                            break;
                        default:
                            _context.Functions.CreativeSetSlot(39, SlotType.Create(_context, blockId, 64));
                            break;
                    }

                    return true;
                }
            }

            ZerGo0Debugger.Info(_context.Player.GetUsername(), $"Missing {blockName}");
            return false;

        }

        public bool CheckItemCount(IEnumerable<string> blockNames, bool creativeRefill = false)
        {
            return blockNames.All(blockName => CheckItemCount(blockName, creativeRefill));
        }

        public async Task WaitTillGrounded()
        {
            while (!_context.Player.PhysicsEngine.isGrounded) await _context.TickManager.Sleep(1);
        }

        public bool IsBlockId(ILocation loc, int blockId)
        {
            return _context.World.GetBlockId(loc) != (ushort) blockId;
        }

        public async Task<bool> PlaceBlockAt(ILocation location, string blockName, int tickdelay)
        {
            var blockIdNullable = Blocks.Instance.GetId(blockName);
            if (blockName == "string")
                blockIdNullable = 287;

            if (blockIdNullable == null) return false;
            var blockId = blockIdNullable.Value;

            if (_context.World.GetBlockId(location) == 0)
            {
                if (_context.World.GetBlockId(location.Offset(1)) == 132 && 
                    _context.World.GetBlockId(location.Offset(-1)) == 0)
                    return await PlaceBlockOn(location.Offset(1), 0, blockName, tickdelay);
                if (_context.World.GetBlockId(location.Offset(-1)) == 132)
                    return await PlaceBlockOn(location.Offset(-1), 1, blockName, tickdelay);
                if (_context.World.GetBlockId(location.Offset(0, 0, -1)) == 132)
                    return await PlaceBlockOn(location.Offset(0, 0, -1), 3, blockName, tickdelay);
                if (_context.World.GetBlockId(location.Offset(0, 0, 1)) == 132)
                    return await PlaceBlockOn(location.Offset(0, 0, 1), 2, blockName, tickdelay);
                if (_context.World.GetBlockId(location.Offset(-1, 0, 0)) == 132)
                    return await PlaceBlockOn(location.Offset(-1, 0, 0), 5, blockName, tickdelay);
                if (_context.World.GetBlockId(location.Offset(1, 0, 0)) == 132)
                    return await PlaceBlockOn(location.Offset(1, 0, 0), 4, blockName, tickdelay);
            }

            var failSafeCount = 0;
            while (true)
            {
                if (Stopped) return false;
                if (!CheckItemCount(blockName, true)) continue;

                if (failSafeCount > 40 && !_ignoreFailSafe) return false;
                failSafeCount++;

                ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"PlaceBlockAtLoc: {location} | " +
                                                                    $"BlockID: {_context.World.GetBlock(location).GetId()} | " +
                                                                    $"Place BlockID: {blockName}");
                await _inventory.Select(blockId);

                await _context.Player.LookAtSmooth(location);

                await _context.World.PlaceAt(location);
                await _context.TickManager.Sleep(tickdelay);

                var blockCheckCount = 0;
                while (blockId != 4 && _context.World.GetBlock(location).GetId() == 4 &&
                       _context.World.GetBlock(location).GetId() != 0)
                {
                    if (blockCheckCount > 15) return false;
                    blockCheckCount++;
                    await _context.TickManager.Sleep(1);
                }

                if (_context.World.GetBlock(location).GetId() != 0)
                {
                    ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"DONE PlaceBlockAtLoc: {location} | " +
                                                                        $"BlockID: {_context.World.GetBlock(location).GetId()} | " +
                                                                        $"Place BlockID: {blockName}");
                    break;
                }
            }

            return true;
        }

        public async Task<bool> PlaceBlocksAt(ILocation[] locations, string blockName, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockAt(location, blockName, tickdelay))
                    return false;

            return true;
        }

        public async Task<bool> PlaceBlockOn(ILocation location, int blockFace, string blockName, int tickdelay)
        {
            var blockIdNullable = Blocks.Instance.GetId(blockName);
            if (blockName == "string")
                blockIdNullable = 287;

            if (blockIdNullable == null) return false;
            var blockId = blockIdNullable.Value;

            var locationOffset = location;

            switch (blockFace)
            {
                case 0:
                    locationOffset = location.Offset(-1);
                    break;
                case 1:
                    locationOffset = location.Offset(1);
                    break;
                case 2:
                    locationOffset = location.Offset(0, 0, -1);
                    break;
                case 3:
                    locationOffset = location.Offset(0, 0, 1);
                    break;
                case 4:
                    locationOffset = location.Offset(-1, 0, 0);
                    break;
                case 5:
                    locationOffset = location.Offset(+1, 0, 0);
                    break;
            }

            var failSafeCount = 0;
            while (true)
            {
                if (Stopped) return false;
                if (!CheckItemCount(blockName, true)) continue;

                if (failSafeCount > 40 && !_ignoreFailSafe) return false;
                failSafeCount++;


                ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"PlaceBlockOnLoc: {locationOffset} | " +
                                                                    $"BlockID: {_context.World.GetBlock(locationOffset).GetId()} | " +
                                                                    $"Place BlockID: {blockId}");
                await _inventory.Select(blockId);

                await _context.Player.LookAtSmooth(location);

                await _context.World.PlaceOn(location, (sbyte) blockFace);
                await _context.TickManager.Sleep(tickdelay);

                var blockCheckCount = 0;
                while (blockId != 4 && _context.World.GetBlock(locationOffset).GetId() == 4 &&
                       _context.World.GetBlock(locationOffset).GetId() != 0)
                {
                    if (blockCheckCount > 15) return false;
                    blockCheckCount++;
                    await _context.TickManager.Sleep(1);
                }

                if (_context.World.GetBlock(locationOffset).GetId() != 0)
                {
                    ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"DONE PlaceBlockOnLoc: {locationOffset} | " +
                                                                        $"BlockID: {_context.World.GetBlock(locationOffset).GetId()} | " +
                                                                        $"Place BlockID: {blockId}");
                    break;
                }
            }

            return true;
        }

        public async Task<bool> PlaceBlocksOn(ILocation[] locations, int blockFace, string blockName, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockOn(location, blockFace, blockName, tickdelay))
                    return false;

            return true;
        }

        public async Task<bool> BreakBlock(ILocation location, int tickdelay)
        {
            var failSafeCount = 0;
            while (_context.World.GetBlockId(location) != 0)
            {
                if (Stopped) return false;

                if (failSafeCount > 40 && !_ignoreFailSafe) return false;
                failSafeCount++;
                
                var bestTool = _context.World.GetBlock(location);
                if (bestTool != null) await bestTool.SelectTool();

                await _context.Player.LookAtSmooth(location);

                await _context.World.Dig(location);
                await _context.TickManager.Sleep(tickdelay);
            }

            return true;
        }

        public async Task<bool> BreakBlocks(ILocation[] locations, int tickdelay)
        {
            foreach (var location in locations)
                if (!await BreakBlock(location, tickdelay))
                    return false;

            return true;
        }

        public async Task<bool> GoToLocation(ILocation location, MapOptions mapOptions = null)
        {
            ZerGo0Debugger.Debug(_context.Player.GetUsername(), 
                $"GoToLocation Start: {_context.Player.GetLocation()} | GoToLocation Destination: {location}");
            var moveResult = await _context.Player.MoveTo(location, mapOptions).Task;

            if (moveResult.Result == MoveResultType.Completed)
            {
                await WaitTillGrounded();
                ZerGo0Debugger.Debug(_context.Player.GetUsername(),
                    $"GoToLocation succeeded! | moveResult: {moveResult.Result}");
                return true;
            }

            ZerGo0Debugger.Debug(_context.Player.GetUsername(),
                $"GoToLocation failed! | moveResult: {moveResult.Result}");
            return false;
        }

        public async Task<bool> CreateLayer(IEnumerable<ILocation> locList, string blockName, int tickDelay)
        {
            var locations = new Queue<ILocation>(locList);
            while (locations.Count > 0)
            {
                if (Stopped) return false;

                if (!CheckItemCount(blockName, true)) continue;

                var location = locations.Dequeue();

                if (location == null || _context.World.GetBlockId(location) != 0) continue;

                if (!await PlaceBlockAt(location, blockName, tickDelay)) return false;

                if (_context.World.GetBlockId(location) == 0) locations.Enqueue(location);
            }

            return true;
        }

        public async Task<bool> BackToStart(int tickdelay, ILocation startLoc)
        {
            ZerGo0Debugger.Debug(_context.Player.GetUsername(), "BackToStart");
            while (!CurrentLocation().Compare(startLoc))
            {
                if (Stopped) return false;

                if (!await PlaceCactusBackToStart(tickdelay)) return false;

                if (!await GoToLocation(CurrentLocation().Offset(-2), MAP_OPTIONS_MINE)) return false;
            }
            
            if (!await PlaceCactusBackToStart(tickdelay)) return false;

            return true;
        }

        private async Task<bool> PlaceCactusBackToStart(int tickdelay)
        {
            foreach (var location in AreaAroundPlayer(1))
            {
                if (Stopped) return false;

                if (!CheckForPlaceableSpot(location)) continue;

                if (!await PlaceBlockAt(location, "cactus", tickdelay)) return false;
            }

            return true;
        }

#region Helper Functions

        public ILocation CurrentLocation()
        {
            return _context.Player.GetLocation();
        }

        public IPosition CurrentPosition()
        {
            return _context.Player.GetPosition();
        }

        private bool CheckForPlaceableSpot(ILocation location)
        {
            return _context.World.GetBlockId(location) == 0 && _context.World.GetBlockId(location.Offset(-1)) == 12 &&
                   _context.World.IsWalkable(location.Offset(-1)) &&
                   _context.World.GetBlock(location.Offset(-1)).IsVisible();
        }

        private ILocation[] AreaAroundPlayer(int yOffset = 0)
        {
            var currentLoc = _context.Player.GetLocation();

            return new[]
            {
                currentLoc.Offset(-1, yOffset, -1), currentLoc.Offset(0, yOffset, -1),
                currentLoc.Offset(1, yOffset, -1), currentLoc.Offset(-1, yOffset, 0),
                currentLoc.Offset(1, yOffset, 0), currentLoc.Offset(-1, yOffset, 1),
                currentLoc.Offset(0, yOffset, 1), currentLoc.Offset(1, yOffset, 1)
            };
        }

#endregion
    }
}