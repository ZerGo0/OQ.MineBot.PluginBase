using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using RandomWordWinner.Tasks;

namespace RandomWordWinner
{
    [Plugin(1, "Random Word Winner", "Auto. send requested word. (Original author: Dampen59)")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("Text in front of the random word",
                "Example : You need to enter a captcha.", ""));
            Setting.Add(new StringSetting("Text with the random word",
                "Example : The first to write this word: %randomword% will win and get a reward!", ""));
            Setting.Add(new StringSetting("Command used to send the random word :",
                "If you need to do '/win 123' to send the random word, then just enter '/win' below. Leave blank if no command.",
                ""));
            Setting.Add(new NumberSetting("Minimum Delay",
                "The bot will send the message between the minimum delay and max delay.", 100, 0, 10000));
            Setting.Add(new NumberSetting("Maximum Delay",
                "The bot will send the message between the minimum delay and max delay.", 250, 1, 10000, 2));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadChat) return new PluginResponse(false, "'Load chat' must be enabled.");
            return Setting.At(3).Get<int>() > Setting.At(4).Get<int>()
                ? new PluginResponse(false, "Minimum Delay is greater than Maximum Delay.")
                : new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new Chat(Setting.At(0).Get<string>(), Setting.At(1).Get<string>(), Setting.At(2).Get<string>(),
                Setting.At(3).Get<int>(), Setting.At(4).Get<int>()));
        }
    }
}