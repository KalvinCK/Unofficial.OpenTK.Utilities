using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class Texture2DMultiSampler() : TexturesMultiSamplerImplements(TextureTargetMultisample2d.Texture2DMultisample)
{
    public int Width => base._Width; 
    public int Height => base._Height;
    public int Samples => base._Samples;
    public bool FixedSampleLocations => base._FixedSampleLocations;
    public Size Size => new Size(Width, Height);
    public Texture2DMultiSampler(SizedInternalFormat SizedInternalFormat, int width, int height, int samples = 4, bool fixedSampleLocations = true) : this()
    {
        base.AllocateTextures(SizedInternalFormat, width, height, 1, samples, fixedSampleLocations);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int samples = 4, bool fixedSampleLocations = true)
    {
        base.AllocateTextures(SizedInternalFormat, width, height, 1, samples, fixedSampleLocations);
    }
}
