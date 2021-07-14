using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageChangePassword : FPageInput
    {
        private List<string> listCheck;
        private string rKey, pwdtype, pwdUseOld, networkKey;
        private readonly bool isLogin;
        private readonly Regex pwdReg5 = new Regex("((?=.*\\d)(?=.*[a-z]).{8,})"), pwdReg6 = new Regex("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,})"), pwdReg7 = new Regex("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).{8,})");
        public readonly FInputText CurrentInput, NewInput, ReNewInput;

        public FPageChangePassword(bool isLoginFirst, bool isHasPullToRefresh, bool enableScrolling = false) : base(isHasPullToRefresh, enableScrolling)
        {
            isLogin = isLoginFirst;
            CurrentInput = new FInputText();
            NewInput = new FInputText();
            ReNewInput = new FInputText();
            Submiter.Clicked += OnSubmit;
            Closer.Clicked += OnClose;
            InitForm();
            Update(FSetting.V);
            Base();
        }

        public override void Update(bool v)
        {
            Title = FText.ChangePassword;
            CurrentInput.Title = FText.ConfirmPassword;
            NewInput.Title = FText.PasswordHint;
            ReNewInput.Title = FText.ReNewPasswordHint;
            Submiter.Text = FText.Accept;
            Closer.Text = FText.Cancel;
        }

        private void InitForm()
        {
            CurrentInput.Rendering();
            NewInput.Rendering();
            ReNewInput.Rendering();

            CurrentInput.NotAllowsNull = NewInput.NotAllowsNull = ReNewInput.NotAllowsNull = true;
            CurrentInput.IsPassword = NewInput.IsPassword = ReNewInput.IsPassword = true;

            Form.RowSpacing = 0;
            Form.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });
            Form.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Form.RowDefinitions.Add(new RowDefinition { Height = 1 });

            Form.Children.Add(CurrentInput, 0, 0);
            Form.Children.Add(new FLine(), 0, 1);
            Form.Children.Add(NewInput, 0, 2);
            Form.Children.Add(new FLine(), 0, 3);
            Form.Children.Add(ReNewInput, 0, 4);
            Form.Children.Add(new FLine(), 0, 5);
        }

        private async void Base()
        {
            await SetBusy(true);
            await RequestFirst();
            await SetBusy(false);
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

        private async void OnSubmit(object sender, EventArgs e)
        {
            CurrentInput.UnFocusInput();
            NewInput.UnFocusInput();
            ReNewInput.UnFocusInput();

            if (string.IsNullOrEmpty(rKey) && string.IsNullOrEmpty(pwdtype) && string.IsNullOrEmpty(pwdUseOld))
            {
                await RequestFirst();
            }

            if (!CheckInputEmpty(606, CurrentInput, NewInput, ReNewInput))
                return;

            if (NewInput.Value == CurrentInput.Value && pwdUseOld != "1")
            {
                MessagingCenter.Send(new FMessage(0, 1000, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (listCheck.Find((x) => x == NewInput.Value) != null)
            {
                MessagingCenter.Send(new FMessage(0, 1006, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (NewInput.Value != ReNewInput.Value)
            {
                MessagingCenter.Send(new FMessage(0, 1007, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            switch (pwdtype)
            {
                case "2":
                    if (NewInput.Value.Length < 8)
                    {
                        MessagingCenter.Send(new FMessage(0, 1002, ""), FChannel.ALERT_BY_MESSAGE);
                        return;
                    }
                    break;

                case "3":
                    if (!this.pwdReg5.IsMatch(NewInput.Value))
                    {
                        MessagingCenter.Send(new FMessage(0, 1003, ""), FChannel.ALERT_BY_MESSAGE);
                        return;
                    }
                    break;

                case "4":
                    if (!this.pwdReg6.IsMatch(NewInput.Value))
                    {
                        MessagingCenter.Send(new FMessage(0, 1004, ""), FChannel.ALERT_BY_MESSAGE);
                        return;
                    }
                    break;

                case "5":
                    if (!this.pwdReg7.IsMatch(NewInput.Value))
                    {
                        MessagingCenter.Send(new FMessage(0, 1005, ""), FChannel.ALERT_BY_MESSAGE);
                        return;
                    }
                    break;

                default:
                    break;
            }
            await SetBusy(true);
            await GetResult();
            await SetBusy(false);
        }

        private async Task RequestFirst()
        {
            networkKey = FSetting.NetWorkKey;
            try
            {
                var message = await GetKeyUser();
                if (message.Success == 1)
                {
                    var data = message.ToDataSet();
                    rKey = data.Tables[0].Rows[0]["rkey"].ToString().Trim();
                    pwdtype = data.Tables[0].Rows[0]["pwd_type"].ToString().Trim();
                    pwdUseOld = data.Tables[0].Rows[0]["pwd_useold_yn"].ToString().Trim();
                    listCheck = new List<string>(data.Tables[0].Rows[0]["listCheck"].ToString().Trim().Split(","));
                }
                else
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
            }
            catch
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
            }
            finally
            {
                RemoveKey();
            }
        }

        private async Task GetResult()
        {
            if (isLogin) FSetting.NetWorkKey = networkKey;
            var message = await FServices.ExecuteCommand("ChangePassword", "System", new DataSet().AddTable(new DataTable().AddRowValue("dvcurrentPwd", CurrentInput.Value.MD5()).AddRowValue(0, "dvnewPwd", NewInput.Value.MD5()).AddRowValue(0, "username", FString.Username)), "0", FExtensionParam.New(true, FText.AttributeString("V", "ChangePassword"), FText.AttributeString("E", "ChangePassword"), FAction.ChangePassword), true);
            try
            {
                if (message.Success != 1)
                {
                    RemoveKey();
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }
                var data = message.ToDataSet();
                var res = data?.Tables[0].Rows[0]["result"].ToString().Trim();
                if (res.Equals("1") || res.Equals("2"))
                {
                    RemoveKey();
                    MessagingCenter.Send(new FMessage(0, res.Equals("1") ? 1001 : 1000, ""), FChannel.ALERT_BY_MESSAGE);
                    return;
                }
                if (FSetting.UseLocalAuthen) UpdateBio(NewInput.Value);
                if (isLogin)
                {
                    MessagingCenter.Send(new FMessage(), FChannel.SET_INDEX);
                    MessagingCenter.Send(new FMessage(0, 1009, ""), FChannel.ALERT_BY_MESSAGE);
                    return;
                }
                await Navigation.PopAsync();
                MessagingCenter.Send(new FMessage(0, 1009, ""), FChannel.ALERT_BY_MESSAGE);
            }
            catch
            {
                RemoveKey();
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private void RemoveKey()
        {
            if (isLogin)
                FSetting.ClearKey();
        }

        private Task<FMessage> GetKeyUser()
        {
            return FServices.ExecuteCommand("ChangePasswordGetRKey", "System", null, "0", null, true, null, string.Empty);
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        private async void UpdateBio(string cur)
        {
            var key = FUtility.GetRandomString();
            var pwd = (cur.MD5() + key + FSetting.DeviceID).MD5();
            var message = await FServices.SaveKey(key, pwd, FAction.UpdateBiometric);
            if (message.OK) FString.UpdatePassword(pwd);
        }
    }
}