using Dalamud.Interface.Utility.Raii;
using Plugin.Windows.MainWindow.Enums;

namespace Plugin.Windows.MainWindow;

internal class Sidebar
{
    public static (CollapsingHeaders header, Enum category)? selectedCategory = null;

    public static void DrawSideBar()
    {
        float useContentHeight = -40f; // button height + spacing
        float useMenuWidth = 180f;
        float useContentWidth = ImGui.GetContentRegionAvail().X;

        // Pick the smaller value between useMenuWidth and useContentWidth
        float finalWidth = Math.Min(useMenuWidth, useContentWidth);

        using (var sidebarChild = ImRaii.Child("SideBarMainWindow", new Vector2(finalWidth, useContentHeight * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoScrollbar))
        {
            if (sidebarChild)
            {
                using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, ImGuiHelpers.ScaledVector2(5, 0));

                foreach (CollapsingHeaders header in Enum.GetValues(typeof(CollapsingHeaders)))
                {

                    var isCurrent = selectedHeader == header;
                    ImGui.SetNextItemOpen(isCurrent);
                    if (ImGui.CollapsingHeader(header.ToString()))
                    {
                        if (!isCurrent)
                        {
                            // Set selectedHeader to the newly opened header
                            selectedHeader = header;

                            // Get the first category for the newly opened header
                            List<Enum> category = GetCategoriesByHeader(header).ToList();
                            if (category.Count != 0)
                            {
                                Enum firstCategory = category.First();

                                // Set the selected category to the first category of the new header
                                selectedCategory = (header, firstCategory);
                                ContentWindow.ShowChildWindowForCategory(header, firstCategory);
                            }
                        }

                        // Render the categories under the selected header
                        IEnumerable<Enum> categories = GetCategoriesByHeader(header);

                        foreach (Enum category in categories)
                        {
                            ImGui.Indent();
                            if (ImGui.Selectable(category.ToString(), Sidebar.selectedCategory?.category.Equals(category) ?? false))
                            {
                                ContentWindow.ShowChildWindowForCategory(header, category);
                            }
                            ImGui.Unindent();
                        }
                    }
                }
            }
        }
    }

    // Initialize selectedHeader with the default value
    internal static CollapsingHeaders selectedHeader = CollapsingHeaders.Character;

    /// <summary> Sets the categories per header </summary>
    /// <param name="header"> </param>
    /// <returns> </returns>
    private static IEnumerable<Enum> GetCategoriesByHeader(CollapsingHeaders header)
    {
        switch (header)
        {
            case CollapsingHeaders.Character:
                return Enum.GetValues(typeof(Character)).Cast<Enum>();

            case CollapsingHeaders.Test:
                return Enum.GetValues(typeof(Test)).Cast<Enum>();

            default:
                return Enumerable.Empty<Enum>();
        }
    }
}
