using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct PointLightHandle {
		internal int id;
		internal PointLightHandle(int id) {
			this.id = id;
		}
	}

	internal class Point_Light {
		internal Vector3 location;
		internal float brightness;
	}

	public partial class Renderer {
		internal List<Point_Light> point_lights = new List<Point_Light>();

		internal void AssertValid(PointLightHandle plh) {
			if (point_lights.Count <= plh.id) {
				throw new System.ArgumentException("Point Light Handle ID larger than array: " + plh.id.ToString());
			}
			if (point_lights[plh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted point light: " + plh.id.ToString());
			}
		}

		public PointLightHandle AddPointLight(Vector3 location, float brightness) {
			Point_Light pl = new Point_Light();
			pl.location = location;
			pl.brightness = brightness;
			point_lights.Add(pl);
			return new PointLightHandle(point_lights.Count - 1);
		}

		public void Delete(PointLightHandle plh) {
			AssertValid(plh);

			point_lights[plh.id] = null;
		}

		/////////////////////////////////////
		// Point Light Getters and Setters //
		/////////////////////////////////////

		public Vector3 GetLocation(PointLightHandle plh) {
			AssertValid(plh);

			return point_lights[plh.id].location;
		}

		public float GetBrightness(PointLightHandle plh) {
			AssertValid(plh);

			return point_lights[plh.id].brightness;
		}

		public void SetLocation(PointLightHandle plh, Vector3 location) {
			AssertValid(plh);

			point_lights[plh.id].location = location;
		}

		public void SetBrightness(PointLightHandle plh, float brightness) {
			AssertValid(plh);

			point_lights[plh.id].brightness = brightness;
		}
	}
}