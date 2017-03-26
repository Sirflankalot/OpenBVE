using OpenTK;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal int width;
        internal int height;

		public void Initialize(int width, int height) {
            this.width = width;
            this.height = height;

			InitializeShaders();
			InitializeFramebuffers();
		}


        public void PrepareForRender() {
            Algorithms.UpdateMeshNormals(meshes, 0, meshes.Count);
            Algorithms.UpdateObjectMatrices(objects, 0, objects.Count);

            GFXInterface.UpdateVAOVBO(meshes, 0, meshes.Count);
            GFXInterface.UploadMeshes(meshes, 0, meshes.Count);

            GFXInterface.UpdateTextureObjects(textures, 0, textures.Count);
            GFXInterface.UploadTextures(textures, 0, textures.Count);

            Algorithms.UpdateCameraMatrices(cameras, 0, cameras.Count, (float) width / height);
			Algorithms.UpdateObjectModelViewMatrices(objects, 0, objects.Count, ref cameras[active_camera].view_matrix);

			Algorithms.UpdateSunMatrix(sun, cameras[active_camera].focal_point);
        }

        public void RenderAll() {
            PrepareForRender();
            RenderAllObjects();
        }

        public void Deinitialize() {
			DeleteShaders();
			DeleteFramebuffers();
		}

		public void Resize(int width, int height) {
			this.width = width;
			this.height = height;

			foreach (Camera c in cameras) {
				c.matrix_valid = false;
			}

			DeleteFramebuffers();
			InitializeFramebuffers();
		}
	}
}