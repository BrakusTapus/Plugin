﻿//using ECommons.Automation;
//using ECommons.GameHelpers;
//using ECommons.Throttlers;
//using FFXIVClientStructs.FFXIV.Client.Game;
//using Plugin.Data;
//using Plugin.Systems;
//using Lumina.Excel.GeneratedSheets;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Action = Lumina.Excel.GeneratedSheets.Action;

//namespace Plugin.Tasks.Utility;
//public static unsafe class TaskMoveToHouse
//{
//    public static void Enqueue(PlotInfo info, bool includeFirst)
//    {
//        P.TaskManager.EnqueueMulti(
//            new(UseSprint),
//            new(() => LoadPath(info, includeFirst), "LoadPath"),
//            new(WaitUntilPathCompleted, TaskSettings.Timeout5M)
//            );
//    }

//    public static bool? UseSprint()
//    {
//        if(!C.UseSprintPeloton) return true;
//        if(Player.IsAnimationLocked) return false;
//        if(Player.Object.StatusList.Any(x => x.StatusId.EqualsAny<uint>(50, 1199))) return true;
//        uint[] abilities = [3, 7557];
//        foreach(var ability in abilities)
//        {
//            if(ActionManager.Instance()->GetActionStatus(ActionType.Action, ability) == 0)
//            {
//                if(EzThrottler.Throttle("ExecSpritAction"))
//                {
//                    Chat.Instance.ExecuteCommand($"/action \"{Svc.Data.GetExcelSheet<Action>().GetRow(ability).Name.ExtractText()}\"");
//                    return true;
//                }
//            }
//        }
//        return true;
//    }

//    public static bool? LoadPath(PlotInfo info, bool includeFirst)
//    {
//        if(info.Path.Count == 0) return null;
//        P.FollowPath.Stop();
//        P.FollowPath.Move([.. info.Path], true);
//        if(!includeFirst) P.FollowPath.RemoveFirst();
//        return true;
//    }

//    public static bool WaitUntilPathCompleted() => P.FollowPath.Waypoints.Count == 0;
//}
