using FastMobile.FXamarin.Core;
using System;
using System.Threading.Tasks;

namespace FastMobile.Core
{
    public class CPageChart : FPage
    {
        private readonly string contr;
        private readonly FChartType TypeC;

        public CPageChart(FChartType fChartType, string controller, bool IsHasPullToRefresh) : base(IsHasPullToRefresh, true)
        {
            TypeC = fChartType;
            contr = controller;
        }

        public async override void Init()
        {
            base.Init();
            if (!HasNetwork)
                return;
            await SetBusy(true);
            await Create();
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
            await (Content as IFLayout)?.OnChanged();
            await SetRefresh(false);
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        private Task Create()
        {
            switch (TypeC)
            {
                case FChartType.Bar:
                    Content = new CXyChart(contr);
                    return (Content as IFLayout)?.OnLoaded();

                case FChartType.Pie:
                    Content = new CCircularChart(contr);
                    return (Content as IFLayout)?.OnLoaded();

                case FChartType.Tri:
                    Content = new CTriangularChart(contr);
                    return (Content as IFLayout)?.OnLoaded();

                default:
                    return Task.CompletedTask;
            }
        }
    }
}