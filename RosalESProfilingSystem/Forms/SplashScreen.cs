using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class SplashScreen: Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(SplashScreen_Paint);
            this.DoubleBuffered = true;
        }

        private void SplashScreen_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(3, 22, 151),
                Color.FromArgb(8, 114, 217),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
