using AutoSplashPotion.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace AutoSplashPotion
{
#if DEBUG
    [Plugin(1, "Auto Potion Thrower", "(DEBUG BUILD)", "")]
#else
    [Plugin(1, "Auto Potion Thrower", "Throws a Splash Potion if it's under the health threshold.", "")]
#endif
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) 
        {
            Setting.Add(new DescriptionSetting(
                "It will either try to throw a Instant Health or Regeneration Splash Potion.\n", null));
            
            Setting.Add(new NumberSetting("Health Threshold", "It will throw the Splash Potion if it's below this value.", 2, 2, 255));
            Setting.Add(new BoolSetting("Instant Health Potion", "Do you want to use Instant Health Potions?", true));
            Setting.Add(new BoolSetting("Regeneration Potion", "Do you want to use Regeneration Potions?", true));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings) {
            if (!Setting.At(2).Get<bool>() && !Setting.At(3).Get<bool>())
                return new PluginResponse(false, "You didn't select any potion type, please check your plugin settings!");
            
            if (!botSettings.loadInventory) 
                return new PluginResponse(false, "'Load inventory' must be enabled.");

            ZerGo0Debugger.PluginSettings = Setting.GetCollection();
            
            return new PluginResponse(true);
        }

        public override void OnStart() {
            RegisterTask(new Equipment(Setting.At(1).Get<int>(), Setting.At(2).Get<bool>(),
                Setting.At(3).Get<bool>()));
        }
    }
}
