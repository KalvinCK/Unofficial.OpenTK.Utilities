﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class Texture2DMultisamplerArray() : TexturesMultisamplerImplements(TextureTargetMultisample3d.Texture2DMultisampleArray)
{
    public Texture2DMultisamplerArray(TextureFormat TextureFormat, int width, int height, int layers, int samples = 4, bool fixedSampleLocations = true)
        : this()
    {
        this.AllocateTextures(TextureFormat, width, height, layers, samples, fixedSampleLocations);
    }

    public new int Width => base.Width;

    public new int Height => base.Height;

    public int Layers => this.Depth;

    public Vector3i Size => new Vector3i(base.Width, base.Height, this.Depth);

    public void ToAllocate(TextureFormat TextureFormat, int width, int height, int layers, int samples = 4, bool fixedSampleLocations = true)
    {
        this.AllocateTextures(TextureFormat, width, height, layers, samples, fixedSampleLocations);
    }
}
