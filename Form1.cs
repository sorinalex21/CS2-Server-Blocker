using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using CS2ServerPicker.Services;
using CS2ServerPicker.Models;

namespace CS2ServerPicker
{
    public partial class Form1 : Form
    {
        public const string APP_VERSION = "v1.2.0";
        private const string APP_AUTHOR = "By Krons";

        private TextBox txtSearch;
        private Button btnSettings;

        private readonly SteamApiService _steamService;
        private readonly FirewallService _firewallService;
        private readonly PingService _pingService;
        private readonly FlagService _flagService;

        private System.Windows.Forms.Timer _autoRefreshTimer;
        private Button btnRefreshPing;
        private ToolStripStatusLabel lblUpdateStatus;

        public Form1()
        {
            InitializeComponent();
            try { this.Icon = new Icon("icon.ico"); } catch { }

            this.Text = $"CS2 Server Blocker {APP_VERSION} - {APP_AUTHOR}";
            this.StartPosition = FormStartPosition.CenterScreen;

            EnableDoubleBuffering(dataGridView1);

            _steamService = new SteamApiService();
            _firewallService = new FirewallService();
            _pingService = new PingService();
            _flagService = new FlagService();

            SetupLanguageDropdown();
            ApplySavedLanguage();

            // Init Theme
            ThemeColors.IsDarkMode = Properties.Settings.Default.DarkMode;
            
            SetupLayout();       // Layout triggers grid setup too
            SetupGridColumns();  // Ensure columns are set up after styling grid
            UpdateUI(); // Force column creation and localization
            ApplyTheme(); // Apply styles initially

            // Disable buttons at startup
            btnApply.Enabled = false;
            btnReset.Enabled = false;

            // Timer for AutoRefresh
            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 5000; // 5 seconds
            _autoRefreshTimer.Tick += _autoRefreshTimer_Tick;
            _autoRefreshTimer.Enabled = Properties.Settings.Default.AutoRefreshPing;

            // Event Wiring
            btnLoad.Click -= btnLoad_Click;
            btnLoad.Click += btnLoad_Click;
            // ... (rest of event wiring)
            btnApply.Click -= btnApply_Click;
            btnApply.Click += btnApply_Click;

            btnReset.Click -= btnReset_Click;
            btnReset.Click += btnReset_Click;

            if (btnRefreshPing != null) btnRefreshPing.Click += BtnRefreshPing_Click;

            dataGridView1.CurrentCellDirtyStateChanged += DataGridView1_CurrentCellDirtyStateChanged;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView1.MouseClick -= DataGridView1_MouseClick;
            dataGridView1.MouseClick += DataGridView1_MouseClick;

            if (txtSearch != null) txtSearch.TextChanged += TxtSearch_TextChanged;
            if (btnSettings != null) btnSettings.Click += BtnSettings_Click;

            // Check for updates
            _ = CheckForUpdatesAsync();
        }

        public void ToggleAutoRefresh(bool enabled)
        {
            _autoRefreshTimer.Enabled = enabled;
        }

        private async void _autoRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && btnLoad.Enabled) // Only refresh if loaded and not loading
            {
                await ProcessServerDetails();
            }
        }

        private async void BtnRefreshPing_Click(object sender, EventArgs e)
        {
             btnRefreshPing.Enabled = false;
             await ProcessServerDetails();
             btnRefreshPing.Enabled = true;
        }

        public void ApplyTheme()
        {
            // Reload global theme state if needed, though IsDarkMode is static.
            // Apply Window Style
            this.UseImmersiveDarkMode(ThemeColors.IsDarkMode);
            this.StyleDarkWindow();

            // Apply Control Styles
            btnLoad.StylePrimaryButton();
            btnRefreshPing.StyleButton(); // Style the new button
            btnApply.StyleSuccessButton();
            btnReset.StyleDangerButton();
            
            if (btnSettings != null) btnSettings.StyleButton();
            if (txtSearch != null) txtSearch.StyleTextBox();
            if (comboLimbaj != null) comboLimbaj.StyleComboBox();

            // Status Strip
            statusStrip1.BackColor = ThemeColors.Surface;
            statusStrip1.ForeColor = ThemeColors.Text; // Ensure text is visible
            UpdateStatusLabel(); // Updates text color
            
            if (lblUpdateStatus != null)
            {
                 lblUpdateStatus.ForeColor = ThemeColors.Danger;
                 lblUpdateStatus.IsLink = true;
                 lblUpdateStatus.LinkColor = ThemeColors.Danger;
                 lblUpdateStatus.ActiveLinkColor = ThemeColors.Warning;
                 lblUpdateStatus.VisitedLinkColor = ThemeColors.Danger;
            }

            // Grid
            dataGridView1.StyleGridView();
            
            // Force row color update
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                 if (row.IsNewRow) continue;
                 UpdateRowColor(row);
            }
            
            dataGridView1.Refresh(); // Force repaint
        }

        private void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        // --- LAYOUT ---
        private void SetupLayout()
        {
            this.MinimumSize = new Size(900, 600);
            int margin = 20;
            int statusBarHeight = 25;

            // Top Bar area
            btnLoad.Location = new Point(margin, margin);
            btnLoad.Size = new Size(180, 35); // Bigger button

            // Refresh Ping Button
            if (btnRefreshPing == null)
            {
                btnRefreshPing = new Button();
                btnRefreshPing.Text = Properties.Resources.BtnRefreshPing;
                btnRefreshPing.Size = new Size(140, 35);
                btnRefreshPing.Cursor = Cursors.Hand;
                btnRefreshPing.StyleButton();
                this.Controls.Add(btnRefreshPing);
            }
            btnRefreshPing.Location = new Point(btnLoad.Right + 10, margin);
            
            if (txtSearch == null)
            {
                txtSearch = new TextBox();
                txtSearch.PlaceholderText = "Search (ex: Poland)...";
                txtSearch.Size = new Size(200, 35); // Slightly smaller to fit
                this.Controls.Add(txtSearch);
            }
            txtSearch.StyleTextBox(); // Apply Style
            txtSearch.Location = new Point(btnRefreshPing.Right + 20, margin + (btnLoad.Height - txtSearch.Height) / 2); // Vertically centered to button
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            comboLimbaj.Location = new Point(this.ClientSize.Width - comboLimbaj.Width - margin, margin + (btnLoad.Height - comboLimbaj.Height)/2);
            comboLimbaj.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            if (btnSettings == null)
            {
                btnSettings = new Button();
                btnSettings.Text = "⚙️";
                btnSettings.Size = new Size(40, 35); // Bigger settings button matching Load button height
                btnSettings.Cursor = Cursors.Hand;
                btnSettings.StyleButton(); // Style it
                this.Controls.Add(btnSettings);
            }
            btnSettings.Location = new Point(comboLimbaj.Location.X - btnSettings.Width - 10, btnLoad.Location.Y);
            btnSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;


            // Bottom Bar area
            btnReset.Size = new Size(160, 35);
            btnReset.Location = new Point(this.ClientSize.Width - btnReset.Width - margin,
                                          this.ClientSize.Height - btnReset.Height - margin - statusBarHeight);
            btnReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            btnApply.Size = new Size(160, 35);
            btnApply.Location = new Point(btnReset.Location.X - btnApply.Width - margin,
                                          btnReset.Location.Y);
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            // Update Label in StatusStrip
            if (lblUpdateStatus == null)
            {
                lblUpdateStatus = new ToolStripStatusLabel();
                lblUpdateStatus.Spring = true;
                lblUpdateStatus.TextAlign = ContentAlignment.MiddleRight;
                lblUpdateStatus.Click += LblUpdateStatus_Click;
                statusStrip1.Items.Add(lblUpdateStatus);
            }

            // Grid Area
            // Add a panel for grid potentially? No, just grid floating is fine but let's give it margins
            dataGridView1.Location = new Point(margin, btnLoad.Bottom + margin);
            int bottomSpace = (this.ClientSize.Height - btnApply.Top) + margin; // Space reserved for bottom buttons
            dataGridView1.Size = new Size(this.ClientSize.Width - (margin * 2),
                                          this.ClientSize.Height - dataGridView1.Top - bottomSpace - 10);
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            
            // Grid Styling
            dataGridView1.StyleGridView();
        }

        private void LblUpdateStatus_Click(object sender, EventArgs e)
        {
            if (lblUpdateStatus.Tag != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = lblUpdateStatus.Tag.ToString(),
                        UseShellExecute = true
                    });
                }
                catch { }
            }
        }

        // --- EVENTS ---

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try { 
                SettingsForm settingsForm = new SettingsForm(); 
                settingsForm.StyleDarkWindow(); // Style settings form too
                settingsForm.ShowDialog(); 
            } catch { }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearch.Text.ToLower().Trim();
            dataGridView1.SuspendLayout();
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (string.IsNullOrEmpty(filter)) { row.Visible = true; continue; }

                    string country = row.Cells["colCountry"].Value?.ToString().ToLower() ?? "";
                    string city = row.Cells["colCity"].Value?.ToString().ToLower() ?? "";
                    row.Visible = country.Contains(filter) || city.Contains(filter);
                }
            }
            catch { }
            finally { dataGridView1.ResumeLayout(); }
        }

        // --- BUTTON LOGIC ---

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;

            // Ensure they remain disabled during loading
            btnApply.Enabled = false;
            btnReset.Enabled = false;
            if (btnRefreshPing != null) btnRefreshPing.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
            btnLoad.Text = Properties.Resources.MsgDownloading;
            dataGridView1.Rows.Clear();
            // SetupGridColumns(); // Removed to prevent column flickering

            try
            {
                List<GameServer> servers = await _steamService.GetRelaysAsync();
                List<string> blockedCities = _firewallService.GetBlockedCities();

                var preparedList = servers.Select(server =>
                {
                    string code = _flagService.GetCountryCode(server.City);
                    string countryName = _flagService.GetCountryName(code);

                    if (countryName == "Unknown" || countryName == "Localhost")
                    {
                        var parts = server.City.Split(' ');
                        if (parts.Length > 0) countryName = parts[0];
                    }

                    return new
                    {
                        ServerObject = server,
                        CountryCode = code,
                        CountryName = countryName
                    };
                })
                .OrderBy(x => x.CountryName)
                .ThenBy(x => x.ServerObject.City)
                .ToList();

                // Failsafe: Ensure columns exist before populating
                if (!dataGridView1.Columns.Contains("colIPs"))
                {
                    dataGridView1.Columns.Add("colIPs", "IPs");
                    dataGridView1.Columns["colIPs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns["colIPs"].Width = 70;
                }
                
                List<string> citiesToSync = new List<string>();
                Dictionary<string, string> cityIpMap = new Dictionary<string, string>();
                
                // Get precise counting of blocked IPs
                var blockedCounts = _firewallService.GetBlockedIpCounts();

                foreach (var item in preparedList)
                {
                    string normalizedCity = item.ServerObject.City.Replace(" ", "");
                    bool isBlocked = blockedCities.Any(b => normalizedCity.Contains(b.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));

                    // Calculate IP Stats
                    int totalIps = 0;
                    if (SteamApiService.CityRanges.ContainsKey(item.ServerObject.City))
                        totalIps = SteamApiService.CityRanges[item.ServerObject.City].Count;
                    else
                        totalIps = 1; // Fallback

                    // Count blocked for this city
                    // We need to match the normalized name used in FirewallService key
                    string keyToFind = normalizedCity;
                    // Try exact match or similar logic to what we did in parsing
                    int blockedIps = 0;
                    if(blockedCounts.Keys.Any(k => k.Equals(normalizedCity, StringComparison.OrdinalIgnoreCase)))
                    {
                         blockedIps = blockedCounts.FirstOrDefault(k => k.Key.Equals(normalizedCity, StringComparison.OrdinalIgnoreCase)).Value;
                    }

                    string ipStatus = $"{blockedIps}/{totalIps}";

                    // Add empty row first, then set cells by name to avoid index mismatch
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];

                    row.Cells["colBlock"].Value = isBlocked;
                    // colCountry expects string value for Text painting, image is set in Tag
                    row.Cells["colCountry"].Value = item.CountryName; 
                    row.Cells["colCity"].Value = item.ServerObject.City;
                    row.Cells["colPing"].Value = -1L;
                    row.Cells["colIPs"].Value = ipStatus; // Explicit assignment

                    row.Tag = isBlocked;
                    row.Cells["colPing"].Tag = item.ServerObject.IpAddress;

                    UpdateRowColor(row);

                    if (isBlocked)
                    {
                        // Check if counts mismatch and we need sync
                        if (blockedIps != totalIps)
                        {
                             citiesToSync.Add(item.ServerObject.City);
                             cityIpMap[item.ServerObject.City] = item.ServerObject.IpAddress;
                        }
                    }
                }

                // SMART SYNC: Automatically update firewall rules ONLY if setting is enabled
                if (Properties.Settings.Default.AutoUpdateRules && citiesToSync.Count > 0)
                {
                   await Task.Run(() => _firewallService.BatchUpdateRules(citiesToSync, new List<string>(), cityIpMap));
                   
                   // Update UI to reflect new counts immediately (visual fake-update)
                   foreach(DataGridViewRow row in dataGridView1.Rows)
                   {
                        string city = row.Cells["colCity"].Value.ToString();
                        if (citiesToSync.Contains(city))
                        {
                            // Assume full block
                            if (SteamApiService.CityRanges.ContainsKey(city))
                            {
                                int total = SteamApiService.CityRanges[city].Count;
                                row.Cells["colIPs"].Value = $"{total}/{total}";
                            }
                        }
                   }
                }

                UpdateStatusLabel();
                await ProcessServerDetails();
                MessageBox.Show(Properties.Resources.MsgDone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.TitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnLoad.Enabled = true;

                // Enable if data is loaded
                if (dataGridView1.Rows.Count > 0)
                {
                    btnApply.Enabled = true;
                    btnReset.Enabled = true;
                    if (btnRefreshPing != null) btnRefreshPing.Enabled = true;
                }
                UpdateUI();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (sender is Button btnStart)
            {
                btnStart.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                btnStart.Text = Properties.Resources.MsgWorking;
            }

            try
            {
                List<string> citiesToBlock = new List<string>();
                List<string> citiesToUnblock = new List<string>();
                Dictionary<string, string> cityIpMap = new Dictionary<string, string>();
                int blockedCount = 0;

                // 1. Gather Data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    bool isChecked = Convert.ToBoolean(row.Cells["colBlock"].Value);
                    string city = row.Cells["colCity"].Value?.ToString();
                    string ip = row.Cells["colPing"].Tag?.ToString();

                    if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(ip)) continue;

                    if (isChecked)
                    {
                        citiesToBlock.Add(city);
                        cityIpMap[city] = ip; // Store IP for blocking needs
                        blockedCount++;
                    }
                    else
                    {
                        citiesToUnblock.Add(city);
                    }
                }

                // 2. Perform Batch Update
                _firewallService.BatchUpdateRules(citiesToBlock, citiesToUnblock, cityIpMap);

                // 3. Update UI locally (assume success to avoid re-reading slow WMI/Netsh)
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    bool isChecked = Convert.ToBoolean(row.Cells["colBlock"].Value);
                    row.Tag = isChecked; // Update 'IsActuallyBlocked' status
                    UpdateRowColor(row);
                }

                UpdateStatusLabel();
                UpdateIpColumn(); // Refresh IP counts (e.g. 1/2 -> 2/2)
                string message = string.Format(Properties.Resources.MsgApplySuccess, blockedCount);
                MessageBox.Show(message, Properties.Resources.MsgApplyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string errBody = string.Format(Properties.Resources.MsgFirewallError, ex.Message) + "\n" + Properties.Resources.MsgAdminReq;
                MessageBox.Show(errBody, Properties.Resources.MsgErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                if (sender is Button btnEnd)
                {
                    btnEnd.Enabled = true;
                    btnEnd.Text = Properties.Resources.btnApply;
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.MsgConfirmReset, Properties.Resources.TitleConfirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                _firewallService.DeleteAllRulesVitezaLuminis();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    row.Cells["colBlock"].Value = false;
                    row.Tag = false;
                    UpdateRowColor(row);
                }
                UpdateStatusLabel();
                UpdateIpColumn(); // Reset counts to 0/Total
                MessageBox.Show(Properties.Resources.MsgResetDone, Properties.Resources.TitleInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.TitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // --- GRID CONFIG ---

        private void SetupGridColumns()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // Borderless style handled by StyleGridView(), setting background here just to be safe if refreshed
            dataGridView1.BackgroundColor = ThemeColors.Surface; 
            
            dataGridView1.RowTemplate.Height = 35; // Taller rows for modern look
            dataGridView1.AllowUserToAddRows = false;


            // 0. Checkbox
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "colBlock";
            checkColumn.HeaderText = "";
            checkColumn.Width = 50;
            checkColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            checkColumn.Resizable = DataGridViewTriState.False;
            checkColumn.ReadOnly = false;
            dataGridView1.Columns.Add(checkColumn);

            // 1. Country Flag
            DataGridViewTextBoxColumn countryCol = new DataGridViewTextBoxColumn();
            countryCol.Name = "colCountry";
            countryCol.HeaderText = Properties.Resources.ColFlag;
            countryCol.Width = 140;
            countryCol.ReadOnly = true;
            dataGridView1.Columns.Add(countryCol);

            // 2. City
            DataGridViewTextBoxColumn cityCol = new DataGridViewTextBoxColumn();
            cityCol.Name = "colCity";
            cityCol.HeaderText = Properties.Resources.ColCity;
            cityCol.ReadOnly = true;
            dataGridView1.Columns.Add(cityCol);

            // 3. Ping
            DataGridViewTextBoxColumn pingCol = new DataGridViewTextBoxColumn();
            pingCol.Name = "colPing";
            pingCol.HeaderText = Properties.Resources.ColPing;
            pingCol.ValueType = typeof(long);
            pingCol.ReadOnly = true;
            dataGridView1.Columns.Add(pingCol);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[1].Width = 140;
            dataGridView1.Columns["colPing"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void UpdateRowColor(DataGridViewRow row)
        {
            if (row.Tag == null) return;

            bool isChecked = Convert.ToBoolean(row.Cells["colBlock"].Value);
            bool isActuallyBlocked = (bool)row.Tag;

            if (isChecked != isActuallyBlocked)
            {
                row.DefaultCellStyle.BackColor = ThemeColors.GridRowDiff;
                row.DefaultCellStyle.SelectionBackColor = ThemeColors.GridRowDiffSel;
            }
            else if (isChecked && isActuallyBlocked)
            {
                row.DefaultCellStyle.BackColor = ThemeColors.GridRowBloch;
                row.DefaultCellStyle.SelectionBackColor = ThemeColors.GridRowBlochSel;
            }
            else
            {
                row.DefaultCellStyle.BackColor = ThemeColors.Surface;
                row.DefaultCellStyle.SelectionBackColor = ThemeColors.GridSelectionBack;
            }
            row.DefaultCellStyle.ForeColor = ThemeColors.Text;
        }

        private void UpdateStatusLabel()
        {
            int blockedCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag != null && (bool)row.Tag == true) blockedCount++;
            }
            lblStatus.Text = $"{blockedCount} servers blocked active in Firewall.";
            // Use Theme Colors
            lblStatus.ForeColor = (blockedCount > 0) ? ThemeColors.Danger : ThemeColors.TextDim;
            statusStrip1.BackColor = ThemeColors.Surface; // Status Strip background
        }

        // --- ASYNC & PAINTING ---

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "colPing" && e.Value != null)
            {
                // Check if row is blocked
                bool isBlocked = false;
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    var val = dataGridView1.Rows[e.RowIndex].Cells["colBlock"].Value;
                    if (val != null && val is bool b) isBlocked = b;
                }

                if (isBlocked)
                {
                    e.Value = "Blocked";
                    e.CellStyle.ForeColor = ThemeColors.Danger;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                    e.FormattingApplied = true;
                    return;
                }

                if (long.TryParse(e.Value.ToString(), out long latency))
                {
                    if (latency == -1) { e.Value = "..."; }
                    else if (latency == 999 || latency == long.MaxValue) { e.Value = "Timeout"; e.CellStyle.ForeColor = ThemeColors.Danger; }
                    else
                    {
                        e.Value = $"{latency} ms";
                        if (latency < 50) e.CellStyle.ForeColor = ThemeColors.Success; // Green
                        else if (latency < 100) e.CellStyle.ForeColor = ThemeColors.Warning; // Orange/Gold
                        else e.CellStyle.ForeColor = ThemeColors.Danger; // Red
                    }

                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                    e.FormattingApplied = true;
                }
            }
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "colCountry" && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Image flagImg = cell.Tag as Image;
                string countryName = e.Value?.ToString() ?? "";

                if (flagImg != null)
                {
                    int flagW = 20; int flagH = 15;
                    int yPos = e.CellBounds.Y + (e.CellBounds.Height - flagH) / 2;
                    e.Graphics.DrawImage(flagImg, new Rectangle(e.CellBounds.X + 5, yPos, flagW, flagH));
                }

                if (!string.IsNullOrEmpty(countryName))
                {
                    // Use Theme Text Color
                    using (Brush brush = new SolidBrush(ThemeColors.Text)) 
                    {
                        TextRenderer.DrawText(e.Graphics, countryName, e.CellStyle.Font,
                            new Point(e.CellBounds.X + 30, e.CellBounds.Y + (e.CellBounds.Height - e.CellStyle.Font.Height) / 2),
                            e.CellStyle.ForeColor); // Use cell style forecolor which might be affected by selection
                    }
                }
                e.Handled = true;
            }
        }

        private async Task ProcessServerDetails()
        {
            var tasks = new List<Task>();
            using (var semaphore = new SemaphoreSlim(25))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    tasks.Add(ProcessSingleRowAsync(row, semaphore));
                }
                await Task.WhenAll(tasks);
            }
        }

        private async Task ProcessSingleRowAsync(DataGridViewRow row, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                string city = row.Cells["colCity"].Value?.ToString();
                string ip = row.Cells["colPing"].Tag?.ToString();

                if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(ip)) return;

                string countryCode = _flagService.GetCountryCode(city);
                Image flag = await _flagService.GetFlagImageAsync(countryCode);
                long latency = await _pingService.GetLatencyAsync(ip);

                this.BeginInvoke(new Action(() => // Changed to BeginInvoke for async safety
                {
                    if (row.DataGridView != null)
                    {
                        row.Cells["colCountry"].Tag = flag;
                        row.Cells["colPing"].Value = latency;
                        dataGridView1.InvalidateCell(row.Cells["colCountry"]);
                    }
                }));
            }
            catch { }
            finally { semaphore.Release(); }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "CS2ServerPicker-App");
                    string url = "https://api.github.com/repos/sorinalex21/CS2-Server-Blocker/releases/latest";
                    string json = await client.GetStringAsync(url);

                    dynamic release = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    string tagName = release.tag_name;
                    string htmlUrl = release.html_url;

                    if (!string.IsNullOrEmpty(tagName) && tagName != APP_VERSION)
                    {
                        var msg = Properties.Resources.MsgUpdateStatus;
                        // Use string format if the resource has {0}, otherwise fallback
                        string statusText = string.Format(msg, tagName);

                        this.Invoke(new Action(() =>
                        {
                            if (lblUpdateStatus != null)
                            {
                                lblUpdateStatus.Text = statusText;
                                lblUpdateStatus.Tag = htmlUrl;
                                lblUpdateStatus.Visible = true;
                            }
                        }));
                    }
                }
            }
            catch { }
        }

        // --- HELPERS ---

        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty) dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) UpdateRowColor(dataGridView1.Rows[e.RowIndex]);
        }

        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[currentMouseOverRow].Selected = true;

                    string countryName = dataGridView1.Rows[currentMouseOverRow].Cells["colCountry"].Value?.ToString();
                    if (string.IsNullOrEmpty(countryName)) return;

                    ContextMenuStrip mnu = new ContextMenuStrip();
                    
                    // Dark Theme for Context Menu
                    mnu.BackColor = ThemeColors.SurfaceLight;
                    mnu.ForeColor = ThemeColors.Text;
                    mnu.RenderMode = ToolStripRenderMode.System; // Simplifies rendering or use Professional with custom color table
                    
                    ToolStripMenuItem btnBlock = new ToolStripMenuItem(string.Format(Properties.Resources.CtxBlock, countryName));
                    btnBlock.Click += (s, ev) => SetCountrySelection(true, countryName);
                    btnBlock.BackColor = ThemeColors.SurfaceLight;
                    btnBlock.ForeColor = ThemeColors.Text;

                    ToolStripMenuItem btnUnblock = new ToolStripMenuItem(string.Format(Properties.Resources.CtxUnblock, countryName));
                    btnUnblock.Click += (s, ev) => SetCountrySelection(false, countryName);
                    btnUnblock.BackColor = ThemeColors.SurfaceLight;
                    btnUnblock.ForeColor = ThemeColors.Text;

                    mnu.Items.Add(btnBlock);
                    mnu.Items.Add(btnUnblock);
                    mnu.Show(dataGridView1, new Point(e.X, e.Y));
                }
            }
        }

        private void SetCountrySelection(bool block, string targetCountry)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["colCountry"].Value?.ToString() == targetCountry)
                {
                    row.Cells["colBlock"].Value = block;
                    UpdateRowColor(row);
                }
            }
        }

        private async void SetupLanguageDropdown()
        {
            comboLimbaj.DrawMode = DrawMode.OwnerDrawFixed;
            comboLimbaj.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLimbaj.DrawItem += comboLimbaj_DrawItem;
            comboLimbaj.ItemHeight = 24; 

            comboLimbaj.Items.Clear();
            var languages = new List<LanguageOption>
            {
                new LanguageOption { Name = "English", Code = "" },
                new LanguageOption { Name = "Română", Code = "ro" },
                new LanguageOption { Name = "Русский", Code = "ru" },
                new LanguageOption { Name = "Español", Code = "es" },
                new LanguageOption { Name = "Português", Code = "pt" },
                new LanguageOption { Name = "Français", Code = "fr" }
            };

            // Add items synchronously so they are available immediately
            foreach (var lang in languages)
            {
                comboLimbaj.Items.Add(lang);
            }

            // Load flags asynchronously in the background
            foreach (LanguageOption lang in comboLimbaj.Items)
            {
                string countryCode = lang.Code == "" ? "gb" : lang.Code;
                lang.Flag = await _flagService.GetFlagImageAsync(countryCode);
            }
            
            // Trigger redraw to show flags once loaded
            comboLimbaj.Invalidate();
        }

        private void comboLimbaj_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ComboBox combo = sender as ComboBox;
            LanguageOption item = combo.Items[e.Index] as LanguageOption;

            // Draw Background
            e.DrawBackground();

            // Draw Flag
            int iconSize = 20; // 24x24 mostly, fit in 20
            Rectangle flagRect = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + (e.Bounds.Height - iconSize) / 2, iconSize, iconSize);
            
            if (item.Flag != null)
            {
                e.Graphics.DrawImage(item.Flag, flagRect);
            }

            // Draw Text
            string text = item.Name;
            Brush textBrush = new SolidBrush(e.ForeColor);
            // Center text vertically
            float y = e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2;
            e.Graphics.DrawString(text, e.Font, textBrush, e.Bounds.Left + iconSize + 8, y);

             // Draw Focus Rectangle
            e.DrawFocusRectangle();
        }

        private void ApplySavedLanguage()
        {
            string savedLang = Properties.Settings.Default.LimbaPreferata;
            foreach (LanguageOption item in comboLimbaj.Items)
            {
                if (item.Code == savedLang) { comboLimbaj.SelectedItem = item; return; }
            }
            comboLimbaj.SelectedIndex = 0;
        }

        private void ChangeLanguage(string langCode)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(langCode);
            UpdateUI();
        }

        private void comboLimbaj_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLimbaj.SelectedItem is LanguageOption optiune)
            {
                ChangeLanguage(optiune.Code);
                Properties.Settings.Default.LimbaPreferata = optiune.Code;
                Properties.Settings.Default.Save();
            }
        }

        private void UpdateUI()
        {
            btnLoad.Text = Properties.Resources.BtnLoad;
            btnApply.Text = Properties.Resources.btnApply;
            btnReset.Text = Properties.Resources.BtnReset;
            if (btnRefreshPing != null) btnRefreshPing.Text = Properties.Resources.BtnRefreshPing;

            if (dataGridView1.Columns.Contains("colBlock"))
            {
                dataGridView1.Columns["colBlock"].HeaderText = Properties.Resources.ColBlock;
                dataGridView1.Columns["colCountry"].HeaderText = Properties.Resources.ColFlag;
                dataGridView1.Columns["colCity"].HeaderText = Properties.Resources.ColCity;
                dataGridView1.Columns["colPing"].HeaderText = Properties.Resources.ColPing;
                if (dataGridView1.Columns.Contains("colIPs")) dataGridView1.Columns["colIPs"].HeaderText = "IPs";
            }
            if (!dataGridView1.Columns.Contains("colBlock"))
            {
                var col = new DataGridViewCheckBoxColumn();
                col.Name = "colBlock";
                col.HeaderText = Properties.Resources.ColBlock;
                col.Width = 50;
                dataGridView1.Columns.Add(col);
            }
            if (!dataGridView1.Columns.Contains("colCountry"))
            {
                var col = new DataGridViewImageColumn();
                col.Name = "colCountry";
                col.HeaderText = Properties.Resources.ColFlag;
                col.Width = 50;
                dataGridView1.Columns.Add(col);
            }
            if (!dataGridView1.Columns.Contains("colCity"))
            {
                dataGridView1.Columns.Add("colCity", Properties.Resources.ColCity);
            }
            if (!dataGridView1.Columns.Contains("colPing"))
            {
                dataGridView1.Columns.Add("colPing", Properties.Resources.ColPing);
            }
            if (!dataGridView1.Columns.Contains("colIPs"))
            {
                dataGridView1.Columns.Add("colIPs", "IPs");
                dataGridView1.Columns["colIPs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns.Contains("colBlock"))
            {
                dataGridView1.Columns["colBlock"].DisplayIndex = 0;
                dataGridView1.Columns["colBlock"].Width = 90;
                dataGridView1.Columns["colBlock"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns.Contains("colCountry"))
            {
                dataGridView1.Columns["colCountry"].DisplayIndex = 1;
                dataGridView1.Columns["colCountry"].Width = 100; // Increased +10
                dataGridView1.Columns["colCountry"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft; // Aligned Left
            }

            if (dataGridView1.Columns.Contains("colCity"))
            {
                dataGridView1.Columns["colCity"].DisplayIndex = 2;
                dataGridView1.Columns["colCity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["colCity"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft; // Aligned Left
            }
                
            if (dataGridView1.Columns.Contains("colIPs"))
            {
                dataGridView1.Columns["colIPs"].DisplayIndex = 3;
                dataGridView1.Columns["colIPs"].Width = 70;
                dataGridView1.Columns["colIPs"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns.Contains("colPing"))
            {
                dataGridView1.Columns["colPing"].DisplayIndex = 4;
                dataGridView1.Columns["colPing"].Width = 100;
                dataGridView1.Columns["colPing"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void UpdateIpColumn()
        {
            try
            {
                var blockedCounts = _firewallService.GetBlockedIpCounts();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    string city = row.Cells["colCity"].Value?.ToString();
                    if (string.IsNullOrEmpty(city)) continue;

                    string normalizedCity = city.Replace(" ", "");
                    int blockedIps = 0;
                    if (blockedCounts.Keys.Any(k => k.Equals(normalizedCity, StringComparison.OrdinalIgnoreCase)))
                    {
                        blockedIps = blockedCounts.FirstOrDefault(k => k.Key.Equals(normalizedCity, StringComparison.OrdinalIgnoreCase)).Value;
                    }

                    int totalIps = 1;
                    if (SteamApiService.CityRanges != null && SteamApiService.CityRanges.ContainsKey(city))
                        totalIps = SteamApiService.CityRanges[city].Count;

                    row.Cells["colIPs"].Value = $"{blockedIps}/{totalIps}";
                }
            }
            catch { }
        }
    }
}
