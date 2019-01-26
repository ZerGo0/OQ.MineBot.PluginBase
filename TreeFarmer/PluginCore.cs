using System;
using System.Linq;
using System.Threading.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.Protocols.Classes.Base;
using TreeFarmerPlugin.Tasks;

namespace TreeFarmerPlugin
{
    [Plugin(1, "Tree Farmer", "Farms trees and shit.", "https://www.youtube.com/watch?v=6huwXOm3U6w")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("Macro on inventory full",
                "Starts the macro when the bots inventory is full.", ""));
            Setting.Add(new BoolSetting("Replant", "Check this if you want to replant trees (type ignored)", false));
            Setting.Add(new StringSetting("Start x y z", "Leave emtpy if you want to farm infinitely. (0 0 0)", ""));
            Setting.Add(new StringSetting("End x y z", "Leave emtpy if you want to farm infinitely. (0 0 0)", ""));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            if (string.IsNullOrWhiteSpace(Setting.At(2).Get<string>()) &&
                string.IsNullOrWhiteSpace(Setting.At(3).Get<string>())) return new PluginResponse(true);

            if (!Setting.At(2).Get<string>().Contains(' ') || !Setting.At(3).Get<string>().Contains(' '))
                return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");

            var startSplit = Setting.At(2).Get<string>().Split(' ');
            var endSplit = Setting.At(3).Get<string>().Split(' ');
            if (startSplit.Length != 3 || endSplit.Length != 3)
                return new PluginResponse(false, "Invalid coordinates (must be x y z).");

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var macro = new MacroSync();

            if (!string.IsNullOrWhiteSpace(Setting.At(2).Get<string>()) &&
                !string.IsNullOrWhiteSpace(Setting.At(3).Get<string>()))
            {
#if (DEBUG)
                Console.WriteLine("Got coords");
#endif
                var startSplit = Setting.At(2).Get<string>().Split(' ');
                var endSplit = Setting.At(3).Get<string>().Split(' ');

                RegisterTask(new MineArea(Setting.At(1).Get<bool>(),
                    new Location(int.Parse(startSplit[0]), int.Parse(startSplit[1]), int.Parse(startSplit[2])),
                    new Location(int.Parse(endSplit[0]), int.Parse(endSplit[1]), int.Parse(endSplit[2])), macro));
                RegisterTask(new InventoryMonitor(Setting.At(0).Get<string>(), macro));
            }
            else
            {
                RegisterTask(new Mine(Setting.At(1).Get<bool>(), macro));
                RegisterTask(new InventoryMonitor(Setting.At(0).Get<string>(), macro));
            }
        }

        public override void OnStop()
        {
            Mine.BeingMined.Clear();
            MineArea.Broken.Clear();
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