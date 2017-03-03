using OpenTK;
using System.Collections.Generic;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    
    public struct Texture_Handle {
        internal int id;
        internal Texture_Handle(int id) {
            this.id = id;
        }
    }

    public struct Mesh_Handle {
        internal int id;
        internal Mesh_Handle(int id) {
            this.id = id;
        }
    }

    public struct Object_Handle {
        internal int id;
        internal Object_Handle(int id) {
            this.id = id;
        }
    }

    public struct Vertex {
        public Vector3 position;
        public Vector2 tex_pos;
        public Vector3 normal;
    }

    internal class Mesh {
        internal List<Vertex> vertices;
        internal List<int> indices;

        public Mesh Copy() {
            Mesh m = new Mesh();
            m.vertices.AddRange(vertices);
            return m;
        }
    }

    public struct Pixel {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }

    internal class Texture {
        internal List<Pixel> pixels;

        public Texture Copy() {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            return t;
        }
    }

    internal class Object {
        internal int mesh_id;
        internal int tex_id;
        internal Vector3 position;
        internal Vector3 rotation;
        internal Vector3 scale;
        internal Matrix4 transform;
        internal bool valid;
    }

    public partial class Renderer {
        internal List<Mesh> meshes;
        internal List<Texture> textures;
        internal List<Object> objects;

        internal void assert_valid(Mesh_Handle mh) {
            if (meshes.Count <= mh.id) {
                throw new System.ArgumentException("Mesh Handle ID larger than array: " + mh.id.ToString());
            }
            if (meshes[mh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted mesh: " + mh.id.ToString());
            }
        }
        
        internal void assert_valid(Object_Handle oh) {
            if (objects.Count <= oh.id) {
                throw new System.ArgumentException("Object Handle ID larger than array: " + oh.id.ToString());
            }
            if (objects[oh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted object: " + oh.id.ToString());
            }
        }

        internal void assert_valid(Texture_Handle th) {
            if (textures.Count <= th.id) {
                throw new System.ArgumentException("Texture Handle ID larger than array: " + th.id.ToString());
            }
            if (textures[th.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted texture: " + th.id.ToString());
            }
        }

        public Mesh_Handle add_mesh(Vertex[] mesh, int[] vertex_indices, bool has_normals = false) {
            Mesh m = new Mesh();
            m.vertices.AddRange(mesh);
            m.indices.AddRange(vertex_indices);
            meshes.Add(m);
            return new Mesh_Handle(meshes.Count - 1);
        }

        public Texture_Handle add_texture(Pixel[] pixels) {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            textures.Add(t);
            return new Texture_Handle(textures.Count - 1);
        }

        public Object_Handle add_object(Mesh_Handle mh, Texture_Handle th) {
            Object o = new Object();
            o.mesh_id = mh.id;
            o.tex_id = th.id;
            objects.Add(o);
            return new Object_Handle(objects.Count - 1);
        }

        public void delete_mesh(Mesh_Handle mh) {
            meshes[mh.id] = null;
        }

        public void delete_texture(Texture_Handle th) {
            textures[th.id] = null;
        }

        public void delete_object(Object_Handle oh) {
            objects[oh.id] = null;
        }

        public Vector3 get_object_location(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].position;
        }

        public Vector3 get_object_rotation(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].rotation;
        }

        public Vector3 get_object_scale(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].scale;
        }

        public void set_object_location(Object_Handle oh, Vector3 pos) {
            assert_valid(oh);

            objects[oh.id].position = pos;
            objects[oh.id].valid = false;
        }

        public void set_object_rotation(Object_Handle oh, Vector3 rot) {
            assert_valid(oh);

            objects[oh.id].rotation = rot;
            objects[oh.id].valid = false;
        }

        public void set_object_scale(Object_Handle oh, Vector3 scale) {
            assert_valid(oh);

            objects[oh.id].scale = scale;
            objects[oh.id].valid = false;
        }
    }
}
