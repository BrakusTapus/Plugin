//using ECommons.GameHelpers;

//using Plugin.Schedulers;
//using Plugin.Utilities;

//namespace Plugin.Tasks.SameWorld;

//internal static class TaskTpToAethernetDestination
//{
//    internal static void Enqueue(WorldChangeAetheryte worldChangeAetheryte)
//    {
//        if(C.WaitForScreenReady) P.TaskManager.Enqueue(Utils.WaitForScreen);
//        P.TaskManager.Enqueue(() => WorldChange.ExecuteTPToAethernetDestination((uint)worldChangeAetheryte));
//        P.TaskManager.Enqueue(() => Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51], "WaitUntilBetweenAreas");
//        P.TaskManager.Enqueue(WorldChange.WaitUntilNotBusy, TaskSettings.Timeout2M);
//        P.TaskManager.Enqueue(() => ECommons.GameHelpers.Player.Interactable && Svc.ClientState.TerritoryType == worldChangeAetheryte.GetTerritory(), "WaitUntilPlayerInteractable", TaskSettings.Timeout2M);
//    }
//}
