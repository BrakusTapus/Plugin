using Dalamud.Interface.Utility.Raii;
using Plugin.Utilities.UI;

namespace Plugin.Windows.MainWindow;
internal class Header
{
    public static float HeaderFooterHeight = 40;

    public static void DrawHeader()
    {
        var style = ImGui.GetStyle();
        var windowSize = ImGui.GetWindowSize();

        using (var headerMainWindow = ImRaii.Child("HeaderBarMainWindow", new Vector2(windowSize.X - style.WindowPadding.X * 2, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (headerMainWindow)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(5, 5));
                ImGui.SetCursorPosY(7);
                ImGui.SetCursorPosX(windowSize.X - 110);
                Buttons.SettingsButton();
                ImGui.PopStyleVar(2);
            }
        }
        ImGui.Separator();
    }
}
