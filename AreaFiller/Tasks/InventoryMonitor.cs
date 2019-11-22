using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace AreaFiller.Tasks
{
    public class InventoryMonitor : ITask, ITickListener
    {
        private readonly int _fillerId;
        private readonly MacroSync _fillermacro;
        private readonly string _fillername;

        public InventoryMonitor(string fillerid, string fillername, MacroSync fillerMacro)
        {
            _fillerId = int.Parse(fillerid);
            _fillermacro = fillerMacro;
            _fillername = fillername;
        }

        public async Task OnTick()
        {
            if (Inventory.GetAmountOfItem((ushort) _fillerId) < 1)
            {
                ZerGo0Debugger.Info(Context.Player.GetUsername(), "Is out of building blocks!");
                _fillermacro.Run(Context, _fillername);
            }

            await Context.TickManager.Sleep(1);
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_fillermacro.IsMacroRunning() &&
                   Inventory.GetAmountOfItem((ushort) _fillerId) < 1;
        }
    }
}