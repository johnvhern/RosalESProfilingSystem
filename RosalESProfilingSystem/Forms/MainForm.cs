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
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();

            SidePanelNavigation sidePanelNavigation = new SidePanelNavigation(this);
            sidePanelNavigation.Dock = DockStyle.Left;
            this.Controls.Add(sidePanelNavigation);

            this.Load += MainForm_Load;

            showAppVersion();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Home());
        }

        public void OpenForm(Form form)
        {
            panel1.Controls.Clear();
            
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.None;
            form.Size = panel1.ClientSize;
            form.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            panel1.Controls.Add(form);
            form.Show();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int y = Screen.PrimaryScreen.Bounds.Height;
            int x = Screen.PrimaryScreen.Bounds.Width;
            this.Height = y;
            this.Width = x;
            this.Left = 0;
            this.Top = 0;
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void btnMinimize_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void showAppVersion()
        {
            string version = Application.ProductVersion;
            label1.Text = $"Rosal Elementary School Profiling System | Version: {version}";
        }
    }
}
