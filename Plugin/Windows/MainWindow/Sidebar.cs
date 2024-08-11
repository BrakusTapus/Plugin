using Dalamud.Interface.Utility.Raii;

namespace Plugin.Windows.MainWindow;

internal class Sidebar
{
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

    private static readonly Dictionary<(CategoryTabHeaders, Enum), Action> categoryDrawActions = new Dictionary<(CategoryTabHeaders, Enum), Action>
    {
        { (CategoryTabHeaders.Character, CharacterCategories.Info), CharacterInfo.DrawCharacterInfo },
        //{ (CategoryTabHeaders.Character, CharacterCategories.Retainers), CharacterInfo.DrawRetainers },
        //{ (CategoryTabHeaders.Features, FeatureCategories.General), DrawGeneralFeatures },
        //{ (CategoryTabHeaders.Features, FeatureCategories.Combat), DrawCombatFeatures },
        //{ (CategoryTabHeaders.Automation, AutomationCategories.Retainer), DrawRetainersAutomation },
        //{ (CategoryTabHeaders.Commands, CommandCategories.Teleports), DrawTeleports},
        //{ (CategoryTabHeaders.Links, LinksCategories.General), DrawLinks},

    };


    private static IEnumerable<Enum> GetCategoriesByHeader(CategoryTabHeaders header)
    {
        switch (header)
        {
            case CategoryTabHeaders.Character:
                return Enum.GetValues(typeof(CharacterCategories)).Cast<Enum>();
            //case CategoryTabHeaders.Features:
            //    return Enum.GetValues(typeof(FeatureCategories)).Cast<Enum>();
            //case CategoryTabHeaders.Automation:
            //    return Enum.GetValues(typeof(AutomationCategories)).Cast<Enum>();
            //case CategoryTabHeaders.Commands:
            //    return Enum.GetValues(typeof(CommandCategories)).Cast<Enum>();
            //case CategoryTabHeaders.Links:
            //    return Enum.GetValues(typeof(LinksCategories)).Cast<Enum>();
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
        float contentW = MainMenu.WindowContentRegionWidth;
        float contentH = MainMenu.WindowContentRegionHeight - MainMenu.HeaderFooterHeight;
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

    /// <summary>
    /// The collapsing headers on the side.
    /// </summary>
    internal enum CategoryTabHeaders
    {
        Character,
        //Features,
        //Automation,
        //Commands,
        //Links,
        //Debug
    }

    /// <summary>
    /// Should be like a character screen/menu
    /// </summary>
    internal enum CharacterCategories
    {
        Info,
        //Retainers,
    }


    //internal enum FeatureCategories
    //{
    //    General,
    //    Combat
    //}

    //internal enum AutomationCategories
    //{
    //    //Hunts,
    //    Retainer,
    //    //Misc
    //}

    //internal enum CommandCategories
    //{
    //    Teleports,
    //}

    //internal enum LinksCategories
    //{
    //    General,
    //}

    //internal enum NothingFound
    //{

    //}
}
