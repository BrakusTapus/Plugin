using Plugin.Windows;

namespace Plugin.Utility.Extensions;

internal static class ChildWindow
{
    #region WindowSizes
    private static float HeaderFooterHeight => 40;
    private static float WindowPaddingX => ImGui.GetStyle().WindowPadding.X;
    private static float WindowPaddingY => ImGui.GetStyle().WindowPadding.Y;
    private static float WindowWidth => ImGui.GetWindowWidth() - ImGui.GetStyle().WindowPadding.X;
    private static float WindowHeight => ImGui.GetWindowHeight() - ImGui.GetStyle().WindowPadding.Y;
    private static float WindowContentWidth => ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    private static float WindowContentHeight => ImGui.GetWindowContentRegionMax().Y - ImGui.GetWindowContentRegionMin().Y;
    private static float WindowContentRegionWidth => ImGui.GetContentRegionAvail().X;
    private static float WindowContentRegionHeight => ImGui.GetContentRegionAvail().Y;
    #endregion

    #region Padded Child Window
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
    #endregion

    #region Header
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
    #endregion


    public static TabHeaders? selectedHeader = null;
    public static void DrawSideBar()
    {
        ImGui.PushID("Categories");
        float sidebarW = WindowContentRegionWidth / 4;
        float sidebarH = WindowContentRegionHeight - HeaderFooterHeight;
        ImGui.SetCursorPosX(WindowPaddingX);

        if (ImGui.BeginChild("SideBarMainWindow", new Vector2(sidebarW, sidebarH), true, ImGuiWindowFlags.NoScrollbar))
        {
            foreach (TabHeaders header in Enum.GetValues(typeof(TabHeaders)))
            {
                bool isOpen = selectedHeader == header;
                if (ImGui.CollapsingHeader(header.ToString(), ref isOpen))
                {
                    if (isOpen && selectedHeader != header)
                    {
                        selectedHeader = header;
                    }
                    else if (!isOpen && selectedHeader == header)
                    {
                        selectedHeader = null;
                    }
                    if (isOpen)
                    {
                        Svc.Log.Warning("isOpen = " + isOpen);
                            
                        // Get the categories for the selected header
                        IEnumerable<string> categories = GetCategoriesByHeader(header);

                        foreach (string category in categories)
                        {
                            if (ImGui.Selectable(category))
                            {
                                // Handle the selected category
                                ShowChildWindowForCategory(header, category);
                            }
                        }
                    }

                }
            }
        }
        ImGui.EndChild();
        ImGui.PopID();
        ImGui.SameLine();

        float curserCurrentPos = ImGui.GetCursorPos().X;
        ImGui.SetCursorPosX(curserCurrentPos + 10);
        ImGui.SameLine();
    }

/* Old method for drawing categories
    public static void DrawSideBar()
    {
        ImGui.PushID("Categories");
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
        ImGui.PopID();
        ImGui.SameLine();

        float curserCurrentPos = ImGui.GetCursorPos().X;
        ImGui.SetCursorPosX(curserCurrentPos + 10);
        ImGui.SameLine();
    }
*/


    private static IEnumerable<string> GetCategoriesByHeader(TabHeaders header)
    {
        switch (header)
        {
            case TabHeaders.About:
                return Enum.GetNames(typeof(AboutCategories));
            case TabHeaders.Features:
                return Enum.GetNames(typeof(FeatureCategories));
            case TabHeaders.Automation:
                return Enum.GetNames(typeof(AutomationCategories));
            case TabHeaders.Commands:
                return Enum.GetNames(typeof(CommandCategories));
            case TabHeaders.Links:
                return Enum.GetNames(typeof(LinksCategories));
            default:
                throw new Exception("Header not found");
        }
    }

    public static (TabHeaders header, string category)? selectedCategory = null;
    private static void ShowChildWindowForCategory(TabHeaders header, string category)
    {
        selectedCategory = (header, category);
    }

/*
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
*/

    public static void DrawContent()
    {
        float contentW = WindowContentRegionWidth;
        float contentH = WindowContentRegionHeight - HeaderFooterHeight;
        if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
        {

            if (selectedCategory.HasValue)
            {
                var (header, category) = selectedCategory.Value;
                // You can add your rendering logic here based on header and category
                ImGuiHelpers.CenteredText($"Currently viewing {category} under {header} header.");
            }
            else
            {
                string defaultText = "Select a category from the sidebar.";
                ImGuiHelpers.CenteredText(defaultText);
                
            }
        }
        ImGui.EndChild();
    }


    #region Footer
    public static void DrawFooter()
    {
        ImGui.Separator();
        if (ImGui.BeginChild("FooterMainWindow", new Vector2(WindowContentRegionWidth, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGuiHelpers.CenteredText("This is the Footer!");
        }
        ImGui.EndChild();
    }
    #endregion


    internal enum TabHeaders
    {
        About,
        Features,
        Automation,
        Commands,
        Links
    }

    internal enum AboutCategories
    {
        About,
        Info,
    }


    internal enum FeatureCategories
    {
        General,
        QoL,
        Combat
    }

    internal enum AutomationCategories
    {
        Hunts,
        Retainers,
        Misc
    }

    internal enum CommandCategories
    {
        Teleports,
    }

    internal enum LinksCategories
    {
        General,
        Savage,
        Ultimates
    }
}
