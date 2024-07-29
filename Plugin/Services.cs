using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Utility.OtherServices;
using Plugin.Utility.UI;

namespace Plugin;

public class Services
{
    public static TextureService TextureService { get; private set; } = null!;
    public static UiPaths UiPaths { get; private set; } = null!;

    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;

    internal static bool IsInitialized = false;

    public static void Initialize(IDalamudPluginInterface pluginInterface)
    {
        if (IsInitialized)
        {
            PluginLog.Warning("Service.cs -> Service already initialized, skipping");
            return;
        }

        IsInitialized = true;
        try
        {
            pluginInterface.Create<Services>();

            string uiPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "UI");
            UiPaths = new UiPaths(uiPath);

            TextureService = new TextureService(TextureProvider, DataManager, uiPath);

            PluginLog.Debug("TextureServices successfully initialized.");
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Error initializing Services: {ex}");
        }
    }
}
