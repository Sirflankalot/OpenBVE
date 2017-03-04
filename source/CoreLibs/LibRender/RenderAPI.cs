namespace LibRender {
    public partial class Renderer {
        internal Shader vertex_shader;
        internal Shader fragment_shader;
        internal ShaderProgram prog;

        public void initialize() {
            vertex_shader = new Shader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader, Shaders.basic_vs);
            fragment_shader = new Shader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader, Shaders.basic_fs);
            prog = new ShaderProgram(vertex_shader, fragment_shader);
        }

        public void update() {

        }

        public void render_all() {

        }

        public void deinitialize() {
            vertex_shader.clear();
            fragment_shader.clear();
            prog.clear();
        }
    }
}