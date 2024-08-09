//using ECommons.GameHelpers;

//using Plugin.Schedulers;

//namespace Plugin.Tasks.SameWorld;

//public static class TaskReturnToGateway
//{
//    public static void Enqueue(WorldChangeAetheryte gateway)
//    {
//        if(C.WaitForScreenReady) P.TaskManager.Enqueue(Utils.WaitForScreen);
//        P.TaskManager.Enqueue(WaitUntilInteractable);
//        P.TaskManager.Enqueue(() =>
//        {
//            if(Svc.ClientState.TerritoryType != gateway.GetTerritory())
//            {
//                P.TaskManager.InsertMulti(
//                    new(() => WorldChange.ExecuteTPToAethernetDestination((uint)gateway), $"ExecuteTPToAethernetDestination({gateway})"),
//                    new(() => Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51], "WaitUntilBetweenAreas"),
//                    new(WorldChange.WaitUntilNotBusy, TaskSettings.Timeout2M),
//                    new(() => Player.Interactable && Svc.ClientState.TerritoryType == gateway.GetTerritory(), "WaitUntilPlayerInteractable", TaskSettings.Timeout2M)
//                    );
//            }
//        }, "TaskReturnToGatewayMaster");
//    }

//    public static bool? WaitUntilInteractable() => Player.Interactable;
//}
