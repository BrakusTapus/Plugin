using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

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
}
