namespace FastMobile.FXamarin.Core
{
    public class FChartXySerie : FChartSeries
    {
        public bool IsTransposed { get; set; }
        public string Color { get; set; }
        public string GroupingLabel { get; set; }
        public FXyChartType Type { get; set; }
        public FYAxis YAxis { get; set; }
        public FXAxis XAxis { get; set; }
    }
}