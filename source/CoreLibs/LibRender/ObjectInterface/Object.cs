using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct ObjectHandle {
		internal long id;
		internal ObjectHandle(long id) {
			this.id = id;
		}
	}

	internal class Object {
		internal MeshHandle mesh;
		internal TextureHandle texture;
		internal bool visible;
		internal Vector3 position;
		internal Vector3 rotation;
		internal Vector3 scale = new Vector3(1, 1, 1);
		internal Matrix4 transform;
		internal bool matrix_valid = false;
		internal Matrix3 inverse_model_view_matrix;
		internal bool inverse_model_view_valid = false;

		internal ObjectHandle handle;
	}

	public partial class Renderer {
		internal long object_id = 0;
		internal Dictionary<long, int> object_translation = new Dictionary<long, int>();
		internal List<Object> objects = new List<Object>();

		internal int AssertValid(ObjectHandle oh) {
			int real;
			if (!object_translation.TryGetValue(oh.id, out real)) {
				throw new System.ArgumentException("Invalid ObjectHandle, no possible translation" + oh.id.ToString());
			}
			if (objects.Count <= real) {
				throw new System.ArgumentException("Object Handle ID larger than array: " + oh.id.ToString());
			}
			if (objects[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted object: " + oh.id.ToString());
			}
			return real;
		}

		public ObjectHandle AddObject(MeshHandle mh, TextureHandle th, bool visible = true) {
			AssertValid(mh);
			AssertValid(th);

			Object o = new Object() {
				mesh = mh,
				texture = th,
				visible = visible
			};

			long id = object_id++;
			o.handle = new ObjectHandle(id);

			object_translation.Add(id, objects.Count);
			objects.Add(o);
			return o.handle;
		}

		public void Update(ObjectHandle oh, MeshHandle mh, TextureHandle th) {
			int id = AssertValid(oh);
			AssertValid(mh);
			AssertValid(th);

			objects[id].mesh = mh;
			objects[id].texture = th;
		}

		public void Delete(ObjectHandle oh) {
			int id = AssertValid(oh);

			object_translation.Remove(oh.id);
			objects[id] = null;
		}

		////////////////////////////////
		// Object Setters and Getters //
		////////////////////////////////

		public bool GetVisibility(ObjectHandle oh) {
			int id = AssertValid(oh);

			return objects[id].visible;
		}

		public Vector3 GetLocation(ObjectHandle oh) {
			int id = AssertValid(oh);

			return objects[id].position;
		}

		public Vector3 GetRotation(ObjectHandle oh) {
			int id = AssertValid(oh);

			return objects[id].rotation;
		}

		public Vector3 GetScale(ObjectHandle oh) {
			int id = AssertValid(oh);

			return objects[id].scale;
		}

		public void SetVisibility(ObjectHandle oh, bool visible) {
			int id = AssertValid(oh);

			objects[id].visible = visible;
		}

		public void SetLocation(ObjectHandle oh, Vector3 pos) {
			int id = AssertValid(oh);

			objects[id].position = pos;
			objects[id].matrix_valid = false;
			objects[id].inverse_model_view_valid = false;
		}

		public void SetRotation(ObjectHandle oh, Vector3 rot) {
			int id = AssertValid(oh);
			objects[id].rotation = rot;
			objects[id].matrix_valid = false;
			objects[id].inverse_model_view_valid = false;
		}

		public void SetScale(ObjectHandle oh, Vector3 scale) {
			int id = AssertValid(oh);

			objects[id].scale = scale;
			objects[id].matrix_valid = false;
			objects[id].inverse_model_view_valid = false;
		}
	}
}