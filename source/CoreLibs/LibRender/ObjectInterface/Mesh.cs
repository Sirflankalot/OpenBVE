using OpenTK;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct MeshHandle {
		internal long id;
		internal MeshHandle(long id) {
			this.id = id;
		}
	}

	internal class Mesh {
		internal List<Vertex3D> vertices = new List<Vertex3D>();
		internal List<int> indices = new List<int>();
		internal List<Vector3> normals = new List<Vector3>();

		internal bool updated_normals = false;

		internal int gl_vao_id = 0;
		internal int gl_vert_id = 0;
		internal int gl_indices_id = 0;
		internal bool uploaded = false;

		internal MeshHandle handle;

		public Mesh Copy() {
			Mesh m = new Mesh();
			m.vertices.AddRange(vertices);
			m.indices.AddRange(indices);
			if (updated_normals) {
				m.normals.AddRange(normals);
			}
			m.handle = handle;
			return m;
		}
	}

	public partial class Renderer {
		internal long meshes_current_id = 0;
		internal Dictionary<long, int> meshes_translation = new Dictionary<long, int>();
		internal List<Mesh> meshes = new List<Mesh>();
		
		internal int AssertValid(MeshHandle mh) {
			int real;
			if (!meshes_translation.TryGetValue(mh.id, out real)) {
				throw new System.ArgumentException("Invalid MeshHandle, no possible translation" + mh.id.ToString());
			}
			if (meshes.Count <= real) {
				throw new System.ArgumentException("Mesh Handle ID larger than array: " + mh.id.ToString());
			}
			if (meshes[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted mesh: " + mh.id.ToString());
			}
			return real;
		}

		public MeshHandle AddMesh(Vertex3D[] mesh, int[] vertex_indices) {
			Mesh m = new Mesh();
			m.vertices.AddRange(mesh);
			m.indices.AddRange(vertex_indices);

			long id = meshes_current_id++;
			m.handle = new MeshHandle(id);

			meshes_translation.Add(id, meshes.Count);
			meshes.Add(m);
			return m.handle;
		}

		public void Update(MeshHandle mh, Vertex3D[] mesh, int[] vertex_indices) {
			int id = AssertValid(mh);

			meshes[id].vertices.Clear();
			meshes[id].vertices.AddRange(mesh);
			meshes[id].indices.Clear();
			meshes[id].indices.AddRange(vertex_indices);
			meshes[id].updated_normals = false;
			meshes[id].uploaded = false;
		}

		public void Delete(MeshHandle mh) {
			int id = AssertValid(mh);

			GLFunc.DeleteVertexArray(meshes[id].gl_vao_id);
			GLFunc.DeleteBuffer(meshes[id].gl_vert_id);
			GLFunc.DeleteBuffer(meshes[id].gl_indices_id);

			meshes_translation.Remove(mh.id);
			meshes[id] = null;
		}
	}
}