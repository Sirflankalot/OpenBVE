using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using GLFunc = OpenTK.Graphics.OpenGL.GL;
using OpenBveApi;

namespace LibRender {
	public struct TextHandle {
		internal long id;
		internal TextHandle(long id) {
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

		internal TextHandle handle;

		internal Text Copy() {
			Text t = new Text() {
				font = font,
				color = color,
				location = location,
				texture_ready = texture_ready,
				visible = visible
			};
			if (texture_ready) {
				t.texture.AddRange(texture);
			}
			return t;
		}
	}

	public partial class Renderer {
		internal long texts_id = 0;
		internal Dictionary<long, int> texts_translation = new Dictionary<long, int>();
		internal List<Text> texts = new List<Text>();

		internal int AssertValid(TextHandle th) {
			int real;
			if (!texts_translation.TryGetValue(th.id, out real)) {
				throw new System.ArgumentException("Invalid TextHandle, no possible translation" + th.id.ToString());
			}
			if (texts.Count <= real) {
				throw new System.ArgumentException("Text Handle ID larger than array: " + th.id.ToString());
			}
			if (texts[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted text: " + th.id.ToString());
			}
			return real;
		}

		public TextHandle AddText(string text, Font font, Pixel color, Position location, int depth = 0, int max_width = 0) {
			Text t = new Text() {
				text = text,
				font = font,
				color = new Vector4(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f),
				location = location,
				depth = depth,
				max_width = max_width
			};

			long id = texts_id++;
			t.handle = new TextHandle(id);

			texts_translation.Add(id, texts.Count);
			texts.Add(t);
			return t.handle;
		}

		public void Delete(TextHandle th) {
			int id = AssertValid(th);

			GLFunc.DeleteTexture(texts[id].gl_tex_id);

			texts_translation.Remove(th.id);
			texts[id] = null;
		}

		//////////////////////////////
		// Text Setters and Getters //
		//////////////////////////////

		public string GetText(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].text;
		}

		public Font GetFont(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].font;
		}

		public int GetMaxWidth(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].max_width;
		}

		public int GetDepth(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].depth;
		}

		public Vector2 GetDimentions(TextHandle th) {
			int id = AssertValid(th);

			if (texts[id].texture_ready == false) {
				Algorithms.UpdateTextTextures(texts, id, id + 1, settings.text_rendering_quality);
			}

			return new Vector2(texts[id].width, texts[id].height);
		}

		public Pixel GetColor(TextHandle th) {
			int id = AssertValid(th);

			Vector4 orig = texts[id].color;
			return new Pixel { r = (byte) (orig.X * 255.0f), g = (byte) (orig.Y * 255.0f), b = (byte) (orig.Z * 255.0f), a = (byte) (orig.W * 255.0f) };
		}

		public Position GetLocation(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].location;
		}

		public bool GetVisibility(TextHandle th) {
			int id = AssertValid(th);

			return texts[id].visible;
		}

		public void SetText(TextHandle th, string text) {
			int id = AssertValid(th);

			texts[id].text = text;
			texts[id].texture_ready = false;
			texts[id].uploaded = false;
		}

		public void SetFont(TextHandle th, Font font) {
			int id = AssertValid(th);

			texts[id].font = font;
			texts[id].texture_ready = false;
			texts[id].uploaded = false;
		}

		public void SetMaxWidth(TextHandle th, int max_width) {
			int id = AssertValid(th);

			texts[id].max_width = max_width;
			texts[id].texture_ready = false;
			texts[id].uploaded = false;
		}

		public void SetDepth(TextHandle th, int depth) {
			int id = AssertValid(th);

			texts[id].depth = depth;
		}

		public void SetColor(TextHandle th, Pixel color) {
			int id = AssertValid(th);

			texts[id].color = new Vector4(color.r / 255.0f, color.g / 255.0f, color.b / 255.0f, color.a / 255.0f);
		}

		public void SetLocation(TextHandle th, Position location) {
			int id = AssertValid(th);

			texts[id].location = location;
		}

		public void SetVisibility(TextHandle th, bool visible) {
			int id = AssertValid(th);

			texts[id].visible = visible;
		}
	}
}