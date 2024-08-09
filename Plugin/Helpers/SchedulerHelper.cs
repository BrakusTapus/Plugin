﻿using System.Collections.Generic;
using System;
using Dalamud.Plugin.Services;

namespace Plugin.Helpers
{
    internal static class SchedulerHelper
    {
        internal class Schedule
        {
            internal string Name { get; set; } = string.Empty;

            internal List<Action> Action { get; set; } = [() => { }];

            internal int TimeMS { get; set; } = 0;

            internal Func<bool>? Condition { get; set; } = null;

            internal bool RunOnce { get; set; } = true;
        }

        internal static HashSet<Schedule> schedules = [];

        internal static bool ScheduleAction(string name, Action action, int timeMS, bool runOnce = true) => schedules.Add(new Schedule() { Name = name, Action = [action], TimeMS = Environment.TickCount + timeMS, RunOnce = runOnce });

        internal static bool ScheduleAction(string name, List<Action> action, int timeMS, bool runOnce = true) => schedules.Add(new Schedule() { Name = name, Action = action, TimeMS = Environment.TickCount + timeMS, RunOnce = runOnce });

        internal static bool ScheduleAction(string name, Action action, Func<bool> condition, bool runOnce = true) => schedules.Add(new Schedule() { Name = name, Action = [action], Condition = condition, RunOnce = runOnce });

        internal static bool ScheduleAction(string name, List<Action> action, Func<bool> condition, bool runOnce = true) => schedules.Add(new Schedule() { Name = name, Action = action, Condition = condition, RunOnce = runOnce });

        internal static int DescheduleAction(string name) => schedules.RemoveWhere(s => s.Name == name);

        internal static void ScheduleInvoker(IFramework _)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.TimeMS != 0 ? Environment.TickCount >= schedule.TimeMS : schedule.Condition?.Invoke() ?? false)
                {
                    schedule.Action.ForEach(a => a.Invoke());
                    if (schedule.RunOnce)
                        schedules.Remove(schedule);
                }
            }
        }
    }
}
