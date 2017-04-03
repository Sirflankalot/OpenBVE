using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct CameraHandle {
		internal int id;
		internal CameraHandle(int id) {
			this.id = id;
		}
	}

	internal class Camera {
		internal Vector3 focal_point;
		internal Vector2 rotation;
		internal float distance;
		internal Matrix4 transform_matrix = new Matrix4();
		internal Matrix4 view_matrix = new Matrix4();
		internal Matrix4 proj_matrix = new Matrix4();
		internal Matrix4 inverse_projection_matrix = new Matrix4();
		internal bool matrix_valid = false;
		internal float fov;
	}

	public partial class Renderer {
		internal List<Camera> cameras = new List<Camera>() { new Camera() { focal_point = new Vector3(0), rotation = new Vector2(0), distance = -10, fov = 50 } };
		internal int active_camera;

		internal void AssertValid(CameraHandle ch) {
			if (cameras.Count <= ch.id) {
				throw new System.ArgumentException("Camera Handle ID larger than array: " + ch.id.ToString());
			}
			if (cameras[ch.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted camera: " + ch.id.ToString());
			}
		}

		public CameraHandle AddCamera(Vector3 location, Vector2 rotation, float fov, bool active = true) {
			Camera c = new Camera();
			c.focal_point = location;
			c.rotation = rotation;
			c.fov = fov;
			cameras.Add(c);
			var index = cameras.Count - 1;
			if (active) {
				active_camera = index;
			}
			return new CameraHandle(index);
		}

		public void Delete(CameraHandle ch) {
			AssertValid(ch);

			// TODO: Log deleting default camera
			if (ch.id == 0) {
				return;
			}

			cameras[ch.id] = null;
		}

		////////////////////////////////
		// Camera Setters and Getters //
		////////////////////////////////

		public Vector3 GetLocation(CameraHandle ch) {
			AssertValid(ch);

			return cameras[ch.id].focal_point;
		}

		public Vector2 GetRotation(CameraHandle ch) {
			AssertValid(ch);

			return cameras[ch.id].rotation;
		}

		public float GetDistance(CameraHandle ch) {
			AssertValid(ch);

			return cameras[ch.id].distance;
		}

		public float GetFOV(CameraHandle ch) {
			AssertValid(ch);

			return cameras[ch.id].fov;
		}

		public CameraHandle GetActiveCamera() {
			return new CameraHandle(active_camera);
		}

		public CameraHandle GetStartingCamera() {
			return new CameraHandle(0);
		}

		public void SetLocation(CameraHandle ch, Vector3 location) {
			AssertValid(ch);

			cameras[ch.id].focal_point = location;
			cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetRotation(CameraHandle ch, Vector2 rotation) {
			AssertValid(ch);

			cameras[ch.id].rotation = rotation;
			cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetDistance(CameraHandle ch, float distance) {
			AssertValid(ch);

			cameras[ch.id].distance = distance;
			cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetFOV(CameraHandle ch, float fov) {
			AssertValid(ch);

			cameras[ch.id].fov = fov;
			cameras[ch.id].matrix_valid = false;
		}

		public void SetActiveCamera(CameraHandle ch) {
			AssertValid(ch);

			active_camera = ch.id;
			cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}
	}
}