using Dalamud.Interface.Utility.Raii;
using Plugin.Windows.MainWindow.Enums;

namespace Plugin.Windows.MainWindow;

internal class ContentWindow
{
    internal static void DrawContent()
    {
        float contentW = MainMenu.WindowContentRegionWidth;
        float contentH = MainMenu.WindowContentRegionHeight - MainMenu.HeaderFooterHeight;
        using (ImRaii.IEndObject contentMainWindow = ImRaii.Child("ContentMainWindow", new Vector2(contentW, contentH - (5 * ImGuiHelpers.GlobalScale)), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground))
        {
            if (contentMainWindow)
            {
                using ImRaii.Style style = ImRaii.PushStyle(ImGuiStyleVar.ChildBorderSize, 10f);
                if (Sidebar.selectedCategory.HasValue)
                {
                    (CollapsingHeaders header, Enum category) = Sidebar.selectedCategory.Value;

                    if (categoryDrawActions.TryGetValue((header, category), out Action? drawAction))
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
        }

        //if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH - (5 * ImGuiHelpers.GlobalScale)), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground))
        //{
        //    if (Sidebar.selectedCategory.HasValue)
        //    {
        //        (CollapsingHeaders header, Enum category) = Sidebar.selectedCategory.Value;

        //        if (categoryDrawActions.TryGetValue((header, category), out Action? drawAction))
        //        {
        //            drawAction.Invoke();
        //        }
        //        else
        //        {
        //            ImGuiHelpers.CenteredText("No content available for this category.");
        //        }
        //    }
        //    else
        //    {
        //        string defaultText = "Select a category from the sidebar.";
        //        ImGuiHelpers.CenteredText(defaultText);
        //    }
        //}
        //ImGui.EndChild();
    }

    internal static void ShowChildWindowForCategory(CollapsingHeaders header, Enum category)
    {
        Sidebar.selectedCategory = (header, category);
    }

    private static readonly Dictionary<(CollapsingHeaders, Enum), Action> categoryDrawActions = new Dictionary<(CollapsingHeaders, Enum), Action>
    {
        { (CollapsingHeaders.Character, Character.Info), CharacterInfo.DrawCharacterInfo },
    };
}
