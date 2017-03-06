using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal void ClearScreen() {
            GLFunc.ClearColor(0, 0, 0, 1);
            GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit);
        }

        internal void RenderAllObjects() {
            GLFunc.CullFace(GL.CullFaceMode.Back);

            prog.Use();

            GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
            
            foreach (Object o in objects) {
                if (o == null || !o.visible || meshes[o.mesh_id] == null || textures[o.tex_id] == null) {
                    continue;
                }

                // Reference to subobjects
                Mesh m = meshes[o.mesh_id];
                Texture t = textures[o.tex_id];
                Camera c = cameras[active_camera];

                GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

                GLFunc.UniformMatrix4(prog.GetUniform("world_mat"), false, ref o.transform);
                GLFunc.UniformMatrix4(prog.GetUniform("view_mat"), false, ref c.transform);

                GLFunc.BindVertexArray(m.gl_vao_id);

                GLFunc.DrawElements(GL.BeginMode.Triangles, m.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

                GLFunc.BindVertexArray(0);
            }

            GLFunc.CullFace(GL.CullFaceMode.Front);
        }
    }
}