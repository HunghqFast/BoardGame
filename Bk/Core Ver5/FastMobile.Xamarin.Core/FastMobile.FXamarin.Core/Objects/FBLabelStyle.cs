using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBLabelStyle
    {
        public int MaxLine { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }
        public double[] Width { get; set; }
        public double[] Margin { get; set; }
        public double[] Padding { get; set; }
        public int FontSize { get; set; }
        public FontAttributes FontAttributes { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public FSizeType WidthType { get; set; }
    }
}