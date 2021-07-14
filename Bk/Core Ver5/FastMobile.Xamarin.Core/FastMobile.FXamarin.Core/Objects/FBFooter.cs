using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBFooter
    {
        public bool IsVisible { get; set; }
        public bool ShowLine { get; set; }
        public StackOrientation Orientation { get; set; }
        public List<FBFooterView> ListFooter { get; set; }
    }
}