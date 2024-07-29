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
using ImGuiNET;
using Plugin.Utility.UI;

namespace Plugin.OtherServices;

public class TextureService
{
    private readonly ITextureProvider textureProvider;
    private readonly IDataManager dataManager;
    private readonly string uiBasePath;

    public TextureService(ITextureProvider textureProvider, IDataManager dataManager)
    {
        this.textureProvider = textureProvider;
        this.dataManager = dataManager;
        this.uiBasePath = uiBasePath;
    }

    public void Draw(string path, DrawInfo drawInfo)
    {
        var textureWrap = textureProvider.GetFromGame(path).GetWrapOrEmpty();
        Draw(textureWrap, drawInfo);
    }

    public void DrawIcon(GameIconLookup gameIconLookup, DrawInfo drawInfo)
    {
        var textureWrap = textureProvider.GetFromGameIcon(gameIconLookup).GetWrapOrEmpty();
        Draw(textureWrap, drawInfo);
    }

    public void DrawIcon(int iconId, bool isHq, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup((uint)iconId, isHq), drawInfo);

    public void DrawIcon(uint iconId, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup(iconId), drawInfo);

    public void DrawIcon(int iconId, DrawInfo drawInfo)
        => DrawIcon(new GameIconLookup((uint)iconId), drawInfo);

    public static void Draw(IDalamudTextureWrap? textureWrap, DrawInfo drawInfo)
    {
        if (textureWrap == null)
        {
            ImGui.Dummy(drawInfo.DrawSize ?? Vector2.Zero);
            return;
        }

        var size = drawInfo.DrawSize ?? textureWrap.Size;

        if (!ImGuiExt.IsInViewport(size))
        {
            ImGui.Dummy(size);
            return;
        }

        var uv0 = drawInfo.Uv0 ?? Vector2.Zero;
        var uv1 = drawInfo.Uv1 ?? Vector2.One;

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


    // Overloaded methods for convenience
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

}

public struct DrawInfo
{
    public DrawInfo()
    {
    }

    public DrawInfo(Vector2 size)
    {
        DrawSize = size;
    }

    public DrawInfo(float size)
    {
        DrawSize = new(size);
    }

    public DrawInfo(float width, float height)
    {
        DrawSize = new(width, height);
    }

    public Vector2? DrawSize { get; set; }
    public Vector2? Uv0 { get; set; }
    public Vector2? Uv1 { get; set; }
    public Vector4? TintColor { get; set; }
    public Vector4? BorderColor { get; set; }
    public bool TransformUv { get; set; }

    public static implicit operator DrawInfo(Vector2 size) => new(size);
    public static implicit operator DrawInfo(float size) => new(size);
    public static implicit operator DrawInfo(int size) => new(size);
}