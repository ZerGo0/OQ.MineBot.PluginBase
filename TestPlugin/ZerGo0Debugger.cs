using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using OQ.MineBot.PluginBase.Base;

namespace TestPlugin
{
    public class ZerGo0Debugger
    {
        public static Dictionary<string, IPluginSetting> PluginSettings;

        public ZerGo0Debugger(Dictionary<string, IPluginSetting> pluginSettings)
        {
            PluginSettings = pluginSettings;
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

        public static void Debug(string playerName, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                              $"[{playerName}] {message}");
        }

        public static void Error(string playerName, Exception ex, object classObj = null)
        {
            var message = "";

            for (var i = 0; i < 30; i++) message += Environment.NewLine;
            ;


            message += @"##################################################################################";
            message += @"#  _____           ___       ___      ___     _                                  #";
            message += @"# / _  / ___ _ __ / _ \___  / _ \    /   \___| |__  _   _  __ _  __ _  ___ _ __  #";
            message += @"# \// / / _ \ '__/ /_\/ _ \| | | |  / /\ / _ \ '_ \| | | |/ _` |/ _` |/ _ \ '__| #";
            message += @"#  / //\  __/ | / /_\\ (_) | |_| | / /_//  __/ |_) | |_| | (_| | (_| |  __/ |    #";
            message += @"# /____/\___|_| \____/\___/ \___(_)___,' \___|_.__/ \__,_|\__, |\__, |\___|_|    #";
            message += @"#                                                         |___/ |___/            #";
            message += @"##################################################################################";
            message += DebuggerLine();
            message += DebuggerLine("General Stuff");
            message += DebuggerLine("Time", DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            message += DebuggerLine("Botname", playerName);
            message += DebuggerLine("Plugin Name",
                typeof(PluginCore).GetCustomAttributesData()[0].ConstructorArguments[1].Value.ToString());
            message += DebuggerLine("Plugin Version", typeof(PluginCore).GetCustomAttributesData()[0]
                .ConstructorArguments[0].Value
                .ToString());
            message += DebuggerLine();

            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.Static |
                               BindingFlags.NonPublic |
                               BindingFlags.Public;

            if (PluginSettings.Count > 0)
            {
                message += DebuggerLine("Plugin Settings");

                foreach (var setting in PluginSettings)
                    message += DebuggerLine(setting.Value.name, setting.Value.value.ToString());

                message += DebuggerLine();
            }

            if (classObj != null)
            {
                var classType = classObj.GetType();

                if (classType.GetFields(bindingFlags).Length > 1)
                {
                    message += DebuggerLine("Plugin Class Variables");

                    foreach (var variable in classType.GetFields(bindingFlags))
                        if (variable.IsStatic)
                        {
                            if (variable.GetValue(null) != null)
                                message += DebuggerLine(variable.Name, variable.GetValue(null).ToString());
                            else
                                message += DebuggerLine(variable.Name, "null");
                        }
                        else
                        {
                            if (variable.GetValue(classObj) != null)
                                message += DebuggerLine(variable.Name, variable.GetValue(classObj).ToString());
                            else
                                message += DebuggerLine(variable.Name, "null");
                        }

                    message += DebuggerLine();
                }
            }

            message += DebuggerLine("Error Stuff");
            message += DebuggerLine("Error in", ex.Source);
            message += DebuggerLine("Error message", ex.Message);
            if (ex.InnerException != null) message += DebuggerLine("Inner Exception", ex.InnerException.Message);

            var stackTrace = ex.StackTrace.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            var strackTraceFirst = true;
            foreach (var line in stackTrace)
                if (strackTraceFirst)
                {
                    strackTraceFirst = false;
                    message += DebuggerLine("StackTrace", line.TrimStart(' '));
                }
                else
                {
                    message += DebuggerLine("", line.TrimStart(' '));
                }

            message += @"##################################################################################";

            Console.WriteLine(message);
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
    }
}