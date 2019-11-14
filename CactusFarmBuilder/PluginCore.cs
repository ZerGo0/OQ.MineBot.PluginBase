using CactusFarmBuilder.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace CactusFarmBuilder
{
#if DEBUG
    [Plugin(5, "Cactus Farm Builder", "(DEBUG BUILD)", "https://www.youtube.com/watch?v=uVdLJZZzuBs")]
#else
    [Plugin(5, "Cactus Farm Builder", "[BETA] Builds a cactus farm for you.",
        "https://www.youtube.com/watch?v=uVdLJZZzuBs")]
#endif
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new DescriptionSetting("The following items are needed for all layouts: Cactus, Sand, String" +
                                               "\n\nThere are 3 different Layouts: CropHopper, CropHopper Creative Range and Vanilla." +
                                               "\n\nAll layouts support Creative mode, it will automatically detect Creative Mode!\n",
                null));
            Setting.Add(new LinkSetting("Don't forget to leave a Like! :)", "",
                "https://www.minecraftbot.com/plugins.html"));

            Setting.Add(new ComboSetting("Layout", "", new[] {"CropHopper", "CropHopper Creative Range", "Vanilla"},
                0));
            Setting.Add(new ComboSetting("Speed mode", null, new[] {"Slow", "Normal", "Fast"}, 3));
            Setting.Add(new NumberSetting("Max layers", "1 Layer = Y+2", 50, 1, 127));
            Setting.Add(new ComboSetting("Extend in this direction", null, new[] {"North", "East", "South", "West"},
                1));

            var layoutPreviews = new GroupSetting("Layout Previews", "You can find a Preview for each layout here.");
            layoutPreviews.Add(new LinkSetting("CropHopper Layout Showcase", "", "https://i.imgur.com/bC41giN.png"));
            layoutPreviews.Add(new LinkSetting("CropHopper Creative Range Layout Showcase", "",
                "https://i.imgur.com/L15mAwd.png"));
            layoutPreviews.Add(new LinkSetting("Vanilla Layout Showcase", "", "https://i.imgur.com/b2oivhu.png"));
            Setting.Add(layoutPreviews);
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
#if !DEBUG
            if (Setting.At(2).Get<int>() == 1)
                return new PluginResponse(false, "The 'CropHopper Creative Range' layout isn't available right now.");
#endif
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");

            if (botSettings.staticWorlds) return new PluginResponse(false, "'Shared worlds' should be disabled.");

            if (!botSettings.loadInventory) return new PluginResponse(false, "'Load inventory' should be enabled.");

            ZerGo0Debugger.PluginSettings = Setting.GetCollection();

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            var mode = Setting.At(2).Get<int>();
            var speedmode = 0;
            switch (Setting.At(3).Get<int>())
            {
                case 0:
                    speedmode = 4;
                    break;
                case 1:
                    speedmode = 2;
                    break;
                case 2:
                    speedmode = 0;
                    break;
            }

            var maxlayers = Setting.At(4).Get<int>();
            var direction = Setting.At(5).Get<int>();

            switch (mode)
            {
                case 0:
                    RegisterTask(new CropHopperDefault(speedmode, maxlayers, direction));
                    break;
                case 1:
#if DEBUG
                    RegisterTask(new CropHopperCreative(speedmode, maxlayers, direction));
#endif
                    break;
                case 2:
                    RegisterTask(new VanillaMode(speedmode, maxlayers, direction));
                    break;
            }
        }
    }
}