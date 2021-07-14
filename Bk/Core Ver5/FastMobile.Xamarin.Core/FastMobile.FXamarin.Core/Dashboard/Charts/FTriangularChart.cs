using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FTriangularChart : FAccumulationChart
    {
        public FTriangularChart() : base()
        {
            Chart.Type = FChartType.Tri;
        }

        protected override void InitSeries(bool isLoading, ChartSeries series = null)
        {
            switch (Model.TriSeries.Type)
            {
                case FTriangularChartType.Funnel:
                    series = new FunnelSeries();
                    InitSerie(series, Model.TriSeries, DataChart, isLoading);
                    ((FunnelSeries)series).MinWidth = 20;
                    AddSerie(series);
                    break;

                default:
                    series = new PyramidSeries();
                    InitSerie(series, Model.TriSeries, DataChart, isLoading);
                    ((PyramidSeries)series).PyramidMode = ChartPyramidMode.Surface;
                    AddSerie(series);
                    break;
            }
        }

        protected override void InitSerie(ChartSeries serie, FChartSeries model, object itemSource, bool isLoading, int index = 1, bool isSector = false)
        {
            ((TriangularSeries)serie).YBindingPath = "Value1";
            ((TriangularSeries)serie).StrokeColor = Color.FromHex((model as FChartTriangularSerie).StrokeColor);
            base.InitSerie(serie, model, itemSource, isLoading, index, isSector);
        }
    }
}