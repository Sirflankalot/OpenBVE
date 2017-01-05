﻿using System;
using System.Globalization;
using OpenBveApi.Packages;

namespace OpenBve
{
	internal partial class Interface
	{
		/// <summary>Defines the levels of motion blur</summary>
		internal enum MotionBlurMode
		{
			/// <summary>Motion blur is disabled</summary>
			None = 0,
			/// <summary>Low motion blur</summary>
			Low = 1,
			/// <summary>Medium motion blur</summary>
			Medium = 2,
			/// <summary>High motion blur</summary>
			High = 3
		}

		/// <summary>Defines the range at which a sound will be loaded</summary>
		internal enum SoundRange
		{
			Low = 0,
			Medium = 1,
			High = 2
		}

		/// <summary>Defines the possible timetable display modes</summary>
		internal enum TimeTableMode
		{
			/// <summary>Timetable display is disabled</summary>
			None = 0,
			/// <summary>The custom timetable (if set) will be displayed first, followed by the auto-generated timetable</summary>
			Default = 1,
			/// <summary>Only the auto-generated timetable will be displayed</summary>
			AutoGenerated = 2,
			/// <summary>The auto-generated timetable will be displayed only if no custom timetable is available</summary>
			PreferCustom = 3
		}
		internal enum GameMode
		{
			Arcade = 0,
			Normal = 1,
			Expert = 2,
			Developer = 3
		}
		internal enum InterpolationMode
		{
			NearestNeighbor,
			Bilinear,
			NearestNeighborMipmapped,
			BilinearMipmapped,
			TrilinearMipmapped,
			AnisotropicFiltering
		}
		internal class Options
		{
			/// <summary>The ISO 639-1 code for the current user interface language</summary>
			internal string LanguageCode;
			/// <summary>Whether the program is to be run in full-screen mode</summary>
			internal bool FullscreenMode;
			/// <summary>Whether the program is to be rendered using vertical syncronisation</summary>
			internal bool VerticalSynchronization;
			/// <summary>The screen width (Windowed Mode)</summary>
			internal int WindowWidth;
			/// <summary>The screen height (Windowed Mode)</summary>
			internal int WindowHeight;
			/// <summary>The screen width (Fullscreen Mode)</summary>
			internal int FullscreenWidth;
			/// <summary>The screen height (Fullscreen Mode)</summary>
			internal int FullscreenHeight;
			/// <summary>The number of bits per pixel (Only relevant in fullscreen mode)</summary>
			internal int FullscreenBits;
			/// <summary>The on disk folder in which user interface components are stored</summary>
			internal string UserInterfaceFolder;
			/// <summary>The current pixel interpolation mode </summary>
			internal InterpolationMode Interpolation;
			/// <summary>The current transparency quality mode</summary>
			internal Renderer.TransparencyMode TransparencyMode;
			/// <summary>The level of anisotropic filtering to be applied</summary>
			internal int AnisotropicFilteringLevel;
			/// <summary>The maximum level of anisotropic filtering supported by the system</summary>
			internal int AnisotropicFilteringMaximum;
			/// <summary>The accelerated time factor (1x to 5x)</summary>
			internal int TimeAccelerationFactor;
			/// <summary>The level of antialiasing to be applied</summary>
			internal int AntiAliasingLevel;
			/// <summary>The viewing distance in meters</summary>
			internal int ViewingDistance;
			/// <summary>The current type of motion blur</summary>
			internal MotionBlurMode MotionBlur;
			/*
			 * Note: Object optimisation takes time whilst loading, but may increase the render performance of an
			 * object by checking for duplicate vertices etc.
			 */
			/// <summary>The minimum number of vertices for basic optimisation to be performed on an object</summary>
			internal int ObjectOptimizationBasicThreshold;
			/// <summary>The minimum number of verticies for full optimisation to be performed on an object</summary>
			internal int ObjectOptimizationFullThreshold;
			/// <summary>Whether duplicate verticies are culled during loading</summary>
			internal bool ObjectOptimizationVertexCulling;
			/// <summary>Whether toppling is enabled</summary>
			internal bool Toppling;
			/// <summary>Whether collisions between trains are enabled</summary>
			internal bool Collisions;
			/// <summary>Whether derailments are enabled</summary>
			internal bool Derailments;
			/// <summary>Whether the black-box data logger is enabled</summary>
			internal bool BlackBox;
			/// <summary>Whether joystick support is enabled</summary>
			internal bool UseJoysticks;
			/// <summary>The threshold below which joystick axis motion will be disregarded</summary>
			internal double JoystickAxisThreshold;
			/// <summary>The delay after which a held-down key will start to repeat</summary>
			internal double KeyRepeatDelay;
			/// <summary>The interval at which a held down key will repeat after the intial delay</summary>
			internal double KeyRepeatInterval;
			/// <summary>The current sound model</summary>
			internal Sounds.SoundModels SoundModel;
			/// <summary>The range outside of which sounds will be inaudible</summary>
			internal SoundRange SoundRange;
			/// <summary>The maximum number of sounds playing at any one time</summary>
			internal int SoundNumber;
			/// <summary>Whether warning messages are to be shown</summary>
			internal bool ShowWarningMessages;
			/// <summary>Whether error messages are to be shown</summary>
			internal bool ShowErrorMessages;
			/// <summary>The current route's on-disk folder path</summary>
			internal string RouteFolder;
			/// <summary>The current train's on-disk folder path</summary>
			internal string TrainFolder;
			/// <summary>The list of recently used routes</summary>
			internal string[] RecentlyUsedRoutes;
			/// <summary>The list of recently used trains</summary>
			internal string[] RecentlyUsedTrains;
			/// <summary>The maximum number of recently used routes/ trains to display</summary>
			internal int RecentlyUsedLimit;
			/// <summary>The list of recently used route character encodings</summary>
			internal TextEncoding.EncodingValue[] RouteEncodings;
			/// <summary>The list of recently used train character encodings</summary>
			internal TextEncoding.EncodingValue[] TrainEncodings;
			/// <summary>The game mode- Affects how the score is calculated</summary>
			internal GameMode GameMode;
			/// <summary>The width of the main menu window</summary>
			internal int MainMenuWidth;
			/// <summary>The height of the main menu window</summary>
			internal int MainMenuHeight;
			/// <summary>Whether the use of OpenGL display lists is disabled</summary>
			internal bool DisableDisplayLists;
			/// <summary>Whether the simulation will load all textures and sounds into system memory on initial load</summary>
			internal bool LoadInAdvance;
			/// <summary>Whether the simulation will dynamically unload unused textures</summary>
			internal bool UnloadUnusedTextures;
			/// <summary>Whether EB application is possible from the use of a joystick axis</summary>
			internal bool AllowAxisEB;
			/// <summary>Whether to prefer the native OpenTK operating system backend</summary>
			internal bool PreferNativeBackend = true;

			internal TimeTableMode TimeTableStyle;

			internal CompressionType packageCompressionType;
			/*
			 * Note: Disabling texture resizing may produce artifacts at the edges of textures,
			 * and may display issues with certain graphics cards.
			 */
			/// <summary>Whether textures are to be resized to the power of two rule</summary>
			internal bool NoTextureResize;
			/*
			 * Note: The following options were (are) used by the Managed Content system, and are currently non-functional
			 */
			/// <summary>The proxy URL to use when retrieving content from the internet</summary>
			internal string ProxyUrl;
			/// <summary>The proxy username to use when retrieving content from the internet</summary>
			internal string ProxyUserName;
			/// <summary>The proxy password to use when retrieving content from the internet</summary>
			internal string ProxyPassword;
			/// <summary>Creates a new instance of the options class with default values set</summary>
			internal Options()
			{
				this.LanguageCode = "en-US";
				this.FullscreenMode = false;
				this.VerticalSynchronization = true;
				this.WindowWidth = 960;
				this.WindowHeight = 600;
				this.FullscreenWidth = 1024;
				this.FullscreenHeight = 768;
				this.FullscreenBits = 32;
				this.UserInterfaceFolder = "Default";
				this.Interpolation = Interface.InterpolationMode.BilinearMipmapped;
				this.TransparencyMode = Renderer.TransparencyMode.Quality;
				this.AnisotropicFilteringLevel = 0;
				this.AnisotropicFilteringMaximum = 0;
				this.AntiAliasingLevel = 0;
				this.ViewingDistance = 600;
				this.MotionBlur = MotionBlurMode.None;
				this.Toppling = true;
				this.Collisions = true;
				this.Derailments = true;
				this.GameMode = GameMode.Normal;
				this.BlackBox = false;
				this.UseJoysticks = true;
				this.JoystickAxisThreshold = 0.0;
				this.KeyRepeatDelay = 0.5;
				this.KeyRepeatInterval = 0.1;
				this.SoundModel = Sounds.SoundModels.Inverse;
				this.SoundRange = SoundRange.Low;
				this.SoundNumber = 16;
				this.ShowWarningMessages = true;
				this.ShowErrorMessages = true;
				this.ObjectOptimizationBasicThreshold = 10000;
				this.ObjectOptimizationFullThreshold = 1000;
				this.ObjectOptimizationVertexCulling = false;
				this.RouteFolder = "";
				this.TrainFolder = "";
				this.RecentlyUsedRoutes = new string[] { };
				this.RecentlyUsedTrains = new string[] { };
				this.RecentlyUsedLimit = 10;
				this.RouteEncodings = new TextEncoding.EncodingValue[] { };
				this.TrainEncodings = new TextEncoding.EncodingValue[] { };
				this.MainMenuWidth = 0;
				this.MainMenuHeight = 0;
				this.DisableDisplayLists = false;
				this.LoadInAdvance = false;
				this.UnloadUnusedTextures = false;
				this.NoTextureResize = false;
				this.ProxyUrl = string.Empty;
				this.ProxyUserName = string.Empty;
				this.ProxyPassword = string.Empty;
				this.TimeAccelerationFactor = 5;
				this.AllowAxisEB = true;
				this.TimeTableStyle = TimeTableMode.Default;
				this.packageCompressionType = CompressionType.Zip;
			}
		}
		/// <summary>The current game options</summary>
		internal static Options CurrentOptions;
		/// <summary>Loads the options file from disk</summary>
		internal static void LoadOptions()
		{
			CurrentOptions = new Options();
			CultureInfo Culture = CultureInfo.InvariantCulture;
			string File = OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "options.cfg");
			if (System.IO.File.Exists(File))
			{
				// load options
				string[] Lines = System.IO.File.ReadAllLines(File, new System.Text.UTF8Encoding());
				string Section = "";
				for (int i = 0; i < Lines.Length; i++)
				{
					Lines[i] = Lines[i].Trim();
					if (Lines[i].Length != 0 && !Lines[i].StartsWith(";", StringComparison.OrdinalIgnoreCase))
					{
						if (Lines[i].StartsWith("[", StringComparison.Ordinal) & Lines[i].EndsWith("]", StringComparison.Ordinal))
						{
							Section = Lines[i].Substring(1, Lines[i].Length - 2).Trim().ToLowerInvariant();
						}
						else
						{
							int j = Lines[i].IndexOf("=", StringComparison.OrdinalIgnoreCase);
							string Key, Value;
							if (j >= 0)
							{
								Key = Lines[i].Substring(0, j).TrimEnd().ToLowerInvariant();
								Value = Lines[i].Substring(j + 1).TrimStart();
							}
							else
							{
								Key = "";
								Value = Lines[i];
							}
							switch (Section)
							{
								case "language":
									switch (Key)
									{
										case "code":
											Interface.CurrentOptions.LanguageCode = Value.Length != 0 ? Value : "en-US";
											break;
									} break;
								case "interface":
									switch (Key)
									{
										case "folder":
											Interface.CurrentOptions.UserInterfaceFolder = Value.Length != 0 ? Value : "Default";
											break;
										case "timetablemode":
											switch (Value.ToLowerInvariant())
											{
												case "none":
													Interface.CurrentOptions.TimeTableStyle = TimeTableMode.None;
													break;
												case "default":
													Interface.CurrentOptions.TimeTableStyle = TimeTableMode.Default;
													break;
												case "autogenerated":
													Interface.CurrentOptions.TimeTableStyle = TimeTableMode.AutoGenerated;
													break;
												case "prefercustom":
													Interface.CurrentOptions.TimeTableStyle = TimeTableMode.PreferCustom;
													break;
											}
											break;
									} break;
								case "display":
									switch (Key)
									{
										case "prefernativebackend":
											Interface.CurrentOptions.PreferNativeBackend = string.Compare(Value, "true", StringComparison.OrdinalIgnoreCase) == 0;
											break;
										case "mode":
											Interface.CurrentOptions.FullscreenMode = string.Compare(Value, "fullscreen", StringComparison.OrdinalIgnoreCase) == 0;
											break;
										case "vsync":
											Interface.CurrentOptions.VerticalSynchronization = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "windowwidth":
											{
												int a;
												if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a))
												{
													a = 960;
												}
												Interface.CurrentOptions.WindowWidth = a;
											} break;
										case "windowheight":
											{
												int a;
												if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a))
												{
													a = 600;
												}
												Interface.CurrentOptions.WindowHeight = a;
											} break;
										case "fullscreenwidth":
											{
												int a;
												if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a))
												{
													a = 1024;
												}
												Interface.CurrentOptions.FullscreenWidth = a;
											} break;
										case "fullscreenheight":
											{
												int a;
												if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a))
												{
													a = 768;
												}
												Interface.CurrentOptions.FullscreenHeight = a;
											} break;
										case "fullscreenbits":
											{
												int a;
												if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a))
												{
													a = 32;
												}
												Interface.CurrentOptions.FullscreenBits = a;
											} break;
										case "mainmenuwidth":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.MainMenuWidth = a;
											} break;
										case "mainmenuheight":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.MainMenuHeight = a;
											} break;
										case "disabledisplaylists":
											Interface.CurrentOptions.DisableDisplayLists = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "loadinadvance":
											Interface.CurrentOptions.LoadInAdvance = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "unloadtextures":
											Interface.CurrentOptions.UnloadUnusedTextures = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "notextureresize":
											Interface.CurrentOptions.NoTextureResize = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
									} break;
								case "quality":
									switch (Key)
									{
										case "interpolation":
											switch (Value.ToLowerInvariant())
											{
												case "nearestneighbor": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.NearestNeighbor; break;
												case "bilinear": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.Bilinear; break;
												case "nearestneighbormipmapped": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.NearestNeighborMipmapped; break;
												case "bilinearmipmapped": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.BilinearMipmapped; break;
												case "trilinearmipmapped": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.TrilinearMipmapped; break;
												case "anisotropicfiltering": Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.AnisotropicFiltering; break;
												default: Interface.CurrentOptions.Interpolation = Interface.InterpolationMode.BilinearMipmapped; break;
											} break;
										case "anisotropicfilteringlevel":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.AnisotropicFilteringLevel = a;
											} break;
										case "anisotropicfilteringmaximum":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.AnisotropicFilteringMaximum = a;
											} break;
										case "antialiasinglevel":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.AntiAliasingLevel = a;
											} break;
										case "transparencymode":
											switch (Value.ToLowerInvariant())
											{
												case "sharp": Interface.CurrentOptions.TransparencyMode = Renderer.TransparencyMode.Performance; break;
												case "smooth": Interface.CurrentOptions.TransparencyMode = Renderer.TransparencyMode.Quality; break;
												default:
													{
														int a;
														if (int.TryParse(Value, NumberStyles.Integer, Culture, out a))
														{
															Interface.CurrentOptions.TransparencyMode = (Renderer.TransparencyMode)a;
														}
														else
														{
															Interface.CurrentOptions.TransparencyMode = Renderer.TransparencyMode.Quality;
														}
														break;
													}
											} break;
										case "viewingdistance":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.ViewingDistance = a;
											} break;
										case "motionblur":
											switch (Value.ToLowerInvariant())
											{
												case "low": Interface.CurrentOptions.MotionBlur = MotionBlurMode.Low; break;
												case "medium": Interface.CurrentOptions.MotionBlur = MotionBlurMode.Medium; break;
												case "high": Interface.CurrentOptions.MotionBlur = MotionBlurMode.High; break;
												default: Interface.CurrentOptions.MotionBlur = MotionBlurMode.None; break;
											} break;
									} break;
								case "objectoptimization":
									switch (Key)
									{
										case "basicthreshold":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.ObjectOptimizationBasicThreshold = a;
											} break;
										case "fullthreshold":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.ObjectOptimizationFullThreshold = a;
											} break;
										case "vertexCulling":
											{
												Interface.CurrentOptions.ObjectOptimizationVertexCulling = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											} break;
									} break;
								case "simulation":
									switch (Key)
									{
										case "toppling":
											Interface.CurrentOptions.Toppling = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "collisions":
											Interface.CurrentOptions.Collisions = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "derailments":
											Interface.CurrentOptions.Derailments = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "blackbox":
											Interface.CurrentOptions.BlackBox = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "mode":
											switch (Value.ToLowerInvariant())
											{
												case "arcade": Interface.CurrentOptions.GameMode = Interface.GameMode.Arcade; break;
												case "normal": Interface.CurrentOptions.GameMode = Interface.GameMode.Normal; break;
												case "expert": Interface.CurrentOptions.GameMode = Interface.GameMode.Expert; break;
												default: Interface.CurrentOptions.GameMode = Interface.GameMode.Normal; break;
											} break;
										case "acceleratedtimefactor":
											int tf;
											int.TryParse(Value, NumberStyles.Integer, Culture, out tf);
											if (tf <= 0)
											{
												tf = 5;
											}
											Interface.CurrentOptions.TimeAccelerationFactor = tf;
											break;
									} break;
								case "controls":
									switch (Key)
									{
										case "usejoysticks":
											Interface.CurrentOptions.UseJoysticks = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "joystickaxiseb":
											Interface.CurrentOptions.AllowAxisEB = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "joystickaxisthreshold":
											{
												double a;
												double.TryParse(Value, NumberStyles.Float, Culture, out a);
												Interface.CurrentOptions.JoystickAxisThreshold = a;
											} break;
										case "keyrepeatdelay":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												if (a <= 0) a = 500;
												Interface.CurrentOptions.KeyRepeatDelay = 0.001 * (double)a;
											} break;
										case "keyrepeatinterval":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												if (a <= 0) a = 100;
												Interface.CurrentOptions.KeyRepeatInterval = 0.001 * (double)a;
											} break;
									} break;
								case "sound":
									switch (Key)
									{
										case "model":
											switch (Value.ToLowerInvariant())
											{
												case "linear": Interface.CurrentOptions.SoundModel = Sounds.SoundModels.Linear; break;
												default: Interface.CurrentOptions.SoundModel = Sounds.SoundModels.Inverse; break;
											}
											break;
										case "range":
											switch (Value.ToLowerInvariant())
											{
												case "low": Interface.CurrentOptions.SoundRange = SoundRange.Low; break;
												case "medium": Interface.CurrentOptions.SoundRange = SoundRange.Medium; break;
												case "high": Interface.CurrentOptions.SoundRange = SoundRange.High; break;
												default: Interface.CurrentOptions.SoundRange = SoundRange.Low; break;
											}
											break;
										case "number":
											{
												int a;
												int.TryParse(Value, NumberStyles.Integer, Culture, out a);
												Interface.CurrentOptions.SoundNumber = a < 16 ? 16 : a;
											} break;
									} break;
								case "verbosity":
									switch (Key)
									{
										case "showwarningmessages":
											Interface.CurrentOptions.ShowWarningMessages = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "showerrormessages":
											Interface.CurrentOptions.ShowErrorMessages = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
										case "debuglog":
											Program.GenerateDebugLogging = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
											break;
									} break;
								case "folders":
									switch (Key)
									{
										case "route":
											Interface.CurrentOptions.RouteFolder = Value;
											break;
										case "train":
											Interface.CurrentOptions.TrainFolder = Value;
											break;
									} break;
								case "proxy":
									switch (Key)
									{
										case "url":
											Interface.CurrentOptions.ProxyUrl = Value;
											break;
										case "username":
											Interface.CurrentOptions.ProxyUserName = Value;
											break;
										case "password":
											Interface.CurrentOptions.ProxyPassword = Value;
											break;
									} break;
								case "packages":
									switch (Key)
									{
										case "compression":
											switch (Value.ToLowerInvariant())
											{
												case "zip":
													Interface.CurrentOptions.packageCompressionType = CompressionType.Zip;
													break;
												case "bzip":
													Interface.CurrentOptions.packageCompressionType = CompressionType.BZ2;
													break;
												case "gzip":
													Interface.CurrentOptions.packageCompressionType = CompressionType.TarGZ;
													break;
											}
											break;
									} break;
								case "recentlyusedroutes":
									{
										int n = Interface.CurrentOptions.RecentlyUsedRoutes.Length;
										Array.Resize<string>(ref Interface.CurrentOptions.RecentlyUsedRoutes, n + 1);
										Interface.CurrentOptions.RecentlyUsedRoutes[n] = Value;
									} break;
								case "recentlyusedtrains":
									{
										int n = Interface.CurrentOptions.RecentlyUsedTrains.Length;
										Array.Resize<string>(ref Interface.CurrentOptions.RecentlyUsedTrains, n + 1);
										Interface.CurrentOptions.RecentlyUsedTrains[n] = Value;
									} break;
								case "routeencodings":
									{
										int a = System.Text.Encoding.UTF8.CodePage;
										int.TryParse(Key, NumberStyles.Integer, Culture, out a);
										int n = Interface.CurrentOptions.RouteEncodings.Length;
										Array.Resize<TextEncoding.EncodingValue>(ref Interface.CurrentOptions.RouteEncodings, n + 1);
										Interface.CurrentOptions.RouteEncodings[n].Codepage = a;
										Interface.CurrentOptions.RouteEncodings[n].Value = Value;
									} break;
								case "trainencodings":
									{
										int a = System.Text.Encoding.UTF8.CodePage;
										int.TryParse(Key, NumberStyles.Integer, Culture, out a);
										int n = Interface.CurrentOptions.TrainEncodings.Length;
										Array.Resize<TextEncoding.EncodingValue>(ref Interface.CurrentOptions.TrainEncodings, n + 1);
										Interface.CurrentOptions.TrainEncodings[n].Codepage = a;
										Interface.CurrentOptions.TrainEncodings[n].Value = Value;
									} break;
							}
						}
					}
				}
			}
			else
			{
				// file not found
				string Code = CultureInfo.CurrentUICulture.Name;
				if (string.IsNullOrEmpty(Code)) Code = "en-US";
				File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder("Languages"), Code + ".cfg");
				if (System.IO.File.Exists(File))
				{
					CurrentOptions.LanguageCode = Code;
				}
				else
				{
					try
					{
						int i = Code.IndexOf("-", StringComparison.Ordinal);
						if (i > 0)
						{
							Code = Code.Substring(0, i);
							File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder("Languages"), Code + ".cfg");
							if (System.IO.File.Exists(File))
							{
								CurrentOptions.LanguageCode = Code;
							}
						}
					}
					catch
					{
						CurrentOptions.LanguageCode = "en-US";
					}
				}
			}
		}
		internal static void SaveOptions()
		{
			CultureInfo Culture = CultureInfo.InvariantCulture;
			System.Text.StringBuilder Builder = new System.Text.StringBuilder();
			Builder.AppendLine("; Options");
			Builder.AppendLine("; =======");
			Builder.AppendLine("; This file was automatically generated. Please modify only if you know what you're doing.");
			Builder.AppendLine();
			Builder.AppendLine("[language]");
			Builder.AppendLine("code = " + CurrentOptions.LanguageCode);
			Builder.AppendLine();
			Builder.AppendLine("[interface]");
			Builder.AppendLine("folder = " + CurrentOptions.UserInterfaceFolder);
			{
				string t; switch (CurrentOptions.TimeTableStyle)
				{
					case TimeTableMode.None: t = "none"; break;
					case TimeTableMode.Default: t = "default"; break;
					case TimeTableMode.AutoGenerated: t = "autogenerated"; break;
					case TimeTableMode.PreferCustom: t = "prefercustom"; break;
					default: t = "default"; break;
				}
				Builder.AppendLine("timetablemode = " + t);
			}
			Builder.AppendLine();
			Builder.AppendLine("[display]");
			Builder.AppendLine("preferNativeBackend = " + (CurrentOptions.PreferNativeBackend ? "true" : "false"));
			Builder.AppendLine("mode = " + (CurrentOptions.FullscreenMode ? "fullscreen" : "window"));
			Builder.AppendLine("vsync = " + (CurrentOptions.VerticalSynchronization ? "true" : "false"));
			Builder.AppendLine("windowWidth = " + CurrentOptions.WindowWidth.ToString(Culture));
			Builder.AppendLine("windowHeight = " + CurrentOptions.WindowHeight.ToString(Culture));
			Builder.AppendLine("fullscreenWidth = " + CurrentOptions.FullscreenWidth.ToString(Culture));
			Builder.AppendLine("fullscreenHeight = " + CurrentOptions.FullscreenHeight.ToString(Culture));
			Builder.AppendLine("fullscreenBits = " + CurrentOptions.FullscreenBits.ToString(Culture));
			Builder.AppendLine("mainmenuWidth = " + CurrentOptions.MainMenuWidth.ToString(Culture));
			Builder.AppendLine("mainmenuHeight = " + CurrentOptions.MainMenuHeight.ToString(Culture));
			Builder.AppendLine("disableDisplayLists = " + (CurrentOptions.DisableDisplayLists ? "true" : "false"));
			Builder.AppendLine("loadInAdvance = " + (CurrentOptions.LoadInAdvance ? "true" : "false"));
			Builder.AppendLine("unloadtextures = " + (CurrentOptions.UnloadUnusedTextures ? "true" : "false"));
			Builder.AppendLine("noTextureResize = " + (CurrentOptions.NoTextureResize ? "true" : "false"));
			Builder.AppendLine();
			Builder.AppendLine("[quality]");
			{
				string t; switch (CurrentOptions.Interpolation)
				{
					case Interface.InterpolationMode.NearestNeighbor: t = "nearestNeighbor"; break;
					case Interface.InterpolationMode.Bilinear: t = "bilinear"; break;
					case Interface.InterpolationMode.NearestNeighborMipmapped: t = "nearestNeighborMipmapped"; break;
					case Interface.InterpolationMode.BilinearMipmapped: t = "bilinearMipmapped"; break;
					case Interface.InterpolationMode.TrilinearMipmapped: t = "trilinearMipmapped"; break;
					case Interface.InterpolationMode.AnisotropicFiltering: t = "anisotropicFiltering"; break;
					default: t = "bilinearMipmapped"; break;
				}
				Builder.AppendLine("interpolation = " + t);
			}
			Builder.AppendLine("anisotropicFilteringLevel = " + CurrentOptions.AnisotropicFilteringLevel.ToString(Culture));
			Builder.AppendLine("anisotropicFilteringMaximum = " + CurrentOptions.AnisotropicFilteringMaximum.ToString(Culture));
			Builder.AppendLine("antiAliasingLevel = " + CurrentOptions.AntiAliasingLevel.ToString(Culture));
			Builder.AppendLine("transparencyMode = " + ((int)CurrentOptions.TransparencyMode).ToString(Culture));
			Builder.AppendLine("viewingDistance = " + CurrentOptions.ViewingDistance.ToString(Culture));
			{
				string t; switch (CurrentOptions.MotionBlur)
				{
					case MotionBlurMode.Low: t = "low"; break;
					case MotionBlurMode.Medium: t = "medium"; break;
					case MotionBlurMode.High: t = "high"; break;
					default: t = "none"; break;
				}
				Builder.AppendLine("motionBlur = " + t);
			}
			Builder.AppendLine();
			Builder.AppendLine("[objectOptimization]");
			Builder.AppendLine("basicThreshold = " + CurrentOptions.ObjectOptimizationBasicThreshold.ToString(Culture));
			Builder.AppendLine("fullThreshold = " + CurrentOptions.ObjectOptimizationFullThreshold.ToString(Culture));
			Builder.AppendLine("vertexCulling = " + CurrentOptions.ObjectOptimizationVertexCulling.ToString(Culture));
			Builder.AppendLine();
			Builder.AppendLine("[simulation]");
			Builder.AppendLine("toppling = " + (CurrentOptions.Toppling ? "true" : "false"));
			Builder.AppendLine("collisions = " + (CurrentOptions.Collisions ? "true" : "false"));
			Builder.AppendLine("derailments = " + (CurrentOptions.Derailments ? "true" : "false"));
			Builder.AppendLine("blackbox = " + (CurrentOptions.BlackBox ? "true" : "false"));
			Builder.Append("mode = ");
			switch (CurrentOptions.GameMode)
			{
				case Interface.GameMode.Arcade: Builder.AppendLine("arcade"); break;
				case Interface.GameMode.Normal: Builder.AppendLine("normal"); break;
				case Interface.GameMode.Expert: Builder.AppendLine("expert"); break;
				default: Builder.AppendLine("normal"); break;
			}
			Builder.Append("acceleratedtimefactor = " + CurrentOptions.TimeAccelerationFactor);
			Builder.AppendLine();
			Builder.AppendLine("[verbosity]");
			Builder.AppendLine("showWarningMessages = " + (CurrentOptions.ShowWarningMessages ? "true" : "false"));
			Builder.AppendLine("showErrorMessages = " + (CurrentOptions.ShowErrorMessages ? "true" : "false"));
			Builder.AppendLine("debugLog = " + (Program.GenerateDebugLogging ? "true" : "false"));
			Builder.AppendLine();
			Builder.AppendLine("[controls]");
			Builder.AppendLine("useJoysticks = " + (CurrentOptions.UseJoysticks ? "true" : "false"));
			Builder.AppendLine("joystickAxisEB = " + (CurrentOptions.AllowAxisEB ? "true" : "false"));
			Builder.AppendLine("joystickAxisthreshold = " + CurrentOptions.JoystickAxisThreshold.ToString(Culture));
			Builder.AppendLine("keyRepeatDelay = " + (1000.0 * CurrentOptions.KeyRepeatDelay).ToString("0", Culture));
			Builder.AppendLine("keyRepeatInterval = " + (1000.0 * CurrentOptions.KeyRepeatInterval).ToString("0", Culture));
			Builder.AppendLine();
			Builder.AppendLine("[sound]");
			Builder.Append("model = ");
			switch (CurrentOptions.SoundModel)
			{
				case Sounds.SoundModels.Linear: Builder.AppendLine("linear"); break;
				default: Builder.AppendLine("inverse"); break;
			}
			Builder.Append("range = ");
			switch (CurrentOptions.SoundRange)
			{
				case SoundRange.Low: Builder.AppendLine("low"); break;
				case SoundRange.Medium: Builder.AppendLine("medium"); break;
				case SoundRange.High: Builder.AppendLine("high"); break;
				default: Builder.AppendLine("low"); break;
			}
			Builder.AppendLine("number = " + CurrentOptions.SoundNumber.ToString(Culture));
			Builder.AppendLine();
			Builder.AppendLine("[proxy]");
			Builder.AppendLine("url = " + CurrentOptions.ProxyUrl);
			Builder.AppendLine("username = " + CurrentOptions.ProxyUserName);
			Builder.AppendLine("password = " + CurrentOptions.ProxyPassword);
			Builder.AppendLine();
			Builder.AppendLine("[packages]");
			Builder.Append("compression = ");
			switch (CurrentOptions.packageCompressionType)
			{
				case CompressionType.Zip: Builder.AppendLine("zip"); break;
				case CompressionType.TarGZ: Builder.AppendLine("gzip"); break;
				case CompressionType.BZ2: Builder.AppendLine("bzip"); break;
				default: Builder.AppendLine("zip"); break;
			}
			Builder.AppendLine();
			Builder.AppendLine("[folders]");
			Builder.AppendLine("route = " + CurrentOptions.RouteFolder);
			Builder.AppendLine("train = " + CurrentOptions.TrainFolder);
			Builder.AppendLine();
			Builder.AppendLine("[recentlyUsedRoutes]");
			for (int i = 0; i < CurrentOptions.RecentlyUsedRoutes.Length; i++)
			{
				Builder.AppendLine(CurrentOptions.RecentlyUsedRoutes[i]);
			}
			Builder.AppendLine();
			Builder.AppendLine("[recentlyUsedTrains]");
			for (int i = 0; i < CurrentOptions.RecentlyUsedTrains.Length; i++)
			{
				Builder.AppendLine(CurrentOptions.RecentlyUsedTrains[i]);
			}
			Builder.AppendLine();
			Builder.AppendLine("[routeEncodings]");
			for (int i = 0; i < CurrentOptions.RouteEncodings.Length; i++)
			{
				Builder.AppendLine(CurrentOptions.RouteEncodings[i].Codepage.ToString(Culture) + " = " + CurrentOptions.RouteEncodings[i].Value);
			}
			Builder.AppendLine();
			Builder.AppendLine("[trainEncodings]");
			for (int i = 0; i < CurrentOptions.TrainEncodings.Length; i++)
			{
				Builder.AppendLine(CurrentOptions.TrainEncodings[i].Codepage.ToString(Culture) + " = " + CurrentOptions.TrainEncodings[i].Value);
			}
			string File = OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "options.cfg");
			System.IO.File.WriteAllText(File, Builder.ToString(), new System.Text.UTF8Encoding(true));
		}
	}
}
