﻿using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI;
using Plugin.Schedulers;
using Plugin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Tasks.SameWorld;
public static class TaskApproachAndInteractWithApartmentEntrance
{
    public static void Enqueue()
    {
        P.TaskManager.Enqueue(() => Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51], "WaitUntilBetweenAreas");
        P.TaskManager.Enqueue(Utils.WaitForScreen);
        P.TaskManager.Enqueue(TargetApartmentEntrance);
        P.TaskManager.Enqueue(WorldChange.LockOn);
        P.TaskManager.Enqueue(WorldChange.EnableAutomove);
        P.TaskManager.Enqueue(() => Vector3.Distance(ECommons.GameHelpers.Player.Object.Position, Svc.Targets.Target.Position) < 3.5f, "ReachApartment");
        P.TaskManager.Enqueue(WorldChange.DisableAutomove);
        P.TaskManager.Enqueue(InteractWithApartmentEntrance);
    }

    public static bool TargetApartmentEntrance()
    {
        //2007402	apartment building entrance	0	apartment building entrances	0	1	1	0	0
        foreach(var x in Svc.Objects.OrderBy(x => Vector3.Distance(x.Position, ECommons.GameHelpers.Player.Object.Position)))
        {
            if(x.DataId == 2007402)
            {
                if(!x.IsTarget())
                {
                    if(EzThrottler.Throttle("TargetApartment"))
                    {
                        Svc.Targets.SetTarget(x);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static unsafe bool InteractWithApartmentEntrance()
    {
        if(ECommons.GameHelpers.Player.IsAnimationLocked) return false;
        if(Svc.Targets.Target?.ObjectKind == ObjectKind.EventObj && Svc.Targets.Target?.DataId == 2007402)
        {
            if(EzThrottler.Throttle("InteractWithApartment", 5000))
            {
                TargetSystem.Instance()->InteractWithObject(Svc.Targets.Target.Struct(), false);
                return true;
            }
        }
        return false;
    }

    public static unsafe bool GoToMyApartment()
    {
        return Utils.TrySelectSpecificEntry(Lang.GoToMyApartment, () => EzThrottler.Throttle("SelectStringApartment"));
    }
}
