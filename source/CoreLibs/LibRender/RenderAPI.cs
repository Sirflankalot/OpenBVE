using System.Diagnostics;
using OpenTK;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    public partial class Renderer {
        internal int display_width;
        internal int display_height;
		
		private GL.TextureMinFilter tex_filt_min;
		private GL.TextureMagFilter tex_filt_mag;
		private int tex_filt_aniso = 0;

		public void Initialize(int width, int height) {
            this.display_width = width;
            this.display_height = height;

			InitializeShaders();
			InitializeFramebuffers();
			SetSetting(settings);

			cube_mesh = CreateCubeMesh();
			square_mesh = CreateSquareMesh();

			Log("Initializing!", Settings.Verbosity.Level0);
		}


        public void PrepareForRender() {
            var gc_timer = Stopwatch.StartNew();
			Algorithms.GarbageCollectUnused(this);
            gc_timer.Stop();
            statistics.val_subframe_time.garbage_collection = gc_timer.ElapsedTicks / (Stopwatch.Frequency / 1000f);

            var update_timer = Stopwatch.StartNew();
            Algorithms.UpdateMeshNormals(meshes, 0, meshes.Count);
            Algorithms.UpdateObjectMatrices(objects, 0, objects.Count);

            GFXInterface.UpdateVAOVBO(meshes, 0, meshes.Count);
            GFXInterface.UploadMeshes(meshes, 0, meshes.Count);

            GFXInterface.UpdateTextureObjects(textures, 0, textures.Count);
            GFXInterface.UploadTextures(textures, 0, textures.Count, tex_filt_min, tex_filt_mag, tex_filt_aniso);

            Algorithms.UpdateCameraMatrices(cameras, 0, cameras.Count, (float) display_width / display_height, settings.view_distance);
			Algorithms.UpdateObjectModelViewMatrices(objects, 0, objects.Count, ref cameras[active_camera].view_matrix);

			Algorithms.UpdateSunMatrix(sun, cameras[active_camera].focal_point);

            Algorithms.UpdateTextTextures(texts, 0, texts.Count, settings.text_rendering_quality);
			GFXInterface.UpdateTextTextureObjects(texts, 0, texts.Count);
			GFXInterface.UploadTextTextures(texts, 0, texts.Count);

			GFXInterface.UpdateFlatMeshVAOVBO(flat_meshes, 0, flat_meshes.Count);
			GFXInterface.UploadFlatMeshMeshes(flat_meshes, 0, flat_meshes.Count);

			Algorithms.UpdateUIElementMatrices(uielements, 0, uielements.Count);
            update_timer.Stop();
            statistics.val_subframe_time.updating = update_timer.ElapsedTicks / (Stopwatch.Frequency / 1000f);
        }

        private long last_tick = 0;
        public void RenderAll() {
            long cur_tick = Stopwatch.GetTimestamp();
            if (last_tick != 0) {
                statistics.val_frame_time = (cur_tick - last_tick) / (Stopwatch.Frequency / 1000f);
            }
            last_tick = cur_tick;
            PrepareForRender();
            RenderAllObjects();
            statistics.val_frame_count++;
        }

        public void Deinitialize() {
			DeleteShaders();
			DeleteFramebuffers();
		}

		public void Resize(int width, int height) {
			this.display_width = width;
			this.display_height = height;

			foreach (Camera c in cameras) {
				c.matrix_valid = false;
			}

			DeleteFramebuffers();
			InitializeFramebuffers();
		}
	}
}