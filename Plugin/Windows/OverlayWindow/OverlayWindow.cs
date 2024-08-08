using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Plugin.Windows;

public class OverlayWindow : Window
{
    public OverlayWindow()
        : base(nameof(OverlayWindow), ImGuiNET.ImGuiWindowFlags.None, true)
    {
        IsOpen = false;
        AllowClickthrough = true;
        RespectCloseHotkey = true;
    }

    public override void PreDraw()
    {
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        //ImGuiHelpers.SetNextWindowPosRelativeMainViewport(Vector2.Zero);
        //ImGui.SetNextWindowSize(ImGuiHelpers.MainViewport.Size);

        base.PreDraw();
    }

    public override void Draw()
    {
        throw new NotImplementedException();
    }

    public override bool DrawConditions()
    {
        return base.DrawConditions();
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}
