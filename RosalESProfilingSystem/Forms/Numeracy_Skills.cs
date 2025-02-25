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
    public partial class Numeracy_Skills: Form
    {
        public Numeracy_Skills()
        {
            InitializeComponent();

            ContextMenuStrip_Numeracy contextMenuStrip_Numeracy = new ContextMenuStrip_Numeracy(this);
            contextMenuStrip_Numeracy.Dock = DockStyle.Top;
            this.Controls.Add(contextMenuStrip_Numeracy);

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Numeracy_Dashboard());
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
