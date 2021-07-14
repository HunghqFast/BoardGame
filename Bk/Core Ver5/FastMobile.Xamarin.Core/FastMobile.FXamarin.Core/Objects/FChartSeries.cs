namespace FastMobile.FXamarin.Core
{
    public class FChartSeries
    {
        public string Header { get; set; }
        public string Key { get; set; }
        public string Format { get; set; }
        public string XTooltipDescription { get; set; }
        public string YTooltipDescription { get; set; }
        public FBLabelStyle XDescriptionStyle { get; set; }
        public FBLabelStyle YDescriptionStyle { get; set; }
        public FBLabelStyle XValueStyle { get; set; }
        public FBLabelStyle YValueStyle { get; set; }
        public string SelectedColor { get; set; }
        public bool EnableColor { get; set; }
        public bool EnableAnimation { get; set; }
        public bool IsVisible { get; set; }
        public bool OnlyNumberAtTooltip { get; set; }
        public bool EnableTooltipTemplate { get; set; }
        public double StrokeWidth { get; set; }
        public double AnimationDuration { get; set; }
        public FChartDataMaker DataMaker { get; set; }
    }
}