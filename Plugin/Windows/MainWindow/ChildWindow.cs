using Dalamud.Interface.Utility.Raii;
using Plugin.Windows;

namespace Plugin.Windows;

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
        if (ImGui.BeginChild("HeaderMainWindow", new Vector2(WindowContentRegionWidth, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            float textwidth = ImGui.CalcTextSize("This is the Header!").X;
            ImGui.SetCursorPosX(WindowWidth / 2 - textwidth / 2);
            ImGui.TextDisabled("This is the Header!");

        }
        ImGui.EndChild();
        ImGui.Separator();
    }
    #endregion

    public static CategoryTabHeaders? selectedHeader = null;

    public static void DrawSideBar()
    {
        var useContentHeight = -40f; // button height + spacing
        var useMenuWidth = 180f;     // works fine as static value, table can be resized by user
        var useContentWidth = ImGui.GetContentRegionAvail().X;

        // Pick the smaller value between useMenuWidth and useContentWidth
        var finalWidth = Math.Min(useMenuWidth, useContentWidth);

        using (var sidebarChild = ImRaii.Child("SideBarMainWindow", new Vector2(finalWidth, useContentHeight * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoScrollbar))
        {
            if (sidebarChild)
            {
                using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, ImGuiHelpers.ScaledVector2(5, 0));

                foreach (CategoryTabHeaders header in Enum.GetValues(typeof(CategoryTabHeaders)))
                {
                    bool isOpen = selectedHeader == header;

                    if (ImGui.CollapsingHeader(header.ToString(), ImGuiTreeNodeFlags.Framed))
                    {
                        // Only change selectedHeader if the header is being opened
                        if (isOpen && selectedHeader != header)
                        {
                            selectedHeader = header;
                        }

                        if (!isOpen)
                        {
                            IEnumerable<Enum> categories = GetCategoriesByHeader(header);

                            foreach (Enum category in categories)
                            {
                                ImGui.Indent();
                                if (ImGui.Selectable(category.ToString(), selectedCategory?.category.Equals(category) ?? false))
                                {
                                    ShowChildWindowForCategory(header, category);
                                }
                                ImGui.Unindent();
                            }
                        }
                    }
                }
            }
        }
    }

    //public static void DrawSideBar()
    //{
    //    var useContentHeight = -40f; // button height + spacing
    //    var useMenuWidth = 180f;     // works fine as static value, table can be resized by user
    //    var useContentWidth = ImGui.GetContentRegionAvail().X;

    //    // Pick the smaller value between useMenuWidth and useContentWidth
    //    var finalWidth = Math.Min(useMenuWidth, useContentWidth);

    //    using (var sidebarChild = ImRaii.Child("SideBarMainWindow", new Vector2(finalWidth, useContentHeight * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoScrollbar))
    //        if (sidebarChild)
    //        {
    //            using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, ImGuiHelpers.ScaledVector2(5, 0));

    //            try
    //            {
    //                foreach (CategoryTabHeaders header in Enum.GetValues(typeof(CategoryTabHeaders)))
    //                {
    //                    bool isOpen = selectedHeader == header;
    //                    bool isExpanded = false;

    //                    // Pass isOpen by reference to ImGui.CollapsingHeader
    //                    if (ImGui.CollapsingHeader(header.ToString(), ImGuiTreeNodeFlags.Framed))
    //                    {
    //                        // Only change selectedHeader if the header is being opened
    //                        if (isOpen)
    //                        {
    //                            if (selectedHeader != header)
    //                            {
    //                                isExpanded = true;
    //                                selectedHeader = header;
    //                            }
    //                        }
    //                        //else if (!isExpanded && selectedHeader == null)
    //                        //{

    //                        //    selectedHeader = null;
    //                        //}
    //                        if (!isOpen)
    //                        {
    //                            isExpanded = false;
    //                            IEnumerable<Enum> categories = GetCategoriesByHeader(header);

    //                            foreach (string category in categories)
    //                            {
    //                                ImGui.Indent();
    //                                if (ImGui.Selectable(category))
    //                                {
    //                                    ShowChildWindowForCategory(header, category);
    //                                }
    //                                ImGui.Unindent();
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {

    //                        //Svc.Log.Warning($"{header} header did not collapse/expand");
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {

    //                Svc.Log.Error(ex, "Could not draw plugin categories");
    //            }

    //            //float sidebarW = WindowContentRegionWidth / 4;
    //            //float sidebarH = WindowContentRegionHeight - HeaderFooterHeight;
    //            //ImGui.SetCursorPosX(WindowPaddingX);

    //            //if (ImGui.BeginChild("SideBarMainWindow", new Vector2(sidebarW, sidebarH), true, ImGuiWindowFlags.NoScrollbar))
    //            //{
    //            //foreach (TabHeaders header in Enum.GetValues(typeof(TabHeaders)))
    //            //{
    //            //    bool isOpen = selectedHeader == header;

    //            //    // Pass isOpen by reference to ImGui.CollapsingHeader
    //            //    if (ImGui.CollapsingHeader(header.ToString(), ImGuiTreeNodeFlags.Framed))
    //            //    {
    //            //        // Only change selectedHeader if the header is being opened
    //            //        if (isOpen)
    //            //        {
    //            //            if (selectedHeader != header)
    //            //            {
    //            //                selectedHeader = header;
    //            //            }
    //            //        }
    //            //        else if (!isOpen && selectedHeader == header)
    //            //        {
    //            //            selectedHeader = null;
    //            //        }
    //            //        if (!isOpen)
    //            //        {
    //            //            IEnumerable<string> categories = GetCategoriesByHeader(header);

    //            //            foreach (string category in categories)
    //            //            {
    //            //                if (ImGui.Selectable(category))
    //            //                {
    //            //                    ShowChildWindowForCategory(header, category);
    //            //                }
    //            //            }
    //            //        }
    //            //    }
    //            //    else
    //            //    {
    //            //        Svc.Log.Warning($"{header} header did not collapse/expand");
    //            //    }
    //            //}
    //            //}
    //            //ImGui.EndChild();
    //        }
    //}

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


    private static IEnumerable<Enum> GetCategoriesByHeader(CategoryTabHeaders header)
    {
        switch (header)
        {
            case CategoryTabHeaders.About:
                return Enum.GetValues(typeof(AboutCategories)).Cast<Enum>();
            case CategoryTabHeaders.Features:
                return Enum.GetValues(typeof(FeatureCategories)).Cast<Enum>();
            case CategoryTabHeaders.Automation:
                return Enum.GetValues(typeof(AutomationCategories)).Cast<Enum>();
            case CategoryTabHeaders.Commands:
                return Enum.GetValues(typeof(CommandCategories)).Cast<Enum>();
            case CategoryTabHeaders.Links:
                return Enum.GetValues(typeof(LinksCategories)).Cast<Enum>();
            default:
                return Enumerable.Empty<Enum>();
        }
    }

    public static (CategoryTabHeaders header, Enum category)? selectedCategory = null;
    private static void ShowChildWindowForCategory(CategoryTabHeaders header, Enum category)
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

    //public static void DrawContent()
    //{
    //    float contentW = WindowContentRegionWidth;
    //    float contentH = WindowContentRegionHeight - HeaderFooterHeight;
    //    if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
    //    {

    //        if (selectedCategory.HasValue)
    //        {
    //            var (header, category) = selectedCategory.Value;
    //            // You can add your rendering logic here based on header and category
    //            ImGuiHelpers.CenteredText($"Currently viewing {category} under {header} header.");
    //        }
    //        else
    //        {
    //            string defaultText = "Select a category from the sidebar.";
    //            ImGuiHelpers.CenteredText(defaultText);

    //        }
    //    }
    //    ImGui.EndChild();
    //}

    //public static void DrawContent()
    //{
    //    float contentW = WindowContentRegionWidth;
    //    float contentH = WindowContentRegionHeight - HeaderFooterHeight;
    //    if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
    //    {
    //        if (selectedCategory.HasValue)
    //        {
    //            var (header, category) = selectedCategory.Value;

    //            if (categoryDrawActions.TryGetValue((header, category), out var drawAction))
    //            {
    //                drawAction.Invoke();
    //            }
    //            else
    //            {
    //                ImGuiHelpers.CenteredText("No content available for this category.");
    //            }
    //        }
    //        else
    //        {
    //            string defaultText = "Select a category from the sidebar.";
    //            ImGuiHelpers.CenteredText(defaultText);
    //        }
    //    }
    //    ImGui.EndChild();
    //}

    public static void DrawContent()
    {
        float contentW = WindowContentRegionWidth;
        float contentH = WindowContentRegionHeight - HeaderFooterHeight;
        if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
        {
            if (selectedCategory.HasValue)
            {
                var (header, category) = selectedCategory.Value;

                if (categoryDrawActions.TryGetValue((header, category), out var drawAction))
                {
                    drawAction.Invoke();
                }
                else
                {
                    ImGuiHelpers.CenteredText("No content available for this category.");
                }
            }
            else
            {
                string defaultText = "Select a category from the sidebar.";
                ImGuiHelpers.CenteredText(defaultText);
            }
        }
        ImGui.EndChild();
    }


    #region About Tab
    public static void DrawAboutInfoDetails()
    {
        ImGui.Text("Additional details about the software.");
        // Add more ImGui rendering calls specific to this category
    }
    public static void Changelogs()
    {
        ImGui.Text("This is the About Info section.");
        // Add more ImGui rendering calls specific to this category
    }
    #endregion

    #region features
    public static void DrawGeneralFeatures()
    {
        ImGui.Text("General features of the software.");
        // Add more ImGui rendering calls specific to this category
    }

    public static void DrawCombatFeatures()
    {
        ImGui.Text("Combat-related features.");
        // Add more ImGui rendering calls specific to this category
    }
    #endregion

    #region Automation

    public static void DrawHuntsAutomation()
    {
        ImGui.Text("Automated hunts tasks.");
        // Add more ImGui rendering calls specific to this category
    }

    public static void DrawRetainersAutomation()
    {
        ImGui.Text("Automated retainer tasks.");
    }

    private static void DrawMarkerBoardAutomationUI()
    {

    }

    public static void DrawOthersAutomation()
    {
        ImGui.Text("Other Automated tasks.");
        // Add more ImGui rendering calls specific to this category
    }
    #endregion

    #region Teleports
    public static void DrawTeleports()
    {
        ImGui.Text("Various Teleports.");
        // Add more ImGui rendering calls specific to this category
    }

    #endregion

    #region Links
    public static void DrawLinks()
    {
        ImGui.Text("Various Useful websites!.");
    }

    #endregion

    private static readonly Dictionary<(CategoryTabHeaders, Enum), Action> categoryDrawActions = new Dictionary<(CategoryTabHeaders, Enum), Action>
    {
        { (CategoryTabHeaders.About, AboutCategories.Info), DrawAboutInfoDetails },
        { (CategoryTabHeaders.About, AboutCategories.Changelogs), Changelogs },
        { (CategoryTabHeaders.Features, FeatureCategories.General), DrawGeneralFeatures },
        { (CategoryTabHeaders.Features, FeatureCategories.Combat), DrawCombatFeatures },
        { (CategoryTabHeaders.Automation, AutomationCategories.Hunts), DrawHuntsAutomation },
        { (CategoryTabHeaders.Automation, AutomationCategories.Retainers), DrawRetainersAutomation },
        { (CategoryTabHeaders.Automation, AutomationCategories.Misc), DrawOthersAutomation},
        { (CategoryTabHeaders.Commands, CommandCategories.Teleports), DrawTeleports},
        { (CategoryTabHeaders.Links, LinksCategories.General), DrawLinks},
    };


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


    internal enum CategoryTabHeaders
    {
        About,
        Features,
        Automation,
        Commands,
        Links
    }

    internal enum AboutCategories
    {
        Info,
        Changelogs,
    }


    internal enum FeatureCategories
    {
        General,
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
        //Savage,
        //Ultimates
    }

    internal enum NothingFound
    {

    }
}
