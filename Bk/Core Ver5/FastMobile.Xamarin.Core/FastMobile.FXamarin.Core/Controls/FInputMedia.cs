using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XPath = System.IO.Path;

namespace FastMobile.FXamarin.Core
{
    public abstract class FInputMedia : FInput, IFInputRequest, IFVisibleView
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FInputMedia));
        public static readonly BindableProperty IsClientProperty = BindableProperty.Create("IsClient", typeof(bool), typeof(FInputMedia));
        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public bool IsClient { get => (bool)GetValue(IsClientProperty); set => SetValue(IsClientProperty, value); }

        public string MediaPath { get; set; }
        public string Controller { get; }
        public string SysField { get; }
        public string LineNumber { get; }
        public List<string> AllowExtensions { get; }
        protected byte[] MediaData { get; set; }
        protected string SaveFileName { get; set; }
        protected string SaveFileExt { get; set; }

        protected bool IsEdit { get; private set; }
        protected ImageButton V;
        protected FButton U, D, C, P;
        protected Grid G;

        private string Ticket;

        #region Public

        public FInputMedia(string controller, string sysField, string lineNumber, List<string> allow) : base()
        {
            Controller = controller;
            SysField = sysField;
            LineNumber = lineNumber;
            AllowExtensions = allow;
        }

        public FInputMedia(FField field) : base(field)
        {
            Controller = ValueByKey(field.Keys, "Controller");
            SysField = ValueByKey(field.Keys, "Field");
            LineNumber = ValueByKey(field.Keys, "LineNumber", "1");
            AllowExtensions = FFunc.GetArrayString(ValueByKey(field.Keys, "Allow", "*"));
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            if (DefaultValue != null)
            {
                IsClient = false;
                SetValue(DefaultValue.ToString(), "3", false, null);
            }
        }

        public async Task InvokeService()
        {
            if (!IsClient || (MediaData == null && Value != "2") || Value == "3") return;

            await Root.SetBusy(true);
            if (!await GenTicket())
            {
                await Root.SetBusy(false);
                return;
            }

            var files = new List<FFileInfo>();
            var fileInfo = new FFileInfo(MediaPath, $"{LineNumber}.Insert", MediaData, SaveFileName);
            if (fileInfo.Data == null || fileInfo.Data.Length == 0)
            {
                await Root.SetBusy(false);
                return;
            }

            files.Add(fileInfo);
            var message = await FServices.Attachment(Controller, Root.Input[SysField].GetInput(0).ToString().TrimEnd(), Ticket, "1", files);
            if (FSetting.IsDebug && !message.OK) MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
        }

        public override bool IsEqual(object oldValue)
        {
            return MediaData == null && Value != "2";
        }

        public bool IsShouldVisible()
        {
            return IsShouldVisible(Value);
        }

        public bool IsShouldVisible(object value)
        {
            return value != null && !value.Equals("2");
        }

        #endregion Public

        #region Protected

        protected abstract FButton CaptureButton();

        protected abstract string UploadTitle();

        protected abstract string DownloadTitle();

        protected abstract Task<FileResult> TakeMedia();

        protected abstract Task<FileResult> PickMedia();

        protected abstract ImageSource FileImage(string path);

        protected abstract IEnumerable<FButton> ExtendButtons();

        protected abstract void OnImageClicked(object sender, EventArgs e);

        protected override void Init()
        {
            V = new ImageButton();
            P = CaptureButton();
            U = new FButton(UploadTitle(), FIcons.ImageOutline);
            D = new FButton(DownloadTitle(), FIcons.Download);
            C = new FButton(FText.Delete, FIcons.Close);
            G = new Grid();
            base.Init();
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            var val = value.Count > 0 ? value[0] ?? "" : "";
            if (string.IsNullOrWhiteSpace(val))
            {
                base.SetInput(value, isCompleted, isDisable);
                return;
            }

            IsClient = false;
            IsEdit = true;
            if (!File.Exists(val)) val = XPath.Combine(FString.ServiceUrl, val);
            SetValue(val, "3", false, null);
        }

        //Mode 0 - Insert, 1 - Update, 2 - Delete, 3 - Nothing
        protected override object ReturnValue(int mode)
        {
            return Value ?? "3";
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsModifierPropertyName)
            {
                OnPropertyChanged("Value");
                return;
            }

            if (propertyName == TitleProperty.PropertyName)
            {
                HasPadding = IsShowTitle = !string.IsNullOrEmpty(Title);
                V.Margin = IsShowTitle ? new Thickness(0, 5, PaddingSize, PaddingSize) : PaddingSize;
                return;
            }
        }

        protected override View SetContentView()
        {
            HeightInput = -1;
            IsClient = true;
            HasPadding = IsShowTitle = !string.IsNullOrEmpty(Title);

            var s = new StackLayout();
            var v = new ScrollView();

            s.Spacing = FSetting.SpacingButtons;
            s.Orientation = StackOrientation.Horizontal;

            D.SetBinding(View.IsVisibleProperty, IsClientProperty.PropertyName, converter: FInvertBool.Instance);
            C.SetBinding(View.IsVisibleProperty, new MultiBinding { Mode = BindingMode.TwoWay, Bindings = new List<BindingBase> { new Binding(IsModifierPropertyName), new Binding(ValueProperty.PropertyName, converter: new FValueToVisible(this)) }, Converter = new FMultiBoolConvert() });

            U.Clicked += OnPickMedia;
            D.Clicked += OnDownloadMedia;
            C.Clicked += OnRemoveMedia;
            P.Clicked += OnCaptureMedia;

            s.Children.Add(U);
            s.Children.Add(P);
            s.Children.Add(D);
            var ext = ExtendButtons();
            ext?.ForEach(x => s.Children.Add(x));
            s.Children.Add(C);

            v.HorizontalOptions = LayoutOptions.StartAndExpand;
            v.Content = s;
            v.HeightRequest = FSetting.HeightRowGrid;
            v.Orientation = ScrollOrientation.Horizontal;
            v.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

            V.BorderWidth = 0;
            V.CornerRadius = 5;
            V.BorderColor = V.BackgroundColor = Color.Transparent;
            V.HorizontalOptions = V.VerticalOptions = LayoutOptions.Start;
            V.Margin = IsShowTitle ? new Thickness(0, 5, PaddingSize, PaddingSize) : PaddingSize;
            V.Clicked += OnImageClicked;

            G.RowSpacing = 0;
            G.HorizontalOptions = G.VerticalOptions = LayoutOptions.Fill;
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            G.RowDefinitions.Add(new RowDefinition { Height = 1 });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            G.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            G.Children.Add(V, 0, 0);
            G.Children.Add(new FLine(), 0, 1);
            G.Children.Add(v, 0, 2);

            return G;
        }

        protected virtual async Task OnTakeMedia(object arg)
        {
            if (!IsModifier) return;
            try
            {
                await Root.SetBusy(true);
                await LoadMedia(await TakeMedia());
                await Root.SetBusy(false);
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                else MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                await Root.SetBusy(false);
            }
        }

        protected virtual async Task OnPickMedia(object sender)
        {
            if (!IsModifier) return;
            try
            {
                await Root.SetBusy(true);
                await LoadMedia(await PickMedia());
                await Root.SetBusy(false);
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                else MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                await Root.SetBusy(false);
            }
        }

        protected virtual async Task LoadMedia(FileResult file)
        {
            if (file == null) return;

            if (!AllowExtensions.Contains("*") && !AllowExtensions.Contains(Path.GetExtension(file.FullPath)))
            {
                MessagingCenter.Send(FMessage.FromFail(900), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            MediaData = await FUtility.GetFileData(file);
            SetValue(await FUtility.GetFilePath(file), IsEdit ? "1" : "0", true, null);
            IsClient = true;
        }

        protected virtual void OnClearMedia(object arg)
        {
            SetValue(null, IsEdit ? "2" : "3", true, null);
            IsClient = true;
            MediaData = null;
        }

        protected virtual void SetValue(string path, string value, bool completed, ImageSource viewSource)
        {
            try
            {
                Value = value;
                MediaPath = path;
                V.Source = viewSource ?? FileImage(path);
                if (completed) OnCompleteValue(this, new FInputChangeValueEventArgs(Value));
            }
            catch { }
        }

        #endregion Protected

        #region Private

        private async void OnCaptureMedia(object sender, EventArgs e)
        {
            await OnTakeMedia(null);
        }

        private async void OnPickMedia(object sender, EventArgs e)
        {
            await OnPickMedia(sender);
        }

        private async void OnDownloadMedia(object sender, EventArgs e)
        {
            await OnDownloadItem();
        }

        private void OnRemoveMedia(object sender, EventArgs e)
        {
            OnClearMedia(sender);
        }

        private async Task OnDownloadItem()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            await Root.SetBusy(true);
            var fileName = FUtility.GetParamFromUrl(MediaPath, "n");
            FInterface.IFDownload.OnFileDownloaded += FUtility.DownloadCompleted;
            await FInterface.IFDownload.DownloadFile(FServices.DownloadUrl(Controller, Controller, fileName, Root.Input[SysField].GetInput(0).ToString(), "1", Ticket, 1), fileName);
            FInterface.IFDownload.OnFileDownloaded -= FUtility.DownloadCompleted;
            await Root.SetBusy(false);
        }

        protected virtual async Task<bool> GenTicket()
        {
            var ds = new DataSet();
            ds.AddTable(new DataTable().AddRowValue("controller", Controller));
            var message = await FServices.ExecuteCommand("GetTicket", "System", ds, "0", null, true);
            if (!message.OK)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            ds = message.ToDataSet();
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || !ds.Tables[0].Columns.Contains("ticket"))
            {
                MessagingCenter.Send(FMessage.FromFail(208), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            Ticket = ds.Tables[0].Rows[0]["ticket"].ToString();
            return true;
        }

        private string ValueByKey(Dictionary<string, string> dic, string key, string defaultValue = "")
        {
            return dic.ContainsKey(key) ? dic[key] : defaultValue;
        }

        #endregion Private

        #region Nested

        protected class FValueToVisible : IValueConverter
        {
            public IFVisibleView This;
            private object Value;

            public FValueToVisible(IFVisibleView view)
            {
                This = view;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Value = value;
                return This.IsShouldVisible(value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return Value;
            }
        }

        #endregion Nested
    }
}