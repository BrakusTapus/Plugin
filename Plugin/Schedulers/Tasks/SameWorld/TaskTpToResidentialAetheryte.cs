//using ECommons.GameHelpers;

//using Plugin.Schedulers;
//using Plugin.Utilities;
//using Plugin.Utilities.Enums;
//using Player = ECommons.GameHelpers.Player;

//namespace Plugin.Tasks.SameWorld;
//public static class TaskTpToResidentialAetheryte
//{
//    public static void Insert(ResidentialAetheryteKind target)
//    {
//        P.TaskManager.Insert(() => Player.Interactable && Svc.ClientState.TerritoryType == target.GetTerritory(), "WaitUntilPlayerInteractable", TaskSettings.Timeout2M);
//        P.TaskManager.Insert(WorldChange.WaitUntilNotBusy, TaskSettings.Timeout2M);
//        P.TaskManager.Insert(() => Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51], "WaitUntilBetweenAreas");
//        P.TaskManager.Insert(() => WorldChange.ExecuteTPToAethernetDestination((uint)target), $"ExecuteTPToAethernetDestination {target}");
//        if(C.WaitForScreenReady) P.TaskManager.Insert(Utils.WaitForScreen);
//    }
//}
