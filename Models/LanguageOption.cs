using System.Drawing;

namespace CS2ServerPicker.Models
{
    public class LanguageOption
    {
        public string Name { get; set; }
        public string Code { get; set; } 
        public Image Flag { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}