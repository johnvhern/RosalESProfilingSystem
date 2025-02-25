using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Components
{
    public partial class TimeDateBar: UserControl
    {
        public TimeDateBar()
        {
            
            InitializeComponent();

           

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            lblDateTime.Text = now.ToString("f");
        }

        private void TimeDateBar_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
