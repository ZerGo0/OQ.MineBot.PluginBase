using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Classes.Entity.Mob;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.Protocols.Classes.Base;

namespace DebugPlugin
{
    public partial class DebugForm : Form
    {
        private static Dictionary<string, string> _OLD_DEBUG_DICTIONARY;

        private static string _CURREN_BOTUSERNAME;
        private readonly IBotContext _context;
        private ILocation _currentLocation;
        private string _oldclosestmobloc;
        private string _oldclosestplayerloc;
        private string _oldclosesttargetloc;
        private string _oldDimension;
        private string _oldGamemode;
        private bool _oldGrounded;
        private bool _oldGroundedFirstTime;
        private int _oldHealth;
        private bool _oldHealthFirstTime;
        private sbyte _oldHeldItemCount;
        private short _oldHeldItemId;
        private string _oldHeldItemNbt;
        private bool _oldHeldItemNbtFirstTime;
        private int _oldHeldItemSlot;
        private bool _oldInvFSlotsFirstTime;

        private bool _oldInvFull;
        private bool _oldInvFullFirstTime;
        private int _oldInvSlots;
        private int _oldInvUSlots;
        private bool _oldInWater;
        private bool _oldInWaterFirstTime;
        private bool _oldJumping;
        private bool _oldJumpingFirstTime;
        private IMobEntity _oldmobaround;
        private bool? _oldPathComplete;
        private bool? _oldPathDisposed;
        private bool? _oldPathSearched;
        private bool? _oldPathStopped;

        private string _oldPathTarget;
        private bool? _oldPathValid;

        private IPlayerEntity _oldplayeraround;
        private IEntity _oldtargetaround;
        private int _oldWindowSlotsUsed;

        private string _oldWindowTitle;
        private bool _setAlive;

        public DebugForm(string name, IBotContext context)
        {
            InitializeComponent();

            KeyDown += (sender, args) =>
            {
                args.SuppressKeyPress = true; // Disable sound.
            };
            Chat_Box.KeyDown += (sender, args) =>
            {
                args.SuppressKeyPress = true; // Disable sound.
            };

            _context = context;

            _context.Events.onChat += OnChatMessage;
            _context.Events.onHealthUpdate += OnHealthUpdate;
            _context.Events.onTick += Events_onTick;
            _context.Events.onInventoryChanged += OnInventoryChanged;

            _context.Events.onDisconnected +=
                (player1, reason) =>
                {
                    if (!IsDisposed && !Disposing)
                        BeginInvoke((MethodInvoker) delegate
                        {
                            if (!IsDisposed && !Disposing) Close();
                        });
                };

            _CURREN_BOTUSERNAME = _context.Player.GetUsername();
            Bot_Name_Text.AppendText(_context.Player.GetUsername());
            Bot_UUID_Text.AppendText(_context.Player.GetUuid());

            Bot_Health_Text.AppendText(_context.Player.GetHealth().ToString(CultureInfo.InvariantCulture));

            _oldPathTarget = null;
            _oldPathComplete = null;
            _oldPathDisposed = null;
            _oldPathSearched = null;
            _oldPathStopped = null;
            _oldPathValid = null;

            Text = "[Bot: " + name + "] Debug Menu";
        }

        private void Events_onTick(IBotContext context)
        {
            try
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (context.Events != null)
                            //Unhook, form closed.
                            context.Events.onTick -= Events_onTick;
                    }
                    else
                    {
                        try
                        {
                            #region Bot General Stuff

                            #region Bot Hunger Update

                            if (_oldHealth != (int) context.Player.GetHealth() || !_oldHealthFirstTime)
                            {
                                OnHungerUpdate((int) context.Player.GetHealth());
                                OnHungerUpdate((int) context.Player.GetFood());

                                _oldHealth = (int) context.Player.GetHealth();
                                _oldHealthFirstTime = true;
                            }

                            #endregion

                            #region Bot Location

                            //Update it only when needed
                            if (CurrentLocation().Distance(_currentLocation) > 0 || _currentLocation == null)
                            {
                                Bot_Location_Text.Clear();
                                Bot_Location_Text.AppendText(CurrentLocation().ToString());
                                _currentLocation = CurrentLocation();
                            }

                            #endregion

                            #region Bot Dimension

                            //TODO: Add this back when it get's added to the IPlayerController.cs
                            //Update it only when needed
                            // if (context.Entities.dimension.ToString() != _oldDimension || string.IsNullOrEmpty(_oldDimension))
                            // {
                            //     Bot_Dimension_Text.Clear();
                            //     Bot_Dimension_Text.AppendText(player.status.entity.dimension.ToString());
                            //     _oldDimension = player.status.entity.dimension.ToString();
                            // }

                            #endregion

                            #region Bot Gamemode

                            //TODO: Check if it get's fixed in the next updates, it doesn't update the
                            //TODO: value when the gamemode get's change ingame
                            //Update it only when needed
                            if (context.Player.GetGamemode().ToString() != _oldGamemode ||
                                string.IsNullOrEmpty(_oldGamemode))
                            {
                                Bot_Gamemode_Text.Clear();
                                Bot_Gamemode_Text.AppendText(context.Player.GetGamemode().ToString());
                                _oldGamemode = context.Player.GetGamemode().ToString();
                            }

                            #endregion

                            #region Bot Alive

                            if (context.Player.IsDead() && _setAlive)
                            {
                                Bot_Dead_Text.Clear();
                                Bot_Dead_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Bold);
                                Bot_Dead_Text.SelectionColor = Color.DarkRed;
                                Bot_Dead_Text.AppendText(context.Player.IsDead().ToString());
                                _setAlive = false;
                            }
                            else if (!context.Player.IsDead() && !_setAlive)
                            {
                                Bot_Dead_Text.Clear();
                                Bot_Dead_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Regular);
                                Bot_Dead_Text.SelectionColor = Color.Black;
                                Bot_Dead_Text.AppendText(context.Player.IsDead().ToString());
                                _setAlive = true;
                            }

                            #endregion

                            #region Bot Grounded

                            if (context.Player.PhysicsEngine.isGrounded != _oldGrounded || !_oldGroundedFirstTime)
                            {
                                Bot_Grounded_Text.Clear();
                                Bot_Grounded_Text.AppendText(context.Player.PhysicsEngine.isGrounded.ToString());

                                _oldGrounded = context.Player.PhysicsEngine.isGrounded;
                                _oldGroundedFirstTime = true;
                            }

                            #endregion


                            #region Bot in Water

                            if (context.Player.PhysicsEngine.inWater != _oldInWater || !_oldInWaterFirstTime)
                            {
                                Bot_In_Water_Text.Clear();
                                Bot_In_Water_Text.AppendText(context.Player.PhysicsEngine.inWater.ToString());

                                _oldInWater = context.Player.PhysicsEngine.inWater;
                                _oldInWaterFirstTime = true;
                            }

                            #endregion

                            #region Bot Jumping

                            if (context.Player.PhysicsEngine.jumping != _oldJumping || !_oldJumpingFirstTime)
                            {
                                Bot_Jumping_Text.Clear();
                                Bot_Jumping_Text.AppendText(context.Player.PhysicsEngine.jumping.ToString());

                                _oldJumping = context.Player.PhysicsEngine.jumping;
                                _oldJumpingFirstTime = true;
                            }

                            #endregion

                            #endregion

                            #region Bot Inv Stuff

                            #region Bot Inv Full

                            if (context.Containers.GetInventory().IsFull() && _oldInvFull ||
                                context.Containers.GetInventory().IsFull() && !_oldInvFullFirstTime)
                            {
                                Bot_Inv_Full_Text.Clear();
                                Bot_Inv_Full_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Bold);
                                Bot_Inv_Full_Text.SelectionColor = Color.DarkRed;
                                Bot_Inv_Full_Text.AppendText(context.Containers.GetInventory().IsFull().ToString());
                                _oldInvFull = false;
                                _oldInvFullFirstTime = true;
                            }
                            else if (!context.Containers.GetInventory().IsFull() && !_oldInvFull)
                            {
                                Bot_Inv_Full_Text.Clear();
                                Bot_Inv_Full_Text.SelectionFont = new Font(Bot_Dead_Text.Font, FontStyle.Regular);
                                Bot_Inv_Full_Text.SelectionColor = Color.Black;
                                Bot_Inv_Full_Text.AppendText(context.Containers.GetInventory().IsFull().ToString());
                                _oldInvFull = true;
                            }

                            #endregion

                            #region Bot Free/Used Slots Firsttime

                            if (!_oldInvFSlotsFirstTime)
                            {
                                var freeinvslots = 0;
                                for (var invslot = 9; invslot <= 44; invslot++)
                                    if (context.Containers.GetInventory().GetAt(invslot).Id == -1)
                                        freeinvslots++;

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

                            var helditemid = context.Player.GetHeldSlot().Id;

                            if (helditemid != _oldHeldItemId)
                            {
                                Held_Item_ID_Text.Clear();
                                Held_Item_ID_Text.AppendText(helditemid.ToString());

                                _oldHeldItemId = helditemid;
                            }

                            #endregion

                            #region Bot Held Item NBT

                            var helditemnbt = context.Player.GetHeldSlot().Nbt;

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

                            var helditemcount = context.Player.GetHeldSlot().Count;

                            if (helditemcount != _oldHeldItemCount)
                            {
                                Held_Item_Count_Text.Clear();
                                Held_Item_Count_Text.AppendText(helditemcount.ToString());

                                _oldHeldItemCount = helditemcount;
                            }

                            #endregion

                            #region Bot Held Item Slot

                            if (context.Player.GetHeldSlot().Index != _oldHeldItemSlot)
                            {
                                Held_Item_Slot_Text.Clear();
                                Held_Item_Slot_Text.AppendText(context.Player.GetHeldSlot().Index.ToString());

                                _oldHeldItemSlot = context.Player.GetHeldSlot().Index;
                            }

                            #endregion

                            #endregion

                            #region Bot Closest Stuff

                            #region Bot Closest Player

                            var playeraround = context.Entities.GetClosestPlayer();

                            if (playeraround != null)
                            {
                                var playeraroundloc = new Location((int) playeraround.Position.X,
                                    (int) playeraround.Position.Y, (int) playeraround.Position.Z);

                                if (playeraroundloc.ToString() != _oldclosestplayerloc)
                                {
                                    Closest_Player_Loc_Text.Clear();
                                    Closest_Player_Loc_Text.AppendText(playeraroundloc.ToString());

                                    Closest_Player_Dist_Text.Clear();
                                    Closest_Player_Dist_Text.AppendText(CurrentLocation().Distance(playeraroundloc)
                                        .ToString(CultureInfo.InvariantCulture));

                                    _oldclosestplayerloc = playeraroundloc.ToString();
                                }

                                if (playeraround != _oldplayeraround ||
                                    string.IsNullOrWhiteSpace(Targetable_Player_Text.Text))
                                {
                                    Closest_Player_Text.Clear();
                                    if (playeraround.GetName().Length > 0)
                                        Closest_Player_Text.AppendText(playeraround.GetName());

                                    Closest_Player_UUID_Text.Clear();
                                    Closest_Player_UUID_Text.AppendText(playeraround.GetUuid());

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

                            var mobaround = context.Entities.GetClosestMob();

                            if (mobaround != null)
                            {
                                var mobaroundloc = new Location((int) mobaround.Position.X,
                                    (int) mobaround.Position.Y, (int) mobaround.Position.Z);

                                if (mobaroundloc.ToString() != _oldclosestmobloc)
                                {
                                    Closest_Mob_Loc_Text.Clear();
                                    Closest_Mob_Loc_Text.AppendText(mobaroundloc.ToString());

                                    Closest_Mob_Dist_Text.Clear();
                                    Closest_Mob_Dist_Text.AppendText(CurrentLocation().Distance(mobaroundloc)
                                        .ToString(CultureInfo.InvariantCulture));

                                    _oldclosestmobloc = mobaroundloc.ToString();
                                }

                                if (mobaround != _oldmobaround)
                                {
                                    Closest_Mob_Text.Clear();
                                    Closest_Mob_Text.AppendText(mobaround.MobType.ToString());

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

                            var targetObject = context.Entities.GetClosestObject();

                            if (targetObject != null)
                            {
                                var targetaroundloc = new Location((int) targetObject.Position.X,
                                    (int) targetObject.Position.Y, (int) targetObject.Position.Z);

                                if (targetaroundloc.ToString() != _oldclosesttargetloc ||
                                    string.IsNullOrWhiteSpace(Targetable_Player_Loc_Text.Text))
                                {
                                    Targetable_Player_Loc_Text.Clear();
                                    Targetable_Player_Loc_Text.AppendText(targetaroundloc.ToString());

                                    Targetable_Player_Dist_Text.Clear();
                                    Targetable_Player_Dist_Text.AppendText(CurrentLocation().Distance(targetaroundloc)
                                        .ToString(CultureInfo.InvariantCulture));

                                    _oldclosesttargetloc = targetaroundloc.ToString();
                                }

                                if (targetObject != _oldtargetaround ||
                                    string.IsNullOrWhiteSpace(Targetable_Player_Text.Text))
                                {
                                    Targetable_Player_Text.Clear();
                                    if (targetObject != null)
                                        Targetable_Player_Text.AppendText(targetObject.Type.ToString());

                                    //TODO: Check if this is right
                                    Targetable_Player_UUID_Text.Clear();
                                    Targetable_Player_UUID_Text.AppendText(targetObject.Object.Id.ToString());

                                    _oldtargetaround = targetObject;
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
                            for (var getwindowid = 9999; getwindowid > 0; getwindowid--)
                                if (context.Containers.GetWindow(getwindowid) != null)
                                {
                                    var winSlotCount = context.Containers.GetWindow(getwindowid).GetSlotCount();

                                    for (var windowslots = 0; windowslots <= winSlotCount + 36; windowslots++)
                                    {
                                        if (context.Containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                            continue;

                                        if (context.Containers.GetWindow(getwindowid).GetAt(windowslots).Id != -1)
                                            winSlotsUsed++;
                                    }

                                    if (context.Containers.GetWindow(getwindowid).GetWindowName().Raw !=
                                        _oldWindowTitle &&
                                        context.Containers.GetWindow(getwindowid).GetWindowType() != "Inventory" ||
                                        context.Containers.GetWindow(getwindowid).GetWindowType() != "Inventory" &&
                                        winSlotsUsed != _oldWindowSlotsUsed)
                                    {
                                        var winTitle = context.Containers.GetWindow(getwindowid).GetWindowName().Raw;
                                        var winType = context.Containers.GetWindow(getwindowid).GetWindowType();
                                        var winId = context.Containers.GetWindow(getwindowid).GetId();
                                        var winEntityId = "Outdated";
                                        var winmActionId = "Outdated";

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
                                        Window_EntityID_Text.AppendText(winEntityId);
                                        Window_ActionID_Text.AppendText(winmActionId);

                                        for (var windowslots = 0; windowslots <= winSlotCount - 1; windowslots++)
                                        {
                                            if (context.Containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                                continue;

                                            if (context.Containers.GetWindow(getwindowid).GetAt(windowslots).Id !=
                                                -1)
                                            {
                                                Window_Slotids_Text.AppendText(
                                                    "Slot: " + windowslots + "   |   ItemID: " +
                                                    context.Containers.GetWindow(getwindowid)
                                                        .GetAt(windowslots).Id
                                                    + "   |   Metadata: " +
                                                    context.Containers.GetWindow(getwindowid)
                                                        .GetAt(windowslots).Damage + "\n");

                                                if (context.Containers.GetWindow(getwindowid).GetAt(windowslots).Nbt !=
                                                    null)
                                                {
                                                    var windowNbt = context.Containers.GetWindow(getwindowid)
                                                        .GetAt(windowslots).Nbt.ToString();
                                                    Window_Slot_NBT_Text.AppendText(
                                                        "Slot: " + windowslots + "   |   ItemID: " +
                                                        context.Containers.GetWindow(getwindowid).GetAt(windowslots)
                                                            .Id +
                                                        "   |   ItemNBT: \n" + windowNbt);
                                                }
                                            }
                                        }

                                        for (var windowslots = winSlotCount;
                                            windowslots <= winSlotCount + 35;
                                            windowslots++)
                                        {
                                            if (context.Containers.GetWindow(getwindowid).GetAt(windowslots) == null)
                                                continue;

                                            if (context.Containers.GetWindow(getwindowid).GetAt(windowslots).Id !=
                                                -1)
                                                Window_Inv_Slotids_Text.AppendText(
                                                    "Slot: " + windowslots + "   |   ItemID: " +
                                                    context.Containers.GetWindow(getwindowid).GetAt(windowslots).Id
                                                    + "   |   Metadata: " +
                                                    context.Containers.GetWindow(getwindowid).GetAt(windowslots)
                                                        .Damage +
                                                    "\n");
                                        }

                                        _oldWindowTitle = winTitle;
                                        _oldWindowSlotsUsed = winSlotsUsed;
                                    }

                                    break;
                                }

                            #endregion

                            #region Path Stuff

                            if (context.Player.PhysicsEngine.path != null)
                                if (context.Player.PhysicsEngine.path.Target.ToString() != _oldPathTarget ||
                                    context.Player.PhysicsEngine.path.Complete != _oldPathComplete ||
                                    context.Player.PhysicsEngine.path.Disposed != _oldPathDisposed ||
                                    context.Player.PhysicsEngine.path.Searched != _oldPathSearched ||
                                    context.Player.PhysicsEngine.path.Token.stopped != _oldPathStopped ||
                                    context.Player.PhysicsEngine.path.Valid != _oldPathValid)
                                {
                                    Current_Path_Target_Loc_Text.Clear();
                                    Current_Path_Complete_Text.Clear();
                                    Current_Path_Disposed_Text.Clear();
                                    Current_Path_Offset_Text.Clear();
                                    Current_Path_Options_Text.Clear();
                                    Current_Path_Searched_Text.Clear();
                                    Current_Path_Token_Stopped_Text.Clear();
                                    Current_Path_Valid_Text.Clear();


                                    Current_Path_Target_Loc_Text.AppendText(context.Player.PhysicsEngine.path.Target
                                        .ToString());
                                    Current_Path_Complete_Text.AppendText(context.Player.PhysicsEngine.path.Complete
                                        .ToString());
                                    Current_Path_Disposed_Text.AppendText(context.Player.PhysicsEngine.path.Disposed
                                        .ToString());

                                    if (context.Player.PhysicsEngine.path.Offset != null)
                                        Current_Path_Offset_Text.AppendText(context.Player.PhysicsEngine.path.Offset
                                            .ToString());

                                    Current_Path_Searched_Text.AppendText(context.Player.PhysicsEngine.path.Searched
                                        .ToString());
                                    Current_Path_Token_Stopped_Text.AppendText(context.Player.PhysicsEngine.path.Token
                                        .stopped.ToString());
                                    Current_Path_Valid_Text.AppendText(
                                        context.Player.PhysicsEngine.path.Valid.ToString());

                                    Current_Path_Options_Text.AppendText(
                                        "AntiStuck: " + context.Player.PhysicsEngine.path.Options.AntiStuck + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Climb: " + context.Player.PhysicsEngine.path.Options.Climb + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "EdgeStick: " + context.Player.PhysicsEngine.path.Options.EdgeStick + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Fly: " + context.Player.PhysicsEngine.path.Options.Fly + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Look: " + context.Player.PhysicsEngine.path.Options.Look + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Mine: " + context.Player.PhysicsEngine.path.Options.Mine + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "NoCost: " + context.Player.PhysicsEngine.path.Options.NoCost + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "NoSlowdown: " + context.Player.PhysicsEngine.path.Options.NoSlowdown + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "PauseOnNext: " + context.Player.PhysicsEngine.path.Options.PauseOnNext + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Quality: " + context.Player.PhysicsEngine.path.Options.Quality + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "SafeMine: " + context.Player.PhysicsEngine.path.Options.SafeMine + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Sprint: " + context.Player.PhysicsEngine.path.Options.Sprint + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "SprintVisuals: " + context.Player.PhysicsEngine.path.Options.SprintVisuals +
                                        "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Strict: " + context.Player.PhysicsEngine.path.Options.Strict + "\n");
                                    Current_Path_Options_Text.AppendText(
                                        "Swim: " + context.Player.PhysicsEngine.path.Options.Swim + "\n");

                                    _oldPathTarget = context.Player.PhysicsEngine.path.Target.ToString();
                                    _oldPathComplete = context.Player.PhysicsEngine.path.Complete;
                                    _oldPathDisposed = context.Player.PhysicsEngine.path.Disposed;
                                    _oldPathSearched = context.Player.PhysicsEngine.path.Searched;
                                    _oldPathStopped = context.Player.PhysicsEngine.path.Token.stopped;
                                    _oldPathValid = context.Player.PhysicsEngine.path.Valid;
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
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                });
            }
            catch
            {
                //The form is already disposed, unhook.
                context.Events.onTick -= Events_onTick;
            }
        }

        private void OnInventoryChanged(IBotContext context, bool changed, bool removed, ushort id, int countdifference,
            ISlot slot)
        {
            try
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (context.Events != null)
                            //Unhook, form closed.
                            context.Events.onInventoryChanged -= OnInventoryChanged;
                    }
                    else
                    {
                        #region Bot Free/Used Slots

                        var freeinvslots = 0;
                        for (var invslot = 9; invslot <= 44; invslot++)
                            if (context.Containers.GetInventory().GetAt(invslot).Id == -1)
                                freeinvslots++;

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
                context.Events.onInventoryChanged -= OnInventoryChanged;
            }
        }

        private void OnHungerUpdate(int hunger)
        {
            Bot_Hunger_Text.Clear();
            Bot_Hunger_Text.AppendText(hunger.ToString());
        }

        private void OnHealthUpdate(IBotContext context, float health, int food, float foodSaturation)
        {
            try
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (IsDisposed || Disposing)
                    {
                        if (context.Events != null)
                            //Unhook, form closed.
                            context.Events.onHealthUpdate -= OnHealthUpdate;
                    }
                    else
                    {
                        Bot_Health_Text.Clear();
                        Bot_Health_Text.AppendText(context.Player.GetHealth().ToString(CultureInfo.InvariantCulture));
                    }
                });
            }
            catch
            {
                //The form is already disposed, unhook.
                context.Events.onHealthUpdate -= OnHealthUpdate;
            }
        }

        private void OnChatMessage(IBotContext context, IChat message, byte position)
        {
            //If this isn't the chatbox then ignore it.
            if (position > 1) return;

            //Console.WriteLine(message.GetTextRtf());

            //Try-cathc in case the form is already
            //disposed.
            try
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (IsDisposed || Disposing || !Log(message.GetText()))
                    {
                        if (context.Events != null)
                            //Unhook, form closed.
                            context.Events.onChat -= OnChatMessage;
                    }
                    else
                    {
                        // set the current caret position to the end
                        Chat_Box.SelectionStart = Chat_Box.Text.Length;
                        // scroll it automatically
                        Chat_Box.ScrollToCaret();
                    }
                });
            }
            catch
            {
                //The form is already disposed, unhook.
                context.Events.onChat -= OnChatMessage;
            }
        }

        #region Functions

        private ILocation CurrentLocation()
        {
            return _context.Player.GetLocation();
        }

        #endregion

        #region GUI Stuff

        private readonly Queue<string> _logQueue = new Queue<string>();
        private const int LogMax = 100;

        private bool Log(string logText)
        {
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

        private void Chat_Message_KeyDown(object sender, KeyEventArgs e)
        {
            //CTRL+A is needed, we all know that
            if (e.Control && e.KeyCode == Keys.A) Chat_Message.SelectAll();

            //Send the message when we hit ENTER
            if (e.KeyCode == Keys.Enter)
            {
                //Do not send empty messages.
                if (string.IsNullOrWhiteSpace(Chat_Message.Text)) return;

                _context?.Functions?.Chat(Chat_Message.Text);
                Chat_Message.Clear();
            }
        }

        private void Chat_Send_Click(object sender, EventArgs e)
        {
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