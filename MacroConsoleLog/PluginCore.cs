using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace MacroConsoleLog
{
    public class PluginCore : IStartPlugin
    {
        /// <summary>
        /// Should be used to check compatability with the
        /// current version of the bot.
        /// </summary>
        public override void OnLoad(int version, int subversion, int buildversion) { }
        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            /* Regular plugin content */
            return base.OnEnable(botSettings);
        }
    }
}
