using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Plugin.Windows;
using Plugin.Configurations;
using Plugin.Commands;
using ECommons;

namespace Plugin;

public sealed class Plugin : IDalamudPlugin
{
    public string LocalImagesPath { get; }

    public Configs Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("plugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Services.Init(pluginInterface);
        PluginCommands.Enable(this);
        Configuration = Services.PluginInterface.GetPluginConfig() as Configs ?? new Configs();

        ECommonsMain.Init(pluginInterface, this, Module.ObjectFunctions);

        string? uiImagesPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, "UI");
        string? localImagesPath = uiImagesPath;
        LocalImagesPath = localImagesPath;
        string? hitpointsImagePath = Path.Combine(uiImagesPath, "skill", "hitpoints.png");
        string? pluginImagePath = Path.Combine(uiImagesPath, "plugin.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, hitpointsImagePath, pluginImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        Services.PluginInterface.UiBuilder.Draw += DrawUI;
        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        Services.PluginLog.Debug("plugin was loaded!");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        PluginCommands.Disable();
        Configuration.Save();

        ECommonsMain.Dispose();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleConfigUI()
    {
        ConfigWindow.Toggle();
        if (!ConfigWindow.IsOpen)
        {
            Configuration.Save();
        }
    }
    public void ToggleMainUI()
    {
        MainWindow.Toggle();
        if (!MainWindow.IsOpen)
        {
            Configuration.Save();
        }
    }
}
