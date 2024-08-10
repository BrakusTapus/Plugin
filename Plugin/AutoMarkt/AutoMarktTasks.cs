using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using Dalamud.Utility;
using ECommons.Automation;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Plugin.Utilities;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster.RetainerList;
using static FFXIVClientStructs.FFXIV.Client.Game.RetainerManager;

namespace Plugin.AutoMarkt;
public static unsafe partial class AutoMarktTasks
{
    public static bool isInitialized = false;

    public static int PriceReduction = 1;

    public static int LowestAcceptablePrice = 100;

    public static bool SeparateNQAndHQ = true;

    public static int MaxPriceReduction = 5000;


    private static int CurrentItemPrice;
    private static int CurrentMarketLowestPrice;
    private static uint CurrentItemSearchItemID;
    private static bool IsCurrentItemHQ;
    private static int MarketItemCount;
    private static unsafe RetainerManager.Retainer* CurrentRetainer;
    public static unsafe RetainerManager* retainerManager;

    public static void Initialize()
    {
        if (isInitialized)
        {
            MyServices.Services.PluginLog.Warning("RetainerManager instance is already initialized!.");
            return;
        }
        retainerManager = RetainerManager.Instance();         // Get the instance of the RetainerManager and store it in the static property
        isInitialized = true;

        if (retainerManager != null)
        {
            MyServices.Services.PluginLog.Debug("RetainerManager instance initialized successfully.");
        }
        else
        {
            MyServices.Services.PluginLog.Error("Failed to initialize RetainerManager instance.");
        }
    }

    private static unsafe int GetRetainerManagerInstance()
    {
        Retainer* activeRetainer = retainerManager->GetActiveRetainer();
        MarketItemCount = (int)activeRetainer->MarketItemCount; // Casting byte to int
        return MarketItemCount;
    }

    public static class Listing
    {
        public static uint ID;
        public static int Price;
        public static int MarketLowest;
        public static string Retainer = "";
    }



    // Closes menu with all the retainerSortedByIndex 
    internal unsafe static bool? CloseRetainerList()
    {
        if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerList) && IsAddonReady(retainerList))
        {
            if (Utils.GenericThrottle)
            {
                var v = stackalloc AtkValue[1]
                {
                    new()
                    {
                        Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                        Int = -1
                    }
                };
                C.IsCloseActionAutomatic = true;
                retainerList->FireCallback(1, v);
                SimpleLog.DebugLog($"Closing retainerSortedByIndex window");
                Svc.Toasts.ShowQuest("Closing retainerSortedByIndex window!");
                return true;
            }
        }
        return false;
    }


    //1 can select a retainerSortedByIndex based on name
    public unsafe static bool? SelectRetainerByName(string name)
    {
        if (name == null)
        {
            throw new Exception($"Name can not be null or empty");
        }
        if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerList) && IsAddonReady(retainerList))
        {
            var list = new AddonMaster.RetainerList(retainerList);
            foreach (var retainer in list.Retainers)
            {
                if (retainer.Name == name)
                {
                    if (Utils.GenericThrottle)
                    {
                        SimpleLog.DebugLog($"Selecting retainerSortedByIndex {retainer.Name} with index {retainer.Index}");
                        Svc.Toasts.ShowQuest($"Selecting retainerSortedByIndex {retainer.Name} with index {retainer.Index}");
                        retainer.Select();
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public unsafe static bool SelectRetainerByIndex(uint index)
    {
        if (retainerManager == null)
        {
            throw new Exception("RetainerManager instance is null");
        }

        var retainerCount = retainerManager->GetRetainerCount();
        if (index >= retainerCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of available retainers.");
        }

        Retainer* retainerSortedByIndex = retainerManager->GetRetainerBySortedIndex(index);
        if (retainerSortedByIndex == null) return false;

        if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerList) && IsAddonReady(retainerList))
        {

            var list = new AddonMaster.RetainerList(retainerList);
            foreach (var retainer in list.Retainers)
            {
                if (retainer.Index == index)
                {
                    if (Utils.GenericThrottle)
                    {
                        // Log and select the retainerSortedByIndex
                        SimpleLog.DebugLog($"Selecting retainerSortedByIndex {retainer.Name} with index {index}");
                        Svc.Toasts.ShowQuest($"Selecting retainerSortedByIndex {retainer.Name} with index {index}");
                        retainer.Select();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //2 
    public static bool? SelectSellItemsInIventoryOnTheMarket()
    {
        // 2381,"<If(Equal(IntegerParameter(1),0))><Else/><Gui(63)/></If>Sell items in your retainerSortedByIndex's inventory on the market."
        var text = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Addon>().GetRow(2380).Text.ToDalamudString().ExtractText(true);
        Svc.Toasts.ShowQuest($"Item selected: {text}");
        return TrySelectSpecificEntry(text);
    }

    public static unsafe void OnRetainerSellList(AddonEvent type, AddonArgs args)
    {
        var activeRetainer = retainerManager->GetActiveRetainer();

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
    public static void EnqueueSingleItem(int index)
    {
        P.TaskManager.Enqueue(() => ClickSellingItem(index));
        P.TaskManager.EnqueueDelay(100);
        P.TaskManager.Enqueue(ClickAdjustPrice);
        P.TaskManager.EnqueueDelay(100);
        P.TaskManager.Enqueue(ClickComparePrice);
        P.TaskManager.EnqueueDelay(500);
        P.TaskManager.DefaultConfiguration.AbortOnTimeout = false;
        P.TaskManager.Enqueue(GetLowestPrice);
        P.TaskManager.DefaultConfiguration.AbortOnTimeout = true;
        P.TaskManager.EnqueueDelay(100);
        P.TaskManager.Enqueue(FillLowestPrice);
        P.TaskManager.EnqueueDelay(800);
    }

    // Gets listings from the inventory
    public static unsafe void GetListings()
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
    public static unsafe int GetListingsCount()
    {
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.RetainerMarket);
        return container != null ? Enumerable.Range(0, (int)container->Size).Count(i => container->GetInventorySlot(i) != null) : 0;
    }

    // Clicks the selling item in the UI
    public static unsafe bool? ClickSellingItem(int index)
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("RetainerSellList", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0, index, 1);
            return true;
        }

        return false;
    }

    // Clicks the adjust price button in the UI
    public static unsafe bool? ClickAdjustPrice()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("ContextMenu", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0, 0, 0, 0, 0);
            return true;
        }

        return false;
    }

    // Clicks the compare price button in the UI
    public static unsafe bool? ClickComparePrice()
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
    public static unsafe bool? GetLowestPrice()
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

            if (SeparateNQAndHQ && IsCurrentItemHQ)
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
    public static unsafe bool? FillLowestPrice()
    {
        try
        {
            if (GenericHelpers.TryGetAddonByName<AddonRetainerSell>("RetainerSell", out var addon) && GenericHelpers.IsAddonReady(&addon->AtkUnitBase))
            {
                var ui = &addon->AtkUnitBase;
                var priceComponent = addon->AskingPrice;

                // Check if the current market lowest price is below the acceptable price
                if (CurrentMarketLowestPrice - PriceReduction < LowestAcceptablePrice)
                {
                    var message = GetSeString(
                "Item is listed lower than minimum price, skipping",
                SeString.CreateItemLink(
                   CurrentItemSearchItemID,
                    IsCurrentItemHQ  ? ItemPayload.ItemKind.Hq : ItemPayload.ItemKind.Normal),
                CurrentMarketLowestPrice,
                CurrentItemPrice,
                LowestAcceptablePrice);

                    ModuleMessage(message);
                    Callback.Fire((AtkUnitBase*)addon, true, 1);
                    ui->Close(true);

                    return true;
                }

                // Check if the current item price exceeds the maximum acceptable reduction
                if (MaxPriceReduction != 0 &&
                    CurrentItemPrice - CurrentMarketLowestPrice > MaxPriceReduction)
                {
                    var message = GetSeString(
                    "Item has exceeded maximum acceptable reduction, skipping",
                    SeString.CreateItemLink(
                        CurrentItemSearchItemID,
                        IsCurrentItemHQ ? ItemPayload.ItemKind.Hq : ItemPayload.ItemKind.Normal),
                    CurrentMarketLowestPrice,
                    CurrentItemPrice,
                    MaxPriceReduction);

                    ModuleMessage(message);
                    Callback.Fire((AtkUnitBase*)addon, true, 1);
                    ui->Close(true);
                    return true;
                }

                priceComponent->SetValue(CurrentMarketLowestPrice - PriceReduction);
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


    public unsafe static bool HandleMarketBoardError()
    {
        try
        {
            // Check if the error message is displayed
            if (IsMarketBoardErrorDisplayed())
            {
                if (GenericHelpers.TryGetAddonByName<AddonRetainerSell>("ItemSearchResult", out var addon) && GenericHelpers.IsAddonReady(&addon->AtkUnitBase))
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


    // Method to check if the market board error message is displayed
    private static unsafe bool IsMarketBoardErrorDisplayed()
    {
        try
        {
            // Get the ItemSearchResult addon
            var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ItemSearchResult", 1);
            if (addon == null || !IsAddonReady(addon))
            {
                return false; // Return false if the addon is not found or not ready
            }

            // Get the text node where the error message is expected to be displayed
            var node = addon->UldManager.NodeList[26];
            if (node == null)
            {
                return false; // Return false if the node is not found
            }

            var textNode = node->GetAsAtkTextNode();
            if (textNode == null)
            {
                return false; // Return false if the text node is not found
            }

            // Extract and normalize the text from the node
            var text = MemoryHelper.ReadSeString(&textNode->NodeText).ExtractText();
            // Check if the text contains the specific error message
            return text.Contains("Please wait and try your search again.", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e)
        {
            // Log any exceptions that occur while checking for the error message
            e.Log();
            return false; // Return false if an exception occurs
        }
    }

    private static readonly Dictionary<string, string>? resourceData = new Dictionary<string, string>();
    private static readonly Dictionary<string, string>? fbResourceData = new Dictionary<string, string>();

    public static SeString GetSeString(string key, params object[] args)
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






    public static void ModuleMessage(SeString messageTemplate) => ModuleMessage(messageTemplate.TextValue);
    public static void ModuleMessage(string messageTemplate)
    {
        var message = new XivChatEntry
        {
            Message = new SeStringBuilder()
                .AddUiForeground($"[{P.Name}] ", 62)
                .Append(messageTemplate)
                .Build()
        };

        Svc.Chat.Print(message);
    }




























    internal static bool GenericThrottle => C.UseFrameDelay ? FrameThrottler.Throttle("AutoRetainerGenericThrottle", C.FrameDelay) : EzThrottler.Throttle("AutoRetainerGenericThrottle", C.Delay);
    internal static void RethrottleGeneric(int num)
    {
        if (C.UseFrameDelay)
        {
            FrameThrottler.Throttle("AutoRetainerGenericThrottle", num, true);
        }
        else
        {
            EzThrottler.Throttle("AutoRetainerGenericThrottle", num, true);
        }
    }
    internal static void RethrottleGeneric()
    {
        if (C.UseFrameDelay)
        {
            FrameThrottler.Throttle("AutoRetainerGenericThrottle", C.FrameDelay, true);
        }
        else
        {
            EzThrottler.Throttle("AutoRetainerGenericThrottle", C.Delay, true);
        }
    }

    internal static bool TrySelectSpecificEntry(string text, Func<bool> Throttler = null)
    {
        return TrySelectSpecificEntry(new string[] { text }, Throttler);
    }
    internal static bool TrySelectSpecificEntry(IEnumerable<string> text, Func<bool> Throttler = null)
    {
        return TrySelectSpecificEntry((x) => x.StartsWithAny(text), Throttler);
    }

    internal static bool TrySelectSpecificEntry(Func<string, bool> inputTextTest, Func<bool> Throttler = null)
    {
        if (TryGetAddonByName<AddonSelectString>("SelectString", out var addon) && IsAddonReady(&addon->AtkUnitBase))
        {
            if (new AddonMaster.SelectString(addon).Entries.TryGetFirst(x => inputTextTest(x.Text), out var entry))
            {
                if (IsSelectItemEnabled(addon, entry.Index) && (Throttler?.Invoke() ?? GenericThrottle))
                {
                    entry.Select();
                    SimpleLog.DebugLog($"TrySelectSpecificEntry: selecting {entry}");
                    return true;
                }
            }
        }
        else
        {
            RethrottleGeneric();
        }
        return false;
    }

    internal static bool IsSelectItemEnabled(AddonSelectString* addon, int index)
    {
        var step1 = (AtkTextNode*)addon->AtkUnitBase
                    .UldManager.NodeList[2]
                    ->GetComponent()->UldManager.NodeList[index + 1]
                    ->GetComponent()->UldManager.NodeList[3];
        return GenericHelpers.IsSelectItemEnabled(step1);
    }




}
