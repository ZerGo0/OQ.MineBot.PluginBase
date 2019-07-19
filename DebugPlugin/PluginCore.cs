using System;
using System.Threading;
using System.Windows.Forms;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;

namespace DebugPlugin
{
    [Plugin(1338, "Debug Plugin", "Useful for plugin/macro devs.", "https://www.youtube.com/watch?v=pD2yrQJjqvw")]
    public class PluginCore : IRequestPlugin
    {
        public override void OnLoad(int version, int subversion, int buildversion) { }
        public override IRequestFunction[] GetFunctions()
        {
            return new IRequestFunction[] {
                new EnableFunction(),
            };
        }
    }

    public class EnableFunction : IRequestFunction
    {
        /// <summary>
        /// Name of this function.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "Enable";
        }

        public PluginResponse OnRequest(IPlayer player, bool macro = false)
        {
            OnRequest(player);

            return new PluginResponse(true);
        }

        /// <summary>
        /// Called once the user requested
        /// for the plugin to start on this player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public PluginResponse OnRequest(IPlayer player)
        {
            ShowForm(player);
            return new PluginResponse(true);
        }

        /// <summary>
        /// Called once the user requested
        /// for the plugin to start on these players.
        /// (This should not limit you to handle all player.
        /// You can choose to handle each seperartly)
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public PluginResponse OnRequest(IPlayer[] players)
        {
            foreach (var player in players)
                OnRequest(player);

            return new PluginResponse(true);
        }

        private void ShowForm(IPlayer player)
        {
            var chatForm = new DebugForm(player.status.username, player);

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
