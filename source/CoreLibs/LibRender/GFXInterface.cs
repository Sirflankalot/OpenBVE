using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    internal static class GFXInterface {
        internal static void UpdateVAOVBO(List<Mesh> meshes, int start, int end) {
            // Check for valid indices
            if (!(0 <= start && 0 <= end && end <= meshes.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            // List of meshes that need buffers created
            List<int> vaoless_meshes = new List<int>();
            List<int> vertless_meshes = new List<int>();
            List<int> indicesless_meshes = new List<int>();
            for (int i = start; i < end; ++i) {
                if (meshes[i] == null) {
                    continue;
                }
                if (meshes[i].gl_vao_id == 0) {
                    vaoless_meshes.Add(i);
                }
                if (meshes[i].gl_vert_id == 0) {
                    vertless_meshes.Add(i);
                }
                if (meshes[i].gl_indices_id == 0) {
                    indicesless_meshes.Add(i);
                }
            }

            if (vaoless_meshes.Count == 0 && vertless_meshes.Count == 0 && indicesless_meshes.Count == 0) {
                return;
            }

            // Generate VAOs and VBOs
            int[] vao_ids = new int[vaoless_meshes.Count];
            int[] vert_ids = new int[vertless_meshes.Count];
            int[] indices_ids = new int[indicesless_meshes.Count];
            GLFunc.GenVertexArrays(vaoless_meshes.Count, vao_ids);
            GLFunc.GenBuffers(vertless_meshes.Count, vert_ids);
            GLFunc.GenBuffers(indicesless_meshes.Count, indices_ids);

            // Give out VAOs
            for (int i = 0; i < vaoless_meshes.Count; ++i) {
                meshes[vaoless_meshes[i]].gl_vao_id = vao_ids[i];
            }

            // Give out Verticies VBOs
            for (int i = 0; i < vertless_meshes.Count; ++i) {
                meshes[vertless_meshes[i]].gl_vert_id = vert_ids[i];
            }

            // Give out Indices VBOs
            for (int i = 0; i < indicesless_meshes.Count; ++i) {
                meshes[indicesless_meshes[i]].gl_indices_id = indices_ids[i];
            }
        }

        internal static void UploadMeshes(List<Mesh> meshes, int start, int end) {
            // Check for valid indices
            if (!(0 <= start && 0 <= end && end <= meshes.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to mesh
                Mesh m = meshes[i];

                if (m == null || m.uploaded) {
                    continue;
                }

                GLFunc.BindVertexArray(m.gl_vao_id);
                GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, m.gl_vert_id);
                // Fix this garbage
                List<Vertex> vertex_full = new List<Vertex>();
                for (int j = 0; j < m.normals.Count; ++j) {
                    m.vertices.Add(new Vertex() { normal = m.normals[j], position = m.vertices[j].position, tex_pos = m.vertices[j].tex_pos });
                }
                GLFunc.BufferData(GL.BufferTarget.ArrayBuffer, vertex_full.Count, vertex_full.ToArray(), GL.BufferUsageHint.StaticDraw);

                GLFunc.VertexAttribPointer(0, 3, GL.VertexAttribPointerType.Float, false, 8, 0 * sizeof(float));
                GLFunc.VertexAttribPointer(1, 2, GL.VertexAttribPointerType.Float, false, 8, 3 * sizeof(float));
                GLFunc.VertexAttribPointer(2, 3, GL.VertexAttribPointerType.Float, false, 8, 5 * sizeof(float));

                GLFunc.EnableVertexAttribArray(0);
                GLFunc.EnableVertexAttribArray(1);
                GLFunc.EnableVertexAttribArray(2);

                GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, m.gl_indices_id);
                GLFunc.BufferData(GL.BufferTarget.ElementArrayBuffer, m.indices.Count, m.indices.ToArray(), GL.BufferUsageHint.StaticDraw);

                GLFunc.BindVertexArray(0);
                GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, 0);
                GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, 0);

                m.uploaded = true;
            }
        }

        internal static void UpdateTextureObjects(List<Texture> textures, int start, int end) {
            // Check for valid indices
            if (!(0 <= start && 0 <= end && end <= textures.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            // Find textures that need OpenGL ids
            List<int> idless_textures = new List<int>();
            for (int i = start; i < end; ++i) {
                if (textures[i] == null) {
                    continue;
                }
                if (textures[i].gl_id == 0) {
                    idless_textures.Add(i);
                }
            }


            // Create OpenGL textures
            int[] ids = new int[idless_textures.Count];
            GLFunc.CreateTextures(GL.TextureTarget.Texture2D, idless_textures.Count, ids);

            // Give out textures
            for (int i = 0; i < idless_textures.Count; ++i) {
                textures[idless_textures[i]].gl_id = ids[i];
            }
        }

        internal static void UploadTextures(List<Texture> textures, int start, int end) {
            // Check for valid indices
            if (!(0 <= start && 0 <= end && end <= textures.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to object
                Texture t = textures[i];

                if (t == null || t.uploaded) {
                    continue;
                }

                GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);
                GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba16f, t.width, t.height, 0, GL.PixelFormat.Rgba, GL.PixelType.UnsignedByte, t.pixels.ToArray());
                GLFunc.GenerateMipmap(GL.GenerateMipmapTarget.Texture2D);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToBorder);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToBorder);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.LinearMipmapLinear);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Linear);
                GLFunc.BindTexture(GL.TextureTarget.Texture2D, 0);

                t.uploaded = true;
            }
        }
    }
}
