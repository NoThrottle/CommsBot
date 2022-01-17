using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.RepresentationModel;

namespace CommsBot
{
    public partial class Settings : Form
    {

        #region Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        String AD1;
        bool? UD2;
        String AD2;
        String TP;
        bool? OB;

        bool ThereIsChange = false;

        List<System.Guid> audguid = new List<System.Guid>();

        public Settings()
        {
            InitializeComponent();

            SettingsFile file = new SettingsFile();
            AD1 = file.AudioDevice();
            UD2 = file.UseSecondAudioDevice();
            AD2 = file.SecondAudioDevice();
            TP = file.TreePath();
            OB = file.HasBeenOpenedBefore();

            AudioDevicesList();

            //Instantiate Changes to Buttons/TextBoxes based on Settings Files

            textBox2.Text = TP; //TextBox for Path
            if (UD2 == true)
            {
                SecondAudioOutState(true);
                checkBox1.Checked = true;
            } else
            {
                SecondAudioOutState(false);
                checkBox1.Checked = false;

            } //Audio Device Selections and checkbox

            comboBox1.SelectedIndex = ParseAD1Index();
            customComboBox1.SelectedIndex = ParseAD2Index();

            button1.Enabled = false;
        }

        private int ParseAD1Index()
        {
            try
            {
                int i = 0;
                while (i <= audguid.Count - 1)
                {
                    Console.WriteLine(audguid[i]);
                    if (audguid[i] != Guid.Parse(AD1))
                    {
                        i++;
                    }
                    else
                    {
                        return i;
                    }
                }

                MessageBox.Show("Previously Set Audio Device 1 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                return 0;
            }
            catch
            {
                MessageBox.Show("Audio Device 1 was set incorrectly. Resetting to Default", "Error", MessageBoxButtons.OK);
                SettingsFile file = new SettingsFile();
                file.WriteSettings(null, null, null, null, null);
                return 0;
            }

        }

        private int ParseAD2Index()
        {
            try
            {
                int i = 0;
                while (i != audguid.Count)
                {
                    if (audguid[i] != Guid.Parse(AD2))
                    {
                        i++;
                    }
                    else
                    {
                        return i;
                    }
                }

                MessageBox.Show("Previously Set Audio Device 2 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                return 0;
            }
            catch
            {
                MessageBox.Show("Audio Device 2 was set incorrectly. Resetting to Default", "Error", MessageBoxButtons.OK);
                SettingsFile file = new SettingsFile();
                file.WriteSettings(null, null, null, null, null);
                return 0;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            ThereIsChange = true;
            EnableSaveButton();

            if (checkBox1.Checked != true)
            {
                SecondAudioOutState(false);
                UD2 = false;
            } else
            {
                SecondAudioOutState(true);
                UD2 = true;
            }
        }//done

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
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

                label2.Visible = true;
                customComboBox1.Visible = true;

                // Save Button Post Location
                this.button1.Location = new System.Drawing.Point(282, 245);

                // Tree label Post Location
                this.label3.Location = new System.Drawing.Point(6, 174);

                // Path Textbox Post Location
                this.textBox2.Location = new System.Drawing.Point(9, 190);

                //Browse Button Post Location
                this.button2.Location = new System.Drawing.Point(302, 190);

                this.Size = new Size(366, 282);

                ReallyCenterToScreen();



            }
            else
            {
                label2.Visible = false;
                customComboBox1.Visible = false;

                this.button1.Location = new System.Drawing.Point(282, 197);
                this.label3.Location = new System.Drawing.Point(6, 125);
                this.textBox2.Location = new System.Drawing.Point(9, 141);
                this.button2.Location = new System.Drawing.Point(302, 141);

                this.Size = new Size(366, 232);

                ReallyCenterToScreen();

            }
        } //done

        private void button2_Click(object sender, EventArgs e)
        {
            ThereIsChange = true;
            EnableSaveButton();

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = TP;
            dialog.ShowDialog();

            textBox2.Text = dialog.SelectedPath;

        } //done

        private void EnableSaveButton()
        {
            button1.Enabled = true;
        } //done

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AD1 = audguid[comboBox1.SelectedIndex].ToString();
            ThereIsChange = true;
            EnableSaveButton();
        }//done

        private void customComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AD2 = audguid[customComboBox1.SelectedIndex].ToString();
            ThereIsChange = true;
            EnableSaveButton();
        }//Done

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ThereIsChange = true;
            EnableSaveButton();
            TP = textBox2.Text;
        }//done

        //Save
        private void button1_Click(object sender, EventArgs e)
        {
            
            SettingsFile file = new SettingsFile();
            file.WriteSettings(AD1, UD2.ToString(), AD2, TP, OB.ToString());

            //Disable After Saving Possible Changes
            ThereIsChange = false;
            button1.Enabled = false;
        }

        #region Private Returns/Functions
        protected void ReallyCenterToScreen()
        {
            Screen screen = Screen.FromControl(this);

            Rectangle workingArea = screen.WorkingArea;
            this.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
            };
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

        private void AudioDevicesList()
        {
            List<String> devices = new List<String>();
            List<System.Guid> ids = new List<System.Guid>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
                devices.Add(caps.ProductName);
                this.comboBox1.Items.Add(caps.ProductName);
                this.customComboBox1.Items.Add(caps.ProductName);
                ids.Add(caps.ProductGuid);
            }

            audguid = ids;
        }
        #endregion

        #region TopBar
        private void CloseSettings()
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (ThereIsChange)
            {
                DialogResult result = MessageBox.Show("There are unsaved changes. Discard Changes?", "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    CloseSettings();
                }
            }
            else
            {
                CloseSettings();
            }
        }

        #endregion

        #region Draggable
        private void Settings_Click(object sender, EventArgs e)
        {
            draggable();
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
            draggable();
        }

        private void draggable()
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void Settings_MouseDown(object sender, MouseEventArgs e)
        {
            draggable();
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
