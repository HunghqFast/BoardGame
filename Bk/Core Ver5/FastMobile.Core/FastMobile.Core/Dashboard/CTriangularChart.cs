using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CTriangularChart : FTriangularChart
    {
        public bool LoadHasBusy { get; set; }
        private readonly string Controller;
        private bool IsChanged;

        public CTriangularChart(string controller) : base()
        {
            Controller = controller;
            Init();
        }

        public override async Task OnLoaded()
        {
            try
            {
                await Device.InvokeOnMainThreadAsync(async () => await SetBusy(LoadHasBusy && true));
                if (!FSetting.IsAndroid) await Task.Delay(50);
                await GetModel();
                await SetBusy(false);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
            }
        }

        public override async Task OnChanged()
        {
            try
            {
                await Device.InvokeOnMainThreadAsync(async () => await SetBusy(true));
                if (!FSetting.IsAndroid) await Task.Delay(50);
                await GetData();
                await SetBusy(false);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
            }
        }

        private async Task GetModel()
        {
            var message = await OnLoad();
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return;
            }

            SumaryResult = message.ToDataSet();
            InitModel();
            InitHeader();
            InitHeaderItemSource();
            InitDataSource(Model.TriSeries);
            await RefreshSeries(true);
        }

        private async Task GetData()
        {
            if (Model == null)
                await GetModel();
            if (Model.RefreshOnChanging || IsChanged)
            {
                while (Chart.Series.Count > 0)
                    Chart.Series.RemoveAt(0);
            }
            var message = await OnChange();
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return;
            }

            base.SumaryResult = message.ToDataSet();
            base.DataResult = SumaryResult?.Tables[Model.ChangingTable == -1 ? Model.LoadingTable : Model.ChangingTable];
            base.InitDataSource(Model.TriSeries, !Model.RefreshOnChanging);
            RefreshFooter(false);
            if (Model.RefreshOnChanging || IsChanged || Chart.Series.Count == 0)
                await RefreshSeries(false);
        }

        protected async override void OnLeftSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            IsChanged = true;
            base.OnLeftSelectionChanged(sender, e);
            await OnChanged();
            IsChanged = false;
        }

        protected async override void OnRightSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            IsChanged = true;
            base.OnRightSelectionChanged(sender, e);
            await OnChanged();
            IsChanged = false;
        }

        private Task<FMessage> OnChange()
        {
            var dt = new DataTable();
            foreach (var field in Model.Params)
                dt.AddRowValue(0, field.Name, field.Value.Replace("{?left}", CurrentCcodeLeft).Replace("{?right}", CurrentCcodeRight));
            return FServices.ExecuteCommand("2", Controller, new DataSet().AddTable(dt), "400", null, Controller);
        }

        private Task<FMessage> OnLoad()
        {
            return FServices.ExecuteCommand("1", Controller, null, "400", null, Controller);
        }
    }
}