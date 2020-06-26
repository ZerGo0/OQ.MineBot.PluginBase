using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public bool CheckItemCount(string blockname, bool creativeRefill = false)
        {
            if (Stopped) return false;
            var blockIdNullable = Blocks.Instance.GetId(blockname);

            if (blockIdNullable != null)
            {
                var blockId = blockIdNullable.Value;
            }

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

            ZerGo0Debugger.Info(_context.Player.GetUsername(), $"Missing {GetItemIdName(blockId)}");
            return false;

        }

        public bool CheckItemCount(ushort[] itemIds, bool creativeRefill = false)
        {
            foreach (var itemId in itemIds)
                if (!CheckItemCount(itemId, creativeRefill))
                    return false;

            return true;
        }

        public async Task WaitTillGrounded()
        {
            while (!_context.Player.PhysicsEngine.isGrounded) await _context.TickManager.Sleep(1);
        }

        public bool IsBlockId(ILocation loc, int blockId)
        {
            return _context.World.GetBlockId(loc) != (ushort) blockId;
        }

        public async Task<bool> PlaceBlockAt(ILocation location, int itemId, int tickdelay)
        {
            if (_context.World.GetBlockId(location) == 0)
            {
                if (_context.World.GetBlockId(location.Offset(1)) == 132 && 
                    _context.World.GetBlockId(location.Offset(-1)) == 0)
                    return await PlaceBlockOn(location.Offset(1), 0, itemId, tickdelay);
                if (_context.World.GetBlockId(location.Offset(-1)) == 132)
                    return await PlaceBlockOn(location.Offset(-1), 1, itemId, tickdelay);
                if (_context.World.GetBlockId(location.Offset(0, 0, -1)) == 132)
                    return await PlaceBlockOn(location.Offset(0, 0, -1), 3, itemId, tickdelay);
                if (_context.World.GetBlockId(location.Offset(0, 0, 1)) == 132)
                    return await PlaceBlockOn(location.Offset(0, 0, 1), 2, itemId, tickdelay);
                if (_context.World.GetBlockId(location.Offset(-1, 0, 0)) == 132)
                    return await PlaceBlockOn(location.Offset(-1, 0, 0), 5, itemId, tickdelay);
                if (_context.World.GetBlockId(location.Offset(1, 0, 0)) == 132)
                    return await PlaceBlockOn(location.Offset(1, 0, 0), 4, itemId, tickdelay);
            }

            var i = 0;
            while (true)
            {
                if (Stopped) return false;
                if (!CheckItemCount((ushort) itemId, true)) continue;

                if (i > 40) return false;
                i++;

                ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"PlaceBlockAtLoc: {location} | " +
                                                                    $"BlockID: {_context.World.GetBlock(location).GetId()} | " +
                                                                    $"ItemID: {itemId}");
                await _inventory.Select((ushort) itemId);

                await _context.Player.LookAtSmooth(location);

                await _context.World.PlaceAt(location);
                await _context.TickManager.Sleep(tickdelay);

                var blockCheckCount = 0;
                while (itemId != 4 && _context.World.GetBlock(location).GetId() == 4 &&
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
                                                                        $"ItemID: {itemId}");
                    break;
                }
            }

            return true;
        }

        public async Task<bool> PlaceBlocksAt(ILocation[] locations, int itemId, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockAt(location, itemId, tickdelay))
                    return false;

            return true;
        }

        public async Task<bool> PlaceBlockOn(ILocation location, int blockFace, int itemId, int tickdelay)
        {
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

            var i = 0;
            while (true)
            {
                if (Stopped) return false;
                if (!CheckItemCount((ushort) itemId, true)) continue;

                if (i > 40) return false;
                i++;


                ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"PlaceBlockOnLoc: {locationOffset} | " +
                                                                    $"BlockID: {_context.World.GetBlock(locationOffset).GetId()} | " +
                                                                    $"ItemID: {itemId}");
                await _inventory.Select((ushort) itemId);

                await _context.Player.LookAtSmooth(location);

                await _context.World.PlaceOn(location, (sbyte) blockFace);
                await _context.TickManager.Sleep(tickdelay);

                var blockCheckCount = 0;
                while (itemId != 4 && _context.World.GetBlock(locationOffset).GetId() == 4 &&
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
                                                                        $"ItemID: {itemId}");
                    break;
                }
            }

            return true;
        }

        public async Task<bool> PlaceBlocksOn(ILocation[] locations, int blockFace, int itemId, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockOn(location, blockFace, itemId, tickdelay))
                    return false;

            return true;
        }

        public async Task<bool> BreakBlock(ILocation location, int tickdelay)
        {
            var i = 0;
            while (_context.World.GetBlockId(location) != 0)
            {
                if (Stopped) return false;

                if (i > 40) return false;
                i++;
                
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

        public async Task<bool> CreateLayer(IEnumerable<ILocation> locList, int itemId, int tickDelay)
        {
            var locations = new Queue<ILocation>(locList);
            while (locations.Count > 0)
            {
                if (Stopped) return false;

                if (!CheckItemCount((ushort) itemId, true)) continue;

                var location = locations.Dequeue();

                if (location == null || _context.World.GetBlockId(location) != 0) continue;

                if (!await PlaceBlockAt(location, itemId, tickDelay)) return false;

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

                if (!await PlaceBlockAt(location, 81, tickdelay)) return false;
            }

            return true;
        }

#region Helper Functions

        private static string GetItemIdName(int itemId)
        {
            switch (itemId)
            {
                case 12:
                    return "Sand";
                case 81:
                    return "Cactus";
                case 287:
                    return "String";
                default:
                    return itemId.ToString();
            }
        }

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