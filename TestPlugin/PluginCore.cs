using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

using TestPlugin.Tasks;

namespace TestPlugin
{
    [Plugin(1, "Test Plugin", "", "")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("TARGET Username", "Enter the TARGET username here", ""));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            return string.IsNullOrWhiteSpace(Setting.GetValue<string>("TARGET Username"))
                ? new PluginResponse(false, "Invalid TARGET Username, please check your plugin settings.")
                : new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new TestingStuff(Setting.GetValue<string>("TARGET Username")));
        }
    }
}