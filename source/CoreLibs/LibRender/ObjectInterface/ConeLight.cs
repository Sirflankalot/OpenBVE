using OpenTK;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
	public struct ConeLightHandle {
		internal int id;
		internal ConeLightHandle(int id) {
			this.id = id;
		}
	}

	internal class Cone_Light {
		internal Vector3 location;
		internal Vector3 direction;
		internal Matrix4 shadow_matrix;
		internal bool matrix_valid = false;
		internal float brightness;
		internal float fov;
		internal bool shadow;
		internal int gl_tex_id = 0;
	}

	public partial class Renderer {
		internal List<Cone_Light> cone_lights = new List<Cone_Light>();

		internal void AssertValid(ConeLightHandle clh) {
			if (cone_lights.Count <= clh.id) {
				throw new System.ArgumentException("Cone Light Handle ID larger than array: " + clh.id.ToString());
			}
			if (cone_lights[clh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted cone light: " + clh.id.ToString());
			}
		}

		public ConeLightHandle AddConeLight(Vector3 location, Vector3 direction, float brightness, float fov, bool shadow_casting = false) {
			Cone_Light cl = new Cone_Light();
			cl.location = location;
			cl.direction = direction;
			cl.brightness = brightness;
			cl.fov = fov;
			cl.shadow = shadow_casting;
			cone_lights.Add(cl);
			return new ConeLightHandle(cone_lights.Count - 1);
		}

		public void Delete(ConeLightHandle clh) {
			AssertValid(clh);

			int tex = cone_lights[clh.id].gl_tex_id;
			if (tex != 0) {
				GLFunc.DeleteTexture(tex);
			}

			cone_lights[clh.id] = null;
		}

		////////////////////////////////////
		// Cone Light Getters and Setters //
		////////////////////////////////////

		public Vector3 GetLocation(ConeLightHandle clh) {
			AssertValid(clh);

			return cone_lights[clh.id].location;
		}

		public Vector3 GetDirection(ConeLightHandle clh) {
			AssertValid(clh);

			return cone_lights[clh.id].direction;
		}

		public float GetFOV(ConeLightHandle clh) {
			AssertValid(clh);

			return cone_lights[clh.id].fov;
		}

		public float GetBrightness(ConeLightHandle clh) {
			AssertValid(clh);

			return cone_lights[clh.id].brightness;
		}

		public bool GetShadow(ConeLightHandle clh) {
			AssertValid(clh);

			return cone_lights[clh.id].shadow;
		}

		public void SetLocation(ConeLightHandle clh, Vector3 location) {
			AssertValid(clh);

			cone_lights[clh.id].location = location;
		}

		public void SetDirection(ConeLightHandle clh, Vector3 direction) {
			AssertValid(clh);

			cone_lights[clh.id].direction = direction;
		}

		public void SetFOV(ConeLightHandle clh, float fov) {
			AssertValid(clh);

			cone_lights[clh.id].fov = fov;
		}

		public void SetBrightness(ConeLightHandle clh, float brightness) {
			AssertValid(clh);

			cone_lights[clh.id].brightness = brightness;
		}

		public void SetShadow(ConeLightHandle clh, bool shadow) {
			AssertValid(clh);

			cone_lights[clh.id].shadow = shadow;
		}
	}
}