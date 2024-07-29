using System.Numerics;
using ImGuiNET;

namespace Plugin.Utility.UI;

public static class ImageLoader
{
    /// <summary>
    /// Load an image via a url (or full path name + image name)
    /// </summary>
    /// <param name="url"></param>
    public static void LoadImageFromStream(string url)
    {

        var windowPaddingWidth = ImGui.GetStyle().WindowPadding.X;
        var windowPaddingHeight = ImGui.GetStyle().WindowPadding.Y;
        var windowHeight = ImGui.GetWindowHeight();
        var windowWidth = ImGui.GetWindowWidth();
        var backgroundImageSize = new Vector2(windowWidth - windowPaddingWidth, windowHeight - windowPaddingHeight);
        /// var imagePath = Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName!, "Assets", "Images", $"Dark.png");

        if (ImageLoaderHandler.TryGetTextureWrap(url, out var backgroundImage))
        {
            ImGui.Image(backgroundImage.ImGuiHandle, backgroundImageSize);
        }
    }


}