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

    internal Game.Memory Memory;

    private readonly Configs EzConfigs;
    public static Configs C => P.EzConfigs;



    internal DataStore DataStore; // TODO: LifeSTREAM
    internal TinyAetheryte? ActiveAetheryte = null;  // TODO: LifeSTREAM


    internal uint Territory => Svc.ClientState.TerritoryType;

    public TaskManager TaskManager;
    public ResidentialAethernet ResidentialAethernet; // TODO: LifeSTREAM

    private readonly WindowSystem WindowSystem = new("plugin");
    private static ConfigWindow ConfigWindow; // { get; init; }
    private static MainWindow MainWindow; //{ get; init; }
    private static TestWindow TestWindow;

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        Services.Initialize(pluginInterface);
        pluginInterface.Create<SimpleLog>();
        ECommonsMain.Init(pluginInterface, this, Module.ObjectFunctions);

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
    }

    public void Dispose()
    {
        //Svc.Framework.Update -= Framework_Update;

        Memory.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        PluginCommands.Disable();

        ECommonsMain.Dispose();
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

}
