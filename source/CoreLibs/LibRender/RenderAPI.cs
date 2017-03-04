using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal Shader vertex_shader;
        internal Shader fragment_shader;
        internal ShaderProgram prog;

        public void Initialize() {
            vertex_shader = new Shader(GL.ShaderType.VertexShader, Shader_Sources.basic_vs);
            fragment_shader = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.basic_fs);
            prog = new ShaderProgram(vertex_shader, fragment_shader);
        }

        public void Update() {
            Algorithms.UpdateMeshNormals(meshes, 0, meshes.Count);
        }

        public void RenderAll() {

        }

        public void Deinitialize() {
            vertex_shader.Clear();
            fragment_shader.Clear();
            prog.Clear();
        }
    }
}