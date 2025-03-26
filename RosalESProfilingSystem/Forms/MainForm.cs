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
    public partial class MainForm: Form
    {
        private bool isDragging = false;
        public MainForm()
        {
            InitializeComponent();
            SidePanelNavigation sidePanelNavigation = new SidePanelNavigation(this);
            sidePanelNavigation.Dock = DockStyle.Left;
            this.Controls.Add(sidePanelNavigation);
            this.Load += MainForm_Load;
            this.Text = "RosalES Profiling System";
            this.WindowState = FormWindowState.Maximized;
            typeof(Panel).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
            null, panel2, new object[] { true });

        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - enables smoother UI rendering
                return cp;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenForm(new Home());
           
        }

        public void OpenForm(Form form)
        {
            panel2.Controls.Clear();
            
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.None;
            form.Size = panel2.ClientSize;

            panel2.Controls.Add(form);
            form.Show();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //int y = Screen.PrimaryScreen.Bounds.Height;
            //int x = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = y;
            //this.Width = x;
            //this.Left = 0;
            //this.Top = 0;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.ExitThread();  // Ensures all threads exit
                Application.Exit();        // Completely shuts down the app
                Environment.Exit(0);       // Hard exit (if needed)
            }
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {
           

        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int WM_NCLBUTTONDBLCLK = 0x00A3;
            const int SC_MOVE = 0xF010; // Move command
            const int WM_EXITSIZEMOVE = 0x0232; // Event when dragging stops

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                return; // Ignore the double-click event on the title bar
            }

            // Detect when dragging starts
            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_MOVE)
            {
                isDragging = true;
            }

            // Detect when dragging stops
            if (m.Msg == WM_EXITSIZEMOVE && isDragging)
            {
                isDragging = false;
                this.WindowState = FormWindowState.Maximized; // Re-maximize
            }

            base.WndProc(ref m);
        }

    }
}
