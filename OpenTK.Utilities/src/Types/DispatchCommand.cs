namespace OpenTK.Utilities;

public class DispatchCommand(uint numGroupsX = 1, uint numGroupsY = 1, uint numGroupsZ = 1)
{
    public uint NumGroupsX { get; set; } = numGroupsX;

    public uint NumGroupsY { get; set; } = numGroupsY;

    public uint NumGroupsZ { get; set; } = numGroupsZ;
}
