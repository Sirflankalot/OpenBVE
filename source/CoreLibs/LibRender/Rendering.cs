using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
		internal int gBuffer, lBuffer;
		internal int gNormal, gAlbedoSpec, glDepth;
		internal int lColor;

		internal void InitializeFramebuffers() {
			////////////////////////////
			// Initialize the GBuffer //
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
			GLFunc.GenTextures(1, out glDepth);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, glDepth);
			GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, width, height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, glDepth, 0);
            Error.CheckForOpenGlError("Depth Buffer");

			if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
				throw new System.Exception("gBuffer incomplete");
			}

			////////////////////////////
			// Initialize the LBuffer //
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
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, glDepth);
			GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, glDepth, 0);
            Error.CheckForOpenGlError("Depth Buffer 2");

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

			if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
				throw new System.Exception("lBuffer incomplete");
			}
		}

		internal void DeleteFramebuffers() {
			GLFunc.DeleteTexture(gNormal);
			GLFunc.DeleteTexture(gAlbedoSpec);
			GLFunc.DeleteTexture(glDepth);
			GLFunc.DeleteTexture(lColor);
			GLFunc.DeleteFramebuffer(gBuffer);
			GLFunc.DeleteFramebuffer(lBuffer);
		}
		
		internal ShaderProgram geometry_prog;
		internal ShaderProgram lightpass_prog;
		internal ShaderProgram hdrpass_prog;

		internal void InitializeShaders() {
			var geometry_vertex = new Shader(GL.ShaderType.VertexShader, Shader_Sources.geometry_vs);
			var geometry_fragment = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.geometry_fs);
			geometry_prog = new ShaderProgram(geometry_vertex, geometry_fragment);

			var onscreenquad_vertex = new Shader(GL.ShaderType.VertexShader, Shader_Sources.onscreenquad_vs);
			var light_fragment = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.lightpass_fs);
			lightpass_prog = new ShaderProgram(onscreenquad_vertex, light_fragment);

			var hdr_fragment = new Shader(GL.ShaderType.FragmentShader, Shader_Sources.hdrpass_fs);
			hdrpass_prog = new ShaderProgram(onscreenquad_vertex, hdr_fragment);

			geometry_vertex.Clear();
			geometry_fragment.Clear();
			onscreenquad_vertex.Clear();
			hdr_fragment.Clear();
			light_fragment.Clear();
		}

		internal void DeleteShaders() {
			geometry_prog.Clear();
			lightpass_prog.Clear();
		}

        internal void RenderAllObjects() {
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
                if (o == null || !o.visible || meshes[o.mesh_id] == null || textures[o.tex_id] == null) {
                    continue;
                }

                // Reference to subobjects
                Mesh m = meshes[o.mesh_id];
                Texture t = textures[o.tex_id];
                Camera c = cameras[active_camera];

                GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

                GLFunc.UniformMatrix4(geometry_prog.GetUniform("world_mat"), false, ref o.transform);
                GLFunc.UniformMatrix4(geometry_prog.GetUniform("view_mat"), false, ref c.transform);

                GLFunc.BindVertexArray(m.gl_vao_id);

                GLFunc.DrawElements(GL.BeginMode.Triangles, m.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

                GLFunc.BindVertexArray(0);
            }

			// Render to lightbuffer
			lightpass_prog.Use();

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gNormal);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture1);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gAlbedoSpec);

			GLFunc.Uniform1(lightpass_prog.GetUniform("Normal"), 0);
			GLFunc.Uniform1(lightpass_prog.GetUniform("AlbedoSpec"), 1);

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);

			GLFunc.DrawBuffers(1, attachments);
			GLFunc.DepthFunc(GL.DepthFunction.Greater);

			GLFunc.ClearColor(66f / 255f, 149f / 255f, 244f / 255f, 1.0f);
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

			GLFunc.Enable(GL.EnableCap.DepthTest);
			GLFunc.Enable(GL.EnableCap.Blend);
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