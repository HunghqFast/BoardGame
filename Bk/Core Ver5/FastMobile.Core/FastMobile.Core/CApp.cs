using FastMobile.FXamarin.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace FastMobile.Core
{
    public partial class CApp : FApplication, IFNotificationManager
    {
        public bool CurrentIsTabbedPage => (MainPage is NavigationPage) ? (MainPage as NavigationPage).RootPage is TabbedPage : MainPage is TabbedPage;
        public FTabbedPage Menu { get; protected set; }

        public FNavigationPage Index { get; protected set; }

        public CApp() : base()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                FViewPage.ClearAll();
                FApprovalComment.ClearAll();
            }

            FAlertHelper.Init(this);
            Init();
            Subscribe();
            Base();
        }

        public void Base()
        {
            try
            {
                if (FUtility.CurrentIsUrl && !FUtility.IsTimeOut)
                {
                    //if (CConfigure.UseLocalAuthen) SetLogin();
                    //else await SetIndex();
                    SetIndex();
                    return;
                }
                ClearNotifyData();
                MainPage = GetLoginWhenTimeOut();
            }
            catch (Exception ex)
            {
                MainPage = GetLoginWhenTimeOut();
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        #region Interface

        public async Task ShowHome(Func<Task> afterInvoke)
        {
            await PageInits(afterInvoke, Dashboard, Report, Entry, Notify, Others);
        }

        public async Task ShowNotify()
        {
            await OpenDetail();
        }

        public async Task OpenDetail()
        {
            try
            {
                await Menu.Navigation.PopToRootAsync();
                if (IsOpenReport(NotifyAction))
                {
                    LoadReportByAction(NotifyAction);
                    return;
                }

                Menu.CurrentPage = Notify;
                await LoadById(NotifyID, NotifyGroup);
                var page = new CPageNotifyDetail(NotifyGroup == CPageNotifyDetail.ApprovalGroup ? FWebViewType.Approval : FWebViewType.Default, "DetailNotify", "System", NotifyID, new DataSet().AddTable(new DataTable().AddRowValue("idNoti", NotifyID)), "250", false, (Notify as CPageNotifyGroup).Approval is null ? null : (Notify as CPageNotifyGroup).Approval.Model, (Notify as CPageNotifyGroup).Approval is null ? null : (Notify as CPageNotifyGroup).Approval.InvokeWhenOpenedNext, Notify);
                await Menu.Navigation.PushAsync(page);
                await page.OnLoaded();
                ReadById(NotifyID, NotifyGroup);
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug)
                    MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        public void ClearNotifyData()
        {
            NotifyGroup = NotifyID = NotifyAction = "";
        }

        public virtual void ReadById(string id, string group)
        {
        }

        public virtual Task LoadById(string id, string group)
        {
            return Task.CompletedTask;
        }

        public bool IsOpenReport(string action)
        {
            if (string.IsNullOrWhiteSpace(action) || action.Length < 6)
                return false;
            return action.Substring(0, 6) == "REPORT";
        }

        public async void LoadReportByAction(string action)
        {
            try
            {
                if (string.IsNullOrEmpty(action))
                    return;
                var arr = action.Split(FText.NotifyActionCharacter);
                Menu.CurrentPage = Menu.Children[Convert.ToInt32(arr[1])];
                if (arr.Length <= 4)
                {
                    await FPageReport.SetReportByAction(Menu.CurrentPage, arr[3], arr[2]);
                    return;
                }
                await FPageReport.SetReportByAction(Menu.CurrentPage, arr[3], arr[2], resTable: arr[4]?.ReplaceSpamJson()?.JsonToTable(), openDetail: true);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        #endregion Interface

        public override void OnFinish()
        {
            FApprovalComment.ClearAll();
            FViewPage.ClearAll();
            base.OnFinish();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if ((FUtility.IsTimeOut || !FUtility.CurrentIsUrl) && !string.IsNullOrEmpty(FSetting.NetWorkKey))
                MainPage = GetLoginWhenTimeOut();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            ClearNotifyData();
            CConfigure.UpdateTimeSleep();
        }

        protected override void OnResume()
        {
            if ((FUtility.IsTimeOut || !FUtility.CurrentIsUrl) && CurrentIsTabbedPage)
            {
                //CConfigure.UpdateTimeSleep();
                MainPage = GetLoginWhenTimeOut();
                ClearNotifyData();
                base.OnResume();
                return;
            }

            //REMEMBER CHANGE TO 60s
            //if (CConfigure.UseLocalAuthen && CurrentIsTabbedPage && DateTime.Now > CConfigure.TimeSleep.AddSeconds(FSetting.IsDebug ? 10 : 60))
            //{
            //    ShowBiometric();
            //    base.OnResume();
            //    return;
            //}

            base.OnResume();
            //CConfigure.UpdateTimeSleep();
            if (!FSetting.IsAndroid && !string.IsNullOrEmpty(NotifyID) && CurrentIsTabbedPage)
                _ = OpenDetail();
        }

        protected virtual void Init()
        {
        }

        protected virtual void SetDashboard()
        {
            Dashboard = new CPageDashboards() { Title = FText.Dashboard.ToUpper() };
        }

        protected virtual void SetReport()
        {
            Report = new CPageReports("20") { Title = FText.Report.ToUpper() };
        }

        protected virtual void SetEntry()
        {
            Entry = new CPageReports("30") { Title = FText.Entry.ToUpper() };
        }

        protected virtual void SetNotify()
        {
            Notify = new CPageNotifyGroup() { Title = FText.Notify.ToUpper() };
        }

        protected virtual void SetOther()
        {
            Others = new CPageOther() { Title = FText.Others.ToUpper() };
        }

        //protected virtual void SetLogin(bool showBiometric = true)
        //{
        //    ClearNotifyData();
        //}

        //protected virtual void SetLoginWhenTimeOut()
        //{
        //    FSetting.ClearKey();
        //    ClearNotifyData();
        //}

        //protected virtual Page GetLogin(bool showBiometric = true)
        //{
        //    return default;
        //}

        protected virtual Page GetLoginWhenTimeOut()
        {
            return default;
        }

        protected override async void OnLocalNotificationTapped(FNotificationTappedEventArgs e)
        {
            try
            {
                if (e.Data.ToObject<Dictionary<string, string>>() is not Dictionary<string, string> dic)
                    return;

                if (dic.ContainsKey(FText.NotifyCodeKey) && dic.ContainsKey(FText.NotifyGroupKey))
                {
                    NotifyID = dic[FText.NotifyCodeKey];
                    NotifyGroup = dic[FText.NotifyGroupKey];
                    if (dic.ContainsKey(FText.NotifyActionKey))
                        NotifyAction = dic[FText.NotifyActionKey];
                    await OpenDetail();
                    return;
                }
            }
            catch { }
        }

        private void SetIndex()
        {
            try
            {
                Menu = new FTabbedPage();
                Index = new FNavigationPage(Menu);
                MainPage = Index;
                SetDashboard();
                SetReport();
                SetEntry();
                SetNotify();
                SetOther();
                InitMenu();
                FPageProfile.IsOldProfile = true;
                FUtility.RunAfter(Init, TimeSpan.FromMilliseconds(200));
                InitEvent();
                void Init()
                {
                    Device.BeginInvokeOnMainThread(async () => await ShowHome(string.IsNullOrEmpty(NotifyID) ? null : ShowNotify));
                }

                //#if DEBUG
                //                Menu.CurrentPage = Entry;
                //#endif
            }
            catch (Exception ex)
            {
                ShowAlert(new FMessage(ex.Message));
            }
        }

        private void InitEvent()
        {
            if (Notify == null)
                return;
            Notify.ReceivedBadgeForMenu += OnReceived;
        }

        private void OnReceived(object sender, FBadgeMenuArgs e)
        {
            Report?.ItemsSource?.ToList().Find(x => x.Controller == e.Controller)?.SetBadgeText(e.Badge);
            Entry?.ItemsSource?.ToList().Find(x => x.Controller == e.Controller)?.SetBadgeText(e.Badge);
            Others?.ItemsSource?.ToList().Find(x => x.Controller == e.Controller)?.SetBadgeText(e.Badge);
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<FMessage>(this, FChannel.SET_INDEX, (sender) => Device.BeginInvokeOnMainThread(SetIndex));
            MessagingCenter.Subscribe<FMessage>(this, FChannel.TIMEOUT, (s) => Device.BeginInvokeOnMainThread(() => TimeOut(s)));
            MessagingCenter.Subscribe<FMessage>(this, FChannel.NOT_MATCH_VERSION, (s) => Device.BeginInvokeOnMainThread(() => TimeOut(s)));
            MessagingCenter.Subscribe<FMessage>(this, FChannel.OLD_VERSIONS, (s) => Device.BeginInvokeOnMainThread(() => TimeOut(s)));
            MessagingCenter.Subscribe<FMessage>(this, FChannel.LOGOUT, (s) => Device.BeginInvokeOnMainThread(() => MainPage = GetLoginWhenTimeOut()));
        }

        private void InitMenu()
        {
            Menu.Add(Dashboard, Report, Entry, Notify, Others);
            Menu.UnselectedTabColor = Color.FromHex("#6e6e6e");
            Menu.SelectedTabColor = FSetting.PrimaryColor;
            FNavigationPage.SetTitleView(Menu, new FTLHeaderNaviBar(ImageSource.FromFile("logoIndex")));
        }

        private async Task PageInits(Func<Task> afterInvoke, params IFLayout[] pages)
        {
            await FServices.ForAllAsync(pages, OnPageLoad, afterInvoke);
        }

        private Task OnPageLoad(IFLayout page)
        {
            return page.OnLoaded();
        }

        private void TimeOut(FMessage sender)
        {
            if (MainPage is NavigationPage N && !(N.RootPage is CPageLogin))
                MainPage = GetLoginWhenTimeOut();
            Device.BeginInvokeOnMainThread(() => ShowAlert(sender));
        }

        private async void ShowAlert(FMessage s)
        {
            if (s.Code == 205)
            {
                if (await FAlertHelper.Confirm("205", FText.CommandUpdate, FText.CommandCancel))
                    await FInterface.IFVersion.OpenAppInStore();
                return;
            }

            if (s is FMessageToast t)
            {
                FAlertHelper.Toast(t.Code.ToString(), t.Milisecond);
                return;
            }

            FAlertHelper.Show(s.Code.ToString());
        }

        private async void OnCompleted(object sender, FObjectPropertyArgs<bool> e)
        {
            if (e.Value) await FInterface.IFVersion?.OpenAppInStore();
        }

        #region Biometric

        //private async void ShowBiometric()
        //{
        //    try
        //    {
        //        await Task.Delay(1);
        //        if (CConfigure.UseLocalAuthen && !FInterface.IFFingerprint.IsAvailable(false))
        //        {
        //            FailedCallBack();
        //            ShowAlert(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)));
        //            return;
        //        }

        //        var config = new FAuthenticationRequestConfiguration(FAlert.Title, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FSetting.SystemLanguageIsV ? "Hủy" : "Cancel");
        //        var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

        //        if (!authResult.IsAuthenticated)
        //        {
        //            FailedCallBack();
        //            return;
        //        }

        //        SuccessCallBack();
        //    }
        //    catch (Exception ex)
        //    {
        //        FailedCallBack();
        //        if (FSetting.IsDebug) ShowAlert(new FMessage(ex.Message));
        //    }

        //    async void SuccessCallBack()
        //    {
        //        MainPage.IsBusy = true;
        //        if (!await Update())
        //        {
        //            FailedCallBack();
        //            return;
        //        }

        //        MainPage.IsBusy = false;
        //        if (!FSetting.IsAndroid && !string.IsNullOrEmpty(NotifyID) && CurrentIsTabbedPage)
        //            await OpenDetail();
        //        CConfigure.UpdateTimeSleep();
        //    }

        //    void FailedCallBack()
        //    {
        //        SetLogin(false);
        //    }

        //    async Task<bool> Update()
        //    {
        //        try
        //        {
        //            if (FPageProfile.IsOldProfile)
        //            {
        //                var ticketMessage = await FServices.UpdateTicket(checkTime: false);
        //                if (ticketMessage.OK) return true;
        //            }

        //            var keyMessage = await FServices.GetKeyExt();
        //            if (!keyMessage.OK)
        //            {
        //                MessagingCenter.Send(keyMessage, FChannel.ALERT_BY_MESSAGE);
        //                return false;
        //            }

        //            var keys = keyMessage.Message.Split((char)254);
        //            if (keys.Length != 2)
        //            {
        //                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
        //                return false;
        //            }

        //            var (Message, Token) = await FServices.CheckHashExt(true, keys[0], keys[1]);

        //            if (!Message.OK)
        //            {
        //                MessagingCenter.Send(Message, FChannel.ALERT_BY_MESSAGE);
        //                return false;
        //            }

        //            var ds = Message.Message.ToDataSet();
        //            FPageLogin.SetUserInfo(ds.First(), keys[0]);
        //            if (FString.ServiceInternal == "1") FPageLogin.InitFirebase(Token, ds);
        //            FString.SenderID = keys[1];
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
        //            return false;
        //        }
        //    }
        //}

        #endregion Biometric
    }
}