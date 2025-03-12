namespace RosalESProfilingSystem.Components
{
    partial class ContextMenuStrip_Science
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnProgTrack = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnProgTrack
            // 
            this.btnProgTrack.BackColor = System.Drawing.Color.Transparent;
            this.btnProgTrack.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnProgTrack.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnProgTrack.FlatAppearance.BorderSize = 0;
            this.btnProgTrack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProgTrack.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnProgTrack.Location = new System.Drawing.Point(114, 0);
            this.btnProgTrack.Name = "btnProgTrack";
            this.btnProgTrack.Size = new System.Drawing.Size(293, 28);
            this.btnProgTrack.TabIndex = 4;
            this.btnProgTrack.Text = "Science Proficiency Progress Tracking";
            this.btnProgTrack.UseVisualStyleBackColor = false;
            this.btnProgTrack.Click += new System.EventHandler(this.btnProgTrack_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.Transparent;
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(114, 28);
            this.btnDashboard.TabIndex = 3;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // ContextMenuStrip_Science
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnProgTrack);
            this.Controls.Add(this.btnDashboard);
            this.Name = "ContextMenuStrip_Science";
            this.Size = new System.Drawing.Size(407, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnProgTrack;
        private System.Windows.Forms.Button btnDashboard;
    }
}
