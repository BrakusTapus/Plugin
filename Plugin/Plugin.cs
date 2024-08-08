using Plugin.Windows;
using Plugin.Commands;
using ECommons.Configuration;
using Plugin.Utility.Data;
using ECommons.Automation.NeoTaskManager;
using ECommons.Singletons;
using Plugin.Configuration;
using Plugin.Windows.AlphaMainWindow;
using MyServices;

namespace Plugin;

public sealed class Plugin : IDalamudPlugin
{
    internal static Plugin P;
    internal Configs EzConfigs;
    internal Game.Memory Memory;


    internal DataStore DataStore; // TODO: LifeSTREAM
    internal TinyAetheryte? ActiveAetheryte = null;  // TODO: LifeSTREAM


    internal uint Territory => Svc.ClientState.TerritoryType;

    public TaskManager TaskManager;
    public ResidentialAethernet ResidentialAethernet; // TODO: LifeSTREAM
    //internal FollowPath followPath = null;

    private readonly WindowSystem WindowSystem = new("plugin");
    private static ConfigWindow ConfigWindow; // { get; init; }
    private static MainWindow MainWindow; //{ get; init; }
    private static TestWindow AlphaMainWindow;

    public Plugin(IDalamudPluginInterface pluginInterface/*, Configs configs*/)
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
        AlphaMainWindow = new(this, EzConfigs);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(AlphaMainWindow);

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
        AlphaMainWindow.Toggle();

        if (!AlphaMainWindow.IsOpen)
        {
            EzConfig.Save();
        }
    }

}
