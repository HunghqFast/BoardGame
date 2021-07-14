using FastMobile.FXamarin.Core;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageOther : CPageMenu
    {
        public CPageOther() : base("50", true)
        {
            ViewType = MenuViewType("50", FMenuViewType.List);
            InitToolbar();
            ItemTapped += Tabbed;
        }

        protected virtual Task OpenPageAbout(FItemMenu item)
        {
            return Task.CompletedTask;
        }

        protected override void InitDefaultMenu()
        {
            AddItem("001", "00.00.00", FText.AboutProduct, "System", "About", "", FText.System, FIcons.BookInformationVariant.ToFontImageSource(FSetting.InfoColor, 50), "", default, "", "");
            AddItem("002", "00.00.00", FText.ChangePassword, "System", "ChangePassword", "", FText.System, FIcons.ShieldKeyOutline.ToFontImageSource(FSetting.WarningColor, 50), "", default, "", "");
            AddItem("003", "00.00.00", FText.Settings, "System", "Setting", "", FText.System, FIcons.CogOutline.ToFontImageSource(FSetting.DisableColor, 50), "", default, "", "");
            AddItem("004", "00.00.00", FText.Logout, "System", "Logout", "", FText.System, FIcons.Logout.ToFontImageSource(FSetting.DangerColor, 50), "", default, "", "");
        }

        private async void Tabbed(object sender, IFDataEvent e)
        {
            await SetBusy(true);
            await Task.Delay(10);
            await ClickMenu(e.ItemData as FItemMenu);
            await SetBusy(false);
        }

        private async Task ClickMenu(FItemMenu item)
        {
            if (item == null)
                return;
            if (Navigation.NavigationStack.Count != 1)
                return;

            switch (item.Action.Trim())
            {
                case "About":
                    if (item.Controller == "System")
                    {
                        var about = new CPageAbout() { Title = item.Bar };
                        await Navigation.PushAsync(about);
                        about.Init();
                    }
                    break;

                case "Setting":
                    if (item.Controller == "System")
                        await Navigation.PushAsync(new CPageSetting().Init(true), true);
                    break;

                case "Logout":
                    if (item.Controller == "System")
                        if (await FAlertHelper.Confirm("801"))
                        {
                            ClearSettings();
                            await Logout();
                        }
                    break;

                case "ChangePassword":
                    if (item.Controller == "System")
                        await Navigation.PushAsync(new FPageChangePassword(false, false), true);
                    break;

                default:
                    if (string.IsNullOrEmpty(item.Controller))
                        break;
                    if (string.IsNullOrEmpty(item.Action))
                    {
                        if (item.XType.Equals("Spin")) new FPageSpinWheel(this, item.Controller).Init();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.XType))
                            await FPageReport.SetReportByAction(this, item.Action, item.Controller);
                        else
                        {
                            var page = new FPageWebView(FWebViewType.Default, item.Action, item.Controller, item.WMenuId, null, "250", false) { Title = item.Bar };
                            await Navigation.PushAsync(page, true);
                            page.Init();
                        }
                    }
                    break;
            }
        }

        private async Task Logout()
        {
            var key = FSetting.NetWorkKey;
            if (!FString.ServiceInternal.Equals("1"))
            {
                FInterface.IFFirebase?.DeleteToken(FString.SenderID);
                FInterface.IFFirebase?.RefreshToken();
            }
            MessagingCenter.Send(new FMessage(), FChannel.LOGOUT);
            await FServices.ExecuteCommand("Logout", "System", null, "0", FExtensionParam.New(true, FText.AttributeString("V", "SignOut"), FText.AttributeString("E", "SignOut"), FAction.Logout), 5000, false, keys: key);
        }

        private void ClearSettings()
        {
            FViewPage.ClearAll();
            FApprovalComment.ClearAll();
        }

        //private async Task Test()
        //{
        //    var st = new Stopwatch();
        //    st.Start();
        //    var ds = new DataSet();
        //    ds.AddTable(new DataTable());
        //    for (int i = 0; i < 1200; i++)
        //    {
        //        ds.Tables[0].AddRowValue(i, "col_1", "String")
        //            .AddRowValue(i, "col_2", 1)
        //            .AddRowValue(i, "col_3", 1d)
        //            .AddRowValue(i, "col_4", true)
        //            .AddRowValue(i, "col_5", null)
        //            .AddRowValue(i, "col_6", DateTime.Now)
        //            .AddRowValue(i, "col_7", new byte[1], typeof(byte[]))
        //            .AddRowValue(i, "col_8", "Lorem Ipsum is simply dummy text of the printing and typesetting industry.")
        //            .AddRowValue(i, "col_9", 121314.124)
        //            .AddRowValue(i, "col_10", TimeSpan.FromSeconds(30));
        //    }
        //    var message = await FServices.ExecuteCommand("Action001", "System", ds, "0", null);
        //    st.Stop();
        //    Console.WriteLine(st.ElapsedMilliseconds);
        //}
    }
}