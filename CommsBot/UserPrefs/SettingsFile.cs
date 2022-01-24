using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CommsBot
{
    public class SettingsFile
    {
        /* 0 a - Audio Device 1: Integer/ID
         * 1 b - Use 2nd Audio Device: True/False
         * 2 c - Audio Device 2: Integer/ID
         * 3 d - Tree Path: Path
         * 4 e - Has Been Opened Before?: True/False
         * 5 f - Device Search Type: "NAME"/"GUID"
         * 6 g - Index Pack: "0"
         */

        //Defaults
        String AD1 = "0";
        String UD2 = "False";
        String AD2 = "0";
        String TP = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".commsbot", "Say");
        String OB = "False";
        String ST = "NAME";
        String IP = "0";

        List<string> Settings = new List<string>();

        private void FetchSettings()
        {
            String LocalAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String prehome = Path.Combine(LocalAppdata, ".commsbot", "settings.txt");

            String line;
            List<string> lines = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(prehome);
                line = sr.ReadLine();
                lines.Add(line);
                while (line != null)
                {
                    line = sr.ReadLine();
                    lines.Add(line);
                }
                //close the file
                sr.Close();
                Settings = lines;

                AD1 = Settings[0];
                UD2 = Settings[1];
                AD2 = Settings[2];
                TP = Settings[3];
                OB = Settings[4];
                ST = Settings[5];
                IP = Settings[6];

                Console.WriteLine("settings");
                Console.WriteLine(AD1);
                Console.WriteLine(UD2);
                Console.WriteLine(AD2);
                Console.WriteLine(TP);
                Console.WriteLine(OB);
                Console.WriteLine(ST);
                Console.WriteLine(IP);
                Console.WriteLine("settings");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                WriteSettings(DefaultAudioDevice(),null,DefaultAudioDevice(),null,null,null,null);
                FetchSettings();
            }

        }

        private String DefaultAudioDevice()
        {
            try
            {
                if(ST == "NAME")
                {
                    return AudioDevicesList_ProductName()[1];
                }
                else if(ST == "GUID")
                {
                    return AudioDevicesList_GUID()[1];
                }
                else
                {
                    MessageBox.Show("Wrong Settings for Search Type", "Error", MessageBoxButtons.OK);
                    return "1";
                }
            }
            catch
            {
                MessageBox.Show("Could not Fetch your audio devices", "Error", MessageBoxButtons.OK);
                return "1";
            }
        }

        private List<string> AudioDevicesList_GUID()
        {
            List<String> devices = new List<String>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                //Console.WriteLine($"{n}: {caps.ProductName}");
                devices.Add(caps.ProductGuid.ToString());
            }

            return devices;
        }

        private List<string> AudioDevicesList_ProductName()
        {
            List<String> devices = new List<String>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                //Console.WriteLine($"{n}: {caps.ProductName}");
                devices.Add(caps.ProductName);
            }

            return devices;
        }

        public void WriteSettings(string a, string b, string c, string d, string e, string f, string g)
        {
            string LocalAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string prehome = Path.Combine(LocalAppdata, ".commsbot", "settings.txt");

            try
            {
                StreamWriter sw = new StreamWriter(prehome);

                if (a != null) { sw.WriteLine(a); } else { sw.WriteLine(AD1); }
                if (b != null) { sw.WriteLine(b); } else { sw.WriteLine(UD2); }
                if (c != null) { sw.WriteLine(c); } else { sw.WriteLine(AD2); }
                if (d != null) { sw.WriteLine(d); } else { sw.WriteLine(TP); }
                if (e != null) { sw.WriteLine(e); } else { sw.WriteLine(OB); }
                if (f != null) { sw.WriteLine(f); } else { sw.WriteLine(ST); }
                if (g != null) { sw.WriteLine(g); } else { sw.WriteLine(IP); }

                sw.Close();
            }
            catch (Exception z)
            {
                Console.WriteLine("Exception: " + z.Message);
                MessageBox.Show("Can't Write the Settings File" + "\r\n" + "Exception: " + z.Message, "Error", MessageBoxButtons.OK);
            }
        }

        public String AudioDevice()
        {
            FetchSettings();
            return Settings[0];
        }

        public bool? UseSecondAudioDevice()
        {
            FetchSettings();
            if (Settings[1] == "True")
            {
                return true;
            }
            else if (Settings[1] == "False")
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public String SecondAudioDevice()
        {
            FetchSettings();
            return Settings[2];
        }

        public String TreePath()
        {
            FetchSettings();
            return Settings[3];
        }

        public bool? HasBeenOpenedBefore()
        {
            FetchSettings();
            if (Settings[4] == "True")
            {
                return true;
            }
            else if (Settings[4] == "False")
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public String SearchType()
        {
            FetchSettings();
            if ((ST == "NAME") || (ST == "GUID"))
            {
                return ST;
            }
            else
            {
                MessageBox.Show("Incorrect Setting, using NAME Search Type instead.");
                return "NAME";
;           }
        }

        public int PackIndex()
        {
            FetchSettings();

            try
            {
                return int.Parse(IP);
            }
            catch
            {
                MessageBox.Show("Pack Index written improperly","Error", MessageBoxButtons.OK);
                return -1;
            }
        }

    }

}
