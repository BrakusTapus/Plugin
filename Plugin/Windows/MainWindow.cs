using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Plugin.Utility.UI;

namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly UiPaths uiPaths;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, UiPaths uiPaths)
        : base($"{nameof(MainWindow.plugin)}##With a hidden ID")
    {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(10, 20),
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
        if (ImGuiComponents.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.Cog, "Settings", Colours.Transparent, Colours.ButtonActive, Colours.TextHovered))
        {
            plugin.ToggleConfigUI();
        }
        ImGuiExt.NewTooltip("Toggles the config window.");
        ImGui.Spacing();
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        ImGui.Spacing();
    }

}