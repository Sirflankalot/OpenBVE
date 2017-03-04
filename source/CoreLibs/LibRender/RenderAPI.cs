using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal Shader vertex_shader;
        internal Shader fragment_shader;
        internal ShaderProgram prog;

        public void initialize() {
            vertex_shader = new Shader(GL.ShaderType.VertexShader, Shader_Sources.basic_vs);
            fragment_shader = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.basic_fs);
            prog = new ShaderProgram(vertex_shader, fragment_shader);
        }

        public void update() {
            Algorithms.update_mesh_normals(meshes, 0, meshes.Count);
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