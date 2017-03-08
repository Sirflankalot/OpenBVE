using OpenTK;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    public struct Camera_Handle {
        internal int id;
        internal Camera_Handle(int id) {
            this.id = id;
        }
    }

    public struct Cone_Light_Handle {
        internal int id;
        internal Cone_Light_Handle(int id) {
            this.id = id;
        }
    }

    public struct Point_Light_Handle {
        internal int id;
        internal Point_Light_Handle(int id) {
            this.id = id;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 8)]
    public struct Vertex {
        public Vector3 position;
        public Vector2 tex_pos;
        public Vector3 normal;
    }

    internal class Mesh {
        internal List<Vertex> vertices = new List<Vertex>();
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

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct Pixel {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }

    internal class Texture {
        internal List<Pixel> pixels = new List<Pixel>();
        internal int width;
        internal int height;

        internal int gl_id = 0;
        internal bool uploaded = false;

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
        internal Vector3 scale = new Vector3(1, 1, 1);
        internal Matrix4 transform;
        internal bool matrix_valid;
    }

    internal class Camera {
        internal Vector3 focal_point;
        internal Vector2 rotation;
        internal float distance;
        internal Matrix4 transform = new Matrix4();
        internal bool matrix_valid = false;
        internal float fov;
    }

    internal class Cone_Light {
        internal Vector3 location;
        internal Vector3 direction;
        internal Matrix4 transform;
        internal bool matrix_valid = false;
        internal float brightness;
        internal float fov;
        internal bool shadow;
        internal int gl_tex_id = 0;
    }

    internal class Point_Light {
        internal Vector3 location;
        internal float brightness;
    }

    public partial class Renderer {
        internal List<Mesh> meshes = new List<Mesh>();
        internal List<Texture> textures = new List<Texture>();
        internal List<Object> objects = new List<Object>();
        internal List<Camera> cameras = new List<Camera>() { new Camera() { focal_point = new Vector3(0), rotation = new Vector2(0), distance=-3, fov = 50 }};
        internal int active_camera;
        internal List<Cone_Light> cone_lights = new List<Cone_Light>();
        internal List<Point_Light> point_lights = new List<Point_Light>();

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

        internal void AssertValid(Cone_Light_Handle clh) {
            if (cone_lights.Count <= clh.id) {
                throw new System.ArgumentException("Cone Light Handle ID larger than array: " + clh.id.ToString());
            }
            if (cone_lights[clh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted cone light: " + clh.id.ToString());
            }
        }

        internal void AssertValid(Point_Light_Handle plh) {
            if (point_lights.Count <= plh.id) {
                throw new System.ArgumentException("Point Light Handle ID larger than array: " + plh.id.ToString());
            }
            if (point_lights[plh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted point light: " + plh.id.ToString());
            }
        }

        ///////////////////////////////////
        // Adders, Updaters and Deleters //
        ///////////////////////////////////

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
            c.focal_point = location;
            c.rotation = rotation;
            c.fov = fov;
            cameras.Add(c);
            var index = cameras.Count - 1;
            if (active) {
                active_camera = index;
            }
            return new Camera_Handle(index);
        }

        public Cone_Light_Handle AddConeLight(Vector3 location, Vector3 direction, float brightness, float fov, bool shadow_casting = false) {
            Cone_Light cl = new Cone_Light();
            cl.location = location;
            cl.direction = direction;
            cl.brightness = brightness;
            cl.fov = fov;
            cl.shadow = shadow_casting;
            cone_lights.Add(cl);
            return new Cone_Light_Handle(cone_lights.Count - 1);
        }

        public Point_Light_Handle AddPointLight(Vector3 location, float brightness) {
            Point_Light pl = new Point_Light();
            pl.location = location;
            pl.brightness = brightness;
            point_lights.Add(pl);
            return new Point_Light_Handle(point_lights.Count - 1);
        }

        public void Update(Mesh_Handle mh, Vertex[] mesh, int[] vertex_indices) {
            AssertValid(mh);

            meshes[mh.id].vertices.Clear();
            meshes[mh.id].vertices.AddRange(mesh);
            meshes[mh.id].indices.Clear();
            meshes[mh.id].indices.AddRange(vertex_indices);
            meshes[mh.id].updated_normals = false;
            meshes[mh.id].uploaded = false;
        }

        public void Update(Texture_Handle th, Pixel[] pixels, int width, int height) {
            AssertValid(th);

            textures[th.id].pixels.Clear();
            textures[th.id].pixels.AddRange(pixels);
            textures[th.id].width = width;
            textures[th.id].height = height;
            textures[th.id].uploaded = false;
        }

        public void Update(Object_Handle oh, Mesh_Handle mh, Texture_Handle th) {
            AssertValid(oh);
            AssertValid(mh);
            AssertValid(th);

            objects[oh.id].mesh_id = mh.id;
            objects[oh.id].tex_id = th.id;
        }

        public void Delete(Mesh_Handle mh) {
            AssertValid(mh);

            GLFunc.DeleteVertexArray(meshes[mh.id].gl_vao_id);
            GLFunc.DeleteBuffer(meshes[mh.id].gl_vert_id);
            GLFunc.DeleteBuffer(meshes[mh.id].gl_indices_id);

            meshes[mh.id] = null;
        }

        public void Delete(Texture_Handle th) {
            AssertValid(th);

            GLFunc.DeleteTexture(textures[th.id].gl_id);

            textures[th.id] = null;
        }

        public void Delete(Object_Handle oh) {
            AssertValid(oh);

            objects[oh.id] = null;
        }

        public void Delete(Camera_Handle ch) {
            AssertValid(ch);

            if (ch.id == 0) {
                throw new System.ArgumentException("Cannot delete starting camera");
            }

            cameras[ch.id] = null;
        }

        public void Delete(Cone_Light_Handle clh) {
            AssertValid(clh);

            int tex = cone_lights[clh.id].gl_tex_id;
            if (tex != 0) {
                GLFunc.DeleteTexture(tex);
            }

            cone_lights[clh.id] = null;
        }

        public void Delete(Point_Light_Handle plh) {
            AssertValid(plh);

            point_lights[plh.id] = null;
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

            return cameras[ch.id].focal_point;
        }

        public Vector2 GetRotation(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].rotation;
        }

        public float GetDistance(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].distance;
        }

        public float GetFOV(Camera_Handle ch) {
            AssertValid(ch);

            return cameras[ch.id].fov;
        }

        public Camera_Handle GetActiveCamera() {
            return new Camera_Handle(active_camera);
        }

        public Camera_Handle GetStartingCamera() {
            return new Camera_Handle(0);
        }

        public void SetLocation(Camera_Handle ch, Vector3 location) {
            AssertValid(ch);

            cameras[ch.id].focal_point = location;
            cameras[ch.id].matrix_valid = false;
        }

        public void SetRotation(Camera_Handle ch, Vector2 rotation) {
            AssertValid(ch);

            cameras[ch.id].rotation = rotation;
            cameras[ch.id].matrix_valid = false;
        }

        public void SetDistance(Camera_Handle ch, float distance) {
            AssertValid(ch);

            cameras[ch.id].distance = distance;
            cameras[ch.id].matrix_valid = false;
        }

        public void SetFOV(Camera_Handle ch, float fov) {
            AssertValid(ch);

            cameras[ch.id].fov = fov;
            cameras[ch.id].matrix_valid = false;
        }

        public void SetActiveCamera(Camera_Handle ch) {
            AssertValid(ch);

            active_camera = ch.id;
            cameras[ch.id].matrix_valid = false;
        }

        ////////////////////////////////////
        // Cone Light Getters and Setters //
        ////////////////////////////////////

        public Vector3 GetLocation(Cone_Light_Handle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].location;
        }

        public Vector3 GetDirection(Cone_Light_Handle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].direction;
        }

        public float GetFOV(Cone_Light_Handle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].fov;
        }

        public float GetBrightness(Cone_Light_Handle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].brightness;
        }

        public bool GetShadow(Cone_Light_Handle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].shadow;
        }

        public void SetLocation(Cone_Light_Handle clh, Vector3 location) {
            AssertValid(clh);

            cone_lights[clh.id].location = location;
        }

        public void SetDirection(Cone_Light_Handle clh, Vector3 direction) {
            AssertValid(clh);

            cone_lights[clh.id].direction = direction;
        }

        public void SetFOV(Cone_Light_Handle clh, float fov) {
            AssertValid(clh);

            cone_lights[clh.id].fov = fov;
        }

        public void SetBrightness(Cone_Light_Handle clh, float brightness) {
            AssertValid(clh);

            cone_lights[clh.id].brightness = brightness;
        }

        public void SetShadow(Cone_Light_Handle clh, bool shadow) {
            AssertValid(clh);

            cone_lights[clh.id].shadow = shadow;
        }

        ///////////////////////////////////////
        /// Point Light Getters and Setters ///
        ///////////////////////////////////////

        public Vector3 GetLocation(Point_Light_Handle plh) {
            AssertValid(plh);

            return point_lights[plh.id].location;
        }

        public float GetBrightness(Point_Light_Handle plh) {
            AssertValid(plh);

            return point_lights[plh.id].brightness;
        }

        public void SetLocation(Point_Light_Handle plh, Vector3 location) {
            AssertValid(plh);

            point_lights[plh.id].location = location;
        }

        public void SetBrightness(Point_Light_Handle plh, float brightness) {
            AssertValid(plh);

            point_lights[plh.id].brightness = brightness;
        }
    }
}
