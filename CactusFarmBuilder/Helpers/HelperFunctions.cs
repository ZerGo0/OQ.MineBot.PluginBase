using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Classes;
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
        
        public static bool CheckItemCount(IBotContext context, IInventory inventory, ushort itemId, 
            bool creativeRefill = false)
        {
            if (inventory.GetAmountOfItem(itemId) < 1)
            {
                if (context.Player.GetGamemode() == Gamemodes.creative && creativeRefill)
                {
                    ZerGo0Debugger.Debug(context.Player.GetUsername(), $"CreativeSetSlot {itemId.ToString()}");
                    switch (itemId)
                    {
                        case 12:
                            context.Functions.CreativeSetSlot(36, SlotType.Create(context, itemId, 64));
                            break;
                        case 81:
                            context.Functions.CreativeSetSlot(37, SlotType.Create(context, itemId, 64));
                            break;
                        case 287:
                            context.Functions.CreativeSetSlot(38, SlotType.Create(context, itemId, 64));
                            break;
                        default:
                            context.Functions.CreativeSetSlot(39, SlotType.Create(context, itemId, 64));
                            break;
                    }
                    return true;
                }

                ZerGo0Debugger.Info(context.Player.GetUsername(), $"Missing {GetItemIdName(itemId)}");
                return false;
            }
            return true;
        }
        
        public static bool CheckItemCount(IBotContext context, IInventory inventory, ushort[] itemIds, 
            bool creativeRefill = false)
        {
            foreach (var itemId in itemIds)
            {
                if (!CheckItemCount(context, inventory, itemId, creativeRefill)) return false;
            }
            
            return true;
        }

        public static async Task WaitTillGrounded(IBotContext context)
        {
            while (!context.Player.PhysicsEngine.isGrounded) await context.TickManager.Sleep(1);
        }

        public static bool IsBlockId(IBotContext context, ILocation loc, int blockId)
        {
            return context.World.GetBlock(loc).GetId() != (ushort) blockId;
        }

        public static async Task<bool> PlaceBlockAt(IBotContext context, IInventory inventory, ILocation location, 
            int itemId, int tickdelay)
        {
            if (context.World.GetBlockId(location) == 0 && context.World.GetBlockId(location.Offset(-1)) == 132)
            {
                return await PlaceBlockOn(context, inventory, location.Offset(-1), 1, itemId, tickdelay);
            }

            var i = 0;
            while (true)
            {
                if (i > 15) return false;
                i++;
                
                CheckItemCount(context, inventory, (ushort) itemId, true);
                
                ZerGo0Debugger.Debug(context.Player.GetUsername(), $"PlaceBlockAtLoc: {location} | " +
                                                                   $"BlockID: {context.World.GetBlock(location).GetId()} | " +
                                                                   $"ItemID: {itemId}");
                await inventory.Select((ushort) itemId);
                
                await context.Player.LookAtSmooth(location);
                
                await context.World.PlaceAt(location);
                await context.TickManager.Sleep(tickdelay);
                
                var blockCheckCount = 0;
                while (itemId != 4 && context.World.GetBlock(location).GetId() == 4 && 
                       context.World.GetBlock(location).GetId() != 0)
                {
                    if (blockCheckCount > 15) return false;
                    blockCheckCount++;
                    await context.TickManager.Sleep(1);
                }
                
                if (context.World.GetBlock(location).GetId() != 0)
                {
                    ZerGo0Debugger.Debug(context.Player.GetUsername(), $"DONE PlaceBlockAtLoc: {location} | " +
                                                                       $"BlockID: {context.World.GetBlock(location).GetId()} | " +
                                                                       $"ItemID: {itemId}");
                    break;
                }
            }

            return true;
        }
        
        public static async Task<bool> PlaceBlocksAt(IBotContext context, IInventory inventory, ILocation[] locations, 
            int itemId, int tickdelay)
        {
            foreach (var location in locations)
            {
                if (!await PlaceBlockAt(context, inventory, location, itemId, tickdelay)) return false;
            }

            return true;
        }

        public static async Task<bool> PlaceBlockOn(IBotContext context, IInventory inventory, ILocation location, 
            int blockFace, int itemId, int tickdelay)
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
                if (i > 15) return false;
                i++;
                
                CheckItemCount(context, inventory, (ushort) itemId, true);
                
                ZerGo0Debugger.Debug(context.Player.GetUsername(), $"PlaceBlockOnLoc: {locationOffset} | " +
                                                                   $"BlockID: {context.World.GetBlock(locationOffset).GetId()} | " +
                                                                   $"ItemID: {itemId}");
                await inventory.Select((ushort) itemId);

                await context.Player.LookAtSmooth(location);

                await context.World.PlaceOn(location, (sbyte) blockFace);
                await context.TickManager.Sleep(tickdelay);

                var blockCheckCount = 0;
                while (itemId != 4 && context.World.GetBlock(locationOffset).GetId() == 4 && 
                       context.World.GetBlock(locationOffset).GetId() != 0)
                {
                    if (blockCheckCount > 15) return false;
                    blockCheckCount++;
                    await context.TickManager.Sleep(1);
                }
                
                if (context.World.GetBlock(locationOffset).GetId() != 0)
                {
                    ZerGo0Debugger.Debug(context.Player.GetUsername(), $"DONE PlaceBlockOnLoc: {locationOffset} | " +
                                                                       $"BlockID: {context.World.GetBlock(locationOffset).GetId()} | " +
                                                                       $"ItemID: {itemId}");
                    break;
                }
            }

            return true;
        }
        
        public static async Task<bool> PlaceBlocksOn(IBotContext context, IInventory inventory, ILocation[] locations, 
            int blockFace, int itemId, int tickdelay)
        {
            foreach (var location in locations)
            {
                if (!await PlaceBlockOn(context, inventory, location, blockFace, itemId, tickdelay)) return false;
            }

            return true;
        }

        public static async Task<bool> BreakBlock(IBotContext context, IInventory inventory, ILocation location, 
            int tickdelay)
        {
            var i = 0;
            while (context.World.GetBlockId(location) != 0)
            {
                if (i > 15) return false;
                i++;
                
                var sword = inventory.FindBest(EquipmentType.Sword);
                if (sword != null) await sword.Select();

                await context.Player.LookAtSmooth(location);

                await context.World.Dig(location);
                await context.TickManager.Sleep(tickdelay);
            }

            return true;
        }
        
        public static async Task<bool> BreakBlocks(IBotContext context, IInventory inventory, ILocation[] locations, int tickdelay)
        {
            foreach (var location in locations)
            {
                if (!await BreakBlock(context, inventory, location, tickdelay)) return false;
            }

            return true;
        }

        public static async Task<bool> GoToLocation(IBotContext context, ILocation location, MapOptions mapOptions = null)
        {
            ZerGo0Debugger.Debug(context.Player.GetUsername(), $"GoToLocation Destination: {location}");
            var moveResult = await context.Player.MoveTo(location, mapOptions).Task;
            
            if (moveResult.Result == MoveResultType.Completed)
            {
                await WaitTillGrounded(context);
                ZerGo0Debugger.Debug(context.Player.GetUsername(), $"GoToLocation succeeded! | moveResult: {moveResult.Result}");
                return true;
            }

            ZerGo0Debugger.Debug(context.Player.GetUsername(),$"GoToLocation failed! | moveResult: {moveResult.Result}");
            return false;
        }

        public static async Task<bool> CreateLayer(IBotContext context, IInventory inventory, IEnumerable<ILocation> locList, 
            int itemId, int tickDelay)
        {
            var locations = new Queue<ILocation>(locList);
            while (locations.Count > 0)
            {
                if (!CheckItemCount(context, inventory, (ushort) itemId, true)) continue;

                var location = locations.Dequeue();
                
                if (location == null || context.World.GetBlockId(location) != 0) continue;

                if (!await PlaceBlockAt(context, inventory, location, itemId, tickDelay)) return false;
                
                if (context.World.GetBlockId(location) == 0) locations.Enqueue(location);
            }

            return true;
        }

        public static async Task<bool> BackToStart(IBotContext context, IInventory inventory,
            int tickdelay, ILocation startLoc)
        {
            ZerGo0Debugger.Debug(context.Player.GetUsername(), "BackToStart");
            while (!CurrentLocation(context).Compare(startLoc))
            {
                if (!await PlaceCactusBackToStart(context, inventory, tickdelay)) return false;

                if (!await GoToLocation(context, CurrentLocation(context).Offset(-2), MAP_OPTIONS_MINE)) return false;
            }

            return true;
        }

        private static async Task<bool> PlaceCactusBackToStart(IBotContext context, IInventory inventory, int tickdelay)
        {
            foreach (var location in AreaAroundPlayer(context, 1))
            {
                if (!CheckForPlaceableSpot(context, location)) continue;

                if (!await PlaceBlockAt(context, inventory, location, 81, tickdelay)) return false;
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

        public static ILocation CurrentLocation(IBotContext context)
        {
            return context.Player.GetLocation();
        }
        
        public static IPosition CurrentPosition(IBotContext context)
        {
            return context.Player.GetPosition();
        }

        private static bool CheckForPlaceableSpot(IBotContext context, ILocation location)
        {
            return context.World.GetBlockId(location) == 0  && context.World.GetBlockId(location.Offset(-1)) == 12 && 
                   context.World.IsWalkable(location.Offset(-1)) && context.World.GetBlock(location.Offset(-1)).IsVisible();
        }

        private static ILocation[] AreaAroundPlayer(IBotContext context, int yOffset = 0)
        {
            var currentLoc = context.Player.GetLocation();

            return new[]
            {
                currentLoc.Offset(-1, yOffset, -1), currentLoc.Offset(0, yOffset, -1), currentLoc.Offset(1, yOffset, -1),
                currentLoc.Offset(-1, yOffset, 0), currentLoc.Offset(1, yOffset, 0),
                currentLoc.Offset(-1, yOffset, 1), currentLoc.Offset(0, yOffset, 1), currentLoc.Offset(1, yOffset, 1)
            };
        }
        
#endregion
    }
}