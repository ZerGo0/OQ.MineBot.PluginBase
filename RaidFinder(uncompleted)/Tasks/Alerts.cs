using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OQ.MineBot.PluginBase.Classes.Blocks;

namespace RaidFinder.Tasks
{
    public class Alerts : ITask, ITickListener
    {
        private static readonly MapOptions Mo = new MapOptions()
        {
            Look = false,
            Quality = SearchQuality.HIGHEST,
            Mine = true
        };

        private static readonly ConcurrentDictionary<int, DateTime> Idlimit = new ConcurrentDictionary<int, DateTime>();

        private readonly ulong _discord;
        private readonly bool _local;
        private readonly DiscordHelper.Mode _mode;

        private readonly ushort[] _ids;
        private readonly List<ILocation> _oldLocations;
        private readonly List<ILocation> _oldLocationsTempList;
        private readonly List<ILocation> _doneLocations;
        private readonly List<ILocation> _checkLocations;
        private readonly List<int> _blocksinrange;
        private ILocation _oldLocation;

        private bool _firsttime;

        public Alerts(ulong discord, bool local, IEnumerable<string> customids)
        {
            _discord = discord;
            _local = local;

            _ids = (from customid in customids
                where !string.IsNullOrWhiteSpace(customid)
                select Convert.ToUInt16(customid)).ToArray();

            Console.WriteLine("Alerts");
            _oldLocations = new List<ILocation>();
            _oldLocationsTempList = new List<ILocation>();
            _doneLocations = new List<ILocation>();
            _checkLocations = new List<ILocation>();
            _blocksinrange = new List<int>();

            _firsttime = true;
        }

        public override bool Exec()
        {
            return !status.eating;
        }

        public void OnTick()
        {
            world.FindAsync(player, player.status.entity.location.ToLocation(), 64, 70, _ids,
                locarray =>
                {
                    var i = 0;
                    foreach (var loc in locarray)
                    {
                        if (_oldLocations.Contains(loc)) continue;
                        _oldLocations.Add(loc);
                        _oldLocationsTempList.Add(loc);
                    }

                    if (_oldLocations.Count <= 0) return;

                    foreach (var testLocation in _oldLocationsTempList)
                    {
                        var realDistance = testLocation.Distance(_oldLocation);

                        if (realDistance > 20 || _firsttime)
                        {
                            _checkLocations.Add(testLocation);
                            _firsttime = false;
                            _oldLocation = testLocation;
                            continue;
                        }

                        _doneLocations.Add(testLocation);

                        //foreach (var oldLocation in _oldLocations)
                        //{
                        //    var tempDistance = testLocation.Distance(oldLocation);
                        //
                        //    if (!(tempDistance < 21)) continue;
                        //    i++;
                        //    _doneLocations.Add(oldLocation);
                        //}


                        //if (_firsttime || !_doneLocations.Contains(testLocation))
                        //{
                        //    Console.WriteLine("Blocks in Range: " + i + " | Location: " + testLocation);
                        //    _oldLocation = testLocation;
                        //    _doneLocations.Add(testLocation);
                        //
                        //    //if (i == 1)
                        //    //    NotifyUser(ApplyVariables("We have found a block with no blocks around it!\n\nBlock Location: " + testLocation, null, null,
                        //    //        ""), 10, 5);
                        //    //else
                        //    //    NotifyUser(ApplyVariables("We have found a block with " + i + " blocks around it!\n\nBlock Location: " + testLocation, null, null,
                        //    //        ""), 10, 5);
                        //    //
                        //    //Thread.Sleep(3000);
                        //    _firsttime = false;
                        //}
                    }

                    foreach (var testLocation in _checkLocations)
                    {
                        if (_doneLocations.Contains(testLocation)) continue;
                        foreach (var oldLocation in _oldLocations)
                        {
                            var tempDistance = testLocation.Distance(oldLocation);
                            
                            if (!(tempDistance < 21)) continue;
                            i++;
                        }
                        Console.WriteLine("Blocks in Range: " + i + " | Location: " + testLocation);
                        _doneLocations.Add(testLocation);
                        i = 0;
                    }
                });
        }

        public override void Start()
        {
            player.events.onDeath += OnDeath;
        }

        public override void Stop()
        {
            player.events.onDeath -= OnDeath;
        }

        private void OnDeath(IPlayer player)
        {
            NotifyUser(ApplyVariables("Bot has died.", player.status.entity.location.ToLocation(), null, ""), 10, 5);
        }

        private void NotifyUser(IReadOnlyList<string> body, int priority, int id)
        {
            if (Idlimit.ContainsKey(id) && Idlimit[id].Subtract(DateTime.Now).TotalMilliseconds > 0) return;
            Idlimit.AddOrUpdate(id, i => DateTime.Now.AddSeconds(30 / priority),
                (i, time) => DateTime.Now.AddSeconds(30 / priority));

            if (_local)
                DiscordHelper.AlertMessage(_discord, "Raid Finder!", body[0], body[1], priority >= 5, priority, _mode);
            else DiscordHelper.SendMessage(_discord, "Raid Finder!", body[0], body[1], priority >= 5, _mode);
        }

        private string[] ApplyVariables(string text, ILocation playerLocation, ILocation targetLocation,
            string targetName)
        {
            var builder = new StringBuilder();
            builder.AppendLine("**" + text + "**");
            builder.AppendLine();
            if (!string.IsNullOrWhiteSpace(targetName)) builder.AppendLine("Name: *'" + targetName + "'*");
            if (targetLocation != null)
                builder.AppendLine("Location: *'" + targetLocation + "' (" +
                                   Math.Floor(Math.Abs(targetLocation.Distance(playerLocation))) + " blocks away)*");
            builder.AppendLine("Bot name: *'" + status.username + "'*");
            return new[] {text, builder.ToString()};
        }
    }
}