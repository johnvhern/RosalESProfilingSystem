using RosalESProfilingSystem.Components;
using System;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class Home: Form
    {
        public Home()
        {
            InitializeComponent();

            ContextMenuStrip_Home contextMenuStrip_Home = new ContextMenuStrip_Home(this);
            contextMenuStrip_Home.Dock = DockStyle.Top;
            this.Controls.Add(contextMenuStrip_Home);

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Home_Dashboard());
        }

        public void OpenForm(Form form)
        {
            panel1.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panel1.Controls.Add(form);
            form.Show();
        }

    }
}
