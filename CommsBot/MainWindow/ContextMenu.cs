using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;
using CSCore.CoreAudioAPI;
using MMDevice = CSCore.CoreAudioAPI.MMDevice;
using static CommsBot.ext;
using System.Diagnostics;

namespace CommsBot
{
    public partial class MainWindow : Form
    {
        private void InititateMenuListeners()
        {
            contextsettings.Renderer = new MyRenderer();
            SoundPackStripMenuItem.Click += new EventHandler(this.PackManagerPressed);
            contextsettings.Items[contextsettings.Items.Count - 2].Click += new System.EventHandler(this.PreferencesPressed);
            contextsettings.Items[contextsettings.Items.Count - 1].Click += new System.EventHandler(this.AboutPressed);
        }

        private void contextsettings_Opening(object sender, CancelEventArgs e)
        {

        }

        private void contextsettings_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void PreferencesPressed(object sender, EventArgs e)
        {
            EndAnyMusic();

            Settings settings = new Settings();
            MainWindow.ActiveForm.Close();
            settings.Show();
        }

        private void AboutPressed(object sender, EventArgs e)
        {

        }

        private void PackManagerPressed(object sender, EventArgs e)
        {
            EndAnyMusic();
            PacksWindow pack = new PacksWindow();
            MainWindow.ActiveForm.Close();
            pack.Show();
        }

        private void EndAnyMusic()
        {
            if ((outputDevice != null) && (StopQueued != true))
            {
                outputDevice.Stop();
                StopQueued = true;
            }
        }

        private class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }
        }

        private class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(80, 80, 80); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.Orange; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.Yellow; }
            }
        }
    }
}
