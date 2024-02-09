using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct Texture3DWrapping
{
    public required TextureWrapMode WrapModeS;
    public required TextureWrapMode WrapModeT;
    public required TextureWrapMode WrapModeR;

    public static readonly Texture3DWrapping Repeat = new Texture3DWrapping 
    { 
        WrapModeS = TextureWrapMode.Repeat, 
        WrapModeT = TextureWrapMode.Repeat,
        WrapModeR = TextureWrapMode.Repeat
    };
    public static readonly Texture3DWrapping ClampToEdge = new Texture3DWrapping 
    { 
        WrapModeS = TextureWrapMode.ClampToEdge, 
        WrapModeT = TextureWrapMode.ClampToEdge,
        WrapModeR = TextureWrapMode.ClampToEdge
    };
    public static readonly Texture3DWrapping ClampToBorder = new Texture3DWrapping 
    { 
        WrapModeS = TextureWrapMode.ClampToBorder, 
        WrapModeT = TextureWrapMode.ClampToBorder,
        WrapModeR = TextureWrapMode.ClampToBorder
    };
    public static readonly Texture3DWrapping MirroredRepeat = new Texture3DWrapping 
    { 
        WrapModeS = TextureWrapMode.MirroredRepeat, 
        WrapModeT = TextureWrapMode.MirroredRepeat,
        WrapModeR = TextureWrapMode.MirroredRepeat
    };
}
