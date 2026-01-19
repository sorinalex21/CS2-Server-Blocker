using System;
using System.Drawing;
using System.Windows.Forms;

namespace CS2ServerPicker
{
    public static class UIExtensions
    {
        public static void StyleDarkWindow(this Form form)
        {
            form.BackColor = ThemeColors.Background;
            form.ForeColor = ThemeColors.Text;
            form.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
        }

        public static void StyleButton(this Button btn, Color? backColor = null) 
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor ?? ThemeColors.SurfaceLight;
            btn.ForeColor = ThemeColors.Text;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point);
            
            btn.FlatAppearance.MouseOverBackColor = ThemeColors.Accent;
            btn.FlatAppearance.MouseDownBackColor = ThemeColors.AccentDark;
        }

        public static void StylePrimaryButton(this Button btn)
        {
            btn.StyleButton(ThemeColors.Accent);
            btn.FlatAppearance.MouseOverBackColor = ThemeColors.AccentLight;
            btn.FlatAppearance.MouseDownBackColor = ThemeColors.AccentDark;
        }

        public static void StyleDangerButton(this Button btn)
        {
            btn.StyleButton(ThemeColors.Danger);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 70, 70); 
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 10, 20);  
        }

        public static void StyleSuccessButton(this Button btn)
        {
            btn.StyleButton(ThemeColors.Success);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 230, 50); // Lighter Green
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 160, 5);  // Darker Green
        }

        public static void StyleGridView(this DataGridView dgv)
        {
            dgv.BackgroundColor = ThemeColors.Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = ThemeColors.SurfaceLight;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeColors.SurfaceLight;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeColors.Text;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = ThemeColors.SurfaceLight;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Centered Headers
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.BackColor = ThemeColors.Surface;
            dgv.DefaultCellStyle.ForeColor = ThemeColors.Text;
            dgv.DefaultCellStyle.SelectionBackColor = ThemeColors.GridSelectionBack; 
            dgv.DefaultCellStyle.SelectionForeColor = ThemeColors.GridSelectionFore;
            dgv.DefaultCellStyle.Padding = new Padding(5);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = ThemeColors.GridAltRow;
        }

        public static void StyleTextBox(this TextBox txt)
        {
            txt.BackColor = ThemeColors.Surface;
            txt.ForeColor = ThemeColors.Text;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 10F);
        }

        public static void StyleComboBox(this ComboBox box)
        {
            box.BackColor = ThemeColors.Surface;
            box.ForeColor = ThemeColors.Text;
            box.FlatStyle = FlatStyle.Flat;
            box.Font = new Font("Segoe UI", 10F);
        }

        // --- DWM Dark Mode ---
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static void UseImmersiveDarkMode(this Form form, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                int attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                DwmSetWindowAttribute(form.Handle, attribute, ref useImmersiveDarkMode, sizeof(int));
            }
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && (build == -1 || Environment.OSVersion.Version.Build >= build);
        }
    }
}
