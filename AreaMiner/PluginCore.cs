using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AreaMiner.Tasks;
using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base;
using OQ.MineBot.PluginBase.Base.Plugin;
using OQ.MineBot.PluginBase.Bot;
using OQ.MineBot.PluginBase.Classes;
using OQ.MineBot.PluginBase.Classes.Base;
using OQ.MineBot.Protocols.Classes.Base;

namespace AreaMiner
{
    [Plugin(1, "Area miner", "Mines the area that is selected by the user.")]
    public class PluginCore : IStartPlugin
    {
        private static readonly ShareManager Shares = new ShareManager();

        public override void OnLoad(int version, int subversion, int buildversion)
        {
            Setting = new IPluginSetting[7];
            Setting[0] = new StringSetting("Start x y z", "", "0 0 0");
            Setting[1] = new StringSetting("End x y z", "", "0 0 0");
            Setting[2] = new StringSetting("Macro on inventory full",
                "Starts the macro when the bots inventory is full.", "");
            Setting[3] = new ComboSetting("Speed mode", null, new[] {"Accurate", "Fast"}, 0);
            Setting[4] = new StringSetting("Ignore ids", "What blocks should be ignored.", "");
            Setting[5] = new ComboSetting("Path mode", null, new[] {"Advanced (mining & building)", "Basic"}, 0);
            Setting[6] = new StringSetting("Only mine certain ids", "The bot will only mine these id(s).", "");
        }

        public override PluginResponse OnEnable(IBotSettings botSettings)
        {
            if (!botSettings.loadWorld) return new PluginResponse(false, "'Load world' must be enabled.");
            if (string.IsNullOrWhiteSpace(Setting[0].Get<string>()) ||
                string.IsNullOrWhiteSpace(Setting[1].Get<string>()))
                return new PluginResponse(false, "No coordinates have been entered.");
            if (!Setting[0].Get<string>().Contains(' ') || !Setting[1].Get<string>().Contains(' '))
                return new PluginResponse(false, "Invalid coordinates (does not contain ' ').");
            var startSplit = Setting[0].Get<string>().Split(' ');
            var endSplit = Setting[1].Get<string>().Split(' ');
            if (startSplit.Length != 3 || endSplit.Length != 3)
                return new PluginResponse(false, "Invalid coordinates (must be x y z).");

            var blockString = Setting[6].Get<string>();
            var inputids = Regex.Match(blockString, "^[0-9 ]*$");
            return !inputids.Success ? new PluginResponse(false, "Block ID's are wrong! Only numbers and spaces are allowed!") : new PluginResponse(true);
        }

        public override void OnDisable()
        {
            Shares?.Clear();
        }

        public override void OnStart()
        {
            // Split the ids.
            var ids = Setting[4].Get<string>().Split(' ');
            var ignoreIdList = new List<ushort>();
            foreach (var i in ids)
            {
                if (!ushort.TryParse(i, out var id))
                    continue;
                ignoreIdList.Add(id);
            }

            var customids = Setting[6].Get<string>().Split(' ');
            var customblockslist = new List<ushort>();
            foreach (var i in customids)
            {
                if (!ushort.TryParse(i, out var customid))
                    continue;
                customblockslist.Add(customid);
            }

            var startSplit = Setting[0].Get<string>().Split(' ');
            var endSplit = Setting[1].Get<string>().Split(' ');

            var macro = new MacroSync();

            RegisterTask(new InventoryMonitor(Setting[2].Get<string>(), macro));
            RegisterTask(new Path(Shares,
                new Location(int.Parse(startSplit[0]), int.Parse(startSplit[1]), int.Parse(startSplit[2])),
                new Location(int.Parse(endSplit[0]), int.Parse(endSplit[1]), int.Parse(endSplit[2])),
                (PathMode) Setting[5].Get<int>(), macro
            ));
            RegisterTask(new Mine(Shares, (Mode) Setting[3].Get<int>(), (PathMode) Setting[5].Get<int>(),
                ignoreIdList.ToArray(), customblockslist.ToArray(), macro));
        }
    }
}

public class ShareManager
{
    private readonly ConcurrentDictionary<IPlayer, SharedRadiusState> _zones =
        new ConcurrentDictionary<IPlayer, SharedRadiusState>();

    private IRadius _radius;

    public void SetArea(IRadius radius)
    {
        _radius = radius;
    }

    public void Add(IPlayer player, IRadius radius)
    {
        _zones.TryAdd(player, new SharedRadiusState(radius));
        Calculate();
    }

    public void Clear()
    {
        _zones.Clear();
    }

    public IRadius Get(IPlayer player)
    {
        return !_zones.TryGetValue(player, out var state) ? null : state.Radius;
    }

    public void RegisterReached(IPlayer player)
    {
        if (!_zones.TryGetValue(player, out var state)) return;
        state.Reached = true;
    }

    private void Calculate()
    {
        var zones = _zones.ToArray();
        var count = zones.Length;

        int x, z;
        int l;
        if (_radius.xSize > _radius.zSize)
        {
            x = (int) Math.Ceiling(_radius.xSize / (double) count);
            l = _radius.xSize;
            z = _radius.zSize;
            for (var i = 0; i < zones.Length; i++)
                zones[i].Value.Update(new Location(_radius.start.x + x * i, 0, _radius.start.z),
                    new Location(_radius.start.x + x * (i + 1) + (i == zones.Length - 1 ? l - x * (i + 1) : 0), 0,
                        _radius.start.z + z));
        }
        else
        {
            x = _radius.xSize;
            z = (int) Math.Ceiling(_radius.zSize / (double) count);
            l = _radius.zSize;
            for (var i = 0; i < zones.Length; i++)
                zones[i].Value.Update(new Location(_radius.start.x, 0, _radius.start.z + z * i),
                    new Location(_radius.start.x + x, 0,
                        _radius.start.z + z * (i + 1) + (i == zones.Length - 1 ? l - z * (i + 1) : 0)));
        }
    }

    public bool AllReached()
    {
        var temp = _zones.ToArray();
        return temp.All(t => t.Value.Reached);
    }

    public bool MeReached(IPlayer player)
    {
        return _zones.TryGetValue(player, out var state) && state.Reached;
    }
}

public class SharedRadiusState
{
    public readonly IRadius Radius;
    public bool Reached;

    public SharedRadiusState(IRadius radius)
    {
        Radius = radius;
    }

    public void Update(ILocation loc, ILocation loc2)
    {
        Reached = false;
        Radius.UpdateHorizontal(loc, loc2);
    }
}

public class MacroSync
{
    private Task _macroTask;

    public bool IsMacroRunning()
    {
        //Check if there is an instance of the task.
        if (_macroTask == null) return false;
        //Check completion state.
        return !_macroTask.IsCompleted && !_macroTask.IsCanceled && !_macroTask.IsFaulted;
    }

    public void Run(IPlayer player, string name)
    {
        _macroTask = player.functions.StartMacro(name);
    }
}