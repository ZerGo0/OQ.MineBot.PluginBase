using System;
using System.Linq;
using System.Threading;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;

namespace RandomWordWinner.Tasks
{
    public class Chat : ITask
    {
        private static readonly Random Rnd = new Random();
        private readonly string _command;
        private readonly int _maxdelay;
        private readonly int _mindelay;
        private readonly string _pattern;
        private readonly string _trigger;

        public Chat(string trigger, string pattern, string command, int mindelay, int maxdelay)
        {
            _trigger = trigger;
            _pattern = pattern;
            _command = command;
            _mindelay = mindelay;
            _maxdelay = maxdelay;
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

            var strArray1 = _pattern.Split(Convert.ToChar(" "));
            var index1 = 0;
            var strArray2 = strArray1;
            foreach (var index2 in strArray2)
                if (index2 == "%randomword%")
                    break;
                else
                    index1++;

            var strArray3 = message.Parsed.Split(Convert.ToChar(" "));
            if (string.IsNullOrWhiteSpace(_command))
            {
                var randomdelay = Rnd.Next(_mindelay, _maxdelay);
                Thread.Sleep(randomdelay);
                player.functions.Chat(strArray3.ElementAt(index1));
            }
            else
            {
                var randomdelay = Rnd.Next(_mindelay, _maxdelay);
                Thread.Sleep(randomdelay);
                player.functions.Chat(_command + " " + strArray3.ElementAt(index1));
            }
        }
    }
}