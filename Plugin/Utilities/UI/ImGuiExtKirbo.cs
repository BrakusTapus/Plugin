using Dalamud.Interface.Utility.Raii;
using ECommons.Configuration;

namespace Plugin.Utilities.UI;

public static class ImGuiExtKirbo
{
    public static float Scale => ImGuiHelpers.GlobalScale;

    public const string TOOLTIP_ID = "##ToolTip_ID";

    /// <summary>
    /// Wether or not the item is in the viewport
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool IsInViewport(Vector2 size)
    {
        float distanceY = ImGui.GetCursorPosY() - ImGui.GetScrollY();
        return distanceY >= -size.Y && distanceY <= ImGui.GetWindowHeight();
    }

    public static void TextUnformattedColored(uint col, string text)
    {
        using (ImRaii.PushColor(ImGuiCol.Text, col))
            ImGui.TextUnformatted(text);
    }

    public static void PushCursor(Vector2 vec) => ImGui.SetCursorPos(ImGui.GetCursorPos() + vec);
    public static void PushCursor(float x, float y) => PushCursor(new Vector2(x, y));
    public static void PushCursorX(float x) => ImGui.SetCursorPosX(ImGui.GetCursorPosX() + x);
    public static void PushCursorY(float y) => ImGui.SetCursorPosY(ImGui.GetCursorPosY() + y);

    public static ImRaii.Indent ConfigIndent(bool enabled = true) => ImRaii.PushIndent(ImGui.GetFrameHeight() + ImGui.GetStyle().ItemSpacing.X / 2f, true, enabled);

    public static void Checkbox(string name, ref bool v)
    {
        if (ImGui.Checkbox(name, ref v))
            EzConfig.Save();
    }

    public static void Icon(FontAwesomeIcon icon, uint? col = null)
    {
        using var color = col != null ? ImRaii.PushColor(ImGuiCol.Text, (uint)col) : null;
        using (ImRaii.PushFont(UiBuilder.IconFont))
            ImGui.Text(icon.ToIconString());
    }

    public static void DrawSection(string Label, bool PushDown = true, bool RespectUiTheme = false, uint UIColor = 1, bool drawSeparator = true)
    {
        var style = ImGui.GetStyle();

        // push down a bit
        if (PushDown)
            PushCursorY(style.ItemSpacing.Y * 2);

        KirboColor color = ColorEx.Gold;

        TextUnformattedColored(color, Label);

        if (drawSeparator)
        {
            // pull up the separator
            PushCursorY(-style.ItemSpacing.Y + 3);
            ImGui.Separator();
            PushCursorY(style.ItemSpacing.Y * 2 - 1);
        }
    }

    /// <summary>
    /// Renders text aligned with a button on the Y-axis.
    /// </summary>
    /// <param name="text">Text to render.</param>
    /// <param name="buttonLabel">Label of the button to align with.</param>
    public static void TextAlignedWithButton(string text, string buttonLabel)
    {
        // Calculate the height of the button and the text
        float buttonHeight = ImGui.GetTextLineHeightWithSpacing();
        float textHeight = ImGui.CalcTextSize(text).Y;

        // Calculate the difference in height
        float alignmentOffset = (buttonHeight - textHeight) / 2.0f;

        // Adjust cursor Y position for vertical alignment
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + alignmentOffset);

        // Render the text
        ImGui.Text(text);

        // Reset the cursor position and render the button
        ImGui.SameLine();
        ImGui.Button(buttonLabel);
    }

    /// <summary>
    /// Centers an item horizontally in the current window.
    /// </summary>
    /// <param name="itemWidth">The width of the item to center.</param>
    public static void CenterItemHorizontally(float itemWidth)
    {
        float windowWidth = ImGui.GetWindowWidth();
        float centeredX = (windowWidth - itemWidth) / 2.0f;

        ImGui.SetCursorPosX(centeredX);
    }

    /// <summary>
    /// Centers an item vertically in the current window.
    /// </summary>
    /// <param name="itemHeight">The height of the item to center.</param>
    public static void CenterItemVertically(float itemHeight)
    {
        float windowHeight = ImGui.GetWindowHeight();
        float centeredY = (windowHeight - itemHeight) / 2.0f;

        ImGui.SetCursorPosY(centeredY);
    }

    #region ToolTips
    /// <summary>
    ///
    /// </summary>
    /// <param name="s"></param>
    public static void Tooltip(string s)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ImGui.BeginTooltip();
        ImGui.TextWrapped(s);
        ImGui.EndTooltip();
    }

    /// <summary>
    /// Sets and displays a colored text tooltip with the given text and color if the current item is hovered.
    /// </summary>
    public static void Tooltip(string s, Vector4 color)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ImGui.BeginTooltip();
        ImGui.TextColored(color, s);
        ImGui.EndTooltip();
    }

    /// <summary>
    /// Displays a tooltip with the given text if the current item is hovered.
    /// </summary>
    public static void NewTooltip(string s)
    {
        bool nullOrNoText = string.IsNullOrEmpty(s);
        bool notHovered = !ImGui.IsItemHovered();
        if (nullOrNoText || notHovered)
        {
            return;
        }

        ShowTooltip(() => ImGui.Text(s));
    }

    /// <summary>
    /// Displays a tooltip if the action is not null and tooltips are enabled in the configuration.
    /// </summary>
    public static void ShowTooltip(Action act)
    {
        if (act == null)
        {
            return;
        }

        ImGui.SetNextWindowBgAlpha(1);

        using var color = ImRaii.PushColor(ImGuiCol.BorderShadow, ColorEx.DalamudWhite);

        ImGui.SetNextWindowSizeConstraints(new Vector2(150, 0) * ImGuiHelpers.GlobalScale, new Vector2(1200, 1500) * ImGuiHelpers.GlobalScale);
        ImGui.SetWindowPos(TOOLTIP_ID, ImGui.GetIO().MousePos);

        if (ImGui.Begin(TOOLTIP_ID, TOOLTIP_FLAG))
        {
            act();
            ImGui.End();
        }
    }
    #endregion

    #region Sliders
    public static bool SliderIntAsFloat(string id, ref int value, int min, int max, float divider = 1)
    {
        float f = value / divider;
        bool ret = ImGui.SliderFloat(id, ref f, min / divider, max / divider);
        if (ret)
        {
            value = (int)(f * divider);
        }
        return ret;
    }

    public static bool SliderFloatAsInt(string id, ref float value, float min, float max, int divider = 1)
    {
        int i = (int)value / divider;
        bool ret = ImGui.SliderInt(id, ref i, (int)min / divider, (int)max / divider);
        if (ret)
        {
            value = i * divider;
        }
        return ret;
    }
    #endregion

    #region ImGuiWindowFlags

    public static readonly ImGuiWindowFlags TOOLTIP_FLAG =
          ImGuiWindowFlags.Tooltip |
          ImGuiWindowFlags.NoMove |
          ImGuiWindowFlags.NoSavedSettings |
          ImGuiWindowFlags.NoBringToFrontOnFocus |
          ImGuiWindowFlags.NoDecoration |
          ImGuiWindowFlags.NoInputs |
          ImGuiWindowFlags.AlwaysAutoResize;
    #endregion

    #region Seperators

    static unsafe Vector4 GetSeparatorCol()
    {
        // Access the current style
        var style = ImGui.GetStyle();

        // Retrieve the color value for the Separator from the Colors array
        Vector4 separatorCol = style.Colors[(int)ImGuiCol.Separator];

        return separatorCol;
    }

    /// <summary>
    /// Example of a method to create a horizontal line
    /// </summary>
    /// <param name="thickness">The thickness of the line.</param>
    /// <param name="color">The color of the line.</param>
    public static void HorizontalLine(float thickness = 1.0f, uint? color = null)
    {
        // Get the default separator color if no color is provided
        if (!color.HasValue)
        {
            Vector4 separatorColorVec4 = GetSeparatorCol();
            color = ImGui.ColorConvertFloat4ToU32(separatorColorVec4);
        }

        ImGui.PushStyleColor(ImGuiCol.Border, color.Value);
        ImGui.GetWindowDrawList().AddLine(
            new Vector2(ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y + ImGui.GetCursorPos().Y),
            new Vector2(ImGui.GetWindowPos().X + ImGui.GetContentRegionMax().X, ImGui.GetWindowPos().Y + ImGui.GetCursorPos().Y),
            color.Value,
            thickness
        );
        ImGui.PopStyleColor();
    }


    public static void CustomSeparator(float thickness = 1.0f)
    {
        var style = ImGui.GetStyle();
        var originalThickness = style.ItemSpacing.Y;

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, thickness));
        ImGui.Separator();
        ImGui.PopStyleVar();

        // Restore original spacing
        style.ItemSpacing = new Vector2(style.ItemSpacing.X, originalThickness);
    }

    #endregion
}


