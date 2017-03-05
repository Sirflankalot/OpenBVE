using OpenTK;
using System.Linq;
using System.Collections.Generic;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    internal static class Algorithms {
        internal static void UpdateMeshNormals(List<Mesh> meshes, int start, int end) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= meshes.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int mesh_it = start; mesh_it < end; ++mesh_it) {
                // Reference to working mesh
                Mesh m = meshes[mesh_it];

                // Skip if normals already updated or is null
                if (m == null || m.updated_normals) {
                    continue;
                }

                // Set normal list to correct length
                var new_size = m.vertices.Count;
                var old_size = m.normals.Count;
                if (new_size > old_size) {
                    m.normals.Capacity = new_size;
                    m.normals.AddRange(Enumerable.Repeat(new Vector3(0, 0, 0), new_size - old_size));
                }
                else {
                    m.normals.RemoveRange(new_size, old_size - new_size);
                }

                // Zero out normals
                for (int i = 0; i < new_size; i++) {
                    m.normals[i] = new Vector3(0, 0, 0);
                }

                // Calculate each face's normals and add them to vertex normals
                for (int i = 0; i < m.indices.Count; i += 3) {
                    int index1 = m.indices[i + 0];
                    int index2 = m.indices[i + 1];
                    int index3 = m.indices[i + 2];

                    Vector3 vert1 = m.vertices[index1].position;
                    Vector3 vert2 = m.vertices[index2].position;
                    Vector3 vert3 = m.vertices[index3].position;

                    Vector3 side1 = vert2 - vert1;
                    Vector3 side2 = vert2 - vert3;

                    Vector3 cross = (side1.Yzx * side2.Zxy) - (side1.Zxy * side2.Yzx);

                    m.normals[index1] += cross;
                    m.normals[index2] += cross;
                    m.normals[index3] += cross;
                }

                // Normalize all normals
                for (int i = 0; i < m.normals.Count; ++i) {
                    var n = m.normals[i];
                    n.Normalize();
                    m.normals[i] = n;
                }

                // Normals have been updated
                m.updated_normals = true;
            }
        }

        internal static void UpdateObjectMatrices(List<Object> objects, int start, int end) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= objects.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to Object
                Object o = objects[i];

                if (o == null || o.matrix_valid) {
                    continue;
                }

                var translation = Matrix4.CreateTranslation(o.position);
                var rotation = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(o.rotation));
                var scale = Matrix4.CreateScale(o.scale);

                o.transform = scale * rotation * translation;
                o.matrix_valid = true;
            }
        }
    }
}