using FastMobile.FXamarin.Core;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class CPageMenu : FPageMenu
    {
        public CPageMenu(string group, bool isHasPullToRefresh) : base(group, isHasPullToRefresh, false)
        {
            Base();
        }

        public async override void Init()
        {
            if (!HasNetwork)
                return;
            base.Init();
            await SetBusy(true);
            await LoadContent();
            await SetBusy(false);
        }

        public override void NotifyReceived(FMessage message)
        {
            if (!message.OK)
                return;
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
            if (HasInitialized)
                await InitMenu();
            else
                await LoadContent();
            await SetRefresh(false);
        }

        protected override async void OnViewTypeClicked(object sender, EventArgs e)
        {
            await SetBusy(true);
            base.OnViewTypeClicked(sender, e);
            OnLayout();
            CacheViewType(ViewType, Group);
            await SetBusy(false);
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ViewTypeProperty.PropertyName)
                CacheViewType(ViewType, Group);
        }

        protected virtual void InitDefaultMenu()
        {
        }

        protected void AddItem(string wmenuID, string wmenuID0, string bar, string controller, string action, string xtype, string groupname, ImageSource source, string badge, Color badgeColor, string sortedTapped, string sortImportance)
        {
            ItemsSource.Add(new FItemMenu(wmenuID, wmenuID0, bar, controller, action, xtype, groupname, source, badge?.CutString(6), badgeColor, sortedTapped, sortImportance));
        }

        private async void Base()
        {
            await SetBusy(true);
        }

        private Task LoadContent()
        {
            OnLayout();
            return InitMenu();
        }

        private async Task InitMenu()
        {
            var message = await GetMenu();
            ItemsSource = new ObservableCollection<FItemMenu>();
            InitDefaultMenu();
            if (message.Success != 1 && ItemsSource.Count == 0)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                ShowNothing = true;
                return;
            }

            if (message.Success != 1 && ItemsSource.Count > 0)
            {
                UpdateMenuView();
                return;
            }

            try
            {
                var menu = message.ToDataSet();
                if (ItemsSource.Count == 0 && (menu.Tables.Count == 0 || menu.Tables[0].Rows.Count == 0))
                {
                    ShowNothing = true;
                    return;
                }
                InitSource(menu);
                UpdateMenuView();
                //if (menu.Tables.Count == 1 || !menu.Tables[1].Columns.Contains("show_expand"))
                //    return;
                //TurnOnExpand = menu.Tables[1].Rows[0]["show_expand"].Equals(1);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private void InitSource(DataSet data)
        {
            if (data.Tables.Count > 0)
                data.Tables[0].Rows.ForEach<DataRow>(x => AddRow(data.Tables[0], x));
        }

        private void AddRow(DataTable table, DataRow row)
        {
            if (string.IsNullOrEmpty(GetValue(row, "wmenu_id0")) && !string.IsNullOrEmpty(GetValue(row, "controller")))
            {
                AddItem(GetValue(row, "wmenu_id"), GetValue(row, "wmenu_id0"), GetValue(row, "bar"), GetValue(row, "controller"), GetValue(row, "action"), GetValue(row, "xtype"), "", GetValue(row, "icon_url").ToImageSource(GetValue(row, "icon_color"), 50), GetValue(row, "badge", ""), table.Columns.Contains("badge_color") ? Color.FromHex(GetValue(row, "badge_color", "#00ffffff")) : FSetting.DangerColor, GetValue(row, "priority", ""), "");
                return;
            }

            if (string.IsNullOrEmpty(GetValue(row, "wmenu_id0")) && string.IsNullOrEmpty(GetValue(row, "controller")))
                table.Rows.ForEach<DataRow>((x) => AddRow2(x, GetValue(row, "bar"), GetValue(row, "wmenu_id")));
        }

        private void AddRow2(DataRow row, string groupName, string id)
        {
            if (GetValue(row, "wmenu_id0") == id)
                AddItem(GetValue(row, "wmenu_id"), GetValue(row, "wmenu_id0"), GetValue(row, "bar"), GetValue(row, "controller"), GetValue(row, "action"), GetValue(row, "xtype"), groupName, GetValue(row, "icon_url").ToImageSource(GetValue(row, "icon_color"), 50), GetValue(row, "badge", ""), row.Table.Columns.Contains("badge_color") ? Color.FromHex(GetValue(row, "badge_color", "#00ffffff")) : FSetting.DangerColor, GetValue(row, "priority", ""), "");
        }

        private Task<FMessage> GetMenu()
        {
            return FServices.ExecuteCommand("GetMenu", "System", new DataSet().AddTable(new DataTable().AddRowValue("xtype", Group)), "0", null, Group);
        }

        private string GetValue(DataRow row, string name, string defaultValue = null)
        {
            if (defaultValue == null)
                return row[name].ToString();
            return row.Table.Columns.Contains(name) ? row[name].ToString() : defaultValue;
        }
    }
}