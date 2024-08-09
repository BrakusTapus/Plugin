//using ECommons.Automation.NeoTaskManager.Tasks;

//using Plugin.Schedulers;
//using Plugin.Tasks.SameWorld;
//using Plugin.Utilities;

//namespace Plugin.Tasks.CrossWorld;

//internal static class TaskTPAndChangeWorld
//{
//    internal static void Enqueue(string world, WorldChangeAetheryte gateway, bool insert)
//    {
//        P.TaskManager.BeginStack();
//        if(C.WaitForScreenReady) P.TaskManager.Enqueue(Utils.WaitForScreen);
//        if(P.ActiveAetheryte != null && P.ActiveAetheryte.Value.IsWorldChangeAetheryte())
//        {
//            TaskChangeWorld.Enqueue(world);
//        }
//        else
//        {
//            if(Utils.GetReachableWorldChangeAetheryte(!C.WalkToAetheryte) == null)
//            {
//                TaskTpToAethernetDestination.Enqueue(gateway);
//            }
//            P.TaskManager.EnqueueTask(new(() =>
//            {
//                if((P.ActiveAetheryte == null || !P.ActiveAetheryte.Value.IsWorldChangeAetheryte()) && Utils.GetReachableWorldChangeAetheryte() != null)
//                {
//                    P.TaskManager.InsertMulti(
//                        new FrameDelayTask(10),
//                        new(WorldChange.TargetReachableAetheryte),
//                        new(WorldChange.LockOn),
//                        new(WorldChange.EnableAutomove),
//                        new(WorldChange.WaitUntilWorldChangeAetheryteExists),
//                        new(WorldChange.DisableAutomove)
//                        );
//                }
//            }, "ConditionalLockonTask"));
//            P.TaskManager.Enqueue(WorldChange.WaitUntilWorldChangeAetheryteExists);
//            P.TaskManager.EnqueueDelay(10, true);
//            TaskChangeWorld.Enqueue(world);
//        }
//        if(insert)
//        {
//            P.TaskManager.InsertStack();
//        }
//        else
//        {
//            P.TaskManager.EnqueueStack();
//        }
//    }
//}
