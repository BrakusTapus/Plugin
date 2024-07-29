using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Plugin.Configurations;

namespace Plugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configs configuration;
    private Plugin plugin;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 90);
        SizeCondition = ImGuiCond.FirstUseEver;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }

        if (configuration.IsConfigWindowResizeable)
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        if (configuration.IsConfigWindowNoTitleBar)
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }

        if (configuration.IsConfigNoWindowScrollbar)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollbar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollbar;
        }

        if (configuration.IsConfigWindowNoScrollWithMouse)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollWithMouse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        if (configuration.IsConfigWindowNoCollapseable)
        {
            Flags &= ~ImGuiWindowFlags.NoCollapse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        if (configuration.IsConfigWindowNoBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }
    }

    public override void Draw()
    {
        DrawImGuiWindowOptions();
        ImGui.SameLine();

    }

    private void DrawImGuiWindowOptions()
    {
        bool configValue = configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Config Bool", ref configValue))
        {
            configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            configuration.Save();
        }

        bool movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }

        bool resizeable = configuration.IsConfigWindowResizeable;
        if (ImGui.Checkbox("Resizeable Config Window", ref resizeable))
        {
            configuration.IsConfigWindowResizeable = resizeable;
            configuration.Save();
        }

        bool titleBar = configuration.IsConfigWindowNoTitleBar;
        if (ImGui.Checkbox("Title bar", ref titleBar))
        {
            configuration.IsConfigWindowNoTitleBar = titleBar;
            configuration.Save();
        }

        bool scrollbar = configuration.IsConfigNoWindowScrollbar;
        if (ImGui.Checkbox("Scrollbar", ref scrollbar))
        {
            configuration.IsConfigNoWindowScrollbar = scrollbar;
            configuration.Save();
        }

        bool scrollWithMouse = configuration.IsConfigWindowNoScrollWithMouse;
        if (ImGui.Checkbox("Scroll With Mouse", ref scrollWithMouse))
        {
            configuration.IsConfigWindowNoScrollWithMouse = scrollWithMouse;
            configuration.Save();
        }

        bool collapseable = configuration.IsConfigWindowNoCollapseable;
        if (ImGui.Checkbox("Collapseable", ref collapseable))
        {
            configuration.IsConfigWindowNoCollapseable = collapseable;
            configuration.Save();
        }

        bool background = configuration.IsConfigWindowNoBackground;
        if (ImGui.Checkbox("No background", ref background))
        {
            configuration.IsConfigWindowNoBackground = background;
            configuration.Save();
        }
    }
}
