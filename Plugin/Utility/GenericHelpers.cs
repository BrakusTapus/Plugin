using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace Plugin.Utility;

public static unsafe partial class GenericHelpers
{
    public static void Log(this Exception e)
    {
        Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            if (!suppressErrors)
            {
                Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, Action<string, object[]> logAction)
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
    public static void Safe(System.Action a, Action<string> fail, bool suppressErrors = false)
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
                Services.PluginLog.Error("Error while trying to process error handler:");
                Services.PluginLog.Error($"{ex.Message}\n{ex.StackTrace ?? ""}");
                suppressErrors = false;
            }
            if (!suppressErrors)
            {
                Services.PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }
    }

    public static bool TryGetFirst<K, V>(this IDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate, out KeyValuePair<K, V> keyValuePair)
    {
        try
        {
            keyValuePair = dictionary.First(predicate);
            return true;
        }
        catch (Exception)
        {
            keyValuePair = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to get first element of <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, out TSource value)
    {
        if (source == null)
        {
            value = default;
            return false;
        }
        var list = source as IList<TSource>;
        if (list != null)
        {
            if (list.Count > 0)
            {
                value = list[0];
                return true;
            }
        }
        else
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    value = e.Current;
                    return true;
                }
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get first element of IEnumerable
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate">Function to test elements.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource value)
    {
        if (source == null)
        {
            value = default;
            return false;
        }
        if (predicate == null)
        {
            value = default;
            return false;
        }
        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                value = element;
                return true;
            }
        }
        value = default;
        return false;
    }
}
