using OpenTK;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using GL = OpenTK.Graphics.OpenGL;
using GLFunc = OpenTK.Graphics.OpenGL.GL;

namespace LibRender {
    internal static class Algorithms {
        internal static void UpdateMeshNormals(List<Mesh> meshes, int start, int end) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= meshes.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int mesh_it = start; mesh_it < end; ++mesh_it) {
                // Reference to working mesh
                Mesh m = meshes[mesh_it];

                // Skip if normals already updated or is null
                if (m == null || m.updated_normals) {
                    continue;
                }

                // Set normal list to correct length
                var new_size = m.vertices.Count;
                var old_size = m.normals.Count;
                if (new_size > old_size) {
                    m.normals.Capacity = new_size;
                    m.normals.AddRange(Enumerable.Repeat(new Vector3(0, 0, 0), new_size - old_size));
                }
                else {
                    m.normals.RemoveRange(new_size, old_size - new_size);
                }

                // Zero out normals
                for (int i = 0; i < new_size; i++) {
                    m.normals[i] = new Vector3(0, 0, 0);
                }

                // Calculate each face's normals and add them to vertex normals
                for (int i = 0; i < m.indices.Count; i += 3) {
                    int index1 = m.indices[i + 0];
                    int index2 = m.indices[i + 1];
                    int index3 = m.indices[i + 2];

                    Vector3 vert1 = m.vertices[index1].position;
                    Vector3 vert2 = m.vertices[index2].position;
                    Vector3 vert3 = m.vertices[index3].position;

                    Vector3 side1 = vert2 - vert1;
                    Vector3 side2 = vert2 - vert3;

                    Vector3 cross = (side1.Yzx * side2.Zxy) - (side1.Zxy * side2.Yzx);

                    m.normals[index1] += cross;
                    m.normals[index2] += cross;
                    m.normals[index3] += cross;
                }

                // Normalize all normals
                for (int i = 0; i < m.normals.Count; ++i) {
                    var n = m.normals[i];
                    n.Normalize();
                    m.normals[i] = -n;
                }

                // Normals have been updated
                m.updated_normals = true;
            }
        }

		internal static void ClearObjectModelViewMatrices(List<Object> objects, int start, int end) {
			// Check indices
			if (!(0 <= start && 0 <= end && end <= objects.Count && (end == 0 ? start == end : start < end))) {
				throw new System.ArgumentException("Range invalid");
			}

			for (int i = start; i < end; ++i) {
				objects[i].inverse_model_view_valid = false;
			}
		}

        internal static void UpdateObjectMatrices(List<Object> objects, int start, int end) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= objects.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to Object
                Object o = objects[i];

                if (o == null || o.matrix_valid) {
                    continue;
                }

                var translation = Matrix4.CreateTranslation(o.position);
                var rotation = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(o.rotation));
                var scale = Matrix4.CreateScale(o.scale);

                o.transform = scale * rotation * translation;
                o.matrix_valid = true;
            }
        }

		internal static void UpdateObjectModelViewMatrices(List<Object> objects, int start, int end, ref Matrix4 view_matrix) {
			// Check indices
			if (!(0 <= start && 0 <= end && end <= objects.Count && (end == 0 ? start == end : start < end))) {
				throw new System.ArgumentException("Range invalid");
			}

			for (int i = start; i < end; ++i) {
				// Reference to Object
				Object o = objects[i];

				if (o == null || o.inverse_model_view_valid) {
					continue;
				}

				if (!o.matrix_valid) {
					UpdateObjectMatrices(objects, i, i + 1);
				}

				var model_view_mat = o.transform * view_matrix;
				model_view_mat.Invert();
				model_view_mat.Transpose();
				o.inverse_model_view_matrix = new Matrix3(model_view_mat);
			}
		}

        internal static void UpdateCameraMatrices(List<Camera> cameras, int start, int end, float ratio) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= cameras.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to Camera
                Camera c = cameras[i];

                if (c == null || c.matrix_valid) {
                    continue;
                }

				var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(c.fov), ratio, 0.1f, 1000.0f);

				Vector3 cam_point = new Vector3(0, 0, c.distance);
                cam_point = Vector3.TransformVector(cam_point, Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-c.rotation.Y)));
                cam_point = Vector3.TransformVector(cam_point, Matrix4.CreateRotationY(MathHelper.DegreesToRadians(c.rotation.X)));
                cam_point += c.focal_point;

                Matrix4 cam = Matrix4.LookAt(cam_point, c.focal_point, new Vector3(0, 1, 0));

				c.view_matrix = cam;
				c.proj_matrix = proj;
                c.transform_matrix = cam * proj;
				c.inverse_projection_matrix = proj.Inverted();
                c.matrix_valid = true;
            }
        }

        internal static void UpdateConeLightMatrices(List<Cone_Light> cone_lights, int start, int end) {
            // Check indices
            if (!(0 <= start && 0 <= end && end <= cone_lights.Count && (end == 0 ? start == end : start < end))) {
                throw new System.ArgumentException("Range invalid");
            }

            for (int i = start; i < end; ++i) {
                // Reference to Cone Light
                Cone_Light cl = cone_lights[i];

                if (cl == null || cl.matrix_valid || !cl.shadow) {
                    continue;
                }

                // Formula for matrices
                cl.shadow_matrix = new Matrix4();
                cl.matrix_valid = true;
            }
        }

		internal static void UpdateSunMatrix(Sun sun, Vector3 camera_position) {
			if (sun.matrix_valid) {
				return;
			}

			var hoz_rotate = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(sun.location.X));
			var vert_rotate = Matrix3.CreateRotationZ(MathHelper.DegreesToRadians(sun.location.Y));
			sun.direction = new Vector3(1, 0, 0);
			sun.direction = sun.direction * vert_rotate;
			sun.direction = sun.direction * hoz_rotate;
			sun.direction.Normalize();
			Vector3 sun_offset = sun.direction * 500;
			Matrix4 proj = Matrix4.CreateOrthographic(100, 100, 0, 1000);
			Matrix4 cam = Matrix4.LookAt(camera_position + sun_offset, camera_position, new Vector3(0, 1, 0));

			sun.shadow_matrix = cam * proj;
			sun.matrix_valid = true;
		}

		internal static void UpdateTextTextures(List<Text> texts, int start, int end) {
			// Check indices
			if (!(0 <= start && 0 <= end && end <= texts.Count && (end == 0 ? start == end : start < end))) {
				throw new System.ArgumentException("Range invalid");
			}

			// Dummy bitmap with dummy graphics
			using (Image dummy_img = new Bitmap(1, 1))
			using (Graphics dummy_drawing = Graphics.FromImage(dummy_img))
			// create brush for text
			using (Brush text_brush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))
			// Set string formatting
			using (StringFormat sf = new StringFormat()) {
				// sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
				sf.Trimming = StringTrimming.Word;

				for (int i = start; i < end; ++i) {
					// Reference to text
					Text t = texts[i];

					if (t == null || t.texture_ready) {
						continue;
					}

					SizeF text_size;
					if (t.max_width == 0) {
						text_size = dummy_drawing.MeasureString(t.text, t.font);
					}
					else {
						text_size = dummy_drawing.MeasureString(t.text, t.font, t.max_width);
					}
					// create new image of right size
					using (Bitmap img = new Bitmap((int) text_size.Width, (int) text_size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
					using (Graphics drawing = Graphics.FromImage(img)) {
						// set text quality
						// TODO: Options to change at runtime?
						drawing.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
						drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
						drawing.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
						drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
						drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

						// paint the background
						drawing.Clear(Color.Transparent);

						// Render the text and save it to image
						drawing.DrawString(t.text, t.font, text_brush, new RectangleF(0, 0, text_size.Width, text_size.Height), sf);
						drawing.Save();

						// Create pre-allocated memory stream
						int pixel_count = img.Width * img.Height;
						Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
						var img_data = img.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, img.PixelFormat);

						// Address of first line
						IntPtr ptr = img_data.Scan0;

						// Create array with size of bitmap
						int bytes = Math.Abs(img_data.Stride) * img.Height;
						byte[] raw_img = new byte[bytes];

						// Copy into array
						Marshal.Copy(ptr, raw_img, 0, bytes);

						// Allocate memory ahead of time
						t.texture.Clear();
						t.texture.Capacity = pixel_count;

						// Place raw values into pixels
						for (int j = 0; j < bytes; j += 4) {
							Pixel cur = new Pixel();
							cur.r = raw_img[j + 0];
							cur.g = raw_img[j + 1];
							cur.b = raw_img[j + 2];
							cur.a = raw_img[j + 3];
							t.texture.Add(cur);
						}

                        t.width = img.Width;
                        t.height = img.Height;

						t.texture_ready = true;
					}
				}
			}
		}
    }
}
