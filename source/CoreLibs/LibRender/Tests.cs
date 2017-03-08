using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace LibRender {
    public static class Tests {
        public static Object_Handle oh;

        public static void Test(Renderer renderer) {
            Vertex[] mesh = new Vertex[] {
                new Vertex() { position = new Vector3( -1, -1,  1 ), tex_pos = new Vector2(0, 0) }, // Bottom, Left, Front
                new Vertex() { position = new Vector3( -1,  1,  1 ), tex_pos = new Vector2(1, 0) }, // Bottom, Right, Front
                new Vertex() { position = new Vector3(  1,  1,  1 ), tex_pos = new Vector2(1, 1) }, // Top, Right, Front
                new Vertex() { position = new Vector3(  1, -1,  1 ), tex_pos = new Vector2(0, 1) }, // Top, Left, Front
                new Vertex() { position = new Vector3( -1, -1, -1 ), tex_pos = new Vector2(0, 0) }, // Bottom, Left, Back
                new Vertex() { position = new Vector3( -1,  1, -1 ), tex_pos = new Vector2(1, 0) }, // Bottom, Right, Back
                new Vertex() { position = new Vector3(  1,  1, -1 ), tex_pos = new Vector2(1, 1) }, // Top, Right, Back
                new Vertex() { position = new Vector3(  1, -1, -1 ), tex_pos = new Vector2(0, 1) }  // Top, Left, Back
            };

            int[] indices = new int[] {
                0,2,1,  0,3,2,
                4,3,0,  4,7,3,
                4,1,5,  4,0,1,
                3,6,2,  3,7,6,
                1,6,5,  1,2,6,
                7,5,6,  7,4,5
            };

            Pixel[] pixels = new Pixel[16] {
                new Pixel() { r= 68, g= 68, b= 68, a=255 },
                new Pixel() { r=232, g=232, b=232, a=255 },
                new Pixel() { r= 48, g= 48, b= 48, a=255 },
                new Pixel() { r=195, g=195, b=195, a=255 },
                new Pixel() { r= 53, g= 53, b= 53, a=255 },
                new Pixel() { r=169, g=169, b=169, a=255 },
                new Pixel() { r=209, g=209, b=209, a=255 },
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

            var m = renderer.AddMesh(mesh, indices);
            var t = renderer.AddTexture(pixels, 4, 4);
            oh = renderer.AddObject(m, t);
            renderer.SetLocation(oh, new Vector3(0, 0, 5));
        }

        public static void TestFrame(Renderer renderer) {
            var array = renderer.GetLocation(oh);
            array += new Vector3(0.0f, 0.01f, 0.0f);
            renderer.SetLocation(oh, array);
        }
    }
}
