using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //timer1.Interval = 500;
            //timer1.Start();
        }
        public static void AddLgoToTXT(string logstring)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "logs/log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if (!System.IO.File.Exists(path))
            {
                FileStream stream = System.IO.File.Create(path);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(logstring);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
        }
        private void backcall_cg(object o)
        {
            MessageBox.Show("Test");
            //AddLgoToTXT(DateTime.Now.ToString() + "：10分钟后续操作");
        }
    }
}
