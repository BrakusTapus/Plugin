using System;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.Configuration;
using ImGuiExtensions;
using ImGuiNET;
// using Plugin.Configurations; //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
using Plugin.Utility.UI;

namespace Plugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly float _headerFooterHeight = 40f;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like
    // "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be
    // "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base($"{nameof(Plugin)}-{nameof(ConfigWindow)}##0002")
    {
        this.plugin = plugin;

        float mainViewPortWidth = ImGuiHelpers.MainViewport.Size.X;
        float mainViewPortHeight = ImGuiHelpers.MainViewport.Size.Y;
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(mainViewPortWidth, mainViewPortHeight)
        };
        RespectCloseHotkey = true;
        OnCloseSfxId = 24;
        OnOpenSfxId = 23;

        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before DrawImage() is being called, or they won't apply
        if (plugin.EzConfigs.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }

        if (plugin.EzConfigs.IsConfigWindowResizeable)
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        if (plugin.EzConfigs.IsConfigWindowNoTitleBar)
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }

        if (plugin.EzConfigs.IsConfigNoWindowScrollbar)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollbar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollbar;
        }

        if (plugin.EzConfigs.IsConfigWindowNoScrollWithMouse)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollWithMouse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        if (plugin.EzConfigs.IsConfigWindowNoCollapseable)
        {
            Flags &= ~ImGuiWindowFlags.NoCollapse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        if (plugin.EzConfigs.IsConfigWindowNoBackground)
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
        if (ImGui.BeginTabBar("Settings"))
        {
            if (ImGui.BeginTabItem("General Settings"))
            {
                // DrawConfigGroup();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Main Window Settings"))
            {
                ImGui.BeginChild("OtherSettings");
                DrawMainWindowSettings();
                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Config Window Settings"))
            {
                DrawConfigWindowSettings();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    private void DrawConfigGroup()
    {
        bool configValue = plugin.EzConfigs.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Configs Bool", ref configValue))
        {
            plugin.EzConfigs.SomePropertyToBeSavedAndWithADefault = configValue;
            EzConfig.Save();
        }
    }

    private void DrawMainWindowSettings()
    {
        bool movable = plugin.EzConfigs.IsMainWindowMovable;
        if (ImGui.Checkbox("Movable Main Window", ref movable))
        {
            plugin.EzConfigs.IsMainWindowMovable = movable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool resizeable = plugin.EzConfigs.IsMainWindowResizeable;
        if (ImGui.Checkbox("Resizeable Main Window", ref resizeable))
        {
            plugin.EzConfigs.IsMainWindowResizeable = resizeable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool titleBar = plugin.EzConfigs.IsMainWindowNoTitleBar;
        if (ImGui.Checkbox("Main Window Title bar", ref titleBar))
        {
            plugin.EzConfigs.IsMainWindowNoTitleBar = titleBar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (titleBar)
        {
            ImGui.SameLine();
            bool collapseable = plugin.EzConfigs.IsMainWindowNoCollapseable;
            if (ImGui.Checkbox("Main Window Collapseable", ref collapseable))
            {
                plugin.EzConfigs.IsMainWindowNoCollapseable = collapseable;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool scrollbar = plugin.EzConfigs.IsMainNoWindowScrollbar;
        if (ImGui.Checkbox("Main Window Scrollbar", ref scrollbar))
        {
            plugin.EzConfigs.IsMainNoWindowScrollbar = scrollbar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (scrollbar)
        {
            ImGui.SameLine();
            bool scrollWithMouse = plugin.EzConfigs.IsMainWindowNoScrollWithMouse;
            if (ImGui.Checkbox("Main Window Scroll With Mouse", ref scrollWithMouse))
            {
                plugin.EzConfigs.IsMainWindowNoScrollWithMouse = scrollWithMouse;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool noBackground = plugin.EzConfigs.IsMainWindowNoBackground;
        if (ImGui.Checkbox("Background", ref noBackground))
        {
            plugin.EzConfigs.IsMainWindowNoBackground = noBackground;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }
    }

    private void DrawConfigWindowSettings()
    {
        bool movable = plugin.EzConfigs.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Configs Window", ref movable))
        {
            plugin.EzConfigs.IsConfigWindowMovable = movable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool resizeable = plugin.EzConfigs.IsConfigWindowResizeable;
        if (ImGui.Checkbox("Resizeable Configs Window", ref resizeable))
        {
            plugin.EzConfigs.IsConfigWindowResizeable = resizeable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }


        bool titleBar = plugin.EzConfigs.IsConfigWindowNoTitleBar;
        if (ImGui.Checkbox("Title bar", ref titleBar))
        {
            plugin.EzConfigs.IsConfigWindowNoTitleBar = titleBar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed


        }

        if (titleBar)
        {
            ImGui.SameLine();
            bool collapseable = plugin.EzConfigs.IsConfigWindowNoCollapseable;
            if (ImGui.Checkbox("Collapseable", ref collapseable))
            {
                plugin.EzConfigs.IsConfigWindowNoCollapseable = collapseable;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool scrollbar = plugin.EzConfigs.IsConfigNoWindowScrollbar;
        if (ImGui.Checkbox("Scrollbar", ref scrollbar))
        {
            plugin.EzConfigs.IsConfigNoWindowScrollbar = scrollbar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (scrollbar)
        {
            ImGui.SameLine();
            bool scrollWithMouse = plugin.EzConfigs.IsConfigWindowNoScrollWithMouse;
            if (ImGui.Checkbox("Scroll With Mouse", ref scrollWithMouse))
            {
                plugin.EzConfigs.IsConfigWindowNoScrollWithMouse = scrollWithMouse;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }


        bool background = plugin.EzConfigs.IsConfigWindowNoBackground;
        if (ImGui.Checkbox("Background", ref background))
        {
            plugin.EzConfigs.IsConfigWindowNoBackground = background;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }
    }



}
