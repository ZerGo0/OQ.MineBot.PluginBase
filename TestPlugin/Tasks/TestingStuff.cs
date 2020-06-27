using System;
using System.Threading.Tasks;

using OQ.MineBot.GUI.Protocol.Movement.Maps;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Entity.Player;
using OQ.MineBot.PluginBase.Movement.Events;

using TestPlugin.Helpers;

namespace TestPlugin.Tasks
{
    public class TestingStuff : ITask, ITickListener
    {
        private static readonly MapOptions _MAP_OPTIONS_NOLOOK = new MapOptions
        {
            Look = false,
            Quality = SearchQuality.HIGHEST,
            Mine = false,
            Swim = true
        };

        private readonly string _username;

        private bool _currentlyPathing;
        private IPlayerEntity _currentTarget;
        private HelperFunctions _helperFunctions;
        private MoveResult _moveToTarget;

        public TestingStuff(string username)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _username = username;
        }

        public async Task OnTick()
        {
            throw new NullReferenceException();
            try
            {
                var target = Context.Entities.GetPlayer(_username);

                if (target == null) return;

                if (TargetChecker(_currentTarget))
                {
                    _currentlyPathing = false;
                    _moveToTarget = null;
                    _currentTarget = null;
                }
                else
                {
                    await FollowTarget(_currentTarget);
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                        $"PathStatus: {_moveToTarget?.Result.ToString()}");
                }
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(Context.Player.GetUsername(), e, this);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Test");
        }

        public override async Task Start()
        {
            _helperFunctions = new HelperFunctions(Context, Inventory);
        }

        public override async Task Stop()
        {
        }

        public override bool Exec()
        {
            return true;
        }

        private async Task FollowTarget(IPlayerEntity target)
        {
            if (!_currentlyPathing)
            {
                _currentlyPathing = true;
                _moveToTarget = await target.Follow(_MAP_OPTIONS_NOLOOK).Task;
                //                        ZerGo0Debugger.Debug(_botName, $"PathStatus: {_moveToMob?.Result.ToString()}");
                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Follow");
            }
            else
            {
                if (_moveToTarget != null)
                    switch (_moveToTarget.Result)
                    {
                        case MoveResultType.Cancelled:
                        case MoveResultType.Stuck:
                        case MoveResultType.PathNotFound:
                            _currentlyPathing = false;
                            _moveToTarget = null;
                            _currentTarget = null;
                            return;
                        case MoveResultType.Completed:
                            _currentlyPathing = false;
                            _moveToTarget = null;
                            break;
                    }
            }
        }

        private bool TargetChecker(IPlayerEntity target)
        {
            if (!target.IsDead() && !target.HasDespawned && !(target.GetHealth() < 1)) return false;

            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "Resetting Target");
            return true;
        }
    }
}