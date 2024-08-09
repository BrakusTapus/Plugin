using ImGuiExtensions;
using ImGuiNET;
using Plugin.Utilities;
using System.Reflection;

namespace Plugin.FeaturesSetup.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class StringConfigAttribute : BaseConfigAttribute
{
    public string DefaultValue = string.Empty;

    public override void Draw(Tweak tweak, object config, FieldInfo fieldInfo)
    {
        var value = (string)fieldInfo.GetValue(config)!;
        var attr = fieldInfo.GetCustomAttribute<BaseConfigAttribute>();

        ImGui.TextUnformatted(fieldInfo.Name.SplitWords());

        if (ImGui.InputText("##Input", ref value, 50))
        {
            fieldInfo.SetValue(config, value);
            OnChangeInternal(tweak, fieldInfo);
        }

        if (DrawResetButton(DefaultValue))
        {
            fieldInfo.SetValue(config, DefaultValue);
            OnChangeInternal(tweak, fieldInfo);
        }

        if (!attr?.Description.IsNullOrEmpty() ?? false)
            ImGuiHelpers.SafeTextColoredWrapped(ColorEx.Grey, attr!.Description);
    }
}
