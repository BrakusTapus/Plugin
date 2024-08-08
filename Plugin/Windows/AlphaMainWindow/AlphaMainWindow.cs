using ECommons.Configuration;
using ImGuiExtensions;

using Plugin.Tasks.SameWorld;

using Dalamud.Configuration.Internal;
using Dalamud.Console;
using Dalamud.Game.Command;
using Dalamud.Interface.Animation.EasingFunctions;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.ImGuiNotification.Internal;
using Dalamud.Interface.Textures.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Logging.Internal;
using Dalamud.Plugin;
using Dalamud.Plugin.Internal;
using Dalamud.Plugin.Internal.Exceptions;
using Dalamud.Plugin.Internal.Profiles;
using Dalamud.Plugin.Internal.Types;
using Dalamud.Plugin.Internal.Types.Manifest;
using Dalamud.Support;
using Dalamud.Utility;

using ImGuiNET;
using Plugin.Configuration;

namespace Plugin.Windows.AlphaMainWindow;

public class AlphaMainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    // private readonly float _headerFooterHeight = 40f;

    private string searchText = string.Empty;
    private bool isSearchTextPrefilled = false;

    // We give this window a hidden ID using ##
    // So that the user will see "Main Window" as window title,
    // but for ImGui the ID is "My Amazing Window##MainMenu"
    public AlphaMainWindow(Plugin plugin, Configs configs)
        : base(
            $"{nameof(MainWindow)}" + "###AlphaVersion",
            ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar)
    {
        this.IsOpen = true;

        float mainViewPortWidth = ImGuiHelpers.MainViewport.Size.X;
        float mainViewPortHeight = ImGuiHelpers.MainViewport.Size.Y;

        this.Size = new Vector2(500, 300);
        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.SizeConstraints = new WindowSizeConstraints {
            MinimumSize = this.Size.Value,
            MaximumSize = new Vector2(mainViewPortWidth, mainViewPortHeight)
        };

        this.plugin = plugin;

        RespectCloseHotkey = true;
        OnCloseSfxId = 24;
        OnOpenSfxId = 23;

        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
    }

    private enum PluginSortKind
    {
        Alphabetical,
        TurnedOff,
        EnabledDisabled,
    }

    public void Dispose()
    {

    }

    /// <summary>
    /// Open to the installer to the page specified by <paramref name="kind"/>.
    /// </summary>
    /// <param name="kind">The page of the installer to open.</param>
    //public void OpenTo(PluginOpenKind kind)
    //{
    //    this.IsOpen = true;
    //    this.SetOpenPage(kind);
    //}

    /// <summary>
    /// Toggle to the installer to the page specified by <paramref name="kind"/>.
    /// </summary>
    /// <param name="kind">The page of the installer to open.</param>
    //public void ToggleTo(PluginOpenKind kind)
    //{
    //    this.Toggle();

    //    if (this.IsOpen)
    //        this.SetOpenPage(kind);
    //}

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 3f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(5, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 10);
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.9f, 0.5f));
        ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0.5f, 0.5f));

        ImGui.PushStyleColor(ImGuiCol.Border, ColorEx.Transparent);
        ImGui.PushStyleColor(ImGuiCol.Separator, ColorEx.DalamudOrangeVector);

        base.PreDraw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar(10);
        ImGui.PopStyleColor(2);
        base.PostDraw();
    }

    /// <inheritdoc/>
    public override void OnOpen()
    {


    }

    /// <inheritdoc/>
    public override void OnClose()
    {
    }

    /// <inheritdoc/>
    public override void Draw()
    {

        if (ImGui.BeginTabBar("##PluginTabBar"))
        {
            if (ImGui.BeginTabItem("Main Menu"))
            {
                DrawTab1();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Commands"))
            {
                DrawCommandTab();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Settings"))
            {
                DrawSettings();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }


    }


    private void DrawTab1()
    {
        ImGui.Text("Hello! welcome to my plugin!");
    }

    private void DrawTab2()
    {
        ImGui.Text("Nothing just yet!");
    }

    public static readonly char[] InstanceNumbers = "\0".ToCharArray();
    private void DrawTab3()
    {
        ImGui.Text($"Number of tasks: {P.TaskManager.NumQueuedTasks + (P.TaskManager.IsBusy ? 1 : 0)}");
        ImGui.SameLine();
        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Times, "", ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
        {
            Notify.Info($"Discarding {P.TaskManager.NumQueuedTasks + (P.TaskManager.IsBusy ? 1 : 0)} tasks");
            P.TaskManager.Abort();
        }
        ImGuiExt.NewTooltip($"Reset the tasks back to 0\nUse This is tasks seems to be stuck");


        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.Text($"/kirboinstance <number>");
        ImGuiExt.NewTooltip("Switches your current instance to whatever number you used");

        ImGui.Spacing();

        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.DiceOne, "Instance 1", ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
        {
            TaskChangeInstance.Enqueue(1);
            DuoLog.Information($"Changing to instance: {1}");
        }
        ImGui.SameLine();
        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.DiceTwo, "Instance 2", ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
        {
            TaskChangeInstance.Enqueue(2);
            DuoLog.Information($"Changing to instance: {2}");
        }
        ImGui.SameLine();
        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.DiceThree, "Instance 3", ColorEx.Transparent, ColorEx.ButtonActive, ColorEx.TextHovered))
        {
            TaskChangeInstance.Enqueue(3);
            DuoLog.Information($"Changing to instance: {3}");
        }
    }

    public void DrawCommandTab()
    {
        ImGui.Text("Settings Commands");

        if (ImGui.CollapsingHeader("Command 1"))
        {
            ImGui.Text("Description: This is Command 1");
            ImGui.Text("Usage: /command1 <arg>");
            ImGui.InputText("Example Argument", ref exampleArg1, 100);
        }

        if (ImGui.CollapsingHeader("Command 2"))
        {
            ImGui.Text("Description: This is Command 2");
            ImGui.Text("Usage: /command2 <arg>");
            ImGui.InputText("Example Argument", ref exampleArg2, 100);
        }
    }

    private string exampleArg1 = string.Empty;
    private string exampleArg2 = string.Empty;


    private bool setting1;
    private float setting2;

    public void DrawSettings()
    {
        ImGui.Text("Plugin Settings");

        if (ImGui.Checkbox("Enable Feature 1", ref setting1))
        {
            // Handle feature 1 enable/disable
        }

        if (ImGui.SliderFloat("Feature 2 Intensity", ref setting2, 0.0f, 1.0f))
        {
            // Handle feature 2 intensity change
        }
    }

    /// <summary>
    /// Sets the current search text and marks it as prefilled.
    /// </summary>
    /// <param name="text">The search term.</param>
    public void SetSearchText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            this.isSearchTextPrefilled = false;
            this.searchText = string.Empty;
        }
        else
        {
            this.isSearchTextPrefilled = true;
            this.searchText = text;
        }
    }

}