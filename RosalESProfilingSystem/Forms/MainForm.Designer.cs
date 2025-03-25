namespace RosalESProfilingSystem.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel3 = new System.Windows.Forms.Panel();
            this.timeDateBar3 = new RosalESProfilingSystem.Components.TimeDateBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.timeDateBar2 = new RosalESProfilingSystem.Components.TimeDateBar();
            this.timeDateBar1 = new RosalESProfilingSystem.Components.TimeDateBar();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.timeDateBar3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 1007);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1902, 26);
            this.panel3.TabIndex = 2;
            // 
            // timeDateBar3
            // 
            this.timeDateBar3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(114)))), ((int)(((byte)(217)))));
            this.timeDateBar3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateBar3.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeDateBar3.Location = new System.Drawing.Point(0, 0);
            this.timeDateBar3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.timeDateBar3.Name = "timeDateBar3";
            this.timeDateBar3.Size = new System.Drawing.Size(1902, 26);
            this.timeDateBar3.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1902, 1007);
            this.panel2.TabIndex = 4;
            // 
            // timeDateBar2
            // 
            this.timeDateBar2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(114)))), ((int)(((byte)(217)))));
            this.timeDateBar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateBar2.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeDateBar2.Location = new System.Drawing.Point(0, 0);
            this.timeDateBar2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.timeDateBar2.Name = "timeDateBar2";
            this.timeDateBar2.Size = new System.Drawing.Size(1902, 31);
            this.timeDateBar2.TabIndex = 0;
            // 
            // timeDateBar1
            // 
            this.timeDateBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(114)))), ((int)(((byte)(217)))));
            this.timeDateBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateBar1.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeDateBar1.Location = new System.Drawing.Point(0, 0);
            this.timeDateBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.timeDateBar1.Name = "timeDateBar1";
            this.timeDateBar1.Size = new System.Drawing.Size(1158, 25);
            this.timeDateBar1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private Components.TimeDateBar timeDateBar1;
        private Components.TimeDateBar timeDateBar2;
        private Components.TimeDateBar timeDateBar3;
    }
}