using System.Threading;
using System.Windows.Forms;

using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;

namespace DebugPlugin
{
    [Plugin(1337, "Debug Plugin", "Useful for plugin/macro devs.", "https://www.youtube.com/watch?v=pD2yrQJjqvw")]
    public class PluginCore : IRequestPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion)
        {
        }

        public override IRequestFunction[] GetFunctions()
        {
            return new IRequestFunction[]
            {
                new EnableFunction()
            };
        }
    }

    public class EnableFunction : IRequestFunction
    {
        /// <summary>
        ///     Name of this function.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "Enable";
        }

        public PluginResponse OnRequest(IBotContext context, bool macro = false)
        {
            ShowForm(context);

            return new PluginResponse(true);
        }

        public PluginResponse OnRequest(IBotContext[] contexts)
        {
            foreach (var context in contexts)
                OnRequest(context);

            return new PluginResponse(true);
        }

        private void ShowForm(IBotContext context)
        {
            var chatForm = new DebugForm(context.Player.GetUsername(), context);

            var thread = new Thread(ApplicationRunProc);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start(chatForm);
        }

        private static void ApplicationRunProc(object state)
        {
            Application.Run(state as Form);
        }
    }
}