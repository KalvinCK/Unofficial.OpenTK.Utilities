using System;
using System.IO;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images
{
    public static unsafe partial class ImageCore
    {
        public static string GFailureReason { get; private set; }

        public static readonly char[] ParsePngFileInvalidChunk = new char[25];

        public static int NativeAllocations => MemoryStats.Allocations;

        public class Context
        {
            public byte[] _tempBuffer;
            public int img_n = 0;
            public int img_out_n = 0;
            public uint img_x = 0;
            public uint img_y = 0;

            public Context(Stream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                Stream = stream;
            }

            public Stream Stream { get; }
        }

        private static int Err(string str)
        {
            GFailureReason = str;
            return 0;
        }

        public static byte Get8(Context s)
        {
            var b = s.Stream.ReadByte();
            if (b == -1) return 0;

            return (byte)b;
        }

        public static void Skip(Context s, int skip)
        {
            s.Stream.Seek(skip, SeekOrigin.Current);
        }

        public static void Rewind(Context s)
        {
            s.Stream.Seek(0, SeekOrigin.Begin);
        }

        public static int AtEof(Context s)
        {
            return s.Stream.Position == s.Stream.Length ? 1 : 0;
        }

        public static int Getn(Context s, byte* buf, int size)
        {
            if (s._tempBuffer == null ||
                s._tempBuffer.Length < size)
                s._tempBuffer = new byte[size * 2];

            var result = s.Stream.Read(s._tempBuffer, 0, size);
            Marshal.Copy(s._tempBuffer, 0, new IntPtr(buf), result);

            return result;
        }
    }
}
