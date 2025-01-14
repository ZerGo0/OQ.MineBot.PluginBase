﻿using System;
using System.Collections.Generic;
using System.Globalization;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Macro;
using OQ.MineBot.PluginBase.Base.Plugin;

namespace MacroComment
{
    [Plugin(1, "Macro Comment", "This component displays the text that you enter as the description.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
        }
    }

    public class ZerGo0CommentMacroComponent : IExternalMacroComponent
    {
        public ZerGo0CommentMacroComponent()
        {
            Category = MacroComponentCategory.Misc;
            Outputs = new IMacroOutputCollection(
                new KeyValuePair<string, ExternalMacroOutput>("done",
                    new ExternalMacroOutput("Done", "This output gets called once the call finishes", true))
            );
            Variables = new IMacroVariableCollection(
                new KeyValuePair<string, ExternalMacroVariable>("variable_zergo0_comment_text",
                    new ExternalMacroVariable(typeof(string), "Comment Text", "Which text should we display as the description of this component?",
                        "")),
            new KeyValuePair<string, ExternalMacroVariable>("variable_zergo0_comment_output_console_bool",
                    new ExternalMacroVariable(typeof(bool), "Output to console?", "Should we also output the text to the console of the bot?",
                        false))
            );
        }

        public override string GetName()
        {
            return "ZerGo0.Comment";
        }

        public override string GetInternalName()
        {
            return "zergo0_comment_component";
        }

        public override string GetDescription()
        {
            return "This component displays the text that you enter as the description, it also optionally allows you to output that text to the console.";
        }

        public override string GetInteractiveDescription()
        {
            var variableValue = GetVariable<string>("variable_zergo0_comment_text");
            return string.IsNullOrWhiteSpace(variableValue) ? GetDescription() : variableValue;
        }

        public override string Execute(IBotContext context)
        {
            if (!GetVariable<bool>("variable_zergo0_comment_output_console_bool")) return "done";
            var commentText = GetVariable<string>("variable_zergo0_comment_text");

            if (!string.IsNullOrWhiteSpace(commentText))
            {
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                                  $"[{context.Player.GetUsername()}] {commentText}");
            }

            return "done";
        }
    }
}