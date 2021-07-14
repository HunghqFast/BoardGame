using FastMobile.FXamarin.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public partial class CPageLogin : FPageLogin
    {
        protected FPageProfile Profile { get; private set; }

        public CPageLogin(bool showBio) : base(false, hasBio: FSetting.CanUseMachineSecurity)
        {
            //ShowBio = showBio;
            BusyColor = Color.White;
            BusyBackgroundOpacity = 0.3f;
            Form.Logo.Source = ImageSource.FromFile("logoLogin");
            Form.Logo.WidthRequest = FSetting.AppMode == FAppMode.FBO ? 150 : 120;
            Form.Submiter.Clicked += OnLogin;
            Form.Recoverier.Clicked += OnReset;
            Form.BioParent.IsVisible = FSetting.CanUseMachineSecurity;
            Form.Biometric.Clicked += OnBiometric;
            Form.Biometric.Source = FIcons.Fingerprint.ToFontImageSource(Color.White);
            Form.Biometric.Padding = new Thickness(5);
            Form.BioParent.BorderColor = Color.White;
            Form.BioParent.BackgroundColor = Color.Transparent;
        }

        public static void AfterLoginSuccess()
        {
        }

        //public override void Init()
        //{
        //    base.Init();
        //    Biometric();
        //}

        //Init when timeout

        public override FPage Init(bool setInit)
        {
            FSetting.ClearKey();
            base.Init(setInit);
            Biometric();
            return this;
        }

        protected override void OnLanguageChanged(object sender, EventArgs e)
        {
            base.OnLanguageChanged(sender, e);
            Profile?.Update(FSetting.V);
        }

        protected override async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            base.OnProfileButtonClicked(sender, e);
            if (Profile is null)
            {
                Profile = new FPageProfile();
                Profile.ItemTapped += OnProfileChanged;
                await Navigation.PushAsync(Profile, true);
                InitPrimaryProfile();
                Profile.Init();
                ProfileButton.BindingContext = Profile;
                ProfileButton.SetBinding(Button.TextProperty, FPageProfile.CurrentServiceNameProperty.PropertyName);
                return;
            }

            if (!Navigation.NavigationStack.Contains(Profile))
                await Navigation.PushAsync(Profile, true);
        }

        protected virtual void InitPrimaryProfile()
        {
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            await SetBusy(true);
            if (string.IsNullOrEmpty(Form.UsernameInput.InputView.Text))
            {
                MessagingCenter.Send(new FMessage(0, 606, Form.UsernameInput.Hint), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                return;
            }

            if (string.IsNullOrEmpty(Form.PasswordInput.InputView.Text))
            {
                MessagingCenter.Send(new FMessage(0, 606, Form.PasswordInput.Hint), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                return;
            }

            var message = await FServices.GetKeys(Form.UsernameInput.InputView.Text);
            if (message.Success != 1)
            {
                await SetBusy(false);
                if (message.Code == 204)
                {
                    FAlertHelper.Show(message.Code.ToString());
                    return;
                }

                if (message.Code == 205)
                {
                    if (await FAlertHelper.Confirm("205", FText.CommandUpdate, FText.CommandCancel))
                        await FInterface.IFVersion.OpenAppInStore();
                    return;
                }
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                return;
            }

            var result = message.Message.Split((char)254);
            if (result.Length < 4 || result[0] != result[2])
            {
                await SetBusy(false);
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (FString.ServiceInternal == "1")
            {
                await CheckHash(FInterface.IFFirebase?.GetToken(), result[1], result[3]);
                return;
            }
            if (string.IsNullOrEmpty(result[3]))
            {
                await CheckHash("", result[1], result[3]);
                return;
            }
            await CheckHash(await FInterface.IFFirebase?.GetTokenAsync(result[3], 30), result[1], result[3]);
        }

        private async Task CheckHash(string token, string key, string senderID)
        {
            var message = await FServices.CheckHash(Form.UsernameInput.InputView.Text, Form.PasswordInput.InputView.Text, key, token ?? "");
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                return;
            }

            var ds = message.Message.ToDataSet();
            SetUserInfo(ds.Tables[0], Form.PasswordInput.InputView.Text);
            if (FString.ServiceInternal == "1") InitFirebase(token, ds);
            Form.SaveInfo();
            FString.UpdatePassword((Form.PasswordInput.InputView.Text.MD5() + key.MD5() + FSetting.DeviceID).MD5());
            FString.SenderID = senderID;
            if (ds.Tables[0].Columns.Contains("pwdchange") && Convert.ToBoolean(Convert.ToInt32(ds.Tables[0].Rows[0]["pwdchange"])))
            {
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new FPageChangePassword(true, false), true));
                await SetBusy(false);
                return;
            }
            MessagingCenter.Send(new FMessage(), FChannel.SET_INDEX);
        }

        private async void OnReset(object sender, EventArgs e)
        {
            var recovery = new FPageResetPassword(false);
            if (FSetting.AppMode == FAppMode.FBO)
                recovery.UsernameInput.Value = Form.UsernameInput.InputView.Text?.ToUpper();
            else if (Form.UsernameInput.InputView.Text.IsEmail())
                recovery.EmailInput.Value = Form.UsernameInput.InputView.Text;
            await Navigation.PushAsync(recovery, true);
            recovery.Init();
        }

        private void OnProfileChanged(object sender, IFDataEvent e)
        {
            FSetting.ClearKey();
        }

        private void OnBiometric(object sender, EventArgs e)
        {
            if (Check(true)) ShowBiometric(false);
        }

        private void Biometric()
        {
            if (Check(false)) Device.BeginInvokeOnMainThread(() => ShowBiometric(true));
        }

        private async void ShowBiometric(bool fromStart)
        {
            try
            {
                await SetBusy(true);
                var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FSetting.SystemLanguageIsV ? "Hủy" : "Cancel");
                var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);
                //CConfigure.UpdateTimeSleep();

                if (authResult.Status == FFingerprintAuthenticationResultStatus.TooManyAttempts && !fromStart)
                {
                    await SetBusy(false);
                    MessagingCenter.Send(FMessage.FromFail(1254), FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                if (!authResult.IsAuthenticated)
                {
                    await SetBusy(false);
                    return;
                }

                if (!await Update())
                {
                    await SetBusy(false);
                    return;
                }

                MessagingCenter.Send(new FMessage(), FChannel.SET_INDEX);
            }
            catch (Exception ex)
            {
                await SetBusy(false);
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async Task<bool> Update()
        {
            try
            {
                if (FPageProfile.IsOldProfile)
                {
                    var ticketMessage = await FServices.UpdateTicket(checkTime: true);
                    if (ticketMessage.OK) return true;
                }

                var keyMessage = await FServices.GetKeyExt();
                if (!keyMessage.OK)
                {
                    MessagingCenter.Send(keyMessage, FChannel.ALERT_BY_MESSAGE);
                    return false;
                }

                var keys = keyMessage.Message.Split((char)254);
                if (keys.Length != 2)
                {
                    MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                    return false;
                }

                var (Message, Token) = await FServices.CheckHashExt(true, keys[0], keys[1]);

                if (!Message.OK)
                {
                    MessagingCenter.Send(Message, FChannel.ALERT_BY_MESSAGE);
                    return false;
                }

                var ds = Message.Message.ToDataSet();
                SetUserInfo(ds.Tables[0], keys[0]);
                if (FString.ServiceInternal == "1") InitFirebase(Token, ds);
                FString.SenderID = keys[1];
                if (ds.Tables[0].Columns.Contains("pwdchange") && Convert.ToBoolean(Convert.ToInt32(ds.Tables[0].Rows[0]["pwdchange"])))
                {
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new FPageChangePassword(true, false), true));
                    await SetBusy(false);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
        }

        private bool Check(bool alert)
        {
            if (!CConfigure.UseLocalAuthen || !FInterface.IFFingerprint.IsAvailable(false))
            {
                if (alert) MessagingCenter.Send(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            if (string.IsNullOrEmpty(Form.UsernameInput.InputView.Text) || Form.UsernameInput.InputView.Text.ToLower() != FString.Username.ToLower())
            {
                if (alert) MessagingCenter.Send(new FMessage(0, 606, Form.UsernameInput.Hint), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            if (FUtility.IsTimeOut && (string.IsNullOrEmpty(FString.UserID) || string.IsNullOrEmpty(FString.Password) || !FUtility.CurrentIsUrl))
            {
                if (alert) MessagingCenter.Send(FMessage.FromFail(1253), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            return true;
        }
    }
}