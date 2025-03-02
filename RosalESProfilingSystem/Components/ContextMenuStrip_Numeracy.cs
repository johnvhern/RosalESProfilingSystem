using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RosalESProfilingSystem.Forms;

namespace RosalESProfilingSystem.Components
{
    public partial class ContextMenuStrip_Numeracy: UserControl
    {
        private Forms.Numeracy_Skills numeracySkills;
        private Button activeButton;
        public ContextMenuStrip_Numeracy(Forms.Numeracy_Skills form)
        {
            InitializeComponent();
            this.numeracySkills = form;
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

        private void btnProfLearners_Click(object sender, EventArgs e)
        {
            numeracySkills.OpenForm(new Forms.Literacy_ProfOfLearners());
            ColorActiveButton((Button)sender);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            numeracySkills.OpenForm(new Forms.Numeracy_Dashboard());
            ColorActiveButton((Button)sender);
        }

        private void btnProgTrack_Click(object sender, EventArgs e)
        {
            numeracySkills.OpenForm(new Forms.NumeracyProgressTracking_Form());
            ColorActiveButton((Button)sender);
        }
    }
}
