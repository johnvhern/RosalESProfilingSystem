namespace RosalESProfilingSystem.Components
{
    partial class ContextMenuStrip_Home
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
            this.btnHome = new System.Windows.Forms.Button();
            this.btnProfOfLearners = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnHome.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnHome.Location = new System.Drawing.Point(196, 0);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(207, 28);
            this.btnHome.TabIndex = 3;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnProfOfLearners
            // 
            this.btnProfOfLearners.BackColor = System.Drawing.Color.Transparent;
            this.btnProfOfLearners.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnProfOfLearners.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnProfOfLearners.FlatAppearance.BorderSize = 0;
            this.btnProfOfLearners.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfOfLearners.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProfOfLearners.Location = new System.Drawing.Point(0, 0);
            this.btnProfOfLearners.Name = "btnProfOfLearners";
            this.btnProfOfLearners.Size = new System.Drawing.Size(196, 28);
            this.btnProfOfLearners.TabIndex = 2;
            this.btnProfOfLearners.Text = "Profile of Learners";
            this.btnProfOfLearners.UseVisualStyleBackColor = false;
            this.btnProfOfLearners.Click += new System.EventHandler(this.btnProfLearners_Click);
            // 
            // ContextMenuStrip_Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnProfOfLearners);
            this.MaximumSize = new System.Drawing.Size(403, 28);
            this.Name = "ContextMenuStrip_Home";
            this.Size = new System.Drawing.Size(403, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnProfOfLearners;
    }
}
