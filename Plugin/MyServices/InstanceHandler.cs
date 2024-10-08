﻿using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.Configuration;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Plugin;
using Plugin.Tasks.SameWorld;
using Plugin.Utility.Helpers;

namespace MyServices;

public unsafe class InstanceHandler
{
    private InstanceHandler()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "SelectString", OnPostUpdate);
        var gv = CSFramework.Instance()->GameVersionString;
        if (gv != null && gv != P.EzConfigs.GameVersion)
        {
            PluginLog.Information($"New game version detected, new {gv}, old {P.EzConfigs.GameVersion}");
            P.EzConfigs.GameVersion = gv;
            P.EzConfigs.PublicInstances = [];
        }
    }

    public bool CanChangeInstance()
    {
        return P.EzConfigs.ShowInstanceSwitcher && !Utils.IsDisallowedToUseAethernet() && !P.TaskManager.IsBusy && !GenericHelpers.IsOccupied() && S.InstanceHandler.GetInstance() != 0 && TaskChangeInstance.GetAetheryte() != null;
    }

    private void OnPostUpdate(AddonEvent type, AddonArgs args)
    {
        if (
            UIState.Instance()->PublicInstance.IsInstancedArea()
            && Svc.Targets.Target?.ObjectKind == ObjectKind.Aetheryte
            && Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
            && GenericHelpers.TryGetAddonMaster<AddonMaster.SelectString>(out var m)
            && m.IsAddonReady
            && (m.Entries.Any(x => x.Text.ContainsAny("Travel to Instanced Area.")) || m.Text == Svc.Data.GetExcelSheet<Addon>().GetRow(2090).Text.ExtractText())
            )
        {
            var inst = *P.Memory.MaxInstances;
            if (inst < 2 || inst > 9)
            {
                if (EzThrottler.Throttle("InstanceWarning", 5000)) PluginLog.Warning($"Instance count is wrong, received {inst}, please report to developer");
            }
            else
            {
                if (P.EzConfigs.PublicInstances.TryGetValue(Player.Territory, out var value) && value == inst)
                {
                    //
                }
                else
                {
                    P.EzConfigs.PublicInstances[Player.Territory] = inst;
                    EzConfig.Save();
                }
            }
        }
    }

    public int GetInstance()
    {
        return (int)UIState.Instance()->PublicInstance.InstanceId;
    }

    public bool InstancesInitizliaed(out int maxInstances)
    {
        return P.EzConfigs.PublicInstances.TryGetValue(Player.Territory, out maxInstances);
    }

    public void Dispose()
    {
        Svc.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, "SelectString", OnPostUpdate);
    }
}
