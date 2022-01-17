//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Threading.Tasks;
//using System.ComponentModel;

//namespace CommsBot
//{


    //deprecated


    //public partial class WindowHandler : Form
    //{
    //    Form1 form1 = null;
    //    Settings settings = null;

    //    public WindowHandler()
    //    {

    //        form1 = new Form1();

            //this.Visible = false;
            //this.Hide();

            //settings = new CommsBot.Settings();
            //form1 = new Form1();

            //form1.Show();

            //settings.FormClosed += new FormClosedEventHandler(Settings_FormClosed);
            //form1.VisibleChanged += new EventHandler(Form1_Hidden);
            ////settings.VisibleChanged += new EventHandler(Form1_Hidden);
            //form1.FormClosed += new FormClosedEventHandler(Form1_FormClosed);

        }

        //public void SettingsOpen()
        //{
        //    settings = new CommsBot.Settings();
        //    settings.Show();
        //}

        //public void MainWindowOpen()
        //{
        //    Console.WriteLine("hi");
        //    form1.Show();
        //}

        //void Settings_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    Console.WriteLine("hi");
        //    form1.Show();
        //    form1.Visible = true;
        //    //form1.Close();
        //    //form1 = new Form1();
        //}

        //void Form1_Hidden(object sender, EventArgs e)
        //{
        //    if (form1.Visible != true)
        //    {
        //        form1.Visible = false;
        //        SettingsOpen();
        //    } 
        //}
        //void Form1_FormClosed(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}
//    }
//}
