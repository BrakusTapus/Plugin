using System.Reflection;
using Dalamud.Interface.Utility.Raii;
using ImGuiExtensions;
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
        var style = ImGui.GetStyle();
        var windowSize = ImGui.GetWindowSize();

        using (var headerMainWindow = ImRaii.Child("HeaderBarMainWindow", new Vector2(windowSize.X - style.WindowPadding.X * 2, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (headerMainWindow)
            {

                //var searchInputWidth = 180 * ImGuiHelpers.GlobalScale;
                //var searchClearButtonWidth = 25 * ImGuiHelpers.GlobalScale;


                //var headerTextLabels = "Menu's";

                //var headerTextLabelWidth = ImGui.CalcTextSize(headerTextLabels).X;

                //var selectableWidth = headerTextLabelWidth + (style.FramePadding.X * 2);  // This does not include the label
                //var sortSelectWidth = selectableWidth + headerTextLabelWidth + style.ItemInnerSpacing.X;  // Item spacing between the selectable and the label

                //string headerText = "v" + Plugin.P.GetType().Assembly.GetName().Version.ToString();
                //Vector2 headerTextSize = ImGui.CalcTextSize(headerTextLabels);
                //ImGui.SetCursorPosX(ImGui.GetCursorPosX() + style.FramePadding.X * 2);
                //ImGui.SetCursorPosY(ImGui.GetCursorPosY() + style.FramePadding.Y * 2);
                //ImGui.TextDisabled(headerText);

                //ImGui.SameLine();

                //var downShift = ImGui.GetCursorPosY() + (headerTextSize.Y / 4) - 2;
                //ImGui.SetCursorPosY(downShift);

                //ImGui.SetCursorPosX(windowSize.X - sortSelectWidth - (style.ItemSpacing.X * 2) - searchInputWidth - searchClearButtonWidth);
                //ImGui.SameLine();
                //ImGui.TextDisabled(ImGui.GetContentRegionAvail().ToString());
                //ImGui.SameLine();
                //var footerPoS = ImGui.GetContentRegionAvail().Y / 2;

                //var buttonHeight = ImGuiHelpers.GetButtonSize("EXIT").Y /2;
                //var footerPosFinal = footerPoS - buttonHeight;
                //ImGui.SetCursorPosY(footerPosFinal); 
                //ImGui.SameLine();
                //ImGui.Text(ImGui.GetFrameHeight().ToString());
                //ImGui.SameLine();

                float windowWidth = ImGui.GetWindowWidth();
                float buttonWidth = ImGui.CalcTextSize("Exit").X + ImGui.GetStyle().FramePadding.X * 2;
                float buttonPositionX = windowWidth - buttonWidth - 25 - ImGui.GetStyle().WindowPadding.X;
                ImGui.SetCursorPosX(buttonPositionX);

                ImGuiExt.CenterItemVertically(ImGui.GetFrameHeight());
                Buttons.ExitButtonExitButtonMainWindow(true, "Exit");

                ImGui.Separator();

            }
        }
        //ImGui.Separator();
    }
    #endregion

    #region Sidebar(Category window)

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
    #endregion


    #region Content
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
    public static void DrawContent()
    {
        float contentW = WindowContentRegionWidth;
        float contentH = WindowContentRegionHeight - HeaderFooterHeight;
        if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH - (5 * ImGuiHelpers.GlobalScale)), true, ImGuiWindowFlags.NoScrollbar))
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
    #endregion

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
    }

    public static void DrawRetainersAutomation()
    {

        autoAdjustRetainerListings.DrawConfig();

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

        var style = ImGui.GetStyle();
        var windowSize = ImGui.GetWindowSize();
        //ImGui.SetCursorPosY((windowSize.Y - HeaderFooterHeight) - 2);
        //ImGui.Separator();
        ImGui.SetCursorPosY((windowSize.Y - HeaderFooterHeight - style.WindowPadding.Y));
        ImGui.SetCursorPosX(windowSize.X - windowSize.X + style.WindowPadding.X);
        using var footerMainWindow = ImRaii.Child("FooterMainWindow", new Vector2(windowSize.X - style.WindowPadding.X * 2, HeaderFooterHeight), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (footerMainWindow)
        {
            string footerVersionText = "v" + Plugin.P.GetType().Assembly.GetName().Version.ToString();
            var footerVersionTextSize = ImGui.CalcTextSize(footerVersionText);
            ImGui.Separator();
            ImGuiExt.CenterItemVertically(footerVersionTextSize.Y);

            ImGui.TextDisabled(footerVersionText);

            ImGui.SetCursorPosX((ImGui.GetWindowContentRegionMax().X));
            ImGuiExt.CenterItemVertically(22);


            float windowWidth = ImGui.GetWindowWidth();
            float buttonWidth = ImGui.CalcTextSize("Settings").X + ImGui.GetStyle().FramePadding.X * 2;
            float buttonPositionX = windowWidth - buttonWidth - ImGui.GetStyle().WindowPadding.X;

            // Set the cursor position to the calculated X position
            ImGui.SetCursorPosX(buttonPositionX);
            Buttons.SettingsButton();

            //ImGui.SetCursorPosY(ImGui.GetCursorPosY() - footerVersionTextSize.Y);
            //ImGui.SetCursorPosX(style.WindowPadding.X * 4);

            //ImGui.SameLine();
            //ImGui.Dummy(new Vector2(5, 0));
            //ImGui.SameLine();
            //ImGuiHelpers.CenteredText(ImGui.GetContentRegionAvail().ToString());
            //ImGui.SameLine();
            //ImGuiHelpers.CenteredText(ImGui.GetContentRegionMax().ToString());
            //ImGui.SameLine();
            //Buttons.SettingsButton();
        }

    }
    #endregion


    #region Tabs & Categories
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
    #endregion
}
