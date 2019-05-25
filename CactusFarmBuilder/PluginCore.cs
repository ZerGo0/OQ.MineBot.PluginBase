using System.Diagnostics;
using System.Threading.Tasks;
using CactusFarmBuilder.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace CactusFarmBuilder
{
    [Plugin(3, "Cactus Farm Builder", "[BETA] Builds a cactus farm for you. (Will not add any layouts!)", "https://www.youtube.com/watch?v=uVdLJZZzuBs")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new DescriptionSetting("The following items are needed for all layouts: Cactus, Sand, String" +
                                               "\n\nThere are 3 different Layouts: CropHopper Default (selected by default), \nCropHopper Creative and Vanilla Mode." +
                                               "\n\nThe CropHopper Default layout is the most space efficient layout." +
                                               "\n\nAll layouts now support Creative mode, but it has to be activated before\nyou run the plugin! It will automatically detect Creative Mode!" +
                                               "\n\n1 Layer = Y+2\n", null));
            Setting.Add(new ComboSetting("Speed mode", null,
                new[] {"Slow", "Normal", "Fast", "Super Fast", "Hyperspeed"}, 1));
            Setting.Add(new NumberSetting("Max layers", null, 50, 1, 127));
            Setting.Add(new ComboSetting("Extend in this direction", null, new[] { "North", "East", "South", "West" }, 1));

            var cropHopperLayout = new GroupSetting("Crop Hopper Layouts", "Either Legit CropHopper Layout (selected by default) or Creative Layout.");
            cropHopperLayout.Add(new BoolSetting("Creative Layout", null, false));
            cropHopperLayout.Add(new BoolSetting("Linear Mode", "Removes the gaps between each cactus farm pillar", false));
            Setting.Add(cropHopperLayout);

            var vanillaLayout = new GroupSetting("Vanilla Layout", "Enable this if you want to build a vanilla farm.");
            vanillaLayout.Add(new BoolSetting("Vanilla Design", "",false));
            Setting.Add(vanillaLayout);
            
            Setting.Add(new LinkSetting("CropHopper Default Layout Showcase", "", "https://i.imgur.com/RzEdzng.png"));
            Setting.Add(new LinkSetting("CropHopper Default Linear Mode Layout Showcase", "", "https://i.imgur.com/C7sxNVI.png"));
            Setting.Add(new LinkSetting("CropHopper Creative Layout Showcase", "", "https://i.imgur.com/L15mAwd.png"));
            Setting.Add(new LinkSetting("CropHopper Creative Linear Mode Layout Showcase", "", "https://i.imgur.com/EyYUVhd.png"));
            Setting.Add(new LinkSetting("Vanilla Mode Layout Showcase", "", "https://i.imgur.com/XoIR2pj.png"));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");
            
            var cropHopperLayout = Setting.Get("Crop Hopper Layouts") as IParentSetting;
            var vanillaLayout = Setting.Get("Vanilla Layout") as IParentSetting;

            if (cropHopperLayout.GetValue<bool>("Creative Layout") && vanillaLayout.GetValue<bool>("Vanilla Design"))
                return new PluginResponse(false, "Please select only 1 Layout!");

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var cropHopperLayout = Setting.Get("Crop Hopper Layouts") as IParentSetting;
            var vanillaLayout = Setting.Get("Vanilla Layout") as IParentSetting;

            RegisterTask(new Tasks.CactusFarmBuilder(Setting.At(1).Get<int>(), cropHopperLayout.GetValue<bool>("Creative Layout"),
                Setting.At(2).Get<int>(), Setting.At(3).Get<int>(), cropHopperLayout.GetValue<bool>("Linear Mode"), vanillaLayout.GetValue<bool>("Vanilla Design")));
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