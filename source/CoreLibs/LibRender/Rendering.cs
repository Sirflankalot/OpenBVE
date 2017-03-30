using OpenTK;
using System.Collections.Generic;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
	public partial class Renderer {
		internal int gBuffer, lBuffer;
		internal int gNormal, gAlbedoSpec, gDepth;
		internal int lColor, lDepth;
		internal int uiBuffer, uiColor;

		internal void InitializeFramebuffers() {
			if (width == 0 || height == 0) {
				return;
			}

			////////////////////////////
			// Initialize the gBuffer //
			////////////////////////////

            GLFunc.GetError();
			GLFunc.GenFramebuffers(1, out gBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, gBuffer);

			// - Normal color buffer
			GLFunc.GenTextures(1, out gNormal);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gNormal);
			GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba16f, width, height, 0, GL.PixelFormat.Rgba, GL.PixelType.Float, new System.IntPtr(0));
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
            GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2D, gNormal, 0);
            Error.CheckForOpenGlError("Normal Color Buffer");

			// - Color + Specular buffer
			GLFunc.GenTextures(1, out gAlbedoSpec);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gAlbedoSpec);
			GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba, width, height, 0, GL.PixelFormat.Rgba, GL.PixelType.UnsignedByte, new System.IntPtr(0));
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment1, GL.TextureTarget.Texture2D, gAlbedoSpec, 0);
            Error.CheckForOpenGlError("AlbedoSpec Color Buffer");

			// - Depth Buffer
			GLFunc.GenTextures(1, out gDepth);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gDepth);
			GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, width, height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, gDepth, 0);
            Error.CheckForOpenGlError("Depth Buffer");

			if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
				throw new System.Exception("gBuffer incomplete");
			}

			////////////////////////////
			// Initialize the lBuffer //
			////////////////////////////

			GLFunc.GenFramebuffers(1, out lBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);

			// - Color buffer
			GLFunc.GenTextures(1, out lColor);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, lColor);
			GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgb16f, width, height, 0, GL.PixelFormat.Rgb, GL.PixelType.Float, new System.IntPtr(0));
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2D, lColor, 0);
            Error.CheckForOpenGlError("Color Buffer");

            // - Depth Buffer
            GLFunc.GenTextures(1, out lDepth);
            GLFunc.BindTexture(GL.TextureTarget.Texture2D, lDepth);
            GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, width, height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
            GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
            GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
            GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
            GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
            GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, lDepth, 0);
            Error.CheckForOpenGlError("Depth Buffer 2");

			if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
				throw new System.Exception("lBuffer incomplete");
			}

			/////////////////////////////
			// Initialize the uiBuffer //
			/////////////////////////////
			
			GLFunc.GenFramebuffers(1, out uiBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, uiBuffer);

			// Color Buffer
			GLFunc.GenTextures(1, out uiColor);
			GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, uiColor);
			GLFunc.TexImage2DMultisample(GL.TextureTargetMultisample.Texture2DMultisample, 4, GL.PixelInternalFormat.Rgba, width, height, true);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2DMultisample, uiColor, 0);
			Error.CheckForOpenGlError("Multisampled Color Buffer");

			if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
				throw new System.Exception("lBuffer incomplete");
			}

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);
		}

		internal void DeleteFramebuffers() {
			GLFunc.DeleteTexture(gNormal);
			GLFunc.DeleteTexture(gAlbedoSpec);
			GLFunc.DeleteTexture(gDepth);
			GLFunc.DeleteTexture(lColor);
            GLFunc.DeleteTexture(lDepth);
			GLFunc.DeleteTexture(uiColor);
			GLFunc.DeleteFramebuffer(gBuffer);
			GLFunc.DeleteFramebuffer(lBuffer);
			GLFunc.DeleteFramebuffer(uiColor);
		}
		
		internal ShaderProgram geometry_prog;
		internal ShaderProgram lightpass_prog;
		internal ShaderProgram hdrpass_prog;
		internal ShaderProgram text_prog;
		internal ShaderProgram textcopy_prog;
		internal ShaderProgram ui_prog;

		internal void InitializeShaders() {
			var geometry_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.geometry_vs);
			var geometry_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.geometry_fs);
			geometry_prog = new ShaderProgram(geometry_vertex, geometry_fragment);

			var onscreenquad_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.onscreenquad_vs);
			var light_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.lightpass_fs);
			lightpass_prog = new ShaderProgram(onscreenquad_vertex, light_fragment);

			var hdr_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.hdrpass_fs);
			hdrpass_prog = new ShaderProgram(onscreenquad_vertex, hdr_fragment);

			var text_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.text_vs);
			var text_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.text_fs);
			text_prog = new ShaderProgram(text_vertex, text_fragment);

			var textcopy_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.textcopy_fs);
			textcopy_prog = new ShaderProgram(onscreenquad_vertex, textcopy_fragment);

			var ui_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.uielement_vs);
			var ui_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.uielement_fs);
			ui_prog = new ShaderProgram(ui_vertex, ui_fragment);

			geometry_vertex.Clear();
			geometry_fragment.Clear();
			onscreenquad_vertex.Clear();
			hdr_fragment.Clear();
			light_fragment.Clear();
			text_vertex.Clear();
			text_fragment.Clear();
			textcopy_fragment.Clear();
			ui_vertex.Clear();
			ui_fragment.Clear();
		}

		internal void DeleteShaders() {
			geometry_prog.Clear();
			lightpass_prog.Clear();
			hdrpass_prog.Clear();
			text_prog.Clear();
			textcopy_prog.Clear();
			ui_prog.Clear();
		}

		private struct UIInfo {
			internal enum Type {
				text,
				uielement
			};

			internal Type type;
			internal int index;
			internal int depth;
		}

		internal void RenderAllObjects() {
			if (width == 0 || height == 0) {
				return;
			}

			GLFunc.Disable(GL.EnableCap.Blend);
			GLFunc.CullFace(GL.CullFaceMode.Back);
			GLFunc.Enable(GL.EnableCap.CullFace);
			GLFunc.FrontFace(GL.FrontFaceDirection.Ccw);

			GLFunc.Enable(GL.EnableCap.DepthTest);

			geometry_prog.Use();

			GLFunc.Uniform1(geometry_prog.GetUniform("tex"), 0);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, gBuffer);

			// Tell OpenGL which color attachments we'll use
			GL.DrawBuffersEnum[] attachments = new GL.DrawBuffersEnum[] { GL.DrawBuffersEnum.ColorAttachment0, GL.DrawBuffersEnum.ColorAttachment1 };
			GLFunc.DrawBuffers(2, attachments);

			GLFunc.ClearColor(0.5f, 0.5f, 0.5f, 1);
			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);

			foreach (Object o in objects) {
				// Reference to subobjects
				Mesh m = meshes[o.mesh_id];
				Texture t = textures[o.tex_id];
				Camera c = cameras[active_camera];

				if (o == null || !o.visible || m == null || t == null) {
					continue;
				}

				GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

				GLFunc.UniformMatrix4(geometry_prog.GetUniform("world_mat"), false, ref o.transform);
				GLFunc.UniformMatrix4(geometry_prog.GetUniform("view_mat"), false, ref c.transform_matrix);
				GLFunc.UniformMatrix3(geometry_prog.GetUniform("normal_mat"), false, ref o.inverse_model_view_matrix);

				GLFunc.BindVertexArray(m.gl_vao_id);

				GLFunc.DrawElements(GL.BeginMode.Triangles, m.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

				GLFunc.BindVertexArray(0);
			}

			GLFunc.BindFramebuffer(GL.FramebufferTarget.ReadFramebuffer, gBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.DrawFramebuffer, lBuffer);

			GLFunc.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, GL.ClearBufferMask.DepthBufferBit, GL.BlitFramebufferFilter.Nearest);

			// Render to lightbuffer
			lightpass_prog.Use();

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gNormal);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture1);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gAlbedoSpec);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture2);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gDepth);

			var sun_mat = new Matrix3(cameras[active_camera].view_matrix);
			sun_mat.Transpose();
			var sun_vec = sun_mat * sun.direction;
			sun_vec.Normalize();

			GLFunc.Uniform1(lightpass_prog.GetUniform("Normal"), 0);
			GLFunc.Uniform1(lightpass_prog.GetUniform("AlbedoSpec"), 1);
			GLFunc.Uniform1(lightpass_prog.GetUniform("Depth"), 2);
			GLFunc.Uniform3(lightpass_prog.GetUniform("sunDir", true), sun_vec);
			GLFunc.Uniform3(lightpass_prog.GetUniform("suncolor"), sun.color);
			GLFunc.Uniform1(lightpass_prog.GetUniform("sunbrightness"), sun.brightness);
			GLFunc.UniformMatrix4(lightpass_prog.GetUniform("projInverseMatrix"), false, ref cameras[active_camera].inverse_projection_matrix);

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);

			GLFunc.DrawBuffers(1, attachments);
			GLFunc.DepthFunc(GL.DepthFunction.Greater);

			GLFunc.ClearColor(0.05112f, 0.3066f, 0.9075f, 1.0f); // SRGB 66, 149, 244, 255
			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit);
			RenderFullscreenQuad();

			GLFunc.DepthFunc(GL.DepthFunction.Less);

			// Render to Backbuffer
			hdrpass_prog.Use();

			GLFunc.Disable(GL.EnableCap.DepthTest);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, lColor);

			GLFunc.Uniform1(hdrpass_prog.GetUniform("inTexture"), 0);
			GLFunc.Uniform1(hdrpass_prog.GetUniform("exposure"), 1.0f);

			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);
			RenderFullscreenQuad();

			GLFunc.Enable(GL.EnableCap.Blend);
			GLFunc.Disable(GL.EnableCap.CullFace);

			List<UIInfo> uiinfos = new List<UIInfo>();
			for (int i = 0; i < texts.Count; ++i) {
				Text t = texts[i];

				if (t == null || !t.visible) {
					continue;
				}
				uiinfos.Add(new UIInfo() { type = UIInfo.Type.text, index = i, depth = t.depth });
			}
			for (int i = 0; i < uielements.Count; ++i) {
				UIElement uie = uielements[i];

				if (uie == null || !uie.visible || textures[uie.tex_id] == null || flat_meshes[uie.flatmesh_id] == null) {
					continue;
				}
				uiinfos.Add(new UIInfo() { type = UIInfo.Type.uielement, index = i, depth = uie.depth });
			}

			uiinfos.Sort((lhs, rhs) => {
				if (lhs.depth < rhs.depth) {
					return -1;
				}
				if (lhs.depth > rhs.depth) {
					return 1;
				}
				if (lhs.depth == rhs.depth) {
					if (lhs.type == UIInfo.Type.uielement && rhs.type == UIInfo.Type.text) {
						return -1;
					}
					if (lhs.type == UIInfo.Type.text && rhs.type == UIInfo.Type.uielement) {
						return 1;
					}
				}
				return 0;
			});

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, uiBuffer);
			GLFunc.ClearColor(0, 0, 0, 0);
			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit);

			// Render UI and Text on screen
			text_prog.Use();

			GLFunc.Uniform1(text_prog.GetUniform("textTexture"), 0);

			ui_prog.Use();

			GLFunc.Uniform1(ui_prog.GetUniform("uiTexture"), 0);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);

			UIInfo.Type last_shader = UIInfo.Type.uielement;
			foreach (UIInfo info in uiinfos) {
				if (info.type == UIInfo.Type.uielement) {
					if (last_shader != UIInfo.Type.uielement) {
						ui_prog.Use();
						last_shader = UIInfo.Type.uielement;
					}

					UIElement uie = uielements[info.index];

					// References to subobjects
					Texture t = textures[uie.tex_id];
					FlatMesh fm = flat_meshes[uie.flatmesh_id];

					GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

					GLFunc.UniformMatrix2(ui_prog.GetUniform("rotation"), false, ref uie.transform);
					GLFunc.Uniform2(ui_prog.GetUniform("scale"), new Vector2(uie.scale.X * t.width / width, uie.scale.Y * t.height / height));
					GLFunc.Uniform2(ui_prog.GetUniform("translate"), new Vector2(((2 * uie.location.X) + t.width * uie.scale.X) / width, -((2 * uie.location.Y) + t.height * uie.scale.Y) / height));

					GLFunc.BindVertexArray(fm.gl_vao_id);

					GLFunc.DrawElements(GL.BeginMode.Triangles, fm.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

					GLFunc.BindVertexArray(0);
				}
				else if (info.type == UIInfo.Type.text) {
					if (last_shader != UIInfo.Type.text) {
						text_prog.Use();
						last_shader = UIInfo.Type.text;
					}

					Text t = texts[info.index];

					GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
					GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_tex_id);

					GLFunc.Uniform2(text_prog.GetUniform("origin"), new Vector2(t.origin.X / width, (height - t.origin.Y - t.height) / height));
					GLFunc.Uniform2(text_prog.GetUniform("size"), new Vector2((float) t.width / width, (float) t.height / height));

					GLFunc.Uniform4(text_prog.GetUniform("color"), t.color);

					RenderFullscreenQuad();
				}
			}

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

			// Copy text from multisampled buffer to regular buffer
			textcopy_prog.Use();

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, uiColor);

			GLFunc.Uniform1(textcopy_prog.GetUniform("texture"), 0);

			RenderFullscreenQuad();

			GLFunc.Enable(GL.EnableCap.DepthTest);
			GLFunc.Enable(GL.EnableCap.CullFace);
		}

		private int fullscreen_quad_vao = 0;
		private int fullscreen_quad_vbo;
		private void RenderFullscreenQuad() {
			if (fullscreen_quad_vao == 0) {
				float[] quadVertices = new float[] {
					// Position           // Texcoords
					-1.0f,  1.0f,  1.0f,  0.0f,  1.0f,
					-1.0f, -1.0f,  1.0f,  0.0f,  0.0f,
					 1.0f,  1.0f,  1.0f,  1.0f,  1.0f,
					 1.0f, -1.0f,  1.0f,  1.0f,  0.0f,
				};
				GLFunc.GenVertexArrays(1, out fullscreen_quad_vao);
				GLFunc.GenBuffers(1, out fullscreen_quad_vbo);
				GLFunc.BindVertexArray(fullscreen_quad_vao);
				GLFunc.BindBuffer(GL.BufferTarget.ArrayBuffer, fullscreen_quad_vbo);
				GLFunc.BufferData(GL.BufferTarget.ArrayBuffer, 4 * 5 * sizeof(float), quadVertices, GL.BufferUsageHint.StaticDraw);
				GLFunc.EnableVertexAttribArray(0);
				GLFunc.VertexAttribPointer(0, 3, GL.VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
				GLFunc.EnableVertexAttribArray(1);
				GLFunc.VertexAttribPointer(1, 2, GL.VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
			}
			GLFunc.BindVertexArray(fullscreen_quad_vao);
			GLFunc.DrawArrays(GL.PrimitiveType.TriangleStrip, 0, 4);
			GLFunc.BindVertexArray(0);
		}
    }
}