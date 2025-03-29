namespace RosalESProfilingSystem.Components
{
    partial class SidePanelNavigation
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SidePanelNavigation));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnLiteracyPage = new System.Windows.Forms.Button();
            this.panelNumeracy = new System.Windows.Forms.Panel();
            this.btnERUNT = new System.Windows.Forms.Button();
            this.btnRMA = new System.Windows.Forms.Button();
            this.btnNumeracyPage = new System.Windows.Forms.Button();
            this.btnSciProf = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnLogout = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelNumeracy.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.pictureBox1);
            this.flowLayoutPanel1.Controls.Add(this.btnHome);
            this.flowLayoutPanel1.Controls.Add(this.btnLiteracyPage);
            this.flowLayoutPanel1.Controls.Add(this.panelNumeracy);
            this.flowLayoutPanel1.Controls.Add(this.btnSciProf);
            this.flowLayoutPanel1.Controls.Add(this.btnSettings);
            this.flowLayoutPanel1.Controls.Add(this.btnLogout);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(208, 858);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(202, 149);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.Image = ((System.Drawing.Image)(resources.GetObject("btnHome.Image")));
            this.btnHome.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHome.Location = new System.Drawing.Point(0, 158);
            this.btnHome.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnHome.Name = "btnHome";
            this.btnHome.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnHome.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnHome.Size = new System.Drawing.Size(220, 76);
            this.btnHome.TabIndex = 7;
            this.btnHome.Text = "Home";
            this.btnHome.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnLiteracyPage
            // 
            this.btnLiteracyPage.FlatAppearance.BorderSize = 0;
            this.btnLiteracyPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLiteracyPage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLiteracyPage.Image = ((System.Drawing.Image)(resources.GetObject("btnLiteracyPage.Image")));
            this.btnLiteracyPage.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLiteracyPage.Location = new System.Drawing.Point(0, 240);
            this.btnLiteracyPage.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnLiteracyPage.Name = "btnLiteracyPage";
            this.btnLiteracyPage.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnLiteracyPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnLiteracyPage.Size = new System.Drawing.Size(217, 76);
            this.btnLiteracyPage.TabIndex = 8;
            this.btnLiteracyPage.Text = "Literacy";
            this.btnLiteracyPage.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnLiteracyPage.UseVisualStyleBackColor = true;
            this.btnLiteracyPage.Click += new System.EventHandler(this.btnLiteracyPage_Click);
            // 
            // panelNumeracy
            // 
            this.panelNumeracy.Controls.Add(this.btnERUNT);
            this.panelNumeracy.Controls.Add(this.btnRMA);
            this.panelNumeracy.Controls.Add(this.btnNumeracyPage);
            this.panelNumeracy.Location = new System.Drawing.Point(0, 322);
            this.panelNumeracy.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panelNumeracy.MaximumSize = new System.Drawing.Size(207, 155);
            this.panelNumeracy.MinimumSize = new System.Drawing.Size(207, 76);
            this.panelNumeracy.Name = "panelNumeracy";
            this.panelNumeracy.Size = new System.Drawing.Size(207, 76);
            this.panelNumeracy.TabIndex = 12;
            // 
            // btnERUNT
            // 
            this.btnERUNT.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnERUNT.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnERUNT.FlatAppearance.BorderSize = 0;
            this.btnERUNT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnERUNT.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnERUNT.Location = new System.Drawing.Point(0, 115);
            this.btnERUNT.Name = "btnERUNT";
            this.btnERUNT.Size = new System.Drawing.Size(207, 39);
            this.btnERUNT.TabIndex = 4;
            this.btnERUNT.Text = "ERUNT";
            this.btnERUNT.UseVisualStyleBackColor = false;
            this.btnERUNT.Click += new System.EventHandler(this.btnERUNT_Click);
            // 
            // btnRMA
            // 
            this.btnRMA.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnRMA.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRMA.FlatAppearance.BorderSize = 0;
            this.btnRMA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRMA.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRMA.Location = new System.Drawing.Point(0, 76);
            this.btnRMA.Name = "btnRMA";
            this.btnRMA.Size = new System.Drawing.Size(207, 39);
            this.btnRMA.TabIndex = 3;
            this.btnRMA.Text = "RMA";
            this.btnRMA.UseVisualStyleBackColor = false;
            this.btnRMA.Click += new System.EventHandler(this.btnRMA_Click);
            // 
            // btnNumeracyPage
            // 
            this.btnNumeracyPage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNumeracyPage.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNumeracyPage.FlatAppearance.BorderSize = 0;
            this.btnNumeracyPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNumeracyPage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNumeracyPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNumeracyPage.Image")));
            this.btnNumeracyPage.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNumeracyPage.Location = new System.Drawing.Point(0, 0);
            this.btnNumeracyPage.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnNumeracyPage.Name = "btnNumeracyPage";
            this.btnNumeracyPage.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNumeracyPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnNumeracyPage.Size = new System.Drawing.Size(207, 76);
            this.btnNumeracyPage.TabIndex = 2;
            this.btnNumeracyPage.Text = "⮟         Numeracy";
            this.btnNumeracyPage.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnNumeracyPage.UseVisualStyleBackColor = false;
            this.btnNumeracyPage.Click += new System.EventHandler(this.btnNumeracyPage_Click);
            // 
            // btnSciProf
            // 
            this.btnSciProf.FlatAppearance.BorderSize = 0;
            this.btnSciProf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSciProf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSciProf.Image = ((System.Drawing.Image)(resources.GetObject("btnSciProf.Image")));
            this.btnSciProf.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSciProf.Location = new System.Drawing.Point(0, 404);
            this.btnSciProf.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnSciProf.Name = "btnSciProf";
            this.btnSciProf.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnSciProf.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSciProf.Size = new System.Drawing.Size(217, 76);
            this.btnSciProf.TabIndex = 10;
            this.btnSciProf.Text = "Science Proficiency";
            this.btnSciProf.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSciProf.UseVisualStyleBackColor = true;
            this.btnSciProf.Click += new System.EventHandler(this.btnSciProf_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSettings.Location = new System.Drawing.Point(0, 486);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSettings.Size = new System.Drawing.Size(217, 76);
            this.btnSettings.TabIndex = 11;
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 15;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnLogout
            // 
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.Image = ((System.Drawing.Image)(resources.GetObject("btnLogout.Image")));
            this.btnLogout.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogout.Location = new System.Drawing.Point(0, 568);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnLogout.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnLogout.Size = new System.Drawing.Size(217, 76);
            this.btnLogout.TabIndex = 13;
            this.btnLogout.Text = "Logout";
            this.btnLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // SidePanelNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SidePanelNavigation";
            this.Size = new System.Drawing.Size(208, 858);
            this.Load += new System.EventHandler(this.SidePanelNavigation_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelNumeracy.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSciProf;
        private System.Windows.Forms.Panel panelNumeracy;
        private System.Windows.Forms.Button btnERUNT;
        private System.Windows.Forms.Button btnRMA;
        private System.Windows.Forms.Button btnNumeracyPage;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnLiteracyPage;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnLogout;
    }
}
