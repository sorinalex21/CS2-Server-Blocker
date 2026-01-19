using System.Drawing;

namespace CS2ServerPicker
{
    public static class ThemeColors
    {
        public static bool IsDarkMode { get; set; } = true;

        public static Color Background => IsDarkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(240, 240, 245);
        public static Color Surface => IsDarkMode ? Color.FromArgb(45, 45, 48) : Color.White;
        public static Color SurfaceLight => IsDarkMode ? Color.FromArgb(62, 62, 66) : Color.FromArgb(230, 230, 235);
        
        public static Color Accent => IsDarkMode ? Color.FromArgb(0, 122, 204) : Color.FromArgb(100, 180, 255); // Lighter blue for Light Mode
        public static Color AccentDark => IsDarkMode ? Color.FromArgb(0, 99, 177) : Color.FromArgb(80, 160, 235);
        public static Color AccentLight => IsDarkMode ? Color.FromArgb(28, 151, 234) : Color.FromArgb(130, 200, 255);

        public static Color Text => IsDarkMode ? Color.FromArgb(241, 241, 241) : Color.FromArgb(30, 30, 30);
        public static Color TextDim => IsDarkMode ? Color.FromArgb(160, 160, 160) : Color.FromArgb(100, 100, 100);

        public static Color Danger => Color.FromArgb(232, 17, 35);
        public static Color Success => Color.FromArgb(22, 198, 12);
        public static Color Warning => Color.FromArgb(255, 204, 0);

        // Grid Specific
        public static Color GridSelectionBack => IsDarkMode ? Accent : Color.FromArgb(200, 230, 255);
        public static Color GridSelectionFore => IsDarkMode ? Text : Color.Black;
        public static Color GridAltRow => IsDarkMode ? Color.FromArgb(50, 50, 53) : Color.FromArgb(248, 248, 250);

        // Logic for Blocked/Selected rows
        public static Color GridRowBloch => IsDarkMode ? Color.FromArgb(70, 20, 20) : Color.FromArgb(255, 200, 200); // Lighter red for Light Mode
        public static Color GridRowBlochSel => IsDarkMode ? Color.FromArgb(90, 30, 30) : Color.FromArgb(255, 180, 180);

        public static Color GridRowDiff => IsDarkMode ? Color.FromArgb(70, 70, 50) : Color.FromArgb(255, 255, 200); // Lighter yellow for Light Mode
        public static Color GridRowDiffSel => IsDarkMode ? Color.FromArgb(90, 90, 70) : Color.FromArgb(255, 240, 150);
    }
}

