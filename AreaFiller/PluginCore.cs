﻿using System.Linq;
using System.Threading.Tasks;
using AreaFiller.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.Protocols.Classes.Base;

namespace AreaFiller
{
    [Plugin(1, "Area Filler", "Select an area and the bot will fill it.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[5];
            Setting[0] = new StringSetting("Macro on inventory full",
                "Starts the macro when the bots inventory is full.", "");
            Setting[1] = new StringSetting("Start x y z", "(x y z) [Split by space]", "");
            Setting[2] = new StringSetting("End x y z", "(x y z) [Split by space]", "");
            Setting[3] = new StringSetting("Filler Block ID", "The block which is used to fill the area.", "");
            Setting[4] = new StringSetting("Macro if no Filler Block",
                "Starts the macro when the bots has no filler block left.", "");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            if (string.IsNullOrWhiteSpace(Setting[1].Get<string>()) &&
                string.IsNullOrWhiteSpace(Setting[2].Get<string>()))
                return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");

            if (string.IsNullOrWhiteSpace(Setting[3].Get<string>()))
                return new PluginResponse(false, "Invalid Block ID.");

            if (!Setting[1].Get<string>().Contains(' ') || !Setting[2].Get<string>().Contains(' '))
                return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");

            if (!Setting[3].Get<string>().All(char.IsDigit)) return new PluginResponse(false, "Invalid Block ID!");

            var startSplit = Setting[1].Get<string>().Split(' ');
            var endSplit = Setting[2].Get<string>().Split(' ');
            if (startSplit.Length != 3 || endSplit.Length != 3)
                return new PluginResponse(false, "Invalid coordinates (must be x y z).");

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var macro = new MacroSync();
            var fillermacro = new MacroSync();

            var startSplit = Setting[1].Get<string>().Split(' ');
            var endSplit = Setting[2].Get<string>().Split(' ');

            RegisterTask(new FillerArea(Setting[3].Get<string>(),
                new Location(int.Parse(startSplit[0]), int.Parse(startSplit[1]), int.Parse(startSplit[2])),
                new Location(int.Parse(endSplit[0]), int.Parse(endSplit[1]), int.Parse(endSplit[2])), macro, fillermacro));
            RegisterTask(new InventoryMonitor(Setting[3].Get<string>(), Setting[0].Get<string>(), macro, Setting[4].Get<string>(), fillermacro));
        }

        public override void OnStop()
        {
            FillerArea.Broken.Clear();
        }
    }

    public class MacroSync
    {
        private Task _macroTask;

        public bool IsMacroRunning()
        {
            //Check if there is an instance of the task.
            if (_macroTask == null) return false;
            //Check completion state.
            return !_macroTask.IsCompleted && !_macroTask.IsCanceled && !_macroTask.IsFaulted;
        }

        public void Run(IPlayer player, string name)
        {
            _macroTask = player.functions.StartMacro(name);
        }
    }
}