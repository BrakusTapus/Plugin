
using ImGuiScene;
using ImGuiNET;

namespace ImGuiExtensions;

public static partial class ImGuiExt
{



    // Example of a method to draw a separator with custom thickness
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

    /// <summary>
    /// Example of a method to draw a label with custom color
    /// </summary>
    /// <param name="label">Label text.</param>
    /// <param name="color">Color of the label.</param>
    public static void ColoredLabel(string label, Vector4 color)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text(label);
        ImGui.PopStyleColor();
    }

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

    /// <summary>
    /// ColorPicker with palette.
    /// </summary>
    /// <param name="id">Id for the color picker.</param>
    /// <param name="description">The description of the color picker.</param>
    /// <param name="originalColor">The current color.</param>
    /// <returns>Selected color.</returns>
    public static Vector4 ColorPickerWithPalette(int id, string description, Vector4 originalColor)
    {
        return ColorPickerWithPalette(id, description, originalColor, ImGuiColorEditFlags.NoSmallPreview | ImGuiColorEditFlags.NoSidePreview);
    }

    /// <summary>
    /// ColorPicker with palette with color picker options.
    /// </summary>
    /// <param name="id">Id for the color picker.</param>
    /// <param name="description">The description of the color picker.</param>
    /// <param name="originalColor">The current color.</param>
    /// <param name="flags">Flags to customize color picker.</param>
    /// <returns>Selected color.</returns>
    public static Vector4 ColorPickerWithPalette(int id, string description, Vector4 originalColor, ImGuiColorEditFlags flags)
    {
        Vector4 col = originalColor;
        Vector4 result = originalColor;
        List<Vector4> list = ImGuiHelpers.DefaultColorPalette(36);
        if (ImGui.ColorButton($"{description}###ColorPickerButton{id}", originalColor))
        {
            ImGui.OpenPopup($"###ColorPickerPopup{id}");
        }

        if (ImGui.BeginPopup($"###ColorPickerPopup{id}"))
        {
            if (ImGui.ColorPicker4($"###ColorPicker{id}", ref col, flags))
            {
                result = col;
            }

            for (int i = 0; i < 4; i++)
            {
                ImGui.Spacing();
                for (int j = i * 9; j < i * 9 + 9; j++)
                {
                    if (ImGui.ColorButton($"###ColorPickerSwatch{id}{i}{j}", list[j]))
                    {
                        result = list[j];
                    }

                    ImGui.SameLine();
                }
            }

            ImGui.EndPopup();
        }

        return result;
    }



    /// <summary>
    /// Alpha modified IconButton component to use an icon as a button with alpha and
    /// color options.
    /// </summary>
    /// <param name="icon">The icon for the button.</param>
    /// <param name="id">The ID of the button.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <param name="alphaMult">A multiplier for the current alpha levels.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool DisabledButton(FontAwesomeIcon icon, int? id = null, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, float alphaMult = 0.5f)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        string text = icon.ToIconString();
        if (id.HasValue)
        {
            text = $"{text}##{id}";
        }

        bool result = DisabledButton(text, defaultColor, activeColor, hoveredColor, alphaMult);
        ImGui.PopFont();
        return result;
    }

    /// <summary>
    /// Alpha modified Button component to use as a disabled button with alpha and color
    /// options.
    /// </summary>
    /// <param name="labelWithId">The button label with ID.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <param name="alphaMult">A multiplier for the current alpha levels.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool DisabledButton(string labelWithId, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, float alphaMult = 0.5f)
    {
        if (defaultColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, defaultColor.Value);
        }

        if (activeColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.Value);
        }

        if (hoveredColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.Value);
        }

        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * alphaMult);
        bool result = ImGui.Button(labelWithId);
        ImGui.PopStyleVar();
        if (defaultColor.HasValue)
        {
            ImGui.PopStyleColor();
        }

        if (activeColor.HasValue)
        {
            ImGui.PopStyleColor();
        }

        if (hoveredColor.HasValue)
        {
            ImGui.PopStyleColor();
        }

        return result;
    }



    /// <summary>
    /// HelpMarker component to add a help icon with text on hover.
    /// </summary>
    /// <param name="helpText">The text to display on hover.</param>
    public static void HelpMarker(string helpText)
    {
        HelpMarker(helpText, FontAwesomeIcon.InfoCircle);
    }

    /// <summary>
    /// HelpMarker component to add a custom icon with text on hover.
    /// </summary>
    /// <param name="helpText">The text to display on hover.</param>
    /// <param name="icon">The icon to use.</param>
    public static void HelpMarker(string helpText, FontAwesomeIcon icon)
    {
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextDisabled(icon.ToIconString());
        ImGui.PopFont();
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGui.TextUnformatted(helpText);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }



    /// <summary>
    /// IconButton component to use an icon as a button.
    /// </summary>
    /// <param name="icon">The icon for the button.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(FontAwesomeIcon icon)
    {
        return IconButton(icon, null, null, null, null, null, 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="icon">The icon for the button.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(int id, FontAwesomeIcon icon)
    {
        return IconButton(id, icon, null, null, null, null, null, 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="icon">The icon for the button.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(string id, FontAwesomeIcon icon)
    {
        return IconButton(id, icon, null, null, null, null, null, 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button.
    /// </summary>
    /// <param name="iconText">Text already containing the icon string.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(string iconText)
    {
        return IconButton(iconText, null, null, null, null, null, 0);
    }



    public static bool IconButton(FontAwesomeIcon icon, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, Vector4? buttonColor = null, Vector4? borderColor = null, float borderSize = 0)
    {
        return IconButton(icon.ToIconString() ?? "", defaultColor, activeColor, hoveredColor, buttonColor, borderColor, borderSize = 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button.
    /// </summary>
    /// <param name="icon">The icon for the button.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(int id, FontAwesomeIcon icon, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, Vector4? buttonColor = null, Vector4? borderColor = null, float borderSize = 0)
    {
        return IconButton($"{icon.ToIconString()}##{id}", defaultColor, activeColor, hoveredColor, buttonColor, borderColor, borderSize = 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button with color options.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="icon">The icon for the button.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(string id, FontAwesomeIcon icon, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, Vector4? buttonColor = null, Vector4? borderColor = null, float borderSize = 0)
    {
        return IconButton(icon.ToIconString() + "##" + id, defaultColor, activeColor, hoveredColor, buttonColor, borderColor, borderSize = 0);
    }

    /// <summary>
    /// IconButton component to use an icon as a button with color options.
    /// </summary>
    /// <param name="iconText">Text already containing the icon string.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButton(string iconText, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null, Vector4? buttonColor = null, Vector4? borderColor = null, float borderSize = 0)
    {
        int num = 0;
        if (defaultColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, defaultColor.Value);
            num++;
        }

        if (activeColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.Value);
            num++;
        }

        if (hoveredColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.Value);
            num++;
        }

        if (buttonColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, buttonColor.Value);
            num++;
        }

        int numVar = 0;
        if (borderSize > 0)
        {
            ImGui.PushStyleColor(ImGuiCol.Border, borderColor.Value);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
            num++;
            numVar++;
        }

        string text = iconText;
        if (text.Contains("#"))
        {
            text = text.Substring(0, text.IndexOf("#", StringComparison.Ordinal));
        }

        ImGui.PushID(iconText);
        ImGui.PushFont(UiBuilder.IconFont);
        Vector2 vector = ImGui.CalcTextSize(text);
        ImGui.PopFont();
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        float x = vector.X + ImGui.GetStyle().FramePadding.X * 2f;
        float frameHeight = ImGui.GetFrameHeight();
        bool result = ImGui.Button(string.Empty, new Vector2(x, frameHeight));
        Vector2 pos = new Vector2(cursorScreenPos.X + ImGui.GetStyle().FramePadding.X, cursorScreenPos.Y + ImGui.GetStyle().FramePadding.Y);
        ImGui.PushFont(UiBuilder.IconFont);
        windowDrawList.AddText(pos, ImGui.GetColorU32(ImGuiCol.Text), text);
        ImGui.PopFont();
        ImGui.PopID();
        if (num > 0)
        {
            ImGui.PopStyleColor(num);
        }
        if (numVar > 0)
        {
            ImGui.PopStyleVar(numVar);
        }

        return result;
    }

    /// <summary>
    /// IconButton component to use an icon as a button with color options.
    /// </summary>
    /// <param name="icon">Icon to show.</param>
    /// <param name="text">Text to show.</param>
    /// <param name="defaultColor">The default color of the button.</param>
    /// <param name="activeColor">The color of the button when active.</param>
    /// <param name="hoveredColor">The color of the button when hovered.</param>
    /// <returns>Indicator if button is clicked.</returns>
    public static bool IconButtonWithText(FontAwesomeIcon icon, string text, Vector4? defaultColor = null, Vector4? activeColor = null, Vector4? hoveredColor = null)
    {
        int num = 0;
        if (defaultColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, defaultColor.Value);
            num++;
        }

        if (activeColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.Value);
            num++;
        }

        if (hoveredColor.HasValue)
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.Value);
            num++;
        }

        ImGui.PushID(text);
        ImGui.PushFont(UiBuilder.IconFont);
        Vector2 vector = ImGui.CalcTextSize(icon.ToIconString());
        ImGui.PopFont();
        Vector2 vector2 = ImGui.CalcTextSize(text);
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        float num2 = 3f * ImGuiHelpers.GlobalScale;
        float x = vector.X + vector2.X + ImGui.GetStyle().FramePadding.X * 2f + num2;
        float frameHeight = ImGui.GetFrameHeight();
        bool result = ImGui.Button(string.Empty, new Vector2(x, frameHeight));
        Vector2 pos = new Vector2(cursorScreenPos.X + ImGui.GetStyle().FramePadding.X, cursorScreenPos.Y + ImGui.GetStyle().FramePadding.Y);
        ImGui.PushFont(UiBuilder.IconFont);
        windowDrawList.AddText(pos, ImGui.GetColorU32(ImGuiCol.Text), icon.ToIconString());
        ImGui.PopFont();
        Vector2 pos2 = new Vector2(pos.X + vector.X + num2, cursorScreenPos.Y + ImGui.GetStyle().FramePadding.Y);
        windowDrawList.AddText(pos2, ImGui.GetColorU32(ImGuiCol.Text), text);
        ImGui.PopID();
        if (num > 0)
        {
            ImGui.PopStyleColor(num);
        }

        return result;
    }

    /// <summary>
    /// Get width of IconButtonWithText component.
    /// </summary>
    /// <param name="icon">Icon to use.</param>
    /// <param name="text">Text to use.</param>
    /// <returns>Width.</returns>
    internal static float GetIconButtonWithTextWidth(FontAwesomeIcon icon, string text)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        Vector2 vector = ImGui.CalcTextSize(icon.ToIconString());
        ImGui.PopFont();
        Vector2 vector2 = ImGui.CalcTextSize(text);
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        float num = 3f * ImGuiHelpers.GlobalScale;
        return vector.X + vector2.X + ImGui.GetStyle().FramePadding.X * 2f + num;
    }



    /// <summary>
    /// Test component to demonstrate how ImGui components work.
    /// </summary>
    public static void Test()
    {
        ImGui.Text("You are viewing the test component. The test was a success.");
    }

    /// <summary>
    /// TextWithLabel component to show labeled text.
    /// </summary>
    /// <param name="label">The label for text.</param>
    /// <param name="value">The text value.</param>
    /// <param name="hint">The hint to show on hover.</param>
    public static void TextWithLabel(string label, string value, string hint = "")
    {
        ImGui.Text(label + ": ");
        ImGui.SameLine();
        if (string.IsNullOrEmpty(hint))
        {
            ImGui.Text(value);
            return;
        }

        ImGui.Text(value + "*");
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(hint);
        }
    }

    /// <summary>
    /// Draw a toggle button.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    /// <param name="v">The state of the switch.</param>
    /// <returns>If the button has been interacted with this frame.</returns>
    public static bool ToggleButton(string id, ref bool v)
    {
        RangeAccessor<Vector4> colors = ImGui.GetStyle().Colors;
        Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        float frameHeight = ImGui.GetFrameHeight();
        float num = frameHeight * 1.55f;
        float num2 = frameHeight * 0.5f;
        bool result = false;
        ImGui.InvisibleButton(id, new Vector2(num, frameHeight));
        if (ImGui.IsItemClicked())
        {
            v = !v;
            result = true;
        }

        if (ImGui.IsItemHovered())
        {
            windowDrawList.AddRectFilled(cursorScreenPos, new Vector2(cursorScreenPos.X + num, cursorScreenPos.Y + frameHeight), ImGui.GetColorU32((!v) ? colors[23] : new Vector4(0.78f, 0.78f, 0.78f, 1f)), frameHeight * 0.5f);
        }
        else
        {
            windowDrawList.AddRectFilled(cursorScreenPos, new Vector2(cursorScreenPos.X + num, cursorScreenPos.Y + frameHeight), ImGui.GetColorU32((!v) ? (colors[21] * 0.6f) : new Vector4(0.35f, 0.35f, 0.35f, 1f)), frameHeight * 0.5f);
        }

        windowDrawList.AddCircleFilled(new Vector2(cursorScreenPos.X + num2 + (float)(v ? 1 : 0) * (num - num2 * 2f), cursorScreenPos.Y + num2), num2 - 1.5f, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));
        return result;
    }



    /// <summary>
    /// Draw a disabled toggle button.
    /// </summary>
    /// <param name="id">The id of the button.</param>
    /// <param name="v">The state of the switch.</param>
    public static void DisabledToggleButton(string id, bool v)
    {
        RangeAccessor<Vector4> colors = ImGui.GetStyle().Colors;
        Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        float frameHeight = ImGui.GetFrameHeight();
        float num = frameHeight * 1.55f;
        float num2 = frameHeight * 0.5f;
        ImGui.InvisibleButton(id, new Vector2(num, frameHeight));
        float num3 = 0.5f;
        windowDrawList.AddRectFilled(cursorScreenPos, new Vector2(cursorScreenPos.X + num, cursorScreenPos.Y + frameHeight), ImGui.GetColorU32(v ? (colors[21] * num3) : (new Vector4(0.55f, 0.55f, 0.55f, 1f) * num3)), frameHeight * 0.5f);
        windowDrawList.AddCircleFilled(new Vector2(cursorScreenPos.X + num2 + (float)(v ? 1 : 0) * (num - num2 * 2f), cursorScreenPos.Y + num2), num2 - 1.5f, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f) * num3));
    }
}
