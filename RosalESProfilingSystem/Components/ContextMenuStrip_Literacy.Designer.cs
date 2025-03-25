namespace RosalESProfilingSystem.Components
{
    partial class ContextMenuStrip_Literacy
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
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnProgTrack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.Transparent;
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(4);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(152, 34);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnProgTrack
            // 
            this.btnProgTrack.BackColor = System.Drawing.Color.Transparent;
            this.btnProgTrack.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnProgTrack.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnProgTrack.FlatAppearance.BorderSize = 0;
            this.btnProgTrack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProgTrack.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProgTrack.Location = new System.Drawing.Point(152, 0);
            this.btnProgTrack.Margin = new System.Windows.Forms.Padding(4);
            this.btnProgTrack.Name = "btnProgTrack";
            this.btnProgTrack.Size = new System.Drawing.Size(303, 34);
            this.btnProgTrack.TabIndex = 2;
            this.btnProgTrack.Text = "Literacy Progress Tracking";
            this.btnProgTrack.UseVisualStyleBackColor = false;
            this.btnProgTrack.Click += new System.EventHandler(this.btnProgTrack_Click);
            // 
            // ContextMenuStrip_Literacy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnProgTrack);
            this.Controls.Add(this.btnDashboard);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ContextMenuStrip_Literacy";
            this.Size = new System.Drawing.Size(464, 34);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnProgTrack;
    }
}
