using CaptchaBreakerPlugin.Tasks;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace CaptchaBreakerPlugin
{
    [Plugin(1, "CaptchaBreaker", "Auto text captcha solver. (Original author: Dampen59)")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[4];
            Setting[0] = new StringSetting("Words that doesn't change between captcha requests :",
                "Some words used for requesting captcha", "Example : You need to enter a captcha.");
            Setting[1] = new StringSetting("Captcha request pattern :",
                "Captcha request, replace the captcha by %captcha%",
                "Example : You need to enter a captcha, please send %captcha% in the chat in order to connect.");
            Setting[2] = new StringSetting("Command used to send the captcha :",
                "If you need to do '/captcha 123' to send the captcha, just enter '/captcha' below. Leave blank if no command.",
                "");
            Setting[3] = new BoolSetting("Special characters", "Check this if the captcha contains any of this special chars: '*'\",_&#^@'", false);
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            return !botSettings.loadChat ? new PluginResponse(false, "'Load chat' must be enabled.") : new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new Chat(Setting[0].Get<string>(), Setting[1].Get<string>(), Setting[2].Get<string>(), Setting[3].Get<bool>()));
        }
    }
}