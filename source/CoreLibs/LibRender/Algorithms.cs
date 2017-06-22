using OpenTK;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using OpenBveApi;

namespace LibRender {
    internal static class Algorithms {
        internal static void UpdateMeshNormals(List<Mesh> meshes, int start, int end) {
			Utilities.AssertValidIndicies(meshes, start, end);

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
			Utilities.AssertValidIndicies(objects, start, end);

			for (int i = start; i < end; ++i) {
				if (objects[i] == null) {
					continue;
				}

				objects[i].inverse_model_view_valid = false;
			}
		}

        internal static void UpdateObjectMatrices(List<Object> objects, int start, int end) {
			Utilities.AssertValidIndicies(objects, start, end);

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
			Utilities.AssertValidIndicies(objects, start, end);

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

        internal static void UpdateCameraMatrices(List<Camera> cameras, int start, int end, float ratio, float view_distance, bool force = false) {
			Utilities.AssertValidIndicies(cameras, start, end);

            for (int i = start; i < end; ++i) {
                // Reference to Camera
                Camera c = cameras[i];

                if (c == null || (c.matrix_valid && !force)) {
                    continue;
                }

				var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(c.fov), ratio, 0.1f, 1000.0f);

				Vector3 cam_point = new Vector3(0, 0, c.distance);
                cam_point = Vector3.TransformVector(cam_point, Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-c.rotation.Y)));
                cam_point = Vector3.TransformVector(cam_point, Matrix4.CreateRotationY(MathHelper.DegreesToRadians(c.rotation.X)));
                cam_point += c.focal_point;

				c.position = cam_point;
                Matrix4 cam = Matrix4.LookAt(cam_point, c.focal_point, new Vector3(0, 1, 0));

				c.view_matrix = cam;
				c.proj_matrix = proj;
                c.transform_matrix = cam * proj;
				c.inverse_projection_matrix = proj.Inverted();
                c.matrix_valid = true;
            }
        }

        internal static void UpdateConeLightMatrices(List<Cone_Light> cone_lights, int start, int end) {
			Utilities.AssertValidIndicies(cone_lights, start, end);

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

		internal static void UpdateTextTextures(List<Text> texts, int start, int end, Settings.TextRenderingQuality quality) {
			Utilities.AssertValidIndicies(texts, start, end);

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
					using (Bitmap img = new Bitmap((int) text_size.Width + 3, (int) text_size.Height + 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
					using (Graphics drawing = Graphics.FromImage(img)) {
						// set text quality
						switch (quality) {
							case Settings.TextRenderingQuality.Low:
								drawing.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
								drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
								drawing.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
								drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
								drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
								break;
							case Settings.TextRenderingQuality.Medium:
								drawing.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
								drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
								drawing.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
								drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
								drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
								break;
							case Settings.TextRenderingQuality.High:
								drawing.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
								drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
								drawing.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
								drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
								drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
								break;
							case Settings.TextRenderingQuality.Ultra:
								drawing.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
								drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
								drawing.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
								drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
								drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
								break;
						}

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
                            byte val = (byte) (raw_img[j + 0] * 0.2126f + raw_img[j + 1] * 0.7152f + raw_img[j + 2] * 0.0722f);

                            Pixel cur = new Pixel() {
								r = val,
								g = val,
								b = val,
								a = val
							};
							t.texture.Add(cur);
						}

                        t.width = img.Width;
                        t.height = img.Height;

						t.texture_ready = true;
					}
				}
			}
		}

		internal static void UpdateUIElementMatrices(List<UIElement> uielements, int start, int end) {
			Utilities.AssertValidIndicies(uielements, start, end);

			for (int i = start; i < end; ++i) {
				// Reference to uielement
				UIElement uie = uielements[i];

				if (uie == null || uie.matrix_valid) {
					continue;
				}
				
				Matrix2 rot = Matrix2.CreateRotation(MathHelper.DegreesToRadians(uie.rotation));

				uie.transform = rot;
				uie.matrix_valid = true;
			}
		}

		internal static void GarbageCollectUnused(Renderer renderer) {
			const float clear_ratio = 0.5f;
			const float padding = 1.5f;
			int kickback_num = -1;

            ///////////////////
            // Clear Cameras //
            ///////////////////

            renderer.statistics.val_cameras.collected = 0;
            if (renderer.camera_translation.Count / (float)renderer.cameras.Count <= clear_ratio) {
				// Find first null
				for (int i = 0; i < renderer.cameras.Count; ++i) {
					if (renderer.cameras[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.cameras.Count; ++i) {
						if (renderer.cameras[i] == null) {
							continue;
						}

						renderer.cameras[kickback_num] = renderer.cameras[i];
						renderer.cameras[i] = null;
						renderer.camera_translation[renderer.cameras[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.cameras.Count - kickback_num;
                    renderer.cameras.RemoveRange(kickback_num, count);
					renderer.cameras.Capacity = (int)(renderer.cameras.Count * padding);
                    renderer.statistics.val_cameras.collected = count;
				}
			}
            renderer.statistics.val_cameras.total = renderer.camera_translation.Count;


            //////////////////////
            // Clear ConeLights //
            //////////////////////

            renderer.statistics.val_conelights.collected = 0;
            if (renderer.cone_lights_translation.Count / (float) renderer.cone_lights.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.cone_lights.Count; ++i) {
					if (renderer.cone_lights[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.cone_lights.Count; ++i) {
						if (renderer.cone_lights[i] == null) {
							continue;
						}

						renderer.cone_lights[kickback_num] = renderer.cone_lights[i];
						renderer.cone_lights[i] = null;
						renderer.cone_lights_translation[renderer.cone_lights[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.cone_lights.Count - kickback_num;
                    renderer.cone_lights.RemoveRange(kickback_num, count);
					renderer.cone_lights.Capacity = (int) (renderer.cone_lights.Count * padding);
                    renderer.statistics.val_conelights.collected = count;
				}
			}
            renderer.statistics.val_conelights.total = renderer.cone_lights_translation.Count;

            //////////////////////
            // Clear FlatMeshes //
            //////////////////////

            renderer.statistics.val_flatmeshes.collected = 0;
            if (renderer.flat_meshes_translation.Count / (float) renderer.flat_meshes.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.flat_meshes.Count; ++i) {
					if (renderer.flat_meshes[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.flat_meshes.Count; ++i) {
						if (renderer.flat_meshes[i] == null) {
							continue;
						}

						renderer.flat_meshes[kickback_num] = renderer.flat_meshes[i];
						renderer.flat_meshes[i] = null;
						renderer.flat_meshes_translation[renderer.flat_meshes[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.flat_meshes.Count - kickback_num;
                    renderer.flat_meshes.RemoveRange(kickback_num, count);
					renderer.flat_meshes.Capacity = (int) (renderer.flat_meshes.Count * padding);
                    renderer.statistics.val_flatmeshes.collected = count;
				}
			}
            renderer.statistics.val_flatmeshes.total = renderer.flat_meshes_translation.Count;

            //////////////////
            // Clear Meshes //
            //////////////////

            renderer.statistics.val_meshes.collected = 0;
            if (renderer.meshes_translation.Count / (float) renderer.meshes.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.meshes.Count; ++i) {
					if (renderer.meshes[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.meshes.Count; ++i) {
						if (renderer.meshes[i] == null) {
							continue;
						}

						renderer.meshes[kickback_num] = renderer.meshes[i];
						renderer.meshes[i] = null;
						renderer.meshes_translation[renderer.meshes[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.meshes.Count - kickback_num;
                    renderer.meshes.RemoveRange(kickback_num, count);
					renderer.meshes.Capacity = (int) (renderer.meshes.Count * padding);
                    renderer.statistics.val_meshes.collected = count;
				}
			}
            renderer.statistics.val_meshes.total = renderer.meshes_translation.Count;

            ///////////////////
            // Clear Objects //
            ///////////////////

            renderer.statistics.val_objects.collected = 0;
            if (renderer.object_translation.Count / (float) renderer.objects.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.objects.Count; ++i) {
					if (renderer.objects[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.objects.Count; ++i) {
						if (renderer.objects[i] == null) {
							continue;
						}

						renderer.objects[kickback_num] = renderer.objects[i];
						renderer.objects[i] = null;
						renderer.object_translation[renderer.objects[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.objects.Count - kickback_num;
                    renderer.objects.RemoveRange(kickback_num, count);
					renderer.objects.Capacity = (int) (renderer.objects.Count * padding);
                    renderer.statistics.val_objects.collected = count;
				}
			}
            renderer.statistics.val_objects.total = renderer.object_translation.Count;

            ///////////////////////
            // Clear PointLights //
            ///////////////////////

            renderer.statistics.val_pointlights.collected = 0;
            if (renderer.point_lights_translation.Count / (float) renderer.point_lights.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.point_lights.Count; ++i) {
					if (renderer.point_lights[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.point_lights.Count; ++i) {
						if (renderer.point_lights[i] == null) {
							continue;
						}

						renderer.point_lights[kickback_num] = renderer.point_lights[i];
						renderer.point_lights[i] = null;
						renderer.point_lights_translation[renderer.point_lights[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.point_lights.Count - kickback_num;
                    renderer.point_lights.RemoveRange(kickback_num, count);
					renderer.point_lights.Capacity = (int) (renderer.point_lights.Count * padding);
                    renderer.statistics.val_pointlights.collected = count;
				}
			}
            renderer.statistics.val_pointlights.total = renderer.point_lights_translation.Count;

            ////////////////
            // Clear Text //
            ////////////////

            renderer.statistics.val_texts.collected = 0;
            if (renderer.texts_translation.Count / (float) renderer.texts.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.texts.Count; ++i) {
					if (renderer.texts[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.texts.Count; ++i) {
						if (renderer.texts[i] == null) {
							continue;
						}

						renderer.texts[kickback_num] = renderer.texts[i];
						renderer.texts[i] = null;
						renderer.texts_translation[renderer.texts[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.texts.Count - kickback_num;
                    renderer.texts.RemoveRange(kickback_num, count);
					renderer.texts.Capacity = (int) (renderer.texts.Count * padding);
                    renderer.statistics.val_texts.collected = count;
				}
			}
            renderer.statistics.val_texts.total = renderer.texts_translation.Count;

            ////////////////////
            // Clear Textures //
            ////////////////////

            renderer.statistics.val_textures.collected = 0;
            if (renderer.textures_translation.Count / (float) renderer.textures.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.textures.Count; ++i) {
					if (renderer.textures[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.textures.Count; ++i) {
						if (renderer.textures[i] == null) {
							continue;
						}

						renderer.textures[kickback_num] = renderer.textures[i];
						renderer.textures[i] = null;
						renderer.textures_translation[renderer.textures[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.textures.Count - kickback_num;
                    renderer.textures.RemoveRange(kickback_num, count);
					renderer.textures.Capacity = (int) (renderer.textures.Count * padding);
                    renderer.statistics.val_textures.collected = count;
				}
			}
            renderer.statistics.val_textures.total = renderer.textures_translation.Count;

            //////////////////////
            // Clear UIElements //
            //////////////////////

            renderer.statistics.val_uielements.collected = 0;
            if (renderer.uielements_translation.Count / (float) renderer.uielements.Count <= clear_ratio) {
				kickback_num = -1;
				// Find first null
				for (int i = 0; i < renderer.uielements.Count; ++i) {
					if (renderer.uielements[i] == null) {
						kickback_num = i;
						break;
					}
				}

				// From first null compact everything
				if (kickback_num != -1) {
					for (int i = kickback_num + 1; i < renderer.uielements.Count; ++i) {
						if (renderer.uielements[i] == null) {
							continue;
						}

						renderer.uielements[kickback_num] = renderer.uielements[i];
						renderer.uielements[i] = null;
						renderer.uielements_translation[renderer.uielements[kickback_num].handle.id] = kickback_num;
						kickback_num += 1;
					}
                    int count = renderer.uielements.Count - kickback_num;
                    renderer.uielements.RemoveRange(kickback_num, count);
					renderer.uielements.Capacity = (int) (renderer.uielements.Count * padding);
                    renderer.statistics.val_uielements.collected = count;
				}
			}
            renderer.statistics.val_uielements.total = renderer.uielements_translation.Count;
		}
    }
}
