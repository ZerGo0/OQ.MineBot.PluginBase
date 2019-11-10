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
            Setting.Add(new ComboSetting("Mode", null, new[] { "Passive", "Aggressive", "Moving Passive", "Moving Aggressive" }, 0));

            var clickGroup = new GroupSetting("Clicks", "");
            clickGroup.Add(new NumberSetting("Clicks per second", "How fast should the bot attack?", 5, 1, 60));
            clickGroup.Add(new NumberSetting("Miss rate", "How often does the bot miss?", 15, 0, 100));
            Setting.Add(clickGroup);

            var equipmentGroup = new GroupSetting("Equipment", "");
            equipmentGroup.Add(new BoolSetting("Auto equip best armor?", "Should the bot auto equip the best armor it has?", true));
            equipmentGroup.Add(new BoolSetting("Equip best weapon?", "Should the best item be auto equiped?", true));
            Setting.Add(equipmentGroup);
            
            Console.WriteLine(ZerGo0Debugger.DefaultOutputFilename);
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            throw new Exception("TestPlugin");
//            RegisterTask(new ZerGo0Debugger(Setting.GetCollection()));
            RegisterTask(new TestingStuff());
        }
    }
}