namespace Plugin.Utility.Extensions;

internal static class ChildWindow
{
    private static float HeaderFooterHeight => 40;
    private static float WindowPaddingX => ImGui.GetStyle().WindowPadding.X;
    private static float WindowPaddingY => ImGui.GetStyle().WindowPadding.Y;
    private static float WindowWidth => ImGui.GetWindowWidth() - ImGui.GetStyle().WindowPadding.X;
    private static float WindowHeight => ImGui.GetWindowHeight() - ImGui.GetStyle().WindowPadding.Y;
    private static float WindowContentWidth => ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    private static float WindowContentHeight => ImGui.GetWindowContentRegionMax().Y - ImGui.GetWindowContentRegionMin().Y;

    private static float WindowContentRegionWidth => ImGui.GetContentRegionAvail().X; //- ImGui.GetStyle().WindowPadding.X;
    private static float WindowContentRegionHeight => ImGui.GetContentRegionAvail().Y; //- ImGui.GetStyle().WindowPadding.Y;

    public static bool BeginPaddedChild(string str_id, bool border = false, ImGuiWindowFlags flags = 0)
    {
        float padding = ImGui.GetStyle().WindowPadding.X;
        // Set cursor position with padding
        float cursorPosX = ImGui.GetCursorPosX() + padding;
        ImGui.SetCursorPosX(cursorPosX);

        // Adjust the size to account for padding
        // Get the available size and adjust it to account for padding
        Vector2 size = ImGui.GetContentRegionAvail();
        size.X -= 2 * padding;
        size.Y -= 2 * padding;

        // Begin the child window
        return ImGui.BeginChild(str_id, size, border, flags);
    }

    public static void EndPaddedChild()
    {
        ImGui.EndChild();
    }


    public static void DrawHeader()
    {
        ImGui.SetCursorPosX(WindowPaddingX);
        if (ImGui.BeginChild("HeaderMainWindow", new Vector2(WindowContentRegionWidth, HeaderFooterHeight), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {

            float textwidth = ImGui.CalcTextSize("This is the Header!").X;
            ImGui.SetCursorPosX((WindowWidth / 2) - (textwidth / 2));
            ImGui.TextDisabled("This is the Header!");
        }
        ImGui.EndChild();
        ImGui.Separator();
    }

    public static void DrawSideBar()
    {
        float sidebarW = WindowContentRegionWidth / 4;
        float sidebarH = WindowContentRegionHeight - HeaderFooterHeight;
        ImGui.SetCursorPosX(WindowPaddingX);
        if (ImGui.BeginChild("SideBarMainWindow", new Vector2(sidebarW, sidebarH), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.SetCursorPosX(WindowContentRegionWidth / 2);
            float textwidth = ImGui.CalcTextSize("This is the Sidebar!").X;
            ImGui.SetCursorPosX((WindowWidth / 2) - (textwidth / 2));
            ImGui.TextDisabled("This is the Sidebar!");
        }
        ImGui.EndChild();
        ImGui.SameLine();
        float curserCurrentPos = ImGui.GetCursorPos().X;
        ImGui.SetCursorPosX(curserCurrentPos + 10);
        ImGui.SameLine();

    }

    public static void DrawContent()
    {
        float contentW = WindowContentRegionWidth;
        float contentH = WindowContentRegionHeight - HeaderFooterHeight;
        if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.SetCursorPosX(WindowContentRegionWidth / 2);
            float textwidth = ImGui.CalcTextSize("This is the Content!!").X;
            ImGui.SetCursorPosX((WindowWidth / 2) - (textwidth / 2));
            ImGui.TextDisabled("This is the Header!");

        }
        ImGui.EndChild();
    }
    public static void DrawFooter()
    {
        ImGui.Separator();
        if (ImGui.BeginChild("FooterMainWindow", new Vector2(WindowContentRegionWidth, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.SetCursorPosX(WindowWidth / 2);
            ImGui.TextDisabled("This is the Footer!");
        }
        ImGui.EndChild();
    }

}
