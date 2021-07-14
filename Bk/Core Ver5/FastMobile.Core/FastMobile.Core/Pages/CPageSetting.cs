using FastMobile.FXamarin.Core;
using Syncfusion.XForms.Buttons;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageSetting : FPageSetting
    {
        private readonly FInputSwitch CloseScreen, LocalAuthen;
        private readonly SfButton CheckUpdate;
        private readonly Grid DeviceSetting, ReportSetting;

        public CPageSetting() : base()
        {
            CloseScreen = new FInputSwitch();
            LocalAuthen = new FInputSwitch();
            CheckUpdate = new SfButton();
            ReportSetting = RecordView(FText.ReportSetting, 55);
            DeviceSetting = RecordView(FText.ApplicationSetting, 55);
            Content = new StackLayout { Margin = 0, Padding = 0, Spacing = 0, Children = { DeviceSetting, ReportSetting } };
        }

        public override FPage Init(bool set)
        {
            InitCloseScreen();
            InitCheckUpdate();
            InitLocalAuthen();
            return base.Init(set);
        }

        private void InitCheckUpdate()
        {
            CheckUpdate.Text = FText.CheckUpdate;
            CheckUpdate.BackgroundColor = FSetting.BackgroundMain;
            CheckUpdate.TextColor = FSetting.TextColorContent;
            CheckUpdate.FontFamily = FSetting.FontText;
            CheckUpdate.FontSize = FSetting.FontSizeLabelContent;
            CheckUpdate.BorderWidth = 0;
            CheckUpdate.HorizontalOptions = LayoutOptions.Fill;
            CheckUpdate.WidthRequest = FSetting.ScreenWidth;
            CheckUpdate.HorizontalTextAlignment = TextAlignment.Start;
            CheckUpdate.CornerRadius = 0;
            CheckUpdate.HeightRequest = 55;
            CheckUpdate.Clicked += OnCheckUpdate;
            AddView(DeviceSetting, CheckUpdate, 55);
        }

        private void InitLocalAuthen()
        {
            LocalAuthen.Content = FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V);
            LocalAuthen.IsShowTitle = false;
            LocalAuthen.Value = CConfigure.UseLocalAuthen;
            LocalAuthen.Rendering();
            LocalAuthen.VerticalOptions = LayoutOptions.CenterAndExpand;
            LocalAuthen.IsReadOnly = FSetting.CanUseMachineSecurity;
            LocalAuthen.Color = FSetting.CanUseMachineSecurity ? FSetting.TextColorContent : FSetting.DisableColor;
            AddView(DeviceSetting, LocalAuthen, 55, false);
            if (FSetting.CanUseMachineSecurity) LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
        }

        private void InitCloseScreen()
        {
            CloseScreen.Content = FText.CloseScreen;
            CloseScreen.IsShowTitle = false;
            CloseScreen.Value = CConfigure.AutocompleteLookup;
            CloseScreen.Rendering();
            CloseScreen.ChangeValue += OnReportSettingChangedValue;
            CloseScreen.VerticalOptions = LayoutOptions.CenterAndExpand;
            AddView(ReportSetting, CloseScreen, 55);
        }

        private void AddView(Grid s, View v, GridLength height, bool lineTop = true, bool lineBottom = true)
        {
            if (lineTop)
            {
                s.RowDefinitions.Add(new RowDefinition { Height = 1 });
                s.Children.Add(new FLine(), 0, s.RowDefinitions.Count - 1);
            }

            s.RowDefinitions.Add(new RowDefinition { Height = height });
            s.Children.Add(v, 0, s.RowDefinitions.Count - 1);

            if (lineBottom)
            {
                s.RowDefinitions.Add(new RowDefinition { Height = 1 });
                s.Children.Add(new FLine(), 0, s.RowDefinitions.Count - 1);
            }
        }

        private void OnReportSettingChangedValue(object sender, FInputChangeValueEventArgs e)
        {
            CConfigure.AutocompleteLookup = CloseScreen.Value;
        }

        private async void OnLocalAuthenSettingChangedValue(object sender, FInputChangeValueEventArgs e)
        {
            try
            {
                if (!(bool)e.Value)
                {
                    CConfigure.UpdateUseLocalAuthen(false);
                    FString.UpdatePassword("");
                    return;
                }

                LocalAuthen.ChangeValue -= OnLocalAuthenSettingChangedValue;
                LocalAuthen.Value = false;
                await SetBusy(true);
                await Task.Delay(200);

                if (!FInterface.IFFingerprint.IsAvailable(false))
                {
                    LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
                    await SetBusy(false);
                    MessagingCenter.Send(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)), FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                if (!await SavePassword())
                {
                    LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
                    await SetBusy(false);
                    return;
                }

                LocalAuthen.Value = true;
                CConfigure.UpdateUseLocalAuthen(true);
                await Task.Delay(200);
                LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
                await SetBusy(false);
            }
            catch (Exception ex)
            {
                await SetBusy(false);
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async void OnCheckUpdate(object sender, EventArgs e)
        {
            await SetBusy(true);
            if (await FInterface.IFVersion?.IsUsingLatestVersion())
            {
                MessagingCenter.Send(new FMessage(0, 206, ""), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                return;
            }
            await SetBusy(false);
            if (await FAlertHelper.Confirm("207"))
            {
                await SetBusy(true);
                await FInterface.IFVersion?.OpenAppInStore();
                await SetBusy(false);
            }
        }

        private async Task<bool> SavePassword()
        {
            var (OK, Message) = await new FAlertEntry().ShowTextbox(false, true, FText.ConfirmPassword);
            if (!OK) return false;

            if (string.IsNullOrEmpty(Message))
            {
                MessagingCenter.Send(FMessage.FromFail(606, FText.ConfirmPassword), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            try
            {
                var key = FUtility.GetRandomString();
                var pwd = (Message.MD5() + key + FSetting.DeviceID).MD5();
                var saveMessage = await FServices.SaveKey(key, pwd, FAction.TurnOnBiometric);

                if (!saveMessage.OK)
                {
                    MessagingCenter.Send(saveMessage, FChannel.ALERT_BY_MESSAGE);
                    return false;
                }

                FString.UpdatePassword(pwd);
                return true;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
        }

        #region TurnOffRequire

        //private async void OnLocalAuthenSettingChangedValue(object sender, FInputChangeValueEventArgs e)
        //{
        //    try
        //    {
        //        var turnOn = (bool)e.Value;

        //        await SetBusy(true);
        //        LocalAuthen.ChangeValue -= OnLocalAuthenSettingChangedValue;
        //        LocalAuthen.Value = !turnOn;
        //        await Task.Delay(200);

        //        var hasLocal = FInterface.IFFingerprint.IsAvailable(false);
        //        if (turnOn && !hasLocal)
        //        {
        //            LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
        //            await SetBusy(false);
        //            MessagingCenter.Send(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)), FChannel.ALERT_BY_MESSAGE);
        //            return;
        //        }

        //        if (turnOn && await SavePassword())
        //        {
        //            Completed();
        //            return;
        //        }

        //        if (turnOn)
        //        {
        //            LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
        //            await SetBusy(false);
        //            return;
        //        }

        //        if (!turnOn && !hasLocal)
        //        {
        //            Completed();
        //            return;
        //        }

        //        CConfigure.UpdateTimeSleep();
        //        var config = new FAuthenticationRequestConfiguration(FAlert.Title, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FSetting.SystemLanguageIsV ? "Hủy" : "Cancel");
        //        var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);
        //        CConfigure.UpdateTimeSleep();
        //        if (authResult.IsAuthenticated)
        //        {
        //            Completed();
        //            return;
        //        }

        //        LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
        //        if (authResult.Status == FFingerprintAuthenticationResultStatus.Canceled)
        //        {
        //            await SetBusy(false);
        //            return;
        //        }

        //        if (authResult.Status == FFingerprintAuthenticationResultStatus.TooManyAttempts)
        //        {
        //            await SetBusy(false);
        //            MessagingCenter.Send(FMessage.FromFail(1254), FChannel.ALERT_BY_MESSAGE);
        //            return;
        //        }
        //        await SetBusy(false);

        //        async void Completed()
        //        {
        //            CConfigure.UpdateUseLocalAuthen(turnOn);
        //            LocalAuthen.Value = turnOn;
        //            await Task.Delay(200);
        //            LocalAuthen.ChangeValue += OnLocalAuthenSettingChangedValue;
        //            await SetBusy(false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await SetBusy(false);
        //        MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
        //    }
        //}

        #endregion TurnOffRequire
    }
}