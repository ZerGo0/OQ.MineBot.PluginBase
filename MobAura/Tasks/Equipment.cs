using System;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Window;

namespace MobAuraPlugin.Tasks
{
    public class Equipment : ITask, IInventoryListener
    {
        private readonly bool _autoGear;
        private bool _busy;
        private bool _stopped;

        public Equipment(bool autoGear)
        {
            _autoGear = autoGear;
        }

        public async Task OnInventoryChanged()
        {
            try
            {
                if (!_autoGear) return;
                _busy = true;

                await Context.TickManager.Sleep(2);
                Context.Functions.OpenInventory();

                await Context.TickManager.Sleep(2);
                var sword = Inventory.FindBest(EquipmentType.Sword);
                sword?.PutOn();

                await Context.TickManager.Sleep(2);
                var helmet = Inventory.FindBest(EquipmentType.Helmet);
                helmet?.PutOn();

                await Context.TickManager.Sleep(2);
                var chestplate = Inventory.FindBest(EquipmentType.Chestplate);
                chestplate?.PutOn();

                await Context.TickManager.Sleep(2);
                var leggings = Inventory.FindBest(EquipmentType.Leggings);
                leggings?.PutOn();

                await Context.TickManager.Sleep(2);
                var boots = Inventory.FindBest(EquipmentType.Boots);
                boots?.PutOn();

                await Context.TickManager.Sleep(2);
                Context.Functions.CloseInventory();

                _busy = false;
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
                _stopped = true;
            }
        }

        public void OnSlotChanged(ISlot slot)
        {
        }

        public void OnItemAdded(ISlot slot)
        {
        }

        public void OnItemRemoved(ISlot slot)
        {
        }

        public override bool Exec()
        {
            return _autoGear && !_busy && !Context.Player.IsDead() && !Context.Player.State.Eating && !_stopped;
        }

        public override async Task Start()
        {
            try
            {
                await OnInventoryChanged();
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
                _stopped = true;
            }
        }
    }
}