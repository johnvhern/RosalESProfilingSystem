using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Components
{
    public partial class LoadingForm: Form
    {
        public LoadingForm()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(300, 70);
            this.ControlBox = false;
            this.Text = "Loading Competencies...";
            this.ShowInTaskbar = false;
            progressBar1 = new ProgressBar();  // Manually create a new ProgressBar if missing
            this.Controls.Add(progressBar1);   // Add it to the form
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Dock = DockStyle.Fill;
        }
    }
}
