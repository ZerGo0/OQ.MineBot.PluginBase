using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        private readonly string _keywords;
        private readonly DiscordHelper.Mode _mode;

        private string _chattext;

        public Alerts(ulong discord, string keywordses, DiscordHelper.Mode mode)
        {
            _discord = discord;
            _keywords = keywordses;
            _mode = mode;
        }

        public override bool Exec()
        {
            return true;
        }

        public override void Start()
        {
            player.events.onChat += OnChat;
            
            Console.Clear();
            var request = (HttpWebRequest)WebRequest.Create("https://de.namemc.com/search?q=" + player.status.username);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            ServicePointManager
                    .ServerCertificateValidationCallback += 
                (sender, cert, chain, sslPolicyErrors) => true;
            
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            
            // Get the stream containing content returned by the server. 
            // The using block ensures the stream is automatically closed. 
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                
                var match = Regex.Match(responseFromServer, @"(?<=charset\=).*");
                // Display the content.  
                Console.WriteLine(match);
            }
            
            // Close the response.  
            response.Close();
        }

        public override void Stop()
        {
            player.events.onChat -= OnChat;
        }

        private void OnChat(IPlayer player, IChat message, byte position)
        {
            _chattext = message.GetText();

            // Split keywords here
            var splittedkeywords = _keywords.Split(',');

            // check for all keywords
            foreach (var keyword in splittedkeywords)
            {
                if (!_chattext.Contains(keyword)) continue;

                //Get everything after the keyword
                var textmessage =
                    message.GetText().Substring(_chattext.LastIndexOf(keyword, StringComparison.Ordinal) +
                                             keyword.Length);

                //To remove the space after the keyword
                var textmessagetrimmed = textmessage.TrimStart(' ');

                //Checking if message is empty so that we don't pass empty messages to discord
                if (textmessagetrimmed.Length > 0) NotifyUser(ApplyVariables(textmessagetrimmed), 10, 0);
            }
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