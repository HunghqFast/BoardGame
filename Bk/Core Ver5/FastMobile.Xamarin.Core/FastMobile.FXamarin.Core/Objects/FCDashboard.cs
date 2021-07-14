using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FCDashboard : FBDashboard
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }
        public FSizeType SizeType { get; set; }
        public FChartType Type { get; set; }
        public bool SeriesIsVisibleOnLegend { get; set; }
        public bool IsVisibleMultipleSeries { get; set; }
        public bool RefreshOnChanging { get; set; }
        public FXAxis XAxis { get; set; }
        public FYAxis YAxis { get; set; }
        public FChartLegend Legend { get; set; }
        public List<FChartXySerie> XySeries { get; set; }
        public FChartPieSerie PieSeries { get; set; }
        public FChartTriangularSerie TriSeries { get; set; }
        public List<string> Colors { get; set; }
    }

    public class BFooter2
    {
        public BFooter2()
        {
            Rows = new List<BFooterRow>();
        }

        public bool IsVisible { get; set; }
        public bool ShowLine { get; set; }
        public List<BFooterRow> Rows { get; set; }
    }

    public class BFooterRow
    {
        public BFooterRow()
        {
            Views = new List<BTextView>();
        }

        public double Height { get; set; }
        public double[] Padding { get; set; }
        public List<BTextView> Views { get; set; }
    }

    public class BTextView
    {
        public string Width { get; set; }
        public int Table { get; set; }
        public int RefreshTable { get; set; }
        public int Row { get; set; }
        public int RefreshRow { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public string TextColor { get; set; }
        public int MaxLine { get; set; }
        public int FontSize { get; set; }
        public LayoutAlignment Alignment { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public FontAttributes FontAttributes { get; set; }
    }
}