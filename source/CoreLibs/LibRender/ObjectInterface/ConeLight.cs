using OpenTK;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
	public struct ConeLightHandle {
		internal long id;
		internal ConeLightHandle(long id) {
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

		internal ConeLightHandle handle;
	}

	public partial class Renderer {
		internal long cone_lights_id = 0;
		internal Dictionary<long, int> cone_lights_translation = new Dictionary<long, int>();
		internal List<Cone_Light> cone_lights = new List<Cone_Light>();

		internal int AssertValid(ConeLightHandle clh) {
			int real;
			if (!cone_lights_translation.TryGetValue(clh.id, out real)) {
				throw new System.ArgumentException("Invalid ConeLightHandle, no possible translation" + clh.id.ToString());
			}
			if (cone_lights.Count <= real) {
				throw new System.ArgumentException("Cone Light Handle ID larger than array: " + clh.id.ToString());
			}
			if (cone_lights[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted cone light: " + clh.id.ToString());
			}
			return real;
		}

		public ConeLightHandle AddConeLight(Vector3 location, Vector3 direction, float brightness, float fov, bool shadow_casting = false) {
			Cone_Light cl = new Cone_Light() {
				location = location,
				direction = direction,
				brightness = brightness,
				fov = fov,
				shadow = shadow_casting
			};

			long id = cone_lights_id++;
			cl.handle = new ConeLightHandle(id);

			cone_lights_translation.Add(id, cone_lights.Count);
			cone_lights.Add(cl);
			return cl.handle;
		}

		public void Delete(ConeLightHandle clh) {
			int id = AssertValid(clh);

			int tex = cone_lights[id].gl_tex_id;
			if (tex != 0) {
				GLFunc.DeleteTexture(tex);
			}

			cone_lights_translation.Remove(clh.id);
			cone_lights[id] = null;
		}

		////////////////////////////////////
		// Cone Light Getters and Setters //
		////////////////////////////////////

		public Vector3 GetLocation(ConeLightHandle clh) {
			int id = AssertValid(clh);

			return cone_lights[id].location;
		}

		public Vector3 GetDirection(ConeLightHandle clh) {
			int id = AssertValid(clh);

			return cone_lights[id].direction;
		}

		public float GetFOV(ConeLightHandle clh) {
			int id = AssertValid(clh);

			return cone_lights[id].fov;
		}

		public float GetBrightness(ConeLightHandle clh) {
			int id = AssertValid(clh);

			return cone_lights[id].brightness;
		}

		public bool GetShadow(ConeLightHandle clh) {
			int id = AssertValid(clh);

			return cone_lights[id].shadow;
		}

		public void SetLocation(ConeLightHandle clh, Vector3 location) {
			int id = AssertValid(clh);

			cone_lights[id].location = location;
		}

		public void SetDirection(ConeLightHandle clh, Vector3 direction) {
			int id = AssertValid(clh);

			cone_lights[id].direction = direction;
		}

		public void SetFOV(ConeLightHandle clh, float fov) {
			int id = AssertValid(clh);

			cone_lights[id].fov = fov;
		}

		public void SetBrightness(ConeLightHandle clh, float brightness) {
			int id = AssertValid(clh);

			cone_lights[id].brightness = brightness;
		}

		public void SetShadow(ConeLightHandle clh, bool shadow) {
			int id = AssertValid(clh);

			cone_lights[id].shadow = shadow;
		}
	}
}