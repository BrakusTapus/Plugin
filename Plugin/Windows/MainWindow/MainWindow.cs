namespace Plugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "Main Window" as window title,
    // but for ImGui the ID is "My Amazing Window##MainMenu"
    public MainWindow(Plugin plugin) : base($"{nameof(Plugin)}-{nameof(MainWindow)}##0001", ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar)
    {
        this.plugin = plugin;

        float mainViewPortWidth = ImGuiHelpers.MainViewport.Size.X - ImGui.GetStyle().DisplaySafeAreaPadding.X;
        float mainViewPortHeight = ImGuiHelpers.MainViewport.Size.Y - ImGui.GetStyle().DisplaySafeAreaPadding.Y;
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(656, 386),
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
        if (C.IsMainWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }

        if (C.IsMainWindowResizeable)
        {
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        if (C.IsMainWindowNoTitleBar)
        {
            Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoTitleBar;
        }

        if (C.IsMainNoWindowScrollbar)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollbar;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollbar;
        }

        if (C.IsMainWindowNoScrollWithMouse)
        {
            Flags &= ~ImGuiWindowFlags.NoScrollWithMouse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        if (C.IsMainWindowNoCollapseable)
        {
            Flags &= ~ImGuiWindowFlags.NoCollapse;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        if (C.IsMainWindowNoBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 6f);
        //ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGuiExtensions.ColorEx.TransParentDarkType);

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
        ImGui.PopStyleVar(2);
        //ImGui.PopStyleColor(1);
        base.PostDraw();
    }

    public override void Draw()
    {
        ChildWindow.DrawHeader();
        ChildWindow.DrawSideBar();
        ImGui.SameLine();
        ChildWindow.DrawContent();
        ChildWindow.DrawFooter();
    }
}