using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using ECommons.ImGuiMethods;
using ImGuiExtensions;
using ImGuiNET;
using Plugin.Utility.UI;

namespace Plugin.Utility.OtherServices;

public class TextureService
{
    private readonly ITextureProvider textureProvider;

    private readonly IDataManager dataManager;
    public IDataManager DataManager
    {
        get
        {
            return dataManager;
        }
    }

    private readonly string uiBasePath;
    public string UiBasePath
    {
        get
        {
            return uiBasePath;
        }
    }

    /// <summary>
    /// The class depends on ITextureProvider, IDataManager, and a base path string (uiBasePath).
    /// These dependencies are injected through the constructor, ensuring that the necessary services are available when the class is instantiated. 
    /// It also performs null checks to ensure that none of the dependencies are null.
    /// </summary>
    /// <param name="textureProvider"></param>
    /// <param name="dataManager"></param>
    /// <param name="uiBasePath"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TextureService(ITextureProvider textureProvider, IDataManager dataManager, string uiBasePath)
    {
        this.textureProvider = textureProvider ?? throw new ArgumentNullException(nameof(textureProvider));
        this.dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
        this.uiBasePath = uiBasePath ?? throw new ArgumentNullException(nameof(uiBasePath));
    }

    // This method handles the actual drawing logic. If the texture is null, it draws a dummy.
    // It calculates the size, UV coordinates, and then uses ImGui to draw the image with the specified tint and border colors.
    public static void Draw(IDalamudTextureWrap? textureWrap, DrawInfo drawInfo)
    {
        if (textureWrap == null)
        {
            ImGui.Dummy(drawInfo.DrawSize ?? Vector2.Zero);
            return;
        }

        Vector2 size = drawInfo.DrawSize ?? textureWrap.Size;
        size *= drawInfo.Scale; // Apply scaling

        if (!ImGuiExt.IsInViewport(size))
        {
            ImGui.Dummy(size);
            return;
        }

        Vector2 uv0 = drawInfo.Uv0 ?? Vector2.Zero;
        Vector2 uv1 = drawInfo.Uv1 ?? Vector2.One;

        if (drawInfo.TransformUv)
        {
            uv0 /= textureWrap.Size;
            uv1 /= textureWrap.Size;
        }

        ImGui.Image(
            textureWrap.ImGuiHandle,
            size,
            uv0,
            uv1,
            drawInfo.TintColor ?? Vector4.One,
            drawInfo.BorderColor ?? Vector4.Zero);
    }

    #region From File
    /// <summary>
    /// Retrieves a texture using a file path and draws it using the provided DrawInfo.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="drawInfo"></param>
    public void DrawImage(string path, DrawInfo drawInfo)
    {
        IDalamudTextureWrap? textureWrap = textureProvider.GetFromFile(path).GetWrapOrEmpty();
        Draw(textureWrap, drawInfo);
    }

    public void DrawImage(string path)
        => DrawImage(path, new DrawInfo());

    public void DrawImage(string path, Vector2 drawSize)
        => DrawImage(path, new DrawInfo { DrawSize = drawSize });

    public void DrawImage(string path, Vector2 drawSize, DrawInfo drawInfo)
        => DrawImage(path, new DrawInfo { DrawSize = drawSize });

    public void DrawImage(string path, Vector2 drawSize, Vector4 tintColor)
        => DrawImage(path, new DrawInfo { DrawSize = drawSize, TintColor = tintColor });

    public void DrawImage(string path, Vector2 drawSize, Vector4 tintColor, Vector4 borderColor)
        => DrawImage(path, new DrawInfo { DrawSize = drawSize, TintColor = tintColor, BorderColor = borderColor });

    public void DrawImage(string path, float size)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(size) });

    public void DrawImage(string path, float size, Vector4 tintColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(size), TintColor = tintColor });

    public void DrawImage(string path, float size, Vector4 tintColor, Vector4 borderColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(size), TintColor = tintColor, BorderColor = borderColor });

    public void DrawImage(string path, float width, float height)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height) });

    public void DrawImage(string path, float width, float height, Vector4 tintColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor });

    public void DrawImage(string path, float width, float height, Vector4 tintColor, Vector4 borderColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor, BorderColor = borderColor });

    public void DrawImage(string path, float width, float height, DrawInfo drawInfo)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height) });

    public void DrawImage(string path, float width, float height, DrawInfo drawInfo, Vector4 tintColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor });

    public void DrawImage(string path, float width, float height, DrawInfo drawInfo, Vector4 tintColor, Vector4 borderColor)
        => DrawImage(path, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor, BorderColor = borderColor });
    #endregion

    #region From Game
    public void DrawIcon(GameIconLookup gameIconLookup, DrawInfo drawInfo)
    {
        IDalamudTextureWrap? textureWrap = textureProvider.GetFromGameIcon(gameIconLookup).GetWrapOrEmpty();
        Draw(textureWrap, drawInfo);
    }

    // These overloads simplify the drawing of icons by providing multiple ways to specify the icon (e.g., by integer ID, whether it's HQ or not, etc.).
    public void DrawIcon(int iconId, bool isHq, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup((uint)iconId, isHq), drawInfo);

    public void DrawIcon(uint iconId, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup(iconId), drawInfo);

    public void DrawIcon(int iconId, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup((uint)iconId), drawInfo);

    public void DrawIcon(int iconId, Vector2 drawSize)
        => DrawIcon(iconId, new DrawInfo { DrawSize = drawSize });

    public void DrawIcon(int iconId, Vector2 drawSize, Vector4 tintColor)
        => DrawIcon(iconId, new DrawInfo { DrawSize = drawSize, TintColor = tintColor });

    public void DrawIcon(int iconId, Vector2 drawSize, Vector4 tintColor, Vector4 borderColor)
        => DrawIcon(iconId, new DrawInfo { DrawSize = drawSize, TintColor = tintColor, BorderColor = borderColor });

    public void DrawIcon(int iconId)
        => DrawIcon(iconId, new DrawInfo());

    public void DrawIcon(int iconId, float size)
        => DrawIcon(iconId, new DrawInfo { DrawSize = new Vector2(size) });

    public void DrawIcon(int iconId, bool isHq)
        => DrawIcon(iconId, isHq, new DrawInfo());

    public void DrawIcon(int iconId, bool isHq, Vector2 drawSize)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = drawSize });

    public void DrawIcon(int iconId, bool isHq, Vector2 drawSize, Vector4 tintColor)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = drawSize, TintColor = tintColor });

    public void DrawIcon(int iconId, bool isHq, Vector2 drawSize, Vector4 tintColor, Vector4 borderColor)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = drawSize, TintColor = tintColor, BorderColor = borderColor });

    public void DrawIcon(int iconId, bool isHq, float size)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = new Vector2(size) });

    public void DrawIcon(int iconId, bool isHq, float width, float height)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = new Vector2(width, height) });

    public void DrawIcon(int iconId, bool isHq, float width, float height, Vector4 tintColor)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor });

    public void DrawIcon(int iconId, bool isHq, float width, float height, Vector4 tintColor, Vector4 borderColor)
        => DrawIcon(iconId, isHq, new DrawInfo { DrawSize = new Vector2(width, height), TintColor = tintColor, BorderColor = borderColor });
    #endregion

}

public struct DrawInfo
{
    public DrawInfo()
    {
        DrawSize = null;
        Uv0 = null;
        Uv1 = null;
        TintColor = null;
        BorderColor = null;
        TransformUv = false;
        Scale = Vector2.One;
    }

    public DrawInfo(Vector2 size)
    {
        DrawSize = size;
        Uv0 = null;
        Uv1 = null;
        TintColor = null;
        BorderColor = null;
        TransformUv = false;
        Scale = Vector2.One;
    }

    public DrawInfo(Vector2 size, Vector4 tintColor)
    {
        DrawSize = size;
        Uv0 = null;
        Uv1 = null;
        TintColor = tintColor;
        BorderColor = null;
        TransformUv = false;
        Scale = Vector2.One;
    }

    public DrawInfo(Vector2 size, Vector4 tintColor, Vector4 borderColor)
    {
        DrawSize = size;
        Uv0 = null;
        Uv1 = null;
        TintColor = tintColor;
        BorderColor = borderColor;
        TransformUv = false;
        Scale = Vector2.One;
    }

    public DrawInfo(Vector2 size, Vector4 tintColor, Vector4 borderColor, Vector2 scale)
    {
        DrawSize = size;
        Uv0 = null;
        Uv1 = null;
        TintColor = tintColor;
        BorderColor = borderColor;
        TransformUv = false;
        Scale = scale;
    }

    public DrawInfo(float size)
        : this(new Vector2(size)) { }

    public DrawInfo(float width, float height)
        : this(new Vector2(width, height)) { }

    public DrawInfo(float size, Vector4 tintColor)
        : this(new Vector2(size), tintColor) { }

    public DrawInfo(float width, float height, Vector4 tintColor)
        : this(new Vector2(width, height), tintColor) { }

    public DrawInfo(float size, Vector4 tintColor, Vector4 borderColor)
        : this(new Vector2(size), tintColor, borderColor) { }

    public DrawInfo(float width, float height, Vector4 tintColor, Vector4 borderColor)
        : this(new Vector2(width, height), tintColor, borderColor) { }

    public DrawInfo(float size, Vector4 tintColor, Vector4 borderColor, Vector2 scale)
        : this(new Vector2(size), tintColor, borderColor, scale) { }

    public DrawInfo(float width, float height, Vector4 tintColor, Vector4 borderColor, Vector2 scale)
        : this(new Vector2(width, height), tintColor, borderColor, scale) { }

    public Vector2? DrawSize { get; set; }
    public Vector2? Uv0 { get; set; }
    public Vector2? Uv1 { get; set; }
    public Vector4? TintColor { get; set; }
    public Vector4? BorderColor { get; set; }
    public bool TransformUv { get; set; }
    public Vector2 Scale { get; set; }

    public static implicit operator DrawInfo(Vector2 size) => new(size);
    public static implicit operator DrawInfo(float size) => new(size);
    public static implicit operator DrawInfo(int size) => new(size);
}