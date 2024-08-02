using ImGuiNET;

namespace ImGuiExtensions;

//TODO: Add this to the ImGuiEx namespace (ImGui folder) 
public static partial class ImGuiExt
{
    public const string TOOLTIP_ID = "##ToolTip_ID";

    public static readonly ImGuiWindowFlags TOOLTIP_FLAG =
          ImGuiWindowFlags.Tooltip |
          ImGuiWindowFlags.NoMove |
          ImGuiWindowFlags.NoSavedSettings |
          ImGuiWindowFlags.NoBringToFrontOnFocus |
          ImGuiWindowFlags.NoDecoration |
          ImGuiWindowFlags.NoInputs |
          ImGuiWindowFlags.AlwaysAutoResize;
}
