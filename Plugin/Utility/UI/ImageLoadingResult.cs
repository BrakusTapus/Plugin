using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;

namespace Plugin.Utility.UI;

public class ImageLoadingResult
{
    internal ISharedImmediateTexture? ImmediateTexture;
    internal IDalamudTextureWrap? TextureWrap;
    internal IDalamudTextureWrap? Texture => ImmediateTexture?.GetWrapOrDefault() ?? TextureWrap;
    internal bool IsCompleted = false;

    public ImageLoadingResult(ISharedImmediateTexture? immediateTexture)
    {
        ImmediateTexture = immediateTexture;
    }

    public ImageLoadingResult(IDalamudTextureWrap? textureWrap)
    {
        TextureWrap = textureWrap;
    }

    public ImageLoadingResult()
    {
    }
}
