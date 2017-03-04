using System;
using System.Runtime.InteropServices;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace OpenBve
{
    public static class OpenGLCallback
    {
        private static int callback_count = 0;

        public static void callback(GL.DebugSource ds, GL.DebugType dt, int id, GL.DebugSeverity severity, int length, IntPtr message, IntPtr userParam) {
            System.Text.StringBuilder error = new System.Text.StringBuilder();
            callback_count++;

            error.Append("OpenGL Error #");
            error.Append(callback_count);
            error.Append(": \n");
            error.Append("              message: ");
            error.Append(Marshal.PtrToStringAnsi(message, length));
            error.Append("\n");

            error.Append("              type: ");
            switch (dt) {
                case GL.DebugType.DebugTypeError:
                    error.Append("ERROR");
                    break;
                case GL.DebugType.DebugTypeDeprecatedBehavior:
                    error.Append("DEPRECATED BEHAVIOR");
                    break;
                case GL.DebugType.DebugTypeUndefinedBehavior:
                    error.Append("UNDEFINED BEHAVIOR");
                    break;
                case GL.DebugType.DebugTypePortability:
                    error.Append("PORTABILITY");
                    break;
                case GL.DebugType.DebugTypePerformance:
                    error.Append("PERFORMANCE");
                    break;
                case GL.DebugType.DebugTypeMarker:
                    error.Append("MARKER");
                    break;
                case GL.DebugType.DebugTypePopGroup:
                    error.Append("POP GROUP");
                    break;
                case GL.DebugType.DebugTypePushGroup:
                    error.Append("PUSH GROUP");
                    break;
                case GL.DebugType.DebugTypeOther:
                    error.Append("OTHER");
                    break;
            }
            error.Append('\n');

            error.Append("              id: ");
            error.Append(id);
            error.Append("\n");

            error.Append("              serverity: ");
            switch (severity) {
                case GL.DebugSeverity.DebugSeverityLow:
                    error.Append("low");
                    break;
                case GL.DebugSeverity.DebugSeverityMedium:
                    error.Append("medium");
                    break;
                case GL.DebugSeverity.DebugSeverityHigh:
                    error.Append("high");
                    break;
                case GL.DebugSeverity.DebugSeverityNotification:
                    error.Append("info");
                    break;
            }
            error.Append('\n');

            Program.AppendToLogFile(error.ToString());
        }
    }
}

