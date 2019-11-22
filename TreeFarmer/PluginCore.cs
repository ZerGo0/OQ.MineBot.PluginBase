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
#if DEBUG
    [Plugin(2, "Tree Farmer", "(DEBUG BUILD)", "https://www.youtube.com/watch?v=6huwXOm3U6w")]
#else
    [Plugin(2, "Tree Farmer", "Farms trees and shit.", "https://www.youtube.com/watch?v=6huwXOm3U6w")]
#endif
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("Macro on inventory full",
                "Starts the macro when the bots inventory is full.", ""));
            Setting.Add(new BoolSetting("Replant", "Do you want to replant the tree? (type ignored)", false));
            Setting.Add(new LocationSetting("Start x y z", "Leave it at 0 0 0 if you want to farm infinitely."));
            Setting.Add(new LocationSetting("End x y z", "Leave it at 0 0 0 if you want to farm infinitely."));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");
            
            if (!botSettings.loadInventory) return new PluginResponse(false, "'Load inventory' must be enabled.");
            
            if (Setting.At(2).Get<Location>() == null || Setting.At(3).Get<Location>() == null)
                return new PluginResponse(false, "Invalid coordinates, please check your plugin settings.");
            
            ZerGo0Debugger.PluginSettings = Setting.GetCollection();

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var fullInvMacro = new MacroSync();

            var fullInvMacroName = Setting.At(0).Get<string>();
            if (fullInvMacroName.Contains(".macro"))
                fullInvMacroName = fullInvMacroName.Replace(".macro", "");

            if (!Setting.At(2).Get<Location>().Compare(new Location(0,0,0)) &&
                !Setting.At(3).Get<Location>().Compare(new Location(0,0,0)))
            {
                RegisterTask(new MineArea(Setting.At(2).Get<Location>(), 
                    Setting.At(3).Get<Location>(), Setting.At(1).Get<bool>(),
                    fullInvMacro));
//                RegisterTask(new InventoryMonitor(Setting.At(0).Get<string>(), macro));
            }
            else
            {
//                RegisterTask(new Mine(Setting.At(1).Get<bool>(), macro));
//                RegisterTask(new InventoryMonitor(Setting.At(0).Get<string>(), macro));
            }
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