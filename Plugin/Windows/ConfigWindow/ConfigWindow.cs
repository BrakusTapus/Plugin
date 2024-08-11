
using Dalamud.Interface.Utility.Raii;
using ECommons.Configuration;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Plugin.AutoMarkt;
using Plugin.Utilities;
using Plugin.Windows.MainWindow;
using Plugin.Windows.MainWindow.Enums;
using static FFXIVClientStructs.FFXIV.Client.Game.RetainerManager;

namespace Plugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly float _headerFooterHeight = 40f;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like
    // "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be
    // "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base($"{nameof(Plugin)}-{nameof(ConfigWindow)}##0002")
    {
        this.plugin = plugin;

        float mainViewPortWidth = ImGuiHelpers.MainViewport.Size.X;
        float mainViewPortHeight = ImGuiHelpers.MainViewport.Size.Y;
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(mainViewPortWidth, mainViewPortHeight)
        };
        RespectCloseHotkey = true;
        OnCloseSfxId = 24;
        OnOpenSfxId = 23;

        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before DrawImage() is being called, or they won't apply
        if (C.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }

        if (C.IsConfigWindowResizeable)
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        if (C.IsConfigWindowNoTitleBar)
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }

        if (C.IsConfigNoWindowScrollbar)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollbar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollbar;
        }

        if (C.IsConfigWindowNoScrollWithMouse)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollWithMouse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        if (C.IsConfigWindowNoCollapseable)
        {
            Flags &= ~ImGuiWindowFlags.NoCollapse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        if (C.IsConfigWindowNoBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("Settings"))
        {
            if (ImGui.BeginTabItem("General Settings"))
            {
                DrawConfigGroup();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Main Window Settings"))
            {
                ImGui.BeginChild("OtherSettings");
                DrawMainWindowSettings();
                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Config Window Settings"))
            {
                DrawConfigWindowSettings();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Task Buttons"))
            {
                DrawTab4();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Debug"))
            {
                DrawTabDebug();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    private void DrawConfigGroup()
    {
        bool configValue = C.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Configs Bool", ref configValue))
        {
            C.SomePropertyToBeSavedAndWithADefault = configValue;
            EzConfig.Save();
        }
    }

    private void DrawMainWindowSettings()
    {
        bool movable = C.IsMainWindowMovable;
        if (ImGui.Checkbox("Movable Main Window", ref movable))
        {
            C.IsMainWindowMovable = movable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool resizeable = C.IsMainWindowResizeable;
        if (ImGui.Checkbox("Resizeable Main Window", ref resizeable))
        {
            C.IsMainWindowResizeable = resizeable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool titleBar = C.IsMainWindowNoTitleBar;
        if (ImGui.Checkbox("Main Window Title bar", ref titleBar))
        {
            C.IsMainWindowNoTitleBar = titleBar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (titleBar)
        {
            ImGui.SameLine();
            bool collapseable = C.IsMainWindowNoCollapseable;
            if (ImGui.Checkbox("Main Window Collapseable", ref collapseable))
            {
                C.IsMainWindowNoCollapseable = collapseable;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool scrollbar = C.IsMainNoWindowScrollbar;
        if (ImGui.Checkbox("Main Window Scrollbar", ref scrollbar))
        {
            C.IsMainNoWindowScrollbar = scrollbar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (scrollbar)
        {
            ImGui.SameLine();
            bool scrollWithMouse = C.IsMainWindowNoScrollWithMouse;
            if (ImGui.Checkbox("Main Window Scroll With Mouse", ref scrollWithMouse))
            {
                C.IsMainWindowNoScrollWithMouse = scrollWithMouse;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool noBackground = C.IsMainWindowNoBackground;
        if (ImGui.Checkbox("Background", ref noBackground))
        {
            C.IsMainWindowNoBackground = noBackground;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }
    }

    private void DrawConfigWindowSettings()
    {
        bool movable = C.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Configs Window", ref movable))
        {
            C.IsConfigWindowMovable = movable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        bool resizeable = C.IsConfigWindowResizeable;
        if (ImGui.Checkbox("Resizeable Configs Window", ref resizeable))
        {
            C.IsConfigWindowResizeable = resizeable;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }


        bool titleBar = C.IsConfigWindowNoTitleBar;
        if (ImGui.Checkbox("Title bar", ref titleBar))
        {
            C.IsConfigWindowNoTitleBar = titleBar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed


        }

        if (titleBar)
        {
            ImGui.SameLine();
            bool collapseable = C.IsConfigWindowNoCollapseable;
            if (ImGui.Checkbox("Collapseable", ref collapseable))
            {
                C.IsConfigWindowNoCollapseable = collapseable;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }

        bool scrollbar = C.IsConfigNoWindowScrollbar;
        if (ImGui.Checkbox("Scrollbar", ref scrollbar))
        {
            C.IsConfigNoWindowScrollbar = scrollbar;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }

        if (scrollbar)
        {
            ImGui.SameLine();
            bool scrollWithMouse = C.IsConfigWindowNoScrollWithMouse;
            if (ImGui.Checkbox("Scroll With Mouse", ref scrollWithMouse))
            {
                C.IsConfigWindowNoScrollWithMouse = scrollWithMouse;
                EzConfig.Save();
                //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
            }
        }


        bool background = C.IsConfigWindowNoBackground;
        if (ImGui.Checkbox("Background", ref background))
        {
            C.IsConfigWindowNoBackground = background;
            EzConfig.Save();
            //plugin.Config.Save(); //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed
        }
    }

    private string _retainerName = string.Empty; // To store the inputted retainer name

    private unsafe void DrawTab4()
    {
        RetainerManager* retainerMAnager = RetainerManager.Instance();
        Span<byte> CurrentRetainer = retainerMAnager->DisplayOrder;
        Retainer* activeRetainer = retainerMAnager->GetActiveRetainer();

        string Name =  activeRetainer->NameString;
        string retainerId = retainerMAnager->RetainerObjectId.ToString();
        byte Level =  activeRetainer->Level;
        int MarketItemCount = activeRetainer->MarketItemCount;
        uint Gil = activeRetainer->Gil;

        if (ImGui.Button(nameof(AutoMarktTasks.CloseRetainerList)))
        {
            AutoMarktTasks.CloseRetainerList();
        };

        // Button to trigger retainer selection
        if (ImGui.Button("Select Retainer"))
        {
            try
            {
                // Call the SelectRetainerByName method with the inputted name
                var result = AutoMarktTasks.SelectRetainerByName(_retainerName);

                if (result == true)
                {
                    ImGui.Text("Retainer selected successfully.");
                }
                else
                {
                    ImGui.Text("Failed to select retainer.");
                }
            }
            catch (Exception ex)
            {
                // Display error message
                ImGui.Text($"Error: {ex.Message}");
            }
        }
        ImGui.SameLine();
        // Input field for the retainer name
        ImGui.SetNextItemWidth(130);
        ImGui.InputText("Retainer Name", ref _retainerName, 100);



        if (ImGui.Button("Select Sell Item List"))
        {
            AutoMarktTasks.SelectSellItemsInIventoryOnTheMarket();
        }

        if (ImGui.Button("ClickSellingItem 'index'"))
        {

        }

        if (ImGui.Button("ClickAdjustPrice\n(Context Menu)"))
        {
            AutoMarktTasks.ClickAdjustPrice();
        }





        if (ImGui.Button("Step ClickComparePrice"))
        {

        }

        if (ImGui.Button("Step GetLowestPrice"))
        {

        }


        if (ImGui.Button("Step FillLowestPrice"))
        {

        }


        if (ImGui.Button("Step Return to RetainerSellList"))
        {

        }
        if (ImGui.Button("Step Select next Item in list"))
        {

        }
        if (ImGui.Button("Step GetLowestPrice"))
        {

        }

    }

    private unsafe void DrawTabDebug()
    {
        // Display debug information here
        foreach (CollapsingHeaders header in Enum.GetValues(typeof(CollapsingHeaders)))
        {
            bool isOpen = Sidebar.selectedHeader == header;
            ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f), $"Header: {header}, Is Open: {isOpen}");

        }
        ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f), $"Selected Header: {Sidebar.selectedHeader}");


    }
}
