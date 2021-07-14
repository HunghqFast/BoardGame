using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageResetPassword : FPageInput
    {
        public readonly FInputText UsernameInput, EmailInput, CapchaInput;
        public readonly FCapchaImage Captcha;

        private ToolbarItem GuideButton;
        private FXml GuideXML;
        private FXml.FLGuide Guide;

        public FPageResetPassword(bool isHasPullToRefresh, bool enableScrolling = false) : base(isHasPullToRefresh, enableScrolling)
        {
            UsernameInput = new FInputText();
            EmailInput = new FInputText();
            CapchaInput = new FInputText();
            Captcha = new FCapchaImage();

            Submiter.Clicked += OnSubmit;
            Closer.Clicked += OnClose;
            Captcha.Loading += CapchaLoading;
            Update(FSetting.V);
            InitForm();
            BuildGuide();
        }

        public override void Init()
        {
            base.Init();
            CapchaLoading(null, null);
        }

        public override void Update(bool v)
        {
            Title = FText.ResetPassword;
            UsernameInput.Title = FText.Name;
            EmailInput.Title = FText.Email;
            CapchaInput.Title = FText.Code;
            Submiter.Text = FText.Accept;
            Closer.Text = FText.Cancel;
            Captcha.DownloadingText = FText.Downloading;
        }

        private void InitForm()
        {
            UsernameInput.NotAllowsNull = true;
            UsernameInput.IsTextUpper = FSetting.AppMode == FAppMode.FBO;

            EmailInput.NotAllowsNull = true;
            EmailInput.Keyboard = Keyboard.Email;

            CapchaInput.NotAllowsNull = true;

            Captcha.Margin = new Thickness(10, 0, 0, 0);
            Captcha.ReloadText = FIcons.Reload;
            Captcha.ReloadColor = FSetting.PrimaryColor;
            Captcha.ReloadFontFamily = FSetting.FontIcon;
            Captcha.ReloadFontSize = FSetting.SizeButtonIcon;
            UsernameInput.Rendering();
            EmailInput.Rendering();
            CapchaInput.Rendering();

            Form.RowSpacing = 0;
            Form.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });

            if (FSetting.AppMode == FAppMode.FBO)
            {
                Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
                Form.Children.Add(UsernameInput, 0, 0);
                Form.Children.Add(new FLine(), 0, 1);
                Form.Children.Add(EmailInput, 0, 2);
                Form.Children.Add(new FLine(), 0, 3);
                Form.Children.Add(Captcha, 0, 4);
                Form.Children.Add(new FLine(), 0, 5);
                Form.Children.Add(CapchaInput, 0, 6);
                Form.Children.Add(new FLine(), 0, 7);
            }
            else
            {
                Form.Children.Add(EmailInput, 0, 0);
                Form.Children.Add(new FLine(), 0, 1);
                Form.Children.Add(Captcha, 0, 2);
                Form.Children.Add(new FLine(), 0, 3);
                Form.Children.Add(CapchaInput, 0, 4);
                Form.Children.Add(new FLine(), 0, 5);
            }
        }

        private bool CheckInputEmpty(int errorCode, params FInputText[] objList)
        {
            foreach (FInputText f in objList)
            {
                if (string.IsNullOrEmpty(f.Value))
                {
                    MessagingCenter.Send(new FMessage(0, errorCode, f.Title), FChannel.ALERT_BY_MESSAGE);
                    return false;
                }
            }
            return true;
        }

        protected virtual async void OnSubmit(object sender, EventArgs e)
        {
            if (FSetting.AppMode == FAppMode.FBO && string.IsNullOrEmpty(UsernameInput.Value))
            {
                MessagingCenter.Send(new FMessage(0, 606, UsernameInput.Title), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (!CheckInputEmpty(606, EmailInput, CapchaInput))
            {
                await SetBusy(false);
                return;
            }

            if (!EmailInput.Value.IsEmail())
            {
                MessagingCenter.Send(new FMessage(0, 606, EmailInput.Title), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            await SetBusy(true);
            var message = await FServices.Recovery(CapchaInput.Value, EmailInput.Value, UsernameInput.Value);
            await SetBusy(false);

            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return;
            }

            await Navigation.PopAsync(true);
            MessagingCenter.Send(new FMessage(0, 953, ""), FChannel.ALERT_BY_MESSAGE);
        }

        private async void CapchaLoading(object sender, EventArgs e)
        {
            try
            {
                await SetBusy(true);
                var message = await FServices.Capcha();
                await SetBusy(false);
                Captcha.DownloadingIsVisible = false;
                if (message.Success != 1)
                {
                    Captcha.ImageSource = FIcons.AlertRemoveOutline.ToFontImageSource(FSetting.WarningColor, FSetting.SizeIconShow);
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                Captcha.ImageSource = message.Message.ToImageSourceFromBase64();
            }
            catch
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
            }
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        private async void BuildGuide()
        {
            try
            {
                await Task.Delay(1);
                var message = await FServices.ExecuteExtension("600", new FExtensionParam { Key = "001" });
                if (!message.OK)
                    return;
                var xmlContent = message.Message;
                GuideXML = new FXml(xmlContent);
                Guide = GuideXML.GetGuideContent();
                Device.BeginInvokeOnMainThread(GuideUI);
            }
            catch { }
        }

        private void GuideUI()
        {
            if (Guide == null)
                return;
            GuideButton = new ToolbarItem();
            GuideButton.Clicked += OnViewGuide;
            GuideButton.IconImageSource = Guide.Icon?.ToImageSource(Color.FromHex(Guide.Color), FSetting.SizeIconToolbar);
            ToolbarItems.Add(GuideButton);
        }

        private async void OnViewGuide(object sender, EventArgs e)
        {
            try
            {
                await SetBusy(true);
                var view = new WebView();
                view.Source = new HtmlWebViewSource { Html = FHtml.ReplaceHtml(Guide.Body) };

                var page = new FPage(false, false);
                page.Content = view;
                page.Title = Guide.Title;

                await Navigation.PushAsync(page, true);
                await SetBusy(false);
            }
            catch
            {
                await SetBusy(false);
            }
        }
    }
}