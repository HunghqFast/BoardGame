using FastMobile.FXamarin.Core;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageDashboards : FPage
    {
        private readonly ObservableCollection<OverViewModel> Models;
        private readonly StackLayout S;

        public CPageDashboards() : base(true, true)
        {
            Models = new ObservableCollection<OverViewModel>();
            S = new StackLayout();
            Base();
        }

        public async override void Init()
        {
            base.Init();
            if (!HasNetwork)
                return;
            Content = S;
            await SetBusy(true);
            await Load();
            await SetBusy(false);
        }

        protected override async void OnRefreshing(object sender, EventArgs e)
        {
            await SetRefresh(true);
            if (!FUtility.HasNetwork)
            {
                await SetRefresh(false);
                return;
            }

            base.OnRefreshing(sender, e);
            if (Models.Count == 0)
                await Load();
            else
                await Refesh();
            await SetRefresh(false);
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        private async void Base()
        {
            await SetBusy(true);
            S.Spacing = 10;
        }

        private async Task Load()
        {
            await InitModels();
            await RenderContent();
        }

        private async Task InitModels()
        {
            var message = await GetArea();
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                ShowNothing = true;
                return;
            }

            try
            {
                var ds = message.ToDataSet();
                if (ds == null || ds.Tables.Count == 0)
                    return;
                Models.Clear();
                ds.Tables[0].Rows.ForEach<DataRow>(Add);
                UpdateStyle();
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async Task RenderContent()
        {
            if (Models.Count <= 0)
            {
                ShowNothing = true;
                return;
            }
            ShowNothing = false;
            await SetBusy(false);
            await FServices.ForAllAsync(Models, LoadElement);
        }

        private async Task Refesh()
        {
            await FServices.ForAllAsync(Models, x => x.View.OnChanged());
        }

        private Task LoadElement(OverViewModel model)
        {
            return model.View?.OnLoaded();
        }

        private void Add(DataRow row)
        {
            var model = new OverViewModel
            {
                Controller = row["controller"].ToString().Trim(),
                ChartType = (row["xtype"].ToString().Trim() == "C01") ? FChartType.Bar : (row["xtype"].ToString().Trim() == "C02") ? FChartType.Pie : FChartType.Tri,
                ViewType = ViewType(row),
            };
            model.View = GetView(model.ViewType, model.ChartType, model.Controller);
            Models.Add(model);
        }

        private FDBase GetView(FOverviewType overviewType, FChartType chartType, string controller)
        {
            return overviewType switch
            {
                FOverviewType.Chart => chartType switch
                {
                    FChartType.Bar => new CXyChart(controller) { LoadHasBusy = true },
                    FChartType.Pie => new CCircularChart(controller) { LoadHasBusy = true },
                    FChartType.Tri => new CTriangularChart(controller) { LoadHasBusy = true },
                    _ => null
                },
                FOverviewType.ReportGrid => new CGrid(controller) { LoadHasBusy = true },
                _ => null
            };
        }

        private FOverviewType ViewType(DataRow row)
        {
            if (row.Table.Columns.Contains("type"))
                return (row["type"].ToString().Trim() == "G") ? FOverviewType.ReportGrid : FOverviewType.Chart;
            if (row.Table.Columns.Contains("xtype"))
                return (row["xtype"].ToString().Trim() == "G") ? FOverviewType.ReportGrid : FOverviewType.Chart;
            return FOverviewType.Chart;
        }

        private void UpdateStyle()
        {
            S.BackgroundColor = BackgroundColor = Models.Count <= 1 ? FSetting.BackgroundMain : FSetting.BackgroundSpacing;
            if (Models.Count == 0)
                return;

            Models.ForEach(x => S.Children.Add(new Frame() { BorderColor = FSetting.LineBoxReportColor, CornerRadius = 0, Padding = 0, HasShadow = false, Content = x.View }));
        }

        private Task<FMessage> GetArea()
        {
            return FServices.ExecuteCommand("DashboardArea", "System", null, "0", FExtensionParam.New(true, FText.AttributeString("V", "Dashboard"), FText.AttributeString("E", "Dashboard"), FAction.Load));
        }
    }
}