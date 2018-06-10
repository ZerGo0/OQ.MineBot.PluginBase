using System;
using System.Linq;
using System.Text.RegularExpressions;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;

namespace CaptchaBreakerPlugin.Tasks
{
    public class Chat : ITask
    {
        private readonly string _trigger;
        private readonly string _pattern;
        private readonly string _command;
        private readonly bool _specialchars;
        private string _specialcharstring;

        public Chat(string trigger, string pattern, string command, bool specialchars)
        {
            _trigger = trigger;
            _pattern = pattern;
            _command = command;
            _specialchars = specialchars;
        }

        public override bool Exec()
        {
            return true;
        }

        public override void Start()
        {
            player.events.onChat += OnChat;
        }

        public override void Stop()
        {
            player.events.onChat -= OnChat;
        }

        private void OnChat(IPlayer player, IChat message, byte position)
        {
            if (!message.Parsed.Contains(_trigger))
                return;
            Console.WriteLine("[DCaptchaBreaker] - Captcha request DETECTED. Solving.");
            var strArray1 = _pattern.Split(Convert.ToChar(" "));
            var index1 = 0;
            var strArray2 = strArray1;
            for (var index2 = 0; index2 < strArray2.Length; ++index2)
                if (strArray2[index2] == "%captcha%")
                    break;
                else index1++;

            Console.WriteLine("[DCaptchaBreaker] - Captcha found at position : " + index1 + " .");
            var strArray3 = message.Parsed.Split(Convert.ToChar(" "));

            _specialcharstring = strArray3.ElementAt(index1);

            if (_specialchars)
            {
                var specialCharCheck = new Regex("[*'\",_&#^@]");
                _specialcharstring = specialCharCheck.Replace(strArray3.ElementAt(index1), string.Empty);
            }

            Console.WriteLine("[DCaptchaBreaker] - Captcha found, it should be : " + _specialcharstring +
                              " . Sending captcha to server..");
            if (string.IsNullOrWhiteSpace(_command))
            {
                Console.WriteLine("[DCaptchaBreaker] - No captcha command found. Sending.");
                player.functions.Chat(_specialcharstring);
            }
            else
            {
                Console.WriteLine("[DCaptchaBreaker] - Captcha command found. Sending captcha command + captcha.");
                player.functions.Chat(_command + " " + _specialcharstring);
            }
        }
    }
}