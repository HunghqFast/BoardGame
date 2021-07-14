using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FXyChart : FChart
    {
        public static readonly BindableProperty LegendIndexProperty = BindableProperty.Create("LegendIndex", typeof(int), typeof(FXyChart), -1);

        public int LegendIndex
        {
            get => (int)GetValue(LegendIndexProperty);
            set => SetValue(LegendIndexProperty, value);
        }

        public const string FormatNum = "### ### ### ### ##0";

        protected CategoryAxis XAxis { get; private set; }
        protected NumericalAxis YAxis { get; private set; }
        protected ObservableCollection<FItemXyChart> DataChart { get; }
        private readonly List<double> maxValues;

        public FXyChart() : base()
        {
            Chart.LegendItemClicked += ClickedLegend;
            Chart.Type = FChartType.Bar;
            DataChart = new ObservableCollection<FItemXyChart>();
            maxValues = new List<double>();
            XAxis = new CategoryAxis();
            YAxis = new FNumericalAxis();
        }

        protected override void Init()
        {
            YAxis.RangePadding = NumericalPadding.Normal;
            YAxis.EdgeLabelsVisibilityMode = EdgeLabelsVisibilityMode.AlwaysVisible;
            YAxis.LabelStyle.WrappedLabelAlignment = ChartAxisLabelAlignment.Start;
            YAxis.Title.FontFamily = FSetting.FontText;
            YAxis.LabelStyle.FontFamily = FSetting.FontText;
            YAxis.LabelStyle.FontSize = FSetting.FontSizeLabelHint - 3;
            YAxis.AxisLineStyle.StrokeWidth = 1;

            XAxis.LabelPlacement = LabelPlacement.OnTicks;
            XAxis.MaximumLabels = 5;
            XAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Center;
            XAxis.LabelsIntersectAction = AxisLabelsIntersectAction.MultipleRows;
            XAxis.Title.FontFamily = FSetting.FontText;
            XAxis.LabelStyle.FontFamily = FSetting.FontText;
            XAxis.LabelStyle.FontSize = FSetting.FontSizeLabelHint - 3;
            XAxis.AxisLineStyle.StrokeWidth = 1;
            base.Init();
        }

        protected override void InitModel()
        {
            base.InitModel();
            InitXAxis(XAxis, Model.XAxis);
            InitYAxis(YAxis, Model.YAxis);
            Chart.PrimaryAxis = XAxis;
            Chart.SecondaryAxis = YAxis;
        }

        protected virtual void InitDataSource(bool isLoading, bool checkNodata = true)
        {
            DataChart.Clear();
            maxValues.Clear();
            Model.XySeries.ForEach((x) => maxValues.Add(0));
            DataResult.Rows.ForEach<DataRow>((x) => AddRow(x));
            if (checkNodata)
                HasData(isLoading);
        }

        protected async override Task RefreshSeries(bool isLoading)
        {
            await base.RefreshSeries(isLoading);
            try
            {
                if (isLoading)
                    ListIsVisibleSeries.Clear();
                for (int i = 1; i <= Model.XySeries.Count; i++)
                {
                    if (isLoading)
                        ListIsVisibleSeries.Add(Model.XySeries[i - 1].IsVisible);
                    InitSeries(Model.XySeries[i - 1].Type, i, isLoading);
                }
            }
            catch { Error(); }
            finally
            {
                HasData(isLoading);
            }
        }

        protected override void InitSerie(ChartSeries serie, FChartSeries model, object itemSource, bool isLoading, int index = 1, bool isSector = false)
        {
            base.InitSerie(serie, model, itemSource, isLoading, index, isSector);
            var series = serie as XyDataSeries;
            series.EnableDataPointSelection = false;
            series.Color = (!model.EnableColor) ? Color.FromHex((model as FChartXySerie).Color) : serie.Color;
            series.YBindingPath = $"Value{index}";

            if (!Model.IsVisibleMultipleSeries)
                serie.IsVisible = index == LegendIndex + 1;
            else if (!isLoading)
                serie.IsVisible = ListIsVisibleSeries[index - 1];
            if ((model as FChartXySerie).YAxis != null)
            {
                var numAxis = new FNumericalAxis();
                numAxis.RangePadding = NumericalPadding.Normal;
                numAxis.EdgeLabelsVisibilityMode = EdgeLabelsVisibilityMode.Visible;
                numAxis.Title.FontFamily = FSetting.FontText;
                numAxis.LabelStyle.WrappedLabelAlignment = ChartAxisLabelAlignment.Start;
                numAxis.LabelStyle.FontFamily = FSetting.FontText;
                numAxis.LabelStyle.FontSize = FSetting.FontSizeLabelHint - 3;
                InitYAxis(numAxis, (model as FChartXySerie).YAxis);
                series.YAxis = numAxis;
            }
            if ((model as FChartXySerie).XAxis != null)
            {
                var catAxis = new CategoryAxis();
                catAxis.LabelPlacement = LabelPlacement.OnTicks;
                catAxis.MaximumLabels = 5;
                catAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Center;
                catAxis.LabelsIntersectAction = AxisLabelsIntersectAction.MultipleRows;
                catAxis.Title.FontFamily = FSetting.FontText;
                catAxis.LabelStyle.FontFamily = FSetting.FontText;
                catAxis.LabelStyle.FontSize = FSetting.FontSizeLabelHint - 3;
                InitXAxis(catAxis, (model as FChartXySerie).XAxis);
                series.XAxis = catAxis;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == LegendIndexProperty.PropertyName)
            {
                UpdateAxisTitleText();
                return;
            }

            if (propertyName == ShowNothingProperty.PropertyName)
            {
                YAxis.Interval = ShowNothing ? 1 : Double.NaN;
                return;
            }
        }

        protected override void ReplaceLeftRight(string left, string right, bool isLoading)
        {
            base.ReplaceLeftRight(left, right, isLoading);
            XAxis.Title.Text = GetTitleByAxis(Model.XAxis, LegendIndex, left, right);
            YAxis.Title.Text = GetTitleByAxis(Model.YAxis, LegendIndex, left, right);
        }

        private void AddRow(DataRow row)
        {
            FItemXyChart temp = new FItemXyChart();
            Model.XySeries.ForIndex((x, i) => temp = AddObject(x, row, temp, i));
            DataChart.Add(temp);
        }

        private FItemXyChart AddObject(FChartXySerie serie, DataRow row, FItemXyChart item, int k)
        {
            item.Name = row[Model.XAxis.Key].ToString();
            string strk = (k + 1).ToString(), format = serie.Format.Contains("@") ? serie.Format.Replace("@", "").GetCache() : serie.Format;
            var value = Convert.ToDouble(row[serie.Key]);
            FItemXyChart.Type.SetPropValue(item, $"Value{strk}", value);
            FItemXyChart.Type.SetPropValue(item, $"StringValue{strk}", (serie.OnlyNumberAtTooltip || serie.EnableTooltipTemplate) ? value.ToString(format).Trim() : $"{item.Name.Trim()}: {value.ToString(format).Trim()}");
            if (maxValues[k] < value)
                maxValues[k] = value;
            return item;
        }

        private string GetTitleByAxis(FAxis model, int index, string left = "", string right = "")
        {
            if (index < 0)
                return string.Empty;
            if (model.Titles != null && model.Titles.Count > 0)
                return model.Titles[index]?.Replace(LeftCharacter, left).Replace(RightCharacter, right);
            return model.Title.Text?.Replace(LeftCharacter, left).Replace(RightCharacter, right);
        }

        private void UpdateAxisTitleText()
        {
            if (Model == null)
                return;
            RefreshAxisTitle(Model.XAxis, Chart.PrimaryAxis);
            RefreshAxisTitle(Model.YAxis, Chart.SecondaryAxis);
        }

        private void RefreshAxisTitle(FAxis model, ChartAxis axis)
        {
            if (model != null && string.IsNullOrEmpty(model.Title.Text) && !Model.IsVisibleMultipleSeries && model.Titles.Count > 0 && LegendIndex < model.Titles.Count)
                axis.Title.Text = string.IsNullOrEmpty(model.Titles[LegendIndex]) ? model.Title.Text ?? "" : model.Titles[LegendIndex];
        }

        private void ClickedLegend(object sender, ChartLegendItemClickedEventArgs e)
        {
            LegendIndex = e.LegendItem.Index;
            if (Model.IsVisibleMultipleSeries)
            {
                ShowNothing = IsNoData();
                if (ListIsVisibleSeries.Count > e.LegendItem.Index)
                    ListIsVisibleSeries[e.LegendItem.Index] = e.LegendItem.Series.IsVisible;
                return;
            }
            Chart.Series.ForEach((x) => x.IsVisible = x.Id == e.LegendItem.Series.Id);
            ShowNothing = e.LegendItem.Index <= maxValues.Count ? maxValues[e.LegendItem.Index] <= 0 : ShowNothing;
        }

        private void InitSeries(FXyChartType type, int index, bool isLoading, ChartSeries series = null)
        {
            switch (type)
            {
                case FXyChartType.Histogram:
                    series = new HistogramSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.Line:
                    series = new LineSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.Scatters:
                    series = new ScatterSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    ((ScatterSeries)series).ScatterWidth = 25;
                    ((ScatterSeries)series).ScatterHeight = 25;
                    AddSerie(series);
                    break;

                case FXyChartType.Spline:
                    series = new SplineSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    ((SplineSeries)series).SplineType = SplineType.Monotonic;
                    AddSerie(series);
                    break;

                case FXyChartType.StackingArea:
                    series = new StackingAreaSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.StepArea:
                    series = new StepAreaSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.StepLine:
                    series = new StepLineSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.FastLine:
                    series = new FastLineSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    ((FastLineSeries)series).StrokeDashArray = new double[2] { 3, 3 };
                    AddSerie(series);
                    break;

                case FXyChartType.StackingColumn:
                    series = new StackingColumnSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.StackingColumn100:
                    series = new StackingColumn100Series();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.BarHorizontal:
                    series = new BarSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.StackingBarHorizontal:
                    series = new StackingBarSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                case FXyChartType.StackingBar100Horizontal:
                    series = new StackingBar100Series();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    AddSerie(series);
                    break;

                default:
                    Chart.SideBySideSeriesPlacement = true;
                    series = new ColumnSeries();
                    InitSerie(series, Model.XySeries[index - 1], DataChart, isLoading, index);
                    ((ColumnSeries)series).Width = (DataResult.Rows.Count < 4) ? 0.5 : ((ColumnSeries)series).Width;
                    AddSerie(series);
                    break;
            }
        }

        private void HasData(bool isLoading)
        {
            InitLegendIndex(isLoading);

            if (DataResult.Rows.Count == 0)
            {
                base.ShowNothing = true;
                return;
            }
            if (Model.IsVisibleMultipleSeries)
            {
                base.ShowNothing = IsNoData();
                return;
            }
            base.ShowNothing = LegendIndex < maxValues.Count ? maxValues[LegendIndex] <= 0 : base.ShowNothing;
        }

        private bool IsNoData()
        {
            return maxValues.Find((x) => x > 0) == default;
        }

        private void InitLegendIndex(bool isLoading)
        {
            if (isLoading)
                LegendIndex = Model.XySeries.IndexOf(Model.XySeries.Find((x) => x.IsVisible));
        }
    }
}