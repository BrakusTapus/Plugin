using ECommons.GameHelpers;
using Plugin.Schedulers;
using Plugin.Schedulers.Tasks;

namespace Plugin.Schedulers.Tasks.Utility;

internal static class TaskWaitUntilInHomeWorld
{
    internal static void Enqueue()
    {
        P.TaskManager.Enqueue(() => Player.Available && Player.IsInHomeWorld, "Waiting until player returns to home world", TaskSettings.TimeoutInfinite);
        P.TaskManager.Enqueue(DCChange.WaitUntilNotBusy, "Waiting until player is not busy (TaskWaitUntilInHomeWorld)", TaskSettings.Timeout1M);
    }
}
