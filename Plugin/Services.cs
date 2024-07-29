using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Plugin.OtherServices;

namespace Plugin;

public class Services
{
    public static IServiceCollection Collection { get; } = new ServiceCollection();

    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;

    public static TextureService TextureService { get; private set; } = null!;

    internal static bool IsInitialized = false;
    public static void Init(IDalamudPluginInterface pluginInterface)
    {
        if (IsInitialized)
        {
            PluginLog.Debug("Service already initialized, skipping");
        }
        IsInitialized = true;
        try
        {
            pluginInterface.Create<Services>();
            TextureService = new TextureService(TextureProvider, DataManager);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex.ToString());
        }
    }
}
