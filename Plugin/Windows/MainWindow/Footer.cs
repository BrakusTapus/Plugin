using Dalamud.Interface.Utility.Raii;
using Plugin.Utilities.UI;

namespace Plugin.Windows.MainWindow;
internal static class Footer
{
    public static float HeaderFooterHeight = 40;

    public static void DrawFooter()
    {

        var style = ImGui.GetStyle();
        var windowSize = ImGui.GetWindowSize();
        //ImGui.SetCursorPosY((windowSize.Y - HeaderFooterHeight) - 2);
        ImGui.Separator();
        ImGui.SetCursorPosY((windowSize.Y - HeaderFooterHeight - style.WindowPadding.Y));
        ImGui.SetCursorPosX(windowSize.X - windowSize.X + style.WindowPadding.X);
        using var footerMainWindow = ImRaii.Child("FooterMainWindow", new Vector2(windowSize.X - style.WindowPadding.X * 2, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (footerMainWindow)
        {
            string footerVersionText = "v" + Plugin.P.GetType().Assembly.GetName().Version.ToString();
            var footerVersionTextSize = ImGui.CalcTextSize(footerVersionText);
            ImGuiExtKirbo.CenterItemVertically(footerVersionTextSize.Y + -10);

            ImGui.TextDisabled(footerVersionText);

            ImGui.SetCursorPosX((ImGui.GetWindowContentRegionMax().X));
            var buttonHeight = ImGuiHelpers.GetButtonSize("Exit").Y;
            ImGuiExtKirbo.CenterItemVertically(buttonHeight + -10);


            float windowWidth = ImGui.GetWindowWidth();
            float buttonWidth = ImGui.CalcTextSize("Settings").X + ImGui.GetStyle().FramePadding.X * 2;
            float buttonPositionX = windowWidth - buttonWidth - ImGui.GetStyle().WindowPadding.X;

            // Set the cursor position to the calculated X position
            ImGui.SetCursorPosX(buttonPositionX);
            Buttons.ExitButtonExitButtonMainWindow(true, "Exit");
        }

    }
}
