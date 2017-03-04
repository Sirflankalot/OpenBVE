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

        internal void AssertValid(Mesh_Handle mh) {
            if (meshes.Count <= mh.id) {
                throw new System.ArgumentException("Mesh Handle ID larger than array: " + mh.id.ToString());
            }
            if (meshes[mh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted mesh: " + mh.id.ToString());
            }
        }
        
        internal void AssertValid(Object_Handle oh) {
            if (objects.Count <= oh.id) {
                throw new System.ArgumentException("Object Handle ID larger than array: " + oh.id.ToString());
            }
            if (objects[oh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted object: " + oh.id.ToString());
            }
        }

        internal void AssertValid(Texture_Handle th) {
            if (textures.Count <= th.id) {
                throw new System.ArgumentException("Texture Handle ID larger than array: " + th.id.ToString());
            }
            if (textures[th.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted texture: " + th.id.ToString());
            }
        }

        internal void AssertValid(Camera_Handle ch) {
            if (cameras.Count <= ch.id) {
                throw new System.ArgumentException("Camera Handle ID larger than array: " + ch.id.ToString());
            }
            if (cameras[ch.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted camera: " + ch.id.ToString());
            }
        }

        public Mesh_Handle AddMesh(Vertex[] mesh, int[] vertex_indices) {
            Mesh m = new Mesh();
            m.vertices.AddRange(mesh);
            m.indices.AddRange(vertex_indices);
            meshes.Add(m);
            return new Mesh_Handle(meshes.Count - 1);
        }

        public Texture_Handle AddTexture(Pixel[] pixels, int width, int height) {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            t.width = width;
            t.height = height;
            textures.Add(t);
            return new Texture_Handle(textures.Count - 1);
        }

        public Object_Handle AddObject(Mesh_Handle mh, Texture_Handle th, bool visible = true) {
            Object o = new Object();
            o.mesh_id = mh.id;
            o.tex_id = th.id;
            o.visible = visible;
            objects.Add(o);
            return new Object_Handle(objects.Count - 1);
        }

        public Camera_Handle AddCamera(Vector3 location, Vector2 rotation, float fov, bool active = true) {
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

        public void Delete(Mesh_Handle mh) {
            meshes[mh.id] = null;
        }

        public void Delete(Texture_Handle th) {
            textures[th.id] = null;
        }

        public void Delete(Object_Handle oh) {
            objects[oh.id] = null;
        }

        public void Delete(Camera_Handle oh) {
            cameras[oh.id] = null;
        }

        ////////////////////////////////
        // Object Setters and Getters //
        ////////////////////////////////

        public bool GetVisibility(Object_Handle oh) {
            AssertValid(oh);

            return objects[oh.id].visible;
        }

        public Vector3 GetLocation(Object_Handle oh) {
            AssertValid(oh);

            return objects[oh.id].position;
        }

        public Vector3 GetRotation(Object_Handle oh) {
            AssertValid(oh);

            return objects[oh.id].rotation;
        }

        public Vector3 GetScale(Object_Handle oh) {
            AssertValid(oh);

            return objects[oh.id].scale;
        }

        public void SeVisibility(Object_Handle oh, bool visible) {
            AssertValid(oh);

            objects[oh.id].visible = visible;
        }

        public void SetLocation(Object_Handle oh, Vector3 pos) {
            AssertValid(oh);

            objects[oh.id].position = pos;
            objects[oh.id].matrix_valid = false;
        }

        public void SetRotation(Object_Handle oh, Vector3 rot) {
            AssertValid(oh);
            objects[oh.id].rotation = rot;
            objects[oh.id].matrix_valid = false;
        }

        public void SetScale(Object_Handle oh, Vector3 scale) {
            AssertValid(oh);

            objects[oh.id].scale = scale;
            objects[oh.id].matrix_valid = false;
        }

        ////////////////////////////////
        // Camera Setters and Getters //
        ////////////////////////////////

        public Vector3 GetLocation(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].position;
        }

        public Vector2 GetRotation(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].rotation;
        }

        public float GetFOV(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].fov;
        }

        public Camera_Handle GetActiveCamera() {
            return new Camera_Handle(active_camera);
        }

        public void SetLocation(Camera_Handle ch, Vector3 location) {
            AssertValid(ch);

            cameras[ch.id].position = location;
        }

        public void SetRotation(Camera_Handle ch, Vector2 rotation) {
            AssertValid(ch);

            cameras[ch.id].rotation = rotation;
        }

        public void SetFOV(Camera_Handle ch, float fov) {
            AssertValid(ch);

            cameras[ch.id].fov = fov;
        }

        public void SetActiveCamera(Camera_Handle ch) {
            AssertValid(ch);

            active_camera = ch.id;
        }


    }
}
