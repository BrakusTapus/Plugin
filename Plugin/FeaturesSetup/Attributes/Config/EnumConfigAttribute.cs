using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Plugin.Utilities;
using Plugin.Utilities.UI;
using System.Reflection;

namespace Plugin.FeaturesSetup.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumConfigAttribute : BaseConfigAttribute
{
    public bool NoLabel = false;

    public override void Draw(Tweak tweak, object config, FieldInfo fieldInfo)
    {
        var enumType = fieldInfo.FieldType;
        var attr = fieldInfo.GetCustomAttribute<BaseConfigAttribute>();

        string GetOptionLabel(int value) => $"{Enum.GetName(enumType, value)}";

        if (!NoLabel)
        {
            ImGui.TextUnformatted(fieldInfo.Name.SplitWords());
        }

        using var indent = ImGuiExtKirbo.ConfigIndent(!NoLabel);

        var selectedValue = Convert.ToInt32(fieldInfo.GetValue(config) ?? 0);
        using var combo = ImRaii.Combo("##Input", GetOptionLabel(selectedValue));
        if (combo.Success)
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                var value = Convert.ToInt32(Enum.Parse(enumType, name));

                if (ImGui.Selectable(GetOptionLabel(value), selectedValue == value))
                {
                    fieldInfo.SetValue(config, Enum.ToObject(fieldInfo.FieldType, value));
                    OnChangeInternal(tweak, fieldInfo);
                }

                if (selectedValue == value)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
        }
        combo?.Dispose();
        if (!attr?.Description.IsNullOrEmpty() ?? false)
        {
            ImGuiHelpers.SafeTextColoredWrapped(ColorEx.Grey, attr!.Description);
        }
    }
}
