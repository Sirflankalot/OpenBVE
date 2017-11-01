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

		internal Vector3 midpoint = new Vector3();

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
			m.midpoint = midpoint;
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

		private Vector3 MidPoint(Vertex3D[] mesh) {
			Vector3 sum = new Vector3(0, 0, 0);
			foreach (var v in mesh) {
				sum += new Vector3(v.position.X, v.position.Y, v.position.Z);
			}
			return sum / mesh.Length;
		}

		public MeshHandle AddMesh(Vertex3D[] mesh, int[] vertex_indices) {
			Mesh m = new Mesh();
			m.vertices.AddRange(mesh);
			m.indices.AddRange(vertex_indices);

			long id = meshes_current_id++;
			m.handle = new MeshHandle(id);

			m.midpoint = MidPoint(mesh);

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
			meshes[id].midpoint = MidPoint(mesh);
		}

		public void Delete(MeshHandle mh) {
			int id = AssertValid(mh);

			GLFunc.DeleteVertexArray(meshes[id].gl_vao_id);
			GLFunc.DeleteBuffer(meshes[id].gl_vert_id);
			GLFunc.DeleteBuffer(meshes[id].gl_indices_id);

			meshes_translation.Remove(mh.id);
			meshes[id] = null;
		}

        public bool Valid(MeshHandle mh) {
            try {
                AssertValid(mh);
            }
            catch (System.Exception) {
                return false;
            }
            return true;
        }

		/////////////////////////////////////////
		// Utility Functions for Common Idioms //
		/////////////////////////////////////////

		internal MeshHandle cube_mesh;

		/// <summary>
		/// Get handle for premade 2x2x2 (-1 - 1) cube
		/// </summary>
		/// <returns>Cube handle</returns>
		public MeshHandle CubeMesh() {
			return cube_mesh;
		}

		internal MeshHandle CreateCubeMesh() {
			Vertex3D[] cube_mesh_vertices = new Vertex3D[] {
				new Vertex3D() { position = new Vector3(  1, -1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f, -0.5773f, -0.5773f )}, // Bottom, Left, Front
				new Vertex3D() { position = new Vector3(  1, -1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f, -0.5773f,  0.5773f ) }, // Bottom, Right, Front
				new Vertex3D() { position = new Vector3( -1, -1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f, -0.5773f,  0.5773f ) }, // Top, Right, Front
				new Vertex3D() { position = new Vector3( -1, -1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f, -0.5773f, -0.5773f ) }, // Top, Left, Front
				new Vertex3D() { position = new Vector3(  1,  1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f,  0.5773f, -0.5773f ) }, // Bottom, Left, Back
				new Vertex3D() { position = new Vector3(  1,  1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f,  0.5773f,  0.5773f ) }, // Bottom, Right, Back
				new Vertex3D() { position = new Vector3( -1,  1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f,  0.5773f,  0.5773f ) }, // Top, Right, Back
				new Vertex3D() { position = new Vector3( -1,  1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f,  0.5773f, -0.5773f ) }  // Top, Left, Back
			};

			int[] cube_mesh_indices = new int[] {
				1,3,0,
				7,5,4,
				4,1,0,
				5,2,1,
				2,7,3,
				0,7,4,
				1,2,3,
				7,6,5,
				4,5,1,
				5,6,2,
				2,6,7,
				0,3,7
			};

			return AddMesh(cube_mesh_vertices, cube_mesh_indices);
		}
    }
}
