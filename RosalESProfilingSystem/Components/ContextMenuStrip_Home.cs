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
    public partial class ContextMenuStrip_Home: UserControl
    {
        private Forms.Home home;
        private Button activeButton;
        public ContextMenuStrip_Home(Forms.Home form)
        {
            InitializeComponent();
            this.home = form;
            ColorActiveButton(btnHome);
        }

        private void ColorActiveButton(Button btnHome)
        {
            if (activeButton != null)
            {
                activeButton.BackColor = Color.WhiteSmoke;
                activeButton.ForeColor = Color.Black;
            }
            activeButton = btnHome;
            activeButton.BackColor = Color.FromArgb(8, 114, 217);
            activeButton.ForeColor = Color.White;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            home.OpenForm(new Home_Dashboard());
            ColorActiveButton((Button)sender);
        }

        private void btnProfLearners_Click(object sender, EventArgs e)
        {
            home.OpenForm(new Forms.Literacy_ProfOfLearners());
            ColorActiveButton((Button)sender);
        }
    }
}
