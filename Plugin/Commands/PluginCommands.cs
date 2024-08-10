using Dalamud.Game.Command;
using Plugin.Tasks.SameWorld;
using ECommons.MathHelpers;
using Plugin.AutoMarkt;
using Plugin.Internal;

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
            HelpMessage = "Alias for " + Command + ".",
            ShowInHelp = true,
        });
        MyServices.Services.CommandManager.AddHandler(InstanceCommand, new CommandInfo(ProcessCommand) {
            HelpMessage = "<1 - 4>\n    <stop> or <clear> clears enqued tasks!",
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
            MyServices.Services.PluginLog.Debug($"Command: {command} executed with args: {args}");
            Notify.Info($"Command: {command} executed with args: {args}");
        }
        else if (args.Equals("t", StringComparison.OrdinalIgnoreCase) || args.Equals("test", StringComparison.OrdinalIgnoreCase))
        {
            Plugin.ToggleTestWindow();
            MyServices.Services.PluginLog.Debug($"Command: {command} executed with args: {args}");
            Notify.Info($"Command: {command} executed with args: {args}");
        }
        else if (int.TryParse(args, out int index) && index >= 0)
        {
            bool success = AutoMarktTasks.SelectRetainerByIndex((uint)index);
            if (success)
            {
                MyServices.Services.PluginLog.Debug($"Retainer at index {index} selected successfully.");
                Notify.Info($"Retainer at index {index} selected successfully.");
            }
            else
            {
                MyServices.Services.PluginLog.Debug($"Failed to select retainer at index {index}.");
                Notify.Error($"Failed to select retainer at index {index}.");
            }
        }
        else
        {
            // Handle other cases or arguments if needed
            MyServices.Services.PluginLog.Debug($"Command received with unrecognized args: {args}");
            Notify.Info($"Command received with unrecognized args: {args}");
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

        else if (arguments.Length == 1 && int.TryParse(arguments, out int val) && val.InRange(1, 9))
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
