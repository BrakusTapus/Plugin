using Dalamud.Interface.Components;

namespace ImGuiExtensions;

public static partial class ImGuiExt
{
    public static void ExitButton()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, ColorEx.RedBright);
        if (ImGuiComponents.IconButton(FontAwesomeIcon.Times, ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
        {

        }
        ImGui.PopStyleColor();
    }

    public static void ExitButton(bool border)
    {
        if (border)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            ImGui.PushStyleColor(ImGuiCol.Border, ColorEx.Orange);
            ImGui.PushStyleColor(ImGuiCol.Text, ColorEx.RedBright);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Times, ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
            {

            }
            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar();
        }
        else
        {
            ExitButton();
        }
    }
}
