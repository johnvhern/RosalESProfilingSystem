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
            this.btnHome.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.Location = new System.Drawing.Point(0, 0);
            this.btnHome.Margin = new System.Windows.Forms.Padding(4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(111, 33);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnProfOfLearners
            // 
            this.btnProfOfLearners.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnProfOfLearners.FlatAppearance.BorderSize = 0;
            this.btnProfOfLearners.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfOfLearners.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProfOfLearners.Location = new System.Drawing.Point(111, 0);
            this.btnProfOfLearners.Margin = new System.Windows.Forms.Padding(4);
            this.btnProfOfLearners.Name = "btnProfOfLearners";
            this.btnProfOfLearners.Size = new System.Drawing.Size(178, 33);
            this.btnProfOfLearners.TabIndex = 1;
            this.btnProfOfLearners.Text = "Profile of Learners";
            this.btnProfOfLearners.UseVisualStyleBackColor = true;
            this.btnProfOfLearners.Click += new System.EventHandler(this.btnProfLearners_Click);
            // 
            // ContextMenuStrip_Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnProfOfLearners);
            this.Controls.Add(this.btnHome);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ContextMenuStrip_Home";
            this.Size = new System.Drawing.Size(553, 33);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnProfOfLearners;
    }
}
