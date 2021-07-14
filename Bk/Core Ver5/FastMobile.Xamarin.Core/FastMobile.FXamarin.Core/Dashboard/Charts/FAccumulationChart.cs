using Syncfusion.SfChart.XForms;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FAccumulationChart : FChart
    {
        protected double SumPieSeries, ItemSourceValueMax;

        protected ObservableCollection<FItemAccumulationChart> DataChart { get; }

        public FAccumulationChart() : base()
        {
            DataChart = new ObservableCollection<FItemAccumulationChart>();
            Chart.SetBinding(SfChart.IsVisibleProperty, ShowNothingProperty.PropertyName, converter: FInvertBool.Instance);
        }

        public virtual void InitDataSource(FChartSeries model, bool checkNodata = true)
        {
            DataChart.Clear();
            SumPieSeries = ItemSourceValueMax = 0;
            DataResult.Rows.ForEach<DataRow>((x) => SumPieSeries += Convert.ToDouble(x[model.Key]));
            DataResult.Rows.ForEach<DataRow>((x) => AddData(x, model));
            UpdateHeight(DataChart?.Count == 0 || ItemSourceValueMax <= 0);
            if (checkNodata) HasData();
        }

        private void AddData(DataRow row, FChartSeries model)
        {
            var temp = new FItemAccumulationChart();
            temp.Name = row[Model.XAxis.Key].ToString();
            temp.Value1 = Convert.ToDouble(row[model.Key]);
            temp.StringValue1 = model.OnlyNumberAtTooltip || model.EnableTooltipTemplate ? $"{(temp.Value1 / SumPieSeries * 100).ToString("##0.00").Trim()}%" : $"{temp.Name}: " + $"{(temp.Value1 / SumPieSeries * 100).ToString("##0.00").Trim()}%";
            temp.ColorMaker1 = Model.Colors.Count == 0 ? Color.FromHex("#f5f3eb") : Color.FromHex(Model.Colors[0]);
            ItemSourceValueMax = temp.Value1 > ItemSourceValueMax ? temp.Value1 : ItemSourceValueMax;
            DataChart.Add(temp);
        }

        protected async override Task RefreshSeries(bool isLoading)
        {
            await base.RefreshSeries(isLoading);
            await CreateAllSeries(isLoading);
        }

        protected virtual Task CreateAllSeries(bool isLoading)
        {
            if (HasData()) InitSeries(isLoading);
            return Task.CompletedTask;
        }

        protected virtual void InitSeries(bool isLoading, ChartSeries series = null)
        {
        }

        private bool HasData()
        {
            SumPieSeries = 0;
            if (DataChart.Count == 0 || ItemSourceValueMax <= 0)
            {
                ShowNothing = true;
                return false;
            }
            ShowNothing = false;
            return true;
        }
    }
}