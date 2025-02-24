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
    }
}
