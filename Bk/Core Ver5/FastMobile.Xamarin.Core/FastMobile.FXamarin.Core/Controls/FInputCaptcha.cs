using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputCaptcha : FInput
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FInputCaptcha), "");
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FInputCaptcha));
        public static readonly BindableProperty ShowLoadingProperty = BindableProperty.Create("ShowLoading", typeof(bool), typeof(FInputCaptcha), true);

        public string Key { get; private set; }

        public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }

        public bool ShowLoading { get => (bool)GetValue(ShowLoadingProperty); set => SetValue(ShowLoadingProperty, value); }

        #region Public

        public FInputCaptcha() : base()
        {
            Type = FieldType.String;
            IsShowTitle = false;
        }

        public FInputCaptcha(FField field) : base(field)
        {
            Type = FieldType.String;
            IsShowTitle = false;
        }

        public async Task<bool> Valid()
        {
            try
            {
                var message = await FServices.ValidateCaptchaByKey(Key, Value, "0");
                await OnLoading();
                OnCompleteValue(this, EventArgs.Empty);
                if (message.Success == 1)
                {
                    if (message.Message == "0")
                    {
                        MessagingCenter.Send(new FMessage(0, 955, ""), FChannel.ALERT_BY_MESSAGE);
                        return false;
                    }
                    return true;
                }
                MessagingCenter.Send(new FMessage(0, message.Code, ""), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
            catch (Exception ex)
            {
                await OnLoading();
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
        }

        public override void InitValue(bool isRefresh = true)
        {
            base.InitValue(isRefresh);
            Key = $"FastMobile.FXamarin.Core.FInputCapcha.{Root.Controller}";
            _ = OnLoading();
        }

        public override void Clear(bool isCompleted = false)
        {
            base.Clear(isCompleted);
            InitValue(false);
        }

        #endregion Public

        #region Protected

        protected override object ReturnValue(int mode)
        {
            return Value;
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = value[0].TrimEnd(' ');
            base.SetInput(value, isCompleted);
        }

        protected override void OnChangeValue(object sender, EventArgs e)
        {
            base.OnChangeValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override void OnCompleteValue(object sender, EventArgs e)
        {
            base.OnCompleteValue(sender, new FInputChangeValueEventArgs(Value));
        }

        protected override View SetContentView()
        {
            var c = new FCapchaImage();
            c.ReloadText = FIcons.Reload;
            c.ReloadColor = FSetting.PrimaryColor;
            c.ReloadFontFamily = FSetting.FontIcon;
            c.ReloadFontSize = FSetting.SizeButtonIcon;
            c.VerticalOptions = LayoutOptions.CenterAndExpand;
            c.SetBinding(FCapchaImage.ImageSourceProperty, ImageSourceProperty.PropertyName);
            c.SetBinding(FCapchaImage.DownloadingIsVisibleProperty, ShowLoadingProperty.PropertyName);
            c.Loaded += OnLoaded;
            return c;
        }

        #endregion Protected

        #region Private

        private async Task OnLoading()
        {
            try
            {
                var message = await FServices.CaptchaByKey(Key);
                ShowLoading = false;
                if (message.Success != 1)
                {
                    ImageSource = FIcons.AlertRemoveOutline.ToFontImageSource(FSetting.WarningColor, FSetting.SizeIconShow);
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }
                ImageSource = message.Message.ToImageSourceFromBase64();
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async void OnLoaded(object sender, EventArgs e)
        {
            await Root.SetBusy(true);
            OnCompleteValue(sender, e);
            await OnLoading();
            await Root.SetBusy(false);
        }

        #endregion Private
    }
}