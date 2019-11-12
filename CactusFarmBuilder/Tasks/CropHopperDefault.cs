using System;
using System.Threading.Tasks;

using CactusFarmBuilder.Helpers;

using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Blocks;

namespace CactusFarmBuilder.Tasks
{
    public class CropHopperDefault : ITask, ITickListener
    {
        private bool _stopped;
        private int _layerCount;
        private ILocation _startLoc;
        
        private readonly int _tickDelay;
        private readonly int _maxLayers;
        private readonly int _direction;
        private readonly ushort[] _defaultBuldingBlocks;

        public CropHopperDefault(int speedmode, int maxlayers, int direction)
        {
            _maxLayers = maxlayers;
            _direction = direction;
            _tickDelay = speedmode;

            _defaultBuldingBlocks = BlocksGlobal.BUILDING_BLOCKS;
            BlocksGlobal.BUILDING_BLOCKS = new[] {(ushort)12};
        }

        public override Task Start()
        {
            _startLoc = Context.Player.GetLocation();
            
            return null;
        }

        public override Task Stop()
        {
            ZerGo0Debugger.Info(Context.Player.GetUsername(), "CactusFarmBuilder STOPPED!");
            
            _stopped = true;
            BlocksGlobal.BUILDING_BLOCKS = _defaultBuldingBlocks;

            return null;
        }

        public override bool Exec()
        {
            return !Context.Player.IsDead() && !Context.Player.State.Eating && !_stopped;
        }

        public async Task OnTick()
        {
            try
            {
                if (!HelperFunctions.CheckItemCount(Context, Inventory, new ushort[]{12, 81, 287}, true)) return;
                
                if (_layerCount >= _maxLayers)
                {
                    ZerGo0Debugger.Debug(Context.Player.GetUsername(), $"LayerC: {_layerCount} | MaxL: {_maxLayers}");
                    if (!await HelperFunctions.BackToStart(Context, Inventory, _tickDelay, _startLoc))
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
                await HelperFunctions.GoToLocation(Context, CurrentLoc().Offset(1), HelperFunctions.MAP_OPTIONS_BUILD);

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
                await HelperFunctions.GoToLocation(Context, CurrentLoc().Offset(1), HelperFunctions.MAP_OPTIONS_BUILD);

            }
            catch (Exception e)
            {
                ZerGo0Debugger.Error(e, Context, this);
            }
        }
        
        

#region Layers
        
        private async Task<bool> FirstLayer()
        {
            //Note: 1st OUTER Sandlayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 0, 0), CurrentLoc().Offset(2, 0, 2), 
                CurrentLoc().Offset(0, 0, 2), CurrentLoc().Offset(-2, 0, 2),
                CurrentLoc().Offset(-2, 0, 0), CurrentLoc().Offset(-2, 0, -2),
                CurrentLoc().Offset(0, 0, -2), CurrentLoc().Offset(2, 0, -2)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER SandLayer DONE");

            if (_stopped) return false;
            
            //Note: 1st INNER Sandlayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(1, 0, 1), CurrentLoc().Offset(-1, 0, 1),
                CurrentLoc().Offset(-1, 0, -1), CurrentLoc().Offset(1, 0, -1)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER SandLayer DONE");

            if (_stopped) return false;
            
            //Note: 1st OUTER Cactuslayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 1, 0), CurrentLoc().Offset(2, 1, 2), 
                CurrentLoc().Offset(0, 1, 2), CurrentLoc().Offset(-2, 1, 2),
                CurrentLoc().Offset(-2, 1, 0), CurrentLoc().Offset(-2, 1, -2),
                CurrentLoc().Offset(0, 1, -2), CurrentLoc().Offset(2, 1, -2)
            }, 81, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER CactusLayer DONE");

            if (_stopped) return false;
                
            //Note: 1st OUTER StringLayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 1, 1), CurrentLoc().Offset(1, 1, 2),
                CurrentLoc().Offset(-1, 1, 2), CurrentLoc().Offset(-2, 1, 1),
                CurrentLoc().Offset(-2, 1, -1), CurrentLoc().Offset(-1, 1, -2),
                CurrentLoc().Offset(1, 1, -2), CurrentLoc().Offset(2, 1, -1)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st OUTER StringLayer DONE");
            
            if (_stopped) return false;
                
            //Note: 1st INNER Cactuslayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(1, 1, 1), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-1, 1, -1), CurrentLoc().Offset(1, 1, -1)
            }, 81, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st INNER CactusLayer DONE");

            if (_stopped) return false;
                
            //Note: 1st INNER StringLayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(1, 1, 0), CurrentLoc().Offset(0, 1, 1),
                CurrentLoc().Offset(-1, 1, 0), CurrentLoc().Offset(0, 1, -1)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "1st Outed StringLayer DONE");
            
            if (_stopped) return false;

            _layerCount++;

            return true;
        }

        private async Task<bool> SecondLayer()
        {
            //Note: 2nd BOTTOM StringLayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 0, 2), CurrentLoc().Offset(-2, 0, 2), 
                CurrentLoc().Offset(-2, 0, -2),CurrentLoc().Offset(2, 0, -2)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BOTTOM StringLayer DONE");
            
            if (_stopped) return false;
            
            //Note: 2nd TOP StringLayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 1, 2), CurrentLoc().Offset(-2, 1, 2), 
                CurrentLoc().Offset(-2, 1, -2),CurrentLoc().Offset(2, 1, -2)
            }, 287, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd TOP StringLayer DONE");
            
            if (_stopped) return false;
                
            //Note: 2nd BREAK BOTTOM StringLayer
            if (!await HelperFunctions.BreakBlocks(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 0, 2), CurrentLoc().Offset(-2, 0, 2), 
                CurrentLoc().Offset(-2, 0, -2),CurrentLoc().Offset(2, 0, -2)
            }, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd BREAK BOTTOM StringLayer DONE");

            if (_stopped) return false;
            
            //Note: 2nd OUTER Sandlayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 0, 1), CurrentLoc().Offset(1, 0, 2),
                CurrentLoc().Offset(-1, 0, 2), CurrentLoc().Offset(-2, 0, 1),
                CurrentLoc().Offset(-2, 0, -1), CurrentLoc().Offset(-1, 0, -2),
                CurrentLoc().Offset(1, 0, -2), CurrentLoc().Offset(2, 0, -1)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd OUTER SandLayer DONE");

            if (_stopped) return false;
            
            //Note: 2nd INNER Sandlayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(1, 0, 0), CurrentLoc().Offset(0, 0, 1),
                CurrentLoc().Offset(-1, 0, 0), CurrentLoc().Offset(0, 0, -1)
            }, 12, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd INNER SandLayer DONE");

            if (_stopped) return false;
                
            //Note: 2nd OUTER Cactuslayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 1, 1), CurrentLoc().Offset(1, 1, 2),
                CurrentLoc().Offset(-1, 1, 2), CurrentLoc().Offset(-2, 1, 1),
                CurrentLoc().Offset(-2, 1, -1), CurrentLoc().Offset(-1, 1, -2),
                CurrentLoc().Offset(1, 1, -2), CurrentLoc().Offset(2, 1, -1)
            }, 81, _tickDelay)) return false;
            ZerGo0Debugger.Debug(Context.Player.GetUsername(), "2nd OUTER CactusLayer DONE");
            
            if (_stopped) return false;
            
            //Note: 2nd INNER StringLayer
            if (!await HelperFunctions.CreateLayer(Context, Inventory, new[]
            {
                CurrentLoc().Offset(2, 1, 0), CurrentLoc().Offset(1, 1, 1), 
                CurrentLoc().Offset(0, 1, 2), CurrentLoc().Offset(-1, 1, 1),
                CurrentLoc().Offset(-2, 1, 0), CurrentLoc().Offset(-1, 1, -1),
                CurrentLoc().Offset(0, 1, -2), CurrentLoc().Offset(1, 1, -1)
            }, 287, _tickDelay)) return false;
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

            return await HelperFunctions.GoToLocation(Context, nextStart, HelperFunctions.MAP_OPTIONS_MINE);
        }

#endregion
    }
}