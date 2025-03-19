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
    public partial class ERUNT_Form: Form
    {
        public ERUNT_Form()
        {
            InitializeComponent();

            ContextMenyStrip_ERUNT contextMenyStrip_ERUNT = new ContextMenyStrip_ERUNT(this);
            contextMenyStrip_ERUNT.Dock = DockStyle.Top;
            this.Controls.Add(contextMenyStrip_ERUNT);

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Forms.ERUNT_Dashboard());
        }

        public void OpenForm(ERUNT_Dashboard eRUNT_Dashboard)
        {
            panel1.Controls.Clear();

            eRUNT_Dashboard.TopLevel = false;
            eRUNT_Dashboard.FormBorderStyle = FormBorderStyle.None;
            eRUNT_Dashboard.Dock = DockStyle.Fill;

            panel1.Controls.Add(eRUNT_Dashboard);
            eRUNT_Dashboard.Show();
        }
    }
}
