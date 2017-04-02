using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public struct TextureHandle {
        internal int id;
        internal TextureHandle(int id) {
            this.id = id;
        }
    }

    public struct MeshHandle {
        internal int id;
        internal MeshHandle(int id) {
            this.id = id;
        }
    }

    public struct ObjectHandle {
        internal int id;
        internal ObjectHandle(int id) {
            this.id = id;
        }
    }

    public struct CameraHandle {
        internal int id;
        internal CameraHandle(int id) {
            this.id = id;
        }
    }

    public struct ConeLightHandle {
        internal int id;
        internal ConeLightHandle(int id) {
            this.id = id;
        }
    }

    public struct PointLightHandle {
        internal int id;
        internal PointLightHandle(int id) {
            this.id = id;
        }
    }

	public struct TextHandle {
		internal int id;
		internal TextHandle(int id) {
			this.id = id;
		}
	}

	public struct FlatMeshHandle {
		internal int id;
		internal FlatMeshHandle(int id) {
			this.id = id;
		}
	}

	public struct UIElementHandle {
		internal int id;
		internal UIElementHandle(int id) {
			this.id = id;
		}
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 8)]
    public struct Vertex3D {
        public Vector3 position;
        public Vector2 tex_pos;
        public Vector3 normal;
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

        internal bool has_transparancy = false;

        public Texture Copy() {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            t.width = width;
            t.height = height;
            t.has_transparancy = has_transparancy;
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
		internal bool matrix_valid = false;
		internal Matrix3 inverse_model_view_matrix;
		internal bool inverse_model_view_valid = false;
    }

    internal class Camera {
        internal Vector3 focal_point;
        internal Vector2 rotation;
        internal float distance;
        internal Matrix4 transform_matrix = new Matrix4();
		internal Matrix4 view_matrix = new Matrix4();
		internal Matrix4 proj_matrix = new Matrix4();
		internal Matrix4 inverse_projection_matrix = new Matrix4();
        internal bool matrix_valid = false;
        internal float fov;
    }

    internal class Cone_Light {
        internal Vector3 location;
        internal Vector3 direction;
        internal Matrix4 shadow_matrix;
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

	internal class Sun {
		internal Vector3 color = new Vector3(1.0f, 0.8f, 0.8f);
		internal Vector2 location = new Vector2(0.0f, 0.0f);
		internal float brightness = 1.0f;
		internal Vector3 direction;
		internal Matrix4 shadow_matrix;
		internal bool matrix_valid;
		internal int gl_tex_id = 0;
	}

	internal class Text {
		internal Font font;
		internal string text;
		internal int max_width;
		internal int depth;

		internal Vector4 color;

		internal Vector2 origin;

		internal List<Pixel> texture = new List<Pixel>();
        internal int width = 0;
        internal int height = 0;
		internal bool texture_ready = false;

		internal int gl_tex_id = 0;
		internal bool uploaded = false;

		internal bool visible = true;

		internal Text Copy() {
			Text t = new Text();
			t.font = font;
			t.color = color;
			t.origin = origin;
			if (texture_ready) {
				t.texture.AddRange(texture);
			}
			t.texture_ready = texture_ready;
			t.visible = visible;
			return t;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 4)]
	public struct Vertex2D {
		public Vector2 position;
		public Vector2 texcoord;
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

	internal class UIElement {
		internal int flatmesh_id;
		internal int tex_id;
		internal Vector2 location;
		internal Vector2 scale;
		internal float rotation;
		internal int depth;
		internal Matrix2 transform = new Matrix2();
		internal bool matrix_valid;
		internal bool visible = true; 
	}

    public partial class Renderer {
        internal List<Mesh> meshes = new List<Mesh>();
        internal List<Texture> textures = new List<Texture>();
        internal List<Object> objects = new List<Object>();
        internal List<Camera> cameras = new List<Camera>() { new Camera() { focal_point = new Vector3(0), rotation = new Vector2(0), distance=-10, fov = 50 }};
        internal int active_camera;
        internal List<Cone_Light> cone_lights = new List<Cone_Light>();
        internal List<Point_Light> point_lights = new List<Point_Light>();
		internal Sun sun = new Sun();
		internal List<Text> texts = new List<Text>();
		internal List<FlatMesh> flat_meshes = new List<FlatMesh>();
		internal List<UIElement> uielements = new List<UIElement>();

        internal void AssertValid(MeshHandle mh) {
            if (meshes.Count <= mh.id) {
                throw new System.ArgumentException("Mesh Handle ID larger than array: " + mh.id.ToString());
            }
            if (meshes[mh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted mesh: " + mh.id.ToString());
            }
        }
        
        internal void AssertValid(ObjectHandle oh) {
            if (objects.Count <= oh.id) {
                throw new System.ArgumentException("Object Handle ID larger than array: " + oh.id.ToString());
            }
            if (objects[oh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted object: " + oh.id.ToString());
            }
        }

        internal void AssertValid(TextureHandle th) {
            if (textures.Count <= th.id) {
                throw new System.ArgumentException("Texture Handle ID larger than array: " + th.id.ToString());
            }
            if (textures[th.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted texture: " + th.id.ToString());
            }
        }

        internal void AssertValid(CameraHandle ch) {
            if (cameras.Count <= ch.id) {
                throw new System.ArgumentException("Camera Handle ID larger than array: " + ch.id.ToString());
            }
            if (cameras[ch.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted camera: " + ch.id.ToString());
            }
        }

        internal void AssertValid(ConeLightHandle clh) {
            if (cone_lights.Count <= clh.id) {
                throw new System.ArgumentException("Cone Light Handle ID larger than array: " + clh.id.ToString());
            }
            if (cone_lights[clh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted cone light: " + clh.id.ToString());
            }
        }

        internal void AssertValid(PointLightHandle plh) {
            if (point_lights.Count <= plh.id) {
                throw new System.ArgumentException("Point Light Handle ID larger than array: " + plh.id.ToString());
            }
            if (point_lights[plh.id] == null) {
                throw new System.ArgumentNullException("Accessing a deleted point light: " + plh.id.ToString());
            }
        }

		internal void AssertValid(TextHandle th) {
			if (texts.Count <= th.id) {
				throw new System.ArgumentException("Text Handle ID larger than array: " + th.id.ToString());
			}
			if (texts[th.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted text: " + th.id.ToString());
			}
		}

		internal void AssertValid(FlatMeshHandle fmh) {
			if (flat_meshes.Count <= fmh.id) {
				throw new System.ArgumentException("Flat Mesh Handle ID larger than array: " + fmh.id.ToString());
			}

			if (flat_meshes[fmh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted flat mesh: " + fmh.id.ToString());
			}
		}

		internal void AssertValid(UIElementHandle uieh) {
			if (uielements.Count <= uieh.id) {
				throw new System.ArgumentException("UI Element Handle ID larger than array: " + uieh.id.ToString());
			}

			if (uielements[uieh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted uielement: " + uieh.id.ToString());
			}
		}

		///////////////////////////////////
		// Adders, Updaters and Deleters //
		///////////////////////////////////

		public MeshHandle AddMesh(Vertex3D[] mesh, int[] vertex_indices) {
            Mesh m = new Mesh();
            m.vertices.AddRange(mesh);
            m.indices.AddRange(vertex_indices);
            meshes.Add(m);
            return new MeshHandle(meshes.Count - 1);
        }

        public TextureHandle AddTexture(Pixel[] pixels, int width, int height) {
            Texture t = new Texture();
            t.pixels.AddRange(pixels);
            t.width = width;
            t.height = height;
            // Find transparency
            foreach (Pixel p in t.pixels) {
                if (p.a < 255) {
                    t.has_transparancy = true;
                    break;
                }
            }
            textures.Add(t);
            return new TextureHandle(textures.Count - 1);
        }

        public ObjectHandle AddObject(MeshHandle mh, TextureHandle th, bool visible = true) {
            Object o = new Object();
            o.mesh_id = mh.id;
            o.tex_id = th.id;
            o.visible = visible;
            objects.Add(o);
            return new ObjectHandle(objects.Count - 1);
        }

        public CameraHandle AddCamera(Vector3 location, Vector2 rotation, float fov, bool active = true) {
            Camera c = new Camera();
            c.focal_point = location;
            c.rotation = rotation;
            c.fov = fov;
            cameras.Add(c);
            var index = cameras.Count - 1;
            if (active) {
                active_camera = index;
            }
            return new CameraHandle(index);
        }

        public ConeLightHandle AddConeLight(Vector3 location, Vector3 direction, float brightness, float fov, bool shadow_casting = false) {
            Cone_Light cl = new Cone_Light();
            cl.location = location;
            cl.direction = direction;
            cl.brightness = brightness;
            cl.fov = fov;
            cl.shadow = shadow_casting;
            cone_lights.Add(cl);
            return new ConeLightHandle(cone_lights.Count - 1);
        }

        public PointLightHandle AddPointLight(Vector3 location, float brightness) {
            Point_Light pl = new Point_Light();
            pl.location = location;
            pl.brightness = brightness;
            point_lights.Add(pl);
            return new PointLightHandle(point_lights.Count - 1);
        }

		public TextHandle AddText(string text, Font font, Pixel color, Vector2 origin, int depth = 0, int max_width = 0) {
			Text t = new Text();
			t.text = text;
			t.font = font;
			t.color = new Vector4(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f);
			t.origin = origin;
			t.depth = depth;
			t.max_width = max_width;
			texts.Add(t);
			return new TextHandle(texts.Count - 1);
		}

		public FlatMeshHandle AddFlatMesh(Vertex2D[] mesh, int[] indices) {
			FlatMesh fm = new FlatMesh();
			fm.vertices.AddRange(mesh);
			fm.indices.AddRange(indices);
			flat_meshes.Add(fm);
			return new FlatMeshHandle(flat_meshes.Count - 1);
		}

		public UIElementHandle AddUIElement(FlatMeshHandle fmh, TextureHandle th, Vector2 location, Vector2 scale, float rotation = 0, int depth = 0) {
			AssertValid(fmh);
			AssertValid(th);

			UIElement uie = new UIElement();
			uie.flatmesh_id = fmh.id;
			uie.tex_id = th.id;
			uie.location = location;
			uie.scale = scale;
			uie.rotation = rotation;
			uie.depth = depth;
			uielements.Add(uie);
			return new UIElementHandle(uielements.Count - 1);
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

        public void Update(TextureHandle th, Pixel[] pixels, int width, int height) {
            AssertValid(th);

            textures[th.id].pixels.Clear();
            textures[th.id].pixels.AddRange(pixels);
            textures[th.id].width = width;
            textures[th.id].height = height;
            // Find transparency
            foreach (Pixel p in textures[th.id].pixels) {
                if (p.a < 255) {
                    textures[th.id].has_transparancy = true;
                    break;
                }
            }
            textures[th.id].uploaded = false;
        }

        public void Update(ObjectHandle oh, MeshHandle mh, TextureHandle th) {
            AssertValid(oh);
            AssertValid(mh);
            AssertValid(th);

            objects[oh.id].mesh_id = mh.id;
            objects[oh.id].tex_id = th.id;
        }

		public void Update(FlatMeshHandle fmh, Vertex2D[] mesh, int[] indices) {
			AssertValid(fmh);

			flat_meshes[fmh.id].vertices.Clear();
			flat_meshes[fmh.id].vertices.AddRange(mesh);
			flat_meshes[fmh.id].indices.Clear();
			flat_meshes[fmh.id].indices.AddRange(indices);
			flat_meshes[fmh.id].uploaded = false;
		}

		public void Update(UIElementHandle uieh, FlatMeshHandle fmh, TextureHandle th) {
			AssertValid(uieh);
			AssertValid(fmh);
			AssertValid(th);

			uielements[uieh.id].flatmesh_id = fmh.id;
			uielements[uieh.id].tex_id = th.id;
		}

        public void Delete(MeshHandle mh) {
            AssertValid(mh);

            GLFunc.DeleteVertexArray(meshes[mh.id].gl_vao_id);
            GLFunc.DeleteBuffer(meshes[mh.id].gl_vert_id);
            GLFunc.DeleteBuffer(meshes[mh.id].gl_indices_id);

            meshes[mh.id] = null;
        }

        public void Delete(TextureHandle th) {
            AssertValid(th);

            GLFunc.DeleteTexture(textures[th.id].gl_id);

            textures[th.id] = null;
        }

        public void Delete(ObjectHandle oh) {
            AssertValid(oh);

            objects[oh.id] = null;
        }

        public void Delete(CameraHandle ch) {
            AssertValid(ch);

            if (ch.id == 0) {
                throw new System.ArgumentException("Cannot delete starting camera");
            }

            cameras[ch.id] = null;
        }

        public void Delete(ConeLightHandle clh) {
            AssertValid(clh);

            int tex = cone_lights[clh.id].gl_tex_id;
            if (tex != 0) {
                GLFunc.DeleteTexture(tex);
            }

            cone_lights[clh.id] = null;
        }

        public void Delete(PointLightHandle plh) {
            AssertValid(plh);

            point_lights[plh.id] = null;
        }

		public void Delete(TextHandle th) {
			AssertValid(th);

			GLFunc.DeleteTexture(texts[th.id].gl_tex_id);

			texts[th.id] = null;
		}

		public void Delete(FlatMeshHandle fmh) {
			AssertValid(fmh);

			GLFunc.DeleteVertexArray(flat_meshes[fmh.id].gl_vao_id);
			GLFunc.DeleteBuffer(flat_meshes[fmh.id].gl_vert_id);
			GLFunc.DeleteBuffer(flat_meshes[fmh.id].gl_indices_id);

			flat_meshes[fmh.id] = null;
		}

		public void Delete(UIElementHandle uieh) {
			AssertValid(uieh);

			uielements[uieh.id] = null;
		}

        ////////////////////////////////
        // Object Setters and Getters //
        ////////////////////////////////

        public bool GetVisibility(ObjectHandle oh) {
            AssertValid(oh);

            return objects[oh.id].visible;
        }

        public Vector3 GetLocation(ObjectHandle oh) {
            AssertValid(oh);

            return objects[oh.id].position;
        }

        public Vector3 GetRotation(ObjectHandle oh) {
            AssertValid(oh);

            return objects[oh.id].rotation;
        }

        public Vector3 GetScale(ObjectHandle oh) {
            AssertValid(oh);

            return objects[oh.id].scale;
        }

        public void SetVisibility(ObjectHandle oh, bool visible) {
            AssertValid(oh);

            objects[oh.id].visible = visible;
        }

        public void SetLocation(ObjectHandle oh, Vector3 pos) {
            AssertValid(oh);

            objects[oh.id].position = pos;
			objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}

        public void SetRotation(ObjectHandle oh, Vector3 rot) {
            AssertValid(oh);
            objects[oh.id].rotation = rot;
            objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}

        public void SetScale(ObjectHandle oh, Vector3 scale) {
            AssertValid(oh);

            objects[oh.id].scale = scale;
            objects[oh.id].matrix_valid = false;
			objects[oh.id].inverse_model_view_valid = false;
		}

        ////////////////////////////////
        // Camera Setters and Getters //
        ////////////////////////////////

        public Vector3 GetLocation(CameraHandle ch) {
            AssertValid(ch);

            return cameras[ch.id].focal_point;
        }

        public Vector2 GetRotation(CameraHandle ch) {
            AssertValid(ch);

            return cameras[ch.id].rotation;
        }

        public float GetDistance(CameraHandle ch) {
            AssertValid(ch);

            return cameras[ch.id].distance;
        }

        public float GetFOV(CameraHandle ch) {
            AssertValid(ch);

            return cameras[ch.id].fov;
        }

        public CameraHandle GetActiveCamera() {
            return new CameraHandle(active_camera);
        }

        public CameraHandle GetStartingCamera() {
            return new CameraHandle(0);
        }

        public void SetLocation(CameraHandle ch, Vector3 location) {
            AssertValid(ch);

            cameras[ch.id].focal_point = location;
            cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
        }

        public void SetRotation(CameraHandle ch, Vector2 rotation) {
            AssertValid(ch);

            cameras[ch.id].rotation = rotation;
            cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

        public void SetDistance(CameraHandle ch, float distance) {
            AssertValid(ch);

            cameras[ch.id].distance = distance;
            cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

        public void SetFOV(CameraHandle ch, float fov) {
            AssertValid(ch);

            cameras[ch.id].fov = fov;
            cameras[ch.id].matrix_valid = false;
        }

        public void SetActiveCamera(CameraHandle ch) {
            AssertValid(ch);

            active_camera = ch.id;
            cameras[ch.id].matrix_valid = false;
			Algorithms.ClearObjectModelViewMatrices(objects, 0, objects.Count);
		}

        ////////////////////////////////////
        // Cone Light Getters and Setters //
        ////////////////////////////////////

        public Vector3 GetLocation(ConeLightHandle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].location;
        }

        public Vector3 GetDirection(ConeLightHandle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].direction;
        }

        public float GetFOV(ConeLightHandle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].fov;
        }

        public float GetBrightness(ConeLightHandle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].brightness;
        }

        public bool GetShadow(ConeLightHandle clh) {
            AssertValid(clh);

            return cone_lights[clh.id].shadow;
        }

        public void SetLocation(ConeLightHandle clh, Vector3 location) {
            AssertValid(clh);

            cone_lights[clh.id].location = location;
        }

        public void SetDirection(ConeLightHandle clh, Vector3 direction) {
            AssertValid(clh);

            cone_lights[clh.id].direction = direction;
        }

        public void SetFOV(ConeLightHandle clh, float fov) {
            AssertValid(clh);

            cone_lights[clh.id].fov = fov;
        }

        public void SetBrightness(ConeLightHandle clh, float brightness) {
            AssertValid(clh);

            cone_lights[clh.id].brightness = brightness;
        }

        public void SetShadow(ConeLightHandle clh, bool shadow) {
            AssertValid(clh);

            cone_lights[clh.id].shadow = shadow;
        }

        /////////////////////////////////////
        // Point Light Getters and Setters //
        /////////////////////////////////////

        public Vector3 GetLocation(PointLightHandle plh) {
            AssertValid(plh);

            return point_lights[plh.id].location;
        }

        public float GetBrightness(PointLightHandle plh) {
            AssertValid(plh);

            return point_lights[plh.id].brightness;
        }

        public void SetLocation(PointLightHandle plh, Vector3 location) {
            AssertValid(plh);

            point_lights[plh.id].location = location;
        }

        public void SetBrightness(PointLightHandle plh, float brightness) {
            AssertValid(plh);

            point_lights[plh.id].brightness = brightness;
        }

		/////////////////////////////
		// Sun Getters and Setters //
		/////////////////////////////

		public Vector3 GetSunColor() {
			return sun.color;
		}

		public Vector2 GetSunLocation() {
			return sun.location;
		}

		public float GetSunBrightness() {
			return sun.brightness;
		}

		public void SetSunColor(Vector3 color) {
			sun.color = color;
		}

		public void SetSunLocation(Vector2 location) {
			sun.location = location;
			sun.matrix_valid = false;
		}

		public void SetSunBrightness(float brightness) {
			sun.brightness = brightness;
		}

		//////////////////////////////
		// Text Setters and Getters //
		//////////////////////////////

		public string GetText(TextHandle th) {
			AssertValid(th);

			return texts[th.id].text;
		}

		public Font GetFont(TextHandle th) {
			AssertValid(th);

			return texts[th.id].font;
		}

		public int GetMaxWidth(TextHandle th) {
			AssertValid(th);

			return texts[th.id].max_width;
		}

		public int GetDepth(TextHandle th) {
			AssertValid(th);

			return texts[th.id].depth;
		}

		public Vector2 GetDimentions(TextHandle th) {
			AssertValid(th);

			if (texts[th.id].texture_ready == false) {
				Algorithms.UpdateTextTextures(texts, th.id, th.id + 1);
			}

			return new Vector2(texts[th.id].width, texts[th.id].height);
		}

		public Pixel GetColor(TextHandle th) {
			AssertValid(th);

			Vector4 orig = texts[th.id].color;
			return new Pixel { r = (byte) (orig.X * 255.0f), g = (byte) (orig.Y * 255.0f), b = (byte) (orig.Z * 255.0f), a = (byte) (orig.W * 255.0f) };
		}

		public Vector2 GetLocation(TextHandle th) {
			AssertValid(th);

			return texts[th.id].origin;
		}

		public bool GetVisibility(TextHandle th) {
			AssertValid(th);

			return texts[th.id].visible;
		}

		public void SetText(TextHandle th, string text) {
			AssertValid(th);

			texts[th.id].text = text;
			texts[th.id].texture_ready = false;
			texts[th.id].uploaded = false;
		}

		public void SetFont(TextHandle th, Font font) {
			AssertValid(th);

			texts[th.id].font = font;
			texts[th.id].texture_ready = false;
			texts[th.id].uploaded = false;
		}

		public void SetMaxWidth(TextHandle th, int max_width) {
			AssertValid(th);

			texts[th.id].max_width = max_width;
			texts[th.id].texture_ready = false;
			texts[th.id].uploaded = false;
		}

		public void SetDepth(TextHandle th, int depth) {
			AssertValid(th);

			texts[th.id].depth = depth;
		}

		public void SetColor(TextHandle th, Pixel color) {
			AssertValid(th);

			texts[th.id].color = new Vector4(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f);
		}

		public void SetLocation(TextHandle th, Vector2 location) {
			AssertValid(th);

			texts[th.id].origin = location;
		}

		public void SetVisibility(TextHandle th, bool visible) {
			AssertValid(th);

			texts[th.id].visible = visible;
		}

		///////////////////////////////////
		// UIElement Setters and Getters //
		///////////////////////////////////

		public Vector2 GetLocation(UIElementHandle uieh) {
			AssertValid(uieh);

			return uielements[uieh.id].location;
		}

		public Vector2 GetScale(UIElementHandle uieh) {
			AssertValid(uieh);

			return uielements[uieh.id].scale;
		}

		public float GetRotation(UIElementHandle uieh) {
			AssertValid(uieh);

			return uielements[uieh.id].rotation;
		}

		public int GetDepth(UIElementHandle uieh) {
			AssertValid(uieh);

			return uielements[uieh.id].depth;
		}

		public bool GetVisibility(UIElementHandle uieh) {
			AssertValid(uieh);

			return uielements[uieh.id].visible;
		}

		public void SetLocation(UIElementHandle uieh, Vector2 location) {
			AssertValid(uieh);

			uielements[uieh.id].location = location;
		}

		public void SetScale(UIElementHandle uieh, Vector2 scale) {
			AssertValid(uieh);

			uielements[uieh.id].scale = scale;
		}

		public void SetRotation(UIElementHandle uieh, float rotation) {
			AssertValid(uieh);

			uielements[uieh.id].rotation = rotation;
			uielements[uieh.id].matrix_valid = false;
		}

		public void SetDepth(UIElementHandle uieh, int depth) {
			AssertValid(uieh);

			uielements[uieh.id].depth = depth;
		}

		public void SetVisibility(UIElementHandle uieh, bool visible) {
			AssertValid(uieh);

			uielements[uieh.id].visible = visible;
		}
	}
}
