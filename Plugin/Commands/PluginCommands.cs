using Dalamud.Game.Command;

namespace Plugin.Commands;

public static partial class PluginCommands
{
    private static Plugin Plugin;
    public const string Command = "/kirbo";
    public const string AltCommand = "/pinkblob";

    internal static void Enable(Plugin plugin)
    {
        Plugin = plugin;
        Services.CommandManager.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Menu.",
            ShowInHelp = true,
        });
        Services.CommandManager.AddHandler(AltCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "Also Opens Menu.",
            ShowInHelp = true,
        });
        Services.PluginLog.Debug($"Enabled commands: {Command} {AltCommand}");
    }

    internal static void Disable()
    {
        Services.CommandManager.RemoveHandler(Command);
        Services.CommandManager.RemoveHandler(AltCommand);
        Services.PluginLog.Debug($"Disabled commands: {Command} {AltCommand}");
    }

    private static void OnCommand(string command, string args)
    {
        if (string.IsNullOrEmpty(args))
        {
            Plugin.ToggleMainUI();
        }
        else
        {
            // Handle other cases or arguments if needed
            Services.PluginLog.Debug($"Command received with args: {args}");
        }
    }
}
