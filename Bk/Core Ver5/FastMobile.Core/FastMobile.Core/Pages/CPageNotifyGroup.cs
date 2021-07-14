using FastMobile.FXamarin.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageNotifyGroup : FPageNotifyTab
    {
        public virtual CTabContent Approval { get; }

        public CPageNotifyGroup() : base(false, false)
        {
            Base();
        }

        private async void Base()
        {
            await SetBusy(true);
        }

        public async Task InitBadge()
        {
            try
            {
                var message = await FServices.ExecuteCommand("GetCountNotSeenNotify", "System", null, "0", null, false);
                if (message.Success != 1)
                    return;
                var data = message.ToDataSet();
                if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                    return;
                BadgeValue = data.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug)
                    MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        protected async Task<bool> InvokeReadAll(string group)
        {
            try
            {
                await SetBusy(true);
                var message = await FServices.ExecuteCommand("ReadAllNotify", "System", new DataSet().AddTable(new DataTable().AddRowValue("xgroup", group)), "0", null);
                await SetBusy(false);
                if (message.Success != 1)
                {
                    MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return false;
                }
                var data = message.ToDataSet();
                if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                    return false;
                BadgeValue = data.Tables[0].Rows[0][0].ToString();
                return true;
            }
            catch (Exception ex)
            {

                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                return false;
            }
        }

        protected async override void OnReadAllClicked(object sender, EventArgs e)
        {
            if (!FUtility.HasNetwork)
                return;
            if (!await FAlertHelper.Confirm("806"))
                return;
            base.OnReadAllClicked(sender, e);
            if (await InvokeReadAll((TabView.Items[TabView.SelectedIndex].Content as FTabContent).Model.Group))
                (TabView.Items[TabView.SelectedIndex].Content as FTabContent).Model.ReadAll();
        }

        protected void InitEvent(params FTabContent[] tabs)
        {
            tabs.ForEach(x =>
            {
                x.Removing += Doing;
                x.Removed += Done;
                x.Reading += Doing;
                x.Readed += Done;
            });
        }

        private async void Doing(object sender, EventArgs e)
        {
            await SetBusy(true);
        }

        private async void Done(object sender, EventArgs e)
        {
            await SetBusy(false);
        }
    }
}