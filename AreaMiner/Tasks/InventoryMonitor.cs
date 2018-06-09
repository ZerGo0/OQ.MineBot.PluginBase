using System;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace AreaMiner.Tasks
{
    public class InventoryMonitor : ITask, ITickListener, IDeathListener
    {
        private readonly MacroSync _macro;
        private readonly string _macroName;

        public InventoryMonitor(string name, MacroSync macro)
        {
            _macro = macro;
            _macroName = name;
        }

        public void OnDeath()
        {
            throw new NotImplementedException();
        }

        public void OnTick()
        {
            _macro.Run(player, _macroName);
        }

        public override bool Exec()
        {
            return !status.entity.isDead && !status.eating && !_macro.IsMacroRunning() && inventory.IsFull();
        }
    }
}