using System.Threading;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Items;
using OQ.MineBot.PluginBase.Classes.Window.Containers.Subcontainers;

namespace ShieldPlugin.Tasks
{
    public class Equipment : ITask, IInventoryListener
    {
        private readonly bool _autoGear;
        private bool _busy;

        public Equipment(bool autoGear) {
            _autoGear = autoGear;
        }
        
        public override bool Exec() {
            return _autoGear && !_busy && !status.entity.isDead && !status.eating;
        }

        public override void Start() {
            OnInventoryChanged();
        }

        public void OnInventoryChanged() {
            if (!_autoGear) return;
            _busy = true;
            player.functions.OpenInventory();

            ThreadPool.QueueUserWorkItem(state => {
                Thread.Sleep(250);
                if (player.functions.EquipBest(EquipmentSlots.Head,
                    ItemsGlobal.itemHolder.helmets))
                    Thread.Sleep(250);
                if (player.functions.EquipBest(EquipmentSlots.Chest,
                    ItemsGlobal.itemHolder.chestplates))
                    Thread.Sleep(250);
                if (player.functions.EquipBest(EquipmentSlots.Pants,
                    ItemsGlobal.itemHolder.leggings))
                    Thread.Sleep(250);
                player.functions.EquipBest(EquipmentSlots.Boots,
                    ItemsGlobal.itemHolder.boots);
                Thread.Sleep(250);
                player.functions.CloseInventory();
                _busy = false;
            });
        }
        public void OnSlotChanged(ISlot slot) { }
        public void OnItemAdded(ISlot slot) { }
        public void OnItemRemoved(ISlot slot) { }
    }
}