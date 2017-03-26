using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace LibRender {
	public static class Tests {
		internal static Vertex[] mesh = new Vertex[] {
			new Vertex() { position = new Vector3(  1, -1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f, -0.5773f, -0.5773f )}, // Bottom, Left, Front
			new Vertex() { position = new Vector3(  1, -1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f, -0.5773f,  0.5773f ) }, // Bottom, Right, Front
			new Vertex() { position = new Vector3( -1, -1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f, -0.5773f,  0.5773f ) }, // Top, Right, Front
			new Vertex() { position = new Vector3( -1, -1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f, -0.5773f, -0.5773f ) }, // Top, Left, Front
			new Vertex() { position = new Vector3(  1,  1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f,  0.5773f, -0.5773f ) }, // Bottom, Left, Back
			new Vertex() { position = new Vector3(  1,  1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f,  0.5773f,  0.5773f ) }, // Bottom, Right, Back
			new Vertex() { position = new Vector3( -1,  1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f,  0.5773f,  0.5773f ) }, // Top, Right, Back
			new Vertex() { position = new Vector3( -1,  1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f,  0.5773f, -0.5773f ) }  // Top, Left, Back
		};

		internal static int[] indices = new int[] {
			1,3,0,
			7,5,4,
			4,1,0,
			5,2,1,
			2,7,3,
			0,7,4,
			1,2,3,
			7,6,5,
			4,5,1,
			5,6,2,
			2,6,7,
			0,3,7
		};

		internal static Pixel[] pixels = new Pixel[16] {
			new Pixel() { r= 68, g= 68, b= 68, a=255 },
			new Pixel() { r=232, g=232, b=232, a=255 },
			new Pixel() { r= 48, g= 48, b= 48, a=255 },
			new Pixel() { r=195, g=195, b=195, a=255 },
			new Pixel() { r= 53, g= 53, b= 53, a=255 },
			new Pixel() { r=169, g=169, b=169, a=255 },
			new Pixel() { r=209, g=209, b=209, a=155 },
			new Pixel() { r=150, g=150, b=150, a=255 },
			new Pixel() { r=186, g=186, b=186, a=255 },
			new Pixel() { r=  0, g=  0, b=  0, a=255 },
			new Pixel() { r=173, g=173, b=173, a=255 },
			new Pixel() { r= 25, g= 25, b= 25, a=255 },
			new Pixel() { r= 60, g= 60, b= 60, a=255 },
			new Pixel() { r=230, g=230, b=230, a=255 },
			new Pixel() { r= 70, g= 70, b= 70, a=255 },
			new Pixel() { r=185, g=185, b=185, a=255 },
		};

		private static int active_test;
		private static Renderer active_renderer;

		public static void InitializeTest(Renderer renderer, int num) {
			switch (num) {
				case 0:
					Test0.Initialize(renderer);
					break;
				case 1:
					Test1.Initialize(renderer);
					break;
				case 2:
					Test2.Initialize(renderer);
					break;
				default:
					throw new System.Exception("LibRender test #" + num.ToString() + " is not a valid test");
			}

			active_renderer = renderer;
			active_test = num;
		}

		public static void Render() {
			switch (active_test) {
				case 0:
					Test0.Render(active_renderer);
					break;
				case 1:
					Test1.Render(active_renderer);
					break;
				case 2:
					Test2.Render(active_renderer);
					break;
			}
		}
		
		public static class Test0 {
			private static Object_Handle oh;

			public static void Initialize(Renderer renderer) {
				var m = renderer.AddMesh(mesh, indices);
				var t = renderer.AddTexture(pixels, 4, 4);
				oh = renderer.AddObject(m, t);
				renderer.SetLocation(oh, new Vector3(0, 0, 0));
			}

			public static void Render(Renderer renderer) {
				var array = renderer.GetLocation(oh);
				array += new Vector3(0.0f, 0.01f, 0.0f);
				renderer.SetLocation(oh, array);
				var rot = renderer.GetRotation(oh);
				rot += new Vector3(0, 0.0f, 0.1f);
				renderer.SetRotation(oh, rot);
				var cam = renderer.GetStartingCamera();
				var cam_loc = renderer.GetLocation(cam);
				cam_loc.Z += 0.01f;
				renderer.SetLocation(cam, cam_loc);
			}
		}
		
		public static class Test1 {
			private static Object_Handle oh;

			public static void Initialize(Renderer renderer) {
				var m = renderer.AddMesh(mesh, indices);
				var t = renderer.AddTexture(pixels, 4, 4);
				oh = renderer.AddObject(m, t);
				renderer.SetLocation(oh, new Vector3(0, 0, 0));
			}

			public static void Render(Renderer renderer) {
				var cam = renderer.GetStartingCamera();
				var camrot = renderer.GetRotation(cam);
				camrot.X += 0.01f;
				renderer.SetRotation(cam, camrot);
			}
		}

		public static class Test2 {
			private static Object_Handle[] oh_list = new Object_Handle[16];

			public static void Initialize(Renderer renderer) {
				var m = renderer.AddMesh(mesh, indices);
				var t = renderer.AddTexture(pixels, 4, 4);

				for (int i = 0; i < 16; ++i) {
					oh_list[i] = renderer.AddObject(m, t);
					renderer.SetLocation(oh_list[i], new Vector3((i / 4) * 4 - 6, 0, (i % 4) * 4 - 6));
				}

				renderer.SetDistance(renderer.GetStartingCamera(), 20);
				renderer.SetRotation(renderer.GetStartingCamera(), new Vector2(0, 45.0f));

				renderer.SetSunLocation(new Vector2(0.0f, 45.0f));
			}

			public static void Render(Renderer renderer) {
				var cam = renderer.GetStartingCamera();
				var camrot = renderer.GetRotation(cam);
				camrot.X += 0.5f;
				renderer.SetRotation(cam, camrot);

				var sun = renderer.GetSunLocation();
				sun.X += 1.0f;
				renderer.SetSunLocation(sun);
			}
		}
	}
}
