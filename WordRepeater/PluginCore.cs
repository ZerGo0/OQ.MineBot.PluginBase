using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using WordRepeater.Tasks;

namespace WordRepeater
{
    [Plugin(1, "WordRepeater", "Writes everything you send after the keyword.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[2];
            Setting[0] = new StringSetting("Keyword", "Everything after this get's sent to discord", "");
            Setting[1] = new StringSetting("Command",
                "What should be in front of the message? (Leave empty if not needed)", "");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadChat)
                return new PluginResponse(false, "'Load chat' must be enabled.");

            return string.IsNullOrWhiteSpace(Setting[0].Get<string>())
                ? new PluginResponse(false, "No keyword set.")
                : new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new Alerts(Setting[0].Get<string>(), Setting[1].Get<string>()));
        }
    }
}