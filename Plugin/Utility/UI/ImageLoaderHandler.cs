using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using Dalamud.Interface.Textures.TextureWraps;
using Plugin.Utilities.Helpers;

namespace Plugin.Utilities.UI;

//TODO: Add this to the ImGuiEx namespace (ImGui folder) 
public class ImageLoaderHandler
{
    internal static ConcurrentDictionary<string, ImageLoadingResult> CachedTextures = new();
    internal static ConcurrentDictionary<(uint ID, bool HQ), ImageLoadingResult> CachedIcons = new();

    private static readonly List<Func<byte[], byte[]>> _conversionsToBitmap = new() { b => b, };

    static volatile bool ThreadRunning = false;
    internal static HttpClient httpClient = null;

    public static bool TryGetTextureWrap(string url, out IDalamudTextureWrap textureWrap)
    {
        ImageLoadingResult result;
        if (!CachedTextures.TryGetValue(url, out result))
        {
            result = new();
            CachedTextures[url] = result;
            BeginThreadIfNotRunning();
        }
        textureWrap = result.Texture;
        return result.Texture != null;
    }

    internal static void BeginThreadIfNotRunning()
    {
        httpClient ??= new() {
            Timeout = TimeSpan.FromSeconds(10),
        };
        MyServices.Services.PluginLog.Verbose("Starting ThreadLoadImageHandler");
        ThreadRunning = true;
        new Thread(() =>
        {
            int idleTicks = 0;
            GenericHelpersEx.Safe((Action)delegate
            {
                while (idleTicks < 100)
                {
                    GenericHelpersEx.Safe((Action)delegate
                    {
                        {
                            if (CachedTextures.TryGetFirst(x => x.Value.IsCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.IsCompleted = true;
                                MyServices.Services.PluginLog.Verbose("Loading image " + keyValuePair.Key);
                                if (keyValuePair.Key.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || keyValuePair.Key.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                                {
                                    var result = httpClient.GetAsync(keyValuePair.Key).Result;
                                    result.EnsureSuccessStatusCode();
                                    var content = result.Content.ReadAsByteArrayAsync().Result;

                                    IDalamudTextureWrap texture = null;
                                    foreach (var conversion in _conversionsToBitmap)
                                    {
                                        if (conversion == null) continue;

                                        try
                                        {
                                            texture = MyServices.Services.TextureProvider.CreateFromImageAsync(conversion(content)).Result;
                                            if (texture != null) break;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Log();
                                        }
                                    }
                                    keyValuePair.Value.TextureWrap = texture;
                                }
                                else
                                {
                                    if (File.Exists(keyValuePair.Key))
                                    {
                                        keyValuePair.Value.ImmediateTexture = MyServices.Services.TextureProvider.GetFromFile(keyValuePair.Key);
                                    }
                                    else
                                    {
                                        keyValuePair.Value.ImmediateTexture = MyServices.Services.TextureProvider.GetFromGame(keyValuePair.Key);
                                    }
                                }
                            }
                        }
                        {
                            if (CachedIcons.TryGetFirst(x => x.Value.IsCompleted == false, out var keyValuePair))
                            {
                                idleTicks = 0;
                                keyValuePair.Value.IsCompleted = true;
                                MyServices.Services.PluginLog.Verbose($"Loading icon {keyValuePair.Key.ID}, hq={keyValuePair.Key.HQ}");
                                keyValuePair.Value.ImmediateTexture = MyServices.Services.TextureProvider.GetFromGameIcon(new(keyValuePair.Key.ID, hiRes: keyValuePair.Key.HQ));
                            }
                        }
                    });
                    idleTicks++;
                    if (!CachedTextures.Any(x => x.Value.IsCompleted) && !CachedIcons.Any(x => x.Value.IsCompleted)) Thread.Sleep(100);
                }
            });
            MyServices.Services.PluginLog.Verbose($"Stopping ThreadLoadImageHandler, ticks={idleTicks}");
            ThreadRunning = false;
        }).Start();
    }
}
