using System;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace AreaFiller.Tasks
{
    public class InventoryMonitor : ITask, ITickListener
    {
        private readonly MacroSync _macro;
        private readonly string _macroName;
        private readonly MacroSync _fillermacro;
        private readonly string _fillername;
        private readonly int _fillerId;

        public InventoryMonitor(string fillerid, string name, MacroSync macro, string fillername, MacroSync fillerMacro)
        {
            _fillerId = int.Parse(fillerid);
            _macro = macro;
            _fillermacro = fillerMacro;
            _macroName = name;
            _fillername = fillername;
        }

        public void OnTick()
        {
            if (inventory.IsFull())
                _macro.Run(player, _macroName);

            //Console.WriteLine(player.status.username + ": Has no building blocks!");

            if (inventory.FindId(_fillerId) < 1)
                _fillermacro.Run(player, _fillername);
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_macro.IsMacroRunning() && !_fillermacro.IsMacroRunning() && (inventory.IsFull() || (inventory.FindId(_fillerId) < 1));
        }
    }
}