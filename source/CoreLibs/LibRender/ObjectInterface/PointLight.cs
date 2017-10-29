using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct PointLightHandle {
		internal long id;
		internal PointLightHandle(long id) {
			this.id = id;
		}
	}

	internal class Point_Light {
		internal Vector3 location;
		internal float brightness;

		internal PointLightHandle handle;
	}

	public partial class Renderer {
		internal long point_lights_id = 0;
		internal Dictionary<long, int> point_lights_translation = new Dictionary<long, int>();
		internal List<Point_Light> point_lights = new List<Point_Light>();

		internal int AssertValid(PointLightHandle plh) {
			int real;
			if (!point_lights_translation.TryGetValue(plh.id, out real)) {
				throw new System.ArgumentException("Invalid PointLightHandle, no possible translation" + plh.id.ToString());
			}
			if (point_lights.Count <= real) {
				throw new System.ArgumentException("Point Light Handle ID larger than array: " + plh.id.ToString());
			}
			if (point_lights[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted point light: " + plh.id.ToString());
			}
			return real;
		}

		public PointLightHandle AddPointLight(Vector3 location, float brightness) {
			Point_Light pl = new Point_Light();
			pl.location = location;
			pl.brightness = brightness;

			long id = point_lights_id++;
			pl.handle = new PointLightHandle(id);

			point_lights_translation.Add(id, point_lights.Count);
			point_lights.Add(pl);
			return pl.handle;
		}

		public void Delete(PointLightHandle plh) {
			int id = AssertValid(plh);

			point_lights_translation.Remove(plh.id);
			point_lights[id] = null;
		}

        public bool Valid(PointLightHandle plh) {
            try {
                AssertValid(plh);
            }
            catch (System.Exception) {
                return false;
            }
            return true;
        }

        /////////////////////////////////////
        // Point Light Getters and Setters //
        /////////////////////////////////////

        public Vector3 GetFocalPoint(PointLightHandle plh) {
			int id = AssertValid(plh);

			return point_lights[id].location;
		}

		public float GetBrightness(PointLightHandle plh) {
			int id = AssertValid(plh);

			return point_lights[id].brightness;
		}

		public void SetFocalPoint(PointLightHandle plh, Vector3 location) {
			int id = AssertValid(plh);

			point_lights[id].location = location;
		}

		public void SetBrightness(PointLightHandle plh, float brightness) {
			int id = AssertValid(plh);

			point_lights[id].brightness = brightness;
		}
	}
}