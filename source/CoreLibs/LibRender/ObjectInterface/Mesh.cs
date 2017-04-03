using OpenTK;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct MeshHandle {
		internal int id;
		internal MeshHandle(int id) {
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

		public Mesh Copy() {
			Mesh m = new Mesh();
			m.vertices.AddRange(vertices);
			m.indices.AddRange(indices);
			if (updated_normals) {
				m.normals.AddRange(normals);
			}
			return m;
		}
	}

	public partial class Renderer {
		internal List<Mesh> meshes = new List<Mesh>();
		
		internal void AssertValid(MeshHandle mh) {
			if (meshes.Count <= mh.id) {
				throw new System.ArgumentException("Mesh Handle ID larger than array: " + mh.id.ToString());
			}
			if (meshes[mh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted mesh: " + mh.id.ToString());
			}
		}

		public MeshHandle AddMesh(Vertex3D[] mesh, int[] vertex_indices) {
			Mesh m = new Mesh();
			m.vertices.AddRange(mesh);
			m.indices.AddRange(vertex_indices);
			meshes.Add(m);
			return new MeshHandle(meshes.Count - 1);
		}

		public void Update(MeshHandle mh, Vertex3D[] mesh, int[] vertex_indices) {
			AssertValid(mh);

			meshes[mh.id].vertices.Clear();
			meshes[mh.id].vertices.AddRange(mesh);
			meshes[mh.id].indices.Clear();
			meshes[mh.id].indices.AddRange(vertex_indices);
			meshes[mh.id].updated_normals = false;
			meshes[mh.id].uploaded = false;
		}

		public void Delete(MeshHandle mh) {
			AssertValid(mh);

			GLFunc.DeleteVertexArray(meshes[mh.id].gl_vao_id);
			GLFunc.DeleteBuffer(meshes[mh.id].gl_vert_id);
			GLFunc.DeleteBuffer(meshes[mh.id].gl_indices_id);

			meshes[mh.id] = null;
		}
	}
}