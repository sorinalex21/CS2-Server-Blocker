namespace CS2ServerPicker
{
    partial class SettingsForm
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
            this.container = new System.Windows.Forms.SplitContainer();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnGeneral = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.pnlGeneral = new System.Windows.Forms.Panel();
            this.btnToggleTheme = new System.Windows.Forms.Button();
            this.lblTheme = new System.Windows.Forms.Label();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.linkGithub = new System.Windows.Forms.LinkLabel();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.chkAutoUpdateRules = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.container)).BeginInit();
            this.container.Panel1.SuspendLayout();
            this.container.Panel2.SuspendLayout();
            this.container.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.pnlGeneral.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.container.IsSplitterFixed = true;
            this.container.Location = new System.Drawing.Point(0, 0);
            this.container.Name = "container";
            // 
            // container.Panel1
            // 
            this.container.Panel1.Controls.Add(this.panelSidebar);
            // 
            // container.Panel2
            // 
            this.container.Panel2.Controls.Add(this.panelContent);
            this.container.Size = new System.Drawing.Size(600, 400);
            this.container.SplitterDistance = 150;
            this.container.SplitterWidth = 1;
            this.container.TabIndex = 0;
            // 
            // panelSidebar
            // 
            this.panelSidebar.Controls.Add(this.btnAbout);
            this.panelSidebar.Controls.Add(this.btnGeneral);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(150, 400);
            this.panelSidebar.TabIndex = 0;
            // 
            // btnAbout
            // 
            this.btnAbout.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAbout.Location = new System.Drawing.Point(0, 45);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(150, 45);
            this.btnAbout.TabIndex = 1;
            this.btnAbout.Text = "About";
            this.btnAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnGeneral
            // 
            this.btnGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGeneral.Location = new System.Drawing.Point(0, 0);
            this.btnGeneral.Name = "btnGeneral";
            this.btnGeneral.Size = new System.Drawing.Size(150, 45);
            this.btnGeneral.TabIndex = 0;
            this.btnGeneral.Text = "General";
            this.btnGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGeneral.UseVisualStyleBackColor = true;
            this.btnGeneral.Click += new System.EventHandler(this.btnGeneral_Click);
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.pnlGeneral);
            this.panelContent.Controls.Add(this.pnlAbout);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(449, 400);
            this.panelContent.TabIndex = 0;
            // 
            // pnlGeneral
            // 
            this.pnlGeneral.Controls.Add(this.btnToggleTheme);
            this.pnlGeneral.Controls.Add(this.lblTheme);
            this.pnlGeneral.Controls.Add(this.chkAutoRefresh);
            this.pnlGeneral.Controls.Add(this.chkAutoUpdateRules);
            this.pnlGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGeneral.Location = new System.Drawing.Point(0, 0);
            this.pnlGeneral.Name = "pnlGeneral";
            this.pnlGeneral.Padding = new System.Windows.Forms.Padding(20);
            this.pnlGeneral.Size = new System.Drawing.Size(449, 400);
            this.pnlGeneral.TabIndex = 0;
            // 
            // btnToggleTheme
            // 
            this.btnToggleTheme.Location = new System.Drawing.Point(20, 60);
            this.btnToggleTheme.Name = "btnToggleTheme";
            this.btnToggleTheme.Size = new System.Drawing.Size(200, 35);
            this.btnToggleTheme.TabIndex = 1;
            this.btnToggleTheme.Text = "Switch to Light Mode";
            this.btnToggleTheme.UseVisualStyleBackColor = true;
            this.btnToggleTheme.Click += new System.EventHandler(this.btnToggleTheme_Click);
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(20, 20);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(90, 17);
            this.lblTheme.TabIndex = 0;
            this.lblTheme.Text = "Current: Dark";
            // 
            // chkAutoRefresh
            // 
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Location = new System.Drawing.Point(20, 130);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(200, 21);
            this.chkAutoRefresh.TabIndex = 2;
            this.chkAutoRefresh.Text = "Auto-refresh ping";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            // 
            // chkAutoUpdateRules
            // 
            this.chkAutoUpdateRules.AutoSize = true;
            this.chkAutoUpdateRules.Location = new System.Drawing.Point(20, 170);
            this.chkAutoUpdateRules.Name = "chkAutoUpdateRules";
            this.chkAutoUpdateRules.Size = new System.Drawing.Size(200, 21);
            this.chkAutoUpdateRules.TabIndex = 3;
            this.chkAutoUpdateRules.Text = "Auto-update firewall rules";
            this.chkAutoUpdateRules.UseVisualStyleBackColor = true;
            // 
            // pnlAbout
            // 
            this.pnlAbout.Controls.Add(this.linkGithub);
            this.pnlAbout.Controls.Add(this.lblAuthor);
            this.pnlAbout.Controls.Add(this.lblVersion);
            this.pnlAbout.Controls.Add(this.lblAppName);
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(0, 0);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Padding = new System.Windows.Forms.Padding(20);
            this.pnlAbout.Size = new System.Drawing.Size(449, 400);
            this.pnlAbout.TabIndex = 1;
            this.pnlAbout.Visible = false;
            // 
            // linkGithub
            // 
            this.linkGithub.AutoSize = true;
            this.linkGithub.Location = new System.Drawing.Point(20, 120);
            this.linkGithub.Name = "linkGithub";
            this.linkGithub.Size = new System.Drawing.Size(116, 17);
            this.linkGithub.TabIndex = 3;
            this.linkGithub.TabStop = true;
            this.linkGithub.Text = "GitHub Repository";
            this.linkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(20, 80);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(127, 17);
            this.lblAuthor.TabIndex = 2;
            this.lblAuthor.Text = "Developed by Sorin";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(20, 50);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(84, 17);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Version 1.0.0";
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAppName.Location = new System.Drawing.Point(20, 20);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(140, 21);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "CS2 Server Picker";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.container);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.container.Panel1.ResumeLayout(false);
            this.container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.container)).EndInit();
            this.container.ResumeLayout(false);
            this.panelSidebar.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.pnlGeneral.ResumeLayout(false);
            this.pnlGeneral.PerformLayout();
            this.pnlAbout.ResumeLayout(false);
            this.pnlAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer container;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnGeneral;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Panel panelContent;
        // General Page
        private System.Windows.Forms.Panel pnlGeneral;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.Button btnToggleTheme;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.CheckBox chkAutoUpdateRules;
        // About Page
        private System.Windows.Forms.Panel pnlAbout;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.LinkLabel linkGithub;
    }
}