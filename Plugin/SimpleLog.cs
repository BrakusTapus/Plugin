﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace Plugin;

internal sealed class SimpleLog
{
    [PluginService] private static IPluginLog PluginLog { get; set; } = null;

    public static void Verbose(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Verbose($"{m}");
    }

    public static void Debug(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Debug($"{m}");
    }

    public static void Information(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Information($"{m}");
    }

    public static void Fatal(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Fatal($"{m}");
    }

    public static void Log(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Information($"{m}");
    }

    public static void Warning(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Warning($"{m}");
    }

    public static void Error(object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage(message)) PluginLog.Error($"{m}");
    }

    public static void Error(Exception ex, object message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage($"{message}\n{ex}")) PluginLog.Error($"{m}");
    }

    public static void Error(Exception ex, string message, [CallerFilePath] string callerPath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = -1)
    {
        foreach (var m in SplitMessage($"{message}\n{ex}")) PluginLog.Error($"{m}");
    }

    private static IEnumerable<string> SplitMessage(object message)
    {
        if (message is IList list)
        {
            return list.Cast<object>().Select((t, i) => $"{i}: {t}");
        }
        return $"{message}".Split('\n');
    }
}
