using OpenTK;

namespace LibRender {
	public enum WindowOrigin {
		TopLeft,
		TopCenter,
		TopRight,
		CenterLeft,
		CenterCenter,
		CenterRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public enum ObjectOrigin {
		TopLeft,
		TopCenter,
		TopRight,
		CenterLeft,
		CenterCenter,
		CenterRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public struct Position {
		public WindowOrigin window_origin;
		public ObjectOrigin object_origin;
		public Vector2 position;

		public Position(Position old_pos) {
			window_origin = old_pos.window_origin;
			object_origin = old_pos.object_origin;
			position = old_pos.position;
		}

		public Position(WindowOrigin window_origin, ObjectOrigin object_origin, Vector2 position) {
			this.window_origin = window_origin;
			this.object_origin = object_origin;
			this.position = position;
		}

		public Position(WindowOrigin window_origin, ObjectOrigin object_origin, float pos_x, float pos_y) {
			this.window_origin = window_origin;
			this.object_origin = object_origin;
			position = new Vector2(pos_x, pos_y);
		}

		internal void Translate(WindowOrigin window_origin, ObjectOrigin object_origin, int width, int height, Vector2 object_dimentions) {
			// Translation works by resetting the coordinates to TopLeft/TopLeft
			// then moving the coordinates from TopLeft/TopLeft to whatever is required

			// Translate object origin to TopLeft
			switch (this.object_origin) {
				case ObjectOrigin.TopLeft:
					break;
				case ObjectOrigin.TopCenter:
					position.X -= object_dimentions.X / 2.0f;
					break;
				case ObjectOrigin.TopRight:
					position.X -= object_dimentions.X;
					break;
				case ObjectOrigin.CenterLeft:
					position.Y -= object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.CenterCenter:
					position.X -= object_dimentions.X / 2.0f;
					position.Y -= object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.CenterRight:
					position.X -= object_dimentions.X;
					position.Y -= object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.BottomLeft:
					position.Y -= object_dimentions.Y;
					break;
				case ObjectOrigin.BottomCenter:
					position.X -= object_dimentions.X / 2.0f;
					position.Y -= object_dimentions.Y;
					break;
				case ObjectOrigin.BottomRight:
					position.X -= object_dimentions.X;
					position.Y -= object_dimentions.Y;
					break;
			}

			// Translate window origin to TopLeft
			switch (this.window_origin) {
				case WindowOrigin.TopLeft:
					break;
				case WindowOrigin.TopCenter:
					position.X += width / 2.0f;
					break;
				case WindowOrigin.TopRight:
					position.X += width;
					break;
				case WindowOrigin.CenterLeft:
					position.Y += height / 2.0f;
					break;
				case WindowOrigin.CenterCenter:
					position.X += width / 2.0f;
					position.Y += height / 2.0f;
					break;
				case WindowOrigin.CenterRight:
					position.X += width;
					position.Y += height / 2.0f;
					break;
				case WindowOrigin.BottomLeft:
					position.Y += height;
					break;
				case WindowOrigin.BottomCenter:
					position.X += width / 2.0f;
					position.Y += height;
					break;
				case WindowOrigin.BottomRight:
					position.X += width;
					position.Y += height;
					break;
			}

			// Translate object origin from TopLeft to whatever
			switch (object_origin) {
				case ObjectOrigin.TopLeft:
					break;
				case ObjectOrigin.TopCenter:
					position.X += object_dimentions.X / 2.0f;
					break;
				case ObjectOrigin.TopRight:
					position.X += object_dimentions.X;
					break;
				case ObjectOrigin.CenterLeft:
					position.Y += object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.CenterCenter:
					position.X += object_dimentions.X / 2.0f;
					position.Y += object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.CenterRight:
					position.X += object_dimentions.X;
					position.Y += object_dimentions.Y / 2.0f;
					break;
				case ObjectOrigin.BottomLeft:
					position.Y += object_dimentions.Y;
					break;
				case ObjectOrigin.BottomCenter:
					position.X += object_dimentions.X / 2.0f;
					position.Y += object_dimentions.Y;
					break;
				case ObjectOrigin.BottomRight:
					position.X += object_dimentions.X;
					position.Y += object_dimentions.Y;
					break;
			}

			// Translate window origin from TopLeft to whatever
			switch (window_origin) {
				case WindowOrigin.TopLeft:
					break;
				case WindowOrigin.TopCenter:
					position.X -= width / 2.0f;
					break;
				case WindowOrigin.TopRight:
					position.X -= width;
					break;
				case WindowOrigin.CenterLeft:
					position.Y -= height / 2.0f;
					break;
				case WindowOrigin.CenterCenter:
					position.X -= width / 2.0f;
					position.Y -= height / 2.0f;
					break;
				case WindowOrigin.CenterRight:
					position.X -= width;
					position.Y -= height / 2.0f;
					break;
				case WindowOrigin.BottomLeft:
					position.Y -= height;
					break;
				case WindowOrigin.BottomCenter:
					position.X -= width / 2.0f;
					position.Y -= height;
					break;
				case WindowOrigin.BottomRight:
					position.X -= width;
					position.Y -= height;
					break;
			}

			// Set internal flags
			this.window_origin = window_origin;
			this.object_origin = object_origin;
		}

		internal Position Translated(WindowOrigin window_origin, ObjectOrigin object_origin, int width, int height, Vector2 object_dimentions) {
			Position new_pos = new Position(this);
			new_pos.Translate(window_origin, object_origin, width, height, object_dimentions);
			return new_pos;
		}
	}
}
