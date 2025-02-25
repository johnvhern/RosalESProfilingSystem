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
            DateTime now = DateTime.Now;
            InitializeComponent();

            lblDateTime.Text = now.ToString("f");

        }

        private void lblDateTime_Click(object sender, EventArgs e)
        {

        }
    }
}
