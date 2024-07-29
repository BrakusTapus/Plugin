using System;
using System.IO;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Plugin.OtherServices;
using Plugin.Utility.UI;

namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly string HitpointsImagePath;
    private readonly string PluginImagePath;
    private readonly Plugin plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string hitpointsImagePath, string pluginImagePath)
        : base($"{nameof(MainWindow.plugin)}##With a hidden ID", ImGuiWindowFlags.None /*|ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse*/)
    {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        HitpointsImagePath = hitpointsImagePath;
        PluginImagePath = pluginImagePath;
        this.plugin = plugin;
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        if (ImGuiComponents.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.Cog, "Settings", Colours.Transparent, Colours.ButtonActive, Colours.TextHovered))
        {
            plugin.ToggleConfigUI();
        }
        ImGuiExt.NewTooltip("Toggles the config window.");

        ImGui.Spacing();

        IDalamudTextureWrap? hitpointsImage = Services.TextureProvider.GetFromFile(HitpointsImagePath).GetWrapOrDefault();
        if (hitpointsImage != null)
        {
            ImGuiHelpers.ScaledIndent(55f);
            ImGui.Image(hitpointsImage.ImGuiHandle, new Vector2(hitpointsImage.Width, hitpointsImage.Height));
            ImGuiHelpers.ScaledIndent(-55f);
            ImGuiExt.NewTooltip("Have some love:");
        }
        else
        {
            ImGui.Text("HP image not found.");
        }

        IDalamudTextureWrap? pluginImage = Services.TextureProvider.GetFromFile(PluginImagePath).GetWrapOrDefault();
        if (pluginImage != null)
        {
            ImGui.Image(pluginImage.ImGuiHandle, new Vector2(pluginImage.Width, pluginImage.Height));
        }
        else
        {
            ImGui.Text("Plugin image not found.");
        }

        string? image3 = Path.Combine(plugin.LocalImagesPath, "button", "equipment_items_lost_on_death.png");
        ImageLoader.LoadImageFromStream(image3);
    }

    public void DrawIcons()
    {
        // Using overloaded methods for simplicity
        Services.TextureService.DrawIcon(60073, true); // Uses default DrawInfo
        Services.TextureService.DrawIcon(60074, true, new Vector2(24, 24)); // Specifies only the size
        Services.TextureService.DrawIcon(60001, true, new Vector2(24, 24), new Vector4(1, 1, 1, 1)); // Specifies size and tint color
        Services.TextureService.DrawIcon(60002, true, new Vector2(24, 24), new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies size, tint color, and border color
        Services.TextureService.DrawIcon(60003, true, 48); // Specifies size as float
        Services.TextureService.DrawIcon(60004, true, 24, 24); // Specifies width and height
        Services.TextureService.DrawIcon(60011, true, 24, 24, new Vector4(1, 1, 1, 1)); // Specifies width, height, and tint color
        Services.TextureService.DrawIcon(60012, true, 24, 24, new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies width, height, tint color, and border color
        Services.TextureService.DrawIcon(60025, true, 24, 24, new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies width, height, tint color, and border color    
    }

}
