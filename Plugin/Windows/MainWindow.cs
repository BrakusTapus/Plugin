using System;
using System.IO;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Plugin.Utility.UI;

namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    private string HitpointsImagePath;
    private string PluginImagePath;
    private Plugin plugin;

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

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings##ToggleUIWindow"))
        {
            plugin.ToggleConfigUI();
        }
        ImGuiExt.HelpMarker("Toggles the config window.");

        ImGui.Spacing();

        ImGui.Text("Have some love:");
        var hitpointsImage = Services.TextureProvider.GetFromFile(HitpointsImagePath).GetWrapOrDefault();
        if (hitpointsImage != null)
        {
            ImGuiHelpers.ScaledIndent(55f);
            ImGui.Image(hitpointsImage.ImGuiHandle, new Vector2(hitpointsImage.Width, hitpointsImage.Height));
            ImGuiHelpers.ScaledIndent(-55f);
        }
        else
        {
            ImGui.Text("HP image not found.");
        }
        var pluginImage = Services.TextureProvider.GetFromFile(PluginImagePath).GetWrapOrDefault();
        if (pluginImage != null)
        {
            ImGui.Image(pluginImage.ImGuiHandle, new Vector2(pluginImage.Width, pluginImage.Height));
        }
        else
        {
            ImGui.Text("Plugin image not found.");
        }

        //ImageLoader.LoadImageFromStream(HitpointsImagePath);
        var image3 = Path.Combine(plugin.LocalImagesPath, "button", "equipment_items_lost_on_death.png");
        ImageLoader.LoadImageFromStream(image3);
        var image4 = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ae/Github-desktop-logo-symbol.svg/2048px-Github-desktop-logo-symbol.svg.png";
        ImageLoader.LoadImageFromStream(image4);
    }
}
