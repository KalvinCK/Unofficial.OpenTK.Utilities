using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct Texture3DWrapping
{
    public static readonly Texture3DWrapping Repeat = new Texture3DWrapping
    {
        WrapModeS = TextureWrapMode.Repeat,
        WrapModeT = TextureWrapMode.Repeat,
        WrapModeR = TextureWrapMode.Repeat,
    };

    public static readonly Texture3DWrapping ClampToEdge = new Texture3DWrapping
    {
        WrapModeS = TextureWrapMode.ClampToEdge,
        WrapModeT = TextureWrapMode.ClampToEdge,
        WrapModeR = TextureWrapMode.ClampToEdge,
    };

    public static readonly Texture3DWrapping ClampToBorder = new Texture3DWrapping
    {
        WrapModeS = TextureWrapMode.ClampToBorder,
        WrapModeT = TextureWrapMode.ClampToBorder,
        WrapModeR = TextureWrapMode.ClampToBorder,
    };

    public static readonly Texture3DWrapping MirroredRepeat = new Texture3DWrapping
    {
        WrapModeS = TextureWrapMode.MirroredRepeat,
        WrapModeT = TextureWrapMode.MirroredRepeat,
        WrapModeR = TextureWrapMode.MirroredRepeat,
    };

    public Texture3DWrapping(TextureWrapMode TextureWrapModeS, TextureWrapMode TextureWrapModeT, TextureWrapMode TextureWrapModeR)
    {
        this.WrapModeS = TextureWrapModeS;
        this.WrapModeT = TextureWrapModeT;
        this.WrapModeR = TextureWrapModeR;
    }

    public TextureWrapMode WrapModeS { get; set; }

    public TextureWrapMode WrapModeT { get; set; }

    public TextureWrapMode WrapModeR { get; set; }
}
