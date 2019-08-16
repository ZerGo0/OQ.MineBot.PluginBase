﻿using System;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using PvPWandererPlugin.Tasks;

namespace PvPWandererPlugin
{
    [Plugin(1, "PvP Wanderer Bot", "Wanders around and searches for players to attack", "https://www.youtube.com/watch?v=XXX")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) {
            Setting.Add(new ComboSetting("Mode", null, new string[] { "Passive", "Aggressive" }, 0));

            var clickGroup = new GroupSetting("Clicks", "");
                clickGroup.Add(new NumberSetting("Clicks per second", "How fast should the bot attack?", 5, 1, 60, 1));
                clickGroup.Add(new NumberSetting("Miss rate", "How often does the bot miss?", 15, 0, 100, 1));
            Setting.Add(clickGroup);

            var equipmentGroup = new GroupSetting("Equipment", "");
                equipmentGroup.Add(new BoolSetting("Auto equip best armor?", "Should the bot auto equip the best armor it has?", true));
                equipmentGroup.Add(new BoolSetting("Equip best weapon?", "Should the best item be auto equiped?", true));
            Setting.Add(equipmentGroup);
            Console.Clear();
        }

        public override PluginResponse OnEnable(IBotSettings botSettings) {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");
            if (!botSettings.loadEntities || !botSettings.loadMobs) return new PluginResponse(false, "'Load mobs' must be enabled.");
            return new PluginResponse(true);
        }

        public override void OnStart() {

            var clickGroup = (IParentSetting)Setting.Get("Clicks");
            var equipmentGroup = (IParentSetting)Setting.Get("Equipment");

            RegisterTask(new Attack((Mode)Setting.GetValue<int>("Mode"), clickGroup.GetValue<int>("Clicks per second"), clickGroup.GetValue<int>("Miss rate"), equipmentGroup.GetValue<bool>("Equip best weapon?")));
            RegisterTask(new Equipment(equipmentGroup.GetValue<bool>("Auto equip best armor?")));
        }
    }

    public enum Mode
    {
        Passive,
        Aggresive
    }
}
