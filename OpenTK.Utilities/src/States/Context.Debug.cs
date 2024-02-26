using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public static partial class Context
{
    public static class Debug
    {
        private static bool enableDebug = false;

        public static bool Enable
        {
            get => enableDebug;
            set
            {
                if (enableDebug == value)
                {
                    return;
                }

                enableDebug = value;

                var state = GL.GetInteger(GetPName.ContextFlags) is (int)ContextFlagMask.ContextFlagDebugBit;

                if (state)
                {
                    string exeption = "Failed to activate debug.\n" +
                        "Check support: Check whether your hardware and drivers support OpenGL debug mode. " +
                        "Not all drivers or graphics cards can provide full support.\n" +
                        "Context configuration: When creating the OpenGL context (using a library like GLFW, SDL, etc.), " +
                        "you need to specify that you want a debug context. This can usually be done through configuration " +
                        "options or specific function calls when initializing the OpenGL context.";

                    throw new Exception(exeption);
                }

                if (enableDebug)
                {
                    GL.Enable(EnableCap.DebugOutput);
                    GL.Enable(EnableCap.DebugOutputSynchronous);
                    GL.DebugMessageCallback(
                        (DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) =>
                        {
                            if (id is 131169 || id is 131185 || id is 131218 || id is 131204)
                            {
                                return;
                            }

                            string printMsgWarning = $"Source: [{source.ToString().ToLower()}]\nID: {id}\nSeverity: [{severity.ToString().ToLower()}]\nLevel: [{type.ToString().ToLower()}]\nMessage: {Marshal.PtrToStringAnsi(message, length)}\n";
                            Console.WriteLine(printMsgWarning);
                        }, IntPtr.Zero);
                }
                else
                {
                    GL.Disable(EnableCap.DebugOutput);
                    GL.Disable(EnableCap.DebugOutputSynchronous);
                    GL.DebugMessageCallback(null, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        /// Defines the behavior for debug messages.
        /// </summary>
        /// <param name="control">The source of the debug message you want to track.</param>
        /// <param name="type">The type of message you want to control.</param>
        /// <param name="severity">Specifies the severity of the message.</param>
        /// <param name="enable">Specifies whether messages should <c></c> not be ignored.</param>
        /// <param name="msgIDs">An array of message IDs that you want to track. <a>To control all messages pass an empty array.</a>
        /// </param>
        public static void DefineMessageBehavior(
            DebugSourceControl control,
            DebugTypeControl type,
            DebugSeverityControl severity,
            bool enable,
            params int[] msgIDs)
        {
            GL.DebugMessageControl(control, type, severity, msgIDs.Length, msgIDs, enable);
        }
    }
}
