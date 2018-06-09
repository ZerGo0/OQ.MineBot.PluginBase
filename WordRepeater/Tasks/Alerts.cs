using System;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;

namespace WordRepeater.Tasks
{
    public class Alerts : ITask
    {
        private readonly string _command;
        private readonly string _keyword;

        private string _chattext;

        public Alerts(string keyword, string command)
        {
            _keyword = keyword;
            _command = command;
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
            _chattext = message.Parsed;

            if (!_chattext.Contains(_keyword)) return;
            //Get everything after the keyword
            var textmessage =
                message.Parsed.Substring(_chattext.LastIndexOf(_keyword, StringComparison.Ordinal) + _keyword.Length);

            //To remove the space after the keyword
            var textmessagetrimmed = textmessage.TrimStart(' ');

            //Checking if message is empty so that we don't pass empty messages to discord
            if (textmessagetrimmed.Length <= 0) return;
            if (_command.Length > 0)
                actions.Chat(_command + textmessagetrimmed);
            else
                actions.Chat(textmessagetrimmed);
        }
    }
}