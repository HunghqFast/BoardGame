using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.Core;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FAlertBase : BindableObject
    {
        public static readonly BindableProperty AcceptColorProperty = BindableProperty.Create("AcceptColor", typeof(Color), typeof(FAlert), Color.FromHex("#087CFD"));
        public static readonly BindableProperty CancelColorProperty = BindableProperty.Create("CancelColor", typeof(Color), typeof(FAlert), Color.FromHex("#FF3C31"));
        public static readonly BindableProperty SubViewProperty = BindableProperty.Create("SubView", typeof(View), typeof(FAlert));
        public static readonly BindableProperty HelperTextProperty = BindableProperty.Create("HelperText", typeof(string), typeof(FAlert));
        public static readonly BindableProperty ShowHelperProperty = BindableProperty.Create("ShowHelper", typeof(bool), typeof(FAlert), false);

        public Color AcceptColor
        {
            get => (Color)GetValue(AcceptColorProperty);
            set => SetValue(AcceptColorProperty, value);
        }

        public Color CancelColor
        {
            get => (Color)GetValue(CancelColorProperty);
            set => SetValue(CancelColorProperty, value);
        }

        public View SubView
        {
            get => (View)GetValue(SubViewProperty);
            set => SetValue(SubViewProperty, value);
        }

        public string HelperText
        {
            get => (string)GetValue(HelperTextProperty);
            set => SetValue(HelperTextProperty, value);
        }

        public bool ShowHelper
        {
            get => (bool)GetValue(ShowHelperProperty);
            set => SetValue(ShowHelperProperty, value);
        }

        public event EventHandler<EventArgs> Completed;

        public event EventHandler<FObjectPropertyArgs<bool>> Confirmed;

        public static bool CanAlert { get; private set; }

        protected bool ResultConfirm { get; set; }
        protected readonly SfPopupLayout Alert;
        protected readonly Grid View;
        protected readonly Grid ButtonViews;
        protected readonly ContentView ParentSubView;
        protected readonly Label MessageLabel, TitleLabel, HelperLabel;
        protected readonly SfButton ConfirmBtn, CancelBtn;
        protected readonly BoxView Line, Column;
        protected readonly RowDefinition MessageRow, SubViewRow;

        private bool SingleButton;

        static FAlertBase()
        {
            CanAlert = true;
        }

        public FAlertBase()
        {
            MessageRow = new RowDefinition { Height = GridLength.Star };
            SubViewRow = new RowDefinition { Height = GridLength.Auto };
            Alert = new SfPopupLayout { BindingContext = this };
            View = new Grid { BindingContext = this };
            ButtonViews = new Grid { BindingContext = this };
            ParentSubView = new ContentView { BindingContext = this };
            MessageLabel = new Label { BindingContext = this };
            TitleLabel = new Label { BindingContext = this };
            HelperLabel = new Label { BindingContext = this };
            ConfirmBtn = new SfButton { BindingContext = this };
            CancelBtn = new SfButton { BindingContext = this };
            Line = new BoxView { BindingContext = this };
            Column = new BoxView { BindingContext = this };
            Base();
        }

        public static void UpdateCanAlert(bool val = true)
        {
            CanAlert = val;
        }

        public Task Show(string message)
        {
            if (IsShowedOrCanotAlert() || string.IsNullOrEmpty(message))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(() => Load(true, "", message, FText.Accept, ""));
            AfterAlert();
            return Task.CompletedTask;
        }

        public Task Show(string message, string acceptText)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(() => Load(true, "", message, acceptText, ""));
            AfterAlert();
            return Task.CompletedTask;
        }

        public Task Show(string title, string message, string acceptText)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(() => Load(true, title, message, acceptText, ""));
            AfterAlert();
            return Task.CompletedTask;
        }

        public Task Toast(string message, int milisecond)
        {
            if (IsShowedOrCanotAlert() || string.IsNullOrEmpty(message))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(async () =>
            {
                Load(true, "", message, FText.Accept, "");
                if (milisecond > 0)
                {
                    await Task.Delay(milisecond);
                    if (Alert != null) Alert.IsOpen = false;
                }
            });
            AfterAlert();
            return Task.CompletedTask;
        }

        public Task Toast(string message, string acceptText, int milisecond)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(async () =>
            {
                Load(true, "", message, acceptText, "");
                if (milisecond > 0)
                {
                    await Task.Delay(milisecond);
                    if (Alert != null) Alert.IsOpen = false;
                }
                AfterAlert();
            });
            return Task.CompletedTask;
        }

        public Task Toast(string title, string message, string acceptText, int milisecond)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText))
                return Task.CompletedTask;
            BeforeAlert();
            Device.BeginInvokeOnMainThread(async () =>
            {
                Load(true, title, message, acceptText, "");
                if (milisecond > 0)
                {
                    await Task.Delay(milisecond);
                    if (Alert != null) Alert.IsOpen = false;
                }
                AfterAlert();
            });
            return Task.CompletedTask;
        }

        public async Task<bool> Confirm(string message)
        {
            if (IsShowedOrCanotAlert())
                return false;
            BeforeLoadConfirm();
            Load(false, "", message, FText.Yes, FText.No);
            return await WaitConfirm();
        }

        public async Task<bool> Confirm(string message, string acceptText, string cancelText)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return false;
            BeforeLoadConfirm();
            Load(false, "", message, acceptText, cancelText);
            return await WaitConfirm();
        }

        public async Task<bool> Confirm(string title, string message, string acceptText, string cancelText)
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return false;
            BeforeLoadConfirm();
            Load(false, title, message, acceptText, cancelText);
            return await WaitConfirm();
        }

        public void Close()
        {
            Alert.IsOpen = false;
        }

        public void UpdateChilds()
        {
            View.Children.Clear();
            View.ColumnDefinitions = new ColumnDefinitionCollection();
            View.RowDefinitions = new RowDefinitionCollection();

            ButtonViews.Children.Clear();
            ButtonViews.ColumnDefinitions = new ColumnDefinitionCollection();

            View.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            View.RowDefinitions.Add(MessageRow);
            //View.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            View.RowDefinitions.Add(SubViewRow);
            View.RowDefinitions.Add(new RowDefinition { Height = 1 });
            View.RowDefinitions.Add(new RowDefinition { Height = 50 });

            View.Children.Add(TitleLabel, 0, View.Children.Count);
            View.Children.Add(MessageLabel, 0, View.Children.Count);
            //View.Children.Add(HelperLabel, 0,  View.Children.Count;
            View.Children.Add(ParentSubView, 0, View.Children.Count);
            View.Children.Add(Line, 0, View.Children.Count);
            View.Children.Add(ButtonViews, 0, View.Children.Count);

            if (SingleButton)
            {
                ButtonViews.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                ButtonViews.Children.Add(ConfirmBtn, 0, 0);
                return;
            }

            ButtonViews.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ButtonViews.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
            ButtonViews.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ButtonViews.Children.Add(ConfirmBtn, 0, 0);
            ButtonViews.Children.Add(Column, 1, 0);
            ButtonViews.Children.Add(CancelBtn, 2, 0);
        }

        protected bool IsShowedOrCanotAlert()
        {
            return/* Showed || */!CanAlert;
        }

        protected void BeforeLoadConfirm()
        {
            BeforeAlert();
            ResultConfirm = false;
        }

        protected void BeforeAlert()
        {
            CanAlert = false;
        }

        protected void AfterAlert()
        {
            //Showed = true;
        }

        protected virtual void BeforeBase()
        {
        }

        private void Base()
        {
            BeforeBase();
            Line.HeightRequest = 1;
            Line.BackgroundColor = FSetting.LineBoxReportColor;
            Line.HorizontalOptions = LayoutOptions.FillAndExpand;

            Column.VerticalOptions = LayoutOptions.FillAndExpand;
            Column.WidthRequest = 1;
            Column.BackgroundColor = FSetting.LineBoxReportColor;

            TitleLabel.TextColor = FSetting.TextColorTitle;
            TitleLabel.FontAttributes = FontAttributes.Bold;
            TitleLabel.FontFamily = FSetting.FontTextMedium;
            TitleLabel.FontSize = FSetting.FontSizeLabelTitle + 3;
            TitleLabel.MaxLines = 2;
            TitleLabel.Padding = new Thickness(10, 12);
            TitleLabel.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            TitleLabel.VerticalOptions = LayoutOptions.Center;
            TitleLabel.HorizontalOptions = LayoutOptions.Center;

            if (FSetting.IsAndroid)
                MessageLabel.TextColor = FSetting.TextColorContent;
            MessageLabel.FontSize = FSetting.FontSizeLabelTitle;
            MessageLabel.FontFamily = FSetting.FontText;
            MessageLabel.MaxLines = 20;
            MessageLabel.HorizontalOptions = LayoutOptions.Center;
            MessageLabel.HorizontalTextAlignment = TextAlignment.Center;
            MessageLabel.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            MessageLabel.TextType = TextType.Html;
            MessageLabel.Padding = new Thickness(10, 10, 10, 0);

            HelperLabel.TextColor = FSetting.DangerColor;
            HelperLabel.FontSize = FSetting.FontSizeLabelTitle;
            HelperLabel.FontFamily = FSetting.FontText;
            HelperLabel.MaxLines = 20;
            HelperLabel.HorizontalOptions = LayoutOptions.Center;
            HelperLabel.HorizontalTextAlignment = TextAlignment.Center;
            HelperLabel.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            HelperLabel.TextType = TextType.Html;
            HelperLabel.Padding = new Thickness(10, 10, 10, 0);
            HelperLabel.SetBinding(Label.IsVisibleProperty, ShowHelperProperty.PropertyName);
            HelperLabel.SetBinding(Label.TextProperty, HelperTextProperty.PropertyName);

            CancelBtn.BackgroundColor = ConfirmBtn.BackgroundColor = Color.Transparent;
            CancelBtn.SetBinding(SfButton.TextColorProperty, CancelColorProperty.PropertyName);

            ConfirmBtn.BorderWidth = CancelBtn.BorderWidth = 0;
            ConfirmBtn.FontFamily = CancelBtn.FontFamily = FSetting.FontText;
            ConfirmBtn.FontSize = CancelBtn.FontSize = FSetting.FontSizeLabelTitle + 3;
            ConfirmBtn.HorizontalTextAlignment = CancelBtn.HorizontalTextAlignment = ConfirmBtn.VerticalTextAlignment = CancelBtn.VerticalTextAlignment = TextAlignment.Center;
            ConfirmBtn.HorizontalOptions = CancelBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            ConfirmBtn.VerticalOptions = CancelBtn.VerticalOptions = LayoutOptions.Fill;
            ConfirmBtn.SetBinding(SfButton.TextColorProperty, AcceptColorProperty.PropertyName);

            View.RowSpacing = View.ColumnSpacing = 0;
            View.BackgroundColor = FSetting.BackgroundMain;

            ButtonViews.ColumnSpacing = 0;
            ButtonViews.HorizontalOptions = LayoutOptions.Fill;

            ParentSubView.SetBinding(ContentView.ContentProperty, SubViewProperty.PropertyName);

            Alert.StaysOpen = true;
            Alert.PopupView.Margin = 0;
            Alert.PopupView.ShowFooter = false;
            Alert.PopupView.ShowHeader = false;
            Alert.PopupView.WidthRequest = FSetting.ScreenWidth < 330 ? FSetting.ScreenWidth - 20 : 320;
            Alert.PopupView.AutoSizeMode = AutoSizeMode.Height;
            Alert.PopupView.AnimationMode = AnimationMode.Zoom;
            Alert.PopupView.AnimationDuration = 100;
            Alert.PopupView.BackgroundColor = Color.Transparent;
            Alert.PopupView.ContentTemplate = new DataTemplate(() => View);
            Alert.PopupView.PopupStyle.OverlayOpacity = 0.3;
            Alert.PopupView.PopupStyle.CornerRadius = FSetting.IsAndroid ? Convert.ToInt32(20 * Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density) : Alert.PopupView.PopupStyle.CornerRadius;
            Alert.PopupView.PopupStyle.BorderColor = Color.Transparent;
            Alert.PopupView.PopupStyle.BorderThickness = 0;

            ConfirmBtn.Clicked += ConfirmButtonClicked;
            CancelBtn.Clicked += CancelButtonClicked;
            Alert.Closed += AlertClosed;
            Alert.Closing += AlertClosing;
            Alert.Closing += AlertClosing;
        }

        protected virtual void Load(bool single, string title, string message, string accept, string cancel)
        {
            try
            {
                SingleButton = single;
                UpdateChilds();
                TitleLabel.Text = string.IsNullOrEmpty(title) ? FText.ApplicationTitle : title;
                MessageLabel.Text = Config(message);
                ConfirmBtn.Text = accept;
                CancelBtn.Text = cancel;
                Alert.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(FText.ApplicationTitle, ex.Message);
                UpdateCanAlert();
            }
        }

        protected async Task<bool> WaitConfirm()
        {
            while (Alert.IsOpen && IsWait() && Confirmed == null)
                await Task.Delay(100);
            AfterAlert();
            return ResultConfirm;
        }

        protected virtual bool IsWait()
        {
            return true;
        }

        protected virtual string Config(string message)
        {
            message = message.Replace("\n", "</br>");
            message = Align(MessageLabel, message, out var align);
            message = Html(message, align);
            message = Html(message, "*", "background-color:#dceef5;");
            message = Html(message, "#", "color: #1c4580;");
            message = Html(message, "$", "color: #444444;");
            message = Html(message, "@", "color: #035ab8;");
            message = Html(message, "%", "font-weight: bold;");
            message = FHtml.ReplaceTab(message);
            return message;
        }

        private string Html(string message, string key, string style)
        {
            foreach (Match match in new Regex(@$"\{key}\{key}(.*?)\{key}\{key}").Matches(message))
                message = message.Replace($"{key}{key}{match.Groups[1].Value}{key}{key}", @$"<span style=""{style}"">{match.Groups[1].Value}</span>");
            return message;
        }

        private string Html(string message, string align)
        {
            return $@"<div style=""color:{FSetting.TextColorContent.Hex()};text-align:{align};font-family:Roboto;font-size:{FSetting.FontSizeLabelTitle}px;"">{message}</div>";
        }

        private string Align(Label L, string message, out string align)
        {
            if (message.Length < 2)
            {
                align = "center";
                return message;
            }

            L.HorizontalTextAlignment = message.Substring(0, 2) == "$L" ? TextAlignment.Start : message.Substring(0, 2) == "$R" ? TextAlignment.End : L.HorizontalTextAlignment;
            align = message.Substring(0, 2) switch
            {
                "$L" => "left",
                "$R" => "right",
                _ => "center"
            };
            return align == "center" ? message : message.Remove(0, 2);
        }

        private void AlertClosed(object sender, EventArgs e)
        {
            Alert.Dispose();
            Completed?.Invoke(this, EventArgs.Empty);
            Confirmed?.Invoke(this, new FObjectPropertyArgs<bool>(ResultConfirm));
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Alert.IsOpen = false;
        }

        private void ConfirmButtonClicked(object sender, EventArgs e)
        {
            ResultConfirm = true;
            Alert.IsOpen = false;
        }

        private void AlertClosing(object sender, CancelEventArgs e)
        {
            CanAlert = true;
        }
    }
}