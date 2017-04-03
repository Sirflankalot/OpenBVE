using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct TextHandle {
		internal int id;
		internal TextHandle(int id) {
			this.id = id;
		}
	}

	internal class Text {
		internal Font font;
		internal string text;
		internal int max_width;
		internal int depth;

		internal Vector4 color;

		internal Position location;

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
			t.location = location;
			if (texture_ready) {
				t.texture.AddRange(texture);
			}
			t.texture_ready = texture_ready;
			t.visible = visible;
			return t;
		}
	}

	public partial class Renderer {
		internal List<Text> texts = new List<Text>();

		internal void AssertValid(TextHandle th) {
			if (texts.Count <= th.id) {
				throw new System.ArgumentException("Text Handle ID larger than array: " + th.id.ToString());
			}
			if (texts[th.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted text: " + th.id.ToString());
			}
		}

		public TextHandle AddText(string text, Font font, Pixel color, Position location, int depth = 0, int max_width = 0) {
			Text t = new Text();
			t.text = text;
			t.font = font;
			t.color = new Vector4(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f);
			t.location = location;
			t.depth = depth;
			t.max_width = max_width;
			texts.Add(t);
			return new TextHandle(texts.Count - 1);
		}

		public void Delete(TextHandle th) {
			AssertValid(th);

			GLFunc.DeleteTexture(texts[th.id].gl_tex_id);

			texts[th.id] = null;
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

		public Position GetLocation(TextHandle th) {
			AssertValid(th);

			return texts[th.id].location;
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

		public void SetLocation(TextHandle th, Position location) {
			AssertValid(th);

			texts[th.id].location = location;
		}

		public void SetVisibility(TextHandle th, bool visible) {
			AssertValid(th);

			texts[th.id].visible = visible;
		}
	}
}