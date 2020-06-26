using System;
using System.Threading.Tasks;

using CactusFarmBuilder.Helpers;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;
using OQ.MineBot.Protocols.Classes.Base;

namespace CactusFarmBuilder.Tasks
{
    public class CropHopperCreative : ITask, ITickListener
    {
        private readonly ushort[] _defaultBuldingBlocks;
        private readonly int _direction;
        private readonly int _maxLayers;
        private readonly bool _ignoreFailSafe;

        private readonly int _tickDelay;
        private HelperFunctions _helperFunctions;
        private int _layerCount;
        private ILocation _startLoc;
        private bool _stopped;

        public CropHopperCreative(int speedmode, int maxlayers, int direction, bool ignoreFailSafe)
        {
            _maxLayers = maxlayers;
            _direction = direction;
            _tickDelay = speedmode;
            _ignoreFailSafe = ignoreFailSafe;

            var blockIdNullable = Blocks.Instance.GetId("sand");
            if (blockIdNullable == null) return;
            var blockId = blockIdNullable.Value;
            BlocksGlobal.BUILDING_BLOCKS = new[] { blockId };
        }

        public async Task OnTick()
        {
            try
            {
                _startLoc ??= Context.Player.GetLocation();
                _helperFunctions ??= new HelperFunctions(Context, Inventory);
                
                if (_startLoc == null || _helperFunctions == null) return;
                
                if (!_helperFunctions.CheckItemCount(new[] {"sand", "Cactus", "String"}, true)) return;

                if (_layerCount >= _maxLayers)
                {
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"LayerC: {_layerCount} | MaxL: {_maxLayers}");
                    if (!await _helperFunctions.BackToStart(_tickDelay, _startLoc))
                    {
                        _stopped = true;
                        return;
                    }

                    if (!await GoToNextStart()) return;

                    _layerCount = 0;
                    _startLoc = CurrentLoc();
                    return;
                }

#region 1st Layer

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st Layer START");


                await Inventory.Select(35);

                var loc1 = CurrentLoc().Offset(3, -1, 3);
                var loc2 = CurrentLoc().Offset(3, 0, 3);
                var loc3 = CurrentLoc().Offset(3, 1, 3);


                if (!await Context.World.PlaceAt(loc1))
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"!await Context.World.PlaceAt({loc1}) failed!");
                else
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                        $"!await Context.World.PlaceAt({loc1}) succeeded!");

                if (!await Context.World.PlaceAt(loc2))
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"!await Context.World.PlaceAt({loc2}) failed!");
                else
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                        $"!await Context.World.PlaceAt({loc2}) succeeded!");

                if (!await Context.World.PlaceAt(loc3))
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"!await Context.World.PlaceAt({loc3}) failed!");
                else
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                        $"!await Context.World.PlaceAt({loc3}) succeeded!");

                var locTest = new Location(-90, 64, 216);
                var test = Context.World.GetBlock(locTest).GetVisibleFaces();
                if (test != null)
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                        $"Loc: {locTest} | Visible Faces: {test.Length}");

                await Context.Player.LookAtSmooth(loc1);
                await Context.World.PlaceOn(loc1, 1);
                await Context.TickManager.Sleep(4);
                ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                    _helperFunctions.IsBlockId(loc1.Offset(1), 35)
                        ? $"!await Context.World.PlaceOn({loc1}, 1) failed!"
                        : $"!await Context.World.PlaceOn({loc1}, 1) succeeded!");


                await Context.Player.LookAtSmooth(loc2);
                await Context.World.PlaceOn(loc2, 1);
                await Context.TickManager.Sleep(4);
                ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                    _helperFunctions.IsBlockId(loc2.Offset(1), 35)
                        ? $"!await Context.World.PlaceOn({loc2}, 1) failed!"
                        : $"!await Context.World.PlaceOn({loc2}, 1) succeeded!");

                await Context.Player.LookAtSmooth(loc3);
                await Context.World.PlaceOn(loc3, 1);
                await Context.TickManager.Sleep(4);
                ZerGo0Debugger.Debug(Context.Player.GetUsername(),
                    _helperFunctions.IsBlockId(loc3.Offset(1), 35)
                        ? $"!await Context.World.PlaceOn({loc3}, 1) failed!"
                        : $"!await Context.World.PlaceOn({loc3}, 1) succeeded!");

                await Context.TickManager.Sleep(999999);

                if (!await FirstLayer())
                {
                    _stopped = true;
                    return;
                }

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st Layer END");

#endregion

                if (_layerCount >= _maxLayers) return;
                await _helperFunctions.GoToLocation(CurrentLoc().Offset(1), HelperFunctions.MAP_OPTIONS_BUILD);

#region 2nd Layer

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd Layer START");

                if (!await SecondLayer())
                {
                    _stopped = true;
                    return;
                }

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd Layer END");

#endregion

                if (_layerCount >= _maxLayers) return;
                await _helperFunctions.GoToLocation(CurrentLoc().Offset(1), HelperFunctions.MAP_OPTIONS_BUILD);
            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
                _stopped = true;
                if (_helperFunctions != null) _helperFunctions.Stopped = true;
            }
        }

        public override Task Stop()
        {
            ZerGo0Debugger.Info(Context.Player.GetUsername(), "CactusFarmBuilder STOPPED!");

            _stopped = true;
            _helperFunctions.Stopped = true;

            BlocksGlobal.BUILDING_BLOCKS = _defaultBuldingBlocks;
            return null;
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_stopped;
        }


#region Layers

        private async Task<bool> FirstLayer()
        {
            //Note: 1st BOTTOM StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 0, 2), CurrentLoc().Offset(-3, 0, 3),
                CurrentLoc().Offset(-3, 0, -3), CurrentLoc().Offset(3, 0, -3)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st BOTTOM StringLayer DONE");

            if (_stopped) return false;

            //Note: 1st TOP StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(3, 1, 3), CurrentLoc().Offset(-3, 1, 3),
                CurrentLoc().Offset(-3, 1, -3), CurrentLoc().Offset(3, 1, -3)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st TOP StringLayer DONE");

            if (_stopped) return false;

            //Note: 1st BREAK BOTTOM StringLayer
            if (!await _helperFunctions.BreakBlocks(new[]
            {
                CurrentLoc().Offset(3, 0, 3), CurrentLoc().Offset(-3, 0, 3),
                CurrentLoc().Offset(-3, 0, -3), CurrentLoc().Offset(3, 0, -3)
            }, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st BREAK BOTTOM StringLayer DONE");

            if (_stopped) return false;

            //Note: 1st OUTER Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(3, 0, 0), CurrentLoc().Offset(3, 0, 2),
                CurrentLoc().Offset(2, 0, 3), CurrentLoc().Offset(0, 0, 3),
                CurrentLoc().Offset(-2, 0, 3), CurrentLoc().Offset(-3, 0, 2),
                CurrentLoc().Offset(-3, 0, 0), CurrentLoc().Offset(-3, 0, -2),
                CurrentLoc().Offset(-2, 0, -3), CurrentLoc().Offset(0, 0, -3),
                CurrentLoc().Offset(2, 0, -3), CurrentLoc().Offset(3, 0, -2)
            }, "sand", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER SandLayer DONE");

            if (_stopped) return false;

            //Note: 1st OUTER Cactuslayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(3, 1, 0), CurrentLoc().Offset(3, 1, 2),
                CurrentLoc().Offset(2, 1, 3), CurrentLoc().Offset(0, 1, 3),
                CurrentLoc().Offset(-2, 1, 3), CurrentLoc().Offset(-3, 1, 2),
                CurrentLoc().Offset(-3, 1, 0), CurrentLoc().Offset(-3, 1, -2),
                CurrentLoc().Offset(-2, 1, -3), CurrentLoc().Offset(0, 1, -3),
                CurrentLoc().Offset(2, 1, -3), CurrentLoc().Offset(3, 1, -2)
            }, "Cactus", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER Cactuslayer DONE");

            if (_stopped) return false;

            //Note: 1st OUTER Stringlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(3, 1, 1), CurrentLoc().Offset(1, 1, 3),
                CurrentLoc().Offset(-1, 1, 3), CurrentLoc().Offset(-3, 1, 1),
                CurrentLoc().Offset(-3, 1, -1), CurrentLoc().Offset(-1, 1, -3),
                CurrentLoc().Offset(1, 1, -3), CurrentLoc().Offset(3, 1, -1)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER Stringlayer DONE");

            if (_stopped) return false;

            //Note: 1st INNER Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 0, 1), CurrentLoc().Offset(1, 0, 2),
                CurrentLoc().Offset(-1, 0, 2), CurrentLoc().Offset(-2, 0, 1),
                CurrentLoc().Offset(-2, 0, -1), CurrentLoc().Offset(-1, 0, -2),
                CurrentLoc().Offset(1, 0, -2), CurrentLoc().Offset(2, 0, -1)
            }, "sand", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER Sandlayer DONE");

            if (_stopped) return false;

            //Note: 1st INNER Cactuslayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 1, 1), CurrentLoc().Offset(1, 1, 2),
                CurrentLoc().Offset(-1, 1, 2), CurrentLoc().Offset(-2, 1, 1),
                CurrentLoc().Offset(-2, 1, -1), CurrentLoc().Offset(-1, 1, -2),
                CurrentLoc().Offset(1, 1, -2), CurrentLoc().Offset(2, 1, -1)
            }, "Cactus", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER Cactuslayer DONE");

            if (_stopped) return false;

            //Note: 1st INNER Stringlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 1, 0), CurrentLoc().Offset(1, 1, 1),
                CurrentLoc().Offset(0, 1, 2), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-2, 1, 0), CurrentLoc().Offset(-1, 1, -1),
                CurrentLoc().Offset(0, 1, -2), CurrentLoc().Offset(1, 1, -1)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER Stringlayer DONE");

            if (_stopped) return false;

            //Note: 1st INNER INNER Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 0, 0), CurrentLoc().Offset(0, 0, 1),
                CurrentLoc().Offset(-1, 0, 0), CurrentLoc().Offset(0, 0, -1)
            }, "sand", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER INNER Sandlayer DONE");

            if (_stopped) return false;

            await Context.TickManager.Sleep(999999);

            _layerCount++;

            return true;
        }

        private async Task<bool> SecondLayer()
        {
            //Note: 2nd BOTTOM StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 0, 2), CurrentLoc().Offset(-2, 0, 2),
                CurrentLoc().Offset(-2, 0, -2), CurrentLoc().Offset(2, 0, -2)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BOTTOM StringLayer DONE");

            if (_stopped) return false;

            //Note: 2nd TOP StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 1, 2), CurrentLoc().Offset(-2, 1, 2),
                CurrentLoc().Offset(-2, 1, -2), CurrentLoc().Offset(2, 1, -2)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd TOP StringLayer DONE");

            if (_stopped) return false;

            //Note: 2nd BREAK BOTTOM StringLayer
            if (!await _helperFunctions.BreakBlocks(new[]
            {
                CurrentLoc().Offset(2, 0, 2), CurrentLoc().Offset(-2, 0, 2),
                CurrentLoc().Offset(-2, 0, -2), CurrentLoc().Offset(2, 0, -2)
            }, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BREAK BOTTOM StringLayer DONE");

            if (_stopped) return false;

            //Note: 2nd OUTER Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 0, 1), CurrentLoc().Offset(1, 0, 2),
                CurrentLoc().Offset(-1, 0, 2), CurrentLoc().Offset(-2, 0, 1),
                CurrentLoc().Offset(-2, 0, -1), CurrentLoc().Offset(-1, 0, -2),
                CurrentLoc().Offset(1, 0, -2), CurrentLoc().Offset(2, 0, -1)
            }, "sand", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd OUTER SandLayer DONE");

            if (_stopped) return false;

            //Note: 2nd INNER Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 0, 0), CurrentLoc().Offset(0, 0, 1),
                CurrentLoc().Offset(-1, 0, 0), CurrentLoc().Offset(0, 0, -1)
            }, "sand", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd INNER SandLayer DONE");

            if (_stopped) return false;

            //Note: 2nd OUTER Cactuslayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 1, 1), CurrentLoc().Offset(1, 1, 2),
                CurrentLoc().Offset(-1, 1, 2), CurrentLoc().Offset(-2, 1, 1),
                CurrentLoc().Offset(-2, 1, -1), CurrentLoc().Offset(-1, 1, -2),
                CurrentLoc().Offset(1, 1, -2), CurrentLoc().Offset(2, 1, -1)
            }, "Cactus", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd OUTER CactusLayer DONE");

            if (_stopped) return false;

            //Note: 2nd INNER StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(2, 1, 0), CurrentLoc().Offset(1, 1, 1),
                CurrentLoc().Offset(0, 1, 2), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-2, 1, 0), CurrentLoc().Offset(-1, 1, -1),
                CurrentLoc().Offset(0, 1, -2), CurrentLoc().Offset(1, 1, -1)
            }, "String", _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd INNER StringLayer DONE");

            if (_stopped) return false;

            _layerCount++;

            return true;
        }

#endregion

#region Helper Functions

        private ILocation CurrentLoc()
        {
            return Context.Player.GetLocation();
        }

        private async Task<bool> GoToNextStart()
        {
            var nextStart = CurrentLoc().Offset(0, -1, -4);
            switch (_direction)
            {
                case 0:
                    nextStart = CurrentLoc().Offset(0, -1, -4);
                    break;
                case 1:
                    nextStart = CurrentLoc().Offset(+4, -1, 0);
                    break;
                case 4:
                    nextStart = CurrentLoc().Offset(0, -1, 4);
                    break;
                case 3:
                    nextStart = CurrentLoc().Offset(-4, -1, 0);
                    break;
            }

            return await _helperFunctions.GoToLocation(nextStart, HelperFunctions.MAP_OPTIONS_MINE);
        }

#endregion
    }
}