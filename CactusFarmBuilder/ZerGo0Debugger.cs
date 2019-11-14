using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.PluginBase.Utility;

namespace CactusFarmBuilder
{
    public class ZerGo0Debugger : ITask, ITickListener
    {
        public static Dictionary<string, IPluginSetting> PluginSettings = new Dictionary<string, IPluginSetting>();
        private static IBotSettings _BOT_SETTINGS;
        private IBotContext _context;

        public ZerGo0Debugger(Dictionary<string, IPluginSetting> pluginSettings = null, IBotSettings botSettings = null)
        {
            if (pluginSettings != null) PluginSettings = pluginSettings;
            if (botSettings != null) _BOT_SETTINGS = botSettings;
            AppDomain.CurrentDomain.UnhandledException += TestHandler;
        }

        public static string DefaultOutputFilename
        {
            get
            {
                var localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                if (localPath == null) return null;
                var folder = new Uri(localPath).LocalPath + "\\logs";

                if (!Directory.Exists(folder))
                {
                    Console.WriteLine("Directory doesn't exist, creating the directory...");
                    Directory.CreateDirectory(folder);
                }

                var fileName =
                    $"log_{DateTime.Today:yyyy-MM-dd}_{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}.txt";
                return $"{folder}\\{fileName}";
            }
        }

        public Task OnTick()
        {
            return null;
        }

        public static void Debug(string playerName, string message)
        {
#if DEBUG
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                              $"[{playerName}] {message}");
#endif
        }

        public static void Info(string playerName, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                              $"[{playerName}] {message}");
        }

        private void TestHandler(object sender, UnhandledExceptionEventArgs args)
        {
            //Console.Clear();
            Error((Exception) args.ExceptionObject, _context);
        }

        public static void Error(Exception ex, IBotContext context = null, object classObj = null)
        {
            var nL = Environment.NewLine;
            var message = "";
            var discordMessage = "";

            for (var i = 0; i < 30; i++) message += Environment.NewLine;
            ;

#region General Stuff

            message += @"##################################################################################" + nL;
            message += @"#  _____           ___       ___      ___     _                                  #" + nL;
            message += @"# / _  / ___ _ __ / _ \___  / _ \    /   \___| |__  _   _  __ _  __ _  ___ _ __  #" + nL;
            message += @"# \// / / _ \ '__/ /_\/ _ \| | | |  / /\ / _ \ '_ \| | | |/ _` |/ _` |/ _ \ '__| #" + nL;
            message += @"#  / //\  __/ | / /_\\ (_) | |_| | / /_//  __/ |_) | |_| | (_| | (_| |  __/ |    #" + nL;
            message += @"# /____/\___|_| \____/\___/ \___(_)___,' \___|_.__/ \__,_|\__, |\__, |\___|_|    #" + nL;
            message += @"#                                                         |___/ |___/            #" + nL;
            message += @"##################################################################################" + nL;
            message += DebuggerLine();
            message += DebuggerLine("General Stuff");
            message += DebuggerLine("Time", DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            if (context != null) message += DebuggerLine("Botname", context?.Player.GetUsername());
            message += DebuggerLine("Plugin Name",
                typeof(PluginCore).GetCustomAttributesData()[0].ConstructorArguments[1].Value.ToString());
            message += DebuggerLine("Plugin Version", typeof(PluginCore).GetCustomAttributesData()[0]
                .ConstructorArguments[0].Value
                .ToString());
            message += DebuggerLine();

            discordMessage += "**General Stuff**" + nL;
            discordMessage += $"Time: {DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}" + nL;
            discordMessage += $"Botname: {context?.Player.GetUsername()}" + nL;
            discordMessage +=
                $"Plugin Name: {typeof(PluginCore).GetCustomAttributesData()[0].ConstructorArguments[1].Value}" + nL;
            discordMessage +=
                $"Plugin Version: {typeof(PluginCore).GetCustomAttributesData()[0].ConstructorArguments[0].Value}" + nL;
            discordMessage += "" + nL;

#endregion

#region Plugin Settings

            if (PluginSettings.Count > 0)
            {
                message += DebuggerLine("Plugin Settings");
                discordMessage += "**Plugin Settings**" + nL;

                foreach (var setting in PluginSettings.Where(setting => setting.Value != null))
                {
                    if (setting.Value.GetType() == typeof(DescriptionSetting) ||
                        setting.Value.GetType() == typeof(LinkSetting) ||
                        setting.Value.GetType() == typeof(GroupSetting)) continue;
                    if (string.IsNullOrEmpty(setting.Value.name)) setting.Value.name = "null";
                    if (string.IsNullOrEmpty(setting.Value.value.ToString())) setting.Value.value = "null";

                    if (context != null)
                        Debug(context.Player.GetUsername(), $"Type: {setting.Value.GetType()}");

                    message += DebuggerLine(setting.Value.name, setting.Value.value.ToString());
                    discordMessage += $"{setting.Value.name}: {setting.Value.value}" + nL;
                }

                message += DebuggerLine();
                discordMessage += "" + nL;
            }

#endregion

#region Bot Settings

            if (context != null || _BOT_SETTINGS != null)
            {
                var settings = _BOT_SETTINGS;

                if (context != null)
                {
                    settings = context.Settings;
                    _BOT_SETTINGS = settings;
                }

                if (settings != null)
                {
                    message += DebuggerLine("Bot Settings");
                    discordMessage += "**Bot Settings**" + nL;

                    message += DebuggerLine("Reconnect", settings.reconnect.ToString());
                    discordMessage += $"Reconnect: {settings.reconnect.ToString()}" + nL;

                    message += DebuggerLine("Max Reconnects", settings.maxReconnects.ToString());
                    discordMessage += $"Max Reconnects: {settings.maxReconnects}" + nL;

                    message += DebuggerLine("Load World", settings.loadWorld.ToString());
                    discordMessage += $"Load World: {settings.loadWorld}" + nL;

                    message += DebuggerLine("Shared World", settings.staticWorlds.ToString());
                    discordMessage += $"Shared World: {settings.staticWorlds}" + nL;

                    message += DebuggerLine("Load Chat", settings.loadChat.ToString());
                    discordMessage += $"Load Chat: {settings.loadChat}" + nL;

                    message += DebuggerLine("Load Inv", settings.loadInventory.ToString());
                    discordMessage += $"Load Inv: {settings.loadInventory}" + nL;

                    message += DebuggerLine("Load Entities", settings.loadEntities.ToString());
                    discordMessage += $"Load Entities: {settings.loadEntities}" + nL;

                    message += DebuggerLine("Load Players", settings.loadPlayers.ToString());
                    discordMessage += $"Load Players: {settings.loadPlayers}" + nL;

                    message += DebuggerLine("Load Mobs", settings.loadMobs.ToString());
                    discordMessage += $"Load Mobs: {settings.loadMobs}" + nL;

                    message += DebuggerLine();
                    discordMessage += "" + nL;
                }
            }

#endregion

            if (classObj != null)
            {
                var classType = classObj.GetType();

                const BindingFlags bindingFlags = BindingFlags.Instance |
                                                  BindingFlags.Static |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Public;

                if (classType.GetFields(bindingFlags).Length > 1)
                {
                    message += DebuggerLine($"Plugin Class Variables ({classType.Name})");
                    discordMessage += $"**Plugin Class Variables ({classType.Name})**" + nL;

                    foreach (var variable in classType.GetFields(bindingFlags))
                        if (variable.IsStatic)
                        {
                            if (variable.GetValue(null) != null)
                            {
                                message += DebuggerLine(variable.Name, variable.GetValue(null).ToString());
                                discordMessage += $"{variable.Name}: {variable.GetValue(null)}" + nL;
                            }
                            else
                            {
                                message += DebuggerLine(variable.Name, "null");
                                discordMessage += $"{variable.Name}: null" + nL;
                            }
                        }
                        else
                        {
                            if (variable.GetValue(classObj) != null)
                            {
                                message += DebuggerLine(variable.Name, variable.GetValue(classObj).ToString());
                                discordMessage += $"{variable.Name}: {variable.GetValue(classObj)}" + nL;
                            }
                            else
                            {
                                message += DebuggerLine(variable.Name, "null");
                                discordMessage += $"{variable.Name}: null" + nL;
                            }
                        }

                    message += DebuggerLine();
                    discordMessage += "" + nL;
                }
            }

            message += DebuggerLine("Error Stuff");
            discordMessage += "**Error Stuff**" + nL;

            message += DebuggerLine("Error in", ex.Source);
            discordMessage += $"Error in: {ex.Source}" + nL;

            message += DebuggerLine("Error message", ex.Message);
            discordMessage += $"Error message: {ex.Message}" + nL;

            if (ex.InnerException != null)
            {
                message += DebuggerLine("Inner Exception", ex.InnerException.Message);
                discordMessage += $"Inner Exception: {ex.InnerException.Message}" + nL;
            }

            var stackTrace = ex.StackTrace.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            var strackTraceFirst = true;
            foreach (var line in stackTrace)
                if (strackTraceFirst)
                {
                    strackTraceFirst = false;
                    message += DebuggerLine("StackTrace", line.TrimStart(' '));
                    var discordLine = $"```StackTrace: {line.TrimStart(' ')}" + nL;
                    if (discordMessage.Length + discordLine.Length < 2045) discordMessage += discordLine;
                }
                else
                {
                    message += DebuggerLine("", line.TrimStart(' '));
                    var discordLine = $"{line.TrimStart(' ')}" + nL;
                    if (discordMessage.Length + discordLine.Length < 2045) discordMessage += discordLine;
                }

            if (discordMessage.Length + 3 <= 2048) discordMessage += "```";

            message += @"##################################################################################";

            Console.WriteLine(message);

            DiscordHelper.SendMessage(642764008717549569, "ZerGo0Debugger Crash Report", "",
                discordMessage, true, 0);
        }

        private static string DebuggerLine(string name = "", string value = "")
        {
            var nL = Environment.NewLine;
            var padChar = ' ';
            var padWidthName = 18;
            var padWidthVal = 61;

            if ((name.Length > 0) & (value.Length > 0))
            {
                if ((name + ":").Length > padWidthName) padWidthVal = padWidthVal - (name + ":").Length + padWidthName;
                return "# " + (name + ":").PadRight(padWidthName, padChar) + value.PadRight(padWidthVal, padChar) +
                       "#" + nL;
            }

            if (name.Length > padWidthName) padWidthVal = padWidthVal - name.Length + padWidthName;

            return "# " + name.PadRight(padWidthName, padChar) + value.PadRight(padWidthVal, padChar) + "#" + nL;
        }

        public override bool Exec()
        {
            return true;
        }

        public override Task Start()
        {
            _context = Context;
            if (_context != null) Console.WriteLine(Context.Player.GetUsername());
            return null;
        }
    }
}