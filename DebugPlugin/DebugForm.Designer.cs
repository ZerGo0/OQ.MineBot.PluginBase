using System.Drawing;
using System.Windows.Forms;

namespace DebugPlugin
{
    partial class DebugForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Chat_Message = new System.Windows.Forms.TextBox();
            this.Chat_Box = new System.Windows.Forms.RichTextBox();
            this.Bot_Name = new System.Windows.Forms.Label();
            this.Bot_Name_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Health = new System.Windows.Forms.Label();
            this.Bot_Hunger = new System.Windows.Forms.Label();
            this.Bot_Health_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Hunger_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Location = new System.Windows.Forms.Label();
            this.Bot_Location_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Dimension = new System.Windows.Forms.Label();
            this.Bot_Dimension_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Gamemode = new System.Windows.Forms.Label();
            this.Bot_Gamemode_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_UUID = new System.Windows.Forms.Label();
            this.Bot_UUID_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Dead = new System.Windows.Forms.Label();
            this.Bot_Dead_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Inv_Full = new System.Windows.Forms.Label();
            this.Bot_Inv_FSlots = new System.Windows.Forms.Label();
            this.Bot_Inv_FSlots_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Inv_Full_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Inv_USlots = new System.Windows.Forms.Label();
            this.Bot_Inv_USlots_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Player = new System.Windows.Forms.Label();
            this.Closest_Player_UUID = new System.Windows.Forms.Label();
            this.Closest_Player_Loc = new System.Windows.Forms.Label();
            this.Closest_Player_Dist = new System.Windows.Forms.Label();
            this.Closest_Player_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Player_UUID_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Player_Loc_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Player_Dist_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Mob = new System.Windows.Forms.Label();
            this.Closest_Mob_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Mob_Loc = new System.Windows.Forms.Label();
            this.Closest_Mob_Loc_Text = new System.Windows.Forms.RichTextBox();
            this.Closest_Mob_Dist = new System.Windows.Forms.Label();
            this.Closest_Mob_Dist_Text = new System.Windows.Forms.RichTextBox();
            this.Targetable_Player_Text = new System.Windows.Forms.RichTextBox();
            this.Targetable_Player_UUID_Text = new System.Windows.Forms.RichTextBox();
            this.Targetable_Player_Loc_Text = new System.Windows.Forms.RichTextBox();
            this.Targetable_Player_Dist_Text = new System.Windows.Forms.RichTextBox();
            this.Targetable_Player = new System.Windows.Forms.Label();
            this.Targetable_Player_UUID = new System.Windows.Forms.Label();
            this.Targetable_Player_Loc = new System.Windows.Forms.Label();
            this.Targetable_Player_Dist = new System.Windows.Forms.Label();
            this.Held_Item_ID = new System.Windows.Forms.Label();
            this.Held_Item_ID_Text = new System.Windows.Forms.RichTextBox();
            this.Held_Item_Count = new System.Windows.Forms.Label();
            this.Held_Item_Count_Text = new System.Windows.Forms.RichTextBox();
            this.Held_Item_Slot = new System.Windows.Forms.Label();
            this.Held_Item_Slot_Text = new System.Windows.Forms.RichTextBox();
            this.Held_Item_NBT = new System.Windows.Forms.Label();
            this.Held_Item_NBT_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Title = new System.Windows.Forms.Label();
            this.Window_Title_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Type = new System.Windows.Forms.Label();
            this.Window_ID = new System.Windows.Forms.Label();
            this.Window_Type_Text = new System.Windows.Forms.RichTextBox();
            this.Window_ID_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Slotcount_Text = new System.Windows.Forms.RichTextBox();
            this.Window_EntityID_Text = new System.Windows.Forms.RichTextBox();
            this.Window_ActionID_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Slotcount = new System.Windows.Forms.Label();
            this.Window_EntityID = new System.Windows.Forms.Label();
            this.Window_ActionID = new System.Windows.Forms.Label();
            this.Window_Slotids = new System.Windows.Forms.Label();
            this.Window_Slotids_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Inv_Slotids = new System.Windows.Forms.Label();
            this.Window_Inv_Slotids_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Target_Loc = new System.Windows.Forms.Label();
            this.Current_Path_Target_Loc_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Complete = new System.Windows.Forms.Label();
            this.Current_Path_Complete_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Disposed_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Disposed = new System.Windows.Forms.Label();
            this.Current_Path_Offset_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Offset = new System.Windows.Forms.Label();
            this.Current_Path_Options_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Options = new System.Windows.Forms.Label();
            this.Current_Path_Searched_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Searched = new System.Windows.Forms.Label();
            this.Current_Path_Token_Stopped_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Token_Stopped = new System.Windows.Forms.Label();
            this.Current_Path_Valid_Text = new System.Windows.Forms.RichTextBox();
            this.Current_Path_Valid = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Bot_Jumping = new System.Windows.Forms.Label();
            this.Bot_Jumping_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_In_Water_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_In_Water = new System.Windows.Forms.Label();
            this.Bot_Grounded_Text = new System.Windows.Forms.RichTextBox();
            this.Bot_Grounded = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.Window_Slot_NBT_Text = new System.Windows.Forms.RichTextBox();
            this.Window_Slot_NBT = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.Debug_Console = new System.Windows.Forms.RichTextBox();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.Debug_Values = new System.Windows.Forms.RichTextBox();
            this.Chat_Send = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // Chat_Message
            // 
            this.Chat_Message.AcceptsReturn = true;
            this.Chat_Message.AcceptsTab = true;
            this.Chat_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Chat_Message.Location = new System.Drawing.Point(15, 391);
            this.Chat_Message.MaxLength = 256;
            this.Chat_Message.Name = "Chat_Message";
            this.Chat_Message.Size = new System.Drawing.Size(441, 23);
            this.Chat_Message.TabIndex = 1;
            this.Chat_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chat_Message_KeyDown);
            // 
            // Chat_Box
            // 
            this.Chat_Box.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (224)))), ((int) (((byte) (224)))),
                ((int) (((byte) (224)))));
            this.Chat_Box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Chat_Box.HideSelection = false;
            this.Chat_Box.Location = new System.Drawing.Point(15, 8);
            this.Chat_Box.Name = "Chat_Box";
            this.Chat_Box.Size = new System.Drawing.Size(535, 376);
            this.Chat_Box.TabIndex = 3;
            this.Chat_Box.Text = "";
            this.Chat_Box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chat_Box_KeyDown);
            this.Chat_Box.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chat_Box_MouseUp);
            // 
            // Bot_Name
            // 
            this.Bot_Name.AutoSize = true;
            this.Bot_Name.Location = new System.Drawing.Point(3, 10);
            this.Bot_Name.Name = "Bot_Name";
            this.Bot_Name.Size = new System.Drawing.Size(58, 15);
            this.Bot_Name.TabIndex = 4;
            this.Bot_Name.Text = "Botname:";
            // 
            // Bot_Name_Text
            // 
            this.Bot_Name_Text.HideSelection = false;
            this.Bot_Name_Text.Location = new System.Drawing.Point(107, 7);
            this.Bot_Name_Text.Name = "Bot_Name_Text";
            this.Bot_Name_Text.ReadOnly = true;
            this.Bot_Name_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Name_Text.Size = new System.Drawing.Size(168, 25);
            this.Bot_Name_Text.TabIndex = 5;
            this.Bot_Name_Text.Text = "";
            // 
            // Bot_Health
            // 
            this.Bot_Health.AutoSize = true;
            this.Bot_Health.Location = new System.Drawing.Point(3, 87);
            this.Bot_Health.Name = "Bot_Health";
            this.Bot_Health.Size = new System.Drawing.Size(66, 15);
            this.Bot_Health.TabIndex = 6;
            this.Bot_Health.Text = "Bot Health:";
            // 
            // Bot_Hunger
            // 
            this.Bot_Hunger.AutoSize = true;
            this.Bot_Hunger.Location = new System.Drawing.Point(3, 119);
            this.Bot_Hunger.Name = "Bot_Hunger";
            this.Bot_Hunger.Size = new System.Drawing.Size(71, 15);
            this.Bot_Hunger.TabIndex = 7;
            this.Bot_Hunger.Text = "Bot Hunger:";
            // 
            // Bot_Health_Text
            // 
            this.Bot_Health_Text.HideSelection = false;
            this.Bot_Health_Text.Location = new System.Drawing.Point(107, 83);
            this.Bot_Health_Text.Name = "Bot_Health_Text";
            this.Bot_Health_Text.ReadOnly = true;
            this.Bot_Health_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Health_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Health_Text.TabIndex = 8;
            this.Bot_Health_Text.Text = "";
            // 
            // Bot_Hunger_Text
            // 
            this.Bot_Hunger_Text.HideSelection = false;
            this.Bot_Hunger_Text.Location = new System.Drawing.Point(107, 115);
            this.Bot_Hunger_Text.Name = "Bot_Hunger_Text";
            this.Bot_Hunger_Text.ReadOnly = true;
            this.Bot_Hunger_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Hunger_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Hunger_Text.TabIndex = 9;
            this.Bot_Hunger_Text.Text = "";
            // 
            // Bot_Location
            // 
            this.Bot_Location.AutoSize = true;
            this.Bot_Location.Location = new System.Drawing.Point(3, 151);
            this.Bot_Location.Name = "Bot_Location";
            this.Bot_Location.Size = new System.Drawing.Size(77, 15);
            this.Bot_Location.TabIndex = 10;
            this.Bot_Location.Text = "Bot Location:";
            // 
            // Bot_Location_Text
            // 
            this.Bot_Location_Text.HideSelection = false;
            this.Bot_Location_Text.Location = new System.Drawing.Point(107, 148);
            this.Bot_Location_Text.Name = "Bot_Location_Text";
            this.Bot_Location_Text.ReadOnly = true;
            this.Bot_Location_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Location_Text.Size = new System.Drawing.Size(168, 25);
            this.Bot_Location_Text.TabIndex = 11;
            this.Bot_Location_Text.Text = "";
            // 
            // Bot_Dimension
            // 
            this.Bot_Dimension.AutoSize = true;
            this.Bot_Dimension.Location = new System.Drawing.Point(3, 183);
            this.Bot_Dimension.Name = "Bot_Dimension";
            this.Bot_Dimension.Size = new System.Drawing.Size(88, 15);
            this.Bot_Dimension.TabIndex = 12;
            this.Bot_Dimension.Text = "Bot Dimension:";
            // 
            // Bot_Dimension_Text
            // 
            this.Bot_Dimension_Text.HideSelection = false;
            this.Bot_Dimension_Text.Location = new System.Drawing.Point(107, 180);
            this.Bot_Dimension_Text.Name = "Bot_Dimension_Text";
            this.Bot_Dimension_Text.ReadOnly = true;
            this.Bot_Dimension_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Dimension_Text.Size = new System.Drawing.Size(168, 25);
            this.Bot_Dimension_Text.TabIndex = 13;
            this.Bot_Dimension_Text.Text = "";
            // 
            // Bot_Gamemode
            // 
            this.Bot_Gamemode.AutoSize = true;
            this.Bot_Gamemode.Location = new System.Drawing.Point(3, 216);
            this.Bot_Gamemode.Name = "Bot_Gamemode";
            this.Bot_Gamemode.Size = new System.Drawing.Size(93, 15);
            this.Bot_Gamemode.TabIndex = 14;
            this.Bot_Gamemode.Text = "Bot Gamemode:";
            // 
            // Bot_Gamemode_Text
            // 
            this.Bot_Gamemode_Text.HideSelection = false;
            this.Bot_Gamemode_Text.Location = new System.Drawing.Point(107, 212);
            this.Bot_Gamemode_Text.Name = "Bot_Gamemode_Text";
            this.Bot_Gamemode_Text.ReadOnly = true;
            this.Bot_Gamemode_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Gamemode_Text.Size = new System.Drawing.Size(168, 25);
            this.Bot_Gamemode_Text.TabIndex = 15;
            this.Bot_Gamemode_Text.Text = "";
            // 
            // Bot_UUID
            // 
            this.Bot_UUID.AutoSize = true;
            this.Bot_UUID.Location = new System.Drawing.Point(3, 43);
            this.Bot_UUID.Name = "Bot_UUID";
            this.Bot_UUID.Size = new System.Drawing.Size(58, 15);
            this.Bot_UUID.TabIndex = 16;
            this.Bot_UUID.Text = "Bot UUID:";
            // 
            // Bot_UUID_Text
            // 
            this.Bot_UUID_Text.HideSelection = false;
            this.Bot_UUID_Text.Location = new System.Drawing.Point(107, 39);
            this.Bot_UUID_Text.Name = "Bot_UUID_Text";
            this.Bot_UUID_Text.ReadOnly = true;
            this.Bot_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_UUID_Text.Size = new System.Drawing.Size(168, 36);
            this.Bot_UUID_Text.TabIndex = 17;
            this.Bot_UUID_Text.Text = "";
            // 
            // Bot_Dead
            // 
            this.Bot_Dead.AutoSize = true;
            this.Bot_Dead.Location = new System.Drawing.Point(3, 248);
            this.Bot_Dead.Name = "Bot_Dead";
            this.Bot_Dead.Size = new System.Drawing.Size(58, 15);
            this.Bot_Dead.TabIndex = 18;
            this.Bot_Dead.Text = "Bot Dead:";
            // 
            // Bot_Dead_Text
            // 
            this.Bot_Dead_Text.HideSelection = false;
            this.Bot_Dead_Text.Location = new System.Drawing.Point(107, 245);
            this.Bot_Dead_Text.Name = "Bot_Dead_Text";
            this.Bot_Dead_Text.ReadOnly = true;
            this.Bot_Dead_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Dead_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Dead_Text.TabIndex = 19;
            this.Bot_Dead_Text.Text = "";
            // 
            // Bot_Inv_Full
            // 
            this.Bot_Inv_Full.AutoSize = true;
            this.Bot_Inv_Full.Location = new System.Drawing.Point(3, 10);
            this.Bot_Inv_Full.Name = "Bot_Inv_Full";
            this.Bot_Inv_Full.Size = new System.Drawing.Size(69, 15);
            this.Bot_Inv_Full.TabIndex = 20;
            this.Bot_Inv_Full.Text = "Bot Inv Full:";
            // 
            // Bot_Inv_FSlots
            // 
            this.Bot_Inv_FSlots.AutoSize = true;
            this.Bot_Inv_FSlots.Location = new System.Drawing.Point(3, 43);
            this.Bot_Inv_FSlots.Name = "Bot_Inv_FSlots";
            this.Bot_Inv_FSlots.Size = new System.Drawing.Size(81, 15);
            this.Bot_Inv_FSlots.TabIndex = 21;
            this.Bot_Inv_FSlots.Text = "Bot Free Slots:";
            // 
            // Bot_Inv_FSlots_Text
            // 
            this.Bot_Inv_FSlots_Text.HideSelection = false;
            this.Bot_Inv_FSlots_Text.Location = new System.Drawing.Point(111, 39);
            this.Bot_Inv_FSlots_Text.Name = "Bot_Inv_FSlots_Text";
            this.Bot_Inv_FSlots_Text.ReadOnly = true;
            this.Bot_Inv_FSlots_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_FSlots_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Inv_FSlots_Text.TabIndex = 22;
            this.Bot_Inv_FSlots_Text.Text = "";
            // 
            // Bot_Inv_Full_Text
            // 
            this.Bot_Inv_Full_Text.HideSelection = false;
            this.Bot_Inv_Full_Text.Location = new System.Drawing.Point(111, 7);
            this.Bot_Inv_Full_Text.Name = "Bot_Inv_Full_Text";
            this.Bot_Inv_Full_Text.ReadOnly = true;
            this.Bot_Inv_Full_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_Full_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Inv_Full_Text.TabIndex = 23;
            this.Bot_Inv_Full_Text.Text = "";
            // 
            // Bot_Inv_USlots
            // 
            this.Bot_Inv_USlots.AutoSize = true;
            this.Bot_Inv_USlots.Location = new System.Drawing.Point(3, 75);
            this.Bot_Inv_USlots.Name = "Bot_Inv_USlots";
            this.Bot_Inv_USlots.Size = new System.Drawing.Size(85, 15);
            this.Bot_Inv_USlots.TabIndex = 24;
            this.Bot_Inv_USlots.Text = "Bot Used Slots:";
            // 
            // Bot_Inv_USlots_Text
            // 
            this.Bot_Inv_USlots_Text.HideSelection = false;
            this.Bot_Inv_USlots_Text.Location = new System.Drawing.Point(111, 72);
            this.Bot_Inv_USlots_Text.Name = "Bot_Inv_USlots_Text";
            this.Bot_Inv_USlots_Text.ReadOnly = true;
            this.Bot_Inv_USlots_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_USlots_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Inv_USlots_Text.TabIndex = 25;
            this.Bot_Inv_USlots_Text.Text = "";
            // 
            // Closest_Player
            // 
            this.Closest_Player.AutoSize = true;
            this.Closest_Player.Location = new System.Drawing.Point(3, 10);
            this.Closest_Player.Name = "Closest_Player";
            this.Closest_Player.Size = new System.Drawing.Size(83, 15);
            this.Closest_Player.TabIndex = 26;
            this.Closest_Player.Text = "Closest Player:";
            // 
            // Closest_Player_UUID
            // 
            this.Closest_Player_UUID.AutoSize = true;
            this.Closest_Player_UUID.Location = new System.Drawing.Point(3, 47);
            this.Closest_Player_UUID.Name = "Closest_Player_UUID";
            this.Closest_Player_UUID.Size = new System.Drawing.Size(113, 15);
            this.Closest_Player_UUID.TabIndex = 27;
            this.Closest_Player_UUID.Text = "Closest Player UUID:";
            // 
            // Closest_Player_Loc
            // 
            this.Closest_Player_Loc.AutoSize = true;
            this.Closest_Player_Loc.Location = new System.Drawing.Point(3, 87);
            this.Closest_Player_Loc.Name = "Closest_Player_Loc";
            this.Closest_Player_Loc.Size = new System.Drawing.Size(105, 15);
            this.Closest_Player_Loc.TabIndex = 28;
            this.Closest_Player_Loc.Text = "Closest Player Loc:";
            // 
            // Closest_Player_Dist
            // 
            this.Closest_Player_Dist.AutoSize = true;
            this.Closest_Player_Dist.Location = new System.Drawing.Point(3, 119);
            this.Closest_Player_Dist.Name = "Closest_Player_Dist";
            this.Closest_Player_Dist.Size = new System.Drawing.Size(106, 15);
            this.Closest_Player_Dist.TabIndex = 29;
            this.Closest_Player_Dist.Text = "Closest Player Dist:";
            // 
            // Closest_Player_Text
            // 
            this.Closest_Player_Text.HideSelection = false;
            this.Closest_Player_Text.Location = new System.Drawing.Point(134, 7);
            this.Closest_Player_Text.Name = "Closest_Player_Text";
            this.Closest_Player_Text.ReadOnly = true;
            this.Closest_Player_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Player_Text.TabIndex = 30;
            this.Closest_Player_Text.Text = "";
            // 
            // Closest_Player_UUID_Text
            // 
            this.Closest_Player_UUID_Text.HideSelection = false;
            this.Closest_Player_UUID_Text.Location = new System.Drawing.Point(134, 39);
            this.Closest_Player_UUID_Text.Name = "Closest_Player_UUID_Text";
            this.Closest_Player_UUID_Text.ReadOnly = true;
            this.Closest_Player_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_UUID_Text.Size = new System.Drawing.Size(168, 36);
            this.Closest_Player_UUID_Text.TabIndex = 31;
            this.Closest_Player_UUID_Text.Text = "";
            // 
            // Closest_Player_Loc_Text
            // 
            this.Closest_Player_Loc_Text.HideSelection = false;
            this.Closest_Player_Loc_Text.Location = new System.Drawing.Point(134, 83);
            this.Closest_Player_Loc_Text.Name = "Closest_Player_Loc_Text";
            this.Closest_Player_Loc_Text.ReadOnly = true;
            this.Closest_Player_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Loc_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Player_Loc_Text.TabIndex = 32;
            this.Closest_Player_Loc_Text.Text = "";
            // 
            // Closest_Player_Dist_Text
            // 
            this.Closest_Player_Dist_Text.HideSelection = false;
            this.Closest_Player_Dist_Text.Location = new System.Drawing.Point(134, 115);
            this.Closest_Player_Dist_Text.Name = "Closest_Player_Dist_Text";
            this.Closest_Player_Dist_Text.ReadOnly = true;
            this.Closest_Player_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Dist_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Player_Dist_Text.TabIndex = 33;
            this.Closest_Player_Dist_Text.Text = "";
            // 
            // Closest_Mob
            // 
            this.Closest_Mob.AutoSize = true;
            this.Closest_Mob.Location = new System.Drawing.Point(3, 313);
            this.Closest_Mob.Name = "Closest_Mob";
            this.Closest_Mob.Size = new System.Drawing.Size(76, 15);
            this.Closest_Mob.TabIndex = 34;
            this.Closest_Mob.Text = "Closest Mob:";
            // 
            // Closest_Mob_Text
            // 
            this.Closest_Mob_Text.HideSelection = false;
            this.Closest_Mob_Text.Location = new System.Drawing.Point(134, 309);
            this.Closest_Mob_Text.Name = "Closest_Mob_Text";
            this.Closest_Mob_Text.ReadOnly = true;
            this.Closest_Mob_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Mob_Text.TabIndex = 35;
            this.Closest_Mob_Text.Text = "";
            // 
            // Closest_Mob_Loc
            // 
            this.Closest_Mob_Loc.AutoSize = true;
            this.Closest_Mob_Loc.Location = new System.Drawing.Point(3, 345);
            this.Closest_Mob_Loc.Name = "Closest_Mob_Loc";
            this.Closest_Mob_Loc.Size = new System.Drawing.Size(98, 15);
            this.Closest_Mob_Loc.TabIndex = 36;
            this.Closest_Mob_Loc.Text = "Closest Mob Loc:";
            // 
            // Closest_Mob_Loc_Text
            // 
            this.Closest_Mob_Loc_Text.HideSelection = false;
            this.Closest_Mob_Loc_Text.Location = new System.Drawing.Point(134, 342);
            this.Closest_Mob_Loc_Text.Name = "Closest_Mob_Loc_Text";
            this.Closest_Mob_Loc_Text.ReadOnly = true;
            this.Closest_Mob_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Loc_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Mob_Loc_Text.TabIndex = 37;
            this.Closest_Mob_Loc_Text.Text = "";
            // 
            // Closest_Mob_Dist
            // 
            this.Closest_Mob_Dist.AutoSize = true;
            this.Closest_Mob_Dist.Location = new System.Drawing.Point(3, 377);
            this.Closest_Mob_Dist.Name = "Closest_Mob_Dist";
            this.Closest_Mob_Dist.Size = new System.Drawing.Size(99, 15);
            this.Closest_Mob_Dist.TabIndex = 38;
            this.Closest_Mob_Dist.Text = "Closest Mob Dist:";
            // 
            // Closest_Mob_Dist_Text
            // 
            this.Closest_Mob_Dist_Text.HideSelection = false;
            this.Closest_Mob_Dist_Text.Location = new System.Drawing.Point(134, 374);
            this.Closest_Mob_Dist_Text.Name = "Closest_Mob_Dist_Text";
            this.Closest_Mob_Dist_Text.ReadOnly = true;
            this.Closest_Mob_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Dist_Text.Size = new System.Drawing.Size(168, 25);
            this.Closest_Mob_Dist_Text.TabIndex = 39;
            this.Closest_Mob_Dist_Text.Text = "";
            // 
            // Targetable_Player_Text
            // 
            this.Targetable_Player_Text.HideSelection = false;
            this.Targetable_Player_Text.Location = new System.Drawing.Point(134, 155);
            this.Targetable_Player_Text.Name = "Targetable_Player_Text";
            this.Targetable_Player_Text.ReadOnly = true;
            this.Targetable_Player_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Text.Size = new System.Drawing.Size(168, 25);
            this.Targetable_Player_Text.TabIndex = 40;
            this.Targetable_Player_Text.Text = "";
            // 
            // Targetable_Player_UUID_Text
            // 
            this.Targetable_Player_UUID_Text.HideSelection = false;
            this.Targetable_Player_UUID_Text.Location = new System.Drawing.Point(134, 187);
            this.Targetable_Player_UUID_Text.Name = "Targetable_Player_UUID_Text";
            this.Targetable_Player_UUID_Text.ReadOnly = true;
            this.Targetable_Player_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_UUID_Text.Size = new System.Drawing.Size(168, 36);
            this.Targetable_Player_UUID_Text.TabIndex = 41;
            this.Targetable_Player_UUID_Text.Text = "";
            // 
            // Targetable_Player_Loc_Text
            // 
            this.Targetable_Player_Loc_Text.HideSelection = false;
            this.Targetable_Player_Loc_Text.Location = new System.Drawing.Point(134, 231);
            this.Targetable_Player_Loc_Text.Name = "Targetable_Player_Loc_Text";
            this.Targetable_Player_Loc_Text.ReadOnly = true;
            this.Targetable_Player_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Loc_Text.Size = new System.Drawing.Size(168, 25);
            this.Targetable_Player_Loc_Text.TabIndex = 42;
            this.Targetable_Player_Loc_Text.Text = "";
            // 
            // Targetable_Player_Dist_Text
            // 
            this.Targetable_Player_Dist_Text.HideSelection = false;
            this.Targetable_Player_Dist_Text.Location = new System.Drawing.Point(134, 270);
            this.Targetable_Player_Dist_Text.Name = "Targetable_Player_Dist_Text";
            this.Targetable_Player_Dist_Text.ReadOnly = true;
            this.Targetable_Player_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Dist_Text.Size = new System.Drawing.Size(168, 25);
            this.Targetable_Player_Dist_Text.TabIndex = 43;
            this.Targetable_Player_Dist_Text.Text = "";
            // 
            // Targetable_Player
            // 
            this.Targetable_Player.AutoSize = true;
            this.Targetable_Player.Location = new System.Drawing.Point(3, 158);
            this.Targetable_Player.Name = "Targetable_Player";
            this.Targetable_Player.Size = new System.Drawing.Size(99, 15);
            this.Targetable_Player.TabIndex = 44;
            this.Targetable_Player.Text = "Targetable Player:";
            // 
            // Targetable_Player_UUID
            // 
            this.Targetable_Player_UUID.AutoSize = true;
            this.Targetable_Player_UUID.Location = new System.Drawing.Point(3, 190);
            this.Targetable_Player_UUID.Name = "Targetable_Player_UUID";
            this.Targetable_Player_UUID.Size = new System.Drawing.Size(96, 30);
            this.Targetable_Player_UUID.TabIndex = 45;
            this.Targetable_Player_UUID.Text = "Targetable Player\r\nUUID:";
            // 
            // Targetable_Player_Loc
            // 
            this.Targetable_Player_Loc.AutoSize = true;
            this.Targetable_Player_Loc.Location = new System.Drawing.Point(3, 231);
            this.Targetable_Player_Loc.Name = "Targetable_Player_Loc";
            this.Targetable_Player_Loc.Size = new System.Drawing.Size(96, 30);
            this.Targetable_Player_Loc.TabIndex = 46;
            this.Targetable_Player_Loc.Text = "Targetable Player\r\nLoc:";
            // 
            // Targetable_Player_Dist
            // 
            this.Targetable_Player_Dist.AutoSize = true;
            this.Targetable_Player_Dist.Location = new System.Drawing.Point(3, 270);
            this.Targetable_Player_Dist.Name = "Targetable_Player_Dist";
            this.Targetable_Player_Dist.Size = new System.Drawing.Size(96, 30);
            this.Targetable_Player_Dist.TabIndex = 47;
            this.Targetable_Player_Dist.Text = "Targetable Player\r\nDist:";
            // 
            // Held_Item_ID
            // 
            this.Held_Item_ID.AutoSize = true;
            this.Held_Item_ID.Location = new System.Drawing.Point(3, 119);
            this.Held_Item_ID.Name = "Held_Item_ID";
            this.Held_Item_ID.Size = new System.Drawing.Size(76, 15);
            this.Held_Item_ID.TabIndex = 48;
            this.Held_Item_ID.Text = "Held Item ID:";
            // 
            // Held_Item_ID_Text
            // 
            this.Held_Item_ID_Text.HideSelection = false;
            this.Held_Item_ID_Text.Location = new System.Drawing.Point(111, 115);
            this.Held_Item_ID_Text.Name = "Held_Item_ID_Text";
            this.Held_Item_ID_Text.ReadOnly = true;
            this.Held_Item_ID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_ID_Text.Size = new System.Drawing.Size(41, 25);
            this.Held_Item_ID_Text.TabIndex = 49;
            this.Held_Item_ID_Text.Text = "";
            // 
            // Held_Item_Count
            // 
            this.Held_Item_Count.AutoSize = true;
            this.Held_Item_Count.Location = new System.Drawing.Point(3, 151);
            this.Held_Item_Count.Name = "Held_Item_Count";
            this.Held_Item_Count.Size = new System.Drawing.Size(98, 15);
            this.Held_Item_Count.TabIndex = 50;
            this.Held_Item_Count.Text = "Held Item Count:";
            // 
            // Held_Item_Count_Text
            // 
            this.Held_Item_Count_Text.HideSelection = false;
            this.Held_Item_Count_Text.Location = new System.Drawing.Point(111, 148);
            this.Held_Item_Count_Text.Name = "Held_Item_Count_Text";
            this.Held_Item_Count_Text.ReadOnly = true;
            this.Held_Item_Count_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_Count_Text.Size = new System.Drawing.Size(41, 25);
            this.Held_Item_Count_Text.TabIndex = 51;
            this.Held_Item_Count_Text.Text = "";
            // 
            // Held_Item_Slot
            // 
            this.Held_Item_Slot.AutoSize = true;
            this.Held_Item_Slot.Location = new System.Drawing.Point(3, 183);
            this.Held_Item_Slot.Name = "Held_Item_Slot";
            this.Held_Item_Slot.Size = new System.Drawing.Size(85, 15);
            this.Held_Item_Slot.TabIndex = 52;
            this.Held_Item_Slot.Text = "Held Item Slot:";
            // 
            // Held_Item_Slot_Text
            // 
            this.Held_Item_Slot_Text.HideSelection = false;
            this.Held_Item_Slot_Text.Location = new System.Drawing.Point(111, 180);
            this.Held_Item_Slot_Text.Name = "Held_Item_Slot_Text";
            this.Held_Item_Slot_Text.ReadOnly = true;
            this.Held_Item_Slot_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_Slot_Text.Size = new System.Drawing.Size(41, 25);
            this.Held_Item_Slot_Text.TabIndex = 53;
            this.Held_Item_Slot_Text.Text = "";
            // 
            // Held_Item_NBT
            // 
            this.Held_Item_NBT.AutoSize = true;
            this.Held_Item_NBT.Location = new System.Drawing.Point(233, 210);
            this.Held_Item_NBT.Name = "Held_Item_NBT";
            this.Held_Item_NBT.Size = new System.Drawing.Size(86, 15);
            this.Held_Item_NBT.TabIndex = 54;
            this.Held_Item_NBT.Text = "Held Item NBT:";
            // 
            // Held_Item_NBT_Text
            // 
            this.Held_Item_NBT_Text.HideSelection = false;
            this.Held_Item_NBT_Text.Location = new System.Drawing.Point(7, 228);
            this.Held_Item_NBT_Text.Name = "Held_Item_NBT_Text";
            this.Held_Item_NBT_Text.ReadOnly = true;
            this.Held_Item_NBT_Text.Size = new System.Drawing.Size(511, 145);
            this.Held_Item_NBT_Text.TabIndex = 55;
            this.Held_Item_NBT_Text.Text = "";
            // 
            // Window_Title
            // 
            this.Window_Title.AutoSize = true;
            this.Window_Title.Location = new System.Drawing.Point(3, 10);
            this.Window_Title.Name = "Window_Title";
            this.Window_Title.Size = new System.Drawing.Size(79, 15);
            this.Window_Title.TabIndex = 56;
            this.Window_Title.Text = "Window Title:";
            // 
            // Window_Title_Text
            // 
            this.Window_Title_Text.HideSelection = false;
            this.Window_Title_Text.Location = new System.Drawing.Point(126, 7);
            this.Window_Title_Text.Name = "Window_Title_Text";
            this.Window_Title_Text.ReadOnly = true;
            this.Window_Title_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Window_Title_Text.Size = new System.Drawing.Size(238, 58);
            this.Window_Title_Text.TabIndex = 57;
            this.Window_Title_Text.Text = "";
            // 
            // Window_Type
            // 
            this.Window_Type.AutoSize = true;
            this.Window_Type.Location = new System.Drawing.Point(3, 76);
            this.Window_Type.Name = "Window_Type";
            this.Window_Type.Size = new System.Drawing.Size(81, 15);
            this.Window_Type.TabIndex = 58;
            this.Window_Type.Text = "Window Type:";
            // 
            // Window_ID
            // 
            this.Window_ID.AutoSize = true;
            this.Window_ID.Location = new System.Drawing.Point(3, 108);
            this.Window_ID.Name = "Window_ID";
            this.Window_ID.Size = new System.Drawing.Size(68, 15);
            this.Window_ID.TabIndex = 59;
            this.Window_ID.Text = "Window ID:";
            // 
            // Window_Type_Text
            // 
            this.Window_Type_Text.HideSelection = false;
            this.Window_Type_Text.Location = new System.Drawing.Point(126, 73);
            this.Window_Type_Text.Name = "Window_Type_Text";
            this.Window_Type_Text.ReadOnly = true;
            this.Window_Type_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_Type_Text.Size = new System.Drawing.Size(238, 25);
            this.Window_Type_Text.TabIndex = 60;
            this.Window_Type_Text.Text = "";
            // 
            // Window_ID_Text
            // 
            this.Window_ID_Text.HideSelection = false;
            this.Window_ID_Text.Location = new System.Drawing.Point(126, 105);
            this.Window_ID_Text.Name = "Window_ID_Text";
            this.Window_ID_Text.ReadOnly = true;
            this.Window_ID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_ID_Text.Size = new System.Drawing.Size(41, 25);
            this.Window_ID_Text.TabIndex = 61;
            this.Window_ID_Text.Text = "";
            // 
            // Window_Slotcount_Text
            // 
            this.Window_Slotcount_Text.HideSelection = false;
            this.Window_Slotcount_Text.Location = new System.Drawing.Point(126, 137);
            this.Window_Slotcount_Text.Name = "Window_Slotcount_Text";
            this.Window_Slotcount_Text.ReadOnly = true;
            this.Window_Slotcount_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_Slotcount_Text.Size = new System.Drawing.Size(41, 25);
            this.Window_Slotcount_Text.TabIndex = 62;
            this.Window_Slotcount_Text.Text = "";
            // 
            // Window_EntityID_Text
            // 
            this.Window_EntityID_Text.HideSelection = false;
            this.Window_EntityID_Text.Location = new System.Drawing.Point(126, 170);
            this.Window_EntityID_Text.Name = "Window_EntityID_Text";
            this.Window_EntityID_Text.ReadOnly = true;
            this.Window_EntityID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_EntityID_Text.Size = new System.Drawing.Size(41, 25);
            this.Window_EntityID_Text.TabIndex = 63;
            this.Window_EntityID_Text.Text = "";
            // 
            // Window_ActionID_Text
            // 
            this.Window_ActionID_Text.HideSelection = false;
            this.Window_ActionID_Text.Location = new System.Drawing.Point(126, 202);
            this.Window_ActionID_Text.Name = "Window_ActionID_Text";
            this.Window_ActionID_Text.ReadOnly = true;
            this.Window_ActionID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_ActionID_Text.Size = new System.Drawing.Size(41, 25);
            this.Window_ActionID_Text.TabIndex = 64;
            this.Window_ActionID_Text.Text = "";
            // 
            // Window_Slotcount
            // 
            this.Window_Slotcount.AutoSize = true;
            this.Window_Slotcount.Location = new System.Drawing.Point(3, 141);
            this.Window_Slotcount.Name = "Window_Slotcount";
            this.Window_Slotcount.Size = new System.Drawing.Size(108, 15);
            this.Window_Slotcount.TabIndex = 65;
            this.Window_Slotcount.Text = "Window Slotcount:";
            // 
            // Window_EntityID
            // 
            this.Window_EntityID.AutoSize = true;
            this.Window_EntityID.Location = new System.Drawing.Point(3, 173);
            this.Window_EntityID.Name = "Window_EntityID";
            this.Window_EntityID.Size = new System.Drawing.Size(98, 15);
            this.Window_EntityID.TabIndex = 66;
            this.Window_EntityID.Text = "Window EntityID:";
            // 
            // Window_ActionID
            // 
            this.Window_ActionID.AutoSize = true;
            this.Window_ActionID.Location = new System.Drawing.Point(3, 205);
            this.Window_ActionID.Name = "Window_ActionID";
            this.Window_ActionID.Size = new System.Drawing.Size(103, 15);
            this.Window_ActionID.TabIndex = 67;
            this.Window_ActionID.Text = "Window ActionID:";
            // 
            // Window_Slotids
            // 
            this.Window_Slotids.AutoSize = true;
            this.Window_Slotids.Location = new System.Drawing.Point(3, 238);
            this.Window_Slotids.Name = "Window_Slotids";
            this.Window_Slotids.Size = new System.Drawing.Size(92, 15);
            this.Window_Slotids.TabIndex = 68;
            this.Window_Slotids.Text = "Window Slotids:";
            // 
            // Window_Slotids_Text
            // 
            this.Window_Slotids_Text.HideSelection = false;
            this.Window_Slotids_Text.Location = new System.Drawing.Point(126, 234);
            this.Window_Slotids_Text.Name = "Window_Slotids_Text";
            this.Window_Slotids_Text.ReadOnly = true;
            this.Window_Slotids_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Window_Slotids_Text.Size = new System.Drawing.Size(367, 87);
            this.Window_Slotids_Text.TabIndex = 69;
            this.Window_Slotids_Text.Text = "";
            // 
            // Window_Inv_Slotids
            // 
            this.Window_Inv_Slotids.AutoSize = true;
            this.Window_Inv_Slotids.Location = new System.Drawing.Point(3, 332);
            this.Window_Inv_Slotids.Name = "Window_Inv_Slotids";
            this.Window_Inv_Slotids.Size = new System.Drawing.Size(111, 15);
            this.Window_Inv_Slotids.TabIndex = 70;
            this.Window_Inv_Slotids.Text = "Window Inv Slotids:";
            // 
            // Window_Inv_Slotids_Text
            // 
            this.Window_Inv_Slotids_Text.HideSelection = false;
            this.Window_Inv_Slotids_Text.Location = new System.Drawing.Point(126, 329);
            this.Window_Inv_Slotids_Text.Name = "Window_Inv_Slotids_Text";
            this.Window_Inv_Slotids_Text.ReadOnly = true;
            this.Window_Inv_Slotids_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Window_Inv_Slotids_Text.Size = new System.Drawing.Size(367, 87);
            this.Window_Inv_Slotids_Text.TabIndex = 71;
            this.Window_Inv_Slotids_Text.Text = "";
            // 
            // Current_Path_Target_Loc
            // 
            this.Current_Path_Target_Loc.AutoSize = true;
            this.Current_Path_Target_Loc.Location = new System.Drawing.Point(3, 10);
            this.Current_Path_Target_Loc.Name = "Current_Path_Target_Loc";
            this.Current_Path_Target_Loc.Size = new System.Drawing.Size(134, 15);
            this.Current_Path_Target_Loc.TabIndex = 72;
            this.Current_Path_Target_Loc.Text = "Current Path Target Loc:";
            // 
            // Current_Path_Target_Loc_Text
            // 
            this.Current_Path_Target_Loc_Text.HideSelection = false;
            this.Current_Path_Target_Loc_Text.Location = new System.Drawing.Point(182, 7);
            this.Current_Path_Target_Loc_Text.Name = "Current_Path_Target_Loc_Text";
            this.Current_Path_Target_Loc_Text.ReadOnly = true;
            this.Current_Path_Target_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Target_Loc_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Target_Loc_Text.TabIndex = 73;
            this.Current_Path_Target_Loc_Text.Text = "";
            // 
            // Current_Path_Complete
            // 
            this.Current_Path_Complete.AutoSize = true;
            this.Current_Path_Complete.Location = new System.Drawing.Point(3, 43);
            this.Current_Path_Complete.Name = "Current_Path_Complete";
            this.Current_Path_Complete.Size = new System.Drawing.Size(132, 15);
            this.Current_Path_Complete.TabIndex = 74;
            this.Current_Path_Complete.Text = "Current Path Complete:";
            // 
            // Current_Path_Complete_Text
            // 
            this.Current_Path_Complete_Text.HideSelection = false;
            this.Current_Path_Complete_Text.Location = new System.Drawing.Point(182, 39);
            this.Current_Path_Complete_Text.Name = "Current_Path_Complete_Text";
            this.Current_Path_Complete_Text.ReadOnly = true;
            this.Current_Path_Complete_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Complete_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Complete_Text.TabIndex = 75;
            this.Current_Path_Complete_Text.Text = "";
            // 
            // Current_Path_Disposed_Text
            // 
            this.Current_Path_Disposed_Text.HideSelection = false;
            this.Current_Path_Disposed_Text.Location = new System.Drawing.Point(182, 72);
            this.Current_Path_Disposed_Text.Name = "Current_Path_Disposed_Text";
            this.Current_Path_Disposed_Text.ReadOnly = true;
            this.Current_Path_Disposed_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Disposed_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Disposed_Text.TabIndex = 76;
            this.Current_Path_Disposed_Text.Text = "";
            // 
            // Current_Path_Disposed
            // 
            this.Current_Path_Disposed.AutoSize = true;
            this.Current_Path_Disposed.Location = new System.Drawing.Point(3, 75);
            this.Current_Path_Disposed.Name = "Current_Path_Disposed";
            this.Current_Path_Disposed.Size = new System.Drawing.Size(128, 15);
            this.Current_Path_Disposed.TabIndex = 77;
            this.Current_Path_Disposed.Text = "Current Path Disposed:";
            // 
            // Current_Path_Offset_Text
            // 
            this.Current_Path_Offset_Text.HideSelection = false;
            this.Current_Path_Offset_Text.Location = new System.Drawing.Point(182, 104);
            this.Current_Path_Offset_Text.Name = "Current_Path_Offset_Text";
            this.Current_Path_Offset_Text.ReadOnly = true;
            this.Current_Path_Offset_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Offset_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Offset_Text.TabIndex = 78;
            this.Current_Path_Offset_Text.Text = "";
            // 
            // Current_Path_Offset
            // 
            this.Current_Path_Offset.AutoSize = true;
            this.Current_Path_Offset.Location = new System.Drawing.Point(3, 107);
            this.Current_Path_Offset.Name = "Current_Path_Offset";
            this.Current_Path_Offset.Size = new System.Drawing.Size(112, 15);
            this.Current_Path_Offset.TabIndex = 79;
            this.Current_Path_Offset.Text = "Current Path Offset:";
            // 
            // Current_Path_Options_Text
            // 
            this.Current_Path_Options_Text.HideSelection = false;
            this.Current_Path_Options_Text.Location = new System.Drawing.Point(182, 233);
            this.Current_Path_Options_Text.Name = "Current_Path_Options_Text";
            this.Current_Path_Options_Text.ReadOnly = true;
            this.Current_Path_Options_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Current_Path_Options_Text.Size = new System.Drawing.Size(168, 214);
            this.Current_Path_Options_Text.TabIndex = 80;
            this.Current_Path_Options_Text.Text = "";
            // 
            // Current_Path_Options
            // 
            this.Current_Path_Options.AutoSize = true;
            this.Current_Path_Options.Location = new System.Drawing.Point(3, 237);
            this.Current_Path_Options.Name = "Current_Path_Options";
            this.Current_Path_Options.Size = new System.Drawing.Size(122, 15);
            this.Current_Path_Options.TabIndex = 81;
            this.Current_Path_Options.Text = "Current Path Options:";
            // 
            // Current_Path_Searched_Text
            // 
            this.Current_Path_Searched_Text.HideSelection = false;
            this.Current_Path_Searched_Text.Location = new System.Drawing.Point(182, 136);
            this.Current_Path_Searched_Text.Name = "Current_Path_Searched_Text";
            this.Current_Path_Searched_Text.ReadOnly = true;
            this.Current_Path_Searched_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Searched_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Searched_Text.TabIndex = 82;
            this.Current_Path_Searched_Text.Text = "";
            // 
            // Current_Path_Searched
            // 
            this.Current_Path_Searched.AutoSize = true;
            this.Current_Path_Searched.Location = new System.Drawing.Point(3, 140);
            this.Current_Path_Searched.Name = "Current_Path_Searched";
            this.Current_Path_Searched.Size = new System.Drawing.Size(128, 15);
            this.Current_Path_Searched.TabIndex = 83;
            this.Current_Path_Searched.Text = "Current Path Searched:";
            // 
            // Current_Path_Token_Stopped_Text
            // 
            this.Current_Path_Token_Stopped_Text.HideSelection = false;
            this.Current_Path_Token_Stopped_Text.Location = new System.Drawing.Point(182, 168);
            this.Current_Path_Token_Stopped_Text.Name = "Current_Path_Token_Stopped_Text";
            this.Current_Path_Token_Stopped_Text.ReadOnly = true;
            this.Current_Path_Token_Stopped_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Token_Stopped_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Token_Stopped_Text.TabIndex = 84;
            this.Current_Path_Token_Stopped_Text.Text = "";
            // 
            // Current_Path_Token_Stopped
            // 
            this.Current_Path_Token_Stopped.AutoSize = true;
            this.Current_Path_Token_Stopped.Location = new System.Drawing.Point(3, 172);
            this.Current_Path_Token_Stopped.Name = "Current_Path_Token_Stopped";
            this.Current_Path_Token_Stopped.Size = new System.Drawing.Size(158, 15);
            this.Current_Path_Token_Stopped.TabIndex = 85;
            this.Current_Path_Token_Stopped.Text = "Current Path Token Stopped:";
            // 
            // Current_Path_Valid_Text
            // 
            this.Current_Path_Valid_Text.HideSelection = false;
            this.Current_Path_Valid_Text.Location = new System.Drawing.Point(182, 201);
            this.Current_Path_Valid_Text.Name = "Current_Path_Valid_Text";
            this.Current_Path_Valid_Text.ReadOnly = true;
            this.Current_Path_Valid_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Current_Path_Valid_Text.Size = new System.Drawing.Size(168, 25);
            this.Current_Path_Valid_Text.TabIndex = 86;
            this.Current_Path_Valid_Text.Text = "";
            // 
            // Current_Path_Valid
            // 
            this.Current_Path_Valid.AutoSize = true;
            this.Current_Path_Valid.Location = new System.Drawing.Point(3, 204);
            this.Current_Path_Valid.Name = "Current_Path_Valid";
            this.Current_Path_Valid.Size = new System.Drawing.Size(105, 15);
            this.Current_Path_Valid.TabIndex = 87;
            this.Current_Path_Valid.Text = "Current Path Valid:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Location = new System.Drawing.Point(558, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(535, 406);
            this.tabControl1.TabIndex = 88;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.Bot_Jumping);
            this.tabPage1.Controls.Add(this.Bot_Jumping_Text);
            this.tabPage1.Controls.Add(this.Bot_In_Water_Text);
            this.tabPage1.Controls.Add(this.Bot_In_Water);
            this.tabPage1.Controls.Add(this.Bot_Grounded_Text);
            this.tabPage1.Controls.Add(this.Bot_Grounded);
            this.tabPage1.Controls.Add(this.Bot_Name_Text);
            this.tabPage1.Controls.Add(this.Bot_UUID_Text);
            this.tabPage1.Controls.Add(this.Bot_Health_Text);
            this.tabPage1.Controls.Add(this.Bot_Hunger_Text);
            this.tabPage1.Controls.Add(this.Bot_Location_Text);
            this.tabPage1.Controls.Add(this.Bot_Dimension_Text);
            this.tabPage1.Controls.Add(this.Bot_Gamemode_Text);
            this.tabPage1.Controls.Add(this.Bot_Dead_Text);
            this.tabPage1.Controls.Add(this.Bot_Name);
            this.tabPage1.Controls.Add(this.Bot_Health);
            this.tabPage1.Controls.Add(this.Bot_Hunger);
            this.tabPage1.Controls.Add(this.Bot_Location);
            this.tabPage1.Controls.Add(this.Bot_Dimension);
            this.tabPage1.Controls.Add(this.Bot_Gamemode);
            this.tabPage1.Controls.Add(this.Bot_UUID);
            this.tabPage1.Controls.Add(this.Bot_Dead);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(527, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Bot_Jumping
            // 
            this.Bot_Jumping.AutoSize = true;
            this.Bot_Jumping.Location = new System.Drawing.Point(3, 345);
            this.Bot_Jumping.Name = "Bot_Jumping";
            this.Bot_Jumping.Size = new System.Drawing.Size(77, 15);
            this.Bot_Jumping.TabIndex = 25;
            this.Bot_Jumping.Text = "Bot Jumping:";
            // 
            // Bot_Jumping_Text
            // 
            this.Bot_Jumping_Text.HideSelection = false;
            this.Bot_Jumping_Text.Location = new System.Drawing.Point(107, 342);
            this.Bot_Jumping_Text.Name = "Bot_Jumping_Text";
            this.Bot_Jumping_Text.ReadOnly = true;
            this.Bot_Jumping_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Jumping_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Jumping_Text.TabIndex = 24;
            this.Bot_Jumping_Text.Text = "";
            // 
            // Bot_In_Water_Text
            // 
            this.Bot_In_Water_Text.HideSelection = false;
            this.Bot_In_Water_Text.Location = new System.Drawing.Point(107, 309);
            this.Bot_In_Water_Text.Name = "Bot_In_Water_Text";
            this.Bot_In_Water_Text.ReadOnly = true;
            this.Bot_In_Water_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_In_Water_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_In_Water_Text.TabIndex = 23;
            this.Bot_In_Water_Text.Text = "";
            // 
            // Bot_In_Water
            // 
            this.Bot_In_Water.AutoSize = true;
            this.Bot_In_Water.Location = new System.Drawing.Point(3, 313);
            this.Bot_In_Water.Name = "Bot_In_Water";
            this.Bot_In_Water.Size = new System.Drawing.Size(73, 15);
            this.Bot_In_Water.TabIndex = 22;
            this.Bot_In_Water.Text = "Bot in water:";
            // 
            // Bot_Grounded_Text
            // 
            this.Bot_Grounded_Text.HideSelection = false;
            this.Bot_Grounded_Text.Location = new System.Drawing.Point(107, 277);
            this.Bot_Grounded_Text.Name = "Bot_Grounded_Text";
            this.Bot_Grounded_Text.ReadOnly = true;
            this.Bot_Grounded_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Grounded_Text.Size = new System.Drawing.Size(41, 25);
            this.Bot_Grounded_Text.TabIndex = 21;
            this.Bot_Grounded_Text.Text = "";
            // 
            // Bot_Grounded
            // 
            this.Bot_Grounded.AutoSize = true;
            this.Bot_Grounded.Location = new System.Drawing.Point(3, 280);
            this.Bot_Grounded.Name = "Bot_Grounded";
            this.Bot_Grounded.Size = new System.Drawing.Size(84, 15);
            this.Bot_Grounded.TabIndex = 20;
            this.Bot_Grounded.Text = "Bot Grounded:";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.Bot_Inv_Full_Text);
            this.tabPage2.Controls.Add(this.Bot_Inv_Full);
            this.tabPage2.Controls.Add(this.Bot_Inv_FSlots);
            this.tabPage2.Controls.Add(this.Bot_Inv_FSlots_Text);
            this.tabPage2.Controls.Add(this.Bot_Inv_USlots);
            this.tabPage2.Controls.Add(this.Bot_Inv_USlots_Text);
            this.tabPage2.Controls.Add(this.Held_Item_ID);
            this.tabPage2.Controls.Add(this.Held_Item_ID_Text);
            this.tabPage2.Controls.Add(this.Held_Item_Count);
            this.tabPage2.Controls.Add(this.Held_Item_Count_Text);
            this.tabPage2.Controls.Add(this.Held_Item_Slot);
            this.tabPage2.Controls.Add(this.Held_Item_Slot_Text);
            this.tabPage2.Controls.Add(this.Held_Item_NBT);
            this.tabPage2.Controls.Add(this.Held_Item_NBT_Text);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(527, 380);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Inventory";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.Closest_Player_Text);
            this.tabPage3.Controls.Add(this.Closest_Player);
            this.tabPage3.Controls.Add(this.Closest_Player_UUID);
            this.tabPage3.Controls.Add(this.Closest_Player_Loc);
            this.tabPage3.Controls.Add(this.Closest_Player_Dist);
            this.tabPage3.Controls.Add(this.Closest_Player_UUID_Text);
            this.tabPage3.Controls.Add(this.Closest_Player_Loc_Text);
            this.tabPage3.Controls.Add(this.Closest_Player_Dist_Text);
            this.tabPage3.Controls.Add(this.Targetable_Player_Text);
            this.tabPage3.Controls.Add(this.Targetable_Player_UUID_Text);
            this.tabPage3.Controls.Add(this.Targetable_Player_Loc_Text);
            this.tabPage3.Controls.Add(this.Targetable_Player_Dist_Text);
            this.tabPage3.Controls.Add(this.Targetable_Player);
            this.tabPage3.Controls.Add(this.Targetable_Player_UUID);
            this.tabPage3.Controls.Add(this.Targetable_Player_Loc);
            this.tabPage3.Controls.Add(this.Targetable_Player_Dist);
            this.tabPage3.Controls.Add(this.Closest_Mob_Text);
            this.tabPage3.Controls.Add(this.Closest_Mob);
            this.tabPage3.Controls.Add(this.Closest_Mob_Loc);
            this.tabPage3.Controls.Add(this.Closest_Mob_Loc_Text);
            this.tabPage3.Controls.Add(this.Closest_Mob_Dist);
            this.tabPage3.Controls.Add(this.Closest_Mob_Dist_Text);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(527, 380);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Closest Stuff";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.tabPage4.Controls.Add(this.Window_Slot_NBT_Text);
            this.tabPage4.Controls.Add(this.Window_Slot_NBT);
            this.tabPage4.Controls.Add(this.Window_Title_Text);
            this.tabPage4.Controls.Add(this.Window_Title);
            this.tabPage4.Controls.Add(this.Window_Type);
            this.tabPage4.Controls.Add(this.Window_ID);
            this.tabPage4.Controls.Add(this.Window_Type_Text);
            this.tabPage4.Controls.Add(this.Window_ID_Text);
            this.tabPage4.Controls.Add(this.Window_Slotcount_Text);
            this.tabPage4.Controls.Add(this.Window_EntityID_Text);
            this.tabPage4.Controls.Add(this.Window_ActionID_Text);
            this.tabPage4.Controls.Add(this.Window_Slotcount);
            this.tabPage4.Controls.Add(this.Window_EntityID);
            this.tabPage4.Controls.Add(this.Window_ActionID);
            this.tabPage4.Controls.Add(this.Window_Slotids);
            this.tabPage4.Controls.Add(this.Window_Slotids_Text);
            this.tabPage4.Controls.Add(this.Window_Inv_Slotids);
            this.tabPage4.Controls.Add(this.Window_Inv_Slotids_Text);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(527, 380);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Window/Container";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Window_Slot_NBT_Text
            // 
            this.Window_Slot_NBT_Text.HideSelection = false;
            this.Window_Slot_NBT_Text.Location = new System.Drawing.Point(126, 423);
            this.Window_Slot_NBT_Text.Name = "Window_Slot_NBT_Text";
            this.Window_Slot_NBT_Text.ReadOnly = true;
            this.Window_Slot_NBT_Text.Size = new System.Drawing.Size(367, 87);
            this.Window_Slot_NBT_Text.TabIndex = 73;
            this.Window_Slot_NBT_Text.Text = "";
            // 
            // Window_Slot_NBT
            // 
            this.Window_Slot_NBT.AutoSize = true;
            this.Window_Slot_NBT.Location = new System.Drawing.Point(3, 427);
            this.Window_Slot_NBT.Name = "Window_Slot_NBT";
            this.Window_Slot_NBT.Size = new System.Drawing.Size(101, 15);
            this.Window_Slot_NBT.TabIndex = 72;
            this.Window_Slot_NBT.Text = "Window Slot NBT:";
            this.Window_Slot_NBT.UseMnemonic = false;
            // 
            // tabPage5
            // 
            this.tabPage5.AutoScroll = true;
            this.tabPage5.Controls.Add(this.Current_Path_Target_Loc_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Options);
            this.tabPage5.Controls.Add(this.Current_Path_Options_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Valid);
            this.tabPage5.Controls.Add(this.Current_Path_Target_Loc);
            this.tabPage5.Controls.Add(this.Current_Path_Valid_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Complete);
            this.tabPage5.Controls.Add(this.Current_Path_Token_Stopped);
            this.tabPage5.Controls.Add(this.Current_Path_Complete_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Token_Stopped_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Disposed_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Searched);
            this.tabPage5.Controls.Add(this.Current_Path_Disposed);
            this.tabPage5.Controls.Add(this.Current_Path_Searched_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Offset_Text);
            this.tabPage5.Controls.Add(this.Current_Path_Offset);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(527, 380);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Current Path";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.Debug_Console);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(527, 380);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Debug Console";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // Debug_Console
            // 
            this.Debug_Console.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (224)))),
                ((int) (((byte) (224)))), ((int) (((byte) (224)))));
            this.Debug_Console.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Debug_Console.HideSelection = false;
            this.Debug_Console.Location = new System.Drawing.Point(7, 7);
            this.Debug_Console.Name = "Debug_Console";
            this.Debug_Console.Size = new System.Drawing.Size(512, 362);
            this.Debug_Console.TabIndex = 89;
            this.Debug_Console.Text = "";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.Debug_Values);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(527, 380);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Debug Values";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // Debug_Values
            // 
            this.Debug_Values.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (224)))),
                ((int) (((byte) (224)))), ((int) (((byte) (224)))));
            this.Debug_Values.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Debug_Values.ForeColor = System.Drawing.Color.Black;
            this.Debug_Values.Location = new System.Drawing.Point(7, 7);
            this.Debug_Values.Name = "Debug_Values";
            this.Debug_Values.Size = new System.Drawing.Size(512, 362);
            this.Debug_Values.TabIndex = 0;
            this.Debug_Values.Text = "";
            // 
            // Chat_Send
            // 
            this.Chat_Send.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))),
                ((int) (((byte) (64)))));
            this.Chat_Send.Location = new System.Drawing.Point(463, 391);
            this.Chat_Send.Name = "Chat_Send";
            this.Chat_Send.Size = new System.Drawing.Size(87, 23);
            this.Chat_Send.TabIndex = 2;
            this.Chat_Send.Text = "Send";
            this.Chat_Send.UseVisualStyleBackColor = false;
            this.Chat_Send.Click += new System.EventHandler(this.Chat_Send_Click);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1156, 442);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.Chat_Box);
            this.Controls.Add(this.Chat_Send);
            this.Controls.Add(this.Chat_Message);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DebugForm";
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.ShowIcon = false;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

#endregion
        private System.Windows.Forms.TextBox Chat_Message;
        private System.Windows.Forms.RichTextBox Chat_Box;
        private System.Windows.Forms.Button Chat_Send;
        private System.Windows.Forms.RichTextBox Debug_Console;
        private System.Windows.Forms.RichTextBox Debug_Values;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label Window_Slot_NBT;
        private System.Windows.Forms.RichTextBox Window_Slot_NBT_Text;
        private System.Windows.Forms.RichTextBox Bot_Jumping_Text;
        private System.Windows.Forms.Label Bot_Jumping;
        private System.Windows.Forms.Label Bot_In_Water;
        private System.Windows.Forms.RichTextBox Bot_In_Water_Text;
        private System.Windows.Forms.Label Bot_Grounded;
        private System.Windows.Forms.RichTextBox Bot_Grounded_Text;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label Current_Path_Valid;
        private System.Windows.Forms.RichTextBox Current_Path_Valid_Text;
        private System.Windows.Forms.Label Current_Path_Token_Stopped;
        private System.Windows.Forms.RichTextBox Current_Path_Token_Stopped_Text;
        private System.Windows.Forms.Label Current_Path_Searched;
        private System.Windows.Forms.RichTextBox Current_Path_Searched_Text;
        private System.Windows.Forms.Label Current_Path_Options;
        private System.Windows.Forms.RichTextBox Current_Path_Options_Text;
        private System.Windows.Forms.Label Current_Path_Offset;
        private System.Windows.Forms.RichTextBox Current_Path_Offset_Text;
        private System.Windows.Forms.Label Current_Path_Disposed;
        private System.Windows.Forms.RichTextBox Current_Path_Disposed_Text;
        private System.Windows.Forms.RichTextBox Current_Path_Complete_Text;
        private System.Windows.Forms.Label Current_Path_Complete;
        private System.Windows.Forms.RichTextBox Current_Path_Target_Loc_Text;
        private System.Windows.Forms.Label Current_Path_Target_Loc;
        private System.Windows.Forms.RichTextBox Window_Inv_Slotids_Text;
        private System.Windows.Forms.Label Window_Inv_Slotids;
        private System.Windows.Forms.RichTextBox Window_Slotids_Text;
        private System.Windows.Forms.Label Window_Slotids;
        private System.Windows.Forms.Label Window_ActionID;
        private System.Windows.Forms.Label Window_EntityID;
        private System.Windows.Forms.Label Window_Slotcount;
        private System.Windows.Forms.RichTextBox Window_ActionID_Text;
        private System.Windows.Forms.RichTextBox Window_EntityID_Text;
        private System.Windows.Forms.RichTextBox Window_Slotcount_Text;
        private System.Windows.Forms.RichTextBox Window_ID_Text;
        private System.Windows.Forms.RichTextBox Window_Type_Text;
        private System.Windows.Forms.Label Window_ID;
        private System.Windows.Forms.Label Window_Type;
        private System.Windows.Forms.RichTextBox Window_Title_Text;
        private System.Windows.Forms.Label Window_Title;
        private System.Windows.Forms.RichTextBox Held_Item_NBT_Text;
        private System.Windows.Forms.Label Held_Item_NBT;
        private System.Windows.Forms.RichTextBox Held_Item_Slot_Text;
        private System.Windows.Forms.Label Held_Item_Slot;
        private System.Windows.Forms.RichTextBox Held_Item_Count_Text;
        private System.Windows.Forms.Label Held_Item_Count;
        private System.Windows.Forms.RichTextBox Held_Item_ID_Text;
        private System.Windows.Forms.Label Held_Item_ID;
        private System.Windows.Forms.Label Targetable_Player_Dist;
        private System.Windows.Forms.Label Targetable_Player_Loc;
        private System.Windows.Forms.Label Targetable_Player_UUID;
        private System.Windows.Forms.Label Targetable_Player;
        private System.Windows.Forms.RichTextBox Targetable_Player_Dist_Text;
        private System.Windows.Forms.RichTextBox Targetable_Player_Loc_Text;
        private System.Windows.Forms.RichTextBox Targetable_Player_UUID_Text;
        private System.Windows.Forms.RichTextBox Targetable_Player_Text;
        private System.Windows.Forms.RichTextBox Closest_Mob_Dist_Text;
        private System.Windows.Forms.Label Closest_Mob_Dist;
        private System.Windows.Forms.RichTextBox Closest_Mob_Loc_Text;
        private System.Windows.Forms.Label Closest_Mob_Loc;
        private System.Windows.Forms.RichTextBox Closest_Mob_Text;
        private System.Windows.Forms.Label Closest_Mob;
        private System.Windows.Forms.RichTextBox Closest_Player_Dist_Text;
        private System.Windows.Forms.RichTextBox Closest_Player_Loc_Text;
        private System.Windows.Forms.RichTextBox Closest_Player_UUID_Text;
        private System.Windows.Forms.RichTextBox Closest_Player_Text;
        private System.Windows.Forms.Label Closest_Player_Dist;
        private System.Windows.Forms.Label Closest_Player_Loc;
        private System.Windows.Forms.Label Closest_Player_UUID;
        private System.Windows.Forms.Label Closest_Player;
        private System.Windows.Forms.RichTextBox Bot_Inv_USlots_Text;
        private System.Windows.Forms.Label Bot_Inv_USlots;
        private System.Windows.Forms.RichTextBox Bot_Inv_Full_Text;
        private System.Windows.Forms.RichTextBox Bot_Inv_FSlots_Text;
        private System.Windows.Forms.Label Bot_Inv_FSlots;
        private System.Windows.Forms.Label Bot_Inv_Full;
        private System.Windows.Forms.RichTextBox Bot_Dead_Text;
        private System.Windows.Forms.Label Bot_Dead;
        private System.Windows.Forms.RichTextBox Bot_UUID_Text;
        private System.Windows.Forms.Label Bot_UUID;
        private System.Windows.Forms.RichTextBox Bot_Gamemode_Text;
        private System.Windows.Forms.Label Bot_Gamemode;
        private System.Windows.Forms.RichTextBox Bot_Dimension_Text;
        private System.Windows.Forms.Label Bot_Dimension;
        private System.Windows.Forms.RichTextBox Bot_Location_Text;
        private System.Windows.Forms.Label Bot_Location;
        private System.Windows.Forms.RichTextBox Bot_Hunger_Text;
        private System.Windows.Forms.RichTextBox Bot_Health_Text;
        private System.Windows.Forms.Label Bot_Hunger;
        private System.Windows.Forms.Label Bot_Health;
        private System.Windows.Forms.RichTextBox Bot_Name_Text;
        private System.Windows.Forms.Label Bot_Name;
    }
}