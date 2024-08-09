using Dalamud.Interface;
using ImGuiExtensions;

namespace Plugin.FeaturesSetup.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ConfigInfoAttribute(string label, string desc) : Attribute
{
    public string Label { get; init; } = label;
    public string Description { get; init; } = desc;
    public FontAwesomeIcon Icon { get; init; } = FontAwesomeIcon.InfoCircle;
    internal KirboColor Color { get; init; } = ColorEx.Grey;
}
