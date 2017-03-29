using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace LibRender {
	public static class Tests {
		internal static Vertex3D[] mesh = new Vertex3D[] {
			new Vertex3D() { position = new Vector3(  1, -1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f, -0.5773f, -0.5773f )}, // Bottom, Left, Front
			new Vertex3D() { position = new Vector3(  1, -1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f, -0.5773f,  0.5773f ) }, // Bottom, Right, Front
			new Vertex3D() { position = new Vector3( -1, -1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f, -0.5773f,  0.5773f ) }, // Top, Right, Front
			new Vertex3D() { position = new Vector3( -1, -1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f, -0.5773f, -0.5773f ) }, // Top, Left, Front
			new Vertex3D() { position = new Vector3(  1,  1, -1 ), tex_pos = new Vector2(0, 0), normal = new Vector3(  0.5773f,  0.5773f, -0.5773f ) }, // Bottom, Left, Back
			new Vertex3D() { position = new Vector3(  1,  1,  1 ), tex_pos = new Vector2(1, 0), normal = new Vector3(  0.5773f,  0.5773f,  0.5773f ) }, // Bottom, Right, Back
			new Vertex3D() { position = new Vector3( -1,  1,  1 ), tex_pos = new Vector2(1, 1), normal = new Vector3( -0.5773f,  0.5773f,  0.5773f ) }, // Top, Right, Back
			new Vertex3D() { position = new Vector3( -1,  1, -1 ), tex_pos = new Vector2(0, 1), normal = new Vector3( -0.5773f,  0.5773f, -0.5773f ) }  // Top, Left, Back
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

		internal static Vertex2D[] ui_panel = new Vertex2D[] {
			new Vertex2D() { position = new Vector2(-1, 1), texcoord = new Vector2(0, 0) },
			new Vertex2D() { position = new Vector2( 0, 0.5f), texcoord = new Vector2(0.5f, 0.25f) },
			new Vertex2D() { position = new Vector2( 1, 0.25f), texcoord = new Vector2(1, 0.375f) },
			new Vertex2D() { position = new Vector2(-1, -1), texcoord = new Vector2(0,  1) },
			new Vertex2D() { position = new Vector2( 1, -1), texcoord = new Vector2(1,  1) },
		};

		internal static int[] ui_panel_index = new int[] {
			0, 1, 3,
			1, 2, 3,
			2, 3, 4
		};

		internal static Pixel[] ui_panel_tex = new Pixel[] {
			new Pixel() { r=255, g=  0, b=  0, a=255},
			new Pixel() { r=  0, g=255, b=  0, a=255},
			new Pixel() { r=  0, g=  0, b=255, a=255},
			new Pixel() { r=255, g=255, b=255, a=255},
			new Pixel() { r=255, g=  0, b=  0, a=128},
			new Pixel() { r=  0, g=255, b=  0, a=128},
			new Pixel() { r=  0, g=  0, b=255, a=128},
			new Pixel() { r=255, g=255, b=255, a=128}
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
			private static ObjectHandle oh;

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
			private static ObjectHandle oh;

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
			private static ObjectHandle[] oh_list = new ObjectHandle[16];
			private static TextHandle one;
			private static TextHandle two;
			private static UIElementHandle rainbows;

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

				Font f = new Font(FontFamily.GenericSansSerif, 50, FontStyle.Regular, GraphicsUnit.Pixel);

				one = renderer.AddText("Good morning from the lovely state of New York!", f, new Pixel{ r=255, g=255, b=255, a=255}, new Vector2(0, 0), 0, renderer.width);
				two = renderer.AddText("Frame: 0", f, new Pixel { r = 255, g = 255, b = 255, a = 255 }, new Vector2(0, renderer.GetDimentions(one).Y), 0);

				var fm = renderer.AddFlatMesh(ui_panel, ui_panel_index);
				var uit = renderer.AddTexture(ui_panel_tex, 4, 2);
				rainbows = renderer.AddUIElement(fm, uit, new Vector2(0, 0), new Vector2(75), 0, 1);
			}

			static int frames = 0;
			public static void Render(Renderer renderer) {
				++frames;

				var cam = renderer.GetStartingCamera();
				var camrot = renderer.GetRotation(cam);
				camrot.X += 0.05f;
				renderer.SetRotation(cam, camrot);

				var sun = renderer.GetSunLocation();
				sun.X += 0.1f;
				renderer.SetSunLocation(sun);

				renderer.SetText(two, "Frame: " + frames.ToString());
				renderer.SetColor(two, new Pixel { r = (byte) (frames / 20 % 255), g = (byte) ((frames / 20 + 128) % 255), b = 123, a = 255 });

				if (frames % 100 == 0) {
					renderer.SetVisibility(rainbows, !renderer.GetVisibility(rainbows));
				}

				if (frames % 512 == 0) {
					renderer.SetVisibility(two, !renderer.GetVisibility(two));
				}
			}
		}
	}
}
