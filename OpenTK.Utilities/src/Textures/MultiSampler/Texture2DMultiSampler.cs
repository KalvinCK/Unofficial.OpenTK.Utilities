﻿using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class Texture2DMultiSampler() : TexturesMultiSamplerImplements(TextureTargetMultisample2d.Texture2DMultisample)
{
    public Texture2DMultiSampler(SizedInternalFormat SizedInternalFormat, int width, int height, int samples = 4, bool fixedSampleLocations = true)
        : this()
    {
        this.AllocateTextures(SizedInternalFormat, width, height, 1, samples, fixedSampleLocations);
    }

    public new int Width => base.Width;

    public new int Height => base.Height;

    public Size Size => new Size(base.Width, base.Height);

    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int samples = 4, bool fixedSampleLocations = true)
    {
        this.AllocateTextures(SizedInternalFormat, width, height, 1, samples, fixedSampleLocations);
    }
}
