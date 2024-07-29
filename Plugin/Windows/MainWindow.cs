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
using Plugin.Utility;
using Plugin.Utility.UI;

namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    //string hitpointsImagePath = Path.Combine(Services.UiPaths.SkillPath, "hitpoints.png");
    //string pluginImagePath = Path.Combine(Services.UiPaths.BasePath, "plugin.png");

    //UiPaths = new UiPaths(Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, "UI"));
    //string hitpointsImagePath = Path.Combine(UiPaths.SkillPath, "hitpoints.png");
    //string pluginImagePath = Path.Combine(UiPaths.BasePath, "plugin.png");
    private readonly Plugin plugin;
    private readonly UiPaths uiPaths;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, UiPaths uiPaths)
        : base($"{nameof(MainWindow.plugin)}##With a hidden ID")
    {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(800, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        SizeCondition = ImGuiCond.None;

        this.plugin = plugin;
        this.uiPaths = uiPaths;
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
        //ImGui.Text($"UI Base Path: {UiPaths.ToString()}");
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        if (ImGuiComponents.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.Cog, "Settings", Colours.Transparent, Colours.ButtonActive, Colours.TextHovered))
        {
            plugin.ToggleConfigUI();
        }
        ImGuiExt.NewTooltip("Toggles the config window.");

        ImGui.Spacing();

        IDalamudTextureWrap? hitpointsImage = DrawImages(UiPaths.SkillPath, "hitpoints.png");
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

        ImGui.SameLine();

        IDalamudTextureWrap? pluginimage2 = DrawImplingImages("lucky.png");
        if (pluginimage2 != null)
        {
            ImGui.Image(pluginimage2.ImGuiHandle, new Vector2(pluginimage2.Width, pluginimage2.Height));
        }
        else
        {
            ImGui.Text("Plugin image not found.");
        }

        ImGui.SameLine();

        IDalamudTextureWrap? bondBannerImage = DrawImages(UiPaths.BondsPouchPath,"banner.png");
        if (bondBannerImage != null)
        {
            ImGui.Image(bondBannerImage.ImGuiHandle, new Vector2(bondBannerImage.Width, bondBannerImage.Height));
        }
        else
        {
            ImGui.Text("image not found.");
        }

        DrawIcons();
        //string? image3 = Path.Combine(plugin.UiPaths , "button", "equipment_items_lost_on_death.png");
        //ImageLoader.LoadImageFromStream(image3);
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

    public IDalamudTextureWrap? DrawImplingImages(string filename)
    {
        string fullPath = Path.Combine(UiPaths.ImplingPath, filename);
        return Services.TextureProvider.GetFromFile(fullPath).GetWrapOrDefault();
    }

    public IDalamudTextureWrap? DrawImages(string catagoryUiPaths, string filename)
    {
        string fullPath = Path.Combine(catagoryUiPaths, filename);
        return Services.TextureProvider.GetFromFile(fullPath).GetWrapOrDefault();
    }

}
