// public class MainWindow 
// {
//     // Store the currently selected header and category.
//     public static TabHeaders? SelectedHeader = null;
//     public static string SelectedCategory = null;

//     public static void DrawContent()
//     {
//         float contentW = WindowContentRegionWidth;
//         float contentH = WindowContentRegionHeight - HeaderFooterHeight;

//         if (ImGui.BeginChild("ContentMainWindow", new Vector2(contentW, contentH), true, ImGuiWindowFlags.NoScrollbar))
//         {
//             ImGui.SetCursorPosX(WindowContentRegionWidth / 2);

//             // Check which header and category is selected, then display relevant content.
//             switch (SelectedHeader)
//             {
//                 case TabHeaders.About:
//                     DrawAboutContent();
//                     break;

//                 case TabHeaders.Features:
//                     DrawFeaturesContent();
//                     break;

//                 // ... Add cases for other headers ...

//                 default:
//                     float textwidth = ImGui.CalcTextSize("Please select a category").X;
//                     ImGui.SetCursorPosX((WindowWidth / 2) - (textwidth / 2));
//                     ImGui.TextDisabled("Please select a category");
//                     break;
//             }
//         }

//         ImGui.EndChild();
//     }

//     private static void DrawAboutContent()
//     {
//         switch (SelectedCategory)
//         {
//             case nameof(AboutCategories.About):
//                 ImGui.Text("This is About content...");
//                 break;

//             case nameof(AboutCategories.Info):
//                 ImGui.Text("This is Info content...");
//                 break;

//             // ... Add cases for other categories ...
//         }
//     }

//     private static void DrawFeaturesContent()
//     {
//         switch (SelectedCategory)
//         {
//             case nameof(FeatureCategories.General):
//                 ImGui.Text("This is General feature content...");
//                 break;

//             case nameof(FeatureCategories.QoL):
//                 ImGui.Text("This is QoL feature content...");
//                 break;

//             // ... Add cases for other categories ...
//         }
//     }

//     // ... Add methods to draw content for other headers ...
// }