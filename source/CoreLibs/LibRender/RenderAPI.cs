namespace LibRender {
    public partial class Renderer {
        internal Shader vs;
        internal Shader fs;
        internal ShaderProgram prog;

        public void initialize() {
            vs = new Shader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader, Shaders.basic_vs);
            fs = new Shader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader, Shaders.basic_fs);
            prog = new ShaderProgram(vs, fs);
        }
    }
}