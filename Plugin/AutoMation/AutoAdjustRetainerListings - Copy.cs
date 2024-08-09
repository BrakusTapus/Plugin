using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Memory;
using ECommons;
using ECommons.Automation;
using ECommons.Configuration;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Plugin.FeaturesSetup;
using Plugin.FeaturesSetup.Attributes;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Plugin.Features;

public class AutoAdjustRetainerListingsConfiguration
{
    [IntConfig(DefaultValue = 1)]
    public int PriceReduction = 1;

    [IntConfig(DefaultValue = 100)]
    public int LowestAcceptablePrice = 100;

    [BoolConfig]
    public bool SeparateNQAndHQ = false;

    [IntConfig(DefaultValue = 0)]
    public int MaxPriceReduction = 0;
}

[Tweak]
public partial class AutoAdjustRetainerListings : Tweak<AutoAdjustRetainerListingsConfiguration>
{
    public override string Name => "Auto Adjust Retainer Listings";
    public override string Description => "This feature is undergoing functionality and configuration renovations and is subject to heavy changes in the future. Do not speak of this feature.";

    private static Listing? CurrentItem;
    private static Listing? LastItem;
    private static List<Listing>? Listings;
    private static int CurrentItemPrice;
    private static int CurrentMarketLowestPrice;
    private static uint CurrentItemSearchItemID;
    private static bool IsCurrentItemHQ;
    private static unsafe RetainerManager.Retainer* CurrentRetainer;

    public class Listing
    {
        public uint ID;
        public int Price;
        public int MarketLowest;
        public string Retainer = "";
    }

    public override void Enable()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "RetainerSellList", OnRetainerSellList); // List of items
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "RetainerSell", OnRetainerSell);
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "RetainerSell", OnRetainerSell);
    }

    public override void Disable()
    {
        Svc.AddonLifecycle.UnregisterListener(OnRetainerSellList);
        Svc.AddonLifecycle.UnregisterListener(OnRetainerSell);
        TaskManager.Abort();
    }

    public override void DrawConfig()
    {
        DrawConfigUI();
        base.DrawConfig();
        //if (ImGui.Button("cancel"))
        //    TaskManager.Abort();

    }

    private static bool IsEnAbled = false;
    private void DrawConfigUI()
    {
        if (ImGui.Checkbox("Enable Auto Adjust Retainer Listings", ref IsEnAbled))
        {
            // Update logic based on the new state of IsEnAbled
            if (IsEnAbled)
            {
                Enable(); // Call the enable method
            }
            else
            {
                Disable(); // Call the disable method
            }
        }

        try
        {
            bool stepMode = TaskManager.StepMode;
            if (ImGui.Checkbox("Step Mode", ref stepMode))
            {
                TaskManager.StepMode = stepMode;
            }
            ImGui.SameLine();
            // Always render the "Advance Task" button
            if (ImGui.Button("Advance Task"))
            {
                if (TaskManager.StepMode)
                {
                    TaskManager.Step(); // Manually advance tasks if StepMode is enabled
                }
                else
                {
                    // Optionally provide feedback if StepMode is not enabled
                    ImGui.Text("Step Mode is disabled. Enable it to manually advance tasks.");
                }
            }
            RenderTaskManagerInfo();
        }
        catch (Exception ex)
        {

            Svc.Log.Error($"{ex}");
        }

    }

    void RenderTaskManagerInfo()
    {
        // Default values for TaskManager properties
        string currentTask = TaskManager.CurrentTask?.ToString() ?? "No Current Task";
        bool isBusy = TaskManager.IsBusy;
        int maxTasks = TaskManager.MaxTasks > 0 ? TaskManager.MaxTasks : 0;
        int numQueuedTasks = TaskManager.NumQueuedTasks > 0 ? TaskManager.NumQueuedTasks : 0;
        float progress = TaskManager.Progress >= 0 ? TaskManager.Progress : 0.0f;
        long remainingTimeMS = TaskManager.RemainingTimeMS >= 0 ? TaskManager.RemainingTimeMS : 0L;

        // Render each property on a separate line
        ImGui.Text("Current Task: " + currentTask);
        ImGui.Text("Is Busy: " + isBusy.ToString());
        ImGui.Text("Max Tasks: " + maxTasks.ToString());
        ImGui.Text("Queued Tasks: " + numQueuedTasks.ToString());
        ImGui.Text("Progress: " + progress.ToString("0.00%"));  // Display as percentage
        ImGui.Text("Remaining Time (ms): " + remainingTimeMS.ToString());
    }

    //Method to handle the retainer sell event
    private void OnRetainerSell(AddonEvent eventType, AddonArgs addonInfo)
    {
        switch (eventType)
        {
            case AddonEvent.PostSetup:
                if (TaskManager.IsBusy) return;
                TaskManager.Enqueue(ClickComparePrice);
                TaskManager.DefaultConfiguration.AbortOnTimeout = false;
                TaskManager.EnqueueDelay(500);
                TaskManager.Enqueue(GetLowestPrice);
                TaskManager.DefaultConfiguration.AbortOnTimeout = true;
                TaskManager.EnqueueDelay(100);
                TaskManager.Enqueue(FillLowestPrice);
                break;
            case AddonEvent.PreFinalize:
                if (TaskManager.NumQueuedTasks <= 1)
                    TaskManager.Abort();
                break;
        }
    }

    // Method to handle the retainer sell list event
    private unsafe void OnRetainerSellList(AddonEvent type, AddonArgs args)
    {
        var activeRetainer = RetainerManager.Instance()->GetActiveRetainer();
        if (CurrentRetainer == null || CurrentRetainer != activeRetainer)
            CurrentRetainer = activeRetainer;
        else
            return;

        for (var i = 0; i < activeRetainer->MarketItemCount; i++)
        {
            EnqueueSingleItem(i);
            CurrentMarketLowestPrice = 0;
        }
    }

    // Enqueues a single item for processing
    private void EnqueueSingleItem(int index)
    {
        TaskManager.Enqueue(() => ClickSellingItem(index));
        TaskManager.EnqueueDelay(100);
        TaskManager.Enqueue(ClickAdjustPrice);
        TaskManager.EnqueueDelay(100);
        TaskManager.Enqueue(ClickComparePrice);
        TaskManager.EnqueueDelay(500);
        TaskManager.DefaultConfiguration.AbortOnTimeout = false;
        TaskManager.Enqueue(GetLowestPrice);
        TaskManager.DefaultConfiguration.AbortOnTimeout = true;
        TaskManager.EnqueueDelay(100);
        TaskManager.Enqueue(FillLowestPrice);
        TaskManager.EnqueueDelay(800);
    }

    // Gets listings from the inventory
    private static unsafe void GetListings()
    {
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.RetainerMarket);
        for (var i = 0; i < container->Size; i++)
        {
            var item = container->GetInventorySlot(i);
            if (item == null) continue;
        }
        Enumerable.Range(0, (int)container->Size).Count(i => container->GetInventorySlot(i) != null);
    }

    // Gets the count of listings in the inventory
    private static unsafe int GetListingsCount()
    {
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.RetainerMarket);
        return container != null ? Enumerable.Range(0, (int)container->Size).Count(i => container->GetInventorySlot(i) != null) : 0;
    }

    // Clicks the selling item in the UI
    private static unsafe bool? ClickSellingItem(int index)
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("RetainerSellList", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0, index, 1);
            return true;
        }

        return false;
    }

    // Clicks the adjust price button in the UI
    private static unsafe bool? ClickAdjustPrice()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("ContextMenu", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0, 0, 0, 0, 0);
            return true;
        }

        return false;
    }

    // Clicks the compare price button in the UI
    private static unsafe bool? ClickComparePrice()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("RetainerSell", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            CurrentItemPrice = addon->AtkValues[5].Int;
            IsCurrentItemHQ = Marshal.PtrToStringUTF8((nint)addon->AtkValues[1].String)!.Contains(''); // hq symbol

            Callback.Fire(addon, true, 4);
            return true;
        }

        return false;
    }

    // Gets the lowest price from the market board
    private unsafe bool? GetLowestPrice()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("ItemSearchResult", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            CurrentItemSearchItemID = AgentItemSearch.Instance()->ResultItemId;
            var searchResult = addon->GetTextNodeById(29)->NodeText.ExtractText();
            if (string.IsNullOrEmpty(searchResult)) return false;

            if (int.Parse(AutoRetainerPriceAdjustRegex().Replace(searchResult, "")) == 0)
            {
                CurrentMarketLowestPrice = 0;
                addon->Close(true);
                return true;
            }

            if (Config.SeparateNQAndHQ && IsCurrentItemHQ)
            {
                var foundHQItem = false;
                for (var i = 1; i <= 12 && !foundHQItem; i++)
                {
                    var listing = addon->UldManager.NodeList[5]->GetAsAtkComponentNode()->Component->UldManager.NodeList[i]->GetAsAtkComponentNode()->Component->UldManager.NodeList;
                    if (listing[13]->GetAsAtkImageNode()->AtkResNode.IsVisible())
                    {
                        var priceText = listing[10]->GetAsAtkTextNode()->NodeText.ExtractText();
                        if (int.TryParse(AutoRetainerPriceAdjustRegex().Replace(priceText, ""), out CurrentMarketLowestPrice)) foundHQItem = true;
                    }
                }

                if (!foundHQItem)
                {
                    var priceText = addon->UldManager.NodeList[5]->GetAsAtkComponentNode()->Component->UldManager.NodeList[1]->GetAsAtkComponentNode()->Component->UldManager.NodeList[10]->GetAsAtkTextNode()->NodeText.ExtractText();
                    if (!int.TryParse(AutoRetainerPriceAdjustRegex().Replace(priceText, ""), out CurrentMarketLowestPrice)) return false;
                }
            }
            else
            {
                var priceText = addon->UldManager.NodeList[5]->GetAsAtkComponentNode()->Component->UldManager.NodeList[1]->GetAsAtkComponentNode()->Component->UldManager.NodeList[10]->GetAsAtkTextNode()->NodeText.ExtractText();
                if (!int.TryParse(AutoRetainerPriceAdjustRegex().Replace(priceText, ""), out CurrentMarketLowestPrice)) return false;
            }

            addon->Close(true);
            return true;
        }

        return false;
    }

    // Fills the lowest price into the UI
    private unsafe bool? FillLowestPrice()
    {
        try
        {
            if (GenericHelpers.TryGetAddonByName<AddonRetainerSell>("RetainerSell", out var addon) && GenericHelpers.IsAddonReady(&addon->AtkUnitBase))
            {
                var ui = &addon->AtkUnitBase;
                var priceComponent = addon->AskingPrice;

                // Check if the current market lowest price is below the acceptable price
                if (CurrentMarketLowestPrice - Config.PriceReduction < Config.LowestAcceptablePrice)
                {
                    var message = GetSeString(
                "Item is listed lower than minimum price, skipping",
                SeString.CreateItemLink(
                   CurrentItemSearchItemID,
                    IsCurrentItemHQ  ? ItemPayload.ItemKind.Hq : ItemPayload.ItemKind.Normal),
                CurrentMarketLowestPrice,
                CurrentItemPrice,
                Config.LowestAcceptablePrice);

                    ModuleMessage(message);
                    Callback.Fire((AtkUnitBase*)addon, true, 1);
                    ui->Close(true);

                    return true;
                }

                // Check if the current item price exceeds the maximum acceptable reduction
                if (Config.MaxPriceReduction != 0 &&
                    CurrentItemPrice - CurrentMarketLowestPrice > Config.MaxPriceReduction)
                {
                    var message = GetSeString(
                    "Item has exceeded maximum acceptable reduction, skipping",
                    SeString.CreateItemLink(
                        CurrentItemSearchItemID,
                        IsCurrentItemHQ ? ItemPayload.ItemKind.Hq : ItemPayload.ItemKind.Normal),
                    CurrentMarketLowestPrice,
                    CurrentItemPrice,
                    Config.MaxPriceReduction);

                    ModuleMessage(message);
                    Callback.Fire((AtkUnitBase*)addon, true, 1);
                    ui->Close(true);
                    return true;
                }

                priceComponent->SetValue(CurrentMarketLowestPrice - Config.PriceReduction);
                Callback.Fire((AtkUnitBase*)addon, true, 0);
                ui->Close(true);
                return true;
            }
        }
        catch (Exception ex)
        {
            // Log the exception and skip the item
            DuoLog.Error($"Exception in FillLowestPrice: {ex.Message}");
            return false; // Indicate that the operation failed and should be skipped
        }

        return false;
    }


    private unsafe bool HandleMarketBoardError()
    {
        try
        {
            // Check if the error message is displayed
            if (IsMarketBoardErrorDisplayed())
            {
                if (GenericHelpers.TryGetAddonByName<AddonRetainerSell>("RetainerSell", out var addon) && GenericHelpers.IsAddonReady(&addon->AtkUnitBase))
                {
                    var ui = &addon->AtkUnitBase;
                    var priceComponent = addon->AskingPrice;
                    Callback.Fire((AtkUnitBase*)addon, true, 0);
                    ui->Close(true);
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {
            e.Log();
        }
        return false;
    }


    private unsafe bool IsMarketBoardErrorDisplayed()
    {
        try
        {
            // Try to get the addon by name
            var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ItemSearchResult", 1);
            if (addon == null || !IsAddonReady(addon))
            {
                return false;
            }

            // Get the node that might contain the error message
            var node = addon->UldManager.NodeList[26];
            if (node == null)
            {
                return false;
            }

            var textNode = node->GetAsAtkTextNode();
            if (textNode == null)
            {
                return false;
            }

            var text = MemoryHelper.ReadSeString(&textNode->NodeText).ExtractText();
            return text.Contains("Please wait and try your search again.", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e)
        {
            e.Log();
            return false;
        }
    }




    private readonly Dictionary<string, string>? resourceData = new Dictionary<string, string>();
    private readonly Dictionary<string, string>? fbResourceData = new Dictionary<string, string>();

    public SeString GetSeString(string key, params object[] args)
    {
        var format = resourceData.TryGetValue(key, out var resValue) ? resValue : fbResourceData.GetValueOrDefault(key);
        var ssb = new SeStringBuilder();
        var lastIndex = 0;

        ssb.AddUiForeground($"[{nameof(Plugin.Name)}]", 34);
        foreach (var match in SeStringRegex().Matches(format).Cast<Match>())
        {
            ssb.AddUiForeground(format[lastIndex..match.Index], 2);
            lastIndex = match.Index + match.Length;

            if (int.TryParse(match.Groups[1].Value, out var argIndex) && argIndex >= 0 && argIndex < args.Length)
            {
                if (args[argIndex] is SeString @seString)
                {
                    ssb.Append(@seString);
                }
                else
                {
                    ssb.AddUiForeground(args[argIndex].ToString(), 2);
                }
            }
        }

        ssb.AddUiForeground(format[lastIndex..], 2);
        return ssb.Build();
    }

    [GeneratedRegex("\\{(\\d+)\\}")]
    private static partial Regex SeStringRegex();

    [GeneratedRegex("[^0-9]")]
    private static partial Regex AutoRetainerPriceAdjustRegex();
}
