﻿using Plugin.Schedulers;
using Plugin.Schedulers.Tasks;
using Plugin.Utilities;

namespace Plugin.Schedulers.Tasks.CrossDC;

internal class TaskReturnToHomeDC
{
    internal static void Enqueue(string charaName, uint charaWorld)
    {
        PluginLog.Debug($"Beginning returning home process.");
        P.TaskManager.Enqueue(() => DCChange.OpenContextMenuForChara(charaName, charaWorld), nameof(DCChange.OpenContextMenuForChara), TaskSettings.Timeout5M);
        P.TaskManager.Enqueue(DCChange.SelectReturnToHomeWorld);
        P.TaskManager.Enqueue(DCChange.ConfirmDcVisit, TaskSettings.Timeout2M);
        P.TaskManager.Enqueue(DCChange.ConfirmDcVisit2, TaskSettings.Timeout2M);
        P.TaskManager.Enqueue(DCChange.SelectOk, TaskSettings.TimeoutInfinite);
        //P.TaskManager.Enqueue(() => DCChange.SelectServiceAccount(Utils.GetServiceAccount(charaName, charaWorld)), $"SelectServiceAccount_{charaName}@{charaWorld}", TaskSettings.Timeout1M);
    }
}
