using OpenTK;
using System.Collections.Generic;

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

    public struct Camera_Handle {
        internal int id;
        internal Camera_Handle(int id) {
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
        internal List<Vector3> normals;

        internal bool updated_normals = false;

        internal int gl_id = 0;

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

    public struct Pixel {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }

    internal class Texture {
        internal List<Pixel> pixels;
        internal int width;
        internal int height;

        internal int gl_id = 0;

        public Texture Copy() {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            t.width = width;
            t.height = height;
            return t;
        }
    }

    internal class Object {
        internal int mesh_id;
        internal int tex_id;
        internal bool visible;
        internal Vector3 position;
        internal Vector3 rotation;
        internal Vector3 scale;
        internal Matrix4 transform;
        internal bool matrix_valid;
    }

    internal class Camera {
        internal Vector3 position;
        internal Vector2 rotation;
        internal Matrix4 transform;
        internal bool matrix_valid;
        internal float fov;
    }

    public partial class Renderer {
        internal List<Mesh> meshes;
        internal List<Texture> textures;
        internal List<Object> objects;
        internal List<Camera> cameras;
        internal int active_camera;

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

        internal void assert_valid(Camera_Handle ch) {
            if (cameras.Count <= ch.id) {
                throw new System.ArgumentException("Camera Handle ID larger than array: " + ch.id.ToString());
            }
            if (cameras[ch.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted camera: " + ch.id.ToString());
            }
        }

        public Mesh_Handle add_mesh(Vertex[] mesh, int[] vertex_indices, bool has_normals = false) {
            Mesh m = new Mesh();
            m.vertices.AddRange(mesh);
            m.indices.AddRange(vertex_indices);
            meshes.Add(m);
            return new Mesh_Handle(meshes.Count - 1);
        }

        public Texture_Handle add_texture(Pixel[] pixels, int width, int height) {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            t.width = width;
            t.height = height;
            textures.Add(t);
            return new Texture_Handle(textures.Count - 1);
        }

        public Object_Handle add_object(Mesh_Handle mh, Texture_Handle th, bool visible = true) {
            Object o = new Object();
            o.mesh_id = mh.id;
            o.tex_id = th.id;
            o.visible = visible;
            objects.Add(o);
            return new Object_Handle(objects.Count - 1);
        }

        public Camera_Handle add_camera(Vector3 location, Vector2 rotation, float fov, bool active = true) {
            Camera c = new Camera();
            c.position = location;
            c.rotation = rotation;
            c.fov = fov;
            cameras.Add(c);
            var index = cameras.Count - 1;
            if (active) {
                active_camera = index;
            }
            return new Camera_Handle(index);
        }

        public void delete(Mesh_Handle mh) {
            meshes[mh.id] = null;
        }

        public void delete(Texture_Handle th) {
            textures[th.id] = null;
        }

        public void delete(Object_Handle oh) {
            objects[oh.id] = null;
        }

        public void delete(Camera_Handle oh) {
            cameras[oh.id] = null;
        }

        ////////////////////////////////
        // Object Setters and Getters //
        ////////////////////////////////

        public bool get_visibility(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].visible;
        }

        public Vector3 get_location(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].position;
        }

        public Vector3 get_rotation(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].rotation;
        }

        public Vector3 get_scale(Object_Handle oh) {
            assert_valid(oh);

            return objects[oh.id].scale;
        }

        public void set_visibility(Object_Handle oh, bool visible) {
            assert_valid(oh);

            objects[oh.id].visible = visible;
        }

        public void set_location(Object_Handle oh, Vector3 pos) {
            assert_valid(oh);

            objects[oh.id].position = pos;
            objects[oh.id].matrix_valid = false;
        }

        public void set_rotation(Object_Handle oh, Vector3 rot) {
            assert_valid(oh);

            objects[oh.id].rotation = rot;
            objects[oh.id].matrix_valid = false;
        }

        public void set_scale(Object_Handle oh, Vector3 scale) {
            assert_valid(oh);

            objects[oh.id].scale = scale;
            objects[oh.id].matrix_valid = false;
        }

        ////////////////////////////////
        // Camera Setters and Getters //
        ////////////////////////////////

        public Vector3 get_location(Camera_Handle ch) {
            assert_valid(ch);

            return cameras[ch.id].position;
        }

        public Vector2 get_rotation(Camera_Handle ch) {
            assert_valid(ch);

            return cameras[ch.id].rotation;
        }

        public float get_fov(Camera_Handle ch) {
            assert_valid(ch);

            return cameras[ch.id].fov;
        }

        public Camera_Handle get_active_camera() {
            return new Camera_Handle(active_camera);
        }

        public void set_location(Camera_Handle ch, Vector3 location) {
            assert_valid(ch);

            cameras[ch.id].position = location;
        }

        public void set_rotation(Camera_Handle ch, Vector2 rotation) {
            assert_valid(ch);

            cameras[ch.id].rotation = rotation;
        }

        public void set_fov(Camera_Handle ch, float fov) {
            assert_valid(ch);

            cameras[ch.id].fov = fov;
        }

        public void set_active_camera(Camera_Handle ch) {
            assert_valid(ch);

            active_camera = ch.id;
        }


    }
}
