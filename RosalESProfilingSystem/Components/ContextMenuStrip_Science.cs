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
    public partial class ContextMenuStrip_Science: UserControl
    {
        private Forms.ScienceProficiency scienceProficiency;
        private Button activeButton;
        public ContextMenuStrip_Science(Forms.ScienceProficiency form)
        {
            InitializeComponent();
            this.scienceProficiency = form;
            ColorActiveButton(btnDashboard);
        }

        private void ColorActiveButton(Button btnDashboard)
        {
            if (activeButton != null)
            {
                activeButton.BackColor = SystemColors.Control;
                activeButton.ForeColor = Color.Black;
            }
            activeButton = btnDashboard;
            activeButton.BackColor = Color.FromArgb(8, 114, 217);
            activeButton.ForeColor = Color.White;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            scienceProficiency.OpenForm(new Forms.ScienceProficiency_Dashboard());
            ColorActiveButton((Button)sender);
        }
    }
}
