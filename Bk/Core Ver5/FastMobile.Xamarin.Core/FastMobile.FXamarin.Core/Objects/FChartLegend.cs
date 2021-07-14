using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FChartLegend
    {
        public double[] ItemMargin { get; set; }
        public double Width { get; set; }
        public string LabelColor { get; set; }
        public bool IsVisible { get; set; }
        public bool ShowIcon { get; set; }
        public LegendPlacement DockPosition { get; set; }
        public ChartOrientation Orientation { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public ChartLegendOverflowMode OverflowMode { get; set; }
        public FTitleStyle Title { get; set; }
        public FChartLegendBackground Background { get; set; }
    }
}