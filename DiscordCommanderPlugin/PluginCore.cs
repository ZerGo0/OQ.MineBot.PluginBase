using System;
using DiscordCommander.Tasks;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.PluginBase.Utility;

namespace DiscordCommander
{
    [Plugin(1, "Discord Commander", "Writes the user on discord when a user writes: 'KEYWORD message' ingame.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("User or Channel ID",
                "Enable developer mode: Settings->Appearance->Developer mode. Copy id: right click channel and click 'Copy ID'.",
                ""));
            Setting.Add(new StringSetting("Keyword", "Everything after this get's sent to discord. (split by ',')", ""));
            Setting.Add(new ComboSetting("Mode", "Notification mode", new[] {"none", "@everyone", "@everyone + tts"},
                1));
            Setting.Add(new LinkSetting("Add bot",
                "Adds the bot to your discord channel (you must have administrator permissions).",
                "https://discordapp.com/oauth2/authorize?client_id=299708378236583939&scope=bot&permissions=6152"));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadChat)
                return new PluginResponse(false, "[Discord Commander] 'Load chat' must be enabled.");

            if (string.IsNullOrWhiteSpace(Setting.At(1).Get<string>()))
                return new PluginResponse(false, "No keyword set.");

            try
            {
                if (string.IsNullOrWhiteSpace(Setting.At(0).Get<string>()))
                    return new PluginResponse(false, "Could not parse discord id.");
            }
            catch (Exception)
            {
                return new PluginResponse(false, "Could not parse discord id.");
            }

            return new PluginResponse(true);
        }

        public override void OnStart()
        {
            // Add listening tasks.
            RegisterTask(new Alerts(ulong.Parse(Setting.At(0).Get<string>()), Setting.At(1).Get<string>(),
                (DiscordHelper.Mode)Setting.At(2).Get<int>()));
        }
    }
}