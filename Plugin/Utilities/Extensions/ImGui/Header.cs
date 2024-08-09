namespace ImGuiExtensions;

public static class Header
{
    public record HeaderOptions
    {
        public bool Collapsible { get; init; } = false;
        public uint TextColor { get; init; } = ImGui.GetColorU32(ImGuiCol.Text);
        public Action TextAction { get; init; } = null;
        public uint BorderColor { get; init; } = ImGui.GetColorU32(ImGuiCol.Border);
        public Vector2 BorderPadding { get; init; } = ImGui.GetStyle().WindowPadding;
        public float BorderRounding { get; init; } = ImGui.GetStyle().FrameRounding;
        public ImDrawFlags DrawFlags { get; init; } = ImDrawFlags.None;
        public float BorderThickness { get; init; } = 2f;
        public float Width { get; set; }
        public float MaxX { get; set; }
    }

    private static readonly Stack<HeaderOptions> headerOptionsStack = new();

    public unsafe static bool BeginHeader(string? id = null, float minimumWindowPercent = 1.0f, HeaderOptions? options = null)
    {
        options ??= new HeaderOptions();
        ImGui.BeginGroup();

        bool open = true;
        if (!string.IsNullOrEmpty(id))
        {
            if (!options.Collapsible)
            {
                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(options.TextColor), id);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, options.TextColor);
                open = ImGui.TreeNodeEx(id, ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.DefaultOpen);
                ImGui.PopStyleColor();
            }

            options.TextAction?.Invoke();

            ImGui.Indent();
            ImGui.Unindent();
        }

        ImGuiStylePtr style = ImGui.GetStyle();
        float spacing = style.ItemSpacing.X * (1 - minimumWindowPercent);
        float contentRegionWidth = headerOptionsStack.TryPeek(out var parent) ? parent.Width - parent.BorderPadding.X * 2 : ImGui.GetWindowContentRegionMax().X - style.WindowPadding.X;
        float width = Math.Max((contentRegionWidth * minimumWindowPercent) - spacing, 1);
        options.Width = minimumWindowPercent > 0 ? width : 0;

        ImGui.BeginGroup();
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        ImGui.Dummy(options.BorderPadding with { X = width });
        ImGui.PopStyleVar();

        Vector2 max = ImGui.GetItemRectMax();
        options.MaxX = max.X;

        if (options.Width > 0)
        {
            ImGui.PushClipRect(ImGui.GetItemRectMin(), max with { Y = 10000 }, true);
        }

        ImGui.Indent(Math.Max(options.BorderPadding.X, 0.01f));
        ImGui.PushItemWidth(MathF.Floor((width - (options.BorderPadding.X * 2)) * 0.65f));

        headerOptionsStack.Push(options);
        if (open)
        {
            return true;
        }

        ImGui.TextDisabled(". . .");
        EndHeader();
        return false;
    }

    public static bool BeginHeader(string? text, HeaderOptions? options) => BeginHeader(text, 1.0f, options);

    public static bool BeginHeader(uint borderColor, float minimumWindowPercent = 1.0f) => BeginHeader(null, minimumWindowPercent, new HeaderOptions { BorderColor = borderColor });

    public unsafe static void EndHeader()
    {
        HeaderOptions options = headerOptionsStack.Pop();
        bool autoAdjust = options.Width <= 0;
        ImGui.PopItemWidth();
        ImGui.Unindent(Math.Max(options.BorderPadding.X, 0.01f));

        if (!autoAdjust)
        {
            ImGui.PopClipRect();
        }

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - ImGui.GetStyle().ItemSpacing.Y);
        ImGui.Dummy(options.BorderPadding with { X = 0 });

        if (!autoAdjust)
        {
            ImGuiWindow* window = Native.GetCurrentWindow();
            window->CursorMaxPos = window->CursorMaxPos with { X = options.MaxX };
        }

        ImGui.EndGroup();

        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = autoAdjust ? ImGui.GetItemRectMax() : ImGui.GetItemRectMax() with { X = options.MaxX };

        ImGui.GetWindowDrawList().AddRect(min, max, options.BorderColor, options.BorderRounding, options.DrawFlags, options.BorderThickness);

        ImGui.EndGroup();
    }
}
