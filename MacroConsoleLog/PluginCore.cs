using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Macro;
using OQ.MineBot.PluginBase.Base.Plugin;

namespace MacroConsoleLog
{
    [Plugin(1, "Macro Console Log", "Allows you to send message via the macro to the console of the bot.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) { }
    }

    public class ZerGo0ConsoleLogMacroComponent : IExternalMacroComponent
    {

        public ZerGo0ConsoleLogMacroComponent()
        {
            Category = MacroComponentCategory.Misc;
            Outputs = new IMacroOutputCollection(
                new KeyValuePair<string, ExternalMacroOutput>("success", new ExternalMacroOutput("Success", "This output gets called once the call finishes", true))
            );
            Variables = new IMacroVariableCollection(
                new KeyValuePair<string, ExternalMacroVariable>("variable_zergo0_console_message", new ExternalMacroVariable(typeof(string), "Message", "What message should we send to chat?", "my default message!"))
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
            return "This is a test macro component";
        }
        public override string GetInteractiveDescription()
        {
            var variableValue = GetVariable<string>("variable_zergo0_console_message");
            return
                string.IsNullOrWhiteSpace(variableValue) ? GetDescription()
                    : $"I will say {variableValue}.";
        }

        public override string Execute(IBotContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} " +
                              $"[{context.Player.GetUsername()}] {GetVariable<string>("variable_zergo0_console_message")}");
            return "success";
        }
    }
}
