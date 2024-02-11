﻿using System.Diagnostics;

namespace OpenTK.Utilities;

internal class Helpers
{
    [Conditional("DEBUG")]
    public static void PrintWarning(string msg)
    {
        Debug.Print(msg);
    }
}
