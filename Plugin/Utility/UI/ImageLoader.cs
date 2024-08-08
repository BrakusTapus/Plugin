using System.IO;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiExtensions;
using ImGuiNET;

namespace Plugin.Utility.UI;

//TODO: Add this to the ImGuiEx namespace (ImGui folder) 
public static class ImageLoader
{
    /// <summary>
    /// Load an image via a url (or full path name + image name)
    /// </summary>
    /// <param name="url"></param>
    public static void LoadImageFromStream(string url)
    {
        float paddingWidth = ImGui.GetStyle().WindowPadding.X;
        float paddingHeight = ImGui.GetStyle().WindowPadding.Y;
        float windowY = ImGui.GetWindowHeight();
        float windowX = ImGui.GetWindowWidth();
        Vector2 imageSize = new(windowX - paddingWidth, windowY - paddingHeight);
        /// var imagePath = Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName!, "Assets", "Images", $"Dark.png");

        if (ImageLoaderHandler.TryGetTextureWrap(url, out Dalamud.Interface.Textures.TextureWraps.IDalamudTextureWrap? image))
        {
            ImGui.Image(image.ImGuiHandle, imageSize);
        }
    }


    #region From File
    /// <summary>
    /// Draw an image using various parameters with defaults.
    /// </summary>
    /// <param name="catagoryUiPaths">The base path for the image category.</param>
    /// <param name="fileName">The file name of the image.</param>
    /// <param name="size">The size of the image.</param>
    /// <param name="tintColor">The tint color of the image. Default is Vector4.One (white).</param>
    /// <param name="borderColor">The border color of the image. Default is Vector4.Zero (transparent).</param>
    public static void DrawImage(string? catagoryUiPaths, string? fileName, float size = 0, Vector4? tintColor = null, Vector4? borderColor = null)
    {
        DrawImage(catagoryUiPaths, fileName, new Vector2(size, size), tintColor, borderColor);
    }

    /// <summary>
    /// Draw an image using various parameters with defaults.
    /// </summary>
    /// <param name="catagoryUiPaths">The base path for the image category.</param>
    /// <param name="fileName">The file name of the image.</param>
    /// <param name="size">The size of the image.</param>
    /// <param name="tintColor">The tint color of the image. Default is Vector4.One (white).</param>
    /// <param name="borderColor">The border color of the image. Default is Vector4.Zero (transparent).</param>
    public static void DrawImage(string? catagoryUiPaths, string? fileName, Vector2 size, Vector4? tintColor = null, Vector4? borderColor = null)
    {
        if (catagoryUiPaths != null && fileName != null)
        {
            MyServices.Services.TextureService.DrawImage(
                Path.Combine(catagoryUiPaths, fileName),
                size,
                tintColor ?? Vector4.One,
                borderColor ?? Vector4.Zero);
        }
        else
        {
            MyServices.Services.PluginLog.Error($"Paths are null!");
        }
    }
    #endregion


    #region From Game
    /// <summary>
    /// Draw an icon using various parameters with defaults.
    /// </summary>
    /// <param name="iconID">The ID of the icon.</param>
    /// <param name="isHQ">Flag indicating if the icon is of high quality.</param>
    /// <param name="size">The size of the icon.</param>
    /// <param name="tintColor">The tint color of the icon. Default is Vector4.One (white).</param>
    /// <param name="borderColor">The border color of the icon. Default is Vector4.Zero (transparent).</param>
    public static void DrawIcon(int iconID, bool isHQ = false, float size = 0, Vector4? tintColor = null, Vector4? borderColor = null)
    {
        DrawIcon(iconID, isHQ, new Vector2(size, size), tintColor, borderColor);
    }

    /// <summary>
    /// Draw an icon using various parameters with defaults.
    /// </summary>
    /// <param name="iconID">The ID of the icon.</param>
    /// <param name="isHQ">Flag indicating if the icon is of high quality.</param>
    /// <param name="size">The size of the icon.</param>
    /// <param name="tintColor">The tint color of the icon. Default is Vector4.One (white).</param>
    /// <param name="borderColor">The border color of the icon. Default is Vector4.Zero (transparent).</param>
    public static void DrawIcon(int iconID, bool isHQ, Vector2 size, Vector4? tintColor = null, Vector4? borderColor = null)
    {
        if (iconID != 0)
        {
            MyServices.Services.TextureService.DrawIcon(
                iconID,
                isHQ,
                size,
                tintColor ?? Vector4.One,
                borderColor ?? Vector4.Zero);
        }
        else
        {
            MyServices.Services.PluginLog.Error("iconID is 0!");
        }
    }
    #endregion


#if DEBUG
    /// <summary>
    /// This is more here to test/play with tbh
    /// </summary>
    internal static void DrawImagesForTesting(string? catagoryUiPaths, string? filename)
    {
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png");
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", 64);
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", 64, ColorEx.DarkType);
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", 64, ColorEx.DarkType, ColorEx.Silver);
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", new Vector2(64, 64));
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", new Vector2(64, 64), ColorEx.DarkType);
        ImGui.SameLine();
        ImageLoader.DrawImage(UiPaths.OtherPath, "compass.png", new Vector2(64, 64), ColorEx.DarkType, ColorEx.Silver);
    }

    /// <summary>
    /// This is more here to test/play with tbh
    /// </summary>
    internal static void DrawIconsForTesting()
    {
        MyServices.Services.TextureService.DrawIcon(60073, true); // Uses default DrawInfo
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60074, true, new Vector2(64, 64)); // Specifies only the size
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60001, true, new Vector2(64, 64), new Vector4(1, 1, 1, 1)); // Specifies size and tint color
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60002, true, new Vector2(24, 64), new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies size, tint color, and border color
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60003, true, 64); // Specifies size as float
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60004, true, 64, 64); // Specifies width and height
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60011, true, 64, 64, new Vector4(1, 1, 1, 1)); // Specifies width, height, and tint color
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60012, true, 64, 64, new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies width, height, tint color, and border color
        ImGui.SameLine();
        MyServices.Services.TextureService.DrawIcon(60025, true, 64, 64, new Vector4(1, 1, 1, 1), new Vector4(0, 0, 0, 1)); // Specifies width, height, tint color, and border color    
    }
#endif

}