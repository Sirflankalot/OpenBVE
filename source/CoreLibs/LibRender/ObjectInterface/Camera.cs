using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct CameraHandle {
		internal long id;
		internal CameraHandle(long id) {
			this.id = id;
		}
	}

	internal class Camera {
		internal Vector3 focal_point;
		internal Vector3 position;
		internal Vector2 rotation;
		internal float distance;
		internal Matrix4 transform_matrix = new Matrix4();
		internal Matrix4 view_matrix = new Matrix4();
		internal Matrix4 proj_matrix = new Matrix4();
		internal Matrix4 inverse_projection_matrix = new Matrix4();
		internal bool matrix_valid = false;
		internal float fov;

		internal CameraHandle handle;
	}

	public partial class Renderer {
		internal long camera_current_id = 1;
		internal Dictionary<long, int> camera_translation = new Dictionary<long, int>() { { 0, 0 } };
		internal List<Camera> cameras = new List<Camera>() { new Camera() { focal_point = new Vector3(0), rotation = new Vector2(0), distance = -10, fov = 50, handle = new CameraHandle(0) } };
		internal int active_camera;

		internal int AssertValid(CameraHandle ch) {
			int real;
			if(!camera_translation.TryGetValue(ch.id, out real)) {
				throw new System.ArgumentException("Invalid CameraHandle, no possible translation" + ch.id.ToString());
			}
			if (cameras.Count <= real) {
				throw new System.ArgumentException("Camera Handle ID larger than array: " + ch.id.ToString());
			}
			if (cameras[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted camera: " + ch.id.ToString());
			}
			return real;
		}

		public CameraHandle AddCamera(Vector3 location, Vector2 rotation, float fov, bool active = true) {
			Camera c = new Camera() {
				focal_point = location,
				rotation = rotation,
				fov = fov
			};

			long id = camera_current_id++;
			c.handle = new CameraHandle(id);

			int index_active = cameras.Count;
			camera_translation.Add(id, index_active);
			cameras.Add(c);
			if (active) {
				active_camera = index_active;
			}
			return c.handle;
		}

		public void Delete(CameraHandle ch) {
			int id = AssertValid(ch);
			
			if (id == 0) {
				return;
			}

			camera_translation.Remove(ch.id);
			cameras[id] = null;
		}

        public bool Valid(CameraHandle ch) {
            try {
                AssertValid(ch);
            }
            catch(System.Exception) {
                return false;
            }
            return true;
        }

		////////////////////////////////
		// Camera Setters and Getters //
		////////////////////////////////

        public Vector3 GetEyeVector(CameraHandle ch) {
            int id = AssertValid(ch);

            return (cameras[id].position - cameras[id].focal_point).Normalized();
        }

		public Vector3 GetLocation(CameraHandle ch) {
			int id = AssertValid(ch);

			return cameras[id].focal_point;
		}

		public Vector2 GetRotation(CameraHandle ch) {
			int id = AssertValid(ch);

			return cameras[id].rotation;
		}

		public float GetDistance(CameraHandle ch) {
			int id = AssertValid(ch);

			return cameras[id].distance;
		}

		public float GetFOV(CameraHandle ch) {
			int id = AssertValid(ch);

			return cameras[id].fov;
		}

		public CameraHandle GetActiveCamera() {
			return new CameraHandle(active_camera);
		}

		public CameraHandle GetStartingCamera() {
			return new CameraHandle(0);
		}

		public void SetLocation(CameraHandle ch, Vector3 location) {
			int id = AssertValid(ch);

			cameras[id].focal_point = location;
			cameras[id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetRotation(CameraHandle ch, Vector2 rotation) {
			int id = AssertValid(ch);

			cameras[id].rotation = rotation;
			cameras[id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetDistance(CameraHandle ch, float distance) {
			int id = AssertValid(ch);

			cameras[id].distance = distance;
			cameras[id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

		public void SetFOV(CameraHandle ch, float fov) {
			int id = AssertValid(ch);

			cameras[id].fov = fov;
			cameras[id].matrix_valid = false;
		}

		public void SetActiveCamera(CameraHandle ch) {
			int id = AssertValid(ch);

			active_camera = id;
			cameras[id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}
	}
}