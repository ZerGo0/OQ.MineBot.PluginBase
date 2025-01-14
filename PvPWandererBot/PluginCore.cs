﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.PluginBase.Classes;

using PvPBotPlugin.Tasks;

namespace PvPBotPlugin
{
    [Plugin(3, "PvPBot [BETA]", "Wanders around and searches for players to attack", "")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new ComboSetting("Mode", null, new[] {"Patrol Mode"}, 0));

            var combatGroup = new GroupSetting("Combat Settings", "");
            combatGroup.Add(new NumberSetting("Clicks per second", "How fast should the bot attack?", 5, 1, 60));
            combatGroup.Add(new NumberSetting("Miss Rate", "What should the possibility of a miss be?", 15, 0, 100));
            combatGroup.Add(new StringSetting("Target Whitelist",
                "Targets that you don't want to attack. (Split by SPACE)", ""));
            Setting.Add(combatGroup);

            var equipmentGroup = new GroupSetting("Equipment Settings", "");
            equipmentGroup.Add(new BoolSetting("Auto Armor",
                "Should the bot auto equip the best armour it has? (ignores enchantments)", true));
            equipmentGroup.Add(new BoolSetting("Auto Sword",
                "Should the best sword be auto equipped? (ignores enchantments)", true));
            Setting.Add(equipmentGroup);

            Console.Clear();
        }

        public override void OnDisable()
        {
            Attack.SharedTarget = null;
            Attack.SharedTargetLoaded = new ConcurrentDictionary<string, bool>();
            Attack.SharedTargetLocations = new List<ILocation>();
            Attack.SharedTargetReachable = new ConcurrentDictionary<string, bool>();
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld)
                return new PluginResponse(false, "'Load world' must be enabled.");
            if (botSettings.staticWorlds)
                return new PluginResponse(false, "'Shared world' must be disabled.");
            if (!botSettings.loadEntities || !botSettings.loadPlayers)
                return new PluginResponse(false, "'Load players' must be enabled.");
            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var combatGroup = (IParentSetting) Setting.Get("Combat Settings");
            var equipmentGroup = (IParentSetting) Setting.Get("Equipment Settings");
            var whitelistedTargets = combatGroup.GetValue<string>("Target Whitelist").Split(' ').ToList();

            RegisterTask(new Attack(Setting.GetValue<int>("Mode"),
                combatGroup.GetValue<int>("Clicks per second"),
                combatGroup.GetValue<int>("Miss Rate"),
                whitelistedTargets,
                equipmentGroup.GetValue<bool>("Auto Armor")));
            RegisterTask(new Equipment(equipmentGroup.GetValue<bool>("Auto Armor")));
        }
    }

    public static class ZerGo0Debugger
    {
        public static void WriteConsole(string playerName, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                              $"[{playerName}] {message}");
        }

        public static void ErrorMessage(string playerName, Exception ex)
        {
            var stack = ex.StackTrace;
            var source = ex.Source;
            var message = ex.Message;
            var nL = Environment.NewLine;

            const int padWidth = 18;
            const char padChar = ' ';

            var error = nL + 
                        "ERROR IN:".PadRight(padWidth, padChar) + $"{source}{nL}" +
                        "ERROR MESSAGE:".PadRight(padWidth, padChar) + $"{message}{nL}" +
                        "LAST INSTRUCTION:".PadRight(padWidth, padChar) + $"{stack.TrimStart(' ')}";

            if (ex.InnerException != null) error += nL + 
                                                    "INNER EXCEPTION:".PadRight(padWidth, padChar) + 
                                                    $"{ex.InnerException.Message}";

            error += nL;

            WriteConsole(playerName, error);
        }
    }
}