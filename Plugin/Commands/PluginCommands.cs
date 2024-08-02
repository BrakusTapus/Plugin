using Dalamud.Game.Command;
using Plugin.MyServices;
using Plugin.Tasks.SameWorld;
using ECommons.MathHelpers;

namespace Plugin.Commands;

public static partial class PluginCommands
{
    private static Plugin Plugin;
    public const string Command = "/kirbo";
    public const string AltCommand = "/ko";
    public const string InstanceCommand = "/kirboinstance";

    internal static void Enable(Plugin plugin)
    {
        Plugin = plugin;
        MyServices.Services.CommandManager.AddHandler(Command, new CommandInfo(OnCommand) {
            HelpMessage = "Opens Menu.",
            ShowInHelp = true,
        });
        MyServices.Services.CommandManager.AddHandler(AltCommand, new CommandInfo(OnCommand) {
            HelpMessage = "Also Opens Menu.",
            ShowInHelp = true,
        });
        MyServices.Services.CommandManager.AddHandler(InstanceCommand, new CommandInfo(ProcessCommand) {
            HelpMessage = "Change instance",
            ShowInHelp = true,
        });
        MyServices.Services.PluginLog.Debug($"Enabled commands: {Command} {AltCommand} {InstanceCommand}");
    }

    internal static void Disable()
    {
        MyServices.Services.CommandManager.RemoveHandler(Command);
        MyServices.Services.CommandManager.RemoveHandler(AltCommand);
        MyServices.Services.PluginLog.Debug($"Disabled commands: {Command} {AltCommand}  {InstanceCommand}");
    }

    private static void OnCommand(string command, string args)
    {
        if (string.IsNullOrEmpty(args))
        {
            Plugin.ToggleMainWindow();
            MyServices.Services.PluginLog.Debug($"Command: {command} executed!");
        }
        else if (args.Equals("c", StringComparison.OrdinalIgnoreCase) || args.Equals("config", StringComparison.OrdinalIgnoreCase))
        {
            Plugin.ToggleConfigWindow();
            MyServices.Services.PluginLog.Debug($"Config command: {command} executed with args: {args}");
        }
        else
        {
            // Handle other cases or arguments if needed
            MyServices.Services.PluginLog.Debug($"Command received with unrecognized args: {args}");
        }
    }

    internal static void ProcessCommand(string command, string arguments)
    {
        if (arguments == "stop" || arguments == "clear")
        {
            Notify.Info($"Discarding {P.TaskManager.NumQueuedTasks + (P.TaskManager.IsBusy ? 1 : 0)} tasks");
            P.TaskManager.Abort();
            //followPath?.Stop();
        }

        else if (arguments.Length == 1 && int.TryParse(arguments, out var val) && val.InRange(1, 9))
        {
            if (S.InstanceHandler.GetInstance() == val)
            {
                DuoLog.Warning($"Already in instance {val}");
            }
            else if (S.InstanceHandler.CanChangeInstance())
            {
                TaskChangeInstance.Enqueue(val);
                DuoLog.Information($"Changing to instance: {val}");
            }
            else
            {
                DuoLog.Error($"Can't change instance now");
            }
        }
    }
}
