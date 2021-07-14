using FastMobile.FXamarin.Core;
using System.Threading.Tasks;

namespace FastMobile.Core
{
    public class CPageReports : CPageMenu
    {
        public CPageReports(string group) : base(group, true)
        {
            ViewType = MenuViewType(group, FMenuViewType.Grid);
            InitToolbar();
            ItemTapped += Tabbed;
        }

        private async void Tabbed(object sender, IFDataEvent e)
        {
            await SetBusy(true);
            await ClickMenu(e.ItemData as FItemMenu);
            await SetBusy(false);
        }

        private async Task ClickMenu(FItemMenu item)
        {
            if (item == null)
                return;
            switch (item.XType)
            {
                case "C01":
                    await ShowPage(FChartType.Bar, item.Controller, true, item.Bar);
                    break;

                case "C02":
                    await ShowPage(FChartType.Pie, item.Controller, true, item.Bar);
                    break;

                case "C03":
                    await ShowPage(FChartType.Tri, item.Controller, true, item.Bar);
                    break;
                default:
                    await FPageReport.SetReportByAction(this, item.Action, item.Controller);
                    break;
            };
        }

        private async Task ShowPage(FChartType type, string controller, bool hasRefresh, string title)
        {
            var chartPage = new CPageChart(type, controller, hasRefresh) { Title = title };
            await Navigation.PushAsync(chartPage);
            chartPage.Init();
        }
    }
}