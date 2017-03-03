using OpenTK;
using System.Collections.Generic;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender
{
    public partial class Renderer {
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
            Vector3 position;
            Vector2 tex_pos;
            Vector3 normal;
        }

        internal class Mesh {
            internal List<Vertex> vertices;

            public Mesh Copy() {
                Mesh m = new Mesh();
                m.vertices.AddRange(vertices);
                return m;
            }
        }

        public struct Pixel {
            byte r;
            byte g;
            byte b;
            byte a;
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

        internal List<Mesh> meshes;
        internal List<Texture> textures;
        internal List<Object> objects;

        Mesh_Handle add_mesh(Mesh mesh) {
            meshes.Add(mesh.Copy());
            return new Mesh_Handle(meshes.Count - 1);
        }

        Texture_Handle add_texture(Pixel[] pixels) {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            textures.Add(t);
            return new Texture_Handle(textures.Count - 1);
        }

        Object_Handle add_object(Mesh_Handle mh, Texture_Handle th) {
            Object o = new Object();
            o.mesh_id = mh.id;
            o.tex_id = th.id;
            objects.Add(o);
            return new Object_Handle(objects.Count - 1);
        }

        void delete_mesh(Mesh_Handle mh) {
            meshes[mh.id] = null;
        }

        void delete_texture(Texture_Handle th) {
            textures[th.id] = null;
        }

        void delete_object(Object_Handle oh) {
            objects[oh.id] = null;
        }
    }
}
