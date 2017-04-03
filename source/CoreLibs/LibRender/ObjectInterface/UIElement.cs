using OpenTK;
using System.Collections.Generic;

namespace LibRender {
	public struct UIElementHandle {
		internal int id;
		internal UIElementHandle(int id) {
			this.id = id;
		}
	}

	internal class UIElement {
		internal int flatmesh_id;
		internal int tex_id;
		internal Position location;
		internal Vector2 scale;
		internal float rotation;
		internal int depth;
		internal Matrix2 transform = new Matrix2();
		internal bool matrix_valid;
		internal bool visible = true;
	}

	public partial class Renderer {
		internal List<UIElement> uielements = new List<UIElement>();

		internal void AssertValid(UIElementHandle uieh) {
			if (uielements.Count <= uieh.id) {
				throw new System.ArgumentException("UI Element Handle ID larger than array: " + uieh.id.ToString());
			}

			if (uielements[uieh.id] == null) {
				throw new System.ArgumentNullException("Accessing a deleted uielement: " + uieh.id.ToString());
			}
		}

		public UIElementHandle AddUIElement(FlatMeshHandle fmh, TextureHandle th, Position location, Vector2 scale, float rotation = 0, int depth = 0) {
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

		public void Update(UIElementHandle uieh, FlatMeshHandle fmh, TextureHandle th) {
			AssertValid(uieh);
			AssertValid(fmh);
			AssertValid(th);

			uielements[uieh.id].flatmesh_id = fmh.id;
			uielements[uieh.id].tex_id = th.id;
		}

		public void Delete(UIElementHandle uieh) {
			AssertValid(uieh);

			uielements[uieh.id] = null;
		}

		///////////////////////////////////
		// UIElement Setters and Getters //
		///////////////////////////////////

		public Position GetLocation(UIElementHandle uieh) {
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

		public void SetLocation(UIElementHandle uieh, Position location) {
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