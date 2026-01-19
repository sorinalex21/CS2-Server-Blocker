using System;
using System.Windows.Forms;
using System.Linq;

namespace CS2ServerPicker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            
            // Bind Events & Data
            chkAutoRefresh.Checked = Properties.Settings.Default.AutoRefreshPing;
            chkAutoRefresh.CheckedChanged += ChkAutoRefresh_CheckedChanged;

            chkAutoUpdateRules.Checked = Properties.Settings.Default.AutoUpdateRules;
            chkAutoUpdateRules.CheckedChanged += ChkAutoUpdateRules_CheckedChanged;

            UpdateUI(); 
            ApplyTheme();
        }

        private void ChkAutoUpdateRules_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoUpdateRules = chkAutoUpdateRules.Checked;
            Properties.Settings.Default.Save();
        }

        private void ChkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoRefreshPing = chkAutoRefresh.Checked;
            Properties.Settings.Default.Save();

            // Update main form immediately
            var mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.ToggleAutoRefresh(chkAutoRefresh.Checked);
            }
        }

        private void ApplyTheme()
        {
            this.StyleDarkWindow();
            
            // Sidebar Styling
            panelSidebar.BackColor = ThemeColors.SurfaceLight;
            btnGeneral.StyleButton(ThemeColors.SurfaceLight);
            btnAbout.StyleButton(ThemeColors.SurfaceLight);
            
            // Active Tab Styling (simplified: just standard button style for now, maybe add active state later)
            // Ideally we'd highlight the active one.
            
            // Content Styling
            panelContent.BackColor = ThemeColors.Background;
            pnlGeneral.BackColor = ThemeColors.Background;
            pnlAbout.BackColor = ThemeColors.Background;

            // Labels
            lblTheme.ForeColor = ThemeColors.Text;
            lblAppName.ForeColor = ThemeColors.Text;
            lblVersion.ForeColor = ThemeColors.Text;
            lblAuthor.ForeColor = ThemeColors.Text;
            linkGithub.LinkColor = ThemeColors.Accent;
            linkGithub.ActiveLinkColor = ThemeColors.AccentLight;
            linkGithub.VisitedLinkColor = ThemeColors.Accent;

            btnToggleTheme.StylePrimaryButton();
            
            if (chkAutoRefresh != null)
            {
                chkAutoRefresh.ForeColor = ThemeColors.Text;
                chkAutoRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            }
            if (chkAutoUpdateRules != null)
            {
                chkAutoUpdateRules.ForeColor = ThemeColors.Text;
                chkAutoUpdateRules.Font = new System.Drawing.Font("Segoe UI", 10F);
            }
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            ThemeColors.IsDarkMode = !ThemeColors.IsDarkMode;
            Properties.Settings.Default.DarkMode = ThemeColors.IsDarkMode;
            Properties.Settings.Default.Save();

            ApplyTheme();
            UpdateUI();

            var mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.ApplyTheme();
            }
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            ShowTab(pnlGeneral);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            ShowTab(pnlAbout);
        }

        private void ShowTab(Panel panelToShow)
        {
            pnlGeneral.Visible = false;
            pnlAbout.Visible = false;
            panelToShow.Visible = true;
            panelToShow.Dock = DockStyle.Fill; 
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/sorinalex21/CS2-Server-Blocker", // Assuming this is the repo, or just a placeholder
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void UpdateUI()
        {
            var resManager = new System.Resources.ResourceManager("CS2ServerPicker.Properties.Resources", typeof(SettingsForm).Assembly);
            
            this.Text = resManager.GetString("Settings_Title") ?? "Settings";
            btnGeneral.Text = resManager.GetString("Settings_General") ?? "General";
            btnAbout.Text = resManager.GetString("Settings_About") ?? "About";

            // General Tab
            string modeText = ThemeColors.IsDarkMode ? "Dark" : "Light";
            string format = resManager.GetString("LblCurrentMode") ?? "Current Mode: {0}";
            lblTheme.Text = string.Format(format, modeText);
            
            string btnText = ThemeColors.IsDarkMode 
                ? resManager.GetString("BtnSwitchToLight") 
                : resManager.GetString("BtnSwitchToDark");
            btnToggleTheme.Text = btnText ?? (ThemeColors.IsDarkMode ? "Switch to Light Mode" : "Switch to Dark Mode");
            
            if (chkAutoRefresh != null)
            {
                chkAutoRefresh.Text = resManager.GetString("ChkAutoRefresh") ?? "Auto-refresh ping (every 5s)";
            }

            // About Tab
            lblAppName.Text = resManager.GetString("About_AppName") ?? "CS2 Server Blocker";
            
            string verFormat = resManager.GetString("About_Version") ?? "Version {0}";
            lblVersion.Text = string.Format(verFormat, Form1.APP_VERSION);
            
            // Auto Update Rules
            if (chkAutoUpdateRules != null)
                chkAutoUpdateRules.Text = resManager.GetString("Settings_AutoUpdateRules") ?? "Auto-update firewall rules";

            lblAuthor.Text = resManager.GetString("About_Author") ?? "Developed by Krons";
            linkGithub.Text = resManager.GetString("About_Github") ?? "GitHub Repository";
        }
    }
}
