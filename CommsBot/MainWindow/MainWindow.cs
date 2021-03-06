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

        #region Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static string globalpath;
        public static bool typingcode; //add this later
        public static bool enableKeyHandler = false;

        static String AD1;
        bool? UD2;
        static String AD2;
        String TP;
        bool? OB;
        static String ST;

        static List<System.Guid> audguid = new List<System.Guid>();
        static List<string> audname = new List<string>();

        private bool IsPlayingAudio = false;
        private bool IsPlayingAudio2 = false;
        bool StopQueued = false;

        //Handles MessageBoxes so they don't spam
        private static bool MB01 = false; //User knows Volume Meter is Incorrect
        private static bool MB02 = false; //User knows Audio is played to Deafult Device for dev1
        private static bool MB03 = false; //User knows Audio is played to Deafult Device for dev2

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;      // WS_EX_COMPOSITED
                return handleParam;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InititateMenuListeners();
            timer1.Stop();//Incase it starts up rougely

            enableKeyHandler = false;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            UpdateButton(-1, true);//Initialize Button Names
            HandleHandlers();

            SettingsFile file = new SettingsFile();
            AD1 = file.AudioDevice();
            UD2 = file.UseSecondAudioDevice();
            AD2 = file.SecondAudioDevice();
            TP = file.TreePath();
            OB = file.HasBeenOpenedBefore();
            ST = file.SearchType();

            AudioDevicesList();
        }

        #region TopBar
        private void Close_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void Minimize_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ToggleActive_MouseClick(object sender, MouseEventArgs e)
        {
            HandleHandlers();

        }

        private void Settings_Click(object sender, EventArgs e)
        {

            contextsettings.Show(Cursor.Position);

        }

        #endregion

        private void ProcessDirectories()
        {

        }

        private void UpdateButton(int id, Boolean check)
        {

            String LocalAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String prehome = Path.Combine(LocalAppdata, ".commsbot");
            String home = Path.Combine(LocalAppdata, ".commsbot", "Say");
            String nextpath = globalpath;


            //Console.WriteLine(home);// debug
            Directory.CreateDirectory(home); // create home if it doesnt exist

            if (string.IsNullOrEmpty(nextpath)) { nextpath = home; }
            if (string.IsNullOrEmpty(globalpath)) { globalpath = home; }

            string[] subdirs = Directory.GetDirectories(nextpath);


            DirectoryInfo[] isempty = new DirectoryInfo(nextpath).GetDirectories();

            if (isempty.Length == 0)
            {
                //Console.WriteLine(isempty);
                //Console.WriteLine("upupup");
                PlayList(globalpath);
                //hold any updates to the button and lock them all
                globalpath = home;
                UpdateButton(-2, false);
                return;
            }

            UpdateLabels(globalpath, prehome, "hi");

            switch (id)
            {
                case -2:
                    globalpath = home;
                    UpdateButton(-1, false);
                    break;
                case -1:
                    //Initialization/Updating Only
                    UpdateButtonNaming(nextpath);
                    break;
                case 0:
                    break;
                case 1:
                    if (subdirs.Length >= 1) { globalpath = Path.Combine(nextpath, subdirs[0]); UpdateButton(-1, true); }
                    break;
                case 2:
                    if (subdirs.Length >= 2) { globalpath = Path.Combine(nextpath, subdirs[1]); UpdateButton(-1, true); }
                    break;
                case 3:
                    if (subdirs.Length >= 3) { globalpath = Path.Combine(nextpath, subdirs[2]); UpdateButton(-1, true); }
                    break;
                case 4:
                    if (subdirs.Length >= 4) { globalpath = Path.Combine(nextpath, subdirs[3]); UpdateButton(-1, true); }
                    break;
                case 5:
                    if (subdirs.Length >= 5) { globalpath = Path.Combine(nextpath, subdirs[4]); UpdateButton(-1, true); }
                    break;
                case 6:
                    if (subdirs.Length >= 6) { globalpath = Path.Combine(nextpath, subdirs[5]); UpdateButton(-1, true); }
                    break;
                case 7:
                    if (subdirs.Length >= 7) { globalpath = Path.Combine(nextpath, subdirs[6]); UpdateButton(-1, true); }
                    break;
                case 8:
                    if (subdirs.Length >= 8) { globalpath = Path.Combine(nextpath, subdirs[7]); UpdateButton(-1, true); }
                    break;
                case 9:
                    if (subdirs.Length >= 9) { globalpath = Path.Combine(nextpath, subdirs[8]); UpdateButton(-1, true); }
                    break;
                case 10:
                    btnplus.Text = "hi";
                    break;
                case 11:
                    btnminus.Text = "hi";
                    break;
                case 12:
                    //Console.WriteLine("pressed reset");
                    if (outputDevice != null)
                    {
                        outputDevice.Stop();                        
                        StopQueued = true;
                    }
                    if (outputDevice2 != null)
                    {
                        outputDevice2.Stop();
                        StopQueued = true;
                    }
                    globalpath = home;
                    UpdateButton(-2, false);
                    break;
                case 13:
                    btndivide.Text = "hi";
                    break;
                default:
                    break;
            }

        }

        #region Audio Playback

        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        private WaveOutEvent outputDevice2;
        private AudioFileReader audioFile2;

        private void PlayList(String path)
        {
            string[] audiofiles = Directory.GetFiles(path);

            if (audiofiles.Length == 0) {

                //MessageBox.Show("There are no Audio files in this directory", "Note", MessageBoxButtons.OK);
                label2.Text = "No Audio Files in this Directory";
                label1.Text = "No Audio";
                Thread.Sleep(1500);
            }
            else
            {
                if (IsPlayingAudio == false)
                {
                    Console.WriteLine("isplaying false, playing audio");

                    Random rnd = new Random();
                    playSound(ParseAD1Index() - 1, audiofiles[rnd.Next(0, audiofiles.Length)]);

                    if ((UD2 == true) && (IsPlayingAudio2 == false))
                    {
                        Console.WriteLine("Playing on 2nd Device");
                        playSound2(ParseAD2Index() - 1, audiofiles[rnd.Next(0, audiofiles.Length)]);

                    }
                }
                
            }
        }

        private void playSound(int deviceNumber, String path)
        {

            Console.WriteLine("Playsound");

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber };
                outputDevice.PlaybackStopped += OnPlaybackStopped;
                Console.WriteLine("outdev null");

            }
            else
            {
                outputDevice.PlaybackStopped += OnPlaybackStopped;
                Console.WriteLine("outdev not null");

            }

            if (audioFile == null)
            {
                audioFile = new AudioFileReader(path);
                outputDevice.Init(audioFile);
                Console.WriteLine("audfile null");

            }

            outputDevice.Play();
            csdevice = ParseMMDevice();
            //timer1.Start();
            Console.WriteLine("Play");
            IsPlayingAudio = true;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            //timer1.Stop();
            volumeMeter1.Amplitude = 0;
            Console.WriteLine("Play stopped called");
            ResetMessageBoxLimits();

            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
            }

            if (outputDevice != null)
            {
                outputDevice.Dispose();
                outputDevice = null;
            }

            PostPlaybackStop();
            IsPlayingAudio = false;
            Console.WriteLine("Playstop conditions met");

        }

        private void playSound2(int deviceNumber, String path)
        {

            Console.WriteLine("Playsound2");

            if (outputDevice2 == null)
            {
                outputDevice2 = new WaveOutEvent() { DeviceNumber = deviceNumber };
                outputDevice2.PlaybackStopped += OnPlaybackStopped2;
                Console.WriteLine("outde2v null");

            }
            else
            {
                outputDevice2.PlaybackStopped += OnPlaybackStopped2;
                Console.WriteLine("outdev2 not null");

            }

            if (audioFile2 == null)
            {
                audioFile2 = new AudioFileReader(path);
                outputDevice2.Init(audioFile2);
                Console.WriteLine("audfile2 null");

            }

            outputDevice2.Play();
            Console.WriteLine("Play2");
            IsPlayingAudio2 = true;
        }

        private void OnPlaybackStopped2(object sender, StoppedEventArgs args)

        {
            Console.WriteLine("Play2 stopped called");

            if (audioFile2 != null)
            {
                audioFile2.Dispose();
                audioFile2 = null;
            }

            if (outputDevice2 != null)
            {
                outputDevice2.Dispose();
                outputDevice2 = null;
            }

            PostPlaybackStop();
            IsPlayingAudio2 = false;
            Console.WriteLine("Playstop2 conditions met");

        }

        private void PostPlaybackStop()
        {
            //timer1.Stop();
            StopQueued = false;
        }

        static private void AudioDevicesList()
        {
            List<System.Guid> ids = new List<System.Guid>();
            List<string> nam = new List<string>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                //Console.WriteLine($"{n}: {caps.ProductName}");
                ids.Add(caps.ProductGuid);
                nam.Add(caps.ProductName);
            }

            audguid = ids;
            audname = nam;
        }

        static private int ParseAD1Index()
        {
            if (ST == "NAME")
            {
                int i = 0;
                foreach (var nam in audname)
                {
                    if (nam == AD1)
                    {
                        return i;
                    }
                    i++;
                }

                if (MB02 == false)
                {
                    MessageBox.Show("Previously Set Audio Device 1 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                    MB02 = true;
                }
                return 0;
            }
            else
            {
                int i = 0;

                foreach (System.Guid id in audguid)
                {
                    if (id == Guid.Parse(AD1))
                    {
                        return i;

                    }
                    i++;
                }

                if (MB02 == false)
                {
                    MessageBox.Show("Previously Set Audio Device 1 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                    MB02 = true;
                }
                return 0;
            }

        }

        static private int ParseAD2Index()
        {
            if (ST == "NAME")
            {
                int i = 0;
                foreach (var nam in audname)
                {
                    if (nam == AD2)
                    {
                        return i;
                    }
                    i++;
                }

                if (MB03 == false)
                {
                    MessageBox.Show("Previously Set Audio Device 2 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                    MB03 = true;
                }
                return 0;
            }
            else
            {
                int i = 0;

                foreach (System.Guid id in audguid)
                {
                    if (id == Guid.Parse(AD2))
                    {
                        return i;

                    }
                    i++;
                }

                if (MB03 == false)
                {
                    MessageBox.Show("Previously Set Audio Device 2 cannot be found. Resetting to Default", "Error", MessageBoxButtons.OK);
                    MB03 = true;
                }
                return 0;
            }
        }

        private void ResetMessageBoxLimits()
        {
            MB01 = false;
            MB02 = false;
            MB03 = false;
        }

        #endregion //----------------------

        private void UpdateLabels(String endpath, String prehome, String large)
        {

            label2.Text = String.Join(" > ", endpath.Substring(prehome.Length + 1).Split('\\', '/'));
            label1.Text = endpath.Split('\\', '/')[endpath.Split('\\', '/').Count() - 1];

        }

        private void HandleHandlers()
        {

            if (enableKeyHandler)
            {
                Keyboard.Image = Properties.Resources.keyboard;
                enableKeyHandler = false;

                #region Create KeyHandlers
                bool a = new KeyHandler(Keys.NumPad0, this).Register(0);
                bool b = new KeyHandler(Keys.NumPad1, this).Register(1);
                bool c = new KeyHandler(Keys.NumPad2, this).Register(2);
                bool d = new KeyHandler(Keys.NumPad3, this).Register(3);
                bool e = new KeyHandler(Keys.NumPad4, this).Register(4);
                bool f = new KeyHandler(Keys.NumPad5, this).Register(5);
                bool g = new KeyHandler(Keys.NumPad6, this).Register(6);
                bool h = new KeyHandler(Keys.NumPad7, this).Register(7);
                bool i = new KeyHandler(Keys.NumPad8, this).Register(8);
                bool j = new KeyHandler(Keys.NumPad9, this).Register(9);
                bool k = new KeyHandler(Keys.Add, this).Register(10);
                bool l = new KeyHandler(Keys.Subtract, this).Register(11);
                bool m = new KeyHandler(Keys.Multiply, this).Register(12);
                bool n = new KeyHandler(Keys.Divide, this).Register(13);
                #endregion
            }
            else
            {
                Keyboard.Image = Properties.Resources.keyboard_off;
                enableKeyHandler = true;

                int i = 0;
                while (i != 13)
                {
                    UnregisterHotKey(this.Handle, i);
                    i++;
                }
            }
        }

        private void UpdateButtonNaming(String nextpath)
        {
            //Handles Button Naming
            DirectoryInfo[] diArr = new DirectoryInfo(nextpath).GetDirectories();
            List<String> subdirsname = new List<String>();

            foreach (DirectoryInfo dri in diArr)
            {
                Console.WriteLine(dri.Name);// debug
                subdirsname.Add(dri.Name);
            }

            //----------------------

            List<String> names = subdirsname;
            int i = 0;

            if (subdirsname.Count >= 1) { btn1.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn1.Enabled = true; }
            else { btn1.Text = "1"; btn1.Enabled = false; }
            if (subdirsname.Count >= 2) { btn2.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn2.Enabled = true; }
            else { btn2.Text = "2"; btn2.Enabled = false; }
            if (subdirsname.Count >= 3) { btn3.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn3.Enabled = true; }
            else { btn3.Text = "3"; btn3.Enabled = false; }
            if (subdirsname.Count >= 4) { btn4.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn4.Enabled = true; }
            else { btn4.Text = "4"; btn4.Enabled = false; }
            if (subdirsname.Count >= 5) { btn5.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn5.Enabled = true; }
            else { btn5.Text = "5"; btn5.Enabled = false; }
            if (subdirsname.Count >= 6) { btn6.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn6.Enabled = true; }
            else { btn6.Text = "6"; btn6.Enabled = false; }
            if (subdirsname.Count >= 7) { btn7.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn7.Enabled = true; }
            else { btn7.Text = "7"; btn7.Enabled = false; }
            if (subdirsname.Count >= 8) { btn8.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn8.Enabled = true; }
            else { btn8.Text = "8"; btn8.Enabled = false; }
            if (subdirsname.Count >= 9) { btn9.Text = subdirsname[i]; i++; Console.WriteLine(i + ": " + subdirsname[i - 1]); btn9.Enabled = true; }
            else { btn9.Text = "9"; btn9.Enabled = false; }

        }

        //HandleRequests
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                int id = m.WParam.ToInt32();
                UpdateButton(id, false);
            }
            base.WndProc(ref m);
        }

        #region Draggable
        private void draggable()
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            draggable();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            draggable();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            draggable();
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            draggable();
        }

        #endregion

        #region Useless

        private void volumeMeter1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Volume Meter

        static MMDevice csdevice = null;

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (IsPlayingAudio)
            //{

            //Console.WriteLine("tick");
            //var source = new CancellationTokenSource();
            //Task mine = Task.Run(() => {
            //    getVolumes();
            //    source.Cancel();
            //    }
            //);

            //if (source.IsCancellationRequested)
            //{
            //    mine.Dispose();
            //}
            //Task.Run(() => getVolumes());
            //}
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(CSCore.CoreAudioAPI.DataFlow dataFlow)
        {
            var sessionManager = AudioSessionManager2.FromMMDevice(csdevice);
            return sessionManager;
        }

        private void getVolumes()
        {
            using (var sessionManager = GetDefaultAudioSessionManager2(CSCore.CoreAudioAPI.DataFlow.Render))
            using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            {
                foreach (var session in sessionEnumerator)
                {

                    //Console.WriteLine(session.DisplayName);

                    using (var session2 = session.QueryInterface<AudioSessionControl2>())
                    {

                        if (session2.Process.ProcessName == Process.GetCurrentProcess().ProcessName)
                        {
                            using (var audioMeterInformation = session.QueryInterface<CSCore.CoreAudioAPI.AudioMeterInformation>())
                            {
                                volumeMeter1.Amplitude = audioMeterInformation.GetPeakValue();

                            }
                            
                        }
                    }
                }
            }

        }

        static public CSCore.CoreAudioAPI.MMDevice ParseMMDevice()
        {
            AudioDevicesList();
            CSCore.CoreAudioAPI.MMDeviceEnumerator enumerator = new CSCore.CoreAudioAPI.MMDeviceEnumerator();
            var devices = enumerator.EnumAudioEndpoints(CSCore.CoreAudioAPI.DataFlow.All, CSCore.CoreAudioAPI.DeviceState.Active);

            //int i = 0;

            List<String> devcut = new List<string>();
            List<String> audcut = new List<string>();

            foreach (var device in devices)
            {
                //Console.WriteLine(device);
                if (device != null)
                {
                    devcut.Add(String.Concat(device.FriendlyName.ToString().Spill(32).Where(c => !Char.IsWhiteSpace(c))));
                }
            }

            foreach (var audnam in audname)
            {
                //Console.WriteLine(audname[ParseAD1Index()]);
                audcut.Add(String.Concat(audname[ParseAD1Index()].Where(c => !Char.IsWhiteSpace(c))));
            }

            foreach (var device in devices)
            {

                if (String.Concat(device.FriendlyName.ToString().Spill(31).Where(c => !Char.IsWhiteSpace(c))) ==
                            String.Concat(audname[ParseAD1Index()].Where(c => !Char.IsWhiteSpace(c)))
                       )
                {
                    return device;
                }
                else
                {
                    //Console.WriteLine(String.Concat(device.FriendlyName.ToString().Spill(32).Where(c => !Char.IsWhiteSpace(c))));
                    //Console.WriteLine(String.Concat(audname[ParseAD1Index()].Where(c => !Char.IsWhiteSpace(c))));
                    //Console.WriteLine("Failed");
                }

            }

            if (MB01 == false)
            {
                MB01 = true;
                MessageBox.Show("Unable to update volume meter to " + AD1 + 
                    " . Reading first registered device instead which may be inaccurate.", "Error", MessageBoxButtons.OK);
            }
            return devices[0];

        }
        #endregion

        #region Handle Keypress
        //HandlesKeyPress
        public static class Constants
        {
            //windows message id for hotkey
            public const int WM_HOTKEY_MSG_ID = 0x0312;
        }

        public class KeyHandler
        {
            [DllImport("user32.dll")]
            private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

            [DllImport("user32.dll")]
            private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            private int key;
            private IntPtr hWnd;
            private int id;

            public KeyHandler(Keys key, Form form)
            {
                this.key = (int)key;
                this.hWnd = form.Handle;
                this.id = this.GetHashCode();
            }

            public override int GetHashCode()
            {
                return key ^ hWnd.ToInt32();
            }

            public bool Register(int id)
            {
                return RegisterHotKey(hWnd, id, 0, key);
            }

            public bool Unregister(int id)
            {
                return UnregisterHotKey(hWnd, id);
            }
        }

        #endregion

    }

    //Extensions
    public static class ext
    {
        public static string Spill(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }
    }
}
