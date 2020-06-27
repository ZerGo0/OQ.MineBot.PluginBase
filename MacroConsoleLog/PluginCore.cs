﻿using System;
using System.Collections.Generic;
using System.Globalization;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Macro;
using OQ.MineBot.PluginBase.Base.Plugin;

namespace MacroConsoleLog
{
    [Plugin(1, "Macro Console Log", "Allows you to send messages via the macro to the console of the bot.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
        }
    }

    public class ZerGo0ConsoleLogMacroComponent : IExternalMacroComponent
    {
        public ZerGo0ConsoleLogMacroComponent()
        {
            Category = MacroComponentCategory.Misc;
            Outputs = new IMacroOutputCollection(
                new KeyValuePair<string, ExternalMacroOutput>("done",
                    new ExternalMacroOutput("Done", "This output gets called once the call finishes", true))
            );
            Variables = new IMacroVariableCollection(
                new KeyValuePair<string, ExternalMacroVariable>("variable_zergo0_console_message",
                    new ExternalMacroVariable(typeof(string), "Message", "What message should we send to bot console?",
                        "Hello World!"))
            );
        }

        public override string GetName()
        {
            return "ZerGo0.ConsoleLog";
        }

        public override string GetInternalName()
        {
            return "zergo0_consolelog_component";
        }

        public override string GetDescription()
        {
            return "This component allows you to send messages via a macro to the console of the bot";
        }

        public override string GetInteractiveDescription()
        {
            var variableValue = GetVariable<string>("variable_zergo0_console_message");
            return
                string.IsNullOrWhiteSpace(variableValue)
                    ? GetDescription()
                    : $"I will output '{variableValue}' to the console of the bot.";
        }

        public override string Execute(IBotContext context)
        {
            if (!string.IsNullOrWhiteSpace(GetVariable<string>("variable_zergo0_console_message")))
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                                  $"[{context.Player.GetUsername()}] {GetVariable<string>("variable_zergo0_console_message")}");
            return "done";
        }
    }
}