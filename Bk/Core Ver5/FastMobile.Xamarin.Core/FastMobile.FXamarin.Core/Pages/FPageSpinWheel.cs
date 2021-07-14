using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageSpinWheel : FPage, IFSpinWheelService
    {
        protected const string Type = "spinwheel", Target = "SpinWheel", Load = "Loading", Roll = "Roll";

        protected string StrucDeviceID, Controller, ProgramID;
        protected FSpinWheelContainer Wheel;
        protected FPage Root;
        protected Grid View;

        public FViewPage Struct;
        public string Success;
        public string Fail;

        public event EventHandler RequestSuccessed;

        public FPageSpinWheel(FPage page, string controller) : base(false, false)
        {
            Root = page;
            Controller = controller;
            StrucDeviceID = $"FastMobile.FXamarin.Core.FPageFilter.device_struct_{FString.ServiceName}_{FString.UserID}_{Target}_{Controller}";
            ProgramID = string.Empty;
            View = new Grid();
            Wheel = new FSpinWheelContainer();
            RequestSuccessed -= FPageSpinWheelRequestSuccessed;
            RequestSuccessed += FPageSpinWheelRequestSuccessed;
        }

        #region Protected

        protected virtual FSpinWheelListData InitSpinWheelData(DataTable table)
        {
            var data = new FSpinWheelListData();
            table.Rows.ForEach<DataRow>(row => { data.Add(new FSpinWheelData(row)); });
            data.UpdateValue();
            return data;
        }

        protected virtual void InitSpinWheelDetail()
        {
            Title = Struct.Title;
            BackgroundColor = Color.FromHex(GetDetailsProperty("_BACKGROUND_COLOR_0_", "#ffffff"));
            View.BackgroundColor = Color.FromHex(GetDetailsProperty("_BACKGROUND_COLOR_1_", "#ffffff"));

            Wheel.BackgroundColor = Color.FromHex(GetDetailsProperty("_SPINWHEEL_BACKGROUND_COLOR_", "#ffffff"));
            Wheel.Button.Text = GetDetailsProperty("_SPINWHEEL_CENTER_TEXT_", "", true);
            Wheel.Button.BackgroundColor = Color.FromHex(GetDetailsProperty("_SPINWHEEL_CENTER_COLOR_", "#ffffff"));
            Wheel.Pointer.Fill = Wheel.Pointer.Stroke = new SolidColorBrush(Wheel.Button.BackgroundColor);
            Wheel.Button.TextColor = Color.FromHex(GetDetailsProperty("_SPINWHEEL_CENTER_TEXT_COLOR_", "#ffffff"));
            Wheel.Button.BorderColor = Color.FromHex(GetDetailsProperty("_SPINWHEEL_CENTER_STROKE_COLOR_", "#ffffff"));
            Wheel.Button.BorderWidth = double.Parse(GetDetailsProperty("_SPINWHEEL_CENTER_STROKE_WIDTH_", "2"));

            Wheel.SpinSeries.DoughnutCoefficient = double.Parse(GetDetailsProperty("_SPINWHEEL_CENTER_RATIO_", "0.2"));
            Wheel.SpinSeries.CircularCoefficient = double.Parse(GetDetailsProperty("_SPINWHEEL_RATIO_", "0.8"));
            Wheel.SpinSeries.StrokeWidth = Wheel.Button.BorderWidth = double.Parse(GetDetailsProperty("_SPINWHEEL_STROKE_WITDH_", "1"));
            Wheel.SpinSeries.StrokeColor = Color.FromHex(GetDetailsProperty("_SPINWHEEL_STROKE_COLOR_", "#ffffff"));

            Success = GetDetailsProperty("_MESSAGE_SUCCESS_", "", true);
            Fail = GetDetailsProperty("_MESSAGE_FAIL_", "", true);
        }

        protected virtual void InitButton()
        {
            var connerRadius = double.Parse(GetDetailsProperty("_BUTTON_CORNER_RADIUS_", "5"));
            var backgroundColor = Color.FromHex(GetDetailsProperty("_BUTTON_BACKGROUND_COLOR_", "#ffffff"));
            Struct.Toolbars.ForEach(toolbar =>
            {
                var color = toolbar.GetColor();
                var button = new FButton(toolbar.Title, toolbar.GetIcon(color, FSetting.SizeIconButton));
                button.HeightRequest = 50;
                button.TextColor = color;
                button.CornerRadius = connerRadius;
                button.BackgroundColor = backgroundColor;
                View.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                View.Children.Add(button, 0, View.RowDefinitions.Count - 1);
            });
        }

        #endregion Protected

        #region Public

        public async override void Init()
        {
            base.Init();
            try
            {
                if (!await CheckStructPage()) return;
                InitSpinWheelDetail();
                if (!await CheckStatusPage()) return;
            }
            catch (Exception ex) { MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE); }
        }

        #endregion Public

        #region Private

        private void InitProperty(DataTable data)
        {
            if (data.Rows.Count == 0) return;
            if (data.Columns.Contains("progarmID")) ProgramID = data.Rows[0]["progarmID"].ToString();
        }

        private void InitView(DataTable data)
        {
            Wheel.Data = InitSpinWheelData(data);
            Wheel.Render();
            Wheel.SpinClicked -= WheelSpinClicked;
            Wheel.SpinClicked += WheelSpinClicked;

            View.ColumnSpacing = View.RowSpacing = 10;
            View.Padding = 10;
            View.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            View.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            View.Children.Add(Wheel, 0, 0);
            InitButton();
            Content = View;
        }

        private async Task<bool> CheckStructPage()
        {
            var setting = await FViewPage.InitSettingsFromDevice(StrucDeviceID);
            if (setting != null)
            {
                Struct = new FViewPage(setting);
                return true;
            }

            var check = await GetStruct();
            if (check.Success == 1)
            {
                var data = JObject.Parse(check.Message.AESDecrypt(FSetting.NetWorkKey));
                Struct = new FViewPage(StrucDeviceID, (JObject)data["ViewPage"]);
                return true;
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            return false;
        }

        private async Task<bool> CheckStatusPage()
        {
            var mess = await GetData();
            if (mess.Success == 1)
            {
                var ds = mess.ToDataSet();
                var check = await FReportToolbar.TryCatchMessage(Root, Root, ds, 1, (dt) =>
                {
                    InitProperty(ds.Tables[0]);
                    InitView(dt);
                    RequestSuccessed?.Invoke(this, EventArgs.Empty);
                    return Task.CompletedTask;
                });
                return check.Result;
            }
            MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
            return false;
        }

        private async Task<(bool OK, string Result)> GetRollResult()
        {
            var mess = await GetRoll();
            if (mess.Success == 1)
            {
                string result = string.Empty;
                var check = await FReportToolbar.TryCatchMessage(Root, Root, mess.ToDataSet(), 1, (dt) =>
                {
                    if (dt.Columns.Contains("result")) result = dt.Rows[0]["result"].ToString();
                    return Task.CompletedTask;
                });
                return (check.Result, result.TrimEnd());
            }
            MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
            return (false, string.Empty);
        }

        private string GetDetailsProperty(string name, string init, bool isLanguage = false)
        {
            var property = Struct.Details.Find(x => x.Name.Equals(name));
            if (property == null) return init;
            if (isLanguage) return property.Title;
            return property.DefaultValue == null ? init : property.DefaultValue.ToString();
        }

        private async void FPageSpinWheelRequestSuccessed(object sender, EventArgs e)
        {
            await Root.Navigation.PushAsync(this, true);
        }

        private async void WheelSpinClicked(object sender, FSpinWheelEventArgs e)
        {
            await ShowMessage(await e.SpinAsync(GetRollResult));
        }

        private Task ShowMessage(int index)
        {
            if (index == -2) return Task.CompletedTask;
            if (index == -1 || !Wheel.Data[index].Status) MessagingCenter.Send(FMessage.FromFail(0, Fail), FChannel.ALERT_BY_MESSAGE);
            else MessagingCenter.Send(FMessage.FromFail(0, string.Format(Success, Wheel.Data[index].Name)), FChannel.ALERT_BY_MESSAGE);
            return Task.CompletedTask;
        }

        #endregion Private

        #region Service

        public Task<FMessage> GetStruct()
        {
            return FServices.ExecuteCommand(Target, Controller, new DataSet().AddTable(new DataTable().AddRowValue("type", Type).AddRowValue(0, "target", Target)), "300", FExtensionParam.New(true, Controller, FAction.Initialize), true);
        }

        public async Task<FMessage> GetData()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", Target);
            var result = await FServices.ExecuteCommand(Load, Controller, ds, "300", null);
            ds.Dispose();
            return result;
        }

        public async Task<FMessage> GetRoll()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "response", Target);
            ds.AddTable(new DataTable().AddRowValue("programID", ProgramID));
            FMessage result = await FServices.ExecuteCommand(Roll, Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        #endregion Service
    }
}