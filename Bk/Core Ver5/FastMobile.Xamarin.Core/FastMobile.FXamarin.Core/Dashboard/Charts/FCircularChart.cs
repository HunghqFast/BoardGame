using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FCircularChart : FAccumulationChart
    {
        public FCircularChart() : base()
        {
            Chart.Type = FChartType.Pie;
        }

        protected override void InitSeries(bool isLoading, ChartSeries series = null)
        {
            switch (Model.PieSeries.Type)
            {
                case FPieChartType.Doughnut:
                    series = new DoughnutSeries();
                    InitSerie(series, Model.PieSeries, DataChart, isLoading);
                    ((DoughnutSeries)series).CenterView = new Label() { Text = Model.PieSeries.CenterText };
                    InitLabelStyle(((DoughnutSeries)series).CenterView as Label, Model.PieSeries.CenterTextStyle);
                    AddSerie(series);
                    break;

                case FPieChartType.Sector:
                    series = new PieSeries();
                    InitSerie(series, Model.PieSeries, DataChart, isLoading, 1, true);
                    AddSerie(series);
                    break;

                case FPieChartType.SectorOfDoughnut:
                    series = new DoughnutSeries();
                    InitSerie(series, Model.PieSeries, DataChart, isLoading, 1, true);
                    ((DoughnutSeries)series).CenterView = new Label() { Text = Model.PieSeries.CenterText };
                    InitLabelStyle(((DoughnutSeries)series).CenterView as Label, Model.PieSeries.CenterTextStyle);
                    AddSerie(series);
                    break;

                case FPieChartType.StackedDoughnut:
                    series = new DoughnutSeries();
                    InitSerie(series, Model.PieSeries, DataChart, isLoading);
                    ((DoughnutSeries)series).CenterView = new Label() { Text = Model.PieSeries.CenterText };
                    InitLabelStyle(((DoughnutSeries)series).CenterView as Label, Model.PieSeries.CenterTextStyle);
                    ((DoughnutSeries)series).IsStackedDoughnut = true;
                    ((DoughnutSeries)series).CapStyle = DoughnutCapStyle.BothCurve;
                    ((DoughnutSeries)series).Spacing = 0.2;
                    ((DoughnutSeries)series).MaximumValue = ItemSourceValueMax;
                    AddSerie(series);
                    break;

                default:
                    series = new PieSeries();
                    InitSerie(series, Model.PieSeries, DataChart, isLoading);
                    AddSerie(series);
                    break;
            }
        }

        protected override void InitSerie(ChartSeries serie, FChartSeries model, object itemSource, bool isLoading, int index = 1, bool isSector = false)
        {
            var series = serie as CircularSeries;
            series.YBindingPath = "Value1";
            series.EnableSmartLabels = true;
            series.ShowMarkerAtLineEnd = true;
            series.StartAngle = isSector ? 180 : (model as FChartPieSerie).StartAngle;
            series.EndAngle = isSector ? 360 : (model as FChartPieSerie).EndAngle;
            series.ConnectorLineType = ConnectorLineType.StraightLine;
            series.DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended;
            series.StrokeColor = Color.FromHex((model as FChartPieSerie).StrokeColor);
            base.InitSerie(serie, model, itemSource, isLoading, index, isSector);
        }
    }
}