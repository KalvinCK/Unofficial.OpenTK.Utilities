namespace OpenTK.Utilities;

public struct DispatchCommand(int numGroupsX = 1, int numGroupsY = 1, int numGroupsZ = 1)
{
    public int NumGroupsX = numGroupsX;
    public int NumGroupsY = numGroupsY;
    public int NumGroupsZ = numGroupsZ;
}
