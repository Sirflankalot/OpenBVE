using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    internal class Shader {
        private GL.ShaderType type;
        private string source;
        private int gl_identifier;
        private bool compiled;

        public Shader(GL.ShaderType type, byte[] source) {
            this.type = type;
            this.source = System.Text.Encoding.UTF8.GetString(source);
            gl_identifier = 0;
            compiled = false;
        }

        public void Clear() {
            GLFunc.DeleteShader(gl_identifier);
        }

        public int Compile() {
            if (compiled) {
                return gl_identifier;
            }

            gl_identifier = GLFunc.CreateShader(type);
            GLFunc.ShaderSource(gl_identifier, source);
            GLFunc.CompileShader(gl_identifier);

            var info_log = GLFunc.GetShaderInfoLog(gl_identifier);
            if(info_log.Length != 0) {
                // TODO: Log error
                throw new System.Exception("Shader compilation failed:\n" + info_log);
            }

            compiled = true;
            return gl_identifier;
        }
    }

    internal class ShaderProgram {
        private int gl_ident;
        public ShaderProgram(params Shader[] shaders) {
            gl_ident = GLFunc.CreateProgram();
            foreach(Shader s in shaders) {
                GLFunc.AttachShader(gl_ident, s.Compile());
            }

            GLFunc.LinkProgram(gl_ident);

            var log = GLFunc.GetProgramInfoLog(gl_ident);
            if (log.Length != 0) {
                // TODO: Log error
                throw new System.Exception("Shader linking failed:\n" + log);
            }

            foreach(Shader s in shaders) {
                GLFunc.DetachShader(gl_ident, s.Compile());
            }
        }

        public void Use() {
            GLFunc.UseProgram(gl_ident);
        }

        public void Clear() {
            GLFunc.DeleteProgram(gl_ident);
        }
    }
}
