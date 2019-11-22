using System;
using System.IO;
using System.Reflection;

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
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new TestingStuff());
        }
    }
}