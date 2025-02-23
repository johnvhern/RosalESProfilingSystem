using RosalESProfilingSystem.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class Literacy_Dashboard: Form
    {
        public Literacy_Dashboard()
        {
            InitializeComponent();

           
        }

        private void Literacy_Dashboard_Resize(object sender, EventArgs e)
        {
       
            int y = Screen.PrimaryScreen.Bounds.Height;
            int x = Screen.PrimaryScreen.Bounds.Width;
            this.Height = y-40;
            this.Width = x;
            this.Left = 0;
            this.Top = 0;
        
    }
    }
}
