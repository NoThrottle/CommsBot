using NAudio;
using CSCore;
using NAudio.Wave;
using CSCore.CoreAudioAPI;
using MMDevice = CSCore.CoreAudioAPI.MMDevice;
using System.Diagnostics;

namespace test
{
    public partial class Form1 : Form
    {
        static List<System.Guid> audguid = new List<System.Guid>();
        static List<string> audname = new List<string>();

        private bool IsPlayingAudio = false;
        bool StopQueued = false;

        static int AD1;
        static string adguid = "";

        public Form1()
        {
            InitializeComponent();
            _AudioDevicesList();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                playSound(AD1-1, textBox1.Text);
            }
            catch
            {
                MessageBox.Show("Error playing file. Check if path is correct and file exists", "Error", MessageBoxButtons.OK);
            }
        }

        #region Audio Playback

        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        private void playSound(int deviceNumber, String path)
        {

            Console.WriteLine("Playsound");

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber };
                outputDevice.DesiredLatency = 300;
                outputDevice.NumberOfBuffers = 1;
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
            else
            {
                audioFile.Dispose();
                audioFile = null;
                audioFile = new AudioFileReader(path);
                outputDevice.Init(audioFile);
                Console.WriteLine("audfile not null");

            }

            //outputDevice.Init(audioFile);
            outputDevice.Play();
            timer1.Start();
            Console.WriteLine("Play");
            IsPlayingAudio = true;
        }

        private void OnPlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs args)
        {
            //timer1.Stop();
            Console.WriteLine("Play stopped called");

            if ((audioFile != null) && (outputDevice != null))
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
                audioFile.Dispose();
                audioFile = null;


                IsPlayingAudio = false;
                StopQueued = false;
                Console.WriteLine("Playstop conditions met");

            }

        }

        static private int ParseAD1Index()
        {
            int i = 0;
            while (i != audguid.Count)
            {
                if (audguid[i] != Guid.Parse(adguid))
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

        #endregion //----------------------

        #region Volume Meter
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (IsPlayingAudio)
            //{

            //Console.WriteLine("tick");
            //var source = new CancellationTokenSource();
            //Task mine = Task.Run(() =>
            //{
            //    getVolumes();
            //    source.Cancel();
            //}
            //);

            //if (source.IsCancellationRequested)
            //{
            //    mine.Dispose();
            //}

            volumeMeter1.Amplitude = GetDefaultAudioSessionManager2(DataFlow.Capture).GetSessionEnumerator().QueryInterface<AudioMeterInformation>().GetPeakValue();
            //Task.Run(() => getVolumes());

            //}
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(CSCore.CoreAudioAPI.DataFlow dataFlow)
        {
            using (var enumerator = new CSCore.CoreAudioAPI.MMDeviceEnumerator())
            {
                using (var device = (MMDevice)ParseMMDevice())
                {
                    Console.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }

            

        }

        private void getVolumes()
        {
            using (var sessionManager = GetDefaultAudioSessionManager2(CSCore.CoreAudioAPI.DataFlow.Render))
            using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            {
                foreach (var session in sessionEnumerator)
                {

                    using (var session2 = session.QueryInterface<AudioSessionControl2>())
                    using (var audioMeterInformation = session.QueryInterface<CSCore.CoreAudioAPI.AudioMeterInformation>())
                    {

                        if (session2.Process.ProcessName == Process.GetCurrentProcess().ProcessName)
                        {
                            volumeMeter1.Amplitude = audioMeterInformation.GetPeakValue();
                        }
                    }
                }
            }

        }

        static private void AudioDevicesList()
        {
            List<System.Guid> ids = new List<System.Guid>();
            List<string> nam = new List<string>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
                ids.Add(caps.ProductGuid);
                nam.Add(caps.ProductName);
            }

            audguid = ids;
            audname = nam;
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
                Console.WriteLine(device);
                if (device != null)
                {
                    devcut.Add(String.Concat(device.FriendlyName.ToString().Spill(32).Where(c => !Char.IsWhiteSpace(c))));
                }
            }

            foreach (var audnam in audname)
            {
                Console.WriteLine(audname[ParseAD1Index()]);
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
                    Console.WriteLine(String.Concat(device.FriendlyName.ToString().Spill(32).Where(c => !Char.IsWhiteSpace(c))));
                    Console.WriteLine(String.Concat(audname[ParseAD1Index()].Where(c => !Char.IsWhiteSpace(c))));
                    Console.WriteLine("Failed");
                    //i++;
                }

            }

            MessageBox.Show("Unable to update volume meter to " + AD1 + " . Reading first registered device instead which may be inaccurate.", "Error", MessageBoxButtons.OK);
            return devices[1];

        }
        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AD1 = comboBox1.SelectedIndex;
            adguid = audguid[comboBox1.SelectedIndex].ToString();
        }

        private void _AudioDevicesList()
        {
            List<String> devices = new List<String>();
            List<System.Guid> ids = new List<System.Guid>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
                devices.Add(caps.ProductName);
                this.comboBox1.Items.Add(caps.ProductName);
                ids.Add(caps.ProductGuid);
            }

            audguid = ids;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string filePath = "";
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "wav files (*.wav)|*.wav";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
            }

            textBox1.Text = filePath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
            }
            timer1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
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