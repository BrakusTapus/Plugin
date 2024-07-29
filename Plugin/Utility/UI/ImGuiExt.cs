using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Plugin.Configurations;

namespace Plugin.Utility.UI;

public static class ImGuiExt
{
    /// <summary>
    /// <br>HelpMarker component to add a help icon with text on hover.</br>
    /// <br>helpText: The text to display on hover.</br>
    /// </summary>
    /// <param name="helpText"></param>
    public static void HelpMarker(string helpText)
    {
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextDisabled(FontAwesomeIcon.InfoCircle.ToIconString());
        ImGui.PopFont();
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 40f);
            ImGui.TextColored(Colours.GhostType, helpText);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

    #region ToolTips
    /// <summary>
    ///
    /// </summary>
    /// <param name="s"></param>
    public static void Tooltip(string s)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ImGui.BeginTooltip();
        ImGui.TextWrapped(s);
        ImGui.EndTooltip();
    }

    /// <summary>
    /// Sets and displays a colored text tooltip with the given text and color if the current item is hovered.
    /// </summary>
    public static void Tooltip(string s, Vector4 color)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ImGui.BeginTooltip();
        ImGui.TextColored(color, s);
        ImGui.EndTooltip();
    }

    /// <summary>
    /// Displays a tooltip with the given text if the current item is hovered.
    /// </summary>
    public static void NewTooltip(string s)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ShowTooltip(() => ImGui.Text(s));
    }

    /// <summary>
    /// Displays a tooltip if the action is not null and tooltips are enabled in the configuration.
    /// </summary>
    public static void ShowTooltip(Action act)
    {
        if (act == null) return;
        //if (!Configs.ShowToolTips) return;

        ImGui.SetNextWindowBgAlpha(1);

        using var color = ImRaii.PushColor(ImGuiCol.BorderShadow, Colours.DalamudWhite);

        ImGui.SetNextWindowSizeConstraints(new Vector2(150, 0) * ImGuiHelpers.GlobalScale, new Vector2(1200, 1500) * ImGuiHelpers.GlobalScale);
        ImGui.SetWindowPos(ImGuiFlags.TOOLTIP_ID, ImGui.GetIO().MousePos);

        if (ImGui.Begin(ImGuiFlags.TOOLTIP_ID, ImGuiFlags.TOOLTIP_FLAG))
        {
            act();
            ImGui.End();
        }
    }
    #endregion


    #region Sliders
    public static bool SliderIntAsFloat(string id, ref int value, int min, int max, float divider = 1)
    {
        float f = (float)value / divider;
        bool ret = ImGui.SliderFloat(id, ref f, (float)min / divider, (float)max / divider);
        if (ret)
        {
            value = (int)(f * divider);
        }
        return ret;
    }

    public static bool SliderFloatAsInt(string id, ref float value, float min, float max, int divider = 1)
    {
        int i = (int)value / divider;
        bool ret = ImGui.SliderInt(id, ref i, (int)min / divider, (int)max / divider);
        if (ret)
        {
            value = (float)(i * divider);
        }
        return ret;
    }
    #endregion

    /// <summary>
    /// TODO: Add description
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool IsInViewport(Vector2 size)
    {
        float distanceY = ImGui.GetCursorPosY() - ImGui.GetScrollY();
        return distanceY >= -size.Y && distanceY <= ImGui.GetWindowHeight();
    }
}
