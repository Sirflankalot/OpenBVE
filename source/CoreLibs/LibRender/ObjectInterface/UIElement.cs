using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct UIElementHandle {
		internal long id;
		internal UIElementHandle(long id) {
			this.id = id;
		}
	}

	internal class UIElement {
		internal FlatMeshHandle flatmesh;
		internal TextureHandle texture;
		internal Position location;
		internal Vector2 scale;
		internal float rotation;
		internal int depth;
		internal Matrix2 transform = new Matrix2();
		internal bool matrix_valid;
		internal bool visible = true;

		internal UIElementHandle handle;
	}

	public partial class Renderer {
		internal long uielements_id = 0;
		internal Dictionary<long, int> uielements_translation = new Dictionary<long, int>();
		internal List<UIElement> uielements = new List<UIElement>();

		internal int AssertValid(UIElementHandle uieh) {
			int real;
			if (!uielements_translation.TryGetValue(uieh.id, out real)) {
				throw new System.ArgumentException("Invalid UIElementHandle, no possible translation" + uieh.id.ToString());
			}
			if (uielements.Count <= real) {
				throw new System.ArgumentException("UI Element Handle ID larger than array: " + uieh.id.ToString());
			}

			if (uielements[real] == null) {
				throw new System.ArgumentNullException("Accessing a deleted uielement: " + uieh.id.ToString());
			}
			return real;
		}

		public UIElementHandle AddUIElement(FlatMeshHandle fmh, TextureHandle th, Position location, Vector2 scale, float rotation = 0, int depth = 0) {
			AssertValid(fmh);
			AssertValid(th);

			UIElement uie = new UIElement() {
				flatmesh = fmh,
				texture = th,
				location = location,
				scale = scale,
				rotation = rotation,
				depth = depth
			};

			long id = uielements_id++;
			uie.handle = new UIElementHandle(id);

			uielements_translation.Add(id, uielements.Count);
			uielements.Add(uie);
			return uie.handle;
		}

		public void Update(UIElementHandle uieh, FlatMeshHandle fmh, TextureHandle th) {
			int id = AssertValid(uieh);
			AssertValid(fmh);
			AssertValid(th);

			uielements[id].flatmesh = fmh;
			uielements[id].texture = th;
		}

		public void Delete(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			uielements_translation.Remove(uieh.id);
			uielements[id] = null;
		}

        public bool Valid(UIElementHandle uih) {
            try {
                AssertValid(uih);
            }
            catch (System.Exception) {
                return false;
            }
            return true;
        }

        ///////////////////////////////////
        // UIElement Setters and Getters //
        ///////////////////////////////////

        public Position GetLocation(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			return uielements[id].location;
		}

		public Vector2 GetScale(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			return uielements[id].scale;
		}

		public float GetRotation(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			return uielements[id].rotation;
		}

		public int GetDepth(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			return uielements[id].depth;
		}

		public bool GetVisibility(UIElementHandle uieh) {
			int id = AssertValid(uieh);

			return uielements[id].visible;
		}

		public void SetLocation(UIElementHandle uieh, Position location) {
			int id = AssertValid(uieh);

			uielements[id].location = location;
		}

		public void SetScale(UIElementHandle uieh, Vector2 scale) {
			int id = AssertValid(uieh);

			uielements[id].scale = scale;
		}

		public void SetRotation(UIElementHandle uieh, float rotation) {
			int id = AssertValid(uieh);

			uielements[id].rotation = rotation;
			uielements[id].matrix_valid = false;
		}

		public void SetDepth(UIElementHandle uieh, int depth) {
			int id = AssertValid(uieh);

			uielements[id].depth = depth;
		}

		public void SetVisibility(UIElementHandle uieh, bool visible) {
			int id = AssertValid(uieh);

			uielements[id].visible = visible;
		}
	}
}