using System.Linq;
using System.Threading.Tasks;

using AreaFiller.Tasks;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.Protocols.Classes.Base;

namespace AreaFiller
{
#if DEBUG
    [Plugin(3, "Area Filler", "(DEBUG BUILD)", "https://www.youtube.com/watch?v=ow-QSsbA3p8")]
#else
    [Plugin(3, "Area Filler", "Select an area and the bot will fill it.",
        "https://www.youtube.com/watch?v=ow-QSsbA3p8")]
#endif
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new LocationSetting("Start x y z", "DESC"));
            Setting.Add(new LocationSetting("End x y z", "DESC"));
            Setting.Add(new StringSetting("Building Block ID", "The block which is used to fill the area.", ""));
            Setting.Add(new StringSetting("Macro if no Filler Block",
                "Starts the macro when the bots has no building blocks left. (Doesn't need '.macro' included)", ""));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            if (!botSettings.loadInventory) return new PluginResponse(false, "'Load inventory' must be enabled.");

            if (Setting.At(0).Get<Location>() == null || Setting.At(1).Get<Location>() == null)
                return new PluginResponse(false, "Invalid coordinates, please check your plugin settings.");

            if (string.IsNullOrWhiteSpace(Setting.At(2).Get<string>()))
                return new PluginResponse(false, "Invalid Building Block ID, please check your plugin settings.");

            if (!Setting.At(2).Get<string>().All(char.IsDigit))
                return new PluginResponse(false, "Invalid Building Block ID, please check your plugin settings.");

            ZerGo0Debugger.PluginSettings = Setting.GetCollection();

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var fillermacro = new MacroSync();

            var fillerMacroName = Setting.At(3).Get<string>();
            if (fillerMacroName.Contains(".macro"))
                fillerMacroName = fillerMacroName.Replace(".macro", "");

            RegisterTask(new Filler(Setting.At(0).Get<Location>(), Setting.At(1).Get<Location>(),
                Setting.At(2).Get<string>(), fillermacro));
            RegisterTask(new InventoryMonitor(Setting.At(2).Get<string>(), fillerMacroName, fillermacro));
        }

        public override void OnDisable()
        {
            Filler.CurrentLayer = null;
            Filler.DoingShit = null;
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

        public void Run(IBotContext context, string name)
        {
            _macroTask = context.Functions.StartMacro(name);
        }
    }
}