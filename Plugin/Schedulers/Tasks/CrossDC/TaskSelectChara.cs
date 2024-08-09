using FFXIVClientStructs.FFXIV.Component.GUI;
using Plugin.Schedulers;
using Plugin.Schedulers.Tasks;

namespace Plugin.Schedulers.Tasks.CrossDC;

internal class TaskSelectChara
{
    internal static unsafe void Enqueue(string charaName, uint charaWorld)
    {
        P.TaskManager.Enqueue(() => TryGetAddonByName<AtkUnitBase>("_CharaSelectListMenu", out var addon) && IsAddonReady(addon), "Wait until chara list available", TaskSettings.TimeoutInfinite);
        P.TaskManager.Enqueue(() => DCChange.SelectCharacter(charaName, charaWorld), nameof(DCChange.SelectCharacter));
        P.TaskManager.Enqueue(DCChange.SelectYesLogin, TaskSettings.TimeoutInfinite);
    }
}
