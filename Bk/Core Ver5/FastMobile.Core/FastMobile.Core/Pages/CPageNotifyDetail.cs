using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageNotifyDetail : FPageWebView
    {
        public const string ApprovalGroup = "A";
        public Action<FItemNotify> InvokeWhenOpenedNext;
        private readonly FNotifyGroupModel Model;
        private readonly IFBadge Badge;

        public CPageNotifyDetail(FWebViewType type, string action, string controller, string notifyID, DataSet dataRequest, string method, bool isHasPullToRefresh, FNotifyGroupModel model, Action<FItemNotify> invoke, IFBadge badge = null) : base(type, action, controller, notifyID, dataRequest, method, isHasPullToRefresh)
        {
            Model = model;
            InvokeWhenOpenedNext = invoke;
            Badge = badge;
        }

        public override void Init()
        {
            base.Init();
            UpdateVisibleNextBack();
        }

        protected override async void OnAcceptClicked(object sender, EventArgs e)
        {
            if (Result.Tables.Count < 2)
                return;
            await Execute("905", "1");
        }

        protected override async void OnCancelClicked(object sender, EventArgs e)
        {
            base.OnCancelClicked(sender, e);
            if (Result.Tables.Count < 2)
                return;
            await Execute("906", "2");
        }

        protected override async void OnClosedClicked(object sender, EventArgs e)
        {
            base.OnClosedClicked(sender, e);
            if (Navigation.NavigationStack.Count == 1)
                return;
            await Navigation.PopAsync(true);
        }

        protected override void UpdateBadge()
        {
            if (Result == null || Result.Tables.Count < 1 || !Result.Tables[1].Columns.Contains("not_seen") || Result.Tables[1].Rows.Count < 1)
                return;
            Badge.BadgeValue = Result.Tables[1].Rows[0]["not_seen"].ToString();
        }

        protected override async void OnNextClicked(object sender, EventArgs e)
        {
            base.OnNextClicked(sender, e);

            if (Model == null)
                return;

            var index = Model.DataSource.IndexOf(Model.DataSource.ToList().Find(x => x.Code == ID)) + 1;
            if (Model.DataSource.Count <= index)
            {
                if (Model.CanLoadMore())
                    await Model.Load();
                if (Model.DataSource.Count <= index)
                    return;
            }

            if (Model.DataSource.ElementAt(index) is not FItemNotify item)
                return;

            PushDetail(item, this);
        }

        protected override void OnBackClicked(object sender, EventArgs e)
        {
            base.OnBackClicked(sender, e);

            if (Model == null)
                return;

            var index = Model.DataSource.IndexOf(Model.DataSource.ToList().Find(x => x.Code == ID)) - 1;
            if (index < 0)
                return;

            if (Model.DataSource.ElementAt(index) is not FItemNotify item)
                return;

            PushDetail(item, this);
        }

        private async void Approval(string type, string comment)
        {
            await SetBusy(true);
            var message = await SentApprove(type, comment);
            OnClearCacheComment();
            await SetBusy(false);
            if (message == null)
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            try
            {
                if (message.Success == "1")
                {
                    await WhenSuccess();
                    return;
                }

                await WhenFailed(message.Message);
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async Task<FInvokeResult> SentApprove(string type, string comment)
        {
            return await new FInvoke().OnApprove(type, R1["service_url"].ToString(), R1["namespace"].ToString(), R1["method"].ToString(), R0["notifyid"].ToString(), R0["ref_code"].ToString(), R0["priority"].ToString(), comment, R0.ReplaceData(R1["ext"].ToString()));
        }

        private async Task WhenSuccess()
        {
            if (Model == null || Model.DataSource.ToList().Find(x => x.Code == ID) is not FItemNotify item)
            {
                await Navigation.PopAsync(true);
                return;
            }

            var nextIndex = Model.DataSource.IndexOf(item) + 1;
            if (DeleteWhenSuccess && await SentRemove() is FMessage m && m.Success == 1)
            {
                Model.MinusServerCount();
                Model.DataSource.Remove(item);
                nextIndex--;
            }

            if (!await OnNext(NextWhenSuccess, nextIndex))
                await Navigation.PopAsync(true);
            if (MessageWhenSuccess)
                MessagingCenter.Send(new FMessage(0, 901, ""), FChannel.ALERT_BY_MESSAGE);
        }

        private async Task WhenFailed(string message)
        {
            if (Model == null || Model.DataSource.ToList().Find(x => x.Code == ID) is not FItemNotify item)
            {
                await Navigation.PopAsync(true);
                return;
            }

            var nextIndex = Model.DataSource.IndexOf(item) + 1;
            if (DeleteWhenFailed && await SentRemove() is FMessage m && m.Success == 1)
            {
                Model.MinusServerCount();
                Model.DataSource.Remove(item);
                nextIndex--;
            }
            if (!await OnNext(NextWhenFailed, nextIndex))
                await Navigation.PopAsync(true);
            if (MessageWhenFailed)
                MessagingCenter.Send(string.IsNullOrEmpty(message) ? new FMessage(0, 902, "") : new FMessage(0, 0, message), FChannel.ALERT_BY_MESSAGE);
        }

        private async Task<bool> OnNext(bool next, int nextIdex)
        {
            if (Model == null || !next)
                return false;

            if (Model.DataSource.Count <= nextIdex)
            {
                if (Model.CanLoadMore())
                    await Model.Load();
                if (Model.DataSource.Count <= nextIdex)
                    return false;
            }

            if (Model.DataSource.ElementAt(nextIdex) is not FItemNotify item)
                return false;

            PushDetail(item, this);
            return true;
        }

        private void PushDetail(FItemNotify item, FPage parent)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var page = new CPageNotifyDetail(item.Type, Action, Controller, item.Code, new DataSet().AddTable(new DataTable().AddRowValue("idNoti", item.Code)), Method, false, Model, InvokeWhenOpenedNext, Badge);
                await Navigation.PushAsync(page, false);
                await page.OnLoaded();
                InvokeWhenOpenedNext?.Invoke(item);
                Navigation.RemovePage(parent);
            });
        }

        private Task<FMessage> SentRemove()
        {
            if (string.IsNullOrEmpty(ID))
                return null;
            return FServices.ExecuteCommand("RemoveNotify", "System", new DataSet().AddTable(new DataTable().AddRowValue("code", ID)), "0", null, false);
        }

        private async void UpdateVisibleNextBack()
        {
            if (Model == null)
                return;

            if (Model.DataSource.ToList().Find(x => x.Code == ID) is not FItemNotify item)
                return;

            var index = Model.DataSource.IndexOf(item) + 1;
            if (index == 1)
                Back.IsEnabled = false;

            if (Model.DataSource.Count == index)
            {
                if (Model.CanLoadMore())
                    await Model.Load();
                if (Model.DataSource.Count == index)
                    Next.IsEnabled = false;
            }
        }

        private async Task Execute(string code, string type)
        {
            if (!await FAlertHelper.Confirm(code))
                return;

            if (HasBio)
            {
                var result = await BioPassword(BioNative, true);
                if (result.OK) Approval(type, CommnetText);
                return;
            }

            Approval(type, CommnetText);
        }

        private async Task<(bool OK, string Secret)> BioPassword(bool bioNative, bool alert = true)
        {
            if (!FSetting.UseLocalAuthen)
                return await FServices.ConfirmPassword(FText.ConfirmPassword, alert);

            bioNative = !FSetting.IsAndroid && bioNative;
            if (!FInterface.IFFingerprint.IsAvailable(bioNative))
                return await FServices.ConfirmPassword(FText.ConfirmPassword, alert);

            var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FText.AttributeString(FSetting.SystemLanguage, "Cancel"));
            config.AllowAlternativeAuthentication = bioNative;
            var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

            if (authResult.IsAuthenticated)
                return (true, FString.Password);

            return await FServices.ConfirmPassword(FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V) + FText.AuthenConfirmAppend, alert);
        }
    }
}