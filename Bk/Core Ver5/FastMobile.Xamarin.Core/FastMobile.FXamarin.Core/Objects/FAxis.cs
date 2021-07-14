using Syncfusion.SfChart.XForms;
using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FAxis
    {
        public FTitleStyle Title { get; set; }
        public List<string> Titles { get; set; }
        public FDBLabelStyle LabelStyle { get; set; }
        public int CrossesAt { get; set; }
        public bool IsVisible { get; set; }
        public bool ShowMajorGridLines { get; set; }
        public bool IsInversed { get; set; }
        public bool OpposedPosition { get; set; }
        public ChartAxisLabelAlignment LabelAlignment { get; set; }
        public AxisElementPosition TickPosition { get; set; }
    }
}