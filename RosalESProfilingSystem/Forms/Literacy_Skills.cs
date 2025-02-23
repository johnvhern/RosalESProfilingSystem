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
using System.Windows.Forms.DataVisualization.Charting;

namespace RosalESProfilingSystem.Forms
{
    public partial class Literacy_Skills: Form
    {
        public Literacy_Skills()
        {
            InitializeComponent();

            ContextMenuStrip_Literacy contextMenuStrip_Literacy = new ContextMenuStrip_Literacy(this);
            contextMenuStrip_Literacy.Dock = DockStyle.Top;
            this.Controls.Add(contextMenuStrip_Literacy);

            this.Load += MainForm_Load;


        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Literacy_Dashboard());
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
