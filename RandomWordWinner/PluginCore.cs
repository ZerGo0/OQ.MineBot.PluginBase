using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using RandomWordWinner.Tasks;

namespace RandomWordWinner
{
    [Plugin(6, "Random Word Winner", "Auto. send requested word. (Original author: Dampen59)", "https://www.youtube.com/watch?v=G5hSzRPJ66s")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting.Add(new StringSetting("Text in front of the random word",
                "Example : The first to write this word:", ""));
            Setting.Add(new StringSetting("Text with the random word",
                "Example : The first to write this word: %randomword% will win and get a reward!", ""));
            Setting.Add(new StringSetting("Command used to send the random word :",
                "If you need to do '/win 123' to send the random word, then just enter '/win' below. Leave blank if no command.",
                ""));
            Setting.Add(new NumberSetting("Minimum Delay",
                "The bot will send the message between the minimum delay and max delay.", 100, 0, 10000));
            Setting.Add(new NumberSetting("Maximum Delay",
                "The bot will send the message between the minimum delay and max delay.", 250, 1, 10000, 2));
            Setting.Add(new BoolSetting("Remove Special Characters", "Do you want to remove characters like \", `, . and so on?", false));
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadChat) return new PluginResponse(false, "'Load chat' must be enabled!");
            if (!Setting.At(1).Get<string>().Contains("%randomword%"))
                return new PluginResponse(false, "Second row doesn't contain %randomword% please check your plugin configuration!");
            return Setting.At(3).Get<int>() > Setting.At(4).Get<int>()
                ? new PluginResponse(false, "Minimum Delay is greater than Maximum Delay!")
                : new PluginResponse(true);
        }

        public override void OnStart()
        {
            RegisterTask(new Chat(Setting.At(0).Get<string>(), Setting.At(1).Get<string>(), Setting.At(2).Get<string>(),
                Setting.At(3).Get<int>(), Setting.At(4).Get<int>(), Setting.At(5).Get<bool>()));
        }
    }
}