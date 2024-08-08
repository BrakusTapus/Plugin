using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation.UIInput;
using ECommons.Throttlers;
using Plugin.Utility.Helpers;

namespace Plugin.Features;

internal class Selectstring
{

    internal Selectstring()
    {
        TaskManager = new();
    }

    protected TaskManager TaskManager;

    public void Enable()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, addonName: "SelectString", OnSelectString);
    }

    public void Disable()
    {
        Svc.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, addonName: "SelectString", OnSelectString);
    }

    private void OnSelectString(AddonEvent eventType, AddonArgs addonInfo)
    {
        if (!Svc.Condition[ConditionFlag.OccupiedSummoningBell])
            return;

        if (TaskManager.IsBusy)
            return;

        TaskManager.InsertDelay(500);
        //TaskManager.Insert(SelectSellitems);
    }

    internal unsafe static bool? ClickEntrustDuplicatesConfirm()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("RetainerItemTransferList", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            var button = (AtkComponentButton*)addon->UldManager.NodeList[3]->GetComponent();
            if (addon->UldManager.NodeList[3]->IsVisible() && button->IsEnabled && EzThrottler.Throttle("Throttle button", 500, false))
            {
                button->ClickAddonButton(addon);
                MyServices.Services.PluginLog.Debug($"Clicked duplicates confirm");
                return true;
            }
        }
        else
        {
            GenericHelpersEx.RethrottleGeneric(500);
        }
        return false;
    }

    internal unsafe static bool? SelectListItem(int index)
    {
        // Retrieve the addon named "SelectString"
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("SelectString", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            // Access the list component node at index 2
            var listComponentNode = addon->UldManager.NodeList[2];
            var listComponent = (AtkComponentList*)listComponentNode->GetComponent();

            // Check if the list component node is visible
            if (listComponentNode->IsVisible())
            {
                // Get the item count to ensure the index is valid
                int itemCount = listComponent->GetItemCount();
                if (index >= 0 && index < itemCount)
                {
                    // Select the item at the specified index
                    listComponent->SelectItem(index, true);
                    MyServices.Services.PluginLog.Debug($"Selected item at index {index}.");
                    return true;
                }
                else
                {
                    MyServices.Services.PluginLog.Debug($"Index {index} is out of range.");
                }
            }
        }
        else
        {
            EzThrottler.Throttle("naj");
        }
        return false;
    }

    //internal unsafe static bool? ClickTravelToInstanceArea()
    //{
    //    if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("SelectString", out var addon) && GenericHelpers.IsAddonReady(addon))
    //    {
    //        var button = (AtkComponentButton*)addon->UldManager.NodeList[1]->GetComponent();
    //        if (addon->UldManager.NodeList[3]->IsVisible() && button->IsEnabled && EzThrottler.Throttle("Throttle button", 500, false))
    //        {
    //            button->ClickAddonButton(addon);
    //            Services.PluginLog.Debug($"Clicked duplicates confirm");
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        GenericHelpersEx.RethrottleGeneric(500);
    //    }
    //    return false;
    //}
}
