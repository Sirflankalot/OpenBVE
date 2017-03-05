using OpenTK;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal Shader vertex_shader;
        internal Shader fragment_shader;
        internal ShaderProgram prog;

        internal Matrix4 projection_matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f), 1, 0.1f, 1000.0f);

        public void Initialize() {
            vertex_shader = new Shader(GL.ShaderType.VertexShader, Shader_Sources.basic_vs);
            fragment_shader = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.basic_fs);
            prog = new ShaderProgram(vertex_shader, fragment_shader);
        }

        public void PrepareForRender() {
            Algorithms.UpdateMeshNormals(meshes, 0, meshes.Count);
            Algorithms.UpdateObjectMatrices(objects, 0, objects.Count);
            GFXInterface.UpdateVAOVBO(meshes, 0, meshes.Count);
            GFXInterface.UploadMeshes(meshes, 0, meshes.Count);
            GFXInterface.UpdateTextureObjects(textures, 0, textures.Count);
            GFXInterface.UploadTextures(textures, 0, textures.Count);
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