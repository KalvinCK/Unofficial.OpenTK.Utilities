namespace OpenTK.Utilities.Textures;

public struct CubemapTexturesPath(string rootPath, params string[] files)
{
    public string RootPathTextures = rootPath;

    public string[] Files = files;
}
