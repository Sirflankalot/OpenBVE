// ╔═════════════════════════════════════════════════════════════╗
// ║ Renderer.cs for the Structure Viewer                        ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using OpenBveApi;
using LibRender;
using OpenBveApi.Colors;

namespace OpenBve {
    internal static class Renderer
    {

        // screen (output window)
        internal static int ScreenWidth = 960;
        internal static int ScreenHeight = 600;

        internal static LibRender.Renderer renderer = new LibRender.Renderer();

        // first frame behavior
        internal enum LoadTextureImmediatelyMode { NotYet, Yes }
        internal static LoadTextureImmediatelyMode LoadTexturesImmediately = LoadTextureImmediatelyMode.NotYet;
        internal enum TransparencyMode { Sharp, Smooth }

        // object list
        internal enum ObjectType : byte
        {
            /// <summary>The object is part of the static scenery. The matching ObjectListType is StaticOpaque for fully opaque faces, and DynamicAlpha for all other faces.</summary>
            Static = 1,
            /// <summary>The object is part of the animated scenery or of a train exterior. The matching ObjectListType is DynamicOpaque for fully opaque faces, and DynamicAlpha for all other faces.</summary>
            Dynamic = 2,
            /// <summary>The object is part of the cab. The matching ObjectListType is OverlayOpaque for fully opaque faces, and OverlayAlpha for all other faces.</summary>
            Overlay = 3
        }
        private struct Object
        {
            internal int ObjectIndex;
            internal int[] FaceListIndices;
            internal ObjectType Type;
        }
        private static Object[] ObjectList = new Object[256];
        private static int ObjectListCount = 0;

        // face lists
        private struct ObjectFace
        {
            internal int ObjectListIndex;
            internal int ObjectIndex;
            internal const int MeshIndex = 0;
            internal int FaceIndex;
        }
        // opaque
        private static ObjectFace[] OpaqueList = new ObjectFace[256];
        internal static int OpaqueListCount = 0;
        // transparent color
        private static ObjectFace[] TransparentColorList = new ObjectFace[256];
        private static double[] TransparentColorListDistance = new double[256];
        internal static int TransparentColorListCount = 0;
        // alpha
        private static ObjectFace[] AlphaList = new ObjectFace[256];
        private static double[] AlphaListDistance = new double[256];
        internal static int AlphaListCount = 0;
        // overlay
        private static ObjectFace[] OverlayList = new ObjectFace[256];
        private static double[] OverlayListDistance = new double[256];
        internal static int OverlayListCount = 0;

        // current opengl data
        private static AlphaFunction AlphaFuncComparison = 0;
        private static float AlphaFuncValue = 0.0f;
        private static bool BlendEnabled = false;
        private static bool AlphaTestEnabled = false;
        private static bool CullEnabled = true;
        internal static bool LightingEnabled = false;
        internal static bool FogEnabled = false;
        private static bool TexturingEnabled = false;
        private static bool EmissiveEnabled = false;
        internal static bool TransparentColorDepthSorting = false;

        // options
        internal static bool OptionLighting = true;
        internal static Color24 OptionAmbientColor = new Color24(160, 160, 160);
        internal static Color24 OptionDiffuseColor = new Color24(159, 159, 159);
        internal static OpenBveApi.Math.Vector3 OptionLightPosition = new OpenBveApi.Math.Vector3(0.215920077052065f, 0.875724044222352f, -0.431840154104129f);
        internal static float OptionLightingResultingAmount = 1.0f;
        internal static bool OptionFauxNighttime = true;
        internal static bool OptionNormals = false;
        internal static bool OptionWireframe = false;
        internal static bool OptionBackfaceCulling = true;
        internal static bool OptionCoordinateSystem = false;
        internal static bool OptionInterface = true;

        // background color
        internal static int BackgroundColor = 0;
        internal const int MaxBackgroundColor = 4;
        internal static string GetBackgroundColorName()
        {
            switch (BackgroundColor)
            {
                case 0: return "Light Gray";
                case 1: return "White";
                case 2: return "Black";
                case 3: return "Dark Gray";
                default: return "Custom";
            }
        }
        internal static void ApplyBackgroundColor()
        {
            switch (BackgroundColor)
            {
                case 0:
                    ApplyBackgroundColor(0.67f, 0.67f, 0.67f);
                    break;
                case 1:
                    ApplyBackgroundColor(1.0f, 1.0f, 1.0f);
                    break;
                case 2:
					ApplyBackgroundColor(0.0f, 0.0f, 0.0f);
                    break;
                case 3:
					ApplyBackgroundColor(0.33f, 0.33f, 0.33f);
                    break;
            }
        }

		/// <summary>
		/// Makes program background the supplied color
		/// </summary>
		/// <param name="red">Red component of background (0-255)</param>
		/// <param name="green">Green component of background (0-255)</param>
		/// <param name="blue">Blue component of background (0-255)</param>
		internal static void ApplyBackgroundColor(byte red, byte green, byte blue) {
			ApplyBackgroundColor((float)red / 255.0f, (float)green / 255.0f, (float)blue / 255.0f);
		}
		/// <summary>
		/// Makes program background the supplied color
		/// </summary>
		/// <param name="red">Red component of background (0-1)</param>
		/// <param name="green">Green component of background (0-1)</param>
		/// <param name="blue">Blue component of background (0-1)</param>
		internal static void ApplyBackgroundColor(float red, float green, float blue) {
			renderer.SetSetting(LibRender.Settings.ClearColor.Set, new Vector3(red, green, blue));
		}

		// constants
		private const float inv255 = 1.0f / 255.0f;

        // reset
        internal static void Reset()
        {
            LoadTexturesImmediately = LoadTextureImmediatelyMode.NotYet;
            ObjectList = new Object[256];
            ObjectListCount = 0;
            OpaqueList = new ObjectFace[256];
            OpaqueListCount = 0;
            TransparentColorList = new ObjectFace[256];
            TransparentColorListDistance = new double[256];
            TransparentColorListCount = 0;
            AlphaList = new ObjectFace[256];
            AlphaListDistance = new double[256];
            AlphaListCount = 0;
            OverlayList = new ObjectFace[256];
            OverlayListDistance = new double[256];
            OverlayListCount = 0;
            OptionLighting = true;
            OptionAmbientColor = new Color24(160, 160, 160);
            OptionDiffuseColor = new Color24(160, 160, 160);
            OptionLightPosition = new OpenBveApi.Math.Vector3(0.215920077052065f, 0.875724044222352f, -0.431840154104129f);
            OptionLightingResultingAmount = 1.0f;
            GL.Disable(EnableCap.Fog); FogEnabled = false;
        }

        // initialize
        internal static void Initialize() {
            renderer.Initialize(ScreenWidth, ScreenHeight);
            renderer.SetSetting(LibRender.Settings.TextRenderingQuality.Ultra);
			ApplyBackgroundColor();
			// opengl
			//GL.ShadeModel(ShadingModel.Decal); // what is decal?
			GL.ShadeModel(ShadingModel.Smooth);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			GL.Enable(EnableCap.DepthTest);
			if (!TexturingEnabled) {
				GL.Enable(EnableCap.Texture2D);
				TexturingEnabled = true;
			}
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.DepthFunc(DepthFunction.Lequal);
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
			GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);
			GL.Enable(EnableCap.CullFace);
			CullEnabled = true;
			GL.CullFace(CullFaceMode.Front);
			GL.Disable(EnableCap.Dither);
			// opengl
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.PushMatrix();
			GL.ClearColor(0.67f, 0.67f, 0.67f, 1.0f);
			var mat = Matrix4d.LookAt(0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0);
			GL.MultMatrix(ref mat);
			GL.PopMatrix();
			TransparentColorDepthSorting = Interface.CurrentOptions.TransparencyMode == TransparencyMode.Smooth & Interface.CurrentOptions.Interpolation != TextureManager.InterpolationMode.NearestNeighbor & Interface.CurrentOptions.Interpolation != TextureManager.InterpolationMode.Bilinear;
		}

		// initialize lighting
		internal static void InitializeLighting()
        {
            if (OptionAmbientColor.R == 255 & OptionAmbientColor.G == 255 & OptionAmbientColor.B == 255 & OptionDiffuseColor.R == 0 & OptionDiffuseColor.G == 0 & OptionDiffuseColor.B == 0)
            {
                OptionLighting = false;
            }
            else
            {
                OptionLighting = true;
            }
            if (OptionLighting)
            {
                GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { inv255 * (float)OptionAmbientColor.R, inv255 * (float)OptionAmbientColor.G, inv255 * (float)OptionAmbientColor.B, 1.0f });
                GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { inv255 * (float)OptionDiffuseColor.R, inv255 * (float)OptionDiffuseColor.G, inv255 * (float)OptionDiffuseColor.B, 1.0f });
                GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                GL.Enable(EnableCap.Lighting); LightingEnabled = true;
                GL.Enable(EnableCap.Light0);
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                GL.ShadeModel(ShadingModel.Smooth);
                OptionLightingResultingAmount = (float)((int)OptionAmbientColor.R + (int)OptionAmbientColor.G + (int)OptionAmbientColor.B) / 480.0f;
                if (OptionLightingResultingAmount > 1.0f) OptionLightingResultingAmount = 1.0f;
            }
            else
            {
                GL.Disable(EnableCap.Lighting); LightingEnabled = false;
            }
        }

        // render scene
        internal static byte[] PixelBuffer = null;
        internal static int PixelBufferOpenGlTextureIndex = 0;
        internal static void RenderScene()
        {
            // initialize
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PushMatrix();
            if (LoadTexturesImmediately == LoadTextureImmediatelyMode.NotYet)
            {
                LoadTexturesImmediately = LoadTextureImmediatelyMode.Yes;
                ReAddObjects();
            }
            // setup camera
            double cx = World.AbsoluteCameraPosition.X;
            double cy = World.AbsoluteCameraPosition.Y;
            double cz = World.AbsoluteCameraPosition.Z;
            double dx = World.AbsoluteCameraDirection.X;
            double dy = World.AbsoluteCameraDirection.Y;
            double dz = World.AbsoluteCameraDirection.Z;
            double ux = World.AbsoluteCameraUp.X;
            double uy = World.AbsoluteCameraUp.Y;
            double uz = World.AbsoluteCameraUp.Z;
            var mat = Matrix4d.LookAt(0.0, 0.0, 0.0, dx, dy, dz, ux, uy, uz);
            GL.MultMatrix(ref mat);
            if (OptionLighting)
            {
                GL.Light(LightName.Light0, LightParameter.Position, new float[] { (float)OptionLightPosition.X, (float)OptionLightPosition.Y, (float)OptionLightPosition.Z, 0.0f });
            }
            // render polygons
            GL.Disable(EnableCap.DepthTest);
            if (OptionLighting)
            {
                if (!LightingEnabled)
                {
                    GL.Enable(EnableCap.Lighting);
                    LightingEnabled = true;
                }
            }
            else if (LightingEnabled)
            {
                GL.Disable(EnableCap.Lighting);
                LightingEnabled = false;
            }
            GL.AlphaFunc(AlphaFunction.Greater, 0.0f);
            BlendEnabled = false; GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            LastBoundTexture = 0;
            for (int i = 0; i < OpaqueListCount; i++)
            {
                RenderFace(ref OpaqueList[i], cx, cy, cz);
            }
            // transparent color list
			if (TransparentColorDepthSorting) {
				SortPolygons(TransparentColorList, TransparentColorListCount, TransparentColorListDistance, 1, 0.0);
				BlendEnabled = true; GL.Enable(EnableCap.Blend);
				for (int i = 0; i < TransparentColorListCount; i++) {
					GL.DepthMask(false);
					SetAlphaFunc(AlphaFunction.Less, 1.0f);
					RenderFace(ref TransparentColorList[i], cx, cy, cz);
					GL.DepthMask(true);
					SetAlphaFunc(AlphaFunction.Equal, 1.0f);
					RenderFace(ref TransparentColorList[i], cx, cy, cz);
				}
			} else {
				for (int i = 0; i < TransparentColorListCount; i++) {
					RenderFace(ref TransparentColorList[i], cx, cy, cz);
				}
			}
			// alpha list
			SortPolygons(AlphaList, AlphaListCount, AlphaListDistance, 2, 0.0);
			if (Interface.CurrentOptions.TransparencyMode == TransparencyMode.Smooth) {
				BlendEnabled = true; GL.Enable(EnableCap.Blend);
				bool depthMask = true;
				for (int i = 0; i < AlphaListCount; i++) {
					int r = (int)ObjectManager.Objects[AlphaList[i].ObjectIndex].Mesh.Faces[AlphaList[i].FaceIndex].Material;
					if (ObjectManager.Objects[AlphaList[i].ObjectIndex].Mesh.Materials[r].BlendMode == World.MeshMaterialBlendMode.Additive) {
						if (depthMask) {
							GL.DepthMask(false);
							depthMask = false;
						}
						SetAlphaFunc(AlphaFunction.Greater, 0.0f);
						RenderFace(ref AlphaList[i], cx, cy, cz);
					} else {
						if (depthMask) {
							GL.DepthMask(false);
							depthMask = false;
						}
						SetAlphaFunc(AlphaFunction.Less, 1.0f);
						RenderFace(ref AlphaList[i], cx, cy, cz);
						GL.DepthMask(true);
						depthMask = true;
						SetAlphaFunc(AlphaFunction.Equal, 1.0f);
						RenderFace(ref AlphaList[i], cx, cy, cz);
					}
				}
			} else {
				BlendEnabled = true; GL.Enable(EnableCap.Blend);
				GL.DepthMask(false);
				SetAlphaFunc(AlphaFunction.Greater,  0.0f);
				for (int i = 0; i < AlphaListCount; i++) {
					RenderFace(ref AlphaList[i], cx, cy, cz);
				}
			}
            // overlay list
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            if (FogEnabled)
            {
                GL.Disable(EnableCap.Fog); FogEnabled = false;
            }
            SortPolygons(OverlayList, OverlayListCount, OverlayListDistance, 3, 0.0);
            for (int i = 0; i < OverlayListCount; i++)
            {
                RenderFace(ref OverlayList[i], cx, cy, cz);
            }
            // render overlays
            BlendEnabled = false; GL.Disable(EnableCap.Blend);
            SetAlphaFunc(AlphaFunction.Greater, 0.9f);
            AlphaTestEnabled = false; GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
            if (LightingEnabled)
            {
                GL.Disable(EnableCap.Lighting);
                LightingEnabled = false;
            }
			PrepareCoordinates(OptionCoordinateSystem);
            PrepareOverlays();
            // finalize rendering
            GL.PopMatrix();
            renderer.RenderAll();
        }

        // set alpha func
        private static void SetAlphaFunc(AlphaFunction Comparison, float Value)
        {
            AlphaFuncComparison = Comparison;
            AlphaFuncValue = Value;
            GL.AlphaFunc(Comparison, Value);
        }

        // render face
        private static int LastBoundTexture = 0;
        private static void RenderFace(ref ObjectFace Face, double CameraX, double CameraY, double CameraZ)
        {
            if (CullEnabled)
            {
                if (!OptionBackfaceCulling || (ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Flags & World.MeshFace.Face2Mask) != 0)
                {
                    GL.Disable(EnableCap.CullFace);
                    CullEnabled = false;
                }
            }
            else if (OptionBackfaceCulling)
            {
                if ((ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Flags & World.MeshFace.Face2Mask) == 0)
                {
                    GL.Enable(EnableCap.CullFace);
                    CullEnabled = true;
                }
            }
            int r = (int)ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Material;
            RenderFace(ref ObjectManager.Objects[Face.ObjectIndex].Mesh.Materials[r], ObjectManager.Objects[Face.ObjectIndex].Mesh.Vertices, ref ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex], CameraX, CameraY, CameraZ);
        }
        private static void RenderFace(ref World.MeshMaterial Material, World.Vertex[] Vertices, ref World.MeshFace Face, double CameraX, double CameraY, double CameraZ)
        {
            // texture
            int OpenGlNighttimeTextureIndex = Material.NighttimeTextureIndex >= 0 ? TextureManager.UseTexture(Material.NighttimeTextureIndex, TextureManager.UseMode.Normal) : 0;
            int OpenGlDaytimeTextureIndex = Material.DaytimeTextureIndex >= 0 ? TextureManager.UseTexture(Material.DaytimeTextureIndex, TextureManager.UseMode.Normal) : 0;
            if (OpenGlDaytimeTextureIndex != 0)
            {
                if (!TexturingEnabled)
                {
                    GL.Enable(EnableCap.Texture2D);
                    TexturingEnabled = true;
                }
                if (OpenGlDaytimeTextureIndex != LastBoundTexture)
                {
                    GL.BindTexture(TextureTarget.Texture2D, OpenGlDaytimeTextureIndex);
                    LastBoundTexture = OpenGlDaytimeTextureIndex;
                }
                if (TextureManager.Textures[Material.DaytimeTextureIndex].Transparency != TextureManager.TextureTransparencyMode.None)
                {
                    if (!AlphaTestEnabled)
                    {
                        GL.Enable(EnableCap.AlphaTest);
                        AlphaTestEnabled = true;
                    }
                }
                else if (AlphaTestEnabled)
                {
                    GL.Disable(EnableCap.AlphaTest);
                    AlphaTestEnabled = false;
                }
            }
            else
            {
                if (TexturingEnabled)
                {
                    GL.Disable(EnableCap.Texture2D);
                    TexturingEnabled = false;
                    LastBoundTexture = 0;
                }
                if (AlphaTestEnabled)
                {
                    GL.Disable(EnableCap.AlphaTest);
                    AlphaTestEnabled = false;
                }
            }
            // blend mode
            float factor;
            if (Material.BlendMode == World.MeshMaterialBlendMode.Additive)
            {
                factor = 1.0f;
                if (!BlendEnabled) GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
                if (FogEnabled)
                {
                    GL.Disable(EnableCap.Fog);
                }
            }
            else if (OpenGlNighttimeTextureIndex == 0)
            {
                float blend = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                if (blend > 1.0f) blend = 1.0f;
                factor = 1.0f - 0.8f * blend;
            }
            else
            {
                factor = 1.0f;
            }
            if (OpenGlNighttimeTextureIndex != 0)
            {
                if (LightingEnabled)
                {
                    GL.Disable(EnableCap.Lighting);
                    LightingEnabled = false;
                }
            }
            else
            {
                if (OptionLighting & !LightingEnabled)
                {
                    GL.Enable(EnableCap.Lighting);
                    LightingEnabled = true;
                }
            }
            // render daytime polygon
            int FaceType = Face.Flags & World.MeshFace.FaceTypeMask;
            switch (FaceType)
            {
                case World.MeshFace.FaceTypeTriangles:
                    GL.Begin(PrimitiveType.Triangles);
                    break;
                case World.MeshFace.FaceTypeTriangleStrip:
                    GL.Begin(PrimitiveType.TriangleStrip);
                    break;
                case World.MeshFace.FaceTypeQuads:
                    GL.Begin(PrimitiveType.Quads);
                    break;
                case World.MeshFace.FaceTypeQuadStrip:
                    GL.Begin(PrimitiveType.QuadStrip);
                    break;
                default:
                    GL.Begin(PrimitiveType.Polygon);
                    break;
            }
            if (Material.GlowAttenuationData != 0)
            {
                float alphafactor = (float)GetDistanceFactor(Vertices, ref Face, Material.GlowAttenuationData, CameraX, CameraY, CameraZ);
                GL.Color4(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A * alphafactor);
            }
            else
            {
                GL.Color4(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A);
            }
            if ((Material.Flags & World.MeshMaterial.EmissiveColorMask) != 0)
            {
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { inv255 * (float)Material.EmissiveColor.R, inv255 * (float)Material.EmissiveColor.G, inv255 * (float)Material.EmissiveColor.B, 1.0f });
                EmissiveEnabled = true;
            }
            else if (EmissiveEnabled)
            {
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                EmissiveEnabled = false;
            }
            if (OpenGlDaytimeTextureIndex != 0)
            {
                if (LightingEnabled)
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        GL.Normal3(Face.Vertices[j].Normal.X, Face.Vertices[j].Normal.Y, Face.Vertices[j].Normal.Z);
                        GL.TexCoord2(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                        GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
                else
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        GL.TexCoord2(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                        GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
            }
            else
            {
                if (LightingEnabled)
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        GL.Normal3(Face.Vertices[j].Normal.X, Face.Vertices[j].Normal.Y, Face.Vertices[j].Normal.Z);
                        GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
                else
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
            }
            GL.End();
            // render nighttime polygon
            if (OpenGlNighttimeTextureIndex != 0)
            {
                if (!TexturingEnabled)
                {
                    GL.Enable(EnableCap.Texture2D);
                    TexturingEnabled = true;
                }
                if (!BlendEnabled)
                {
                    GL.Enable(EnableCap.Blend);
                }
                GL.BindTexture(TextureTarget.Texture2D, OpenGlNighttimeTextureIndex);
                LastBoundTexture = 0;
                SetAlphaFunc(AlphaFunction.Greater, 0.0f);
                switch (FaceType)
                {
                    case World.MeshFace.FaceTypeTriangles:
                        GL.Begin(PrimitiveType.Triangles);
                        break;
                    case World.MeshFace.FaceTypeTriangleStrip:
                        GL.Begin(PrimitiveType.TriangleStrip);
                        break;
                    case World.MeshFace.FaceTypeQuads:
                        GL.Begin(PrimitiveType.Quads);
                        break;
                    case World.MeshFace.FaceTypeQuadStrip:
                        GL.Begin(PrimitiveType.QuadStrip);
                        break;
                    default:
                        GL.Begin(PrimitiveType.Polygon);
                        break;
                }
                float alphafactor;
                if (Material.GlowAttenuationData != 0)
                {
                    alphafactor = (float)GetDistanceFactor(Vertices, ref Face, Material.GlowAttenuationData, CameraX, CameraY, CameraZ);
                    float blend = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                    if (blend > 1.0f) blend = 1.0f;
                    alphafactor *= blend;
                }
                else
                {
                    alphafactor = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                    if (alphafactor > 1.0f) alphafactor = 1.0f;
                }
                GL.Color4(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A * alphafactor);
                if ((Material.Flags & World.MeshMaterial.EmissiveColorMask) != 0)
                {
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { inv255 * (float)Material.EmissiveColor.R, inv255 * (float)Material.EmissiveColor.G, inv255 * (float)Material.EmissiveColor.B, 1.0f });
                    EmissiveEnabled = true;
                }
                else if (EmissiveEnabled)
                {
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                    EmissiveEnabled = false;
                }
                for (int j = 0; j < Face.Vertices.Length; j++)
                {
                    GL.TexCoord2(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                    GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                }
                GL.End();
                if (AlphaFuncValue != 0.0)
                {
                    GL.AlphaFunc(AlphaFuncComparison, AlphaFuncValue);
                }
                if (!BlendEnabled)
                {
                    GL.Disable(EnableCap.Blend);
                }
            }
            // normals
            if (OptionNormals)
            {
                if (TexturingEnabled)
                {
                    GL.Disable(EnableCap.Texture2D);
                    TexturingEnabled = false;
                }
                if (AlphaTestEnabled)
                {
                    GL.Disable(EnableCap.AlphaTest);
                    AlphaTestEnabled = false;
                }
                for (int j = 0; j < Face.Vertices.Length; j++)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(inv255 * (float)Material.Color.R, inv255 * (float)Material.Color.G, inv255 * (float)Material.Color.B, 1.0f);
                    GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    GL.Vertex3((float)(Vertices[Face.Vertices[j].Index].Coordinates.X + Face.Vertices[j].Normal.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y + Face.Vertices[j].Normal.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z + Face.Vertices[j].Normal.Z - CameraZ));
                    GL.End();
                }
            }
            // finalize
            if (Material.BlendMode == World.MeshMaterialBlendMode.Additive)
            {
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                if (!BlendEnabled) GL.Disable(EnableCap.Blend);
                if (FogEnabled)
                {
                    GL.Enable(EnableCap.Fog);
                }
            }
        }

		private struct CoordinateHandles {
			internal TextureHandle redtex;
			internal TextureHandle greentex;
			internal TextureHandle bluetex;
			internal ObjectHandle x_box;
			internal ObjectHandle y_box;
			internal ObjectHandle z_box;
			internal bool enabled;
			internal bool created;
		}

		private static CoordinateHandles coordinate_handles = new CoordinateHandles { created = false, enabled = true };

		/// <summary>
		/// Prepare coordinates for rendering
		/// </summary>
		/// <param name="enabled">If coordinate lines are enabled</param>
		private static void PrepareCoordinates(bool enabled) {
			// Create all handles for the coordinates if they weren't created already
			if (coordinate_handles.created == false) {
				coordinate_handles.redtex = renderer.AddTextureFromColor(new Pixel(1.0f, 0, 0, 1.0f));
				coordinate_handles.greentex = renderer.AddTextureFromColor(new Pixel(0, 1.0f, 0, 1.0f));
				coordinate_handles.bluetex = renderer.AddTextureFromColor(new Pixel(0, 0, 1.0f, 1.0f));

				coordinate_handles.x_box = renderer.AddObject(renderer.CubeMesh(), coordinate_handles.redtex);
				coordinate_handles.y_box = renderer.AddObject(renderer.CubeMesh(), coordinate_handles.greentex);
				coordinate_handles.z_box = renderer.AddObject(renderer.CubeMesh(), coordinate_handles.bluetex);

				renderer.SetShading(coordinate_handles.x_box, false);
				renderer.SetShading(coordinate_handles.y_box, false);
				renderer.SetShading(coordinate_handles.z_box, false);

				renderer.SetScale(coordinate_handles.x_box, new Vector3(100.0f, 0.01f, 0.01f));
				renderer.SetScale(coordinate_handles.y_box, new Vector3(0.01f, 100.0f, 0.01f));
				renderer.SetScale(coordinate_handles.z_box, new Vector3(0.01f, 0.01f, 100.0f));

				coordinate_handles.created = true;
			}

			// If previous state is different, change visibility
			if (enabled ^ coordinate_handles.enabled) {
				renderer.SetVisibility(coordinate_handles.x_box, enabled);
				renderer.SetVisibility(coordinate_handles.y_box, enabled);
				renderer.SetVisibility(coordinate_handles.z_box, enabled);

				coordinate_handles.enabled = enabled;
			}
		}

        private struct OverlayHandles {
            internal List<KeyHandles> no_object_keys1;
            internal List<TextHandle> no_object_text;
            internal List<KeyHandles> object_keys1;
            internal List<KeyHandles> object_keys2;
            internal List<KeyHandles> object_keys3;
            internal List<KeyHandles> object_keys4;
            internal List<KeyHandles> object_keys5;
            internal List<KeyHandles> object_keys6;
            internal List<TextHandle> object_text;
            /// <summary>
            /// Tag to tell which rendering mode was the last:
            /// 0 = no interface;
            /// 1 = no object interface;
            /// 2 = object interface
            /// </summary>
            internal byte object_menus;
            internal bool created;
        }

        private static OverlayHandles overlay_handles = new OverlayHandles { created = false, object_menus = 0, no_object_text = new List<TextHandle>(), object_text = new List<TextHandle>() };

        private static void PrepareOverlays() {
            // Grab culture. Has effect on text rendering
            System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

            // Create handles for all possible keys and text that need to be displayed
            // Interface is different if:
            // - There is an object loaded
            // - There are multiple messages to display
            if (overlay_handles.created == false) {
                string[][] Keys;

                // Key icons to display when no object loaded
                Keys = new string[][] { new string[] { "F7" }, new string[] { "F8" } };
                overlay_handles.no_object_keys1 = CreateKeyHandles(new Position(4.0f, 4.0f), 24.0, Keys);

                // Key icons to display when an object is loaded
                Keys = new string[][] { new string[] { "F5" }, new string[] { "F7" }, new string[] { "del" }, new string[] { "F8" } };
                overlay_handles.object_keys1 = CreateKeyHandles(new Position(4.0f, 4.0f), 24.0, Keys);
                Keys = new string[][] { new string[] { "F" }, new string[] { "N" }, new string[] { "L" }, new string[] { "G" }, new string[] { "B" }, new string[] { "I" } };
                overlay_handles.object_keys2 = CreateKeyHandles(new Position(-20f, 4.0f, WindowOrigin.TopRight), 16.0, Keys);
                Keys = new string[][] { new string[] { null, "W", null }, new string[] { "A", "S", "D" } };
                overlay_handles.object_keys3 = CreateKeyHandles(new Position(4.0f, -40.0f, WindowOrigin.BottomLeft), 16.0, Keys);
                Keys = new string[][] { new string[] { null, "↑", null }, new string[] { "←", "↓", "→" } };
                overlay_handles.object_keys4 = CreateKeyHandles(new Position(-28.0f, -40.0f, WindowOrigin.BottomCenter), 16.0, Keys);
                Keys = new string[][] { new string[] { null, "8", "9" }, new string[] { "4", "5", "6" }, new string[] { null, "2", "3" } };
                overlay_handles.object_keys5 = CreateKeyHandles(new Position(-60.0f, -60.0f, WindowOrigin.BottomRight), 16.0, Keys);
                Keys = new string[][] { new string[] { "F9" } };
                overlay_handles.object_keys6 = CreateKeyHandles(new Position(4.0f, 92.0f), 24.0, Keys);

                // Text to display when no object loaded
                overlay_handles.no_object_text.Add(CreateTextHandles("Open one or more objects", new Position(32.0f, 4.0f), Fonts.FontType.Small));
                overlay_handles.no_object_text.Add(CreateTextHandles("Display the options window", new Position(32.0f, 24.0f), Fonts.FontType.Small));
                overlay_handles.no_object_text.Add(CreateTextHandles("v" + System.Windows.Forms.Application.ProductVersion, new Position(8.0f, 20.0f, WindowOrigin.BottomRight, ObjectOrigin.BottomRight), Fonts.FontType.Small));

                // Text to display when an object is loaded
                string current_position = "Position: " + World.AbsoluteCameraPosition.X.ToString("0.00", Culture) + ", " + World.AbsoluteCameraPosition.Y.ToString("0.00", Culture) + ", " + World.AbsoluteCameraPosition.Z.ToString("0.00", Culture);
                overlay_handles.object_text.Add(CreateTextHandles(current_position, new Position(0f, 4f, WindowOrigin.TopCenter, ObjectOrigin.TopCenter), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Reload the currently open objects", new Position(32f, 4f), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Open additional objects", new Position(32f, 24f), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Clear currently open objects", new Position(32f, 44f), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Display the options window", new Position(32f, 64f), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Wireframe: " + (Renderer.OptionWireframe ? "on" : "off"), new Position(-28f, 4f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Normals: " + (Renderer.OptionNormals ? "on" : "off"), new Position(-28f, 24f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Lighting: " + (Program.LightingTarget == 0 ? "night" : "day"), new Position(-28f, 44f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Grid: " + (Renderer.OptionCoordinateSystem ? "on" : "off"), new Position(-28f, 64f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Background: " + GetBackgroundColorName(), new Position(-28f, 84f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Hide interface", new Position(-28f, 104f, WindowOrigin.TopRight, ObjectOrigin.TopRight), Fonts.FontType.Small));
                overlay_handles.object_text.Add(CreateTextHandles("Display the " + Interface.MessageCount.ToString(Culture) + " messages recently generated.", new Position(32f, 92f), Fonts.FontType.Small));

                overlay_handles.created = true;
            }

            // overlay_handles.object_menus is a
            // tag to tell which rendering mode was the last
            // 0 = no interface
            // 1 = no object interface
            // 2 = object interface

			// TODO: Simplify these if/else blocks
            if (OptionInterface) {
                if (ObjectManager.ObjectsUsed == 0 & ObjectManager.AnimatedWorldObjectsUsed == 0) {
                    if (overlay_handles.object_menus != 1) {
                        SetKeyListVisibility(overlay_handles.no_object_keys1, true);
                        SetKeyListVisibility(overlay_handles.object_keys1, false);
                        SetKeyListVisibility(overlay_handles.object_keys2, false);
                        SetKeyListVisibility(overlay_handles.object_keys3, false);
                        SetKeyListVisibility(overlay_handles.object_keys4, false);
                        SetKeyListVisibility(overlay_handles.object_keys5, false);
                        SetKeyListVisibility(overlay_handles.object_keys6, false);
                        SetTextListVisibility(overlay_handles.no_object_text, true);
                        SetTextListVisibility(overlay_handles.object_text, false);

                        overlay_handles.object_menus = 1;
                    }
                }
                else {
                    if (overlay_handles.object_menus != 2) {
                        SetKeyListVisibility(overlay_handles.no_object_keys1, false);
                        SetKeyListVisibility(overlay_handles.object_keys1, true);
                        SetKeyListVisibility(overlay_handles.object_keys2, true);
                        SetKeyListVisibility(overlay_handles.object_keys3, true);
                        SetKeyListVisibility(overlay_handles.object_keys4, true);
                        SetKeyListVisibility(overlay_handles.object_keys5, true);
                        SetKeyListVisibility(overlay_handles.object_keys6, true);
                        SetTextListVisibility(overlay_handles.no_object_text, false);
                        SetTextListVisibility(overlay_handles.object_text, true);

                        overlay_handles.object_menus = 2;
                    }
                    string current_position = "Position: " + World.AbsoluteCameraPosition.X.ToString("0.00", Culture) + ", " + World.AbsoluteCameraPosition.Y.ToString("0.00", Culture) + ", " + World.AbsoluteCameraPosition.Z.ToString("0.00", Culture);
                    renderer.SetText(overlay_handles.object_text[0], current_position);
                    renderer.SetText(overlay_handles.object_text[5], "Wireframe: " + (Renderer.OptionWireframe ? "on" : "off"));
                    renderer.SetText(overlay_handles.object_text[6], "Normals: " + (Renderer.OptionNormals ? "on" : "off"));
                    renderer.SetText(overlay_handles.object_text[7], "Lighting: " + (Program.LightingTarget == 0 ? "night" : "day"));
                    renderer.SetText(overlay_handles.object_text[8], "Grid: " + (Renderer.OptionCoordinateSystem ? "on" : "off"));
                    renderer.SetText(overlay_handles.object_text[9], "Background: " + GetBackgroundColorName());
                    renderer.SetText(overlay_handles.object_text[11], "Display the " + Interface.MessageCount.ToString(Culture) + " messages recently generated.");
                }
            }
            else {
                if (overlay_handles.object_menus == 1) {
                    SetKeyListVisibility(overlay_handles.no_object_keys1, false);
                    SetTextListVisibility(overlay_handles.no_object_text, false);
                }
                else if (overlay_handles.object_menus == 2) {
                    SetKeyListVisibility(overlay_handles.object_keys1, false);
                    SetKeyListVisibility(overlay_handles.object_keys2, false);
                    SetKeyListVisibility(overlay_handles.object_keys3, false);
                    SetKeyListVisibility(overlay_handles.object_keys4, false);
                    SetKeyListVisibility(overlay_handles.object_keys5, false);
                    SetKeyListVisibility(overlay_handles.object_keys6, false);
                    SetTextListVisibility(overlay_handles.object_text, false);
                }
                overlay_handles.object_menus = 0;
            }
        }

        struct KeyHandles {
            internal UIElementHandle light;
            internal UIElementHandle middle;
            internal UIElementHandle dark;
            internal TextHandle text;
        }

        private struct GlobalKeyHandles {
            internal TextureHandle lighttex;
            internal TextureHandle darktex;
            internal TextureHandle midtex;
            internal bool created;
        }

        private static GlobalKeyHandles global_key_handles = new GlobalKeyHandles { created = false };

        /// <summary>
        /// Create renderer handles for the square displaying a key and its name
        /// </summary>
        /// <param name="Left">Pixel offset from left of the screen</param>
        /// <param name="Top">Pixel offset from top of the screen</param>
        /// <param name="Width">Width of the whole key icon</param>
        /// <param name="Keys">List of list of keys to create displays for. Keys[row][column]. Null strings allowed</param>
        /// <returns>List of handles for each key</returns>
        private static List<KeyHandles> CreateKeyHandles(Position position, double Width, string[][] Keys)
        {
            // Creates global variables if they aren't made yet.
            if (global_key_handles.created == false) {
                // If texture handles for key effects haven't been made yet, create them. 
                global_key_handles.darktex = renderer.AddTextureFromColor(new Pixel(0.25f, 0.25f, 0.25f, 0.5f));
                global_key_handles.lighttex = renderer.AddTextureFromColor(new Pixel(0.75f, 0.75f, 0.75f, 0.5f));
                global_key_handles.midtex = renderer.AddTextureFromColor(new Pixel(0.5f, 0.5f, 0.5f, 0.5f));

                global_key_handles.created = true;
            }

            List<KeyHandles> handles = new List<KeyHandles>();

			// All keys are made by three colored boxes and the text of the button.
			// The dark box is to the left and up, the light box is the size of the button,
			// and the mid box is in the middle. 12pt text is overlayed on top of everything.
			Position current_pos = position;
            for (int y = 0; y < Keys.Length; y++) {
                current_pos.position.X = position.position.X;
                for (int x = 0; x < Keys[y].Length; x++) {
                    if (Keys[y][x] != null) {
                        var lightpos = current_pos;
                        var darkpos = current_pos + new Vector2(2.0f, 1.0f);
                        var midpos = current_pos + new Vector2(1.0f);

                        lightpos.object_origin = ObjectOrigin.TopLeft;
                        darkpos.object_origin = ObjectOrigin.TopLeft;
                        midpos.object_origin = ObjectOrigin.TopLeft;

						UIElementHandle lightsquare = renderer.AddUIElement(renderer.SquareMesh(), global_key_handles.lighttex, lightpos, new Vector2((float)Width - 4, 14), 0, -1);
						UIElementHandle darksquare = renderer.AddUIElement(renderer.SquareMesh(), global_key_handles.darktex, darkpos, new Vector2((float)Width - 4, 15), 0, -3);
						UIElementHandle midsquare = renderer.AddUIElement(renderer.SquareMesh(), global_key_handles.midtex, midpos, new Vector2((float)Width - 5, 14), 0, -2);

						Position textpos = current_pos + new Vector2(2f, 0f);
                        Font font = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
						TextHandle text = renderer.AddText(Keys[y][x], font , new Pixel(1.0f, 1.0f, 1.0f, 1.0f), textpos);

                        handles.Add(new KeyHandles { light = lightsquare, middle = midsquare, dark = darksquare, text = text });
                    }
                    // Next button width plus padding
                    current_pos.position.X += (float) (Width + 4.0);
                }
                // Fixed padding because of fixed 12pt font size.
                current_pos.position.Y += 20.0f;
            }

            return handles;
        }

        /// <summary>
        /// Sets every key handle in a list to specific visibility
        /// </summary>
        /// <param name="list">List of key handles to change</param>
        /// <param name="vis">true is visible, false is not</param>
        private static void SetKeyListVisibility(List<KeyHandles> list, bool vis) {
            foreach (KeyHandles kh in list) {
                renderer.SetVisibility(kh.text, vis);
                renderer.SetVisibility(kh.light, vis);
                renderer.SetVisibility(kh.middle, vis);
                renderer.SetVisibility(kh.dark, vis);
            }
        }

        /// <summary>
        /// Sets every text handle handle in a list to specific visibility
        /// </summary>
        /// <param name="list">List of text handles to change</param>
        /// <param name="vis">true is visible, false is not</param>
        private static void SetTextListVisibility(List<TextHandle> list, bool vis) {
            foreach (TextHandle th in list) {
                renderer.SetVisibility(th, vis);
            }
        }

        /// <summary>
        /// Create renderer handles for a string of text
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="position">Position of text</param>
        /// <param name="fonttype">Size of the font</param>
        /// <param name="r">Red component of the color of the text (0-1)</param>
        /// <param name="g">Green component of the color of the text (0-1)</param>
        /// <param name="b">Blue component of the color of the text (0-1)</param>
        /// <param name="a">Alpha component of the color of the text (0-1). 1 is opaque</param>
        /// <param name="shadow">Ignored</param>
        /// <returns>Handle that was created</returns>
        private static TextHandle CreateTextHandles(string text, Position position, Fonts.FontType fonttype, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f, bool shadow = true) {
            Font font = Fonts.ConverttoFont(fonttype);
            return renderer.AddText(text, font, new Pixel(r, g, b, a), position);
        }

        internal static void CameraZoom(float movement) {
            float dist = Renderer.renderer.GetDistance(Renderer.renderer.GetActiveCamera());
            dist += 0.25f * movement;
            Renderer.renderer.SetDistance(Renderer.renderer.GetActiveCamera(), dist);
        }

        internal static void CameraRotate(Vector2 direction) {
            Vector2 movement  = direction * 0.25f;
            movement += Renderer.renderer.GetRotation(Renderer.renderer.GetActiveCamera());
            Renderer.renderer.SetRotation(Renderer.renderer.GetActiveCamera(), movement);
        }

        internal static void CameraMove(Vector2 direction) {
            Vector2 currenteyevec = Renderer.renderer.GetEyeVector(Renderer.renderer.GetActiveCamera()).Xz.Normalized();
            Vector2 leftvec = new Vector2(-currenteyevec.Y, currenteyevec.X);
            Vector2 movement  = -direction.Y * currenteyevec;
            movement += direction.X * leftvec;
            movement *= 0.1f;
            movement += Renderer.renderer.GetLocation(Renderer.renderer.GetActiveCamera()).Xz;
            Renderer.renderer.SetLocation(Renderer.renderer.GetActiveCamera(), new Vector3(movement.X, 0, movement.Y));
        }

        // readd objects
        private static void ReAddObjects()
        {
            Object[] List = new Object[ObjectListCount];
            for (int i = 0; i < ObjectListCount; i++)
            {
                List[i] = ObjectList[i];
            }
            for (int i = 0; i < List.Length; i++)
            {
                HideObject(List[i].ObjectIndex);
            }
            for (int i = 0; i < List.Length; i++)
            {
                ShowObject(List[i].ObjectIndex, List[i].Type);
            }
        }

        // show object
        internal static void ShowObject(int ObjectIndex, ObjectType Type)
        {
            bool Overlay = Type == ObjectType.Overlay;
            if (ObjectManager.Objects[ObjectIndex] == null) return;
            if (ObjectManager.Objects[ObjectIndex].RendererIndex == 0)
            {
                if (ObjectListCount >= ObjectList.Length)
                {
                    Array.Resize<Object>(ref ObjectList, ObjectList.Length << 1);
                }
                ObjectList[ObjectListCount].ObjectIndex = ObjectIndex;
                ObjectList[ObjectListCount].Type = Type;
                int f = ObjectManager.Objects[ObjectIndex].Mesh.Faces.Length;
                ObjectList[ObjectListCount].FaceListIndices = new int[f];
                for (int i = 0; i < f; i++)
                {
                    if (Overlay)
                    {
                        // overlay
                        if (OverlayListCount >= OverlayList.Length)
                        {
                            Array.Resize(ref OverlayList, OverlayList.Length << 1);
                            Array.Resize(ref OverlayListDistance, OverlayList.Length);
                        }
                        OverlayList[OverlayListCount].ObjectIndex = ObjectIndex;
                        OverlayList[OverlayListCount].FaceIndex = i;
                        OverlayList[OverlayListCount].ObjectListIndex = ObjectListCount;
                        ObjectList[ObjectListCount].FaceListIndices[i] = (OverlayListCount << 2) + 3;
                        OverlayListCount++;
                    }
                    else
                    {
                        int k = ObjectManager.Objects[ObjectIndex].Mesh.Faces[i].Material;
                        bool transparentcolor = false, alpha = false;
                        if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].Color.A != 255)
                        {
                            alpha = true;
                        }
                        else if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].BlendMode == World.MeshMaterialBlendMode.Additive)
                        {
                            alpha = true;
                        }
                        else if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].GlowAttenuationData != 0)
                        {
                            alpha = true;
                        }
                        else
                        {
                            int tday = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].DaytimeTextureIndex;
                            if (tday >= 0)
                            {
                                TextureManager.UseTexture(tday, TextureManager.UseMode.Normal);
                                if (TextureManager.Textures[tday].Transparency == TextureManager.TextureTransparencyMode.Alpha)
                                {
                                    alpha = true;
                                }
                                else if (TextureManager.Textures[tday].Transparency == TextureManager.TextureTransparencyMode.TransparentColor)
                                {
                                    transparentcolor = true;
                                }
                            }
                            int tnight = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].NighttimeTextureIndex;
                            if (tnight >= 0)
                            {
                                TextureManager.UseTexture(tnight, TextureManager.UseMode.Normal);
                                if (TextureManager.Textures[tnight].Transparency == TextureManager.TextureTransparencyMode.Alpha)
                                {
                                    alpha = true;
                                }
                                else if (TextureManager.Textures[tnight].Transparency == TextureManager.TextureTransparencyMode.TransparentColor)
                                {
                                    transparentcolor = true;
                                }
                            }
                        }
                        if (alpha)
                        {
                            // alpha
                            if (AlphaListCount >= AlphaList.Length)
                            {
                                Array.Resize(ref AlphaList, AlphaList.Length << 1);
                                Array.Resize(ref AlphaListDistance, AlphaList.Length);
                            }
                            AlphaList[AlphaListCount].ObjectIndex = ObjectIndex;
                            AlphaList[AlphaListCount].FaceIndex = i;
                            AlphaList[AlphaListCount].ObjectListIndex = ObjectListCount;
                            ObjectList[ObjectListCount].FaceListIndices[i] = (AlphaListCount << 2) + 2;
                            AlphaListCount++;
                        }
                        else if (transparentcolor)
                        {
                            // transparent color
                            if (TransparentColorListCount >= TransparentColorList.Length)
                            {
                                Array.Resize(ref TransparentColorList, TransparentColorList.Length << 1);
                                Array.Resize(ref TransparentColorListDistance, TransparentColorList.Length);
                            }
                            TransparentColorList[TransparentColorListCount].ObjectIndex = ObjectIndex;
                            TransparentColorList[TransparentColorListCount].FaceIndex = i;
                            TransparentColorList[TransparentColorListCount].ObjectListIndex = ObjectListCount;
                            ObjectList[ObjectListCount].FaceListIndices[i] = (TransparentColorListCount << 2) + 1;
                            TransparentColorListCount++;
                        }
                        else
                        {
                            // opaque
                            if (OpaqueListCount >= OpaqueList.Length)
                            {
                                Array.Resize(ref OpaqueList, OpaqueList.Length << 1);
                            }
                            OpaqueList[OpaqueListCount].ObjectIndex = ObjectIndex;
                            OpaqueList[OpaqueListCount].FaceIndex = i;
                            OpaqueList[OpaqueListCount].ObjectListIndex = ObjectListCount;
                            ObjectList[ObjectListCount].FaceListIndices[i] = OpaqueListCount << 2;
                            OpaqueListCount++;
                        }
                    }
                }
                ObjectManager.Objects[ObjectIndex].RendererIndex = ObjectListCount + 1;
                ObjectListCount++;
            }
        }

        // hide object
        internal static void HideObject(int ObjectIndex)
        {
            if (ObjectManager.Objects[ObjectIndex] == null) return;
            int k = ObjectManager.Objects[ObjectIndex].RendererIndex - 1;
            if (k >= 0)
            {
                // remove faces
                for (int i = 0; i < ObjectList[k].FaceListIndices.Length; i++)
                {
                    int h = ObjectList[k].FaceListIndices[i];
                    int hi = h >> 2;
                    switch (h & 3)
                    {
                        case 0:
                            // opaque
                            OpaqueList[hi] = OpaqueList[OpaqueListCount - 1];
                            OpaqueListCount--;
                            ObjectList[OpaqueList[hi].ObjectListIndex].FaceListIndices[OpaqueList[hi].FaceIndex] = h;
                            break;
                        case 1:
                            // transparent color
                            TransparentColorList[hi] = TransparentColorList[TransparentColorListCount - 1];
                            TransparentColorListCount--;
                            ObjectList[TransparentColorList[hi].ObjectListIndex].FaceListIndices[TransparentColorList[hi].FaceIndex] = h;
                            break;
                        case 2:
                            // alpha
                            AlphaList[hi] = AlphaList[AlphaListCount - 1];
                            AlphaListCount--;
                            ObjectList[AlphaList[hi].ObjectListIndex].FaceListIndices[AlphaList[hi].FaceIndex] = h;
                            break;
                        case 3:
                            // overlay
                            OverlayList[hi] = OverlayList[OverlayListCount - 1];
                            OverlayListCount--;
                            ObjectList[OverlayList[hi].ObjectListIndex].FaceListIndices[OverlayList[hi].FaceIndex] = h;
                            break;
                    }
                }
                // remove object
                if (k == ObjectListCount - 1)
                {
                    ObjectListCount--;
                }
                else
                {
                    ObjectList[k] = ObjectList[ObjectListCount - 1];
                    ObjectListCount--;
                    for (int i = 0; i < ObjectList[k].FaceListIndices.Length; i++)
                    {
                        int h = ObjectList[k].FaceListIndices[i];
                        int hi = h >> 2;
                        switch (h & 3)
                        {
                            case 0:
                                OpaqueList[hi].ObjectListIndex = k;
                                break;
                            case 1:
                                TransparentColorList[hi].ObjectListIndex = k;
                                break;
                            case 2:
                                AlphaList[hi].ObjectListIndex = k;
                                break;
                            case 3:
                                OverlayList[hi].ObjectListIndex = k;
                                break;
                        }
                    }
                    ObjectManager.Objects[ObjectList[k].ObjectIndex].RendererIndex = k + 1;
                }
                ObjectManager.Objects[ObjectIndex].RendererIndex = 0;
            }
        }

        // sort polygons
        private static void SortPolygons(ObjectFace[] List, int ListCount, double[] ListDistance, int ListOffset, double TimeElapsed)
        {
            // calculate distance
            double cx = World.AbsoluteCameraPosition.X;
            double cy = World.AbsoluteCameraPosition.Y;
            double cz = World.AbsoluteCameraPosition.Z;
            for (int i = 0; i < ListCount; i++)
            {
                int o = List[i].ObjectIndex;
                int f = List[i].FaceIndex;
                if (ObjectManager.Objects[o].Mesh.Faces[f].Vertices.Length >= 3)
                {
                    int v0 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[0].Index;
                    int v1 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[1].Index;
                    int v2 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[2].Index;
                    double v0x = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.X;
                    double v0y = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.Y;
                    double v0z = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.Z;
                    double v1x = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.X;
                    double v1y = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.Y;
                    double v1z = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.Z;
                    double v2x = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.X;
                    double v2y = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.Y;
                    double v2z = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.Z;
                    double w1x = v1x - v0x, w1y = v1y - v0y, w1z = v1z - v0z;
                    double w2x = v2x - v0x, w2y = v2y - v0y, w2z = v2z - v0z;
                    double dx = -w1z * w2y + w1y * w2z;
                    double dy = w1z * w2x - w1x * w2z;
                    double dz = -w1y * w2x + w1x * w2y;
                    double t = dx * dx + dy * dy + dz * dz;
                    if (t != 0.0)
                    {
                        t = 1.0 / Math.Sqrt(t);
                        dx *= t; dy *= t; dz *= t;
                        double w0x = v0x - cx, w0y = v0y - cy, w0z = v0z - cz;
                        t = dx * w0x + dy * w0y + dz * w0z;
                        ListDistance[i] = -t * t;
                    }
                }
            }
            // sort
            Array.Sort<double, ObjectFace>(ListDistance, List, 0, ListCount);
            // update object list
            for (int i = 0; i < ListCount; i++)
            {
                ObjectList[List[i].ObjectListIndex].FaceListIndices[List[i].FaceIndex] = (i << 2) + ListOffset;
            }
        }

        // get distance factor
        private static double GetDistanceFactor(World.Vertex[] Vertices, ref World.MeshFace Face, ushort GlowAttenuationData, double CameraX, double CameraY, double CameraZ)
        {
            if (Face.Vertices.Length != 0)
            {
                double halfdistance;
                World.GlowAttenuationMode mode;
				World.SplitGlowAttenuationData(GlowAttenuationData, out mode, out halfdistance);
				int i = (int)Face.Vertices[0].Index;
                double dx = Vertices[i].Coordinates.X - CameraX;
                double dy = Vertices[i].Coordinates.Y - CameraY;
                double dz = Vertices[i].Coordinates.Z - CameraZ;
                switch (mode)
                {
                    case World.GlowAttenuationMode.DivisionExponent2:
                        {
                            double t = dx * dx + dy * dy + dz * dz;
                            return t / (t + halfdistance * halfdistance);
                        }
                    case World.GlowAttenuationMode.DivisionExponent4:
                        {
                            double t = dx * dx + dy * dy + dz * dz;
                            t *= t; halfdistance *= halfdistance;
                            return t / (t + halfdistance * halfdistance);
                        }
                    default:
                        return 1.0;
                }
            }
            else
            {
                return 1.0;
            }
        }

    }
}