using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Ipc.Exceptions;
using ECommons;
using ECommons.Automation;
using ECommons.Automation.NeoTaskManager;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Tasks.SameWorld;
public static unsafe class TaskChangeInstance
{
    public static readonly char[] InstanceNumbers = "\0".ToCharArray();

    public static readonly Dictionary<int, string> IconMapping = new Dictionary<int, string>
    {
        { 1, "" },
        { 2, "" },
        { 3, "" },
        { 4, "" },
        { 5, "" },
        { 6, "" },
        { 7, "" },
        { 8, "" },
        { 9, "" }
    };

    public static void Enqueue(int number) //TODO: Add a check for when chosen instance number is higher then available!
    {
        TaskManagerTask[]? tasks = new TaskManagerTask[]
        {
            new(InteractWithAetheryte),
            new(SelectTravel),
            new(() => SelectInstance(number), $"SelectInstance({number})"),
            new(() => !GenericHelpers.IsOccupied()),
            new(() =>
            {
                if(Plugin.C.InstanceSwitcherRepeat && number != S.InstanceHandler.GetInstance())
                {
                    Enqueue(number);
                }
            })
        };
        if (C.EnableFlydownInstance)
        {
            P.TaskManager.Enqueue(() =>
            {
                if (!Svc.Condition[ConditionFlag.InFlight])
                {
                    return true;
                }
                if (EzThrottler.Throttle("DropFlight", 1000))
                {
                    Chat.Instance.ExecuteCommand($"/generalaction {Svc.Data.GetExcelSheet<GeneralAction>().GetRow(23).Name}");
                }
                return false;
            });
        }
        P.TaskManager.EnqueueMulti(tasks);
        DuoLog.Warning($"Changing to instance: {number}");
    }

    public static bool SelectInstance(int num)
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.SelectString>(out var m) && m.IsAddonReady)
        {
            foreach (var x in m.Entries)
            {
                if (x.Text.Contains(InstanceNumbers[num]))
                {
                    if (EzThrottler.Throttle("SelectTravelToInstance"))
                    {
                        x.Select();
                        return true;
                    }
                    return false;
                }
            }
        }
        return false;
    }

    public static bool SelectTravel()
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.SelectString>(out var m) && m.IsAddonReady)
        {
            foreach (var x in m.Entries)
            {
                if (x.Text.ContainsAny("Travel to Instanced Area."))
                {
                    if (EzThrottler.Throttle("SelectTravelToInstancedArea"))
                    {
                        x.Select();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool InteractWithAetheryte()
    {
        if (Svc.Condition[ConditionFlag.OccupiedInQuestEvent]) return true;
        var aetheryte = GetAetheryte() ?? throw new NullReferenceException();
        if (aetheryte.IsTarget())
        {
            if (EzThrottler.Throttle("InteractWithAetheryte"))
            {
                TargetSystem.Instance()->InteractWithObject(aetheryte.Struct());
                return false;
            }
        }
        else
        {
            if (EzThrottler.Throttle("AetheryteSetTarget"))
            {
                Svc.Targets.Target = aetheryte;
                return false;
            }
        }
        return false;
    }

    public static IGameObject GetAetheryte()
    {
        foreach (var x in Svc.Objects)
        {
            if (x.ObjectKind == ObjectKind.Aetheryte && x.IsTargetable)
            {
                if (Vector3.Distance(x.Position, Player.Position) < 11f)
                {
                    return x;
                }
            }
        }
        return null;
    }
}