using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ECommons.Throttlers;

#nullable disable
namespace Plugin.Utilities.Helpers;

public static unsafe partial class GenericHelpersEx
{
    //public static void Log(this Exception e)
    //{
    //    Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(Action a, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            if (!suppressErrors)
            {
                MyServices.Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(Action a, Action<string, object[]> logAction)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            logAction($"{e.Message}\n{e.StackTrace ?? ""}", Array.Empty<object>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(Action a, Action<string> fail, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            try
            {
                fail(e.Message);
            }
            catch (Exception ex)
            {
                MyServices.Services.PluginLog.Error("Error while trying to process error handler:");
                MyServices.Services.PluginLog.Error($"{ex.Message}\n{ex.StackTrace ?? ""}");
                suppressErrors = false;
            }
            if (!suppressErrors)
            {
                MyServices.Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }
    }

    //public static bool TryGetFirst<K, V>(this IDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate, out KeyValuePair<K, V> keyValuePair)
    //{
    //    try
    //    {
    //        keyValuePair = dictionary.First(predicate);
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        keyValuePair = default;
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// Attempts to get first element of <see cref="IEnumerable"/>.
    ///// </summary>
    ///// <typeparam name="TSource"></typeparam>
    ///// <param name="source"></param>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, out TSource value)
    //{
    //    if (source == null)
    //    {
    //        value = default;
    //        return false;
    //    }
    //    var list = source as IList<TSource>;
    //    if (list != null)
    //    {
    //        if (list.Count > 0)
    //        {
    //            value = list[0];
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        using (var e = source.GetEnumerator())
    //        {
    //            if (e.MoveNext())
    //            {
    //                value = e.Current;
    //                return true;
    //            }
    //        }
    //    }
    //    value = default;
    //    return false;
    //}

    ///// <summary>
    ///// Attempts to get first element of IEnumerable
    ///// </summary>
    ///// <typeparam name="TSource"></typeparam>
    ///// <param name="source"></param>
    ///// <param name="predicate">Function to test elements.</param>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource value)
    //{
    //    if (source == null)
    //    {
    //        value = default;
    //        return false;
    //    }
    //    if (predicate == null)
    //    {
    //        value = default;
    //        return false;
    //    }
    //    foreach (TSource element in source)
    //    {
    //        if (predicate(element))
    //        {
    //            value = element;
    //            return true;
    //        }
    //    }
    //    value = default;
    //    return false;
    //}

    /// <summary>
    /// Attempts to get first instance of addon by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Addon"></param>
    /// <param name="AddonPtr"></param>
    /// <returns></returns>
    public static bool TryGetAddonByName<T>(string Addon, out T* AddonPtr) where T : unmanaged
    {
        var a = MyServices.Services.GameGui.GetAddonByName(Addon, 1);
        if (a == IntPtr.Zero)
        {
            AddonPtr = null;
            return false;
        }
        else
        {
            AddonPtr = (T*)a;
            return true;
        }
    }

    /// <summary>
    /// Attempts to find out whether SelectString entry is enabled based on text color. 
    /// </summary>
    /// <param name="textNodePtr"></param>
    /// <returns></returns>
    public static bool IsSelectItemEnabled(FFXIVClientStructs.FFXIV.Component.GUI.AtkTextNode* textNodePtr)
    {
        var col = textNodePtr->TextColor;
        //EEE1C5FF
        return (col.A == 0xFF && col.R == 0xEE && col.G == 0xE1 && col.B == 0xC5)
            //7D523BFF
            || (col.A == 0xFF && col.R == 0x7D && col.G == 0x52 && col.B == 0x3B)
            || (col.A == 0xFF && col.R == 0xFF && col.G == 0xFF && col.B == 0xFF)
            // EEE1C5FF
            || (col.A == 0xFF && col.R == 0xEE && col.G == 0xE1 && col.B == 0xC5);
    }

    //public static void LogWarning(this Exception e)
    //{
    //    Services.PluginLog.Warning($"{e.Message}\n{e.StackTrace ?? ""}");
    //}

    //public static void Log(this Exception e)
    //{
    //    Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    //}
    //public static void LogVerbose(this Exception e)
    //{
    //    Services.PluginLog.LogVerbose($"{e.Message}\n{e.StackTrace ?? ""}");
    //}
    //public static void LogInternal(this Exception e)
    //{
    //    InternalLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    //}
    //public static void LogInfo(this Exception e)
    //{
    //    PluginLog.Information($"{e.Message}\n{e.StackTrace ?? ""}");
    //}

    //public static void Log(this Exception e, string ErrorMessage)
    //{
    //    PluginLog.Error($"{ErrorMessage}\n{e.Message}\n{e.StackTrace ?? ""}");
    //}

    //public static void LogDuo(this Exception e)
    //{
    //    DuoLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    //}

    internal static void RethrottleGeneric(int num)
    {
        EzThrottler.Throttle("AutoRetainerGenericThrottle", num, true);
    }
}
