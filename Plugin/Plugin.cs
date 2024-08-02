using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Plugin.Windows;
//using Plugin.Configurations; //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
using Plugin.Commands;
using ECommons;
using Plugin.Utility;
using System.Collections.Generic;
using Plugin.MyServices;
using ECommons.Configuration;
using Plugin.Utility.Data;
using ECommons.Automation.NeoTaskManager;
using ECommons.Singletons;

namespace Plugin;

public sealed class Plugin : IDalamudPlugin
{
    internal static Plugin P;
    //public Configs Config { get; init; } //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
    internal EzConfigs EzConfigs;
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
    private static AlphaMainWindow AlphaMainWindow;

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        MyServices.Services.Initialize(pluginInterface);
        pluginInterface.Create<SimpleLog>();
        ECommonsMain.Init(pluginInterface, this, Module.ObjectFunctions);

        PluginCommands.Enable(this);

        //Config = MyServices.Services.PluginInterface.GetPluginConfig() as Configs ?? new Configs(P); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        EzConfigs = EzConfig.Init<EzConfigs>();
        TaskManager = new();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this/*, this.Config //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed */);
        AlphaMainWindow = new(this);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(AlphaMainWindow);

        //Svc.Framework.Update += Framework_Update;
        Memory = new();
        MyServices.Services.PluginInterface.UiBuilder.Draw += DrawWindows;
        MyServices.Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        MyServices.Services.PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
        MyServices.Services.PluginLog.Debug("plugin was loaded!");

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
        //Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed

        ECommonsMain.Dispose();
    }

    private void DrawWindows() => WindowSystem.Draw();
    public static void ToggleConfigWindow()
    {
        AlphaMainWindow.Toggle();

        if (!AlphaMainWindow.IsOpen)
        {
            EzConfig.Save();
            //this.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        //ConfigWindow.Toggle();

        //if (!ConfigWindow.IsOpen)
        //{
        //    EzConfig.Save();
        //    //this.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        //}
    }

    public static void ToggleMainWindow()
    {
        AlphaMainWindow.Toggle();

        if (!AlphaMainWindow.IsOpen)
        {
            EzConfig.Save();
            //this.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }
        //MainWindow.Toggle();

        //if (!MainWindow.IsOpen)
        //{
        //    EzConfig.Save();
        //    //this.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        //}
    }

}
