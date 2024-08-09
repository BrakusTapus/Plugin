using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using ECommons.EzHookManager;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Plugin.Utilities.Enums;
using Plugin.Tasks.SameWorld;
//using PInvoke;
using System.Windows.Forms;
using PInvoke;
using Plugin.Helpers;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Runtime.InteropServices;
using Plugin.Utilities;

namespace Plugin.Game;

internal unsafe class Memory : IDisposable
{
    private delegate long AddonAreaMap_ReceiveEvent(long a1, ushort a2, uint a3, long a4, long a5);
    [Signature("40 55 56 57 48 8B EC 48 83 EC 70 0F B7 C2", DetourName = nameof(AddonAreaMap_ReceiveEventDetour), Fallibility = Fallibility.Fallible)]
    private Hook<AddonAreaMap_ReceiveEvent> AddonAreaMap_ReceiveEventHook = null!;
    private bool IsLeftMouseHeld = false;

    internal delegate void AddonDKTWorldList_ReceiveEventDelegate(nint a1, short a2, nint a3, AtkEvent* a4, InputData* a5);
    [Signature("40 53 48 83 EC 20 F6 81 ?? ?? ?? ?? ?? 49 8B D9 41 8B C0", DetourName = nameof(AddonDKTWorldList_ReceiveEventDetour))]
    internal Hook<AddonDKTWorldList_ReceiveEventDelegate> AddonDKTWorldList_ReceiveEventHook;

    internal delegate void AtkComponentTreeList_vf31(nint a1, uint a2, byte a3);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B DA 41 0F B6 F0", DetourName = nameof(AtkComponentTreeList_vf31Detour))]
    internal Hook<AtkComponentTreeList_vf31> AtkComponentTreeList_vf31Hook;

    [Signature("4C 8D 0D ?? ?? ?? ?? 4C 8B 11 48 8B D9", ScanType = ScanType.StaticAddress)]
    internal int* MaxInstances;

    private void AtkComponentTreeList_vf31Detour(nint a1, uint a2, byte a3)
    {
        PluginLog.Debug($"AtkComponentTreeList_vf31Detour: {a1:X16}, {a2}, {a3}");
        AtkComponentTreeList_vf31Hook.Original(a1, a2, a3);
    }

    private void AddonDKTWorldList_ReceiveEventDetour(nint a1, short a2, nint a3, AtkEvent* a4, InputData* a5)
    {
        PluginLog.Debug($"AddonDKTWorldCheck_ReceiveEventDetour: {a1:X16}, {a2}, {a3:X16}, {(nint)a4:X16}, {(nint)a5:X16}");
        PluginLog.Debug($"  Event: {(nint)a4->Node:X16}, {(nint)a4->Target:X16}, {(nint)a4->Listener:X16}, {a4->Param}, {(nint)a4->NextEvent:X16}, {a4->Type}, {a4->Unk29}, {a4->Flags}");
        PluginLog.Debug($"  Data: {(nint)a5->unk_8:X16}({*a5->unk_8:X16}/{*a5->unk_8:X16}), [{a5->unk_8s->unk_4}/{a5->unk_8s->SelectedItem}] {a5->unk_16}, {a5->unk_24} | "); //{a5->RawDumpSpan.ToArray().Print()}
        //var span = new Span<byte>((void*)*a5->unk_8, 0x40).ToArray().Select(x => $"{x:X2}");
        //PluginLog.Debug($"  Data 2, {a5->unk_8s->unk_4}, {MemoryHelper.ReadRaw((nint)a5->unk_8s->CategorySelection, 4).Print(",")},  :{string.Join(" ", span)}");
        AddonDKTWorldList_ReceiveEventHook.Original(a1, a2, a3, a4, a5);
    }

    internal void ConstructEvent(AtkUnitBase* addon, int category, int which, int nodeIndex, int itemToSelect, int itemToHighlight)
    {
        if (itemToSelect == 0) throw new Exception("Enumeration starts with 1");
        var Event = stackalloc AtkEvent[1]
        {
            new AtkEvent()
            {
                Node = null,
                Target = (AtkEventTarget*)addon->UldManager.NodeList[nodeIndex],
                Listener = &addon->UldManager.NodeList[nodeIndex]->GetAsAtkComponentNode()->Component->AtkEventListener,
                Param = 1,
                NextEvent = null,
                Type = AtkEventType.ListItemToggle,
                Unk29 = 0,
                Flags = 0,
            }
        };
        var Unk = stackalloc UnknownStruct[1]
        {
            new()
            {
                unk_4 = 1,
                SelectedItem = itemToSelect - 1 + (category << 8)
            }
        };
        var ptr = stackalloc nint[1]
        {
            (nint)Unk
        };
        var Data = stackalloc InputData[1]
        {
            new InputData()
            {
                unk_8 = ptr,
                unk_16 = itemToSelect,
                unk_24 = 0,
            }
        };
        AddonDKTWorldList_ReceiveEventDetour((nint)addon, 35, which, Event, Data);
        AtkComponentTreeList_vf31Detour((nint)addon->UldManager.NodeList[nodeIndex]->GetAsAtkComponentList(), (uint)itemToHighlight, 0);
    }


    public unsafe delegate void RidePillionDelegate(BattleChara* target, int seatIndex);
    public RidePillionDelegate? RidePillion = null!;

    public unsafe delegate void SalvageItemDelegate(AgentSalvage* thisPtr, InventoryItem* item, int addonId, byte a4);
    public SalvageItemDelegate SalvageItem = null!;

    public delegate void AbandonDutyDelegate(bool a1);
    public AbandonDutyDelegate AbandonDuty = null!;

    internal Memory()
    {
        SignatureHelper.Initialise(this);
        EzSignatureHelper.Initialize(this);
        AddonAreaMap_ReceiveEventHook.Enable();
        //AddonDKTWorldList_ReceiveEventHook.Enable();
        RidePillion = Marshal.GetDelegateForFunctionPointer<RidePillionDelegate>(Svc.SigScanner.ScanText("48 85 C9 0F 84 ?? ?? ?? ?? 48 89 6C 24 ?? 56 48 83 EC"));
        SalvageItem = Marshal.GetDelegateForFunctionPointer<SalvageItemDelegate>(Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? EB 46 48 8B 03")); // thanks veyn
        AbandonDuty = Marshal.GetDelegateForFunctionPointer<AbandonDutyDelegate>(Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 43 28 41 B2 01"));
    }


    #region PacketDispatcher
    const string PacketDispatcher_OnReceivePacketHookSig = "40 53 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 8B F2";
    public delegate void PacketDispatcher_OnReceivePacket(nint a1, uint a2, nint a3);
    [EzHook(PacketDispatcher_OnReceivePacketHookSig, false)]
    public EzHook<PacketDispatcher_OnReceivePacket> PacketDispatcher_OnReceivePacketHook = null!;
    [EzHook(PacketDispatcher_OnReceivePacketHookSig, false)]
    public EzHook<PacketDispatcher_OnReceivePacket> PacketDispatcher_OnReceivePacketMonitorHook = null!;

    internal delegate byte PacketDispatcher_OnSendPacket(nint a1, nint a2, nint a3, byte a4);
    [EzHook("48 89 5C 24 ?? 48 89 74 24 ?? 4C 89 64 24 ?? 55 41 56 41 57 48 8B EC 48 83 EC 70", false)]
    internal EzHook<PacketDispatcher_OnSendPacket> PacketDispatcher_OnSendPacketHook = null!;

    public List<uint> DisallowedSentPackets = [];
    public List<uint> DisallowedReceivedPackets = [];

    private byte PacketDispatcher_OnSendPacketDetour(nint a1, nint a2, nint a3, byte a4)
    {
        const byte DefaultReturnValue = 1;

        if (a2 == IntPtr.Zero)
        {
            PluginLog.Error("[HyperFirewall] Error: Opcode pointer is null.");
            return DefaultReturnValue;
        }

        try
        {
            Events.OnPacketSent(a1, a2, a3, a4);
            var opcode = *(ushort*)a2;

            if (DisallowedSentPackets.Contains(opcode))
            {
                PluginLog.Verbose($"[HyperFirewall] Suppressing outgoing packet with opcode {opcode}.");
            }
            else
            {
                PluginLog.Verbose($"[HyperFirewall] Passing outgoing packet with opcode {opcode} through.");
                return PacketDispatcher_OnSendPacketHook.Original(a1, a2, a3, a4);
            }
        }
        catch (Exception e)
        {
            PluginLog.Error($"[HyperFirewall] Exception caught while processing opcode: {e.Message}");
            e.Log();
            return DefaultReturnValue;
        }

        return DefaultReturnValue;
    }

    private void PacketDispatcher_OnReceivePacketDetour(nint a1, uint a2, nint a3)
    {
        if (a3 == IntPtr.Zero)
        {
            PluginLog.Error("[HyperFirewall] Error: Data pointer is null.");
            return;
        }

        try
        {
            Events.OnPacketRecieved(a1, a2, a3);
            var opcode = *(ushort*)(a3 + 2);

            if (DisallowedReceivedPackets.Contains(opcode))
            {
                PluginLog.Verbose($"[HyperFirewall] Suppressing incoming packet with opcode {opcode}.");
            }
            else
            {
                PluginLog.Verbose($"[HyperFirewall] Passing incoming packet with opcode {opcode} through.");
                PacketDispatcher_OnReceivePacketHook.Original(a1, a2, a3);
            }
        }
        catch (Exception e)
        {
            PluginLog.Error($"[HyperFirewall] Exception caught while processing opcode: {e.Message}");
            e.Log();
            return;
        }

        return;
    }
    #endregion

    #region Bewitch
    public unsafe delegate nint NoBewitchActionDelegate(CSGameObject* gameObj, float x, float y, float z, int a5, nint a6);
    [EzHook("40 53 48 83 EC 50 45 33 C0", false)]
    public readonly EzHook<NoBewitchActionDelegate>? BewitchHook;

    private unsafe nint BewitchDetour(CSGameObject* gameObj, float x, float y, float z, int a5, nint a6)
    {
        try
        {
            if (gameObj->IsCharacter())
            {
                Character* chara = gameObj->Character();
                if (chara->GetStatusManager()->HasStatus(3023) || chara->GetStatusManager()->HasStatus(3024))
                    return nint.Zero;
            }
            return BewitchHook!.Original(gameObj, x, y, z, a5, a6);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex.Message, ex);
            return BewitchHook!.Original(gameObj, x, y, z, a5, a6);
        }
    }
    #endregion

    #region Knockback
    public delegate long kbprocDelegate(long gameobj, float rot, float length, long a4, char a5, int a6);
    [EzHook("E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? FF C6", false)]
    public readonly EzHook<kbprocDelegate>? KBProcHook;

    private long KBProcDetour(long gameobj, float rot, float length, long a4, char a5, int a6) => KBProcHook!.Original(gameobj, rot, 0f, a4, a5, a6);
    #endregion



    private long AddonAreaMap_ReceiveEventDetour(long a1, ushort a2, uint a3, long a4, long a5)
    {
        //DuoLog.Information($"{a1}, {a2}, {a3}, {a4}, {a5}");
        try
        {
            if ((P.ActiveAetheryte != null || P.ResidentialAethernet.ActiveAetheryte != null) && Utils.CanUseAetheryte() != AetheryteUseState.None)
            {
                if (a2 == 3)
                {
                    IsLeftMouseHeld = Bitmask.IsBitSet(User32.GetKeyState((int)Keys.LButton), 15);
                }
                if (a2 == 4 && IsLeftMouseHeld)
                {
                    IsLeftMouseHeld = false;
                    if (!Bitmask.IsBitSet(User32.GetKeyState((int)Keys.ControlKey), 15) && !Bitmask.IsBitSet(User32.GetKeyState((int)Keys.LControlKey), 15) && !Bitmask.IsBitSet(User32.GetKeyState((int)Keys.RControlKey), 15))
                    {
                        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("Tooltip", out var addon) && GenericHelpers.IsAddonReady(addon) && addon->IsVisible)
                        {
                            var node = addon->UldManager.NodeList[2]->GetAsAtkTextNode();
                            var text = MemoryHelper.ReadSeString(&node->NodeText).ExtractText();
                            if (P.ActiveAetheryte != null)
                            {
                                var master = Utils.GetMaster();
                                foreach (var x in P.DataStore.Aetherytes[master])
                                {
                                    if (x.Name == text)
                                    {
                                        TaskAethernetTeleport.Enqueue(x);
                                        break;
                                    }
                                }
                            }
                            if (P.ResidentialAethernet.ActiveAetheryte != null)
                            {
                                var zone = P.ResidentialAethernet.ZoneInfo.SafeSelect(Svc.ClientState.TerritoryType);
                                if (zone != null)
                                {
                                    foreach (var x in zone.Aetherytes)
                                    {
                                        if (x.Name == text)
                                        {
                                            TaskAethernetTeleport.Enqueue(x.Name);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            e.Log();
        }
        return AddonAreaMap_ReceiveEventHook!.Original(a1, a2, a3, a4, a5);
    }

    public void Dispose()
    {
        AddonAreaMap_ReceiveEventHook?.Disable();
        AddonAreaMap_ReceiveEventHook?.Dispose();
        AddonDKTWorldList_ReceiveEventHook?.Disable();
        AddonDKTWorldList_ReceiveEventHook?.Dispose();
        AtkComponentTreeList_vf31Hook?.Disable();
        AtkComponentTreeList_vf31Hook?.Dispose();
    }






}
