using OpenTK;
using System.Runtime.InteropServices;

namespace OpenBveApi {

	/// <summary>
	/// Represents a vertex in 3D space with a position, a texture position, and a normal
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 8)]
	public struct Vertex3D {
		/// <summary>The vertex's position in 3D space relative to the rest of the model </summary>
		public Vector3 position;
		/// <summary>Texture coordinate of the vertex from top left (0, 0) to bottom right (1, 1) </summary>
		public Vector2 tex_pos;
		/// <summary>Angle of the normal at the current vertex relative to the rest of the model</summary>
		public Vector3 normal;
	}

	/// <summary>
	/// Represents a RGBA8 pixel
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct Pixel {
		/// <summary>Red component of pixel (0-255)</summary>
		public byte r;
		/// <summary>Green component of pixel (0-255)</summary>
		public byte g;
		/// <summary>Blue component of pixel (0-255)</summary>
		public byte b;
		/// <summary>Alpha component of pixel (0-255). 255 is opaque.</summary>
		public byte a;
	}

	/// <summary>
	/// Represents a vertex in 2D space with a position and a texture position
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 4)]
	public struct Vertex2D {
		/// <summary>The vertex's position in 2D space relative to rest of the shape</summary>
		public Vector2 position;
		/// <summary>Texture coordinate of the vertex from top left (0, 0) to bottom right (1, 1) </summary>
		public Vector2 texcoord;
	}
}