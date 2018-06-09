using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.PluginBase.Utility;

namespace DiscordCommander.Tasks
{
    public class Alerts : ITask
    {
        private static readonly ConcurrentDictionary<int, DateTime> Idlimit = new ConcurrentDictionary<int, DateTime>();
        private readonly ulong _discord;
        private readonly string _keyword;
        private readonly DiscordHelper.Mode _mode;

        private string _chattext;

        public Alerts(ulong discord, string keyword, DiscordHelper.Mode mode)
        {
            _discord = discord;
            _keyword = keyword;
            _mode = mode;
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
            if (textmessagetrimmed.Length > 0) NotifyUser(ApplyVariables(textmessagetrimmed), 10, 0);
        }

        private void NotifyUser(IReadOnlyList<string> body, int priority, int id)
        {
            if (Idlimit.ContainsKey(id) && Idlimit[id].Subtract(DateTime.Now).TotalMilliseconds > 0) return;
            Idlimit.AddOrUpdate(id, i => DateTime.Now.AddSeconds(30 / priority),
                (i, time) => DateTime.Now.AddSeconds(30 / priority));
            DiscordHelper.SendMessage(_discord, "Discord Commander!", body[0], body[1], priority >= 5, _mode);
        }

        private string[] ApplyVariables(string text)
        {
            var builder = new StringBuilder();
            builder.AppendLine("**" + text + "**");
            builder.AppendLine();
            builder.AppendLine("Bot name: *'" + status.username + "'*");
            return new[] {text, builder.ToString()};
        }
    }
}