using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class Texture2DMultiSamplerArray() : TexturesMultiSamplerImplements(TextureTargetMultisample3d.Texture2DMultisampleArray)
{
    public int Width => base._Width; 
    public int Height => base._Height;
    public int Layers => base._Depth;
    public int Samples => base._Samples;
    public bool FixedSampleLocations => base._FixedSampleLocations;

    public Texture2DMultiSamplerArray(SizedInternalFormat SizedInternalFormat, int width, int height, int layers, int samples = 4, bool fixedSampleLocations = true) : this()
    {
        base.AllocateTextures(SizedInternalFormat, width, height, layers, samples, fixedSampleLocations);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int layers, int samples = 4, bool fixedSampleLocations = true)
    {
        base.AllocateTextures(SizedInternalFormat, width, height, layers, samples, fixedSampleLocations);
    }
}
