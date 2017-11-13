using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using LibRender;
using Vector3 = OpenBveApi.Math.Vector3;

namespace OpenBve
{
    class ObjectViewer : OpenTK.GameWindow
    {
        //Deliberately specify the default constructor with various overrides
        public ObjectViewer(int width, int height, GraphicsMode currentGraphicsMode, string openbve,
            GameWindowFlags @default) : base(width, height, currentGraphicsMode, openbve, @default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            try
            {
                System.Drawing.Icon ico = new System.Drawing.Icon("data\\icon.ico");
                this.Icon = ico;
            }
            catch
            {
            }
        }

        internal const string[] commandLineArgs = null;
        
        private static double RotateXSpeed = 0.0;
        private static double RotateYSpeed = 0.0;
        
        private static double MoveXSpeed = 0.0;
        private static double MoveYSpeed = 0.0;
        private static double MoveZSpeed = 0.0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Program.MouseMovement();
            double timeElapsed = CPreciseTimer.GetElapsedTime();
            DateTime time = DateTime.Now;
            Game.SecondsSinceMidnight = (double)(3600 * time.Hour + 60 * time.Minute + time.Second) + 0.001 * (double)time.Millisecond;
            ObjectManager.UpdateAnimatedWorldObjects(timeElapsed, false);
            
			var focal_point = Renderer.renderer.GetFocalPoint(Renderer.renderer.GetActiveCamera());

			World.AbsoluteCameraPosition = new Vector3(focal_point.X, focal_point.Y, focal_point.Z);

            Renderer.RenderScene();
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            Renderer.ScreenWidth = Width;
            Renderer.ScreenHeight = Height;
            Program.UpdateViewport();
        }

        protected override void OnLoad(EventArgs e)
        {
            KeyDown += Program.KeyDown;
            KeyUp += Program.KeyUp;
            MouseDown += Program.MouseEvent;
            MouseUp += Program.MouseEvent;
			MouseWheel += Program.MouseWheelEvent;
	        FileDrop += Program.DragFile;
            Program.ResetCamera();
            Renderer.Initialize();
            Renderer.InitializeLighting();
            Fonts.Initialize();
            Program.UpdateViewport();
            // command line arguments
            // if (commandLineArgs != null)
            // {
            //     for (int i = 0; i < commandLineArgs.Length; i++)
            //     {
            //         if (!Program.SkipArgs[i] && System.IO.File.Exists(commandLineArgs[i]))
            //         {
            //             try
            //             {
            //                 ObjectManager.UnifiedObject o = ObjectManager.LoadObject(commandLineArgs[i],
            //                     System.Text.Encoding.UTF8, ObjectManager.ObjectLoadMode.Normal, false, false, false,0,0,0);
            //                 ObjectManager.CreateObject(o, new Vector3(0.0, 0.0, 0.0),
            //                     new World.Transformation(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), true,
            //                     0.0, 0.0, 25.0, 0.0);
            //             }
            //             catch (Exception ex)
            //             {
            //                 Interface.AddMessage(Interface.MessageType.Critical, false, "Unhandled error (" + ex.Message + ") encountered while processing the file " + commandLineArgs[i] + ".");
            //             }
            //             Array.Resize<string>(ref Program.Files, Program.Files.Length + 1);
            //             Program.Files[Program.Files.Length - 1] = commandLineArgs[i];
            //         }
            //     }
            // }
            ObjectManager.InitializeVisibility();
            ObjectManager.FinishCreatingObjects();
            ObjectManager.UpdateVisibility(0.0, true);
            ObjectManager.UpdateAnimatedWorldObjects(0.01, true);
            Program.UpdateCaption();
        }
    }
}
