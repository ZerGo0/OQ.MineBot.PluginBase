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
        private void InitializeComponent() {
            this.Chat_Message = new System.Windows.Forms.TextBox();
            this.Chat_Send = new System.Windows.Forms.Button();
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
            this.Bot_Alive = new System.Windows.Forms.Label();
            this.Bot_Alive_Text = new System.Windows.Forms.RichTextBox();
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
            this.label1 = new System.Windows.Forms.Label();
            this.Window_Slotids_Text = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Chat_Message
            // 
            this.Chat_Message.AcceptsReturn = true;
            this.Chat_Message.AcceptsTab = true;
            this.Chat_Message.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Chat_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Chat_Message.Location = new System.Drawing.Point(13, 348);
            this.Chat_Message.MaxLength = 256;
            this.Chat_Message.Name = "Chat_Message";
            this.Chat_Message.Size = new System.Drawing.Size(378, 20);
            this.Chat_Message.TabIndex = 1;
            this.Chat_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chat_Message_KeyDown);
            // 
            // Chat_Send
            // 
            this.Chat_Send.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Chat_Send.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Chat_Send.Location = new System.Drawing.Point(397, 348);
            this.Chat_Send.Name = "Chat_Send";
            this.Chat_Send.Size = new System.Drawing.Size(75, 20);
            this.Chat_Send.TabIndex = 2;
            this.Chat_Send.Text = "Send";
            this.Chat_Send.UseVisualStyleBackColor = false;
            this.Chat_Send.Click += new System.EventHandler(this.Chat_Send_Click);
            // 
            // Chat_Box
            // 
            this.Chat_Box.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Chat_Box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Chat_Box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Chat_Box.HideSelection = false;
            this.Chat_Box.Location = new System.Drawing.Point(13, 13);
            this.Chat_Box.Name = "Chat_Box";
            this.Chat_Box.Size = new System.Drawing.Size(459, 330);
            this.Chat_Box.TabIndex = 3;
            this.Chat_Box.Text = "";
            this.Chat_Box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chat_Box_KeyDown);
            this.Chat_Box.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chat_Box_MouseUp);
            // 
            // Bot_Name
            // 
            this.Bot_Name.AutoSize = true;
            this.Bot_Name.Location = new System.Drawing.Point(490, 16);
            this.Bot_Name.Name = "Bot_Name";
            this.Bot_Name.Size = new System.Drawing.Size(52, 13);
            this.Bot_Name.TabIndex = 4;
            this.Bot_Name.Text = "Botname:";
            // 
            // Bot_Name_Text
            // 
            this.Bot_Name_Text.HideSelection = false;
            this.Bot_Name_Text.Location = new System.Drawing.Point(579, 13);
            this.Bot_Name_Text.Name = "Bot_Name_Text";
            this.Bot_Name_Text.ReadOnly = true;
            this.Bot_Name_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Name_Text.Size = new System.Drawing.Size(145, 22);
            this.Bot_Name_Text.TabIndex = 5;
            this.Bot_Name_Text.Text = "";
            // 
            // Bot_Health
            // 
            this.Bot_Health.AutoSize = true;
            this.Bot_Health.Location = new System.Drawing.Point(490, 82);
            this.Bot_Health.Name = "Bot_Health";
            this.Bot_Health.Size = new System.Drawing.Size(60, 13);
            this.Bot_Health.TabIndex = 6;
            this.Bot_Health.Text = "Bot Health:";
            // 
            // Bot_Hunger
            // 
            this.Bot_Hunger.AutoSize = true;
            this.Bot_Hunger.Location = new System.Drawing.Point(490, 110);
            this.Bot_Hunger.Name = "Bot_Hunger";
            this.Bot_Hunger.Size = new System.Drawing.Size(64, 13);
            this.Bot_Hunger.TabIndex = 7;
            this.Bot_Hunger.Text = "Bot Hunger:";
            // 
            // Bot_Health_Text
            // 
            this.Bot_Health_Text.HideSelection = false;
            this.Bot_Health_Text.Location = new System.Drawing.Point(579, 79);
            this.Bot_Health_Text.Name = "Bot_Health_Text";
            this.Bot_Health_Text.ReadOnly = true;
            this.Bot_Health_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Health_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Health_Text.TabIndex = 8;
            this.Bot_Health_Text.Text = "";
            // 
            // Bot_Hunger_Text
            // 
            this.Bot_Hunger_Text.HideSelection = false;
            this.Bot_Hunger_Text.Location = new System.Drawing.Point(579, 107);
            this.Bot_Hunger_Text.Name = "Bot_Hunger_Text";
            this.Bot_Hunger_Text.ReadOnly = true;
            this.Bot_Hunger_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Hunger_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Hunger_Text.TabIndex = 9;
            this.Bot_Hunger_Text.Text = "";
            // 
            // Bot_Location
            // 
            this.Bot_Location.AutoSize = true;
            this.Bot_Location.Location = new System.Drawing.Point(490, 138);
            this.Bot_Location.Name = "Bot_Location";
            this.Bot_Location.Size = new System.Drawing.Size(70, 13);
            this.Bot_Location.TabIndex = 10;
            this.Bot_Location.Text = "Bot Location:";
            // 
            // Bot_Location_Text
            // 
            this.Bot_Location_Text.HideSelection = false;
            this.Bot_Location_Text.Location = new System.Drawing.Point(579, 135);
            this.Bot_Location_Text.Name = "Bot_Location_Text";
            this.Bot_Location_Text.ReadOnly = true;
            this.Bot_Location_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Location_Text.Size = new System.Drawing.Size(145, 22);
            this.Bot_Location_Text.TabIndex = 11;
            this.Bot_Location_Text.Text = "";
            // 
            // Bot_Dimension
            // 
            this.Bot_Dimension.AutoSize = true;
            this.Bot_Dimension.Location = new System.Drawing.Point(490, 166);
            this.Bot_Dimension.Name = "Bot_Dimension";
            this.Bot_Dimension.Size = new System.Drawing.Size(78, 13);
            this.Bot_Dimension.TabIndex = 12;
            this.Bot_Dimension.Text = "Bot Dimension:";
            // 
            // Bot_Dimension_Text
            // 
            this.Bot_Dimension_Text.HideSelection = false;
            this.Bot_Dimension_Text.Location = new System.Drawing.Point(579, 163);
            this.Bot_Dimension_Text.Name = "Bot_Dimension_Text";
            this.Bot_Dimension_Text.ReadOnly = true;
            this.Bot_Dimension_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Dimension_Text.Size = new System.Drawing.Size(145, 22);
            this.Bot_Dimension_Text.TabIndex = 13;
            this.Bot_Dimension_Text.Text = "";
            // 
            // Bot_Gamemode
            // 
            this.Bot_Gamemode.AutoSize = true;
            this.Bot_Gamemode.Location = new System.Drawing.Point(490, 194);
            this.Bot_Gamemode.Name = "Bot_Gamemode";
            this.Bot_Gamemode.Size = new System.Drawing.Size(83, 13);
            this.Bot_Gamemode.TabIndex = 14;
            this.Bot_Gamemode.Text = "Bot Gamemode:";
            // 
            // Bot_Gamemode_Text
            // 
            this.Bot_Gamemode_Text.HideSelection = false;
            this.Bot_Gamemode_Text.Location = new System.Drawing.Point(579, 191);
            this.Bot_Gamemode_Text.Name = "Bot_Gamemode_Text";
            this.Bot_Gamemode_Text.ReadOnly = true;
            this.Bot_Gamemode_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Gamemode_Text.Size = new System.Drawing.Size(145, 22);
            this.Bot_Gamemode_Text.TabIndex = 15;
            this.Bot_Gamemode_Text.Text = "";
            // 
            // Bot_UUID
            // 
            this.Bot_UUID.AutoSize = true;
            this.Bot_UUID.Location = new System.Drawing.Point(490, 50);
            this.Bot_UUID.Name = "Bot_UUID";
            this.Bot_UUID.Size = new System.Drawing.Size(56, 13);
            this.Bot_UUID.TabIndex = 16;
            this.Bot_UUID.Text = "Bot UUID:";
            // 
            // Bot_UUID_Text
            // 
            this.Bot_UUID_Text.HideSelection = false;
            this.Bot_UUID_Text.Location = new System.Drawing.Point(579, 41);
            this.Bot_UUID_Text.Name = "Bot_UUID_Text";
            this.Bot_UUID_Text.ReadOnly = true;
            this.Bot_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_UUID_Text.Size = new System.Drawing.Size(145, 32);
            this.Bot_UUID_Text.TabIndex = 17;
            this.Bot_UUID_Text.Text = "";
            // 
            // Bot_Alive
            // 
            this.Bot_Alive.AutoSize = true;
            this.Bot_Alive.Location = new System.Drawing.Point(490, 222);
            this.Bot_Alive.Name = "Bot_Alive";
            this.Bot_Alive.Size = new System.Drawing.Size(52, 13);
            this.Bot_Alive.TabIndex = 18;
            this.Bot_Alive.Text = "Bot Alive:";
            // 
            // Bot_Alive_Text
            // 
            this.Bot_Alive_Text.HideSelection = false;
            this.Bot_Alive_Text.Location = new System.Drawing.Point(579, 219);
            this.Bot_Alive_Text.Name = "Bot_Alive_Text";
            this.Bot_Alive_Text.ReadOnly = true;
            this.Bot_Alive_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Alive_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Alive_Text.TabIndex = 19;
            this.Bot_Alive_Text.Text = "";
            // 
            // Bot_Inv_Full
            // 
            this.Bot_Inv_Full.AutoSize = true;
            this.Bot_Inv_Full.Location = new System.Drawing.Point(730, 16);
            this.Bot_Inv_Full.Name = "Bot_Inv_Full";
            this.Bot_Inv_Full.Size = new System.Drawing.Size(63, 13);
            this.Bot_Inv_Full.TabIndex = 20;
            this.Bot_Inv_Full.Text = "Bot Inv Full:";
            // 
            // Bot_Inv_FSlots
            // 
            this.Bot_Inv_FSlots.AutoSize = true;
            this.Bot_Inv_FSlots.Location = new System.Drawing.Point(730, 44);
            this.Bot_Inv_FSlots.Name = "Bot_Inv_FSlots";
            this.Bot_Inv_FSlots.Size = new System.Drawing.Size(76, 13);
            this.Bot_Inv_FSlots.TabIndex = 21;
            this.Bot_Inv_FSlots.Text = "Bot Free Slots:";
            // 
            // Bot_Inv_FSlots_Text
            // 
            this.Bot_Inv_FSlots_Text.HideSelection = false;
            this.Bot_Inv_FSlots_Text.Location = new System.Drawing.Point(816, 41);
            this.Bot_Inv_FSlots_Text.Name = "Bot_Inv_FSlots_Text";
            this.Bot_Inv_FSlots_Text.ReadOnly = true;
            this.Bot_Inv_FSlots_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_FSlots_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Inv_FSlots_Text.TabIndex = 22;
            this.Bot_Inv_FSlots_Text.Text = "";
            // 
            // Bot_Inv_Full_Text
            // 
            this.Bot_Inv_Full_Text.HideSelection = false;
            this.Bot_Inv_Full_Text.Location = new System.Drawing.Point(816, 13);
            this.Bot_Inv_Full_Text.Name = "Bot_Inv_Full_Text";
            this.Bot_Inv_Full_Text.ReadOnly = true;
            this.Bot_Inv_Full_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_Full_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Inv_Full_Text.TabIndex = 23;
            this.Bot_Inv_Full_Text.Text = "";
            // 
            // Bot_Inv_USlots
            // 
            this.Bot_Inv_USlots.AutoSize = true;
            this.Bot_Inv_USlots.Location = new System.Drawing.Point(730, 72);
            this.Bot_Inv_USlots.Name = "Bot_Inv_USlots";
            this.Bot_Inv_USlots.Size = new System.Drawing.Size(80, 13);
            this.Bot_Inv_USlots.TabIndex = 24;
            this.Bot_Inv_USlots.Text = "Bot Used Slots:";
            // 
            // Bot_Inv_USlots_Text
            // 
            this.Bot_Inv_USlots_Text.HideSelection = false;
            this.Bot_Inv_USlots_Text.Location = new System.Drawing.Point(816, 69);
            this.Bot_Inv_USlots_Text.Name = "Bot_Inv_USlots_Text";
            this.Bot_Inv_USlots_Text.ReadOnly = true;
            this.Bot_Inv_USlots_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Bot_Inv_USlots_Text.Size = new System.Drawing.Size(36, 22);
            this.Bot_Inv_USlots_Text.TabIndex = 25;
            this.Bot_Inv_USlots_Text.Text = "";
            // 
            // Closest_Player
            // 
            this.Closest_Player.AutoSize = true;
            this.Closest_Player.Location = new System.Drawing.Point(858, 16);
            this.Closest_Player.Name = "Closest_Player";
            this.Closest_Player.Size = new System.Drawing.Size(76, 13);
            this.Closest_Player.TabIndex = 26;
            this.Closest_Player.Text = "Closest Player:";
            // 
            // Closest_Player_UUID
            // 
            this.Closest_Player_UUID.AutoSize = true;
            this.Closest_Player_UUID.Location = new System.Drawing.Point(858, 50);
            this.Closest_Player_UUID.Name = "Closest_Player_UUID";
            this.Closest_Player_UUID.Size = new System.Drawing.Size(106, 13);
            this.Closest_Player_UUID.TabIndex = 27;
            this.Closest_Player_UUID.Text = "Closest Player UUID:";
            // 
            // Closest_Player_Loc
            // 
            this.Closest_Player_Loc.AutoSize = true;
            this.Closest_Player_Loc.Location = new System.Drawing.Point(858, 82);
            this.Closest_Player_Loc.Name = "Closest_Player_Loc";
            this.Closest_Player_Loc.Size = new System.Drawing.Size(97, 13);
            this.Closest_Player_Loc.TabIndex = 28;
            this.Closest_Player_Loc.Text = "Closest Player Loc:";
            // 
            // Closest_Player_Dist
            // 
            this.Closest_Player_Dist.AutoSize = true;
            this.Closest_Player_Dist.Location = new System.Drawing.Point(858, 110);
            this.Closest_Player_Dist.Name = "Closest_Player_Dist";
            this.Closest_Player_Dist.Size = new System.Drawing.Size(97, 13);
            this.Closest_Player_Dist.TabIndex = 29;
            this.Closest_Player_Dist.Text = "Closest Player Dist:";
            // 
            // Closest_Player_Text
            // 
            this.Closest_Player_Text.HideSelection = false;
            this.Closest_Player_Text.Location = new System.Drawing.Point(970, 13);
            this.Closest_Player_Text.Name = "Closest_Player_Text";
            this.Closest_Player_Text.ReadOnly = true;
            this.Closest_Player_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Player_Text.TabIndex = 30;
            this.Closest_Player_Text.Text = "";
            // 
            // Closest_Player_UUID_Text
            // 
            this.Closest_Player_UUID_Text.HideSelection = false;
            this.Closest_Player_UUID_Text.Location = new System.Drawing.Point(970, 41);
            this.Closest_Player_UUID_Text.Name = "Closest_Player_UUID_Text";
            this.Closest_Player_UUID_Text.ReadOnly = true;
            this.Closest_Player_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_UUID_Text.Size = new System.Drawing.Size(145, 32);
            this.Closest_Player_UUID_Text.TabIndex = 31;
            this.Closest_Player_UUID_Text.Text = "";
            // 
            // Closest_Player_Loc_Text
            // 
            this.Closest_Player_Loc_Text.HideSelection = false;
            this.Closest_Player_Loc_Text.Location = new System.Drawing.Point(970, 79);
            this.Closest_Player_Loc_Text.Name = "Closest_Player_Loc_Text";
            this.Closest_Player_Loc_Text.ReadOnly = true;
            this.Closest_Player_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Loc_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Player_Loc_Text.TabIndex = 32;
            this.Closest_Player_Loc_Text.Text = "";
            // 
            // Closest_Player_Dist_Text
            // 
            this.Closest_Player_Dist_Text.HideSelection = false;
            this.Closest_Player_Dist_Text.Location = new System.Drawing.Point(970, 107);
            this.Closest_Player_Dist_Text.Name = "Closest_Player_Dist_Text";
            this.Closest_Player_Dist_Text.ReadOnly = true;
            this.Closest_Player_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Player_Dist_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Player_Dist_Text.TabIndex = 33;
            this.Closest_Player_Dist_Text.Text = "";
            // 
            // Closest_Mob
            // 
            this.Closest_Mob.AutoSize = true;
            this.Closest_Mob.Location = new System.Drawing.Point(858, 293);
            this.Closest_Mob.Name = "Closest_Mob";
            this.Closest_Mob.Size = new System.Drawing.Size(68, 13);
            this.Closest_Mob.TabIndex = 34;
            this.Closest_Mob.Text = "Closest Mob:";
            // 
            // Closest_Mob_Text
            // 
            this.Closest_Mob_Text.HideSelection = false;
            this.Closest_Mob_Text.Location = new System.Drawing.Point(970, 290);
            this.Closest_Mob_Text.Name = "Closest_Mob_Text";
            this.Closest_Mob_Text.ReadOnly = true;
            this.Closest_Mob_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Mob_Text.TabIndex = 35;
            this.Closest_Mob_Text.Text = "";
            // 
            // Closest_Mob_Loc
            // 
            this.Closest_Mob_Loc.AutoSize = true;
            this.Closest_Mob_Loc.Location = new System.Drawing.Point(858, 321);
            this.Closest_Mob_Loc.Name = "Closest_Mob_Loc";
            this.Closest_Mob_Loc.Size = new System.Drawing.Size(89, 13);
            this.Closest_Mob_Loc.TabIndex = 36;
            this.Closest_Mob_Loc.Text = "Closest Mob Loc:";
            // 
            // Closest_Mob_Loc_Text
            // 
            this.Closest_Mob_Loc_Text.HideSelection = false;
            this.Closest_Mob_Loc_Text.Location = new System.Drawing.Point(970, 318);
            this.Closest_Mob_Loc_Text.Name = "Closest_Mob_Loc_Text";
            this.Closest_Mob_Loc_Text.ReadOnly = true;
            this.Closest_Mob_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Loc_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Mob_Loc_Text.TabIndex = 37;
            this.Closest_Mob_Loc_Text.Text = "";
            // 
            // Closest_Mob_Dist
            // 
            this.Closest_Mob_Dist.AutoSize = true;
            this.Closest_Mob_Dist.Location = new System.Drawing.Point(858, 349);
            this.Closest_Mob_Dist.Name = "Closest_Mob_Dist";
            this.Closest_Mob_Dist.Size = new System.Drawing.Size(89, 13);
            this.Closest_Mob_Dist.TabIndex = 38;
            this.Closest_Mob_Dist.Text = "Closest Mob Dist:";
            // 
            // Closest_Mob_Dist_Text
            // 
            this.Closest_Mob_Dist_Text.HideSelection = false;
            this.Closest_Mob_Dist_Text.Location = new System.Drawing.Point(970, 346);
            this.Closest_Mob_Dist_Text.Name = "Closest_Mob_Dist_Text";
            this.Closest_Mob_Dist_Text.ReadOnly = true;
            this.Closest_Mob_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Closest_Mob_Dist_Text.Size = new System.Drawing.Size(145, 22);
            this.Closest_Mob_Dist_Text.TabIndex = 39;
            this.Closest_Mob_Dist_Text.Text = "";
            // 
            // Targetable_Player_Text
            // 
            this.Targetable_Player_Text.HideSelection = false;
            this.Targetable_Player_Text.Location = new System.Drawing.Point(970, 147);
            this.Targetable_Player_Text.Name = "Targetable_Player_Text";
            this.Targetable_Player_Text.ReadOnly = true;
            this.Targetable_Player_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Text.Size = new System.Drawing.Size(145, 22);
            this.Targetable_Player_Text.TabIndex = 40;
            this.Targetable_Player_Text.Text = "";
            // 
            // Targetable_Player_UUID_Text
            // 
            this.Targetable_Player_UUID_Text.HideSelection = false;
            this.Targetable_Player_UUID_Text.Location = new System.Drawing.Point(970, 175);
            this.Targetable_Player_UUID_Text.Name = "Targetable_Player_UUID_Text";
            this.Targetable_Player_UUID_Text.ReadOnly = true;
            this.Targetable_Player_UUID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_UUID_Text.Size = new System.Drawing.Size(145, 32);
            this.Targetable_Player_UUID_Text.TabIndex = 41;
            this.Targetable_Player_UUID_Text.Text = "";
            // 
            // Targetable_Player_Loc_Text
            // 
            this.Targetable_Player_Loc_Text.HideSelection = false;
            this.Targetable_Player_Loc_Text.Location = new System.Drawing.Point(970, 213);
            this.Targetable_Player_Loc_Text.Name = "Targetable_Player_Loc_Text";
            this.Targetable_Player_Loc_Text.ReadOnly = true;
            this.Targetable_Player_Loc_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Loc_Text.Size = new System.Drawing.Size(145, 22);
            this.Targetable_Player_Loc_Text.TabIndex = 42;
            this.Targetable_Player_Loc_Text.Text = "";
            // 
            // Targetable_Player_Dist_Text
            // 
            this.Targetable_Player_Dist_Text.HideSelection = false;
            this.Targetable_Player_Dist_Text.Location = new System.Drawing.Point(970, 247);
            this.Targetable_Player_Dist_Text.Name = "Targetable_Player_Dist_Text";
            this.Targetable_Player_Dist_Text.ReadOnly = true;
            this.Targetable_Player_Dist_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Targetable_Player_Dist_Text.Size = new System.Drawing.Size(145, 22);
            this.Targetable_Player_Dist_Text.TabIndex = 43;
            this.Targetable_Player_Dist_Text.Text = "";
            // 
            // Targetable_Player
            // 
            this.Targetable_Player.AutoSize = true;
            this.Targetable_Player.Location = new System.Drawing.Point(858, 150);
            this.Targetable_Player.Name = "Targetable_Player";
            this.Targetable_Player.Size = new System.Drawing.Size(93, 13);
            this.Targetable_Player.TabIndex = 44;
            this.Targetable_Player.Text = "Targetable Player:";
            // 
            // Targetable_Player_UUID
            // 
            this.Targetable_Player_UUID.AutoSize = true;
            this.Targetable_Player_UUID.Location = new System.Drawing.Point(858, 178);
            this.Targetable_Player_UUID.Name = "Targetable_Player_UUID";
            this.Targetable_Player_UUID.Size = new System.Drawing.Size(90, 26);
            this.Targetable_Player_UUID.TabIndex = 45;
            this.Targetable_Player_UUID.Text = "Targetable Player\r\nUUID:";
            // 
            // Targetable_Player_Loc
            // 
            this.Targetable_Player_Loc.AutoSize = true;
            this.Targetable_Player_Loc.Location = new System.Drawing.Point(858, 213);
            this.Targetable_Player_Loc.Name = "Targetable_Player_Loc";
            this.Targetable_Player_Loc.Size = new System.Drawing.Size(90, 26);
            this.Targetable_Player_Loc.TabIndex = 46;
            this.Targetable_Player_Loc.Text = "Targetable Player\r\nLoc:";
            // 
            // Targetable_Player_Dist
            // 
            this.Targetable_Player_Dist.AutoSize = true;
            this.Targetable_Player_Dist.Location = new System.Drawing.Point(858, 247);
            this.Targetable_Player_Dist.Name = "Targetable_Player_Dist";
            this.Targetable_Player_Dist.Size = new System.Drawing.Size(90, 26);
            this.Targetable_Player_Dist.TabIndex = 47;
            this.Targetable_Player_Dist.Text = "Targetable Player\r\nDist:";
            // 
            // Held_Item_ID
            // 
            this.Held_Item_ID.AutoSize = true;
            this.Held_Item_ID.Location = new System.Drawing.Point(730, 110);
            this.Held_Item_ID.Name = "Held_Item_ID";
            this.Held_Item_ID.Size = new System.Drawing.Size(69, 13);
            this.Held_Item_ID.TabIndex = 48;
            this.Held_Item_ID.Text = "Held Item ID:";
            // 
            // Held_Item_ID_Text
            // 
            this.Held_Item_ID_Text.HideSelection = false;
            this.Held_Item_ID_Text.Location = new System.Drawing.Point(816, 107);
            this.Held_Item_ID_Text.Name = "Held_Item_ID_Text";
            this.Held_Item_ID_Text.ReadOnly = true;
            this.Held_Item_ID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_ID_Text.Size = new System.Drawing.Size(36, 22);
            this.Held_Item_ID_Text.TabIndex = 49;
            this.Held_Item_ID_Text.Text = "";
            // 
            // Held_Item_Count
            // 
            this.Held_Item_Count.AutoSize = true;
            this.Held_Item_Count.Location = new System.Drawing.Point(730, 138);
            this.Held_Item_Count.Name = "Held_Item_Count";
            this.Held_Item_Count.Size = new System.Drawing.Size(86, 13);
            this.Held_Item_Count.TabIndex = 50;
            this.Held_Item_Count.Text = "Held Item Count:";
            // 
            // Held_Item_Count_Text
            // 
            this.Held_Item_Count_Text.HideSelection = false;
            this.Held_Item_Count_Text.Location = new System.Drawing.Point(816, 135);
            this.Held_Item_Count_Text.Name = "Held_Item_Count_Text";
            this.Held_Item_Count_Text.ReadOnly = true;
            this.Held_Item_Count_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_Count_Text.Size = new System.Drawing.Size(36, 22);
            this.Held_Item_Count_Text.TabIndex = 51;
            this.Held_Item_Count_Text.Text = "";
            // 
            // Held_Item_Slot
            // 
            this.Held_Item_Slot.AutoSize = true;
            this.Held_Item_Slot.Location = new System.Drawing.Point(730, 166);
            this.Held_Item_Slot.Name = "Held_Item_Slot";
            this.Held_Item_Slot.Size = new System.Drawing.Size(76, 13);
            this.Held_Item_Slot.TabIndex = 52;
            this.Held_Item_Slot.Text = "Held Item Slot:";
            // 
            // Held_Item_Slot_Text
            // 
            this.Held_Item_Slot_Text.HideSelection = false;
            this.Held_Item_Slot_Text.Location = new System.Drawing.Point(816, 163);
            this.Held_Item_Slot_Text.Name = "Held_Item_Slot_Text";
            this.Held_Item_Slot_Text.ReadOnly = true;
            this.Held_Item_Slot_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Held_Item_Slot_Text.Size = new System.Drawing.Size(36, 22);
            this.Held_Item_Slot_Text.TabIndex = 53;
            this.Held_Item_Slot_Text.Text = "";
            // 
            // Held_Item_NBT
            // 
            this.Held_Item_NBT.AutoSize = true;
            this.Held_Item_NBT.Location = new System.Drawing.Point(644, 250);
            this.Held_Item_NBT.Name = "Held_Item_NBT";
            this.Held_Item_NBT.Size = new System.Drawing.Size(80, 13);
            this.Held_Item_NBT.TabIndex = 54;
            this.Held_Item_NBT.Text = "Held Item NBT:";
            // 
            // Held_Item_NBT_Text
            // 
            this.Held_Item_NBT_Text.HideSelection = false;
            this.Held_Item_NBT_Text.Location = new System.Drawing.Point(493, 266);
            this.Held_Item_NBT_Text.Name = "Held_Item_NBT_Text";
            this.Held_Item_NBT_Text.ReadOnly = true;
            this.Held_Item_NBT_Text.Size = new System.Drawing.Size(359, 102);
            this.Held_Item_NBT_Text.TabIndex = 55;
            this.Held_Item_NBT_Text.Text = "";
            // 
            // Window_Title
            // 
            this.Window_Title.AutoSize = true;
            this.Window_Title.Location = new System.Drawing.Point(1121, 16);
            this.Window_Title.Name = "Window_Title";
            this.Window_Title.Size = new System.Drawing.Size(72, 13);
            this.Window_Title.TabIndex = 56;
            this.Window_Title.Text = "Window Title:";
            // 
            // Window_Title_Text
            // 
            this.Window_Title_Text.HideSelection = false;
            this.Window_Title_Text.Location = new System.Drawing.Point(1226, 13);
            this.Window_Title_Text.Name = "Window_Title_Text";
            this.Window_Title_Text.ReadOnly = true;
            this.Window_Title_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Window_Title_Text.Size = new System.Drawing.Size(205, 51);
            this.Window_Title_Text.TabIndex = 57;
            this.Window_Title_Text.Text = "";
            // 
            // Window_Type
            // 
            this.Window_Type.AutoSize = true;
            this.Window_Type.Location = new System.Drawing.Point(1121, 73);
            this.Window_Type.Name = "Window_Type";
            this.Window_Type.Size = new System.Drawing.Size(76, 13);
            this.Window_Type.TabIndex = 58;
            this.Window_Type.Text = "Window Type:";
            // 
            // Window_ID
            // 
            this.Window_ID.AutoSize = true;
            this.Window_ID.Location = new System.Drawing.Point(1121, 101);
            this.Window_ID.Name = "Window_ID";
            this.Window_ID.Size = new System.Drawing.Size(63, 13);
            this.Window_ID.TabIndex = 59;
            this.Window_ID.Text = "Window ID:";
            // 
            // Window_Type_Text
            // 
            this.Window_Type_Text.HideSelection = false;
            this.Window_Type_Text.Location = new System.Drawing.Point(1226, 70);
            this.Window_Type_Text.Name = "Window_Type_Text";
            this.Window_Type_Text.ReadOnly = true;
            this.Window_Type_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_Type_Text.Size = new System.Drawing.Size(205, 22);
            this.Window_Type_Text.TabIndex = 60;
            this.Window_Type_Text.Text = "";
            // 
            // Window_ID_Text
            // 
            this.Window_ID_Text.HideSelection = false;
            this.Window_ID_Text.Location = new System.Drawing.Point(1226, 98);
            this.Window_ID_Text.Name = "Window_ID_Text";
            this.Window_ID_Text.ReadOnly = true;
            this.Window_ID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_ID_Text.Size = new System.Drawing.Size(36, 22);
            this.Window_ID_Text.TabIndex = 61;
            this.Window_ID_Text.Text = "";
            // 
            // Window_Slotcount_Text
            // 
            this.Window_Slotcount_Text.HideSelection = false;
            this.Window_Slotcount_Text.Location = new System.Drawing.Point(1226, 126);
            this.Window_Slotcount_Text.Name = "Window_Slotcount_Text";
            this.Window_Slotcount_Text.ReadOnly = true;
            this.Window_Slotcount_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_Slotcount_Text.Size = new System.Drawing.Size(36, 22);
            this.Window_Slotcount_Text.TabIndex = 62;
            this.Window_Slotcount_Text.Text = "";
            // 
            // Window_EntityID_Text
            // 
            this.Window_EntityID_Text.HideSelection = false;
            this.Window_EntityID_Text.Location = new System.Drawing.Point(1226, 154);
            this.Window_EntityID_Text.Name = "Window_EntityID_Text";
            this.Window_EntityID_Text.ReadOnly = true;
            this.Window_EntityID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_EntityID_Text.Size = new System.Drawing.Size(36, 22);
            this.Window_EntityID_Text.TabIndex = 63;
            this.Window_EntityID_Text.Text = "";
            // 
            // Window_ActionID_Text
            // 
            this.Window_ActionID_Text.HideSelection = false;
            this.Window_ActionID_Text.Location = new System.Drawing.Point(1224, 182);
            this.Window_ActionID_Text.Name = "Window_ActionID_Text";
            this.Window_ActionID_Text.ReadOnly = true;
            this.Window_ActionID_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.Window_ActionID_Text.Size = new System.Drawing.Size(36, 22);
            this.Window_ActionID_Text.TabIndex = 64;
            this.Window_ActionID_Text.Text = "";
            // 
            // Window_Slotcount
            // 
            this.Window_Slotcount.AutoSize = true;
            this.Window_Slotcount.Location = new System.Drawing.Point(1121, 129);
            this.Window_Slotcount.Name = "Window_Slotcount";
            this.Window_Slotcount.Size = new System.Drawing.Size(97, 13);
            this.Window_Slotcount.TabIndex = 65;
            this.Window_Slotcount.Text = "Window Slotcount:";
            // 
            // Window_EntityID
            // 
            this.Window_EntityID.AutoSize = true;
            this.Window_EntityID.Location = new System.Drawing.Point(1121, 157);
            this.Window_EntityID.Name = "Window_EntityID";
            this.Window_EntityID.Size = new System.Drawing.Size(89, 13);
            this.Window_EntityID.TabIndex = 66;
            this.Window_EntityID.Text = "Window EntityID:";
            // 
            // Window_ActionID
            // 
            this.Window_ActionID.AutoSize = true;
            this.Window_ActionID.Location = new System.Drawing.Point(1121, 185);
            this.Window_ActionID.Name = "Window_ActionID";
            this.Window_ActionID.Size = new System.Drawing.Size(93, 13);
            this.Window_ActionID.TabIndex = 67;
            this.Window_ActionID.Text = "Window ActionID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1121, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "Window Slotids:";
            // 
            // Window_Slotids_Text
            // 
            this.Window_Slotids_Text.HideSelection = false;
            this.Window_Slotids_Text.Location = new System.Drawing.Point(1224, 210);
            this.Window_Slotids_Text.Name = "Window_Slotids_Text";
            this.Window_Slotids_Text.ReadOnly = true;
            this.Window_Slotids_Text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.Window_Slotids_Text.Size = new System.Drawing.Size(205, 158);
            this.Window_Slotids_Text.TabIndex = 69;
            this.Window_Slotids_Text.Text = "";
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1500, 381);
            this.Controls.Add(this.Window_Slotids_Text);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Window_ActionID);
            this.Controls.Add(this.Window_EntityID);
            this.Controls.Add(this.Window_Slotcount);
            this.Controls.Add(this.Window_ActionID_Text);
            this.Controls.Add(this.Window_EntityID_Text);
            this.Controls.Add(this.Window_Slotcount_Text);
            this.Controls.Add(this.Window_ID_Text);
            this.Controls.Add(this.Window_Type_Text);
            this.Controls.Add(this.Window_ID);
            this.Controls.Add(this.Window_Type);
            this.Controls.Add(this.Window_Title_Text);
            this.Controls.Add(this.Window_Title);
            this.Controls.Add(this.Held_Item_NBT_Text);
            this.Controls.Add(this.Held_Item_NBT);
            this.Controls.Add(this.Held_Item_Slot_Text);
            this.Controls.Add(this.Held_Item_Slot);
            this.Controls.Add(this.Held_Item_Count_Text);
            this.Controls.Add(this.Held_Item_Count);
            this.Controls.Add(this.Held_Item_ID_Text);
            this.Controls.Add(this.Held_Item_ID);
            this.Controls.Add(this.Targetable_Player_Dist);
            this.Controls.Add(this.Targetable_Player_Loc);
            this.Controls.Add(this.Targetable_Player_UUID);
            this.Controls.Add(this.Targetable_Player);
            this.Controls.Add(this.Targetable_Player_Dist_Text);
            this.Controls.Add(this.Targetable_Player_Loc_Text);
            this.Controls.Add(this.Targetable_Player_UUID_Text);
            this.Controls.Add(this.Targetable_Player_Text);
            this.Controls.Add(this.Closest_Mob_Dist_Text);
            this.Controls.Add(this.Closest_Mob_Dist);
            this.Controls.Add(this.Closest_Mob_Loc_Text);
            this.Controls.Add(this.Closest_Mob_Loc);
            this.Controls.Add(this.Closest_Mob_Text);
            this.Controls.Add(this.Closest_Mob);
            this.Controls.Add(this.Closest_Player_Dist_Text);
            this.Controls.Add(this.Closest_Player_Loc_Text);
            this.Controls.Add(this.Closest_Player_UUID_Text);
            this.Controls.Add(this.Closest_Player_Text);
            this.Controls.Add(this.Closest_Player_Dist);
            this.Controls.Add(this.Closest_Player_Loc);
            this.Controls.Add(this.Closest_Player_UUID);
            this.Controls.Add(this.Closest_Player);
            this.Controls.Add(this.Bot_Inv_USlots_Text);
            this.Controls.Add(this.Bot_Inv_USlots);
            this.Controls.Add(this.Bot_Inv_Full_Text);
            this.Controls.Add(this.Bot_Inv_FSlots_Text);
            this.Controls.Add(this.Bot_Inv_FSlots);
            this.Controls.Add(this.Bot_Inv_Full);
            this.Controls.Add(this.Bot_Alive_Text);
            this.Controls.Add(this.Bot_Alive);
            this.Controls.Add(this.Bot_UUID_Text);
            this.Controls.Add(this.Bot_UUID);
            this.Controls.Add(this.Bot_Gamemode_Text);
            this.Controls.Add(this.Bot_Gamemode);
            this.Controls.Add(this.Bot_Dimension_Text);
            this.Controls.Add(this.Bot_Dimension);
            this.Controls.Add(this.Bot_Location_Text);
            this.Controls.Add(this.Bot_Location);
            this.Controls.Add(this.Bot_Hunger_Text);
            this.Controls.Add(this.Bot_Health_Text);
            this.Controls.Add(this.Bot_Hunger);
            this.Controls.Add(this.Bot_Health);
            this.Controls.Add(this.Bot_Name_Text);
            this.Controls.Add(this.Bot_Name);
            this.Controls.Add(this.Chat_Box);
            this.Controls.Add(this.Chat_Send);
            this.Controls.Add(this.Chat_Message);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DebugForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox Chat_Message;
        private System.Windows.Forms.Button Chat_Send;
        private System.Windows.Forms.RichTextBox Chat_Box;
        private Label Bot_Name;
        private RichTextBox Bot_Name_Text;
        private Label Bot_Health;
        private Label Bot_Hunger;
        private RichTextBox Bot_Health_Text;
        private RichTextBox Bot_Hunger_Text;
        private Label Bot_Location;
        private RichTextBox Bot_Location_Text;
        private Label Bot_Dimension;
        private RichTextBox Bot_Dimension_Text;
        private Label Bot_Gamemode;
        private RichTextBox Bot_Gamemode_Text;
        private Label Bot_UUID;
        private RichTextBox Bot_UUID_Text;
        private Label Bot_Alive;
        private RichTextBox Bot_Alive_Text;
        private Label Bot_Inv_Full;
        private Label Bot_Inv_FSlots;
        private RichTextBox Bot_Inv_FSlots_Text;
        private RichTextBox Bot_Inv_Full_Text;
        private Label Bot_Inv_USlots;
        private RichTextBox Bot_Inv_USlots_Text;
        private Label Closest_Player;
        private Label Closest_Player_UUID;
        private Label Closest_Player_Loc;
        private Label Closest_Player_Dist;
        private RichTextBox Closest_Player_Text;
        private RichTextBox Closest_Player_UUID_Text;
        private RichTextBox Closest_Player_Loc_Text;
        private RichTextBox Closest_Player_Dist_Text;
        private Label Closest_Mob;
        private RichTextBox Closest_Mob_Text;
        private Label Closest_Mob_Loc;
        private RichTextBox Closest_Mob_Loc_Text;
        private Label Closest_Mob_Dist;
        private RichTextBox Closest_Mob_Dist_Text;
        private RichTextBox Targetable_Player_Text;
        private RichTextBox Targetable_Player_UUID_Text;
        private RichTextBox Targetable_Player_Loc_Text;
        private RichTextBox Targetable_Player_Dist_Text;
        private Label Targetable_Player;
        private Label Targetable_Player_UUID;
        private Label Targetable_Player_Loc;
        private Label Targetable_Player_Dist;
        private Label Held_Item_ID;
        private RichTextBox Held_Item_ID_Text;
        private Label Held_Item_Count;
        private RichTextBox Held_Item_Count_Text;
        private Label Held_Item_Slot;
        private RichTextBox Held_Item_Slot_Text;
        private Label Held_Item_NBT;
        private RichTextBox Held_Item_NBT_Text;
        private Label Window_Title;
        private RichTextBox Window_Title_Text;
        private Label Window_Type;
        private Label Window_ID;
        private RichTextBox Window_Type_Text;
        private RichTextBox Window_ID_Text;
        private RichTextBox Window_Slotcount_Text;
        private RichTextBox Window_EntityID_Text;
        private RichTextBox Window_ActionID_Text;
        private Label Window_Slotcount;
        private Label Window_EntityID;
        private Label Window_ActionID;
        private Label label1;
        private RichTextBox Window_Slotids_Text;
    }
}