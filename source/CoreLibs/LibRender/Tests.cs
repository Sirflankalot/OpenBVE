using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace LibRender {
    public static class Tests {
        public static void test(Renderer renderer) {
            Vertex[] mesh = new Vertex[] {
                new Vertex() { position = new Vector3( -1, -1, -1 ) },
                new Vertex() { position = new Vector3( -1, -1,  1 ) },
                new Vertex() { position = new Vector3( -1,  1, -1 ) },
                new Vertex() { position = new Vector3( -1,  1,  1 ) },
                new Vertex() { position = new Vector3(  1, -1, -1 ) },
                new Vertex() { position = new Vector3(  1, -1,  1 ) },
                new Vertex() { position = new Vector3(  1,  1, -1 ) },
                new Vertex() { position = new Vector3(  1,  1,  1 ) }
            };

            int[] indices = new int[] {
                0, 1, 5,
                0, 5, 4,
                0, 3, 1,
                0, 2, 3,
                0, 4, 2,
                4, 7, 2,
                7, 3, 2,
                7, 6, 3,
                4, 5, 7,
                7, 5, 6,
                1, 3, 5,
                5, 3, 6
            };

            Pixel[] pixels = new Pixel[16] {
                new Pixel() { r= 68, g= 68, b= 68, a= 68 },
                new Pixel() { r=232, g=232, b=232, a=232 },
                new Pixel() { r= 48, g= 48, b= 48, a= 48 },
                new Pixel() { r=195, g=195, b=195, a=195 },
                new Pixel() { r= 53, g= 53, b= 53, a= 53 },
                new Pixel() { r=169, g=169, b=169, a=169 },
                new Pixel() { r=209, g=209, b=209, a=209 },
                new Pixel() { r=150, g=150, b=150, a=150 },
                new Pixel() { r=186, g=186, b=186, a=186 },
                new Pixel() { r=  0, g=  0, b=  0, a=  0 },
                new Pixel() { r=173, g=173, b=173, a=173 },
                new Pixel() { r= 25, g= 25, b= 25, a= 25 },
                new Pixel() { r= 60, g= 60, b= 60, a= 60 },
                new Pixel() { r=230, g=230, b=230, a=230 },
                new Pixel() { r= 70, g= 70, b= 70, a= 70 },
                new Pixel() { r=185, g=185, b=185, a=185 },
            };

            var m = renderer.AddMesh(mesh, indices);
            var t = renderer.AddTexture(pixels, 4, 4);
            renderer.AddObject(m, t);
        }
    }
}
