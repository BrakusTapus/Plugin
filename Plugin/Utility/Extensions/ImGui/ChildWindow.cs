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

        if (ImGui.BeginChild("SideBarMainWindow", new Vector2(sidebarW, sidebarH), true,
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.SetCursorPosX(WindowContentRegionWidth / 2);
            float textwidth = ImGui.CalcTextSize("This is the Sidebar!").X;
            ImGui.SetCursorPosX((WindowWidth / 2) - (textwidth / 2));
            ImGui.TextDisabled("This is the Sidebar!");

            foreach (Headers header in Enum.GetValues(typeof(Headers)))
            {
                if (ImGui.CollapsingHeader(header.ToString()))
                {
                    // Add or remove categories here.
                    IEnumerable<string> categories = GetCategoriesByHeader(header);

                    foreach (var category in categories)
                    {
                        if (ImGui.Selectable(category))
                        {
                            // Handle the selected category.
                            ShowChildWindowForCategory(category);
                        }
                    }
                }
            }
        }
        ImGui.EndChild();

        ImGui.SameLine();

        float curserCurrentPos = ImGui.GetCursorPos().X;
        ImGui.SetCursorPosX(curserCurrentPos + 10);
        ImGui.SameLine();
    }


    private static IEnumerable<string> GetCategoriesByHeader(Headers header)
    {
        switch (header)
        {
            case Headers.About:
                return Enum.GetNames(typeof(AboutCategories));
            case Headers.Features:
                return Enum.GetNames(typeof(FeatureCategories));
            case Headers.Automation:
                return Enum.GetNames(typeof(AutomationCategories));
            case Headers.Commands:
                return Enum.GetNames(typeof(CommandCategories));
            case Headers.Links:
                return Enum.GetNames(typeof(LinksCategories));
            default:
                throw new Exception("Header not found");
        }
    }

    // Define a method to handle category selection.
    private static void ShowChildWindowForCategory(string category)
    {
        // Implement window show logic here based on the selected category.
        switch (category)
        {
            case "Category 1":
                //Console.WriteLine("You have selected Category 1");
                // Further processing for Category 1 
                break;
            case "Category 2":
                //Console.WriteLine("You have selected Category 2");
                // Further processing for Category 2
                break;
            case "Category 3":
                //Console.WriteLine("You have selected Category 3");
                // Further processing for Category 3
                break;
            default:
                //Console.WriteLine("Unknown category");
                break;
        }
    }


    public static void DrawContent()
    {
        float contentW = WindowContentRegionWidth;
        float contentH = WindowContentRegionHeight - HeaderFooterHeight;
        if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
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

public enum Headers
{
    About,
    Features,
    Automation,
    Commands,
    Links
}

public enum AboutCategories
{
    About,
    Info,
}


public enum FeatureCategories
{
    General,
    QoL,
    Combat
}

public enum AutomationCategories
{
    Hunts,
    Retainers,
    Misc
}

public enum CommandCategories
{
    Teleports,
}

public enum LinksCategories
{
    General,
    Savage,
    Ultimates
}