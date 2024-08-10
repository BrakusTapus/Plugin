using ECommons.DalamudServices;
using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Character = Dalamud.Game.ClientState.Objects.Types.ICharacter;

namespace EasyCombat.Commands;

public static unsafe class ResetEnmity
{
    public const string Command = "/dre";
    public static string[] Alias => new string[] { "/dre" };
    public static string Description => "Resets the enmity of all enemies targeting you. Useful for target dummies. Accepts arguments for t(arget) or a(ll). Defaults to all.";
    public static string Name => "Reset Enmity";
    public static List<string> Parameters => new() { "t", "a" };

    // Refactor Disable method to include resource cleanup
    public static void Disable()
    {
        if (!isDisposed)
        {
            Svc.Commands.RemoveHandler(Command);
            Svc.Log.Debug("Disabled Command /dre.");

            isDisposed = true;
        }
    }

    internal static void Enable()
    {
        // Add a handler for the "Reset Enmity" command
        Svc.Commands.AddHandler(ResetEnmity.Command, new CommandInfo(ResetEnmity.OnCommandResetEnmity)
        {
            HelpMessage = ResetEnmity.Description,
            ShowInHelp = true,
        });
        Svc.Log.Debug("ResetEnmity loaded.");
    }

    internal static void OnCommandResetEnmity(string command, string arguments)
    {
        if (command != ResetEnmity.Command)
        {
            // This command handler is not meant for this command
            return;
        }

        // Parse and process the arguments here
        List<string> args = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (var arg in args)
        {
            switch (arg.ToLower()) // Convert to lowercase for case-insensitive comparison
            {
                case "t":
                    Svc.Log.Debug("Resetting target's enmity.");
                    ResetTarget();
                    break;
                case "a":
                    Svc.Log.Debug("Resetting all enmities.");
                    ResetAll();
                    break;
                default:
                    ResetAll();
                    break;
            }
        }
    }

    private static ExecuteCommandDelegate ExecuteCommand;
    private static bool isDisposed = false;
    private delegate long ExecuteCommandDelegate(uint id, int a1, int a2, int a3, int a4);

    private static void Reset(int GameObjectId)
    {
        // Reset enmity at target sig. This doesn't change often, but it does sometimes.
        nint scanText = Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 8D 43 0A");
        ExecuteCommand = Marshal.GetDelegateForFunctionPointer<ExecuteCommandDelegate>(scanText);

        Svc.Log.Debug($"{nameof(ExecuteCommand)} +{scanText - Process.GetCurrentProcess().MainModule!.BaseAddress:X}");
        Svc.Log.Information($"Resetting enmity {GameObjectId}");

        long success = ExecuteCommand(0x13f, GameObjectId, 0, 0, 0);
        Svc.Log.Debug($"Reset enmity of {GameObjectId} returned: {success}");
    }

    private static void ResetAll()
    {
        var addonByName = Svc.GameGui.GetAddonByName("_EnemyList", 1);
        if (addonByName != IntPtr.Zero)
        {
            var addon = (AddonEnemyList*)addonByName;

            // the 21 works now, but if this doesn't in the future, check this. It used to be 19.
            var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUIModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[21];

            for (var i = 0; i < addon->EnemyCount; i++)
            {
                var enemyObjectId = numArray->IntArray[8 + i * 6];
                var enemyChara = CharacterManager.Instance()->LookupBattleCharaByEntityId((uint)enemyObjectId);
                if (enemyChara is null) continue;
                if (enemyChara->Character.NameId == 541) Reset(enemyObjectId);
            }
        }
    }

    private static void ResetTarget()
    {
        var target = Svc.Targets.Target;
        if (target is Character { NameId: 541 }) Reset((int)target.GameObjectId);
    }
}
