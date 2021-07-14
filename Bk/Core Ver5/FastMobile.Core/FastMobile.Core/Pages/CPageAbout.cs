using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageAbout : FPageAbout
    {
        public CPageAbout() : base(false)
        {
            InvokeWhenTapped = InvokeTapped;
        }

        public async override void Init()
        {
            base.Init();
            if (!HasNetwork)
                return;
            await SetBusy(true);
            await LoadContent();
            await SetBusy(false);
        }

        protected override async void OnRefreshing(object sender, EventArgs e)
        {
            await SetRefresh(true);
            if (!FUtility.HasNetwork)
            {
                await SetRefresh(false);
                return;
            }
            base.OnRefreshing(sender, e);
            await LoadItems();
            await SetRefresh(false);
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        private async void InvokeTapped(IFDataEvent e)
        {
            if (e.ItemData is not FItemAbout item)
                return;
            var page = new FPageWebView(FWebViewType.Default, item.Action, item.Controller, item.Controller, null, "250", false) { Title = item.TitlePage };
            await Navigation.PushAsync(page, true);
            page.Init();
        }

        private Task LoadContent()
        {
            return LoadItems();
        }

        private async Task LoadItems()
        {
            try
            {
                var message = await FServices.ExecuteCommand("GetAbout", "System", null, "0", FExtensionParam.New(true, FText.AttributeString("V", "AboutProduct"), FText.AttributeString("E", "AboutProduct"), FAction.Load), "AboutProduct", false);
                if (message.Success != 1)
                {
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                Items.Clear();
                message.ToDataSet()?.Tables[0].Rows.ForEach<DataRow>((x) => AddWithRow(x));
            }
            catch (Exception ex) { MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE); }
        }

        private void AddWithRow(DataRow row)
        {
            AddItem(row["img_url"].ToString(), row["color"].ToString(), row["action"].ToString(), row["controller"].ToString(), row["title_page"].ToString(), row["title"].ToString(), row["subtitle"].ToString());
        }
    }
}