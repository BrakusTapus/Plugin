//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using Dalamud.Interface.Utility.Raii;
//using ImGuiNET;

//namespace Plugin.Windows;

//public class Sidebar
//{
//    private readonly CategoryManager categoryManager;
//    private readonly Dictionary<CategoryTabHeaders, bool> headerStates;

//    public Sidebar(CategoryManager categoryManager)
//    {
//        this.categoryManager = categoryManager;

//        // Initialize header states
//        headerStates = new Dictionary<CategoryTabHeaders, bool>
//        {
//            { CategoryTabHeaders.About, true },
//            { CategoryTabHeaders.Features, false },
//            { CategoryTabHeaders.Automation, false },
//            { CategoryTabHeaders.Commands, false },
//            { CategoryTabHeaders.Links, false }
//        };
//    }

//    public void DrawSideBar()
//    {
//        ImGui.Begin("Sidebar");

//        var useContentHeight = -40f; // Adjust as needed
//        var useMenuWidth = 180f;     // Static width for menu

//        var useContentWidth = ImGui.GetContentRegionAvail().X;

//        using (var sidebarChild = ImRaii.Child("SidebarCategories", new Vector2(useContentWidth, useContentHeight * ImGuiHelpers.GlobalScale)))
//        {
//            if (sidebarChild)
//            {
//                foreach (var header in headerStates.Keys)
//                {
//                    // Get current state
//                    bool isOpen = headerStates[header];

//                    // Render header and update state
//                    bool newIsOpen = ImGui.CollapsingHeader(header.ToString(), ref isOpen);
//                    if (newIsOpen != isOpen)
//                    {
//                        // Update state
//                        headerStates[header] = newIsOpen;
//                    }

//                    // Render categories based on the header's expanded state
//                    if (isOpen)
//                    {
//                        DrawCategorySelectors(header);
//                    }
//                }
//            }
//        }

//        ImGui.End();
//    }

//    private void DrawCategorySelectors(CategoryTabHeaders header)
//    {
//        var colorSearchHighlight = GetSearchHighlightColor();
//        var currentGroup = categoryManager.GetGroupInfoByHeader(header);

//        if (currentGroup == null)
//        {
//            return; // No group found for the header, exit method
//        }

//        var isCurrentGroup = currentGroup.GroupKind == categoryManager.CurrentGroupKind;
//        ImGui.SetNextItemOpen(isCurrentGroup);

//        if (ImGui.CollapsingHeader(currentGroup.Name, isCurrentGroup ? ImGuiTreeNodeFlags.OpenOnDoubleClick : ImGuiTreeNodeFlags.None))
//        {
//            if (!isCurrentGroup)
//            {
//                categoryManager.CurrentGroupKind = currentGroup.GroupKind;
//            }

//            ImGui.Indent();
//            var categoryItemSize = new Vector2(ImGui.GetContentRegionAvail().X - 5 * ImGuiHelpers.GlobalScale, ImGui.GetTextLineHeight());

//            foreach (var categoryKind in currentGroup.Categories)
//            {
//                var categoryInfo = categoryManager.GetCategoryInfo(categoryKind);
//                if (ShouldShowCategory(categoryInfo))
//                {
//                    //var hasSearchHighlight = categoryManager.IsCategoryHighlighted(categoryInfo.CategoryKind);
//                    //if (hasSearchHighlight)
//                    //{
//                    //    ImGui.PushStyleColor(ImGuiCol.Text, colorSearchHighlight);
//                    //}

//                    if (ImGui.Selectable(categoryInfo.Name, categoryManager.CurrentCategoryKind == categoryKind, ImGuiSelectableFlags.None, categoryItemSize))
//                    {
//                        categoryManager.CurrentCategoryKind = categoryKind;
//                    }

//                    //if (hasSearchHighlight)
//                    //{
//                    //    ImGui.PopStyleColor();
//                    //}
//                }
//            }

//            ImGui.Unindent();
//            ImGuiHelpers.ScaledDummy(5);
//        }
//    }

//    private bool ShouldShowCategory(CategoryManager.CategoryInfo categoryInfo)
//    {
//        // Modify this based on how you intend to conditionally show categories
//        // For simplicity, assume all categories should be shown unless specified otherwise
//        return true;
//    }

//    private Vector4 GetSearchHighlightColor()
//    {
//        unsafe
//        {
//            var colorPtr = ImGui.GetStyleColorVec4(ImGuiCol.NavHighlight);
//            return colorPtr != null ? *colorPtr : Vector4.One;
//        }
//    }

//    // Other methods like DrawPluginCategoryContent() can be refactored similarly based on your needs.
//}

//internal enum CategoryTabHeaders
//{
//    About,
//    Features,
//    Automation,
//    Commands,
//    Links
//}