using System;
using System.Threading.Tasks;

using CactusFarmBuilder.Helpers;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;

namespace CactusFarmBuilder.Tasks
{
    public class VanillaMode : ITask, ITickListener
    {
        private readonly ushort[] _defaultBuldingBlocks;
        private readonly int _direction;
        private readonly int _maxLayers;

        private readonly int _tickDelay;
        private HelperFunctions _helperFunctions;
        private int _layerCount;
        private ILocation _startLoc;
        private bool _stopped;

        public VanillaMode(int speedmode, int maxlayers, int direction)
        {
            _maxLayers = maxlayers;
            _direction = direction;
            _tickDelay = speedmode;

            _defaultBuldingBlocks = BlocksGlobal.BUILDING_BLOCKS;
            BlocksGlobal.BUILDING_BLOCKS = new[] {(ushort) 12};
        }

        public async Task OnTick()
        {
            try
            {
                if (_startLoc == null) _startLoc = Context.Player.GetLocation();
                if (_helperFunctions == null) _helperFunctions = new HelperFunctions(Context, Inventory);
                
                if (_startLoc == null || _helperFunctions == null) return;

                if (!_helperFunctions.CheckItemCount(new ushort[] {12, 81, 287}, true)) return;

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

                if (!await FirstLayer())
                {
                    _stopped = true;
                    return;
                }

                ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st Layer END");

#endregion

                if (_layerCount >= _maxLayers) return;
                var tempPathTarget = CurrentLoc().Offset(1);
                if (!await _helperFunctions.GoToLocation(tempPathTarget, HelperFunctions.MAP_OPTIONS_BUILD))
                    if (!await _helperFunctions.GoToLocation(tempPathTarget, HelperFunctions.MAP_OPTIONS_BUILD))
                    {
                        _stopped = true;
                        _helperFunctions.Stopped = true;
                        return;
                    }

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
                tempPathTarget = CurrentLoc().Offset(1);
                await _helperFunctions.GoToLocation(tempPathTarget, HelperFunctions.MAP_OPTIONS_BUILD);
                if (!await _helperFunctions.GoToLocation(tempPathTarget, HelperFunctions.MAP_OPTIONS_BUILD))
                    if (!await _helperFunctions.GoToLocation(tempPathTarget, HelperFunctions.MAP_OPTIONS_BUILD))
                    {
                        _stopped = true;
                        _helperFunctions.Stopped = true;
                        return;
                    }
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
            //Note: 1st Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 0, 1), CurrentLoc().Offset(-1, 0, 1),
                CurrentLoc().Offset(-1, 0, -1), CurrentLoc().Offset(1, 0, -1)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st SandLayer DONE");

            if (_stopped) return false;

            //Note: 1st Cactuslayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 1, 1), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-1, 1, -1), CurrentLoc().Offset(1, 1, -1)
            }, 81, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st CactusLayer DONE");

            if (_stopped) return false;

            //Note: 1st StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(0, 1, 1), CurrentLoc().Offset(0, 1, -1)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st StringLayer DONE");

            if (_stopped) return false;

            _layerCount++;

            return true;
        }

        private async Task<bool> SecondLayer()
        {
            //Note: 2nd Sandlayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(0, 0, 1), CurrentLoc().Offset(0, 0, -1)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd SandLayer DONE");

            if (_stopped) return false;

            //Note: 2nd BOTTOM StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 0, 1), CurrentLoc().Offset(-1, 0, 1),
                CurrentLoc().Offset(-1, 0, -1), CurrentLoc().Offset(1, 0, -1)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BOTTOM StringLayer DONE");

            if (_stopped) return false;

            //Note: 2nd TOP StringLayer
            if (!await _helperFunctions.CreateLayer(new[]
            {
                CurrentLoc().Offset(1, 1, 1), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-1, 1, -1), CurrentLoc().Offset(1, 1, -1)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd TOP StringLayer DONE");

            if (_stopped) return false;

            //Note: 2nd BREAK BOTTOM StringLayer
            if (!await _helperFunctions.BreakBlocks(new[]
            {
                CurrentLoc().Offset(1, 0, 1), CurrentLoc().Offset(-1, 0, 1),
                CurrentLoc().Offset(-1, 0, -1), CurrentLoc().Offset(1, 0, -1)
            }, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BREAK BOTTOM StringLayer DONE");

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
            var nextStart = CurrentLoc().Offset(0, -1, -2);
            switch (_direction)
            {
                case 0:
                    nextStart = CurrentLoc().Offset(0, -1, -2);
                    break;
                case 1:
                    nextStart = CurrentLoc().Offset(+2, -1, 0);
                    break;
                case 2:
                    nextStart = CurrentLoc().Offset(0, -1, 2);
                    break;
                case 3:
                    nextStart = CurrentLoc().Offset(-2, -1, 0);
                    break;
            }

            return await _helperFunctions.GoToLocation(nextStart, HelperFunctions.MAP_OPTIONS_MINE);
        }

#endregion
    }
}