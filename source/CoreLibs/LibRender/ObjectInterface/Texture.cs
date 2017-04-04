using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct TextureHandle {
		internal long id;
		internal TextureHandle(long id) {
			this.id = id;
		}
	}

	internal class Texture {
		internal List<Pixel> pixels = new List<Pixel>();
		internal int width;
		internal int height;

		internal int gl_id = 0;
		internal bool uploaded = false;

		internal bool has_transparancy = false;

		internal TextureHandle handle;

		public Texture Copy() {
			Texture t = new Texture();
			t.pixels.AddRange(pixels);
			t.width = width;
			t.height = height;
			t.has_transparancy = has_transparancy;
			return t;
		}
	}

	public partial class Renderer {
		internal long textures_id = 0;
		internal Dictionary<long, int> textures_translation = new Dictionary<long, int>();
		internal List<Texture> textures = new List<Texture>();

		internal int AssertValid(TextureHandle th) {
			int real;
			if (!textures_translation.TryGetValue(th.id, out real)) {
				throw new System.ArgumentException("Invalid TextureHandle, no possible translation" + th.id.ToString());
			}
			if (textures.Count <= real) {
				throw new System.ArgumentException("Texture Handle ID larger than array: " + th.id.ToString());
			}
			if (textures[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted texture: " + th.id.ToString());
			}
			return real;
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

			long id = textures_id++;
			t.handle = new TextureHandle(id);

			textures_translation.Add(id, textures.Count);
			textures.Add(t);
			return t.handle;
		}

		public void Update(TextureHandle th, Pixel[] pixels, int width, int height) {
			int id = AssertValid(th);

			textures[id].pixels.Clear();
			textures[id].pixels.AddRange(pixels);
			textures[id].width = width;
			textures[id].height = height;
			// Find transparency
			foreach (Pixel p in textures[id].pixels) {
				if (p.a < 255) {
					textures[id].has_transparancy = true;
					break;
				}
			}
			textures[id].uploaded = false;
		}

		public void Delete(TextureHandle th) {
			int id = AssertValid(th);

			GLFunc.DeleteTexture(textures[id].gl_id);

			textures_translation.Remove(th.id);
			textures[id] = null;
		}

		///////////////////////////////
		// Texture Utility Functions //
		///////////////////////////////

		public TextureHandle AddTextureFromColor(Pixel color) {
			return AddTexture(new Pixel[] { color }, 1, 1);
		}
	}
}