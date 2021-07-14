using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace FastMobile.Core
{
    public class CTabContent : FTabContent
    {
        public event EventHandler<EventArgs> BadgeMinus, BadgePlus;

        private readonly FPage Root;

        public CTabContent(FPage root) : base()
        {
            Root = root;
            SetSwipeTemplate(new DataTemplate(() => new FTLSwipeAll(Seen, Remove, isBinding: true)));
        }

        public void InvokeWhenOpenedNext(FItemNotify item)
        {
            if (item == null)
                return;

            if (!item.IsSeen)
            {
                if (int.TryParse(Badge.BadgeValue, out var result)) Badge.BadgeValue = result--.ToString();
                BadgeMinus?.Invoke(this, EventArgs.Empty);
                item.Read();
            }
        }

        protected async override void OnRefreshing(object sender, EventArgs e)
        {
            await SetRefresh(true);
            if (!FUtility.HasNetwork)
            {
                await SetRefresh(false);
                return;
            }
            base.OnRefreshing(sender, e);
            await Refresh();
            await SetRefresh(false);
        }

        protected override ImageSource Icon(string group)
        {
            return FIcons.BullhornOutline.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton);
        }

        protected override async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Root.SetBusy(true);
            base.OnItemTapped(sender, e);
            if (e == null || e.ItemData is not FItemNotify item)
                return;
            var page = new CPageNotifyDetail(item.Type, "DetailNotify", "System", item.Code, new DataSet().AddTable(new DataTable().AddRowValue("idNoti", item.Code)), "250", false, Model, InvokeWhenOpenedNext, Badge);
            await Navigation.PushAsync(page, true);
            await page.OnLoaded();
            InvokeWhenOpenedNext(item);
            await Root.SetBusy(false);
        }

        private async Task Remove(object code)
        {
            if (CurrentSwipe == null || !FUtility.HasNetwork)
                return;
            if (await FAlertHelper.Confirm("805"))
            {
                if (CurrentSwipe == code)
                {
                    if (!UpdateBadge(await SentRemove(CurrentSwipe.Code), false, !CurrentSwipe.IsSeen))
                        return;

                    OnRemoving(this, EventArgs.Empty);
                    BadgeMinus?.Invoke(this, EventArgs.Empty);
                    Model.MinusServerCount();
                    Model.DataSource.Remove(CurrentSwipe);
                    OnRemoved(this, EventArgs.Empty);
                }
            }
            Device.BeginInvokeOnMainThread(() => ResetSwipe());
        }

        private async Task Seen(object code)
        {
            if (CurrentSwipe == null || CurrentSwipe != code)
                return;
            OnReading(this, EventArgs.Empty);

            if (CurrentSwipe.IsSeen)
            {
                if (!UpdateBadge(await SentSeen(CurrentSwipe.Code, "0"), true))
                    return;
                BadgePlus?.Invoke(this, EventArgs.Empty);
                CurrentSwipe.UnRead();
            }
            else
            {
                if (!UpdateBadge(await SentSeen(CurrentSwipe.Code, "1"), false))
                    return;
                BadgeMinus?.Invoke(this, EventArgs.Empty);
                CurrentSwipe.Read();
            }

            OnReaded(this, EventArgs.Empty);
            Device.BeginInvokeOnMainThread(() => ResetSwipe());
        }

        private async Task<FMessage> SentRemove(string code)
        {
            if (CurrentSwipe == null)
                return new FMessage();
            return await FServices.ExecuteCommand("RemoveNotify", "System", new DataSet().AddTable(new DataTable().AddRowValue("code", code)), "0", null, false);
        }

        private async Task<FMessage> SentSeen(string code, string type)
        {
            if (CurrentSwipe == null)
                return new FMessage();
            return await FServices.ExecuteCommand("SeenNotify", "System", new DataSet().AddTable(new DataTable().AddRowValue("code", code).AddRowValue("xtype", type)), "0", null, false);
        }

        private bool UpdateBadge(FMessage message, bool isPlus, bool caculate = true)
        {
            if (!message.OK)
                return false;
            try
            {
                var ds = message.ToDataSet();
                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Columns.Count == 0)
                {
                    if (int.TryParse(Badge.BadgeValue, out var result) && caculate) Badge.BadgeValue = (isPlus ? result++ : result--).ToString();
                    return true;
                }
                Badge.BadgeValue = ds.Tables[0].Columns.Contains("badge") ? ds.Tables[0].Rows[0]["badge"].ToString() : ds.Tables[0].Rows[0][0].ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}