using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.RepresentationModel;

namespace CommsBot
{
    public partial class Settings : Form
    {

        String AD1;
        bool? UD2;
        String AD2;
        String TP;
        bool? OB;

        public Settings()
        {
            InitializeComponent();

            SettingsFile file = new SettingsFile();
            AD1 = file.AudioDevice();
            UD2 = file.UseSecondAudioDevice();
            AD2 = file.SecondAudioDevice();
            TP = file.TreePath();
            OB = file.HasBeenOpenedBefore();

            if (UD2 == true)
            {
                SecondAudioOutState(true);
            } else
            {
                SecondAudioOutState(false);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;      // WS_EX_COMPOSITED
                return handleParam;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked != true)
            {
                SecondAudioOutState(false);
            } else
            {
                SecondAudioOutState(true);
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void SecondAudioOutState(bool yeah)
        {
            if (yeah)
            {
                // label2
                this.label2.AutoSize = true;
                this.label2.ForeColor = System.Drawing.Color.LightGray;
                this.label2.Location = new System.Drawing.Point(5, 125);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(135, 13);
                this.label2.TabIndex = 36;
                this.label2.Text = "Also Use this Audio Device";

                // customComboBox1 choose 2nd audio device
                this.customComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
                this.customComboBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
                this.customComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.customComboBox1.ForeColor = System.Drawing.Color.White;
                this.customComboBox1.FormattingEnabled = true;
                this.customComboBox1.Location = new System.Drawing.Point(8, 141);
                this.customComboBox1.Margin = new System.Windows.Forms.Padding(0);
                this.customComboBox1.MaxDropDownItems = 100;
                this.customComboBox1.Name = "customComboBox1";
                this.customComboBox1.Size = new System.Drawing.Size(348, 21);
                this.customComboBox1.TabIndex = 35;

                // Save Button Post Location
                this.button1.Location = new System.Drawing.Point(282, 245);

                // Tree label Post Location
                this.label3.Location = new System.Drawing.Point(6, 174);

                // Path Textbox Post Location
                this.textBox2.Location = new System.Drawing.Point(9, 190);

                //Browse Button Post Location
                this.button2.Location = new System.Drawing.Point(302, 190);
            }
            else
            {
                label2.Visible = false;
                customComboBox1.Visible = false;

                this.button1.Location = new System.Drawing.Point(282, 197);
                this.label3.Location = new System.Drawing.Point(6, 125);
                this.textBox2.Location = new System.Drawing.Point(9, 141);
                this.button2.Location = new System.Drawing.Point(302, 141);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
    public class CustomComboBox : ComboBox
    {
        private const int WM_PAINT = 0xF;
        private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    // Uncomment this if you don't want the "highlight border".
                    
                    using (var p = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                    }
                    using (var p = new Pen(this.BorderColor, 2))
                    {
                        g.DrawRectangle(p, 2, 2, Width - buttonWidth - 4, Height - 4);
                    }
                }
            }
        }

        public CustomComboBox()
        {
            BorderColor = Color.Black;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color BorderColor { get; set; }
    }
}
