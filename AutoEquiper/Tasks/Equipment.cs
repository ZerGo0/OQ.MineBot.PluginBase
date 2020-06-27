using System;
using System.Threading.Tasks;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Window;

namespace AutoEquiper.Tasks
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
                if (_busy) return;
                _busy = true;

                if (_autoGear)
                {
                    Context.Functions.OpenInventory();

                    await Context.TickManager.Sleep(2);
                    var helmet = Inventory.FindBest(EquipmentType.Helmet);
                    if (helmet != null) await helmet.PutOn();

                    await Context.TickManager.Sleep(2);
                    var chestplate = Inventory.FindBest(EquipmentType.Chestplate);
                    if (chestplate != null) await chestplate.PutOn();

                    await Context.TickManager.Sleep(2);
                    var leggings = Inventory.FindBest(EquipmentType.Leggings);
                    if (leggings != null) await leggings.PutOn();

                    await Context.TickManager.Sleep(2);
                    var boots = Inventory.FindBest(EquipmentType.Boots);
                    if (boots != null) await boots.PutOn();

                    await Context.TickManager.Sleep(2);
                    Context.Functions.CloseInventory();
                }

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
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Test");
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