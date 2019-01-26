using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Classes.Entity.Lists;
using OQ.MineBot.PluginBase.Classes.Entity.Mob;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.PluginBase.Movement.Maps;
using OQ.MineBot.PluginBase.Pathfinding;
using OQ.MineBot.Protocols.Classes.Base;
using Telegram.Bot;

namespace DebugPlugin
{

    public partial class DebugForm : Form
    {
        private readonly IPlayer _player;
        private ILocation _currentLocation;
        private string _oldDimension;
        private string _oldGamemode;
        private bool _setAlive;
        private int _oldHealth;
        private bool _oldHealthFirstTime;
        private bool _oldGrounded;
        private bool _oldInWater;
        private bool _oldJumping;
        private bool _oldGroundedFirstTime;
        private bool _oldInWaterFirstTime;
        private bool _oldJumpingFirstTime;

        private bool _oldInvFull;
        private bool _oldInvFullFirstTime;
        private int _oldInvSlots;
        private bool _oldInvFSlotsFirstTime;
        private int _oldInvUSlots;
        private short _oldHeldItemId;
        private string _oldHeldItemNbt;
        private sbyte _oldHeldItemCount;
        private short _oldHeldItemSlot;
        private bool _oldHeldItemNbtFirstTime;

        private IPlayerEntity _oldplayeraround;
        private string _oldclosestplayerloc;
        private IMobEntity _oldmobaround;
        private string _oldclosestmobloc;
        private IEntity _oldtargetaround;
        private string _oldclosesttargetloc;

        private string _oldWindowTitle;
        private int _oldWindowSlotsUsed;

        private string _oldPathTarget;
        private bool? _oldPathComplete;
        private bool? _oldPathDisposed;
        private bool? _oldPathSearched;
        private bool? _oldPathStopped;
        private bool? _oldPathValid;

        private static Dictionary<string, string> _oldDebugDictionary;

        private static string _currenBotusername;

        public DebugForm(string name, IPlayer player) {
            InitializeComponent();

            KeyDown += (sender, args) => {
                args.SuppressKeyPress = true; // Disable sound.
            };
            Chat_Box.KeyDown += (sender, args) => {
                args.SuppressKeyPress = true; // Disable sound.
            };

            _player = player;
            
            player.events.onChat += OnChatMessage;
            player.events.onHealthUpdate += OnHealthUpdate;
            player.events.onTick += Events_onTick;
            player.events.onInventoryChanged += OnInventoryChanged;

            player.events.onDisconnected +=
                (player1, reason) =>
                {
                    if (!IsDisposed && !Disposing)
                        BeginInvoke((MethodInvoker) delegate
                        {
                            if (!IsDisposed && !Disposing) Close();
                        });
                };

            _currenBotusername = player.status.username;
            Bot_Name_Text.AppendText(player.status.username);
            Bot_UUID_Text.AppendText(player.status.uuid);

            Bot_Health_Text.AppendText(player.status.entity.health.ToString(CultureInfo.InvariantCulture));
            
            _oldPathTarget = null;
            _oldPathComplete = null;
            _oldPathDisposed = null;
            _oldPathSearched = null;
            _oldPathStopped = null;
            _oldPathValid = null;

            Text = "[Bot: " + name + "] Debug Menu";
        }

        private void Events_onTick(IPlayer player)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (player.events != null)
                            //Unhook, form closed.
                            player.events.onTick -= Events_onTick;
                    }
                    else
                    {
                        #region Bot General Stuff
                        
                        #region Bot Hunger Update

                        if (_oldHealth != (int) player.status.entity.health || !_oldHealthFirstTime)
                        {
                            OnHungerUpdate((int)player.status.entity.health);
                            OnHungerUpdate(player.status.entity.food);

                            _oldHealth = (int) player.status.entity.health;
                            _oldHealthFirstTime = true;
                        }

                        #endregion

                        #region Bot Location

                        //Update it only when needed
                        if (player.status.entity.location.ToLocation().Distance(_currentLocation) > 0 || _currentLocation == null)
                        {
                            Bot_Location_Text.Clear();
                            Bot_Location_Text.AppendText(player.status.entity.location.ToLocation().ToString());
                            _currentLocation = player.status.entity.location.ToLocation();
                        }

                        #endregion

                        #region Bot Dimension
                        
                        //Update it only when needed
                        if (player.status.entity.dimension.ToString() != _oldDimension || string.IsNullOrEmpty(_oldDimension))
                        {
                            Bot_Dimension_Text.Clear();
                            Bot_Dimension_Text.AppendText(player.status.entity.dimension.ToString());
                            _oldDimension = player.status.entity.dimension.ToString();
                        }

                        #endregion

                        #region Bot Gamemode

                        //TODO: Check if it get's fixed in the next updates, it doesn't update the
                        //TODO: value when the gamemode get's change ingame
                        //Update it only when needed
                        if (player.status.entity.gamemode.ToString() != _oldGamemode ||
                            string.IsNullOrEmpty(_oldGamemode))
                        {
                            Bot_Gamemode_Text.Clear();
                            Bot_Gamemode_Text.AppendText(player.status.entity.gamemode.ToString());
                            _oldGamemode = player.status.entity.gamemode.ToString();
                        }

                        #endregion

                        #region Bot Alive

                        if (player.status.entity.isDead && _setAlive)
                        {
                            Bot_Dead_Text.Clear();
                            Bot_Dead_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Bold);
                            Bot_Dead_Text.SelectionColor = Color.DarkRed;
                            Bot_Dead_Text.AppendText(player.status.entity.isDead.ToString());
                            _setAlive = false;
                        }
                        else if (!player.status.entity.isDead && !_setAlive)
                        {
                            Bot_Dead_Text.Clear();
                            Bot_Dead_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Regular);
                            Bot_Dead_Text.SelectionColor = Color.Black;
                            Bot_Dead_Text.AppendText(player.status.entity.isDead.ToString());
                            _setAlive = true;
                        }

                        #endregion

                        #region Bot Grounded

                        if (player.physicsEngine.isGrounded != _oldGrounded || !_oldGroundedFirstTime)
                        {
                            Bot_Grounded_Text.Clear();
                            Bot_Grounded_Text.AppendText(player.physicsEngine.isGrounded.ToString());

                            _oldGrounded = player.physicsEngine.isGrounded;
                            _oldGroundedFirstTime = true;
                        }

                        #endregion


                        #region Bot in Water

                        if (player.physicsEngine.inWater != _oldInWater || !_oldInWaterFirstTime)
                        {
                            Bot_In_Water_Text.Clear();
                            Bot_In_Water_Text.AppendText(player.physicsEngine.inWater.ToString());

                            _oldInWater = player.physicsEngine.inWater;
                            _oldInWaterFirstTime = true;
                        }

                        #endregion

                        #region Bot Jumping
                        
                        if (player.physicsEngine.jumping != _oldJumping || !_oldJumpingFirstTime)
                        {
                            Bot_Jumping_Text.Clear();
                            Bot_Jumping_Text.AppendText(player.physicsEngine.jumping.ToString());

                            _oldJumping = player.physicsEngine.jumping;
                            _oldJumpingFirstTime = true;
                        }

                        #endregion

                        #endregion

                        #region Bot Inv Stuff

                        #region Bot Inv Full

                        if (player.status.containers.inventory.IsFull() && _oldInvFull || player.status.containers.inventory.IsFull() && !_oldInvFullFirstTime)
                        {
                            Bot_Inv_Full_Text.Clear();
                            Bot_Inv_Full_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Bold);
                            Bot_Inv_Full_Text.SelectionColor = Color.DarkRed;
                            Bot_Inv_Full_Text.AppendText(player.status.containers.inventory.IsFull().ToString());
                            _oldInvFull = false;
                            _oldInvFullFirstTime = true;
                        }
                        else if (!player.status.containers.inventory.IsFull() && !_oldInvFull)
                        {
                            Bot_Inv_Full_Text.Clear();
                            Bot_Inv_Full_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Regular);
                            Bot_Inv_Full_Text.SelectionColor = Color.Black;
                            Bot_Inv_Full_Text.AppendText(player.status.containers.inventory.IsFull().ToString());
                            _oldInvFull = true;
                        }

                        #endregion

                        #region Bot Free/Used Slots Firsttime

                        if (!_oldInvFSlotsFirstTime)
                        {
                            var freeinvslots = 0;
                            for (int invslot = 9; invslot <= 44; invslot++)
                            {
                                if (player.status.containers.inventory.GetAt(invslot).id == -1)
                                {
                                    freeinvslots++;
                                }
                            }

                            if (freeinvslots != _oldInvSlots)
                            {
                                Bot_Inv_FSlots_Text.Clear();
                                Bot_Inv_FSlots_Text.AppendText(freeinvslots.ToString());

                                _oldInvSlots = freeinvslots;
                            }

                            if (_oldInvUSlots != 36 - _oldInvSlots || _oldInvUSlots == 0)
                            {
                                _oldInvUSlots = 36 - _oldInvSlots;
                                
                                Bot_Inv_USlots_Text.Clear();
                                Bot_Inv_USlots_Text.AppendText(_oldInvUSlots.ToString());
                            }

                            _oldInvFSlotsFirstTime = true;
                        }

                        #endregion

                        #region Bot Held Item ID

                        var helditemid = player.status.containers.inventory.hotbar
                            .GetSlot((byte)player.status.entity.selectedSlot).id;

                        if (helditemid != _oldHeldItemId)
                        {
                            Held_Item_ID_Text.Clear();
                            Held_Item_ID_Text.AppendText(helditemid.ToString());

                            _oldHeldItemId = helditemid;
                        }

                        #endregion

                        #region Bot Held Item NBT

                        var helditemnbt = player.status.containers.inventory.hotbar
                            .GetSlot((byte)player.status.entity.selectedSlot).nbt;

                        const string errortext = "";

                        if (helditemnbt != null)
                        {
                            if (helditemnbt.ToString() != _oldHeldItemNbt || !_oldHeldItemNbtFirstTime)
                            {
                                Held_Item_NBT_Text.Clear();
                                Held_Item_NBT_Text.AppendText(helditemnbt.ToString());

                                _oldHeldItemNbt = helditemnbt.ToString();
                                _oldHeldItemNbtFirstTime = true;
                            }
                        }
                        else if (_oldHeldItemNbt != errortext || !_oldHeldItemNbtFirstTime)
                        {
                            Held_Item_NBT_Text.Clear();
                            Held_Item_NBT_Text.AppendText(errortext);

                            _oldHeldItemNbt = errortext;
                            _oldHeldItemNbtFirstTime = true;
                        }

                        #endregion

                        #region Bot Held Item Count

                        var helditemcount = player.status.containers.inventory.hotbar
                            .GetSlot((byte)player.status.entity.selectedSlot).count;

                        if (helditemcount != _oldHeldItemCount)
                        {
                            Held_Item_Count_Text.Clear();
                            Held_Item_Count_Text.AppendText(helditemcount.ToString());

                            _oldHeldItemCount = helditemcount;
                        }

                        #endregion

                        #region Bot Held Item Slot

                        if (player.status.entity.selectedSlot != _oldHeldItemSlot)
                        {
                            Held_Item_Slot_Text.Clear();
                            Held_Item_Slot_Text.AppendText(player.status.entity.selectedSlot.ToString());

                            _oldHeldItemSlot = player.status.entity.selectedSlot;
                        }

                        #endregion

                        #endregion

                        #region Bot Closest Stuff

                        #region Bot Closest Player

                        var playeraround = (IPlayerEntity)player.entities.FindClosestPlayer(CurrentLocation().x , CurrentLocation().y , CurrentLocation().z);

                        if (playeraround != null)
                        {
                            var playeraroundloc = new Location((int)playeraround.location.X,
                                (int)playeraround.location.Y, (int)playeraround.location.Z);

                            if (playeraroundloc.ToString() != _oldclosestplayerloc)
                            {
                                Closest_Player_Loc_Text.Clear();
                                Closest_Player_Loc_Text.AppendText(playeraroundloc.ToString());

                                Closest_Player_Dist_Text.Clear();
                                Closest_Player_Dist_Text.AppendText(CurrentLocation().Distance(playeraroundloc).ToString(CultureInfo.InvariantCulture));
                                
                                _oldclosestplayerloc = playeraroundloc.ToString();
                            }

                            if (playeraround != _oldplayeraround || string.IsNullOrWhiteSpace(Targetable_Player_Text.Text))
                            {
                                Closest_Player_Text.Clear();
                                if (player.entities.FindNameByUuid(playeraround.uuid) != null)
                                    Closest_Player_Text.AppendText(player.entities.FindNameByUuid(playeraround.uuid)
                                        .Name);

                                Closest_Player_UUID_Text.Clear();
                                Closest_Player_UUID_Text.AppendText(playeraround.uuid);

                                _oldplayeraround = playeraround;
                            }
                        }
                        else
                        {
                            Closest_Player_Text.Clear();
                            Closest_Player_UUID_Text.Clear();
                            Closest_Player_Loc_Text.Clear();
                            Closest_Player_Dist_Text.Clear();
                        }

                        #endregion

                        #region Bot Closest Mob
                        
                        var mobaround = (IMobEntity)player.entities.FindClosestMob(CurrentLocation().x, CurrentLocation().y, CurrentLocation().z);

                        if (mobaround != null)
                        {
                            var mobaroundloc = new Location((int)mobaround.location.X,
                                (int)mobaround.location.Y, (int)mobaround.location.Z);

                            if (mobaroundloc.ToString() != _oldclosestmobloc)
                            {
                                Closest_Mob_Loc_Text.Clear();
                                Closest_Mob_Loc_Text.AppendText(mobaroundloc.ToString());
                                
                                Closest_Mob_Dist_Text.Clear();
                                Closest_Mob_Dist_Text.AppendText(CurrentLocation().Distance(mobaroundloc).ToString(CultureInfo.InvariantCulture));

                                _oldclosestmobloc = mobaroundloc.ToString();
                            }

                            if (mobaround != _oldmobaround)
                            {
                                Closest_Mob_Text.Clear();
                                Closest_Mob_Text.AppendText(mobaround.type.ToString());

                                _oldmobaround = mobaround;
                            }
                        }
                        else
                        {
                            Closest_Mob_Text.Clear();
                            Closest_Mob_Loc_Text.Clear();
                            Closest_Mob_Dist_Text.Clear();
                        }

                        #endregion

                        #region Bot Closest Target

                        var targetaround = (IPlayerEntity)player.entities.FindClosestTarget(CurrentLocation(), Targeter.DefaultFilter);

                        if (targetaround != null)
                        {
                            var targetaroundloc = new Location((int)targetaround.location.X,
                                (int)targetaround.location.Y, (int)targetaround.location.Z);

                            if (targetaroundloc.ToString() != _oldclosesttargetloc || string.IsNullOrWhiteSpace(Targetable_Player_Loc_Text.Text))
                            {
                                Targetable_Player_Loc_Text.Clear();
                                Targetable_Player_Loc_Text.AppendText(targetaroundloc.ToString());
                                
                                Targetable_Player_Dist_Text.Clear();
                                Targetable_Player_Dist_Text.AppendText(CurrentLocation().Distance(targetaroundloc).ToString(CultureInfo.InvariantCulture));

                                _oldclosesttargetloc = targetaroundloc.ToString();
                            }

                            if (targetaround != _oldtargetaround || string.IsNullOrWhiteSpace(Targetable_Player_Text.Text))
                            {
                                Targetable_Player_Text.Clear();
                                if (player.entities.FindNameByUuid(targetaround.uuid) != null)
                                    Targetable_Player_Text.AppendText(player.entities.FindNameByUuid(targetaround.uuid).Name);
                                
                                //TODO: Check if this is right
                                Targetable_Player_UUID_Text.Clear();
                                Targetable_Player_UUID_Text.AppendText(targetaround.uuid);
                                
                                _oldtargetaround = targetaround;
                            }
                        }
                        else
                        {
                            Targetable_Player_Text.Clear();
                            Targetable_Player_UUID_Text.Clear();
                            Targetable_Player_Loc_Text.Clear();
                            Targetable_Player_Dist_Text.Clear();
                        }

                        #endregion

                        #endregion

                        #region Window Stuff

                        var winSlotsUsed = 0;
                        for (int getwindowid = 9999; getwindowid > 0; getwindowid--)
                        {
                            if (player.status.containers.GetWindow(getwindowid) != null)
                            {
                                var winSlotCount = player.status.containers.GetWindow(getwindowid).slotCount;
                                
                                for (var windowslots = 0; windowslots <= winSlotCount + 36; windowslots++)
                                {
                                    if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                        continue;

                                    if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id !=
                                        -1)
                                    {
                                        winSlotsUsed++;
                                    }
                                }

                                if (player.status.containers.GetWindow(getwindowid).windowTitle != _oldWindowTitle && 
                                    player.status.containers.GetWindow(getwindowid).windowType != "Inventory" ||
                                    player.status.containers.GetWindow(getwindowid).windowType != "Inventory" &&
                                    winSlotsUsed != _oldWindowSlotsUsed)
                                {
                                    var winTitle = player.status.containers.GetWindow(getwindowid).windowTitle;
                                    var winType = player.status.containers.GetWindow(getwindowid).windowType;
                                    var winId = player.status.containers.GetWindow(getwindowid).id;
                                    var winEntityId = player.status.containers.GetWindow(getwindowid).entityId;
                                    var winmActionId = player.status.containers.GetWindow(getwindowid).m_actionId;
                                    
                                    Window_Title_Text.Clear();
                                    Window_Type_Text.Clear();
                                    Window_ID_Text.Clear();
                                    Window_Slotcount_Text.Clear();
                                    Window_EntityID_Text.Clear();
                                    Window_ActionID_Text.Clear();
                                    Window_Slotids_Text.Clear();
                                    Window_Inv_Slotids_Text.Clear();
                                    Window_Slot_NBT_Text.Clear();

                                    Window_Title_Text.AppendText(winTitle);
                                    Window_Type_Text.AppendText(winType);
                                    Window_ID_Text.AppendText(winId.ToString());
                                    Window_Slotcount_Text.AppendText(winSlotCount.ToString());
                                    Window_EntityID_Text.AppendText(winEntityId.ToString());
                                    Window_ActionID_Text.AppendText(winmActionId.ToString());

                                    for (var windowslots = 0; windowslots <= winSlotCount - 1; windowslots++)
                                    {
                                        if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                            continue;

                                        if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id !=
                                            -1)
                                        {
                                            Window_Slotids_Text.AppendText("Slot: " + windowslots + "   |   ItemID: " + 
                                                                           player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id
                                                                           + "   |   Metadata: " + player.status.containers.GetWindow(getwindowid).GetAt(windowslots).damage + "\n");
                                            
                                            if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots).nbt != null)
                                            {
                                                var windowNbt = player.status.containers.GetWindow(getwindowid).GetAt(windowslots).nbt.ToString();
                                                Window_Slot_NBT_Text.AppendText("Slot: " + windowslots + "   |   ItemID: " +
                                                                                player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id + 
                                                                                "   |   ItemNBT: \n" + windowNbt);
                                            }
                                        }
                                    }

                                    for (var windowslots = winSlotCount; windowslots <= winSlotCount + 35; windowslots++)
                                    {
                                        if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                            continue;

                                        if (player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id !=
                                            -1)
                                        {
                                            Window_Inv_Slotids_Text.AppendText("Slot: " + windowslots + "   |   ItemID: " +
                                                                           player.status.containers.GetWindow(getwindowid).GetAt(windowslots).id
                                                                               + "   |   Metadata: " + player.status.containers.GetWindow(getwindowid).GetAt(windowslots).damage + "\n");
                                        }
                                    }

                                    _oldWindowTitle = winTitle;
                                    _oldWindowSlotsUsed = winSlotsUsed;
                                }

                                break;
                            }
                        }

                        #endregion

                        #region Path Stuff

                        if (player.physicsEngine.path != null)
                        {
                            if (player.physicsEngine.path.Target.ToString() != _oldPathTarget ||
                                player.physicsEngine.path.Complete != _oldPathComplete || 
                                player.physicsEngine.path.Disposed != _oldPathDisposed ||
                                player.physicsEngine.path.Searched != _oldPathSearched ||
                                player.physicsEngine.path.Token.stopped != _oldPathStopped ||
                                player.physicsEngine.path.Valid != _oldPathValid)
                            {
                                Current_Path_Target_Loc_Text.Clear();
                                Current_Path_Complete_Text.Clear();
                                Current_Path_Disposed_Text.Clear();
                                Current_Path_Offset_Text.Clear();
                                Current_Path_Options_Text.Clear();
                                Current_Path_Searched_Text.Clear();
                                Current_Path_Token_Stopped_Text.Clear();
                                Current_Path_Valid_Text.Clear();


                                Current_Path_Target_Loc_Text.AppendText(player.physicsEngine.path.Target.ToString());
                                Current_Path_Complete_Text.AppendText(player.physicsEngine.path.Complete.ToString());
                                Current_Path_Disposed_Text.AppendText(player.physicsEngine.path.Disposed.ToString());

                                if (player.physicsEngine.path.Offset != null)
                                    Current_Path_Offset_Text.AppendText(player.physicsEngine.path.Offset.ToString());

                                Current_Path_Searched_Text.AppendText(player.physicsEngine.path.Searched.ToString());
                                Current_Path_Token_Stopped_Text.AppendText(player.physicsEngine.path.Token.stopped.ToString());
                                Current_Path_Valid_Text.AppendText(player.physicsEngine.path.Valid.ToString());

                                Current_Path_Options_Text.AppendText("AntiStuck: " + player.physicsEngine.path.Options.AntiStuck + "\n");
                                Current_Path_Options_Text.AppendText("Climb: " + player.physicsEngine.path.Options.Climb + "\n");
                                Current_Path_Options_Text.AppendText("EdgeStick: " + player.physicsEngine.path.Options.EdgeStick + "\n");
                                Current_Path_Options_Text.AppendText("Fly: " + player.physicsEngine.path.Options.Fly + "\n");
                                Current_Path_Options_Text.AppendText("Look: " + player.physicsEngine.path.Options.Look + "\n");
                                Current_Path_Options_Text.AppendText("Mine: " + player.physicsEngine.path.Options.Mine + "\n");
                                Current_Path_Options_Text.AppendText("NoCost: " + player.physicsEngine.path.Options.NoCost + "\n");
                                Current_Path_Options_Text.AppendText("NoSlowdown: " + player.physicsEngine.path.Options.NoSlowdown + "\n");
                                Current_Path_Options_Text.AppendText("PauseOnNext: " + player.physicsEngine.path.Options.PauseOnNext + "\n");
                                Current_Path_Options_Text.AppendText("Quality: " + player.physicsEngine.path.Options.Quality + "\n");
                                Current_Path_Options_Text.AppendText("SafeMine: " + player.physicsEngine.path.Options.SafeMine + "\n");
                                Current_Path_Options_Text.AppendText("Sprint: " + player.physicsEngine.path.Options.Sprint + "\n");
                                Current_Path_Options_Text.AppendText("SprintVisuals: " + player.physicsEngine.path.Options.SprintVisuals + "\n");
                                Current_Path_Options_Text.AppendText("Strict: " + player.physicsEngine.path.Options.Strict + "\n");
                                Current_Path_Options_Text.AppendText("Swim: " + player.physicsEngine.path.Options.Swim + "\n");

                                _oldPathTarget = player.physicsEngine.path.Target.ToString();
                                _oldPathComplete = player.physicsEngine.path.Complete;
                                _oldPathDisposed = player.physicsEngine.path.Disposed;
                                _oldPathSearched = player.physicsEngine.path.Searched;
                                _oldPathStopped = player.physicsEngine.path.Token.stopped;
                                _oldPathValid = player.physicsEngine.path.Valid;
                            }
                        }

                        //Console.WriteLine(player.physicsEngine.Velocity.distance.ToString());
                        //Console.WriteLine(player.physicsEngine.collider.blockId.ToString());
                        //Console.WriteLine(player.physicsEngine.collider.height);
                        //Console.WriteLine(player.physicsEngine.collider.widthx);
                        //Console.WriteLine(player.physicsEngine.collider.widthz);
                        
                        //Console.WriteLine(player.physicsEngine.isGrounded);
                        //Console.WriteLine(player.physicsEngine.jumping);
                        //Console.WriteLine(player.physicsEngine.precomputedRotations.Count);
                        //
                        //Console.WriteLine("\n");
                        //
                        //Console.WriteLine(player.entities.entityList.Keys.Count);
                        //Console.WriteLine(player.entities.entityList.Values.ToString());
                        //
                        //Console.WriteLine(player.entities.playerList.Keys.Count);
                        //Console.WriteLine(player.entities.playerList.Values.ToString());
                        //
                        //Console.WriteLine(player.entities.uuidList.Keys.Count);
                        //Console.WriteLine(player.entities.uuidList.Values.ToString());
                        //
                        //for (int entitycount = 0; entitycount < 999; entitycount++)
                        //{
                        //    if (player.entities.GetEntity(entitycount) != null)
                        //    {
                        //        Console.WriteLine(player.entities.GetEntity(entitycount).entityId);
                        //        Console.WriteLine(player.entities.GetEntity(entitycount).location.ToLocation());
                        //        Console.WriteLine(player.entities.GetEntity(entitycount));
                        //    }
                        //}
                        //
                        //Console.WriteLine("\n");



                        #endregion
                    }
                });
            }
            catch
            {

                //The form is already disposed, unhook.
                player.events.onTick -= Events_onTick;
            }
        }

        private void OnInventoryChanged(IPlayer player, bool changed, bool removed, ushort id, int countdifference, ISlot slot)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (player.events != null)
                            //Unhook, form closed.
                            player.events.onInventoryChanged -= OnInventoryChanged;
                    }
                    else
                    {
                        #region Bot Free/Used Slots

                        var freeinvslots = 0;
                        for (int invslot = 9; invslot <= 44; invslot++)
                        {
                            if (player.status.containers.inventory.GetAt(invslot).id == -1)
                            {
                                freeinvslots++;
                            }
                        }

                        if (freeinvslots != _oldInvSlots)
                        {
                            Bot_Inv_FSlots_Text.Clear();
                            Bot_Inv_FSlots_Text.AppendText(freeinvslots.ToString());
                            _oldInvSlots = freeinvslots;
                        }

                        if (_oldInvUSlots != 36 - freeinvslots)
                        {
                            _oldInvUSlots = 36 - _oldInvSlots;

                            Bot_Inv_USlots_Text.Clear();
                            Bot_Inv_USlots_Text.AppendText(_oldInvUSlots.ToString());
                        }

                        #endregion
                    }
                });
            }
            catch
            {

                //The form is already disposed, unhook.
                player.events.onInventoryChanged -= OnInventoryChanged;
            }
        }

        private void OnHungerUpdate(int hunger)
        {
            Bot_Hunger_Text.Clear();
            Bot_Hunger_Text.AppendText(hunger.ToString());
        }

        private void OnHealthUpdate(IPlayer player, float health, int food, float foodSaturation)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (player.events != null)
                            //Unhook, form closed.
                            player.events.onHealthUpdate -= OnHealthUpdate;
                    }
                    else
                    {
                        Bot_Health_Text.Clear();
                        Bot_Health_Text.AppendText(player.status.entity.health.ToString());
                    }
                });
            }
            catch
            {

                //The form is already disposed, unhook.
                player.events.onHealthUpdate -= OnHealthUpdate;
            }
        }

        private void OnChatMessage(IPlayer player, IChat message, byte position) {

            //If this isn't the chatbox then ignore it.
            if (position > 1) return;
            
            Console.WriteLine(message.GetTextRtf());

            //Try-cathc in case the form is already
            //disposed.
            try
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (IsDisposed || Disposing || !Log(message.GetText())) {
                        if (player.events != null)
                            //Unhook, form closed.
                            player.events.onChat -= OnChatMessage;
                    }
                    else {

                        // set the current caret position to the end
                        Chat_Box.SelectionStart = Chat_Box.Text.Length;
                        // scroll it automatically
                        Chat_Box.ScrollToCaret();
                    }
                });
            }
            catch {

                //The form is already disposed, unhook.
                player.events.onChat -= OnChatMessage;
            }
        }

        #region Functions

        private ILocation CurrentLocation()
        {
            return _player.status.entity.location.ToLocation();
        }

        #endregion

        #region GUI Stuff
        
        private readonly Queue<string> _logQueue = new Queue<string>();
        private const int LogMax = 100;

        private bool Log(string logText) {

            if (Chat_Box.IsDisposed || Chat_Box.Disposing) return false;

            // this should only ever run for 1 loop as you should never go over logMax
            // but if you accidentally manually added to the logQueue - then this would
            // re-adjust you back down to the desired number of log items.
            while (_logQueue.Count > LogMax - 1)
                _logQueue.Dequeue();

            _logQueue.Enqueue(logText);
            Chat_Box.Text = string.Join(Environment.NewLine,
                _logQueue.ToArray());
            
            return true;
        }

        public static class DebugConsole
        {
            public static void DebugText(IPlayer player, int priority, string text)
            {
                if (Debug_Console != null)
                {
                    if (Debug_Console.IsDisposed || Debug_Console.Disposing || !Debug_Console.Visible) return;

                    if (player.status.username == _currenBotusername)
                    {
                        var time = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second +
                                   "] ";

                        switch (priority)
                        {
                            case 0:
                                Debug_Console.SelectionColor = Color.Black;
                                Debug_Console.AppendText(time + text + "\n");
                                break;
                            case 1:
                                Debug_Console.SelectionColor = Color.DarkGreen;
                                Debug_Console.AppendText(time + text + "\n");
                                break;
                            case 2:
                                Debug_Console.SelectionColor = Color.Orange;
                                Debug_Console.AppendText(time + text + "\n");
                                break;
                            case 3:
                                Debug_Console.SelectionFont = new Font(Debug_Console.Font, FontStyle.Bold);
                                Debug_Console.SelectionColor = Color.DarkRed;
                                Debug_Console.AppendText(time + text + "\n");
                                break;
                        }
                    }
                }
            }

            public static void DebugValues(IPlayer player, Dictionary<string,string> debugDictionary)
            {
                if (Debug_Values != null)
                {
                    if (Debug_Values.IsDisposed || Debug_Values.Disposing || !Debug_Values.Visible) return;

                    if (player.status.username == _currenBotusername)
                    {
                        //TODO: Add mutli bot support
                        if(_oldDebugDictionary != null)
                        {
                            if (_oldDebugDictionary.Count == debugDictionary.Count &&
                                !_oldDebugDictionary.Except(debugDictionary).Any()) return;
                        }

                        Debug_Values.Clear();

                        foreach (var value in debugDictionary)
                        {
                                Debug_Values.AppendText(value.Key + ": " + value.Value + "\n");
                        }

                        _oldDebugDictionary = debugDictionary;
                    }
                }
            }
        }

        private void Chat_Message_KeyDown(object sender, KeyEventArgs e)
        {
            //CTRL+A is needed, we all know that
            if (e.Control && e.KeyCode == Keys.A)
            {
                Chat_Message.SelectAll();
            }

            //Send the message when we hit ENTER
            if (e.KeyCode == Keys.Enter)
            {

                //Do not send empty messages.
                if (string.IsNullOrWhiteSpace(Chat_Message.Text)) return;

                _player?.functions?.Chat(Chat_Message.Text);
                Chat_Message.Clear();
            }
        }

        private void Chat_Send_Click(object sender, EventArgs e) {
            Chat_Message_KeyDown(sender, new KeyEventArgs(Keys.Enter));
        }

        private void Chat_Box_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return; //click event

            //Menu stuff for the chatbox, maybe someone needs it idk ^^
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem("Copy");
            menuItem.Click += CopyAction;
            contextMenu.MenuItems.Add(menuItem);

            Chat_Box.ContextMenu = contextMenu;
        }

        private void CopyAction(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, Chat_Box.SelectedText);
        }

        private void Chat_Box_KeyDown(object sender, KeyEventArgs e)
        {
            //So that you can copy the text in the chatbox
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.Clear();
                Clipboard.SetData(DataFormats.Text, Chat_Box.SelectedText);
            }
        }

        #endregion
    }
}