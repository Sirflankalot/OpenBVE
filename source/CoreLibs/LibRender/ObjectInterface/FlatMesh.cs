using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct FlatMeshHandle {
		internal int id;
		internal FlatMeshHandle(int id) {
			this.id = id;
		}
	}

	internal class FlatMesh {
		internal List<Vertex2D> vertices = new List<Vertex2D>();
		internal List<int> indices = new List<int>();

		internal int gl_vao_id = 0;
		internal int gl_vert_id = 0;
		internal int gl_indices_id = 0;

		internal bool uploaded = false;

		internal FlatMesh Copy() {
			FlatMesh fm = new FlatMesh();
			fm.vertices.AddRange(vertices);
			fm.indices.AddRange(indices);

			return fm;
		}
	}

	public partial class Renderer {
		internal List<FlatMesh> flat_meshes = new List<FlatMesh>();

		internal void AssertValid(FlatMeshHandle fmh) {
			if (flat_meshes.Count <= fmh.id) {
				throw new System.ArgumentException("Flat Mesh Handle ID larger than array: " + fmh.id.ToString());
			}

			if (flat_meshes[fmh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted flat mesh: " + fmh.id.ToString());
			}
		}

		public FlatMeshHandle AddFlatMesh(Vertex2D[] mesh, int[] indices) {
			FlatMesh fm = new FlatMesh();
			fm.vertices.AddRange(mesh);
			fm.indices.AddRange(indices);
			flat_meshes.Add(fm);
			return new FlatMeshHandle(flat_meshes.Count - 1);
		}

		public void Update(FlatMeshHandle fmh, Vertex2D[] mesh, int[] indices) {
			AssertValid(fmh);

			flat_meshes[fmh.id].vertices.Clear();
			flat_meshes[fmh.id].vertices.AddRange(mesh);
			flat_meshes[fmh.id].indices.Clear();
			flat_meshes[fmh.id].indices.AddRange(indices);
			flat_meshes[fmh.id].uploaded = false;
		}

		public void Delete(FlatMeshHandle fmh) {
			AssertValid(fmh);

			GLFunc.DeleteVertexArray(flat_meshes[fmh.id].gl_vao_id);
			GLFunc.DeleteBuffer(flat_meshes[fmh.id].gl_vert_id);
			GLFunc.DeleteBuffer(flat_meshes[fmh.id].gl_indices_id);

			flat_meshes[fmh.id] = null;
		}
	}
}