using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenBveApi.Colors;
using OpenTK.Graphics.OpenGL;
using GLPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace OpenBve
{
	internal static class TextureManager
	{

		// options
		internal enum InterpolationMode {
			NearestNeighbor,
			Bilinear,
			NearestNeighborMipmapped,
			BilinearMipmapped,
			TrilinearMipmapped,
			AnisotropicFiltering
		}

		// textures
		internal enum TextureLoadMode {
			Normal,
			Bve4SignalGlow

		}

		internal enum TextureWrapMode {
			Repeat,
			ClampToEdge

		}

		internal enum TextureTransparencyMode {
			None,
			TransparentColor,
			Alpha

		}

		internal class Texture
		{
			internal bool Queried;
			internal bool Loaded;
			internal string FileName;
			internal TextureLoadMode LoadMode; // OBSOLETE
			internal TextureWrapMode WrapModeX; // OBSOLETE
			internal TextureWrapMode WrapModeY; // OBSOLETE
			internal Color24 TransparentColor; // OBSOLETE
			internal byte TransparentColorUsed; // OBSOLETE
			internal TextureTransparencyMode Transparency; // OBSOLETE
			internal int ClipLeft; // OBSOLETE
			internal int ClipTop; // OBSOLETE
			internal int ClipWidth; // OBSOLETE
			internal int ClipHeight; // OBSOLETE
			internal int Width;
			internal int Height;
			internal bool IsRGBA;
			internal byte[] Data;
			internal LibRender.TextureHandle TextureHandle;
			internal int OpenGlTextureIndex; // OBSOLETE
			internal int CyclesSurvived;
			internal bool DontAllowUnload;
			internal bool LoadImmediately;
		}

		internal static Texture[] Textures = new Texture[16];
		private const int MaxCyclesUntilUnload = 4;
		private const double CycleInterval = 10.0;
		private static double CycleTime = 0.0;
		

		internal static int UseTexture(int TextureIndex) {
			if (TextureIndex == -1)
				return 0;
			Textures[TextureIndex].CyclesSurvived = 0;
			if (Textures[TextureIndex].Loaded) {
				return Textures[TextureIndex].OpenGlTextureIndex;
			}
			else if (Textures[TextureIndex].Data != null) {
				AddTextureToLibRender(TextureIndex);
				return Textures[TextureIndex].OpenGlTextureIndex;
			}
			return 0;
		}

		// unuse texture
		internal static void UnuseTexture(int TextureIndex) {
			if (TextureIndex == -1)
				return;
			if (Textures[TextureIndex].Loaded) {
				Renderer.renderer.Delete(Textures[TextureIndex].TextureHandle);
				Textures[TextureIndex].Loaded = false;
			}
		}

		internal static void UnuseAllTextures() {
			for (int i = 0; i < Textures.Length; i++) {
				if (Textures[i] != null) {
					UnuseTexture(i);
				}
			}
		}

		// unregister texture
		internal static void UnregisterTexture(ref int TextureIndex) {
			if (TextureIndex == -1)
				return;
			if (Textures[TextureIndex].Loaded) {
				GL.DeleteTextures(1, new int[] { Textures[TextureIndex].OpenGlTextureIndex });
			}
			Textures[TextureIndex] = null;
			TextureIndex = -1;
		}

		// load texture data
		private static void LoadTextureData(int TextureIndex) {
			if (Textures[TextureIndex].FileName != null && System.IO.File.Exists(Textures[TextureIndex].FileName)) {
				try {
					using (Bitmap Bitmap = (Bitmap)Image.FromFile(Textures[TextureIndex].FileName)) {
						LoadTexture(Bitmap, TextureIndex);
					}
				}
				catch {
					using (Bitmap Bitmap = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format24bppRgb)) {
						LoadTexture(Bitmap, TextureIndex);
					}
				}
			}
			else {
				Textures[TextureIndex].Loaded = true;
				Textures[TextureIndex].Data = null;
			}
		}

		// update
		internal static void Update(double TimeElapsed) {
			CycleTime += TimeElapsed;
			if (CycleTime >= CycleInterval) {
				CycleTime = 0.0;
				for (int i = 0; i < Textures.Length; i++) {
					if (Textures[i] != null) {
						if (Textures[i].Loaded & !Textures[i].DontAllowUnload) {
							Textures[i].CyclesSurvived++;
							if (Textures[i].CyclesSurvived >= 2) {
								Textures[i].Queried = false;
							}
							if (Textures[i].CyclesSurvived >= MaxCyclesUntilUnload) {
								UnuseTexture(i);
							}
						}
						else {
							Textures[i].CyclesSurvived = 0;
						}
					}
				}
			}
		}

		// register texture
		internal static int RegisterTexture(string FileName, Color24 TransparentColor, byte TransparentColorUsed, TextureWrapMode WrapModeX, TextureWrapMode WrapModeY, bool DontAllowUnload) {
			return RegisterTexture(FileName, TransparentColor, TransparentColorUsed, TextureLoadMode.Normal, WrapModeX, WrapModeY, DontAllowUnload, 0, 0, 0, 0);
		}

		internal static int RegisterTexture(string FileName, Color24 TransparentColor, byte TransparentColorUsed, TextureLoadMode LoadMode, TextureWrapMode WrapModeX, TextureWrapMode WrapModeY, bool DontAllowUnload, int ClipLeft, int ClipTop, int ClipWidth, int ClipHeight) {
			int i = FindTexture(FileName, TransparentColor, TransparentColorUsed, LoadMode, WrapModeX, WrapModeY, ClipLeft, ClipTop, ClipWidth, ClipHeight);
			if (i >= 0) {
				return i;
			}
			else {
				i = GetFreeTexture();
				Textures[i] = new Texture {
					Queried = false,
					Loaded = false,
					FileName = FileName,
					TransparentColor = TransparentColor,
					TransparentColorUsed = TransparentColorUsed,
					LoadMode = LoadMode,
					WrapModeX = WrapModeX,
					WrapModeY = WrapModeY,
					ClipLeft = ClipLeft,
					ClipTop = ClipTop,
					ClipWidth = ClipWidth,
					ClipHeight = ClipHeight,
					DontAllowUnload = DontAllowUnload,
					LoadImmediately = false,
					OpenGlTextureIndex = 0
				};
				return i;
			}
		}

		internal static int RegisterTexture(Bitmap Bitmap, bool Alpha) {
			int i = GetFreeTexture();
			int[] a = new int[1];
			GL.GenTextures(1, a);
			Textures[i] = new Texture {
				Queried = false,
				OpenGlTextureIndex = a[0],
				Transparency = TextureTransparencyMode.None,
				TransparentColor = new Color24(0, 0, 0),
				TransparentColorUsed = 0,
				FileName = null,
				Loaded = true,
				DontAllowUnload = true
			};
			if (Alpha) {
				Textures[i].Transparency = TextureTransparencyMode.Alpha;
			}
			LoadTexture(Bitmap, i);
			AddTextureToLibRender(i);
			return i;
		}

		internal static int RegisterTexture(Bitmap Bitmap, Color24 TransparentColor) {
			int i = GetFreeTexture();
			int[] a = new int[1];
			GL.GenTextures(1, a);
			Textures[i] = new Texture {
				Queried = false,
				OpenGlTextureIndex = a[0],
				Transparency = TextureTransparencyMode.TransparentColor,
				TransparentColor = TransparentColor,
				TransparentColorUsed = 1,
				FileName = null,
				Loaded = true,
				DontAllowUnload = true
			};
			LoadTexture(Bitmap, i);
			AddTextureToLibRender(i);
			return i;
		}

		// find texture
		private static int FindTexture(string FileName, Color24 TransparentColor, byte TransparentColorUsed, TextureLoadMode LoadMode, TextureWrapMode WrapModeX, TextureWrapMode WrapModeY, int ClipLeft, int ClipTop, int ClipWidth, int ClipHeight) {
			for (int i = 1; i < Textures.Length; i++) {
				if (Textures[i] != null && Textures[i].FileName != null) {
					if (string.Compare(Textures[i].FileName, FileName, StringComparison.OrdinalIgnoreCase) == 0) {
						if (Textures[i].LoadMode == LoadMode & Textures[i].WrapModeX == WrapModeX & Textures[i].WrapModeY == WrapModeY) {
							if (Textures[i].ClipLeft == ClipLeft & Textures[i].ClipTop == ClipTop & Textures[i].ClipWidth == ClipWidth & Textures[i].ClipHeight == ClipHeight) {
								if (TransparentColorUsed == 0) {
									if (Textures[i].TransparentColorUsed == 0) {
										return i;
									}
								}
								else {
									if (Textures[i].TransparentColorUsed != 0) {
										if (Textures[i].TransparentColor.R == TransparentColor.R & Textures[i].TransparentColor.G == TransparentColor.G & Textures[i].TransparentColor.B == TransparentColor.B) {
											return i;
										}
									}
								}
							}
						}
					}
				}
			}
			return -1;
		}

		// get free texture
		private static int GetFreeTexture() {
			int i;
			for (i = 0; i < Textures.Length; i++) {
				if (Textures[i] == null)
					break;
			}
			if (i >= Textures.Length) {
				Array.Resize<Texture>(ref Textures, Textures.Length << 1);
			}
			return i;
		}
		
		/// <summary>
		/// Load Bitmap into Data of a Texture
		/// </summary>
		/// <param name="Bitmap">Image to Load</param>
		/// <param name="TextureIndex">Index of Texture to load into</param>
		private static void LoadTexture(Bitmap Bitmap, int TextureIndex) {
			try {
				int Stride;
				byte[] Data;

				int Width = Bitmap.Width;
				int Height = Bitmap.Height;
				BitmapData d = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, Bitmap.PixelFormat);
				Stride = d.Stride;
				Data = new byte[Stride * Height];
				System.Runtime.InteropServices.Marshal.Copy(d.Scan0, Data, 0, Stride * Height);
				Bitmap.UnlockBits(d);

				Textures[TextureIndex].Width = Width;
				Textures[TextureIndex].Height = Height;
				Textures[TextureIndex].Data = Data;
			}
			catch (Exception ex) {
				Interface.AddMessage(Interface.MessageType.Error, false, "Internal error in TextureManager.cs::LoadTextureRGBForData: " + ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Converts RGB(A) byte data into OpenBveApi.Pixel array
		/// </summary>
		/// <param name="data">Input byte array in RGB or RGBA format</param>
		/// <param name="width">Width of Image</param>
		/// <param name="height">Height of Image</param>
		/// <param name="alpha">True if Bytestream has Alpha Component</param>
		/// <returns>OpenBveApi.Pixel array of the image</returns>
		private static OpenBveApi.Pixel[] ConvertDatatoPixels(byte[] data, int width, int height, bool alpha) {
			OpenBveApi.Pixel[] value = new OpenBveApi.Pixel[width * height];
			if (alpha) {
				for (int i = 0; i < data.Length; i += 4) {
					value[i] = new OpenBveApi.Pixel(data[i], data[i + 1], data[i + 2], data[i + 3]);
				}
			}
			else {
				for (int i = 0; i < data.Length; i += 3) {
					value[i] = new OpenBveApi.Pixel(data[i], data[i + 1], data[i + 2], 255);
				}
			}
			return value;
		}

		/// <summary>
		/// Creates a LibRender texture handle for texture at specified index
		/// </summary>
		/// <param name="TextureIndex">Index of texture that needs a handle</param>
		private static void AddTextureToLibRender(int TextureIndex) {
			// Reference to texture
			var tex = Textures[TextureIndex];

			// Cache width/height for better perforance
			var width = tex.Width;
			var height = tex.Height;
			var alpha = tex.IsRGBA;

			var pixels = ConvertDatatoPixels(tex.Data, width, height, alpha);

			// Add texture to LibRender
			tex.TextureHandle = Renderer.renderer.AddTexture(pixels, width, height);

			// Clear temporary store of image
			tex.Loaded = true;
			tex.Data = null;
		}
	}
}
