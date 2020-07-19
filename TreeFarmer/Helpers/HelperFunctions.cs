using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.PluginBase.Classes.Window.Containers;
using OQ.MineBot.PluginBase.Movement.Events;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.Protocols.Classes.Base;

namespace TreeFarmerPlugin.Helpers
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

        public static readonly MapOptions MAP_OPTIONS_MINE_BUILD = new MapOptions
        {
            SafeMine = true,
            SafeMove = true,
            Look = true,
            Build = true,
            Mine = true,
            /*AdditionalWeights = new MapOptionWeights
            {
                BlockPlace = 15,
                BlockDig = -15,
            },*/
        };

        private readonly IBotContext _context;
        private readonly IInventory _inventory;

        public bool Stopped = false;

        public HelperFunctions(IBotContext context, IInventory inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        /// <summary>
        ///     Check if the bot has the desired item in it's inventory.
        ///     If 'creatriveRefill' is enabled then it will take the item from the creative menu
        /// </summary>
        public bool CheckItemCount(ushort itemId, bool creativeRefill = false)
        {
            if (Stopped) return false;
            if (_inventory.GetAmountOfItem(itemId) < 1)
            {
                if (_context.Player.GetGamemode() == Gamemodes.creative && creativeRefill)
                {
                    ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"CreativeSetSlot {itemId.ToString()}");
                    switch (itemId)
                    {
                        case 12:
                            _context.Functions.CreativeSetSlot(36, SlotType.Create(_context, itemId, 64));
                            break;
                        case 81:
                            _context.Functions.CreativeSetSlot(37, SlotType.Create(_context, itemId, 64));
                            break;
                        case 287:
                            _context.Functions.CreativeSetSlot(38, SlotType.Create(_context, itemId, 64));
                            break;
                        default:
                            _context.Functions.CreativeSetSlot(39, SlotType.Create(_context, itemId, 64));
                            break;
                    }

                    return true;
                }

//                ZerGo0Debugger.Info(_context.Player.GetUsername(), $"Missing {GetItemIdName(itemId)}");
                return false;
            }

            return true;
        }


        /// <summary>
        ///     Check if the bot has the MULTIPLE items in it's inventory.
        ///     If 'creatriveRefill' is enabled then it will take the items from the creative menu
        /// </summary>
        public bool CheckItemCount(ushort[] itemIds, bool creativeRefill = false)
        {
            foreach (var itemId in itemIds)
                if (!CheckItemCount(itemId, creativeRefill))
                    return false;

            return true;
        }


        /// <summary>
        ///     Waits till the bot is on the ground so that the coords don't mess up.
        /// </summary>
        public async Task WaitTillGrounded()
        {
            while (!_context.Player.PhysicsEngine.isGrounded) await _context.TickManager.Sleep(1);
        }


        /// <summary>
        ///     Check if the block at the desired location has the right blockID
        /// </summary>
        public bool IsBlockId(ILocation loc, int blockId)
        {
            return _context.World.GetBlock(loc).GetId() != (ushort) blockId;
        }


        /// <summary>
        ///     Places the block at the desired location
        /// </summary>
        public async Task<bool> PlaceBlockAt(ILocation location, int itemId, int tickdelay)
        {
            if (_context.World.GetBlockId(location) == 0 && _context.World.GetBlockId(location.Offset(-1)) == 132)
                return await PlaceBlockOn(location.Offset(-1), 1, itemId, tickdelay);

            var i = 0;
            while (true)
            {
                if (Stopped || !_context.World.GetBlock(location).CanPlaceAt())
                {
                    ZerGo0Debugger.Debug(_context.Player.GetUsername(),
                        $"Stopped: {Stopped} | " +
                        $"CanPlaceAt(): {_context.World.GetBlock(location).CanPlaceAt()} | " +
                        $"Compare(): {_context.Player.GetLocation().Compare(location)}");

                    return false;
                }

                if (i > 40) return false;
                i++;

                if (!CheckItemCount((ushort) itemId, true)) continue;

                ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"PlaceBlockAtLoc: {location} | " +
                                                                    $"BlockID: {_context.World.GetBlock(location).GetId()} | " +
                                                                    $"ItemID: {itemId}");
                await _inventory.Select((ushort) itemId);

//                await _context.Player.LookAt(location);
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

        /// <summary>
        ///     Places the blocks at the desired locations
        /// </summary>
        public async Task<bool> PlaceBlocksAt(ILocation[] locations, int itemId, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockAt(location, itemId, tickdelay))
                    return false;

            return true;
        }

        /// <summary>
        ///     Places the block at the 'blockface' of the desired location
        /// </summary>
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

        /// <summary>
        ///     Places the blocks at the 'blockface' of the desired locations
        /// </summary>
        public async Task<bool> PlaceBlocksOn(ILocation[] locations, int blockFace, int itemId, int tickdelay)
        {
            foreach (var location in locations)
                if (!await PlaceBlockOn(location, blockFace, itemId, tickdelay))
                    return false;

            return true;
        }

        /// <summary>
        ///     Break the block at the desired location
        /// </summary>
        public async Task<bool> BreakBlock(ILocation location, int tickdelay = 0)
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

        /// <summary>
        ///     Breaks the blocks at the desired locations
        /// </summary>
        public async Task<bool> BreakBlocks(ILocation[] locations, int tickdelay)
        {
            foreach (var location in locations)
                if (!await BreakBlock(location, tickdelay))
                    return false;

            return true;
        }

        /// <summary>
        ///     Breaks the blocks at the desired locations
        /// </summary>
        public async Task<bool> GoToLocation(ILocation location, MapOptions mapOptions = null)
        {
            ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"GoToLocation Destination: {location}");
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

        public async Task<bool> GoToInteractRange(ILocation location, MapOptions mapOptions = null)
        {
            ZerGo0Debugger.Debug(_context.Player.GetUsername(), $"GoToLocation Destination: {location}");
            var moveResult = await _context.Player.MoveToInteractionRange(location, mapOptions).Task;

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


        public IEnumerable<ILocation> CheckBlocksAroundLoc(ILocation location, int range)
        {
            var blocksAround = new Dictionary<ILocation, float>();

            for (var x = location.X - range; x <= location.X + range; x++)
            for (var z = location.Z - range; z <= location.Z + range; z++)
            {
                var tempLoc = new Location(x, location.Y, z);
                if (_context.World.GetBlockId(tempLoc) == 0)
                    blocksAround.Add(tempLoc, _context.Player.GetLocation().Distance(tempLoc));
            }

            return blocksAround.OrderByDescending(pair => pair.Value).Select(loc => loc.Key).ToList();
        }

        public IEnumerable<ILocation> CheckBlocksOnAllBlockFaces(ILocation location)
        {
            var tempBlocksAround = new List<ILocation>
            {
                new Location(location.X, location.Y - 1, location.Z), /*Bottom*/
                new Location(location.X, location.Y + 1, location.Z), /*Top*/
                new Location(location.X, location.Y, location.Z - 1), /*North*/
                new Location(location.X, location.Y, location.Z + 1), /*South*/
                new Location(location.X - 1, location.Y, location.Z), /*West*/
                new Location(location.X + 1, location.Y, location.Z) /*East*/
            };

            return tempBlocksAround.Where(loc => _context.World.GetBlockId(loc) != 0 &&
                                                 (_context.World.GetBlockId(loc) == 17 ||
                                                  _context.World.GetBlockId(loc) == 18)).ToList();
        }

        public IEnumerable<ILocation> GetWoodBlocks(ILocation blockLocation)
        {
            ZerGo0Debugger.Debug(_context.Player.GetUsername(),
                $"Starting to check for woodblock at: {blockLocation}");

            var woodBlockList = new List<ILocation>();
            var blockQueue = new Queue<ILocation>();
            var checkedBlockList = new List<ILocation>();

            blockQueue.Enqueue(blockLocation);
            if (_context.World.GetBlockId(blockLocation) == 17) woodBlockList.Add(blockLocation);

            while (blockQueue.Count > 0)
            {
                var startBlock = blockQueue.Dequeue();
                checkedBlockList.Add(startBlock);
                var blocksAround = CheckBlocksOnAllBlockFaces(startBlock).ToList();

                foreach (var block in blocksAround.Where(block => !checkedBlockList.Contains(block)))
                {
                    if (_context.World.GetBlockId(block) == 17)
                        woodBlockList.Add(block);

                    checkedBlockList.Add(block);
                    blockQueue.Enqueue(block);
                }
            }

            return woodBlockList;
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