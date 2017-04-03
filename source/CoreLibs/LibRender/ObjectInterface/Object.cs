using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct ObjectHandle {
		internal int id;
		internal ObjectHandle(int id) {
			this.id = id;
		}
	}

	internal class Object {
		internal int mesh_id;
		internal int tex_id;
		internal bool visible;
		internal Vector3 position;
		internal Vector3 rotation;
		internal Vector3 scale = new Vector3(1, 1, 1);
		internal Matrix4 transform;
		internal bool matrix_valid = false;
		internal Matrix3 inverse_model_view_matrix;
		internal bool inverse_model_view_valid = false;
	}

	public partial class Renderer {
		internal List<Object> objects = new List<Object>();

		internal void AssertValid(ObjectHandle oh) {
			if (objects.Count <= oh.id) {
				throw new System.ArgumentException("Object Handle ID larger than array: " + oh.id.ToString());
			}
			if (objects[oh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted object: " + oh.id.ToString());
			}
		}

		public ObjectHandle AddObject(MeshHandle mh, TextureHandle th, bool visible = true) {
			Object o = new Object();
			o.mesh_id = mh.id;
			o.tex_id = th.id;
			o.visible = visible;
			objects.Add(o);
			return new ObjectHandle(objects.Count - 1);
		}

		public void Update(ObjectHandle oh, MeshHandle mh, TextureHandle th) {
			AssertValid(oh);
			AssertValid(mh);
			AssertValid(th);

			objects[oh.id].mesh_id = mh.id;
			objects[oh.id].tex_id = th.id;
		}

		public void Delete(ObjectHandle oh) {
			AssertValid(oh);

			objects[oh.id] = null;
		}

		////////////////////////////////
		// Object Setters and Getters //
		////////////////////////////////

		public bool GetVisibility(ObjectHandle oh) {
			AssertValid(oh);

			return objects[oh.id].visible;
		}

		public Vector3 GetLocation(ObjectHandle oh) {
			AssertValid(oh);

			return objects[oh.id].position;
		}

		public Vector3 GetRotation(ObjectHandle oh) {
			AssertValid(oh);

			return objects[oh.id].rotation;
		}

		public Vector3 GetScale(ObjectHandle oh) {
			AssertValid(oh);

			return objects[oh.id].scale;
		}

		public void SetVisibility(ObjectHandle oh, bool visible) {
			AssertValid(oh);

			objects[oh.id].visible = visible;
		}

		public void SetLocation(ObjectHandle oh, Vector3 pos) {
			AssertValid(oh);

			objects[oh.id].position = pos;
			objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}

		public void SetRotation(ObjectHandle oh, Vector3 rot) {
			AssertValid(oh);
			objects[oh.id].rotation = rot;
			objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}

		public void SetScale(ObjectHandle oh, Vector3 scale) {
			AssertValid(oh);

			objects[oh.id].scale = scale;
			objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}
	}
}