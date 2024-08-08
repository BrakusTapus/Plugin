using Dalamud.Interface.Components;
using ECommons.Configuration;
using ImGuiExtensions;
using ImGuiNET;
using Plugin.Utility.Extensions;
// using Plugin.Configurations; //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed

namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly float _headerFooterHeight = 40f;

    // We give this window a hidden ID using ##
    // So that the user will see "Main Window" as window title,
    // but for ImGui the ID is "My Amazing Window##MainMenu"
    public MainWindow(Plugin plugin) : base($"{nameof(Plugin)}-{nameof(MainWindow)}##0001", ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar)
    {
        this.plugin = plugin;

        float mainViewPortWidth = ImGuiHelpers.MainViewport.Size.X - ImGui.GetStyle().DisplaySafeAreaPadding.X;
        float mainViewPortHeight = ImGuiHelpers.MainViewport.Size.Y - ImGui.GetStyle().DisplaySafeAreaPadding.Y;
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(830, 570),
            MaximumSize = new Vector2(mainViewPortWidth, mainViewPortHeight)
        };
        RespectCloseHotkey = true;
        OnCloseSfxId = 24;
        OnOpenSfxId = 23;

        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
    }

    public override void PreDraw()
    {
        // Flags must be added or removed before DrawImage() is being called, or they won't apply
        if (plugin.EzConfigs.IsMainWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }

        if (plugin.EzConfigs.IsMainWindowResizeable)
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        if (plugin.EzConfigs.IsMainWindowNoTitleBar)
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }

        if (plugin.EzConfigs.IsMainNoWindowScrollbar)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollbar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollbar;
        }

        if (plugin.EzConfigs.IsMainWindowNoScrollWithMouse)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollWithMouse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        if (plugin.EzConfigs.IsMainWindowNoCollapseable)
        {
            Flags &= ~ImGuiWindowFlags.NoCollapse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        if (plugin.EzConfigs.IsMainWindowNoBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1f);
        //ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0.5f);
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        //ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 3f);
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(5, 5));
        //ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(5, 4));
        //ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 10);
        //ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.9f, 0.5f));
        //ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0.5f, 0.5f));

        //ImGui.PushStyleColor(ImGuiCol.Border, ColorEx.Transparent);
        //ImGui.PushStyleColor(ImGuiCol.Separator, ColorEx.DalamudOrangeVector);

        base.PreDraw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar(1);
        //ImGui.PopStyleColor(2);
        base.PostDraw();
    }

    public override void Draw()
    {
        ChildWindow.DrawHeader();
        ChildWindow.DrawSideBar();
        ChildWindow.DrawContent();
        ChildWindow.DrawFooter();
    }

    //private void DrawHeader()
    //{
    //    ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
    //    Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();

    //    var right = ImGui.GetContentRegionMax().X;
    //    ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f)); // Dark footer color
    //    ImGui.BeginChild("Header", new Vector2(0, _headerFooterHeight - ImGui.GetStyle().WindowPadding.Y), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
    //    ImGui.SetCursorPosX(right - 35);
    //    if (Buttons.IconButton(FontAwesomeIcon.Times, ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered, ColorEx.RedBright, ColorEx.ParsedOrange, 1))
    //    {
    //        Plugin.ToggleMainWindow();
    //    }
    //    ImGui.EndChild();
    //    ImGui.Separator();
    //    ImGui.PopStyleColor(1);
    //}


    //private void DrawBody()
    //{
    //    ImGui.SetCursorPos(new Vector2(ImGui.GetStyle().WindowPadding.X , _headerFooterHeight));
    //    float x = ImGui.GetStyle().WindowPadding.X * 2f;
    //    float windowX = ImGui.GetContentRegionAvail().X - x;
    //    ImGui.BeginChild("MainContent", new Vector2(windowX, -(_headerFooterHeight + 5)), true);
    //    ImGui.Text("Welcome to the Main Window!");
    //    ImGui.Spacing();
    //    ImGui.Text($"The random config bool is {plugin.EzConfigs.SomePropertyToBeSavedAndWithADefault}");
    //    ImGui.Spacing();

    //    ImGui.EndChild();
    //}


    //private void DrawFooter()
    //{

    //    //ImGui.SetCursorPosY(-ImGui.GetContentRegionAvail().Y - _headerFooterHeight - ImGui.GetStyle().FramePadding.X);
    //    ImGui.Separator();
    //    ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f)); // Dark footer color
    //    ImGui.BeginChild("Footer", new Vector2(0, _headerFooterHeight), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
    //    if (ImGuiComponents.IconButtonWithText(Dalamud.Interface.FontAwesomeIcon.Cog, "Settings", ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
    //    {
    //        Plugin.ToggleConfigWindow();
    //    }
    //    ImGui.EndChild();
    //    ImGui.PopStyleColor(1);
    //}
}