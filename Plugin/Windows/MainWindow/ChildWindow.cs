using System.Reflection;
using Dalamud.Interface.Utility.Raii;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiExtensions;
using Plugin.AutoMarkt;
using Plugin.Internal;
using Plugin.Windows;
using static FFXIVClientStructs.FFXIV.Client.Game.RetainerManager;

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
            case CategoryTabHeaders.Debug:
                return Enum.GetValues(typeof(DebugCategories)).Cast<Enum>();
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

    private static unsafe void DrawDebugRetainers()
    {
        if (!GameRetainerManager.IsReady)
        {
            //Svc.Log.Warning("GameRetainerManager is not ready.");
            return;
        }

        var retainerManager = RetainerManager.Instance();
        var retainers = GameRetainerManager.GetRetainers();

        // Create a dictionary to map retainers to their display order
        var displayOrderMap = new Dictionary<ulong, int>();

        // Use a fixed block to pin the span in memory
        fixed (byte* displayOrderPtr = retainerManager->DisplayOrder)
        {
            for (int i = 0; i < retainers.Length; i++)
            {
                var retainer = retainers[i];
                displayOrderMap[retainer.RetainerID] = displayOrderPtr[i];
            }
        }

        // Sort retainers by display order
        GameRetainerManager.Retainer[]? sortedRetainers = retainers
        .OrderBy(r => displayOrderMap.ContainsKey(r.RetainerID) ? displayOrderMap[r.RetainerID] : int.MaxValue)
        .ToArray();

        if (ImGui.BeginTable("RetainersTable", 8, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Define columns
            ImGui.TableSetupColumn("Display Order Index");
            ImGui.TableSetupColumn("Internal Index");
            ImGui.TableSetupColumn("Retainer ID");
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Class/Job");
            ImGui.TableSetupColumn("Inventory");
            ImGui.TableSetupColumn("Gil");
            ImGui.TableSetupColumn("Items for Sale");

            ImGui.TableHeadersRow();

            for (int i = 0; i < sortedRetainers.Length; i++)
            {
                GameRetainerManager.Retainer? retainer = sortedRetainers[i];
                var displayOrderIndex = displayOrderMap.ContainsKey(retainer.RetainerID) ? displayOrderMap[retainer.RetainerID] : -1;

                ImGui.TableNextRow();

                // Display columns
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(displayOrderIndex.ToString());

                ImGui.TableSetColumnIndex(1);
                ImGui.Text(i.ToString()); // Internal index


                ImGui.TableSetColumnIndex(2);
                ImGui.Text(retainer.RetainerID.ToString());

                ImGui.TableSetColumnIndex(3);
                ImGui.Text(retainer.Name);



                ImGui.TableSetColumnIndex(4);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(retainer.ClassJob == 0 ? 62143 : 062100 + retainer.ClassJob, true, out var t))
                {
                    ImGui.Image(t.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                string classJobName = ExcelJobHelper.GetJobNameById(retainer.ClassJob);
                ImGui.Text("" + retainer.Level.ToString());





                ImGui.TableSetColumnIndex(5);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(60356, true, out var inv))
                {
                    ImGui.Image(inv.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"{retainer.ItemCount}/175");



                ImGui.TableSetColumnIndex(6);
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(065002, true, out var g))
                {
                    ImGui.Image(g.ImGuiHandle, new(24, 24));
                }
                else
                {
                    ImGui.Dummy(new(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"{retainer.Gil.ToString("N0")}");




                ImGui.TableSetColumnIndex(7);
                //if (ThreadLoadImageHandler.TryGetIconTextureWrap(060570, true, out var g))
                //{
                //    ImGui.Image(g.ImGuiHandle, new(24, 24));
                //}
                //else
                //{
                //    ImGui.Dummy(new(24, 24));
                //}

                if (ThreadLoadImageHandler.TryGetIconTextureWrap(GameRetainerManager.GetTownIconID((retainer.Town)), true, out var town))
                {
                    ImGui.Image(town.ImGuiHandle, new System.Numerics.Vector2(24, 24));
                }
                else
                {
                    ImGui.Dummy(new System.Numerics.Vector2(24, 24));
                }
                ImGui.SameLine();
                ImGui.Text($"Selling {retainer.MarketItemCount} items");


            }

            ImGui.EndTable();
        }
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

    /*
           if (ThreadLoadImageHandler.TryGetIconTextureWrap(ret.Job == 0 ? 62143 : 062100 + ret.Job, true, out var t))
           {
               ImGui.Image(t.ImGuiHandle, new(24, 24));
           }
           else
           {
               ImGui.Dummy(new(24, 24));
           }
   */

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
        { (CategoryTabHeaders.Debug, DebugCategories.Retainers), DrawDebugRetainers}
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
        Links,
        Debug
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

    internal enum DebugCategories
    {
        Retainers,
    }

    internal enum NothingFound
    {

    }
    #endregion
}
