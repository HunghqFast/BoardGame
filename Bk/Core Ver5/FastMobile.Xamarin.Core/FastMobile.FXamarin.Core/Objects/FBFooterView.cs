using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBFooterView
    {
        public int Table { get; set; }
        public int Row { get; set; }
        public int RefreshTable { get; set; }
        public int RefreshRow { get; set; }
        public string Description { get; set; }
        public string DescriptionKey { get; set; }
        public string Value { get; set; }
        public string ValueKey { get; set; }
        public FBLabelStyle DescriptionStyle { get; set; }
        public FBLabelStyle ValueStyle { get; set; }
        public StackOrientation Orientation { get; set; }
    }
}