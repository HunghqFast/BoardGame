namespace FastMobile.FXamarin.Core
{
    public class FChartPieSerie : FChartSeries
    {
        public int StartAngle { get; set; }
        public int EndAngle { get; set; }
        public string StrokeColor { get; set; }
        public string CenterText { get; set; }
        public FBLabelStyle CenterTextStyle { get; set; }
        public FPieChartType Type { get; set; }
    }
}