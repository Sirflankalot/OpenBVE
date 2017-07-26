using System;
using System.Windows.Forms;
using OpenBveApi.Math;
using OpenTK.Graphics;
using LibRender;

namespace OpenBve
{
    public partial class formOptions : Form
    {
        public formOptions()
        {
			Settings set = Renderer.renderer.GetSettings();
            InitializeComponent();
            InterpolationMode.SelectedIndex = (int) set.texture_filtering;
            //AnsiotropicLevel.Value = Interface.CurrentOptions.AnisotropicFilteringLevel;
            AntialiasingLevel.SelectedIndex = ForwardAAMenuConvert(set.forward_aa);
            //TransparencyQuality.SelectedIndex = Interface.CurrentOptions.TransparencyMode == Renderer.TransparencyMode.Sharp ? 0 : 2;
            width.Value = Renderer.ScreenWidth;
            height.Value = Renderer.ScreenHeight;
        }

        internal static DialogResult ShowOptions()
        {
            formOptions Dialog = new formOptions();
            DialogResult Result = Dialog.ShowDialog();
            return Result;
        }

		private int ForwardAAMenuConvert(Settings.ForwardAntialiasing tex) {
			switch (tex) {
				case Settings.ForwardAntialiasing.None:
					return 0;
				case Settings.ForwardAntialiasing.MSAA2:
					return 1;
				case Settings.ForwardAntialiasing.MSAA4:
					return 2;
				case Settings.ForwardAntialiasing.MSAA8:
					return 3;
				case Settings.ForwardAntialiasing.SSAA2:
					return 4;
				case Settings.ForwardAntialiasing.SSAA4:
					return 5;
				default:
					return 0;
			}
		}

		private Settings.ForwardAntialiasing ForwardAAMenuConvert(int tex) {
			switch (tex) {
				case 0:
					return Settings.ForwardAntialiasing.None;
				case 1:
					return Settings.ForwardAntialiasing.MSAA2;
				case 2:
					return Settings.ForwardAntialiasing.MSAA4;
				case 3:
					return Settings.ForwardAntialiasing.MSAA8;
				case 4:
					return Settings.ForwardAntialiasing.SSAA2;
				case 5:
					return Settings.ForwardAntialiasing.SSAA4;
				default:
					return Settings.ForwardAntialiasing.None;
			}
		}

		private void button1_Click(object sender, EventArgs e)
        {
			//Interpolation mode
			Renderer.renderer.SetSetting((Settings.TextureFiltering)InterpolationMode.SelectedIndex);
			//Antialiasing level
			Renderer.renderer.SetSetting(ForwardAAMenuConvert(AntialiasingLevel.SelectedIndex));
            //Set width and height
            if (Renderer.ScreenWidth != width.Value || Renderer.ScreenHeight != height.Value)
            {
                Renderer.ScreenWidth = (int) width.Value;
                Renderer.ScreenHeight = (int) height.Value;
                Program.currentGameWindow.Width = (int) width.Value;
                Program.currentGameWindow.Height = (int) height.Value;
                Program.UpdateViewport();
            }
            Options.SaveOptions();
            this.Close();
        }
    }
}
