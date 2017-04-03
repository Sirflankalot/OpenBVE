using OpenTK;

namespace LibRender {
	internal class Sun {
		internal Vector3 color = new Vector3(1.0f, 0.8f, 0.8f);
		internal Vector2 location = new Vector2(0.0f, 0.0f);
		internal float brightness = 1.0f;
		internal Vector3 direction;
		internal Matrix4 shadow_matrix;
		internal bool matrix_valid;
		internal int gl_tex_id = 0;
	}

	public partial class Renderer {
		internal Sun sun = new Sun();

		/////////////////////////////
		// Sun Getters and Setters //
		/////////////////////////////

		public Vector3 GetSunColor() {
			return sun.color;
		}

		public Vector2 GetSunLocation() {
			return sun.location;
		}

		public float GetSunBrightness() {
			return sun.brightness;
		}

		public void SetSunColor(Vector3 color) {
			sun.color = color;
		}

		public void SetSunLocation(Vector2 location) {
			sun.location = location;
			sun.matrix_valid = false;
		}

		public void SetSunBrightness(float brightness) {
			sun.brightness = brightness;
		}
	}
}