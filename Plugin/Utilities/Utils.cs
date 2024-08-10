using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons.Reflection;
using ECommons.SimpleGui;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

using System.Reflection;

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Memory;
using ECommons.ExcelServices.TerritoryEnumeration;

using ECommons.MathHelpers;

using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;

using Plugin.Data;
using CharaData = (string Name, ushort World);

// using Plugin.Configurations; //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
using Plugin.Utilities.Enums;
using Lumina.Excel.GeneratedSheets;
using ECommons.Throttlers;

namespace Plugin.Utilities;
public unsafe static class Utils
{
    public enum MovementType
    {
        Direct,
        Pathfind
    }

    public static IDalamudTextureWrap? GetIcon(uint iconId) => iconId != 0 ? Svc.Texture?.GetFromGameIcon(iconId).GetWrapOrEmpty() : null;

    public static bool HasPlugin(string name) => DalamudReflector.TryGetDalamudPlugin(name, out _, false, true);

    private static readonly Dictionary<Type, AgentId> AgentIdCache = [];
    public static unsafe T* GetAgent<T>(AgentId id) where T : unmanaged
        => (T*)AgentModule.Instance()->GetAgentByInternalId(id);

    public static unsafe T* GetAgent<T>() where T : unmanaged
    {
        var type = typeof(T);

        if (!AgentIdCache.TryGetValue(type, out var id))
        {
            var attr = type.GetCustomAttribute<AgentAttribute>(false)
                ?? throw new Exception($"Agent {type.FullName} is missing AgentAttribute");

            AgentIdCache.Add(type, id = attr.Id);
        }

        return GetAgent<T>(id);
    }

    public const int UnitListCount = 18;
    public static unsafe AtkUnitBase* GetAddonByID(uint id)
    {
        var unitManagers = &AtkStage.Instance()->RaptureAtkUnitManager->AtkUnitManager.DepthLayerOneList;
        for (var i = 0; i < UnitListCount; i++)
        {
            var unitManager = &unitManagers[i];
            foreach (var j in Enumerable.Range(0, Math.Min(unitManager->Count, unitManager->Entries.Length)))
            {
                var unitBase = unitManager->Entries[j].Value;
                if (unitBase != null && unitBase->Id == id)
                {
                    return unitBase;
                }
            }
        }

        return null;
    }

    public static unsafe bool IsClickingInGameWorld()
        => !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow)
        && !ImGui.GetIO().WantCaptureMouse
        && AtkStage.Instance()->RaptureAtkUnitManager->AtkUnitManager.FocusedUnitsList.Count == 0
        && Framework.Instance()->Cursor->ActiveCursorType == 0;

    public static Vector3 RotatePoint(float cx, float cy, float angle, Vector3 p)
    {
        if (angle == 0f) return p;
        var s = (float)Math.Sin(angle);
        var c = (float)Math.Cos(angle);

        // translate point back to origin:
        p.X -= cx;
        p.Z -= cy;

        // rotate point
        var xnew = p.X * c - p.Z * s;
        var ynew = p.X * s + p.Z * c;

        // translate point back:
        p.X = xnew + cx;
        p.Z = ynew + cy;
        return p;
    }

    public static unsafe Structs.AgentMJICraftSchedule* Agent = (Structs.AgentMJICraftSchedule*)AgentModule.Instance()->GetAgentByInternalId(AgentId.MJICraftSchedule);
    public static unsafe Structs.AgentMJICraftSchedule.AgentData* AgentData => Agent != null ? Agent->Data : null;

    public static unsafe void SetRestCycles(uint mask)
    {
        Svc.Log.Debug($"Setting rest: {mask:X}");
        AgentData->NewRestCycles = mask;
        SynthesizeEvent(5, [new() { Type = AtkValueType.Int, Int = 0 }]);
    }

    private static unsafe void SynthesizeEvent(ulong eventKind, Span<AtkValue> args)
    {
        var eventData = stackalloc int[] { 0, 0, 0 };
        Agent->AgentInterface.ReceiveEvent((AtkValue*)eventData, args.GetPointer(0), (uint)args.Length, eventKind);
    }

    public static T GetService<T>()
    {
        Svc.Log.Info($"Requesting {typeof(T)}");
        var service = typeof(IDalamudPlugin).Assembly.GetType("Dalamud.Service`1")!.MakeGenericType(typeof(T));
        var get = service.GetMethod("Get", BindingFlags.Public | BindingFlags.Static)!;
        return (T)get.Invoke(null, null)!;
    }

    public static bool AllNull(params object[] objects) => objects.All(s => s == null);
    public static bool AnyNull(params object[] objects) => objects.Any(s => s == null);

    public static T? GetWindow<T>() where T : Window
    {
        if (!typeof(T).IsSubclassOf(typeof(Window)))
            return null;
        return EzConfigGui.WindowSystem.Windows.FirstOrDefault(w => w.GetType() == typeof(T)) as T;
    }

    public static void RemoveWindow<T>() where T : Window
    {
        if (!typeof(T).IsSubclassOf(typeof(Window)))
            return;
        var window = EzConfigGui.WindowSystem.Windows.FirstOrDefault(w => w.GetType() == typeof(T));
        if (window != null)
            EzConfigGui.WindowSystem.RemoveWindow(window);
    }

    public static unsafe AtkResNode* GetNodeByIDChain(AtkResNode* node, params int[] ids)
    {
        if (node == null || ids.Length <= 0)
            return null;

        if (node->NodeId == ids[0])
        {
            if (ids.Length == 1)
                return node;

            var newList = new List<int>(ids);
            newList.RemoveAt(0);

            var childNode = node->ChildNode;
            if (childNode != null)
                return GetNodeByIDChain(childNode, [.. newList]);

            if ((int)node->Type >= 1000)
            {
                var componentNode = node->GetAsAtkComponentNode();
                var component = componentNode->Component;
                var uldManager = component->UldManager;
                childNode = uldManager.NodeList[0];
                return childNode == null ? null : GetNodeByIDChain(childNode, [.. newList]);
            }

            return null;
        }

        //check siblings
        var sibNode = node->PrevSiblingNode;
        return sibNode != null ? GetNodeByIDChain(sibNode, ids) : null;
    }

    public static unsafe bool GetUnitBase(string name, out AtkUnitBase* unitBase, int index = 1)
    {
        unitBase = GetUnitBase(name, index);
        return unitBase != null;
    }

    public static unsafe AtkUnitBase* GetUnitBase(string name, int index = 1) => (AtkUnitBase*)Svc.GameGui.GetAddonByName(name, index);

    public static unsafe T* GetUnitBase<T>(string? name = null, int index = 1) where T : unmanaged
    {
        if (string.IsNullOrEmpty(name))
        {
            var attr = (FFXIVClientStructs.Attributes.Addon)typeof(T).GetCustomAttribute(typeof(FFXIVClientStructs.Attributes.Addon));
            if (attr != null)
            {
                name = attr.AddonIdentifiers.FirstOrDefault();
            }
        }

        return string.IsNullOrEmpty(name) ? null : (T*)Svc.GameGui.GetAddonByName(name, index);
    }

    public static unsafe bool GetUnitBase<T>(out T* unitBase, string? name = null, int index = 1) where T : unmanaged
    {
        unitBase = null;
        if (string.IsNullOrEmpty(name))
        {
            var attr = (FFXIVClientStructs.Attributes.Addon)typeof(T).GetCustomAttribute(typeof(FFXIVClientStructs.Attributes.Addon));
            if (attr != null)
            {
                name = attr.AddonIdentifiers.FirstOrDefault();
            }
        }

        if (string.IsNullOrEmpty(name)) return false;

        unitBase = (T*)Svc.GameGui.GetAddonByName(name, index);
        return unitBase != null;
    }



    internal static bool GenericThrottle => C.UseFrameDelay ?
        FrameThrottler.Throttle("AutoRetainerGenericThrottle", C.FrameDelay) :
        EzThrottler.Throttle("AutoRetainerGenericThrottle", C.Delay);

    public static uint[] AethernetShards = [2000151, 2000153, 2000154, 2000155, 2000156, 2000157, 2003395, 2003396, 2003397, 2003398, 2003399, 2003400, 2003401, 2003402, 2003403, 2003404, 2003405, 2003406, 2003407, 2003408, 2003409, 2003995, 2003996, 2003997, 2003998, 2003999, 2004000, 2004968, 2004969, 2004970, 2004971, 2004972, 2004973, 2004974, 2004976, 2004977, 2004978, 2004979, 2004980, 2004981, 2004982, 2004983, 2004984, 2004985, 2004986, 2004987, 2004988, 2004989, 2007434, 2007435, 2007436, 2007437, 2007438, 2007439, 2007855, 2007856, 2007857, 2007858, 2007859, 2007860, 2007861, 2007862, 2007863, 2007864, 2007865, 2007866, 2007867, 2007868, 2007869, 2007870, 2009421, 2009432, 2009433, 2009562, 2009563, 2009564, 2009565, 2009615, 2009616, 2009617, 2009618, 2009713, 2009714, 2009715, 2009981, 2010135, 2011142, 2011162, 2011163, 2011241, 2011243, 2011373, 2011374, 2011384, 2011385, 2011386, 2011387, 2011388, 2011389, 2011573, 2011574, 2011575, 2011677, 2011678, 2011679, 2011680, 2011681, 2011682, 2011683, 2011684, 2011685, 2011686, 2011687, 2011688, 2011689, 2011690, 2011691, 2011692, 2012252, 2012253,];

    public static uint[] HousingAethernet = [MainCities.Limsa_Lominsa_Lower_Decks, MainCities.Uldah_Steps_of_Nald, MainCities.New_Gridania, MainCities.Foundation, MainCities.Kugane];

    public static void DisplayInfo(string s, bool? displayChat = null, bool? displayPopup = null)
    {
        if (displayChat ?? C.DisplayChatTeleport) ECommons.ChatMethods.ChatPrinter.Green($"[Lifestream] {s}");
        if (displayPopup ?? C.DisplayPopupNotifications) Notify.Info(s);
    }

    internal static bool IsDisallowedToUseAethernet()
    {
        return Svc.Condition[ConditionFlag.WaitingToVisitOtherWorld] || Svc.Condition[ConditionFlag.Jumping];
    }

    internal static IGameObject GetReachableWorldChangeAetheryte(bool littleDistance = false) => GetReachableAetheryte(x => TryGetTinyAetheryteFromIGameObject(x, out var ae) && ae?.IsWorldChangeAetheryte() == true, littleDistance);

    internal static IGameObject GetReachableResidentialAetheryte(bool littleDistance = false) => GetReachableAetheryte(x => TryGetTinyAetheryteFromIGameObject(x, out var ae) && ae?.IsResidentialAetheryte() == true, littleDistance);

    internal static IGameObject GetReachableAetheryte(Predicate<IGameObject> predicate, bool littleDistance = false)
    {
        if (!Player.Available) return null;
        var a = Svc.Objects.OrderBy(x => Vector3.DistanceSquared(Player.Object.Position, x.Position)).FirstOrDefault(x => predicate(x));
        if (a != null && a.IsTargetable && Vector3.Distance(a.Position, Player.Object.Position) < (littleDistance ? 13f : 30f))
        {
            return a;
        }
        return null;
    }


    internal static string[] DefaultAddons = [
        "Inventory",
        "InventoryLarge",
        "AreaMap",
        "InventoryRetainerLarge",
        "Currency",
        "Bank",
        "RetainerTask",
        "RetainerList",
        "SelectYesNo",
        "SelectString",
        "SystemMenu",
        "MountNoteBook",
        "FriendList",
        "BlackList",
        "Character",
        "ItemSearch",
        "MonsterNote",
        "GatheringNote",
        "RecipeNote",
        "ContentsFinder",
        "LookingForGroup",
        "Journal",
        "ContentsInfo",
        "SocialList",
        "Macro",
        "ConfigKeybind",
        "GSInfoGeneral",
        "ContentsInfo",
        "PartyMemberList",
        "ContentsFinderConfirm",
        "SelectString"
    ];

    internal static string[] ExtraAddons = [
        "Description",
        "CharaCard",
        "CharaCardEditMenu",
        "BannerList",
        "BannerEditor",
        "CurrencySetting",
        "ArmouryBoard",
        "InventoryBuddy",
        "MiragePrismMiragePlateConfirm",
        "MiragePrismExecute",
        "MiragePrismRemove",
        "MiragePrism",
        "InputNumeric",
        "Trade",
        "ColorantColoring",
        "ColorantEquipment",
        "MateriaAttach",
        "MateriaAttachDialog",
        "Synthesis",
        "CraftActionSimulator",
        "LetterList",
        "RecipeProductList",

    ];

    internal static bool IsResidentialAetheryte(this TinyAetheryte t)
    {
        return t.ID.EqualsAny<uint>(2, 8, 9, 70, 111);
    }

    internal static bool IsAddonsVisible(IEnumerable<string> addons)
    {
        foreach (var x in addons)
        {
            if (GenericHelpers.TryGetAddonByName<AtkUnitBase>(x, out var a) && a->IsVisible) return true;
        }
        return false;
    }

    internal static bool IsMapActive()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("AreaMap", out var map) && GenericHelpers.IsAddonReady(map) && map->IsVisible)
        {
            return true;
        }
        return false;
    }

    internal static bool TryGetTinyAetheryteFromIGameObject(IGameObject a, out TinyAetheryte? t, uint? TerritoryType = null)
    {
        TerritoryType ??= Svc.ClientState.TerritoryType;
        if (a == null)
        {
            t = default;
            return false;
        }
        if (a.ObjectKind == ObjectKind.Aetheryte)
        {
            var pos2 = a.Position.ToVector2();
            foreach (var x in P.DataStore.Aetherytes)
            {
                if (x.Key.TerritoryType == TerritoryType && Vector2.Distance(x.Key.Position, pos2) < 10)
                {
                    t = x.Key;
                    return true;
                }
                foreach (var l in x.Value)
                {
                    if (l.TerritoryType == TerritoryType && Vector2.Distance(l.Position, pos2) < 10)
                    {
                        t = l;
                        return true;
                    }
                }
            }
        }
        t = default;
        return false;
    }

    internal static float ConvertMapMarkerToMapCoordinate(int pos, float scale)
    {
        var num = scale / 100f;
        var rawPosition = (int)((float)(pos - 1024.0) / num * 1000f);
        return ConvertRawPositionToMapCoordinate(rawPosition, scale);
    }

    internal static float ConvertMapMarkerToRawPosition(int pos, float scale)
    {
        var num = scale / 100f;
        var rawPosition = (float)(pos - 1024.0) / num;
        return rawPosition;
    }

    internal static float ConvertRawPositionToMapCoordinate(int pos, float scale)
    {
        var num = scale / 100f;
        return (float)((pos / 1000f * num + 1024.0) / 2048.0 * 41.0 / num + 1.0);
    }

    internal static AtkUnitBase* GetSpecificYesno(params string[] s) => GetSpecificYesno(false, s);

    internal static AtkUnitBase* GetSpecificYesno(bool contains, params string[] s)
    {
        for (var i = 1; i < 100; i++)
        {
            try
            {
                var addon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("SelectYesno", i);
                if (addon == null) return null;
                if (GenericHelpers.IsAddonReady(addon))
                {
                    var textNode = addon->UldManager.NodeList[15]->GetAsAtkTextNode();
                    var text = MemoryHelper.ReadSeString(&textNode->NodeText).ExtractText().Replace(" ", "");
                    if (contains ?
                        text.ContainsAny(s.Select(x => x.Replace(" ", "")))
                        : text.EqualsAny(s.Select(x => x.Replace(" ", "")))
                        )
                    {
                        PluginLog.Verbose($"SelectYesno {s.Print()} addon {i}");
                        return addon;
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
                return null;
            }
        }
        return null;
    }

    internal static string[] GetAvailableWorldDestinations()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("WorldTravelSelect", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            List<string> arr = [];
            for (var i = 3; i <= 9; i++)
            {
                var item = addon->UldManager.NodeList[4]->GetAsAtkComponentNode()->Component->UldManager.NodeList[i];
                var text = MemoryHelper.ReadSeString(&item->GetAsAtkComponentNode()->Component->UldManager.NodeList[4]->GetAsAtkTextNode()->NodeText).ExtractText();
                if (text == "") break;
                arr.Add(text);
            }
            return [.. arr];
        }
        return Array.Empty<string>();
    }

    internal static string[] GetAvailableAethernetDestinations()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("TelepotTown", out var addon) && GenericHelpers.IsAddonReady(addon))
        {
            List<string> arr = [];
            for (var i = 1; i <= 52; i++)
            {
                var item = addon->UldManager.NodeList[16]->GetAsAtkComponentNode()->Component->UldManager.NodeList[i];
                var text = MemoryHelper.ReadSeString(&item->GetAsAtkComponentNode()->Component->UldManager.NodeList[3]->GetAsAtkTextNode()->NodeText).ExtractText().Trim();
                if (text == "") break;
                arr.Add(text);
            }
            return [.. arr];
        }
        return Array.Empty<string>();
    }

    internal static bool IsWorldChangeAetheryte(this TinyAetheryte t)
    {
        return t.ID.EqualsAny<uint>(2, 8, 9);
    }

    internal static TinyAetheryte GetMaster()
    {
        return P.ActiveAetheryte.Value.IsAetheryte ? P.ActiveAetheryte.Value : P.DataStore.GetMaster(P.ActiveAetheryte.Value);
    }

    internal static IGameObject GetValidAetheryte()
    {
        foreach (var x in Svc.Objects)
        {
            if (x.IsAetheryte())
            {
                var d2d = Vector2.Distance(Svc.ClientState.LocalPlayer.Position.ToVector2(), x.Position.ToVector2());
                var d3d = Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position);
                if (P.ResidentialAethernet.IsInResidentialZone() && d3d > 4.6f) continue;

                if (d2d < 11f
                    && d3d < 15f
                    && x.IsVPosValid()
                    && x.IsTargetable)
                {
                    return x;
                }
            }
        }
        return null;
    }

    public static bool IsAetheryte(this IGameObject obj)
    {
        if (obj.ObjectKind == ObjectKind.Aetheryte) return true;
        return AethernetShards.Contains(obj.DataId);
    }

    internal static bool IsVPosValid(this IGameObject x)
    {
        /*if(x.Name.ToString() == Lang.AethernetShard)
        {
            return MathF.Abs(Player.Object.Position.Y - x.Position.Y) < 0.965;
        }*/
        return true;
    }

    internal static AetheryteUseState CanUseAetheryte()
    {
        if (P.TaskManager.IsBusy || GenericHelpers.IsOccupied() || IsDisallowedToUseAethernet()) return AetheryteUseState.None;
        if (P.DataStore.Territories.Contains(P.Territory) && P.ActiveAetheryte != null) return AetheryteUseState.Normal;
        if (P.ResidentialAethernet.IsInResidentialZone() && P.ResidentialAethernet.ActiveAetheryte != null) return AetheryteUseState.Residential;
        return AetheryteUseState.None;
    }

    public static bool? WaitForScreen() => GenericHelpers.IsScreenReady();

    internal static bool TrySelectSpecificEntry(string text, Func<bool> Throttle)
    {
        return TrySelectSpecificEntry(new string[] { text }, Throttle);
    }

    internal static bool TrySelectSpecificEntry(IEnumerable<string> text, Func<bool> Throttle)
    {
        if (GenericHelpers.TryGetAddonByName<AddonSelectString>("SelectString", out var addon) && GenericHelpers.IsAddonReady(&addon->AtkUnitBase))
        {
            var entry = GetEntries(addon).FirstOrDefault(x => x.EqualsAny(text));
            if (entry != null)
            {
                var index = GetEntries(addon).IndexOf(entry);
                if (index >= 0 && IsSelectItemEnabled(addon, index) && Throttle())
                {
                    new SelectStringMaster(addon).Entries[index].Select();
                    PluginLog.Debug($"TrySelectSpecificEntry: selecting {entry}/{index} as requested by {text.Print()}");
                    return true;
                }
            }
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

    internal static List<string> GetEntries(AddonSelectString* addon)
    {
        var list = new List<string>();
        for (var i = 0; i < addon->PopupMenu.PopupMenu.EntryCount; i++)
        {
            list.Add(MemoryHelper.ReadSeStringNullTerminated((nint)addon->PopupMenu.PopupMenu.EntryNames[i]).ExtractText().Trim());
        }
        //PluginLog.Debug($"{list.Print()}");
        return list;
    }

    internal static string GetDataCenter(string world)
    {
        return Svc.Data.GetExcelSheet<World>().First(x => x.Name == world).DataCenter.Value.Name.ToString();
    }

    internal static bool CanAutoLogin()
    {
        return !Svc.ClientState.IsLoggedIn
            && !Svc.Condition.Any()
            && TryGetAddonByName<AtkUnitBase>("_TitleMenu", out var title)
            && IsAddonReady(title)
            && title->UldManager.NodeListCount > 3
            && title->UldManager.NodeList[3]->Color.A == 0xFF
            && !TryGetAddonByName<AtkUnitBase>("TitleDCWorldMap", out _)
            && !TryGetAddonByName<AtkUnitBase>("TitleConnect", out _);
    }

    internal static bool TryGetCharacterIndex(string name, uint world, out int index)
    {
        index = GetCharacterNames().IndexOf((name, (ushort)world));
        return index >= 0;
    }


    internal static List<CharaData> GetCharacterNames()
    {
        List<CharaData> ret = [];
        /*var data = CSFramework.Instance()->UIModule->GetRaptureAtkModule()->AtkModule.GetStringArrayData(1);
        if (data != null)
        {
            for (int i = 60; i < data->AtkArrayData.Size; i++)
            {
                if (data->StringArray[i] == null) break;
                var item = data->StringArray[i];
                if (item != null)
                {
                    var str = MemoryHelper.ReadSeStringNullTerminated((nint)item).ExtractText();
                    if (str == "") break;
                    ret.Add(str);
                }
            }
        }*/
        var agent = AgentLobby.Instance();
        //if (agent->AgentInterface.IsAgentActive())
        {
            var charaSpan = agent->LobbyData.CharaSelectEntries.AsSpan();
            for (var i = 0; i < charaSpan.Length; i++)
            {
                var s = charaSpan[i];
                ret.Add(($"{s.Value->Name.Read()}", s.Value->HomeWorldId));
            }
        }
        return ret;
    }

    internal static DateTime DateFromTimeStamp(uint timeStamp)
    {
        const long timeFromEpoch = 62135596800;
        return timeStamp == 0u
            ? DateTime.MinValue
            : new DateTime((timeStamp + timeFromEpoch) * TimeSpan.TicksPerSecond, DateTimeKind.Utc);
    }

}

