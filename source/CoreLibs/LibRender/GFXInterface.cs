using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
    internal static class GFXInterface {
        internal static void UpdateVAOVBO(List<Mesh> meshes, int start, int end) {
			Utilities.AssertValidIndicies(meshes, start, end);

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
			Utilities.AssertValidIndicies(meshes, start, end);

            for (int i = start; i < end; ++i) {
                // Reference to mesh
                Mesh m = meshes[i];

                if (m == null || m.uploaded) {
                    continue;
                }

                GLFunc.BindVertexArray(m.gl_vao_id);
                GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, m.gl_vert_id);
                // TODO: Fix copying of normals
                List<Vertex3D> vertex_full = new List<Vertex3D>();
                for (int j = 0; j < m.normals.Count; ++j) {
                    vertex_full.Add(new Vertex3D() { normal = m.normals[j], position = m.vertices[j].position, tex_pos = m.vertices[j].tex_pos });
                }
                unsafe {
                    GLFunc.BufferData(GL.BufferTarget.ArrayBuffer, vertex_full.Count * sizeof(Vertex3D), vertex_full.ToArray(), GL.BufferUsageHint.StaticDraw);

					GLFunc.VertexAttribPointer(0, 3, GL.VertexAttribPointerType.Float, false, sizeof(Vertex3D), 0 * sizeof(float)); // Location
					GLFunc.VertexAttribPointer(1, 2, GL.VertexAttribPointerType.Float, false, sizeof(Vertex3D), 3 * sizeof(float)); // Normals
					GLFunc.VertexAttribPointer(2, 3, GL.VertexAttribPointerType.Float, false, sizeof(Vertex3D), 5 * sizeof(float)); // TexCoords
				}

                GLFunc.EnableVertexAttribArray(0);
                GLFunc.EnableVertexAttribArray(1);
                GLFunc.EnableVertexAttribArray(2);

                GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, m.gl_indices_id);
                GLFunc.BufferData(GL.BufferTarget.ElementArrayBuffer, m.indices.Count * sizeof(int), m.indices.ToArray(), GL.BufferUsageHint.StaticDraw);

                GLFunc.BindVertexArray(0);
                GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, 0);
                GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, 0);

                m.uploaded = true;
            }
        }

        internal static void UpdateTextureObjects(List<Texture> textures, int start, int end) {
			Utilities.AssertValidIndicies(textures, start, end);

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

        internal static void UploadTextures(List<Texture> textures, int start, int end, GL.TextureMinFilter min, GL.TextureMagFilter mag, int aniso) {
			Utilities.AssertValidIndicies(textures, start, end);

            for (int i = start; i < end; ++i) {
                // Reference to texture
                Texture t = textures[i];

                if (t == null || t.uploaded) {
                    continue;
                }

                GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);
                GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Srgb8Alpha8, t.width, t.height, 0, GL.PixelFormat.Rgba, GL.PixelType.UnsignedByte, t.pixels.ToArray());
                GLFunc.GenerateMipmap(GL.GenerateMipmapTarget.Texture2D);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.Repeat);
                GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.Repeat);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) min);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) mag);
				if (aniso != 0) {
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, (GL.TextureParameterName) GL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, aniso);
				}
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, 0);

                t.uploaded = true;
            }
		}

		internal static void UpdateTextureFiltering(List<Texture> textures, GL.TextureMinFilter min, GL.TextureMagFilter mag, int aniso) {
			foreach (Texture t in textures) {
				if (t == null || !t.uploaded) {
					continue;
				}

				GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) min);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) mag);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, (GL.TextureParameterName) GL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, aniso);
			}
		}

		internal static void UpdateTextTextureObjects(List<Text> texts, int start, int end) {
			Utilities.AssertValidIndicies(texts, start, end);

			// Find all texts that need texture objects
			List<int> idless_texts = new List<int>();
			for (int i = start; i < end; ++i) {
				if (texts[i] == null) {
					continue;
				}
				if (texts[i].gl_tex_id == 0) {
					idless_texts.Add(i);
				}
			}

			// Create OpenGL Textures
			int[] ids = new int[idless_texts.Count];
			GLFunc.CreateTextures(GL.TextureTarget.Texture2D, idless_texts.Count, ids);

			// Distribute Textures
			for (int i = 0; i < idless_texts.Count; ++i) {
				texts[idless_texts[i]].gl_tex_id = ids[i];
			}
		}

		internal static void UploadTextTextures(List<Text> texts, int start, int end) {
			Utilities.AssertValidIndicies(texts, start, end);

			for (int i = start; i < end; ++i) {
				// Reference to text
				Text t = texts[i];

				if (t == null || t.uploaded) {
					continue;
				}

				GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_tex_id);
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba8, t.width, t.height, 0, GL.PixelFormat.Rgba, GL.PixelType.UnsignedByte, t.texture.ToArray());
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Linear);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Linear);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, 0);

				t.uploaded = true;
			}
		}

        internal static void UpdateFlatMeshVAOVBO(List<FlatMesh> flat_meshes, int start, int end) {
			Utilities.AssertValidIndicies(flat_meshes, start, end);

            List<int> vaoless_flat_meshes = new List<int>();
            List<int> vboless_flat_meshes = new List<int>();
			List<int> indicesless_flat_meshes = new List<int>();
            for (int i = start; i < end; ++i) {
                if (flat_meshes[i] == null) {
                    continue;
                }
                if (flat_meshes[i].gl_vao_id == 0) {
                    vaoless_flat_meshes.Add(i);
                }
                if (flat_meshes[i].gl_vert_id == 0) {
                    vboless_flat_meshes.Add(i);
                }
				if (flat_meshes[i].gl_indices_id == 0) {
					indicesless_flat_meshes.Add(i);
				}
            }

            int[] vao = new int[vaoless_flat_meshes.Count];
            int[] vbo = new int[vboless_flat_meshes.Count];
			int[] indices_vbo = new int[indicesless_flat_meshes.Count];
            GLFunc.CreateVertexArrays(vaoless_flat_meshes.Count, vao);
            GLFunc.CreateBuffers(vboless_flat_meshes.Count, vbo);
			GLFunc.CreateBuffers(indicesless_flat_meshes.Count, indices_vbo);

            // Distribute VAOs
            for (int i = 0; i < vaoless_flat_meshes.Count; ++i) {
                flat_meshes[vaoless_flat_meshes[i]].gl_vao_id = vao[i];
            }

            // Distribute VBOs
            for (int i = 0; i < vboless_flat_meshes.Count; ++i) {
                flat_meshes[vboless_flat_meshes[i]].gl_vert_id = vbo[i];
            }

			// Distribute Indices VBOs
			for (int i = 0; i < indicesless_flat_meshes.Count; ++i) {
				flat_meshes[indicesless_flat_meshes[i]].gl_indices_id = indices_vbo[i];
			}
        }

		internal static void UploadFlatMeshMeshes(List<FlatMesh> flat_meshes, int start, int end) {
			Utilities.AssertValidIndicies(flat_meshes, start, end);

			for (int i = start; i < end; ++i) {
				// Reference to FlatMesh
				FlatMesh fm = flat_meshes[i];

				if (fm == null || fm.uploaded) {
					continue;
				}

				GLFunc.BindVertexArray(fm.gl_vao_id);
				GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, fm.gl_vert_id);

				unsafe {
					GLFunc.BufferData(GL.BufferTarget.ArrayBuffer, fm.vertices.Count * sizeof(Vertex2D), fm.vertices.ToArray(), GL.BufferUsageHint.StaticDraw);

					GLFunc.VertexAttribPointer(0, 2, GL.VertexAttribPointerType.Float, false, sizeof(Vertex2D), 0 * sizeof(float));
					GLFunc.VertexAttribPointer(1, 2, GL.VertexAttribPointerType.Float, false, sizeof(Vertex2D), 2 * sizeof(float));
				}

				GLFunc.EnableVertexAttribArray(0);
				GLFunc.EnableVertexAttribArray(1);

				GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, fm.gl_indices_id);
				GLFunc.BufferData(GL.BufferTarget.ElementArrayBuffer, fm.indices.Count * sizeof(int), fm.indices.ToArray(), GL.BufferUsageHint.StaticDraw);

				GLFunc.BindVertexArray(0);
				GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, 0);
				GLFunc.BindBuffer(GL.BufferTarget.ElementArrayBuffer, 0);

				fm.uploaded = true;
			}
		}
    }
}
