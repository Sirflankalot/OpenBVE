using GL = OpenTK.Graphics.OpenGL;
using System.IO;

namespace LibRender {
	public struct Settings {
		public enum ViewDistance {
			Set
		}

		// TODO: Honor LogFileLocation Setting
		public enum LogFileLocation {
			Set
		}
		
		public enum TextureFiltering {
			None,
			Bilinear,
			Trilinear,
			Anisotropic2,
			Anisotropic4,
			Anisotropic8,
			Anisotropic16
		}
		
		public enum RendererType {
			Forward,
			Deferred
		}
		
		public enum ForwardAntialiasing {
			None = 1,
			MSAA2 = 2,
			MSAA4 = 4,
			MSAA8 = 8,
			SSAA2 = 32,
			SSAA4 = 64,
		}
		
		public enum DeferredAntialiasing {
			None = 1,
			SSAA2 = 2,
			SSAA4 = 4
		}
		
		public enum UIAntialiasing {
			None = 1,
			MSAA2 = 2,
			MSAA4 = 4,
			MSAA8 = 8
		}
		
		public enum TextRenderingQuality {
			Low,
			Medium,
			High,
			Ultra
		}

		public enum Verbosity {
			Level0 = 0,
			Level1 = 1,
			Level2 = 2,
			Level3 = 3
		}

		public enum Wireframe {
			Off = 0,
			On = 1,
			Toggle = 2
		}

		public enum ClearColor {
			Set
		}

		public float view_distance;
		public string log_file_location;
		public TextureFiltering texture_filtering;
		public RendererType renderer_type;
		public ForwardAntialiasing forward_aa;
		public DeferredAntialiasing deferred_aa;
		public UIAntialiasing ui_aa;
		public TextRenderingQuality text_rendering_quality;
		public Verbosity vebosity;
		public Wireframe wireframe;
		public OpenTK.Vector3 clear_color;
	}

	public partial class Renderer {
		internal Settings settings = new Settings {
			view_distance          = 1000.0f,
			log_file_location      = null,
			texture_filtering      = Settings.TextureFiltering.None,
			renderer_type          = Settings.RendererType.Forward,
			forward_aa             = Settings.ForwardAntialiasing.None,
			deferred_aa            = Settings.DeferredAntialiasing.None,
			ui_aa                  = Settings.UIAntialiasing.None,
			text_rendering_quality = Settings.TextRenderingQuality.Low,
			vebosity               = Settings.Verbosity.Level1,
			wireframe              = Settings.Wireframe.Off,
			clear_color            = new OpenTK.Vector3(66, 149, 244) / 255
		};

		public Settings GetSettings() {
			return settings;
		}

		public void SetSetting(Settings set) {
			SetSetting(Settings.ViewDistance.Set, set.view_distance);
			SetSetting(Settings.LogFileLocation.Set, set.log_file_location);
			SetSetting(set.texture_filtering);
			SetSetting(set.renderer_type);
			SetSetting(set.forward_aa);
			SetSetting(set.deferred_aa);
			SetSetting(set.ui_aa);
			SetSetting(set.text_rendering_quality);
			SetSetting(set.vebosity);
			SetSetting(set.wireframe);
		}

		public void SetSetting(Settings.ViewDistance ignore, float distance) {
			if (distance <= 1) {
				throw new System.ArgumentException(distance.ToString() + " is not a valid distance");
			}
			settings.view_distance = distance;
			Algorithms.UpdateCameraMatrices(cameras, 0, cameras.Count, (float) display_width / display_height, settings.view_distance, true);
		}

		public void SetSetting(Settings.LogFileLocation ignore, string location) {
			if (location != null) {
				string dir = Path.GetDirectoryName(location);
				bool exists = Directory.Exists(dir);
				if (!exists) {
					throw new System.ArgumentException(location + " is not a valid path");
				}
				log_file = File.Open(location, FileMode.OpenOrCreate);
				log_file.SetLength(0);
				log_file.Flush();
			}
			settings.log_file_location = location;
		}

		public void SetSetting(Settings.TextureFiltering filtering) {
			settings.texture_filtering = filtering;

			switch (filtering) {
				default:
				case Settings.TextureFiltering.None:
					tex_filt_min = GL.TextureMinFilter.Nearest;
					tex_filt_mag = GL.TextureMagFilter.Nearest;
					break;
				case Settings.TextureFiltering.Bilinear:
					tex_filt_min = GL.TextureMinFilter.Linear;
					tex_filt_mag = GL.TextureMagFilter.Linear;
					break;
				case Settings.TextureFiltering.Trilinear:
					tex_filt_min = GL.TextureMinFilter.LinearMipmapLinear;
					tex_filt_mag = GL.TextureMagFilter.Linear;
					break;
				case Settings.TextureFiltering.Anisotropic2:
					tex_filt_aniso = 2;
					goto case Settings.TextureFiltering.Trilinear;
				case Settings.TextureFiltering.Anisotropic4:
					tex_filt_aniso = 4;
					goto case Settings.TextureFiltering.Trilinear;
				case Settings.TextureFiltering.Anisotropic8:
					tex_filt_aniso = 8;
					goto case Settings.TextureFiltering.Trilinear;
				case Settings.TextureFiltering.Anisotropic16:
					tex_filt_aniso = 16;
					goto case Settings.TextureFiltering.Trilinear;
			}

			GFXInterface.UpdateTextureFiltering(textures, tex_filt_min, tex_filt_mag, tex_filt_aniso);
		}

		public void SetSetting(Settings.RendererType type) {
			settings.renderer_type = type;
			Resize(display_width, display_height);
		}

		public void SetSetting(Settings.ForwardAntialiasing type) {
			settings.forward_aa = type;
		}

		public void SetSetting(Settings.DeferredAntialiasing type) {
			settings.deferred_aa = type;
			Resize(display_width, display_height);
		}

		public void SetSetting(Settings.UIAntialiasing type) {
			settings.ui_aa = type;
		}

		public void SetSetting(Settings.TextRenderingQuality quality) {
			settings.text_rendering_quality = quality;
		}

		public void SetSetting(Settings.Verbosity verbosity) {
			settings.vebosity = verbosity;
		}

		public void SetSetting(Settings.Wireframe wireframe) {
			if (wireframe == Settings.Wireframe.Toggle) {
				settings.wireframe = 1 - settings.wireframe;
			}
			else {
				settings.wireframe = wireframe;
			}
		}

		public void SetSetting(Settings.ClearColor ignore, OpenTK.Vector3 color) {
			settings.clear_color = color;
		}
	}
}
