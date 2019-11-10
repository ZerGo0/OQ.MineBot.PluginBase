using System;
using System.Threading.Tasks;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Entity;
using OQ.MineBot.PluginBase.Classes.Entity.Mob;
using OQ.MineBot.PluginBase.Classes.Window;
using OQ.MineBot.PluginBase.Movement.Events;

namespace MobAuraPlugin.Tasks
{
    public class Attack : ITask, ITickListener
    {
        private static readonly MapOptions MAP_OPTIONS_NOLOOK = new MapOptions
        {
            Look = true,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Swim = true
        };
        
        private static readonly Random _RND = new Random();

        private int _hitTicks;

        private readonly Mode _mode;
        private readonly int _cps;
        private readonly int _ms;
        private readonly bool _autoWeapon;

        private bool _currentlyPathing;
        private MoveResult _moveToMob;
        private IMobEntity _currentTarget;
        private string _botName;

        public Attack(Mode mode, int cps, int ms, bool autoWeapon)
        {
            _mode = mode;
            _cps = cps;
            _ms = ms;
            _autoWeapon = autoWeapon;
        }

        public override async Task Start()
        {
            _botName = Context.Player.GetUsername();
            
            Context.Events.onPlayerUpdate += EventsOnOnPlayerUpdate;
        }

        public override async Task Stop()
        {
            _currentTarget = null;
            Context.Events.onPlayerUpdate -= EventsOnOnPlayerUpdate;
        }

        private void EventsOnOnPlayerUpdate(IStopToken cancel)
        {
            if (!Exec()) return;
            
            try
            {
                AttackClosestTarget(_currentTarget);
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
            }
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating;
        }

        public async Task OnTick()
        {
            try
            {
                if (_currentTarget == null) _currentTarget = TargetSelector();

                if (_currentTarget == null) return;
                
                if (TargetChecker(_currentTarget))
                {
                    _currentlyPathing = false;
                    _moveToMob = null;
                    _currentTarget = null;
                }
                else
                {
                    if (_mode == Mode.MovingPassive || _mode == Mode.MovingAggresive)
                    {
                        await FollowTarget(_currentTarget);
                        ZerGo0Debugger.Debug(_botName, $"PathStatus: {_moveToMob?.Result.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
            }
        }

#region Functions

        private IMobEntity TargetSelector()
        {
            if (_mode == Mode.Passive) return Context.Entities.GetClosestMob(CurrentLocation(), MobType.All, 
                mob => !mob.IsDead() && !mob.HasDespawned && mob.GetHealth() > 0 && 
                       mob.HasLineOfSight() && 
                       CurrentLocation().Distance(mob.Position.ToLocation()) > 3);
            
            return Context.Entities.GetClosestMob(CurrentLocation(), MobType.All, 
                mob => !mob.IsDead() && !mob.HasDespawned && mob.GetHealth() > 0);
        }
        
        private async Task AttackClosestTarget(IMobEntity target)
        {
            if (target == null || CurrentLocation().Distance(target.Position.ToLocation()) > 3 ||
                TargetChecker(target)) target = TargetSelector();
            
            if (target == null) return;
            
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"Targetting: {target.MobType} | {target.Position}");

            if (_mode == Mode.MovingPassive && !VisibilityCheck(target)) return;
                
            if (_mode == Mode.Passive && !VisibilityCheck(target))
            {
                _currentTarget = null;
                return;
            }

            if (_autoWeapon)
            {
                Context.Functions.OpenInventory();
                
                await Context.TickManager.Sleep(1);
                var sword = Inventory.FindBest(EquipmentType.Sword);
                sword?.PutOn();
                
                await Context.TickManager.Sleep(1);
                Context.Functions.CloseInventory();
            }
            
            await Context.Player.LookAt(target.Position.Offset(0.8));
//                Context.Functions.LookAtSmooth(target.Position.Offset(0.8), LookSpeed.medium);

            // 1 hit tick is about 50 ms.
            _hitTicks++;
            var ms = _hitTicks * 50;

            if (ms < 1000 / _cps) return;
            
            _hitTicks = 0; //Hitting, reset tick counter.
            if (_RND.Next(1, 101) < _ms)
                Context.Functions.PerformSwing(); //Miss.
            else 
                target.Attack(); //Hit.
        }
        
        private bool VisibilityCheck(IEntity target)
        {
            if (target == null) return false;
            
            if (_mode == Mode.MovingPassive || _mode == Mode.Passive)
            {
                if (!target.HasLineOfSight() || Context.Player.GetLocation().Distance(target.Position.ToLocation()) > 4) return false;
            }

            return true;
        }
        
        private bool TargetChecker(IMobEntity target)
        {
            if (target.IsDead() || target.HasDespawned || target.GetHealth() < 1 ||
                _mode == Mode.Passive && CurrentLocation().Distance(target.Position.ToLocation()) > 3)
            {
                ZerGo0Debugger.Debug(_botName, "Resetting Target");
                return true;
            }

            return false;
        }

        private async Task FollowTarget(IMobEntity target)
        {
            if (!_currentlyPathing)
            {
                _currentlyPathing = true;
                _moveToMob = await target.Follow(MAP_OPTIONS_NOLOOK).Task;
//                        ZerGo0Debugger.Debug(_botName, $"PathStatus: {_moveToMob?.Result.ToString()}");
                ZerGo0Debugger.Debug(_botName, "Follow");
            }
            else
            {
                if (_moveToMob != null)
                {
                    if (_moveToMob.Result == MoveResultType.Cancelled ||
                        _moveToMob.Result == MoveResultType.Stuck ||
                        _moveToMob.Result == MoveResultType.PathNotFound)
                    {
                        _currentlyPathing = false;
                        _moveToMob = null;
                        _currentTarget = null;
                        return;
                    }

                    if (_moveToMob.Result == MoveResultType.Completed)
                    {
                        _currentlyPathing = false;
                        _moveToMob = null;
                    }
                }
            }
        }

#endregion

#region Helper Functions

        private ILocation CurrentLocation()
        {
            return Context.Player.GetLocation();
        }

#endregion
    }
}