using OpenTK;
using System.Linq;
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
			if (display_width == 0 || display_height == 0) {
				return;
			}

			////////////////////////////
			// Initialize the gBuffer //
			////////////////////////////

            GLFunc.GetError();
			if (settings.renderer_type == Settings.RendererType.Deferred) {
				int offscreen_width = display_width * (int) settings.deferred_aa;
				int offscreen_height = display_height * (int) settings.deferred_aa;

				GLFunc.GenFramebuffers(1, out gBuffer);
				GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, gBuffer);

				// - Normal color buffer
				GLFunc.GenTextures(1, out gNormal);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, gNormal);
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba16f, offscreen_width, offscreen_height, 0, GL.PixelFormat.Rgba, GL.PixelType.Float, new System.IntPtr(0));
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2D, gNormal, 0);
				Error.CheckForOpenGlError("Normal Color Buffer");

				// - Color + Specular buffer
				GLFunc.GenTextures(1, out gAlbedoSpec);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, gAlbedoSpec);
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgba, offscreen_width, offscreen_height, 0, GL.PixelFormat.Rgba, GL.PixelType.UnsignedByte, new System.IntPtr(0));
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment1, GL.TextureTarget.Texture2D, gAlbedoSpec, 0);
				Error.CheckForOpenGlError("AlbedoSpec Color Buffer");

				// - Depth Buffer
				GLFunc.GenTextures(1, out gDepth);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, gDepth);
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, offscreen_width, offscreen_height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
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
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgb16f, offscreen_width, offscreen_height, 0, GL.PixelFormat.Rgb, GL.PixelType.Float, new System.IntPtr(0));
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.LinearMipmapLinear);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Linear);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMaxLevel, (int) settings.deferred_aa / 2);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2D, lColor, 0);
				Error.CheckForOpenGlError("Color Buffer");

				// - Depth Buffer
				GLFunc.GenTextures(1, out lDepth);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, lDepth);
				GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, offscreen_width, offscreen_height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
				GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, lDepth, 0);
				Error.CheckForOpenGlError("Depth Buffer 2");

				if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
					throw new System.Exception("lBuffer incomplete");
				}
			}
			else {
				// Using MSAA or None
				if ((int)settings.forward_aa >> 4 == 0) {
					int msaa_level = (int) settings.forward_aa & 0xF;

					////////////////////////////
					// Initialize the lBuffer //
					////////////////////////////

					GLFunc.GenFramebuffers(1, out lBuffer);
					GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);

					// - Color buffer
					GLFunc.GenTextures(1, out lColor);
					GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, lColor);
					GLFunc.TexImage2DMultisample(GL.TextureTargetMultisample.Texture2DMultisample, msaa_level, GL.PixelInternalFormat.Rgba, display_width, display_height, true);
					GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2DMultisample, lColor, 0);
					Error.CheckForOpenGlError("MS Color Buffer");

					// - Depth Buffer
					GLFunc.GenTextures(1, out lDepth);
					GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, lDepth);
					GLFunc.TexImage2DMultisample(GL.TextureTargetMultisample.Texture2DMultisample, msaa_level, GL.PixelInternalFormat.Depth32fStencil8, display_width, display_height, true);
					GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2DMultisample, lDepth, 0);
					Error.CheckForOpenGlError("MS Depth Buffer 2");

					if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
						throw new System.Exception("lBuffer incomplete");
					}
				}
				// Using SSAA
				else {
					int offscreen_width = display_width * ((int) settings.forward_aa >> 4);
					int offscreen_height = display_height * ((int) settings.forward_aa >> 4);

					////////////////////////////
					// Initialize the lBuffer //
					////////////////////////////

					GLFunc.GenFramebuffers(1, out lBuffer);
					GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);

					// - Color buffer
					GLFunc.GenTextures(1, out lColor);
					GLFunc.BindTexture(GL.TextureTarget.Texture2D, lColor);
					GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Rgb16f, offscreen_width, offscreen_height, 0, GL.PixelFormat.Rgb, GL.PixelType.Float, new System.IntPtr(0));
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.LinearMipmapLinear);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Linear);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMaxLevel, (int) settings.deferred_aa / 2);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
					GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.ColorAttachment0, GL.TextureTarget.Texture2D, lColor, 0);
					Error.CheckForOpenGlError("Color Buffer");

					// - Depth Buffer
					GLFunc.GenTextures(1, out lDepth);
					GLFunc.BindTexture(GL.TextureTarget.Texture2D, lDepth);
					GLFunc.TexImage2D(GL.TextureTarget.Texture2D, 0, GL.PixelInternalFormat.Depth32fStencil8, offscreen_width, offscreen_height, 0, GL.PixelFormat.DepthStencil, GL.PixelType.Float32UnsignedInt248Rev, new System.IntPtr(0));
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, (int) GL.TextureMinFilter.Nearest);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, (int) GL.TextureMagFilter.Nearest);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapS, (int) GL.TextureWrapMode.ClampToEdge);
					GLFunc.TexParameter(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureWrapT, (int) GL.TextureWrapMode.ClampToEdge);
					GLFunc.FramebufferTexture2D(GL.FramebufferTarget.Framebuffer, GL.FramebufferAttachment.DepthStencilAttachment, GL.TextureTarget.Texture2D, lDepth, 0);
					Error.CheckForOpenGlError("Depth Buffer 2");

					if (GLFunc.CheckFramebufferStatus(GL.FramebufferTarget.Framebuffer) != GL.FramebufferErrorCode.FramebufferComplete) {
						throw new System.Exception("lBuffer incomplete");
					}
				}
			}

			/////////////////////////////
			// Initialize the uiBuffer //
			/////////////////////////////
			
			GLFunc.GenFramebuffers(1, out uiBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, uiBuffer);

			// Color Buffer
			GLFunc.GenTextures(1, out uiColor);
			GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, uiColor);
			GLFunc.TexImage2DMultisample(GL.TextureTargetMultisample.Texture2DMultisample, (int)settings.ui_aa, GL.PixelInternalFormat.Rgba, display_width, display_height, true);
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
		
		internal ShaderProgram deferred_geometry_prog;
		internal ShaderProgram forward_geometry_prog;
		internal ShaderProgram forward_geometry_unshaded_prog;
		internal ShaderProgram lightpass_prog;
		internal ShaderProgram hdrpass_prog;
		internal ShaderProgram text_prog;
		internal ShaderProgram textcopy_prog;
		internal ShaderProgram multsample_hdrpass_prog;
		internal ShaderProgram ui_prog;

		internal void InitializeShaders() {
			var deferred_geometry_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.deferred_geometry_vs);
			var deferred_geometry_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.deferred_geometry_fs);
			deferred_geometry_prog = new ShaderProgram(deferred_geometry_vertex, deferred_geometry_fragment);

			var forward_geometry_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.forward_geometry_vs);
			var forward_geometry_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.forward_geometry_fs);
			var forward_geometry_unshaded_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.forward_geometry_unshaded_fs);
			forward_geometry_prog = new ShaderProgram(forward_geometry_vertex, forward_geometry_fragment);
			forward_geometry_unshaded_prog = new ShaderProgram(forward_geometry_vertex, forward_geometry_unshaded_fragment);

			var onscreenquad_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.onscreenquad_vs);
			var light_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.lightpass_fs);
			lightpass_prog = new ShaderProgram(onscreenquad_vertex, light_fragment);

			var hdr_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.hdrpass_fs);
			hdrpass_prog = new ShaderProgram(onscreenquad_vertex, hdr_fragment);

			var ms_hdr_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.multisample_hdrpass_fs);
			multsample_hdrpass_prog = new ShaderProgram(onscreenquad_vertex, ms_hdr_fragment);

			var text_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.text_vs);
			var text_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.text_fs);
			text_prog = new ShaderProgram(text_vertex, text_fragment);

			var textcopy_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.textcopy_fs);
			textcopy_prog = new ShaderProgram(onscreenquad_vertex, textcopy_fragment);

			var ui_vertex = new Shader(GL.ShaderType.VertexShader, ShaderSources.uielement_vs);
			var ui_fragment = new Shader(GL.ShaderType.FragmentShader, ShaderSources.uielement_fs);
			ui_prog = new ShaderProgram(ui_vertex, ui_fragment);

			deferred_geometry_vertex.Clear();
			deferred_geometry_fragment.Clear();
			forward_geometry_unshaded_fragment.Clear();
			onscreenquad_vertex.Clear();
			hdr_fragment.Clear();
			ms_hdr_fragment.Clear();
			light_fragment.Clear();
			text_vertex.Clear();
			text_fragment.Clear();
			textcopy_fragment.Clear();
			ui_vertex.Clear();
			ui_fragment.Clear();
		}

		internal void DeleteShaders() {
			deferred_geometry_prog.Clear();
			forward_geometry_prog.Clear();
			forward_geometry_unshaded_prog.Clear();
			lightpass_prog.Clear();
			hdrpass_prog.Clear();
			multsample_hdrpass_prog.Clear();
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
			if (display_width == 0 || display_height == 0) {
				return;
			}

            // Create timers etc.

            int[] timers = new int[6];
            GLFunc.CreateQueries(GL.QueryTarget.TimeElapsed, 6, timers);

            int[] primitive_counters = new int[3];
            GLFunc.CreateQueries(GL.QueryTarget.PrimitivesGenerated, 3, primitive_counters);

			GLFunc.Disable(GL.EnableCap.Blend);
			GLFunc.CullFace(GL.CullFaceMode.Back);
			GLFunc.Enable(GL.EnableCap.CullFace);
			GLFunc.FrontFace(GL.FrontFaceDirection.Ccw);
		
			int internal_width;
			int internal_height;

			if (settings.renderer_type == Settings.RendererType.Deferred) {
				internal_width = display_width * (int) settings.deferred_aa;
				internal_height = display_height * (int) settings.deferred_aa;
			}
			else {
				if ((int) settings.forward_aa >> 4 != 0) {
					internal_width = display_width * ((int) settings.forward_aa >> 4);
					internal_height = display_height * ((int) settings.forward_aa >> 4);
				}
				else {
					internal_width = display_width;
					internal_height = display_height;
				}
			}

			GLFunc.Viewport(0, 0, internal_width, internal_height);
			GLFunc.Enable(GL.EnableCap.DepthTest);
			GLFunc.DepthRange(0, 1);
			GLFunc.DepthMask(true);

			var sun_mat = new Matrix3(cameras[active_camera].view_matrix);
			sun_mat.Transpose();
			var sun_vec = sun_mat * sun.direction;
			sun_vec.Normalize();

			if (settings.renderer_type == Settings.RendererType.Deferred) {
				RenderAllDeferred(timers, primitive_counters[0], internal_width, internal_height, sun_vec);
			} 

			// Render the transparent parts of the transparent objects

			// Sort all transparent objects by distance from the camera
			GLFunc.Enable(GL.EnableCap.Blend);
			List<Object> transparent;
			if (settings.renderer_type == Settings.RendererType.Deferred) {
				transparent = objects.Where((o) => o != null ? textures[AssertValid(o.texture)].has_transparancy : false)
									 .OrderByDescending((o) => (o.position - cameras[active_camera].position).LengthSquared)
									 .ToList();
			}
			else {
				GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, lBuffer);
				GLFunc.ClearColor(new OpenTK.Graphics.Color4((float) System.Math.Pow(settings.clear_color.X, 2.2), (float)System.Math.Pow(settings.clear_color.Y, 2.2), (float)System.Math.Pow(settings.clear_color.Z, 2.2), 1.0f));
				GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);

				transparent = objects.Where((o) => o != null)
									 .OrderByDescending((o) => (o.shaded))
									 .ThenByDescending((o) => (o.position - cameras[active_camera].position).LengthSquared)
									 .ToList();

                statistics.val_objects.rendered = transparent.Count;
            }

            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[2]);
            GLFunc.BeginQuery(GL.QueryTarget.PrimitivesGenerated, primitive_counters[1]);

			forward_geometry_unshaded_prog.Use();

			GLFunc.Uniform1(forward_geometry_unshaded_prog.GetUniform("model_tex"), 0);
			GLFunc.UniformMatrix4(forward_geometry_unshaded_prog.GetUniform("proj_mat"), false, ref cameras[active_camera].proj_matrix);
			
			forward_geometry_prog.Use();

			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Line);
				GLFunc.LineWidth(2.0f);
			}

			GLFunc.Uniform1(forward_geometry_prog.GetUniform("model_tex"), 0);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);

			GLFunc.UniformMatrix4(forward_geometry_prog.GetUniform("proj_mat"), false, ref cameras[active_camera].proj_matrix);
			GLFunc.Uniform3(forward_geometry_prog.GetUniform("sunDir", true), sun_vec);
			GLFunc.Uniform3(forward_geometry_prog.GetUniform("suncolor"), sun.color);
			GLFunc.Uniform1(forward_geometry_prog.GetUniform("sunbrightness"), sun.brightness);

			bool currently_shaded = true;
			foreach (Object o in transparent) {
				// Reference to subobjects
				Mesh m = meshes[AssertValid(o.mesh)];
				Texture t = textures[AssertValid(o.texture)];
				Camera c = cameras[active_camera];

				if (o == null || !o.visible || m == null || t == null) {
					continue;
				}

				if (o.shaded && !currently_shaded) {
					forward_geometry_prog.Use();
				}
				else if (!o.shaded && currently_shaded) {
					forward_geometry_unshaded_prog.Use();
				}
				currently_shaded = o.shaded;

				ShaderProgram current_shader = o.shaded ? forward_geometry_prog : forward_geometry_unshaded_prog;

				GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

				GLFunc.UniformMatrix4(current_shader.GetUniform("world_mat"), false, ref o.transform);
				GLFunc.UniformMatrix4(current_shader.GetUniform("view_mat"), false, ref c.view_matrix);
				GLFunc.UniformMatrix3(current_shader.GetUniform("normal_mat"), false, ref o.inverse_model_view_matrix);

				GLFunc.BindVertexArray(m.gl_vao_id);

				GLFunc.DrawElements(GL.BeginMode.Triangles, m.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

				GLFunc.BindVertexArray(0);
			}

			GLFunc.Disable(GL.EnableCap.Blend);
			GLFunc.DepthMask(true);
			
			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Fill);
			}

			GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);
            GLFunc.EndQuery(GL.QueryTarget.PrimitivesGenerated);

            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[3]);

			GLFunc.Viewport(0, 0, display_width, display_height);
			// Resolve a SSAA buffer (Deferred SSAA or Forward SSAA)
			if (settings.renderer_type == Settings.RendererType.Deferred || (int) settings.forward_aa >> 4 != 0) {
				// Render to Backbuffer
				hdrpass_prog.Use();

				GLFunc.Disable(GL.EnableCap.DepthTest);
				GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

				GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
				GLFunc.BindTexture(GL.TextureTarget.Texture2D, lColor);
				GLFunc.GenerateMipmap(GL.GenerateMipmapTarget.Texture2D);

				GLFunc.Uniform1(hdrpass_prog.GetUniform("inTexture"), 0);
				GLFunc.Uniform1(hdrpass_prog.GetUniform("exposure"), 1.0f);

				GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);
				RenderFullscreenQuad();
			}
			// Resolve a MSAA buffer (Forward MSAA)
			else {
				int msaa_level = (int) settings.forward_aa & 0xF;

				// Render to Backbuffer
				multsample_hdrpass_prog.Use();

				GLFunc.Disable(GL.EnableCap.DepthTest);
				GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

				GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
				GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, lColor);
				
				GLFunc.Uniform1(multsample_hdrpass_prog.GetUniform("ms_count"), msaa_level);
				GLFunc.Uniform1(multsample_hdrpass_prog.GetUniform("texture"), 0);
				GLFunc.Uniform1(multsample_hdrpass_prog.GetUniform("exposure"), 1.0f);

				GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);
				RenderFullscreenQuad();
			}

			GLFunc.Enable(GL.EnableCap.Blend);
			GLFunc.Disable(GL.EnableCap.CullFace);

            GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);
            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[4]);
            GLFunc.BeginQuery(GL.QueryTarget.PrimitivesGenerated, primitive_counters[2]);

			List<UIInfo> uiinfos = new List<UIInfo>();
			for (int i = 0; i < texts.Count; ++i) {
				Text t = texts[i];

				if (t == null || !t.visible) {
					continue;
				}
				uiinfos.Add(new UIInfo() { type = UIInfo.Type.text, index = i, depth = t.depth });
			}
            int text_count = uiinfos.Count;
			for (int i = 0; i < uielements.Count; ++i) {
				UIElement uie = uielements[i];

				if (uie == null || !uie.visible || textures[AssertValid(uie.texture)] == null || flat_meshes[AssertValid(uie.flatmesh)] == null) {
					continue;
				}
				uiinfos.Add(new UIInfo() { type = UIInfo.Type.uielement, index = i, depth = uie.depth });
			}
            int uie_count = uiinfos.Count - text_count;
            statistics.val_texts.rendered = text_count;
            statistics.val_uielements.rendered = uie_count;

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

			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Line);
				GLFunc.LineWidth(2.0f);
			}

			GLFunc.Uniform1(ui_prog.GetUniform("uiTexture"), 0);
			GLFunc.Uniform1(ui_prog.GetUniform("ratio"), (float) display_width / display_height);
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
					Texture t = textures[AssertValid(uie.texture)];
					FlatMesh fm = flat_meshes[AssertValid(uie.flatmesh)];

					GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

					GLFunc.Uniform1(multsample_hdrpass_prog.GetUniform("ms_count"), (int) settings.ui_aa);
					GLFunc.UniformMatrix2(ui_prog.GetUniform("rotation"), false, ref uie.transform);
					GLFunc.Uniform2(ui_prog.GetUniform("scale"), new Vector2(2 * uie.scale.X * t.width / display_width, 2 * uie.scale.Y * t.height / display_height));
					var adjusted_location = uie.location.Translated(WindowOrigin.TopLeft, ObjectOrigin.TopLeft, display_width, display_height, uie.scale * new Vector2(t.width, t.height));
					GLFunc.Uniform2(ui_prog.GetUniform("translate"), new Vector2(((2 * adjusted_location.position.X) + t.width * uie.scale.X) / display_width, -((2 * adjusted_location.position.Y) + t.height * uie.scale.Y) / display_height));

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

					var adjusted_location = t.location.Translated(WindowOrigin.TopLeft, ObjectOrigin.TopLeft, display_width, display_height, new Vector2(t.width, t.height));
					GLFunc.Uniform2(text_prog.GetUniform("origin"), new Vector2(adjusted_location.position.X / display_width, (display_height - adjusted_location.position.Y - t.height) / display_height));
					GLFunc.Uniform2(text_prog.GetUniform("size"), new Vector2((float) t.width / display_width, (float) t.height / display_height));

					GLFunc.Uniform4(text_prog.GetUniform("color"), t.color);

					RenderFullscreenQuad();
				}
			}
			
			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Fill);
			}

			GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);
            GLFunc.EndQuery(GL.QueryTarget.PrimitivesGenerated);
            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[5]);

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, 0);

			// Copy text from multisampled buffer to regular buffer
			textcopy_prog.Use();

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2DMultisample, uiColor);

			GLFunc.Uniform1(textcopy_prog.GetUniform("texture"), 0);

			RenderFullscreenQuad();

			GLFunc.Enable(GL.EnableCap.DepthTest);
			GLFunc.Enable(GL.EnableCap.CullFace);

            GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);

            // Timing Structures

            int geometry_pass_time = 0;
            int lighting_pass_time = 0;
            int transparent_pass_time = 0;
            int hdr_pass_time = 0;
            int text_pass_time = 0;
            int textcopy_pass_time = 0;

            if (settings.renderer_type == Settings.RendererType.Deferred) {
                GLFunc.GetQueryObject(timers[0], GL.GetQueryObjectParam.QueryResult, out geometry_pass_time);
                GLFunc.GetQueryObject(timers[1], GL.GetQueryObjectParam.QueryResult, out lighting_pass_time);
            }
            GLFunc.GetQueryObject(timers[2], GL.GetQueryObjectParam.QueryResult, out transparent_pass_time);
            GLFunc.GetQueryObject(timers[3], GL.GetQueryObjectParam.QueryResult, out hdr_pass_time);
            GLFunc.GetQueryObject(timers[4], GL.GetQueryObjectParam.QueryResult, out text_pass_time);
            GLFunc.GetQueryObject(timers[5], GL.GetQueryObjectParam.QueryResult, out textcopy_pass_time);

            statistics.val_subframe_time.geometry_pass = geometry_pass_time / 1000000f;
            statistics.val_subframe_time.lighting_pass = lighting_pass_time / 1000000f;
            statistics.val_subframe_time.transparent_pass = transparent_pass_time / 1000000f;
            statistics.val_subframe_time.hdr_pass = hdr_pass_time / 1000000f;
            statistics.val_subframe_time.text_pass = text_pass_time / 1000000f;
            statistics.val_subframe_time.textcopy_pass = textcopy_pass_time / 1000000f;

            // Count primitives drawn

            int deferred_primitives = 0;
            int forward_primitives = 0;
            int ui_primitives = 0;

            GLFunc.GetQueryObject(primitive_counters[0], GL.GetQueryObjectParam.QueryResult, out deferred_primitives);
            GLFunc.GetQueryObject(primitive_counters[1], GL.GetQueryObjectParam.QueryResult, out forward_primitives);
            GLFunc.GetQueryObject(primitive_counters[2], GL.GetQueryObjectParam.QueryResult, out ui_primitives);

            statistics.val_primitives_drawn.deferred = deferred_primitives;
            statistics.val_primitives_drawn.forward = forward_primitives;
            statistics.val_primitives_drawn.ui = ui_primitives;
        }

        private void RenderAllDeferred(int[] timers, int primitive_counter, int internal_width, int internal_height, Vector3 sun_vec) {
            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[0]);
            GLFunc.BeginQuery(GL.QueryTarget.PrimitivesGenerated, primitive_counter);

            deferred_geometry_prog.Use();

			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Line);
				GLFunc.LineWidth(2.0f);
			}

			GLFunc.Uniform1(deferred_geometry_prog.GetUniform("tex"), 0);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);

			GLFunc.UniformMatrix4(deferred_geometry_prog.GetUniform("viewproj_mat"), false, ref cameras[active_camera].transform_matrix);

			GLFunc.BindFramebuffer(GL.FramebufferTarget.Framebuffer, gBuffer);

			// Tell OpenGL which color attachments we'll use
			GL.DrawBuffersEnum[] attachments = new GL.DrawBuffersEnum[] { GL.DrawBuffersEnum.ColorAttachment0, GL.DrawBuffersEnum.ColorAttachment1 };
			GLFunc.DrawBuffers(2, attachments);

			GLFunc.ClearColor(0.5f, 0.5f, 0.5f, 1);
			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit | GL.ClearBufferMask.StencilBufferBit);

			List<Object> sorted_objects =
						   objects.Where((o) => o != null)
								  .OrderBy((o) => (o.position - cameras[active_camera].position).LengthSquared)
								  .ToList();

            statistics.val_objects.rendered = sorted_objects.Count;

			foreach (Object o in sorted_objects) {
				// Reference to subobjects
				Mesh m = meshes[AssertValid(o.mesh)];
				Texture t = textures[AssertValid(o.texture)];
				Camera c = cameras[active_camera];

				if (o == null || !o.visible || !o.shaded || m == null || t == null) {
					continue;
				}

				GLFunc.BindTexture(GL.TextureTarget.Texture2D, t.gl_id);

				GLFunc.UniformMatrix4(deferred_geometry_prog.GetUniform("world_mat"), false, ref o.transform);
				GLFunc.UniformMatrix3(deferred_geometry_prog.GetUniform("normal_mat"), false, ref o.inverse_model_view_matrix);

				GLFunc.BindVertexArray(m.gl_vao_id);

				GLFunc.DrawElements(GL.BeginMode.Triangles, m.indices.Count, GL.DrawElementsType.UnsignedInt, 0);

				GLFunc.BindVertexArray(0);
			}

			if (settings.wireframe == Settings.Wireframe.On) {
				GLFunc.PolygonMode(GL.MaterialFace.FrontAndBack, GL.PolygonMode.Fill);
			}

			GLFunc.BindFramebuffer(GL.FramebufferTarget.ReadFramebuffer, gBuffer);
			GLFunc.BindFramebuffer(GL.FramebufferTarget.DrawFramebuffer, lBuffer);

			GLFunc.BlitFramebuffer(0, 0, internal_width, internal_height, 0, 0, internal_width, internal_height, GL.ClearBufferMask.DepthBufferBit, GL.BlitFramebufferFilter.Nearest);

            GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);
            GLFunc.EndQuery(GL.QueryTarget.PrimitivesGenerated);
            GLFunc.BeginQuery(GL.QueryTarget.TimeElapsed, timers[1]);

			// Render to lightbuffer
			lightpass_prog.Use();

			GLFunc.ActiveTexture(GL.TextureUnit.Texture0);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gNormal);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture1);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gAlbedoSpec);
			GLFunc.ActiveTexture(GL.TextureUnit.Texture2);
			GLFunc.BindTexture(GL.TextureTarget.Texture2D, gDepth);

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
			GLFunc.DepthMask(false);

			GLFunc.ClearColor(new OpenTK.Graphics.Color4((float)System.Math.Pow(settings.clear_color.X, 2.2), (float)System.Math.Pow(settings.clear_color.Y, 2.2), (float)System.Math.Pow(settings.clear_color.Z, 2.2), 1.0f));
			GLFunc.Clear(GL.ClearBufferMask.ColorBufferBit);
			RenderFullscreenQuad();

			GLFunc.DepthFunc(GL.DepthFunction.Less);
            GLFunc.EndQuery(GL.QueryTarget.TimeElapsed);

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