using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Utility;
using OQ.MineBot.Protocols.Classes.Base;
using RaidFinder.Tasks;

namespace RaidFinder
{
    [Plugin(1, "Raid Finder", "Notifies the user on discord when the bot finds the desired block.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[4];
            Setting[0] = new StringSetting("User or Channel ID",
                "Enable developer mode: Settings->Appearance->Developer mode. Copy id: right click channel and click 'Copy ID'.",
                "");
            Setting[1] = new BoolSetting("Local notifications", "", true);
            Setting[2] = new StringSetting("Blocks to scan", "Test", "");
            Setting[3] = new LinkSetting("Add bot",
                "Adds the bot to your discord channel (you must have administrator permissions).",
                "https://discordapp.com/oauth2/authorize?client_id=299708378236583939&scope=bot&permissions=6152");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load worlds' must be enabled.");

            try
            {
                if (string.IsNullOrWhiteSpace(Setting[0].Get<string>()))
                    return new PluginResponse(false, "Could not parse discord id.");
                ulong.Parse(Setting[0].Get<string>());
            }
            catch (Exception)
            {
                return new PluginResponse(false, "Could not parse discord id.");
            }

            // Do warnings.
            if (!botSettings.loadWorld || botSettings.staticWorlds)
                DiscordHelper.Error("[RaidFinder] 'Load worlds' should be enabled, 'Shared worlds' should be disabled.",
                    584);

            Console.WriteLine("Start done");
            var blockString = Setting[2].Get<string>().ToString();
            var inputids = Regex.Match(blockString, "^[0-9,]*$");
            return !inputids.Success
                ? new PluginResponse(false, "Block ID's are wrong! Only numbers and commas are allowed!")
                : new PluginResponse(true);
        }

        public override void OnStart()
        {
            // Add listening tasks.
            RegisterTask(new Alerts(
                ulong.Parse(Setting[0].Get<string>()),
                Setting[1].Get<bool>(), Setting[2].Get<string>().Split(',').ToArray()
            ));
        }
    }
}