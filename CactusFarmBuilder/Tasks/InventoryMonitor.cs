using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace CactusFarmBuilder.Tasks
{
    public class InventoryMonitor : ITask, ITickListener
    {
        private readonly int _fillerId;
        private readonly MacroSync _fillermacro;
        private readonly string _fillername;
        private readonly MacroSync _macro;
        private readonly string _macroName;

        public InventoryMonitor(MacroSync macro)
        {
            //_fillerId = int.Parse(fillerid);
            //_macro = macro;
            //_fillermacro = fillerMacro;
            //_macroName = name;
            //_fillername = fillername;
        }

        public void OnTick()
        {
            //if (inventory.IsFull())
            //{
            //    Console.WriteLine("[Bot: " + player.status.username + "] Inv FULL!");
            //}

            //if (inventory.FindId(_fillerId) < 1)
            //    _fillermacro.Run(player, _fillername);
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating;
        }
    }
}