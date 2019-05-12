using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Utility;

namespace RandomWordWinner.Tasks
{
    public class Chat : ITask
    {
        private static readonly Random Rnd = new Random();
        private readonly string _command;
        private readonly int _maxdelay;
        private readonly int _mindelay;
        private string _pattern;
        private readonly string _trigger;
        private readonly bool _removeSpecialChars;

        public Chat(string trigger, string pattern, string command, int mindelay, int maxdelay, bool removeSpecialChars)
        {
            _trigger = trigger;
            _pattern = pattern;
            _command = command;
            _mindelay = mindelay;
            _maxdelay = maxdelay;
            _removeSpecialChars = removeSpecialChars;
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
            if (!message.GetText().Contains(_trigger)) return;

            if (_removeSpecialChars) _pattern = RemoveSpecialCharacters(_pattern);
            
            var patternArray = _pattern.Split(Convert.ToChar(" "));
            var index1 = 0;
            var patternArraySaved = patternArray;
            
            foreach (var index2 in patternArraySaved)
            {
                if (_removeSpecialChars)
                {
                    if (index2 == "randomword")
                        break;
                }
                else
                {
                    if (index2 == "%randomword%")
                        break;
                }
                
                if (patternArraySaved.Length < index1) index1++;
                else
                {
                    DiscordHelper.__api_hook_ale("[Random Word Winner] Plugin Settings are wrong!", 1);
                    return;
                }
            }
            
            

            var chatMsgSplit = message.GetText().Split(Convert.ToChar(" "));
            if (string.IsNullOrWhiteSpace(_command))
            {
                var randomdelay = Rnd.Next(_mindelay, _maxdelay);
                Thread.Sleep(randomdelay);
                player.functions.Chat(_removeSpecialChars
                    ? RemoveSpecialCharacters(chatMsgSplit.ElementAt(index1))
                    : chatMsgSplit.ElementAt(index1));
            }
            else
            {
                var randomdelay = Rnd.Next(_mindelay, _maxdelay);
                Thread.Sleep(randomdelay);
                player.functions.Chat(_removeSpecialChars
                    ? _command + " " + RemoveSpecialCharacters(chatMsgSplit.ElementAt(index1))
                    : _command + " " + chatMsgSplit.ElementAt(index1));
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^0-9A-Za-z ,]", "", RegexOptions.Compiled);
        }
    }
}