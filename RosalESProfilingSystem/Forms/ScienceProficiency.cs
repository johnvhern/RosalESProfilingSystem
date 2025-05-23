﻿using RosalESProfilingSystem.Components;
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
    public partial class ScienceProficiency: Form
    {
        public ScienceProficiency()
        {
           
            InitializeComponent();
            ContextMenuStrip_Science contextMenuStrip_Science = new ContextMenuStrip_Science(this);
            contextMenuStrip_Science.Dock = DockStyle.Top;
            this.Controls.Add(contextMenuStrip_Science);
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new ScienceProficiency_Dashboard());
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
