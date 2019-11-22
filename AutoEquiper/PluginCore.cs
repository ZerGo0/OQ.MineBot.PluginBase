using AutoEquiper.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace AutoEquiper
{
#if DEBUG
    [Plugin(2, "Auto Equiper", "(DEBUG BUILD)", "")]
#else
    [Plugin(2, "Auto Equiper", "Auto equips the selected items for you.", "")]
#endif
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) 
        {
            Setting.Add(new DescriptionSetting(
                "All items will be equipped after the plugin is start and if something in the inventory gets\nchanged.\n"
                , null));
            
            Setting.Add(new BoolSetting("Auto equip Armor?", "Should the bot auto equip the best armor it has?", true));
//            Setting.Add(new BoolSetting("Equip best weapon?", "Should the best sword be auto equiped?", true));
//            Setting.Add(new BoolSetting("Equip best pickaxe?", "Should the best pickaxe be auto equiped?", true));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings) {
            if (!Setting.At(1).Get<bool>()) 
                return new PluginResponse(false, "Nothing enabled, please check your plugin settings.");
            
            if (!botSettings.loadInventory && Setting.At(1).Get<bool>()) 
                return new PluginResponse(false, "'Load inventory' must be enabled.");

            ZerGo0Debugger.PluginSettings = Setting.GetCollection();
            
            return new PluginResponse(true);
        }

        public override void OnStart() {
            RegisterTask(new Equipment(Setting.At(1).Get<bool>()));
        }
    }
}
