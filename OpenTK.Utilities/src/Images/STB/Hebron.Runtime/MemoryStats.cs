namespace Hebron.Runtime;

internal static unsafe class MemoryStats
{
    private static int allocations;

    public static int Allocations => allocations;

    public static void Allocated()
    {
        Interlocked.Increment(ref allocations);
    }

    public static void Freed()
    {
        Interlocked.Decrement(ref allocations);
    }
}
