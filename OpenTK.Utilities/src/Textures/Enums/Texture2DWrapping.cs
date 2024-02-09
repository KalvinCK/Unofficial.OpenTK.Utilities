using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct Texture2DWrapping
{
    public required TextureWrapMode WrapModeS;
    public required TextureWrapMode WrapModeT;

    public static readonly Texture2DWrapping Repeat = new Texture2DWrapping { WrapModeS = TextureWrapMode.Repeat, WrapModeT = TextureWrapMode.Repeat };
    public static readonly Texture2DWrapping ClampToEdge = new Texture2DWrapping { WrapModeS = TextureWrapMode.ClampToEdge, WrapModeT = TextureWrapMode.ClampToEdge };
    public static readonly Texture2DWrapping ClampToBorder = new Texture2DWrapping { WrapModeS = TextureWrapMode.ClampToBorder, WrapModeT = TextureWrapMode.ClampToBorder };
    public static readonly Texture2DWrapping MirroredRepeat = new Texture2DWrapping { WrapModeS = TextureWrapMode.MirroredRepeat, WrapModeT = TextureWrapMode.MirroredRepeat };
}
