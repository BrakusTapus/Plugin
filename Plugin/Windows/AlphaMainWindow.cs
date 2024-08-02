using Dalamud.Interface.Components;
using ECommons.Configuration;
using ImGuiExtensions;
using ImGuiNET;
using Plugin.Tasks.SameWorld;
// using Plugin.Configurations; //TODO: If migrating to Ecommons EzConfig is succesfol then this can be removed

namespace Plugin.Windows;

public class AlphaMainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly float _headerFooterHeight = 40f;

    // We give this window a hidden ID using ##
    // So that the user will see "Main Window" as window title,
    // but for ImGui the ID is "My Amazing Window##MainMenu"
    public AlphaMainWindow(Plugin plugin) : base($"{nameof(Plugin)}-{nameof(AlphaMainWindow)}##0001", ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar)
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

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
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
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 3f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(5, 5));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(5, 4));
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

    public override void Draw()
    {

        if (ImGui.BeginTabBar("PluginSettings"))
        {
            if (ImGui.BeginTabItem("Main Menu"))
            {
                DrawTab1();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Settings"))
            {
                DrawTab2();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Commands"))
            {
                ImGui.BeginChild("Commands##0001");
                DrawTab3();
                ImGui.EndChild();
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
        var save = false;
        if (ImGuiExt.BeginGroupBox("", 0.5f))
        {
            //save |= ImGui.Checkbox("Enable Auto Dismount", ref plugin.EzConfigs.EnableAutoDismount);
            //ImGuiExt.NewTooltip("Automatically dismounts when an action is used, prior to using the action.");

            ImGuiExt.EndGroupBox();
        }

        ImGui.SameLine();

        if (ImGuiExt.BeginGroupBox("", 0.5f))
        {

            ImGuiExt.EndGroupBox();

        }
    }

    public static readonly char[] InstanceNumbers = "\0".ToCharArray();
    private void DrawTab3()
    {
        ImGui.Text($"Number of current Tasks: {P.TaskManager.NumQueuedTasks + (P.TaskManager.IsBusy ? 1 : 0)}");
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
}