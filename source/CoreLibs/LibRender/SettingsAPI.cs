using System.IO;

namespace LibRender {
	public struct Settings {
		// TODO: Honor ViewDistance Setting
		public enum ViewDistance {
			Set
		}

		// TODO: Honor LogFileLocation Setting
		public enum LogFileLocation {
			Set
		}

		// TODO: Honor TextureFiltering Setting
		public enum TextureFiltering {
			None,
			Bilinear,
			Trilinear,
			Anisotropic2,
			Anisotropic4,
			Anisotropic8,
			Anisotropic16
		}

		// TODO: Honor RendererType Setting
		public enum RendererType {
			Forward,
			Deferred
		}

		// TODO: Honor ForwardAntialiasing Setting
		public enum ForwardAntialiasing {
			None,
			MSAA2,
			MSAA4,
			MSAA8,
			SSAA2,
			SSAA4
		}

		// TODO: Honor DeferredAntialiasing Setting
		public enum DeferredAntialiasing {
			None,
			SSAA2,
			SSAA4
		}

		// TODO: Honor UIAntialiasing Setting
		public enum UIAntialiasing {
			None,
			MSAA2,
			MSAA4,
			MSAA8
		}

		// TODO: Honor TextRenderingQuality Setting
		public enum TextRenderingQuality {
			Low,
			Medium,
			High,
			Ultra
		}

		public float view_distance;
		public string log_file_location;
		public TextureFiltering texture_filtering;
		public RendererType renderer_type;
		public ForwardAntialiasing forward_aa;
		public DeferredAntialiasing deferred_aa;
		public UIAntialiasing ui_aa;
		public TextRenderingQuality text_rendering_quality;
	}

	public partial class Renderer {
		Settings settings = new Settings {
			view_distance          = 1000.0f,
			log_file_location      = null,
			texture_filtering      = Settings.TextureFiltering.None,
			renderer_type          = Settings.RendererType.Forward,
			forward_aa             = Settings.ForwardAntialiasing.None,
			deferred_aa            = Settings.DeferredAntialiasing.None,
			ui_aa                  = Settings.UIAntialiasing.None,
			text_rendering_quality = Settings.TextRenderingQuality.Low,
		};

		public Settings GetSettings() {
			return settings;
		}

		public void SetSetting(Settings set) {
			settings = set;
		}

		public void SetSetting(Settings.ViewDistance ignore, float distance) {
			if (distance <= 1) {
				throw new System.ArgumentException(distance.ToString() + " is not a valid distance");
			}
			settings.view_distance = distance;
		}

		public void SetSetting(Settings.LogFileLocation ignore, string location) {
			if (location != null) {
				string dir = Path.GetDirectoryName(location);
				bool exists = Directory.Exists(dir);
				if (!exists) {
					throw new System.ArgumentException(location + " is not a valid path");
				}
			}
			settings.log_file_location = location;
		}

		public void SetSetting(Settings.TextureFiltering filtering) {
			settings.texture_filtering = filtering;
		}

		public void SetSetting(Settings.RendererType type) {
			settings.renderer_type = type;
		}

		public void SetSetting(Settings.ForwardAntialiasing type) {
			settings.forward_aa = type;
		}

		public void SetSetting(Settings.DeferredAntialiasing type) {
			settings.deferred_aa = type;
		}

		public void SetSetting(Settings.UIAntialiasing type) {
			settings.ui_aa = type;
		}

		public void SetSetting(Settings.TextRenderingQuality quality) {
			settings.text_rendering_quality = quality;
		}
	}
}
