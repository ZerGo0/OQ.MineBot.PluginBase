using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Macro;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;

namespace MacroConsoleLog
{
    [Plugin(1, "Macro Console Log", "[BETA] Builds a cactus farm for you.")]
    public class PluginCore : IStartPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) { }
        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            return base.OnEnable(botSettings);
        }
    }

    public class TestMacroComponent : IExternalMacroComponent
    {

        public TestMacroComponent()
        {
            this.Category = MacroComponentCategory.Misc;
            this.Outputs = new IMacroOutputCollection(
                new KeyValuePair<string, ExternalMacroOutput>("success", new ExternalMacroOutput("Success", "This output gets called once the call finishes", true)),
                new KeyValuePair<string, ExternalMacroOutput>("output_internal_name", new ExternalMacroOutput("Error", "This output will never get called", false))
            );
            this.Variables = new IMacroVariableCollection(
                new KeyValuePair<string, ExternalMacroVariable>("variable_internal_name1", new ExternalMacroVariable(typeof(string), "Message", "What message should we send to chat?", "my default message!"))
            );
        }

        public override string GetName()
        {
            return "Test macro component";
        }

        public override string GetInternalName()
        {
            return "ex:test_macro_component";
        }

        public override string GetDescription()
        {
            return "This is a test macro component";
        }
        public override string GetInteractiveDescription()
        {
            var variableValue = GetVariable<string>("variable_internal_name1");
            return
                string.IsNullOrWhiteSpace(variableValue) ? GetDescription()
                    : $"I will say {variableValue}.";
        }

        public override string Execute(IBotContext Context)
        {
            Context.Functions.Chat("My message: " + GetVariable<string>("variable_internal_name1"));
            return "success"; // or return "output_internal_name"
        }
    }
}
