using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FNotifyGroupModel : FModelBase, IFNotifyParent
    {
        public event EventHandler<FNotifyRequestSuccessEventArgs> RequestSuccess;

        public readonly ObservableCollection<FItemNotify> DataSource;
        public readonly Command<object> LoadMoreItemsCommand;

        //---
        public int ServerCount { get; private set; }

        public int ClientCount => DataSource.Count;

        //---
        public int PageCount { get; private set; }

        public int PageIndex { get; private set; }
        public int LastPage { get; private set; }
        public int LastCount { get; private set; }

        //---
        public int Table { get; set; }

        public string Action { get; set; }
        public string Controller { get; set; }
        public string Group { get; set; }

        //---
        public FWebViewType Type { get; set; }

        public IFBadge Badge { get; set; }
        public Func<string, ImageSource> Icon { get; set; }

        public object FirstItem
        {
            get
            {
                if (DataSource == null || DataSource.Count == 0)
                    return string.Empty;
                return DataSource[0].Code;
            }
        }

        public object LastItem
        {
            get
            {
                if (DataSource == null || DataSource.Count == 0)
                    return "";
                return DataSource[^1].Code;
            }
        }

        private readonly Dictionary<string, string> Codes;

        public FNotifyGroupModel() : base()
        {
            Codes = new Dictionary<string, string>();
            DataSource = new ObservableCollection<FItemNotify>();
            LoadMoreItemsCommand = new Command<object>(LoadMore, CanLoadMore);
        }

        public Task Load()
        {
            return AddNotify("", true);
        }

        public Task LoadOne(FNotifyInformation info)
        {
            return AddNotify(info.ID, false);
        }

        public async Task LoadOne(string id)
        {
            if (DataSource.ToList().Find(x => x.Code == id) != null)
                return;
            await AddNotify(id, false);
        }

        public void Read(string id)
        {
            if (DataSource?.ToList().Find((x) => x.Code == id) is FItemNotify t && !t.IsSeen)
            {
                if (int.TryParse(Badge.BadgeValue, out var result)) Badge.BadgeValue = result++.ToString();
                t.Read();
                t.IsSeen = true;
            }
        }

        public void ReadAll()
        {
            DataSource.ForEach((x) => x.Read());
        }

        public void UnRead(string id)
        {
            DataSource?.ToList().Find((x) => x.Code == id.ToString())?.UnRead();
        }

        public void UnReadAll()
        {
            DataSource.ForEach((x) => x.UnRead());
        }

        public void InitProperty(string action, string controller, string group, int countPerPage, int table, FWebViewType type, IFBadge badge)
        {
            Action = action;
            Controller = controller;
            Group = group;
            PageCount = countPerPage;
            Table = table;
            Type = type;
            Badge = badge;
        }

        public void MinusServerCount()
        {
            ServerCount--;
        }

        protected virtual void OnRequestSuccess(object sender, FNotifyRequestSuccessEventArgs e)
        {
            RequestSuccess?.Invoke(sender, e);
        }

        public bool CanLoadMore(object arg = null)
        {
            if (FUtility.IsTimeOut)
                MessagingCenter.Send(new FMessage(), FChannel.TIMEOUT);
            return FUtility.HasNetwork && ClientCount < ServerCount && !FUtility.IsTimeOut;
        }

        protected virtual Task<FMessage> GetData(string notifyCode, bool isLoadMore)
        {
            var typeInt = typeof(int);
            var ds = new DataSet().AddTable(
                new DataTable()
                .AddRowValue(0, "refresh", 0, typeInt)
                .AddRowValue(0, "pageNumber", PageIndex, typeInt)
                .AddRowValue(0, "rowPerPage", PageCount, typeInt)
                .AddRowValue(0, "lastPage", LastPage, typeInt)
                .AddRowValue(0, "lastCount", LastCount, typeInt)
                .AddRowValue(0, "firstItem", FirstItem)
                .AddRowValue(0, "lastItem", LastItem)
                .AddRowValue(0, "group", Group)
                .AddRowValue(0, "dvNotifyGroup", Group)
                .AddRowValue(0, "notifyCode", notifyCode));
            return FServices.ExecuteCommand(Action, Controller, ds, "0", null, isLoadMore);
        }

        private async void LoadMore(object obj)
        {
            await AddNotify("", true);
        }

        private async Task AddNotify(string code, bool isLoadMore)
        {
            try
            {
                if (!string.IsNullOrEmpty(code) && !isLoadMore && Codes != null && Codes.ContainsKey(code))
                    return;
                PageIndex = isLoadMore ? PageIndex + 1 : PageIndex;
                await SetBusy(true);
                var message = await GetData(code, isLoadMore);
                await SetBusy(false);
                if (message.Success != 1)
                {
                    if (isLoadMore)
                        MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }
                var data = message.ToDataSet();

                if (data.Tables[Table].Rows.Count == 0)
                    PageIndex = isLoadMore ? PageIndex - 1 : PageIndex;

                FormatData(data.Tables[Table], isLoadMore);

                if (data.Tables.Count > 1)
                {
                    if (data.Tables[1].Columns.Contains("total"))
                        ServerCount = Convert.ToInt32(data.Tables[1].Rows[0]["total"]);
                    if (data.Tables[1].Columns.Contains("not_seen"))
                        Badge.BadgeValue = data.Tables[1].Rows[0]["not_seen"].ToString();
                }
                OnRequestSuccess(this, new FNotifyRequestSuccessEventArgs(ServerCount, Badge.BadgeValue, ClientCount, data));
            }
            catch (Exception ex)
            {
                if (isLoadMore) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
            }
        }

        private void FormatData(DataTable table, bool isLoadMore)
        {
            if (table.Rows.Count < 1)
                return;
            if (isLoadMore)
            {
                table.Rows.ForEach<DataRow>((x) =>
                {
                    bool seen = x["status"].ToString().ToLower() == "true" || x["status"].ToString() == "1";
                    AddItem(x["title"].ToString(), x["content"].ToString(), x["time"].ToString(), x["code"].ToString(), x["group_code"].ToString(), seen, seen ? FSetting.DisableColor : FSetting.TextColorTitle, seen ? FSetting.DisableColor : FSetting.ColorTime, seen ? FSetting.FontText : FSetting.FontTextMedium);
                });
                return;
            }
            if (Codes.ContainsKey(table.Rows[0]["code"].ToString()))
                return;
            Insert(table.Rows[0]["title"].ToString(), table.Rows[0]["content"].ToString(), table.Rows[0]["time"].ToString(), table.Rows[0]["code"].ToString(), table.Rows[0]["group_code"].ToString(), false, FSetting.TextColorTitle, FSetting.ColorTime, FSetting.FontTextMedium);
        }

        private void AddItem(string title, string content, string time, string code, string group, bool isSeen, Color colorContent, Color colorTime, string font)
        {
            DataSource.Add(new FItemNotify { Title = title, Content = content, Time = time, Code = code, Icon = Icon?.Invoke(group), IsSeen = isSeen, ColorContent = colorContent, ColorTime = colorTime, Font = font, Type = this.Type }.Refresh());
            Codes.TryAdd(code, code);
        }

        private void Insert(string title, string content, string time, string code, string group, bool isSeen, Color colorContent, Color colorTime, string font)
        {
            DataSource.Insert(0, new FItemNotify { Title = title, Content = content, Time = time, Code = code, Icon = Icon?.Invoke(group), IsSeen = isSeen, ColorContent = colorContent, ColorTime = colorTime, Font = font, Type = this.Type }.Refresh());
            Codes.TryAdd(code, code);
        }
    }
}