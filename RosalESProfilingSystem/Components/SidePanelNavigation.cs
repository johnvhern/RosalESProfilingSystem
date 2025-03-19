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
    public partial class SidePanelNavigation: UserControl
    {
        private Forms.MainForm mainForm;
        private Button activeButton;
        private bool isCollapsed;
        public SidePanelNavigation(Forms.MainForm form)
        {
            InitializeComponent();
            this.mainForm = form;
            ColorActiveButton(btnHome);

        }


        private void ColorActiveButton(Button button)
        {
            if (activeButton != null)
            {
                activeButton.BackColor = SystemColors.ControlLight;
                activeButton.ForeColor = Color.Black;
            }

            activeButton = button;
            activeButton.BackColor = Color.FromArgb(8, 114, 217);
            activeButton.ForeColor = Color.White;
        }

      

        private void btnHome_Click(object sender, EventArgs e)
        {
            mainForm.OpenForm(new Forms.Home());
            ColorActiveButton((Button)sender);
        }

        private void btnLiteracyPage_Click(object sender, EventArgs e)
        {
            mainForm.OpenForm(new Forms.Literacy_Skills());
            ColorActiveButton((Button)sender);
        }

        private void btnNumeracyPage_Click(object sender, EventArgs e)
        {
            timer1.Start();
            ColorActiveButton((Button)sender);
        }

        private void btnSciProf_Click(object sender, EventArgs e)
        {
            mainForm.OpenForm(new Forms.ScienceProficiency());
            ColorActiveButton((Button)sender);
        }

        private void SidePanelNavigation_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panelNumeracy.Height += 10;
                if (panelNumeracy.Size == panelNumeracy.MaximumSize)
                {
                    timer1.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                panelNumeracy.Height -= 10;
                if (panelNumeracy.Size == panelNumeracy.MinimumSize)
                {
                    timer1.Stop();
                    isCollapsed = true;
                }
            }
        }

        private void btnRMA_Click(object sender, EventArgs e)
        {
            mainForm.OpenForm(new Forms.Numeracy_Skills());
            ColorActiveButton((Button)sender);
        }

        private void btnERUNT_Click(object sender, EventArgs e)
        {
            ColorActiveButton((Button)sender);
        }
    }
}
