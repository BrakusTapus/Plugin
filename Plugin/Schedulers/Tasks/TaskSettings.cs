﻿using ECommons.Automation.NeoTaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Schedulers.Tasks;
public static class TaskSettings
{
    public static readonly TaskManagerConfiguration Timeout1M = new(timeLimitMS: 60000);
    public static readonly TaskManagerConfiguration Timeout2M = new(timeLimitMS: 60000 * 2);
    public static readonly TaskManagerConfiguration Timeout5M = new(timeLimitMS: 60000 * 5);
    public static readonly TaskManagerConfiguration Timeout15M = new(timeLimitMS: 60000 * 15);
    public static readonly TaskManagerConfiguration Timeout60M = new(timeLimitMS: 60000 * 60);
    public static readonly TaskManagerConfiguration TimeoutInfinite = new(timeLimitMS: int.MaxValue);
}
