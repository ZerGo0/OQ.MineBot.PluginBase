using OQ.MineBot.PluginBase.Base.Plugin.Tasks;

namespace OreMinerPlugin.Tasks
{
    public class InventoryMonitor : ITask, ITickListener
    {
        private readonly MacroSync _macro;
        private readonly string _macroName;

        public InventoryMonitor(string name, MacroSync macro)
        {
            _macro = macro;
            _macroName = name;
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