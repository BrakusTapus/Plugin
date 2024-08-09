using Plugin.Windows;
using Plugin.Commands;
using ECommons.Configuration;
using ECommons.Automation.NeoTaskManager;
using ECommons.Singletons;
using Plugin.Configuration;
using Plugin.Windows.AlphaMainWindow;
using MyServices;
using Plugin.Data;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Plugin.External;
using Plugin.Features;
using Plugin.FeaturesSetup;
using System.Reflection;
using Plugin.FeaturesSetup.Attributes;
using System.Collections.Specialized;
using Plugin.Utilities.Movement;
using Plugin.Utilities.Enums;
using Plugin.Utilities;
using Plugin.Tasks.SameWorld;
using Plugin.Tasks;
using Plugin.Tasks.CrossWorld;
using Plugin.Tasks.CrossDC;
using Plugin.Schedulers;
using ECommons.Automation.NeoTaskManager.Tasks;
using CharaData = (string Name, ushort World);


namespace Plugin;

public sealed class Plugin : IDalamudPlugin
{
    internal string Name => "Kirbo";
    internal static Plugin P { get; private set; }
    internal bool Started = false;
    internal bool Running = false;
    internal IPlayerCharacter? Player = null;
    internal Vector3 PlayerPosition = Vector3.Zero;
    internal IBattleChara? BossObject;
    internal IGameObject? ClosestInteractableEventObject = null;
    internal IGameObject? ClosestTargetableBattleNpc = null;
    internal string Action = "";
    internal OverrideCamera OverrideCamera;


    internal bool InDungeon = false;

    //internal FollowPath followPath = null;
    //public FollowPath FollowPath
    //{
    //    get
    //    {
    //        followPath ??= new();
    //        return followPath;
    //    }
    //}








    internal Game.Memory Memory;

    private readonly Configs EzConfigs;
    public static Configs Config => P.EzConfigs;

    public static Configs C => P.EzConfigs;
    public static readonly HashSet<Tweak> Tweaks = [];
    public TaskManager TaskManager;



    internal DataStore DataStore; // TODO: Plugin
    internal TinyAetheryte? ActiveAetheryte = null;  // TODO: Plugin


    internal uint Territory => Svc.ClientState.TerritoryType;

    public ResidentialAethernet ResidentialAethernet; // TODO: Plugin

    private readonly WindowSystem WindowSystem = new("plugin");
    private static ConfigWindow ConfigWindow; // { get; init; }
    private static MainWindow MainWindow; //{ get; init; }
    private static TestWindow TestWindow;


    internal static AutoAdjustRetainerListings autoAdjustRetainerListings;


    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        P = this;

        Services.Initialize(pluginInterface);
        pluginInterface.Create<SimpleLog>();
        ECommonsMain.Init(pluginInterface, this, ECommons.Module.ObjectFunctions);

        PluginCommands.Enable(this);

        EzConfigs = EzConfig.Init<Configs>();
        TaskManager = new();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        TestWindow = new(this, EzConfigs);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(TestWindow);

        //Svc.Framework.Update += Framework_Update;
        Memory = new();
        Services.PluginInterface.UiBuilder.Draw += DrawWindows;
        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
        Services.PluginLog.Debug("plugin was loaded!");

        SingletonServiceManager.Initialize(typeof(ServiceStatic));

        Svc.Framework.RunOnFrameworkThread(InitializeTweaks);
        C.EnabledTweaks.CollectionChanged += OnChange;
        autoAdjustRetainerListings = new AutoAdjustRetainerListings();
    }

    public static void OnChange(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var t in Tweaks)
        {
            if (C.EnabledTweaks.Contains(t.InternalName) && !t.Enabled)
                GenericHelpers.TryExecute(t.EnableInternal);
            else if (!C.EnabledTweaks.Contains(t.InternalName) && t.Enabled || t.Enabled && t.IsDebug && !C.ShowDebug)
                t.DisableInternal();
            EzConfig.Save();
        }
    }

    public void Dispose()
    {
        //Svc.Framework.Update -= Framework_Update;

        foreach (var tweak in Tweaks)
        {
            Svc.Log.Debug($"Disposing {tweak.InternalName}");
            GenericHelpers.TryExecute(tweak.DisposeInternal);
        }
        C.EnabledTweaks.CollectionChanged -= OnChange;

        Memory.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        PluginCommands.Disable();

        ECommonsMain.Dispose();
    }

    private void InitializeTweaks()
    {
        foreach (var tweakType in GetType().Assembly.GetTypes().Where(type => type.Namespace == "Automaton.Features" && type.GetCustomAttribute<TweakAttribute>() != null))
        {
            Svc.Log.Verbose($"Initializing {tweakType.Name}");
            GenericHelpers.TryExecute(() => Tweaks.Add((Tweak)Activator.CreateInstance(tweakType)!));
        }

        foreach (var tweak in Tweaks)
        {
            if (!C.EnabledTweaks.Contains(tweak.InternalName))
                continue;

            if (C.EnabledTweaks.Contains(tweak.InternalName) && tweak.IsDebug)
                C.EnabledTweaks.Remove(tweak.InternalName);

            GenericHelpers.TryExecute(tweak.EnableInternal);
        }
    }

    private void DrawWindows() => WindowSystem.Draw();

    public static void ToggleMainWindow()
    {
        MainWindow.Toggle();

        if (!MainWindow.IsOpen)
        {
            EzConfig.Save();
        }
    }

    public static void ToggleConfigWindow()
    {
        ConfigWindow.Toggle();

        if (!ConfigWindow.IsOpen)
        {
            EzConfig.Save();
        }
    }

    public static void ToggleTestWindow()
    {
        TestWindow.Toggle();

        if (!TestWindow.IsOpen)
        {
            EzConfig.Save();
        }
    }

    public void Run(uint territoryType = 0, int loops = 0)
    {
        Svc.Log.Debug($"Run: territoryType={territoryType} loops={loops}");
        //if (territoryType > 0)
        //{
        //    if (ContentHelper.DictionaryContent.TryGetValue(territoryType, out var content))
        //        CurrentTerritoryContent = content;
        //    else
        //    {
        //        Svc.Log.Error($"({territoryType}) is not in our Dictionary as a compatible Duty");
        //        return;
        //    }
        //}

        //if (loops > 0)
        //    Configuration.LoopTimes = loops;

        //if (CurrentTerritoryContent == null)
        //    return;

        ////MainWindow.OpenTab("Mini");
        //if (Configuration.OpenOverlay)
        //{
        //    MainWindow.IsOpen = false;
        //    Overlay.IsOpen = true;
        //}
        //Stage = 99;
        //Running = true;
        //TaskManager.Abort();
        //Svc.Log.Info($"Running {CurrentTerritoryContent.DisplayName} {Configuration.LoopTimes} Times");
        //if (!InDungeon)
        //{
        //    if (Configuration.AutoRepair && InventoryHelper.CanRepair())
        //    {
        //        TaskManager.Enqueue(() => RepairHelper.Invoke(), "Run-AutoRepair");
        //        TaskManager.DelayNext("Run-Delay50", 50);
        //        TaskManager.Enqueue(() => !RepairHelper.RepairRunning, int.MaxValue, "Run-WaitAutoRepairComplete");
        //        TaskManager.Enqueue(() => !ObjectHelper.IsOccupied, "Run-WaitANotIsOccupied");
        //    }
        //    if (!Configuration.Squadron)
        //    {
        //        if (Configuration.RetireToBarracksBeforeLoops)
        //            TaskManager.Enqueue(() => GotoBarracksHelper.Invoke(), "Run-GotoBarracksInvoke");
        //        else if (Configuration.RetireToInnBeforeLoops)
        //            TaskManager.Enqueue(() => GotoInnHelper.Invoke(), "Run-GotoInnInvoke");
        //        TaskManager.DelayNext("Run-Delay50", 50);
        //        TaskManager.Enqueue(() => !GotoBarracksHelper.GotoBarracksRunning && !GotoInnHelper.GotoInnRunning, int.MaxValue, "Run-WaitGotoComplete");
        //    }
        //    if (Configuration.Trust)
        //        _trustManager.RegisterTrust(CurrentTerritoryContent);
        //    else if (Configuration.Support)
        //        _dutySupportManager.RegisterDutySupport(CurrentTerritoryContent);
        //    else if (Configuration.Variant)
        //        _variantManager.RegisterVariantDuty(CurrentTerritoryContent);
        //    else if (Configuration.Regular || Configuration.Trial || Configuration.Raid)
        //    {
        //        TaskManager.Enqueue(() => QueueHelper.Invoke(CurrentTerritoryContent), "Run-Queue");
        //        TaskManager.DelayNext("Run-Delay50", 50);
        //        TaskManager.Enqueue(() => !QueueHelper.QueueRunning, int.MaxValue, "Run-WaitQueueComplete");
        //    }
        //    else if (Configuration.Squadron)
        //    {
        //        TaskManager.Enqueue(() => GotoBarracksHelper.Invoke(), "Run-GotoBarracksInvoke");
        //        TaskManager.DelayNext("Run-Delay50", 50);
        //        TaskManager.Enqueue(() => !GotoBarracksHelper.GotoBarracksRunning && !GotoInnHelper.GotoInnRunning, int.MaxValue, "Run-WaitGotoComplete");
        //        _squadronManager.RegisterSquadron(CurrentTerritoryContent);
        //    }
        //    TaskManager.Enqueue(() => !ObjectHelper.IsValid, "Run");
        //    TaskManager.Enqueue(() => ObjectHelper.IsValid, int.MaxValue, "Run");
        //}
        //TaskManager.Enqueue(() => Svc.DutyState.IsDutyStarted, int.MaxValue, "Run");
        //TaskManager.Enqueue(() => VNavmesh_IPCSubscriber.Nav_IsReady(), int.MaxValue, "Run");
        //TaskManager.Enqueue(() => StartNavigation(true), "Run");
        //CurrentLoop = 1;
    }

    public void StartNavigation(bool startFromZero = true)
    {
        Svc.Log.Debug($"StartNavigation: startFromZero={startFromZero}");
        //if (ContentHelper.DictionaryContent.TryGetValue(Svc.ClientState.TerritoryType, out var content))
        //{
        //    CurrentTerritoryContent = content;
        //    PathFile = $"{Plugin.PathsDirectory.FullName}/({Svc.ClientState.TerritoryType}) {content.Name?.Replace(":", "")}.json";
        //    LoadPath();
        //}
        //else
        //{
        //    CurrentTerritoryContent = null;
        //    PathFile = "";
        //    MainWindow.ShowPopup("Error", "Unable to load content for Territory");
        //    return;
        //}
        ////MainWindow.OpenTab("Mini");
        //if (Configuration.OpenOverlay)
        //{
        //    MainWindow.IsOpen = false;
        //    Overlay.IsOpen = true;
        //}
        //MainListClicked = false;
        //Stage = 1;
        //Started = true;
        //StopForCombat = true;
        //_chat.ExecuteCommand($"/vnav aligncamera enable");
        //_chat.ExecuteCommand($"/vbm cfg AIConfig Enable true");
        //_chat.ExecuteCommand($"/vbmai on");
        //if (Configuration.AutoManageBossModAISettings)
        //    SetBMSettings();
        //if (Configuration.AutoManageRSRState && !Configuration.UsingAlternativeRotationPlugin)
        //    ReflectionHelper.RotationSolver_Reflection.RotationAuto();
        //Svc.Log.Info("Starting Navigation");
        //if (startFromZero)
        //    Indexer = 0;
    }

    internal void StopAndResetALL()
    {
        //Running = false;
        //CurrentLoop = 0;
        //MainListClicked = false;
        //Started = false;
        //Stage = 0;
        //CurrentLoop = 0;
        //_chat.ExecuteCommand($"/vbmai off");
        //_chat.ExecuteCommand($"/vbm cfg AIConfig Enable false");
        //if (Indexer > 0 && !MainListClicked)
        //    Indexer = -1;
        //if (Configuration.OpenOverlay && Configuration.OnlyOpenOverlayWhenRunning)
        //{
        //    Overlay.IsOpen = false;
        //    MainWindow.IsOpen = true;
        //}
        //if (VNavmesh_IPCSubscriber.IsEnabled && VNavmesh_IPCSubscriber.Path_GetTolerance() > 0.25F)
        //    VNavmesh_IPCSubscriber.Path_SetTolerance(0.25f);
        //if (TaskManager.IsBusy)
        //    TaskManager.Abort();
        //FollowHelper.SetFollow(null);
        //if (ExtractHelper.ExtractRunning)
        //    ExtractHelper.Stop();
        //if (GCTurninHelper.GCTurninRunning)
        //    GCTurninHelper.Stop();
        //if (DesynthHelper.DesynthRunning)
        //    DesynthHelper.Stop();
        //if (GotoHelper.GotoRunning)
        //    GotoHelper.Stop();
        //if (GotoInnHelper.GotoInnRunning)
        //    GotoInnHelper.Stop();
        //if (GotoBarracksHelper.GotoBarracksRunning)
        //    GotoBarracksHelper.Stop();
        //if (RepairHelper.RepairRunning)
        //    RepairHelper.Stop();
        //if (QueueHelper.QueueRunning)
        //    QueueHelper.Stop();
        //if (VNavmesh_IPCSubscriber.IsEnabled && VNavmesh_IPCSubscriber.Path_IsRunning())
        //    VNavmesh_IPCSubscriber.Path_Stop();
        //Action = "";
    }



    internal enum WorldChangeAetheryte
    {
        Uldah = 9,
        Gridania = 2,
        Limsa = 8,
    }

    //internal void TPAndChangeWorld(string w, bool isDcTransfer = false, string secondaryTeleport = null, bool noSecondaryTeleport = false, WorldChangeAetheryte? gateway = null, bool? doNotify = null, bool? returnToGateway = null)
    //{
    //    try
    //    {
    //        returnToGateway ??= C.DCReturnToGateway;
    //        gateway ??= C.WorldChangeAetheryte;
    //        doNotify ??= true;
    //        if (secondaryTeleport == null && C.WorldVisitTPToAethernet && !C.WorldVisitTPTarget.IsNullOrEmpty())
    //        {
    //            secondaryTeleport = C.WorldVisitTPTarget;
    //        }
    //        if (isDcTransfer && !C.AllowDcTransfer)
    //        {
    //            Notify.Error($"Data center transfers are not enabled in the configuration.");
    //            return;
    //        }
    //        if (TaskManager.IsBusy)
    //        {
    //            Notify.Error("Another task is in progress");
    //            return;
    //        }
    //        if (!ECommons.GameHelpers.Player.Available)
    //        {
    //            Notify.Error("No player");
    //            return;
    //        }
    //        if (w == ECommons.GameHelpers.Player.CurrentWorld)
    //        {
    //            Notify.Error("Already in this world");
    //            return;
    //        }
    //        /*if(ActionManager.Instance()->GetActionStatus(ActionType.Spell, 5) != 0)
    //        {
    //            Notify.Error("You are unable to teleport at this time");
    //            return;
    //        }*/
    //        if (Svc.Party.Length > 1 && !C.LeavePartyBeforeWorldChange && !C.LeavePartyBeforeWorldChange)
    //        {
    //            Notify.Warning("You must disband party in order to switch worlds");
    //        }
    //        Utils.DisplayInfo($"Destination: {w}");
    //        if (isDcTransfer)
    //        {
    //            var type = DCVType.Unknown;
    //            var homeDC = ECommons.GameHelpers.Player.Object.HomeWorld.GameData.DataCenter.Value.Name.ToString();
    //            var currentDC = ECommons.GameHelpers.Player.Object.CurrentWorld.GameData.DataCenter.Value.Name.ToString();
    //            var targetDC = Utils.GetDataCenter(w);
    //            if (currentDC == homeDC)
    //            {
    //                type = DCVType.HomeToGuest;
    //            }
    //            else
    //            {
    //                if (targetDC == homeDC)
    //                {
    //                    type = DCVType.GuestToHome;
    //                }
    //                else
    //                {
    //                    type = DCVType.GuestToGuest;
    //                }
    //            }
    //            TaskRemoveAfkStatus.Enqueue();
    //            if (type != DCVType.Unknown)
    //            {
    //                if (Config.TeleportToGatewayBeforeLogout && !(TerritoryInfo.Instance()->InSanctuary || ExcelTerritoryHelper.IsSanctuary(Svc.ClientState.TerritoryType)) && !(currentDC == homeDC && Player.HomeWorld != Player.CurrentWorld))
    //                {
    //                    TaskTpToAethernetDestination.Enqueue(gateway.Value);
    //                }
    //                if (Config.LeavePartyBeforeLogout && (Svc.Party.Length > 1 || Svc.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance]))
    //                {
    //                    TaskManager.EnqueueTask(new(WorldChange.LeaveAnyParty));
    //                }
    //            }
    //            if (type == DCVType.HomeToGuest)
    //            {
    //                if (!ECommons.GameHelpers.Player.IsInHomeWorld) TaskTPAndChangeWorld.Enqueue(ECommons.GameHelpers.Player.HomeWorld, gateway.Value, false);
    //                TaskWaitUntilInHomeWorld.Enqueue();
    //                TaskLogoutAndRelog.Enqueue(ECommons.GameHelpers.Player.NameWithWorld);
    //                TaskChangeDatacenter.Enqueue(w, ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskSelectChara.Enqueue(ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskWaitUntilInWorld.Enqueue(w);

    //                if (gateway != null && returnToGateway == true) TaskReturnToGateway.Enqueue(gateway.Value);
    //                if (doNotify == true) TaskDesktopNotification.Enqueue($"Arrived to {w}");
    //                EnqueueSecondary();
    //            }
    //            else if (type == DCVType.GuestToHome)
    //            {
    //                TaskLogoutAndRelog.Enqueue(ECommons.GameHelpers.Player.NameWithWorld);
    //                TaskReturnToHomeDC.Enqueue(ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskSelectChara.Enqueue(ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                if (ECommons.GameHelpers.Player.HomeWorld != w)
    //                {
    //                    TaskManager.EnqueueMulti([
    //                        new(WorldChange.WaitUntilNotBusy, TaskSettings.TimeoutInfinite),
    //                    new DelayTask(1000),
    //                    new(() => TaskTPAndChangeWorld.Enqueue(w, gateway.Value, true), $"TpAndChangeWorld {w} at {gateway.Value}"),
    //                    ]);
    //                }
    //                else
    //                {
    //                    TaskWaitUntilInWorld.Enqueue(w);
    //                }
    //                if (gateway != null && returnToGateway == true) TaskReturnToGateway.Enqueue(gateway.Value);
    //                if (doNotify == true) TaskDesktopNotification.Enqueue($"Arrived to {w}");
    //                EnqueueSecondary();
    //            }
    //            else if (type == DCVType.GuestToGuest)
    //            {
    //                TaskLogoutAndRelog.Enqueue(ECommons.GameHelpers.Player.NameWithWorld);
    //                TaskReturnToHomeDC.Enqueue(ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskChangeDatacenter.Enqueue(w, ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskSelectChara.Enqueue(ECommons.GameHelpers.Player.Name, ECommons.GameHelpers.Player.Object.HomeWorld.Id);
    //                TaskWaitUntilInWorld.Enqueue(w);
    //                if (gateway != null && returnToGateway == true) TaskReturnToGateway.Enqueue(gateway.Value);
    //                TaskDesktopNotification.Enqueue($"Arrived to {w}");
    //                EnqueueSecondary();
    //            }
    //            else
    //            {
    //                DuoLog.Error($"Error - unknown data center visit type");
    //            }
    //            PluginLog.Information($"Data center visit: {type}");
    //        }
    //        else
    //        {
    //            TaskRemoveAfkStatus.Enqueue();
    //            TaskTPAndChangeWorld.Enqueue(w, gateway.Value, false);
    //            if (doNotify == true) TaskDesktopNotification.Enqueue($"Arrived to {w}");
    //            EnqueueSecondary();
    //        }

    //        void EnqueueSecondary()
    //        {
    //            if (noSecondaryTeleport) return;
    //            if (!secondaryTeleport.IsNullOrEmpty())
    //            {
    //                TaskManager.EnqueueMulti([
    //                    new(() => ECommons.GameHelpers.Player.Interactable),
    //                new(() => TaskTryTpToAethernetDestination.Enqueue(secondaryTeleport))
    //                    ]);
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        e.Log();
    //    }
    //}

}
