namespace OpenTK.Utilities.Images;

public struct CubeTexturesPack
{
    public CubeTexturesPack(string rootPath, params string[] files)
    {
        if (files.Length != 6)
        {
            throw new Exception("A cube texture pack must have 6 textures in total");
        }

        this.RootPathTextures = rootPath;
        this.Files = files;
    }

    public string RootPathTextures { get; set; }

    public string[] Files { get; set; }
}
