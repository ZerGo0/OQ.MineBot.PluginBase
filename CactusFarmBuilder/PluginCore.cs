using System;
using System.Threading.Tasks;
using CactusFarmBuilder.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace CactusFarmBuilder
{
    [Plugin(1, "Cactus Farm Builder", "Builds a cactus farm for you (Thanks to @BobTheBotter)")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[4];
            Setting[0] = new ComboSetting("Speed mode", null, new[] { "Slow", "Normal", "Fast", "Super Fast", "Hyperspeed"}, 1);
            Setting[1] = new BoolSetting("Creative range", null, false);
            Setting[2] = new NumberSetting("Max layers", null, 50, 1, 999);
            Setting[3] = new ComboSetting("Extend in this direction", null, new[] { "North", "East", "South", "West" }, 1);
            //Setting[0] = new StringSetting("Macro on inventory full",
            //    "Starts the macro when the bots inventory is full.", "");
            //Setting[1] = new StringSetting("Start x y z", "(x y z) [Split by space]", "");
            //Setting[2] = new StringSetting("End x y z", "(x y z) [Split by space]", "");
            //Setting[3] = new StringSetting("Filler Block ID", "The block which is used to fill the area.", "");
            //Setting[4] = new StringSetting("Macro if no Filler Block",
            //    "Starts the macro when the bots has no filler block left.", "");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            //if (string.IsNullOrWhiteSpace(Setting[1].Get<string>()) &&
            //    string.IsNullOrWhiteSpace(Setting[2].Get<string>()))
            //    return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");
            //
            //if (string.IsNullOrWhiteSpace(Setting[3].Get<string>()))
            //    return new PluginResponse(false, "Invalid Block ID.");
            //
            //if (!Setting[1].Get<string>().Contains(' ') || !Setting[2].Get<string>().Contains(' '))
            //    return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");
            //
            //if (!Setting[3].Get<string>().All(char.IsDigit)) return new PluginResponse(false, "Invalid Block ID!");
            //
            //var startSplit = Setting[1].Get<string>().Split(' ');
            //var endSplit = Setting[2].Get<string>().Split(' ');
            //if (startSplit.Length != 3 || endSplit.Length != 3)
            //    return new PluginResponse(false, "Invalid coordinates (must be x y z).");

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var macro = new MacroSync();
            //var fillermacro = new MacroSync();
            //
            //var startSplit = Setting[1].Get<string>().Split(' ');
            //var endSplit = Setting[2].Get<string>().Split(' ');
            
            RegisterTask(new Tasks.CactusFarmBuilder(Setting[0].Get<int>(), Setting[1].Get<bool>(),
                Setting[2].Get<int>(), Setting[3].Get<int>(), macro));
            RegisterTask(new InventoryMonitor(macro));
        }

        public override void OnStop()
        {
            Tasks.CactusFarmBuilder.Broken.Clear();
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