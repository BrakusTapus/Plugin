using ImGuiExtensions;
using ImGuiNET;
using Plugin.Utilities;
using System.Reflection;

namespace Plugin.FeaturesSetup.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class BoolConfigAttribute : BaseConfigAttribute
{
    public override void Draw(Tweak tweak, object config, FieldInfo fieldInfo)
    {
        var value = (bool)fieldInfo.GetValue(config)!;
        var attr = fieldInfo.GetCustomAttribute<BaseConfigAttribute>();
        var cmdMethod = tweak.CachedType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(mi => mi.GetCustomAttribute<CommandHandlerAttribute>()?.ConfigFieldName == fieldInfo.Name);
        var cmdAttr = cmdMethod?.GetCustomAttribute<CommandHandlerAttribute>();

        var label = cmdAttr?.Commands.FirstOrDefault() ?? (!attr?.Label.IsNullOrEmpty() ?? false ? attr!.Label : fieldInfo.Name.SplitWords());
        if (ImGui.Checkbox($"{label}##Input", ref value))
        {
            fieldInfo.SetValue(config, value);
            OnChangeInternal(tweak, fieldInfo);
        }

        DrawConfigInfos(fieldInfo);

        var desc = !cmdAttr?.HelpMessage.IsNullOrEmpty() ?? false ? cmdAttr!.HelpMessage : !attr?.Description.IsNullOrEmpty() ?? false ? attr!.Description : null;
        if (desc != null)
        {
            ImGuiExt.PushCursorY(-3);
            using var descriptionIndent = ImGuiExt.ConfigIndent();
            ImGuiHelpers.SafeTextColoredWrapped(ColorEx.Grey, desc);
            ImGuiExt.PushCursorY(3);
        }
    }
}
