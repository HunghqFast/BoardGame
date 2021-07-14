using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CGrid : FDGrid
    {
        public bool LoadHasBusy { get; set; }
        private readonly string Controller;

        public CGrid(string controller) : base()
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

        public async Task GetModel()
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
            InitDataSource();
        }

        public async Task GetData()
        {
            if (Model == null)
                await GetModel();
            var message = await OnChange();
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return;
            }

            SumaryResult = message.ToDataSet();
            DataResult = SumaryResult?.Tables[Model.ChangingTable == -1 ? Model.LoadingTable : Model.ChangingTable];
            InitDataSource();
            RefreshFooter(false);
        }

        protected async override void OnLeftSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            base.OnLeftSelectionChanged(sender, e);
            await OnChanged();
        }

        protected async override void OnRightSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            base.OnRightSelectionChanged(sender, e);
            await OnChanged();
        }

        private Task<FMessage> OnChange()
        {
            var dt = new DataTable();
            foreach (var field in Model.Params)
                dt.AddRowValue(0, field.Name, field.Value.Replace("{?left}", CurrentCcodeLeft).Replace("{?right}", CurrentCcodeRight));
            return FServices.ExecuteCommand("2", Controller, new DataSet().AddTable(dt), "500", null, Controller);
        }

        private Task<FMessage> OnLoad()
        {
            return FServices.ExecuteCommand("1", Controller, null, "500", null, Controller);
        }
    }
}