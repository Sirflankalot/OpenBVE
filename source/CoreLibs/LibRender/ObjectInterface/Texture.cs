using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct TextureHandle {
		internal int id;
		internal TextureHandle(int id) {
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
		internal List<Texture> textures = new List<Texture>();

		internal void AssertValid(TextureHandle th) {
			if (textures.Count <= th.id) {
				throw new System.ArgumentException("Texture Handle ID larger than array: " + th.id.ToString());
			}
			if (textures[th.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted texture: " + th.id.ToString());
			}
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

		public void Delete(TextureHandle th) {
			AssertValid(th);

			GLFunc.DeleteTexture(textures[th.id].gl_id);

			textures[th.id] = null;
		}
	}
}