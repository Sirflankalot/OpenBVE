using System;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender
{
    public static class Error {
        /// <summary>Checks whether an OpenGL error has occured this frame</summary>
        /// <param name="Location">The location of the caller (The main loop or the loading screen loop)</param>
        internal static void CheckForOpenGlError(string Location) {
            var error = GLFunc.GetError();
            if (error != GL.ErrorCode.NoError) {
                string message = Location + ": ";
                switch (error) {
                    case GL.ErrorCode.InvalidEnum:
                        message += "GL_INVALID_ENUM";
                        break;
                    case GL.ErrorCode.InvalidValue:
                        message += "GL_INVALID_VALUE";
                        break;
                    case GL.ErrorCode.InvalidOperation:
                        message += "GL_INVALID_OPERATION";
                        break;
                    case GL.ErrorCode.StackOverflow:
                        message += "GL_STACK_OVERFLOW";
                        break;
                    case GL.ErrorCode.StackUnderflow:
                        message += "GL_STACK_UNDERFLOW";
                        break;
                    case GL.ErrorCode.OutOfMemory:
                        message += "GL_OUT_OF_MEMORY";
                        break;
                    case GL.ErrorCode.TableTooLargeExt:
                        message += "GL_TABLE_TOO_LARGE";
                        break;
                    default:
                        message += error.ToString();
                        break;
                }
                throw new InvalidOperationException(message);
            }
        }
    }
}

