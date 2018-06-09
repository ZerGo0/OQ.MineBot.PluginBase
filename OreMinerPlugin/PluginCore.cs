using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OreMinerPlugin.Tasks;

namespace OreMinerPlugin
{
    [Plugin(1, "Ore miner", "Mines ores using xray.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[9];
            Setting[0] = new StringSetting("Macro on inventory full",
                "Starts the macro when the bots inventory is full.", "");
            Setting[1] = new BoolSetting("Diamond ore", "", true);
            Setting[2] = new BoolSetting("Emerald ore", "", true);
            Setting[3] = new BoolSetting("Iron ore", "", true);
            Setting[4] = new BoolSetting("Gold ore", "", true);
            Setting[5] = new BoolSetting("Redstone ore", "", false);
            Setting[6] = new BoolSetting("Lapis Lazuli ore", "", false);
            Setting[7] = new BoolSetting("Coal ore", "", false);
            Setting[8] = new StringSetting("Custom Block ID's",
                "Want to farm something else? Enter the ID here and split them with commas.", "");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");
            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            //Block ID stuff
            var blockString = Setting[8].Get<string>();
            var inputids = Regex.Match(blockString, "^[0-9,]*$");
            return !inputids.Success ? new PluginResponse(false, "Block ID's are wrong! Only numbers and commas are allowed!") : new PluginResponse(true);
        }

        public override void OnStart()
        {
            var macro = new MacroSync();

            RegisterTask(new Mine(Setting[1].Get<bool>(), Setting[2].Get<bool>(), Setting[3].Get<bool>(),
                Setting[4].Get<bool>(), Setting[5].Get<bool>(), Setting[6].Get<bool>(),
                Setting[7].Get<bool>(), Setting[8].Get<string>().Split(',').ToArray(),
                macro));
            RegisterTask(new InventoryMonitor(Setting[0].Get<string>(), macro));
        }

        public override void OnStop()
        {
            Mine.BeingMined.Clear();
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