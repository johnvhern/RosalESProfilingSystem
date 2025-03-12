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
    public partial class ContextMenuStrip_Literacy: UserControl
    {
        private Forms.Literacy_Skills literacySkills;
        private Button activeButton;
        public ContextMenuStrip_Literacy(Forms.Literacy_Skills form)
        {
            InitializeComponent();
            this.literacySkills = form;
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
            literacySkills.OpenForm(new Forms.Literacy_Dashboard());
            ColorActiveButton((Button)sender);
        }

        private void btnProfLearners_Click(object sender, EventArgs e)
        {
            literacySkills.OpenForm(new Forms.Literacy_ProfOfLearners());
            ColorActiveButton((Button)sender);
        }

        private void btnProgTrack_Click(object sender, EventArgs e)
        {
            literacySkills.OpenForm(new Forms.LiteracyProgressTracking());
            ColorActiveButton((Button)sender);  
        }
    }
}
