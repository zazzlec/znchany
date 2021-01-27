using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace anyapp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            timerClose = new System.Threading.Timer(new TimerCallback(backcall), new List<string>() { "1","2" }, 2000, 0);
           
        }

        private System.Threading.Timer timerClose;
        private void backcall(object o)
        {
            List<string> arr = (List<string>)o;
            MessageBox.Show(arr[0]); MessageBox.Show(arr[1]);
            timerClose.Dispose();
        }
    }
}
