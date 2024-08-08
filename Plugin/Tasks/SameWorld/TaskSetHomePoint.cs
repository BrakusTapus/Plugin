using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace Plugin.Tasks.SameWorld;
public static unsafe class TaskSetHomePoint
{

    public static bool SelectSetHomePoint()
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.SelectString>(out var m) && m.IsAddonReady)
        {
            foreach (var x in m.Entries)
            {
                if (x.Text.ContainsAny("Set Home Point."))
                {
                    if (EzThrottler.Throttle("SetHomePoint"))
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
