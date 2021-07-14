using Syncfusion.ListView.XForms;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FGridBase : ContentView, IFPaging, IFReportSettings, IFStatus
    {
        private bool isLock, isFirst, hasComment;
        private int totalItem;

        private FInputTextUnderline E;

        private ReportCacheType CheckType => GetTypeReportData(TargetType);

        public static readonly BindableProperty TriggerRefreshProperty = BindableProperty.Create("TriggerRefresh", typeof(bool), typeof(FGridBase), false);
        public static readonly BindableProperty ListPagingProperty = BindableProperty.Create("ListPaging", typeof(ObservableCollection<int>), typeof(FPaging), new ObservableCollection<int>());
        public static readonly BindableProperty ListItemProperty = BindableProperty.Create("ListItem", typeof(ObservableCollection<int>), typeof(FPaging), new ObservableCollection<int> { 5, 10, 15, 20, 25 });
        public static readonly BindableProperty ItemPerPageProperty = BindableProperty.Create("ItemPerPage", typeof(int), typeof(FGridBase), 20);
        public static readonly BindableProperty ItemToProperty = BindableProperty.Create("ItemTo", typeof(int), typeof(FGridBase), 0);
        public static readonly BindableProperty ItemFromProperty = BindableProperty.Create("ItemFrom", typeof(int), typeof(FGridBase), 0);
        public static readonly BindableProperty PageIndexProperty = BindableProperty.Create("PageIndex", typeof(int), typeof(FGridBase), 0);
        public static readonly BindableProperty IsBusyGridProperty = BindableProperty.Create("IsBusyGrid", typeof(bool), typeof(FGridBase), false);
        public static readonly BindableProperty GridTypeProperty = BindableProperty.Create("GridType", typeof(GridType), typeof(FGridBase), GridType.ListView);
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(FDataObservation), typeof(FGridBase), new FDataObservation());
        public static readonly BindableProperty DetailsProperty = BindableProperty.Create("Details", typeof(FData), typeof(FGridBase), new FData());
        public static readonly BindableProperty ListViewProperty = BindableProperty.Create("ListView", typeof(SfListView), typeof(FGridBase), new SfListView());
        public static readonly BindableProperty GridViewProperty = BindableProperty.Create("GridView", typeof(SfDataGrid), typeof(FGridBase), new SfDataGrid());
        public static readonly BindableProperty WebViewProperty = BindableProperty.Create("WebView", typeof(View), typeof(FGridBase), new StackLayout());
        public static readonly BindableProperty HtmlProperty = BindableProperty.Create("Html", typeof(string), typeof(FGridBase), "");
        public static readonly BindableProperty IsAllowSelectProperty = BindableProperty.Create("IsAllowSelect", typeof(bool), typeof(FGridBase), false);

        public bool Success { get; set; }

        public FTargetType TargetType { get; set; }

        public FGridStyle GridStyle { get; set; }

        public string Controller { get; set; }

        public FPageReport Root { get; set; }

        public FReportMethod Method { get; set; }

        public IFCommand Command { get; set; }

        public IFReportToolbar Toolbar { get; set; }

        public IFApproval Approval { get; set; }

        public string CacheDeviceID => $"FastMobile.FXamarin.Core.FGridBase.{Controller}_Grid";

        public string SettingsDeviceID => $"FastMobile.FXamarin.Core.FGridBase.device_settings_grid_{Controller}";

        public string TotalRecordName => "value";

        public bool TriggerRefresh { get => (bool)GetValue(TriggerRefreshProperty); set => SetValue(TriggerRefreshProperty, value); }

        public ObservableCollection<int> ListItem { get => (ObservableCollection<int>)GetValue(ListItemProperty); set => SetValue(ListItemProperty, value); }

        public ObservableCollection<int> ListPaging { get => (ObservableCollection<int>)GetValue(ListPagingProperty); set => SetValue(ListPagingProperty, value); }

        public int ItemTo { get => (int)GetValue(ItemToProperty); set => SetValue(ItemToProperty, value); }

        public int ItemFrom { get => (int)GetValue(ItemFromProperty); set => SetValue(ItemFromProperty, value); }

        public int ItemPerPage { get => (int)GetValue(ItemPerPageProperty); set => SetValue(ItemPerPageProperty, value); }

        public int PageIndex { get => (int)GetValue(PageIndexProperty); set => SetValue(PageIndexProperty, value); }

        public bool IsBusyGrid { get => (bool)GetValue(IsBusyGridProperty); set => SetValue(IsBusyGridProperty, value); }

        public GridType GridType { get => (GridType)GetValue(GridTypeProperty); set => SetValue(GridTypeProperty, value); }

        public FDataObservation Source { get => (FDataObservation)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        public FData Details { get => (FData)GetValue(DetailsProperty); set => SetValue(DetailsProperty, value); }

        public SfListView ListView { get => (SfListView)GetValue(ListViewProperty); set => SetValue(ListViewProperty, value); }

        public SfDataGrid GridView { get => (SfDataGrid)GetValue(GridViewProperty); set => SetValue(GridViewProperty, value); }

        public View WebView { get => (View)GetValue(WebViewProperty); set => SetValue(WebViewProperty, value); }

        public string Html { get => (string)GetValue(HtmlProperty); set => SetValue(HtmlProperty, value); }

        public bool IsAlowSelect { get => (bool)GetValue(IsAllowSelectProperty); set => SetValue(IsAllowSelectProperty, value); }

        public int TotalItem { get => totalItem <= 0 ? 0 : totalItem; set { totalItem = value; OnPropertyChanged("TotalItem"); } }

        public FViewPage Settings { get; set; }

        public int LastPage { get; set; }

        public int LastIndex { get; set; }

        public string FirstItem
        {
            get
            {
                var result = string.Empty;
                if (Source.Count == 0) return result;
                Method.GetDataItem(ref result, Source[0]);
                return result;
            }
        }

        public string LastItem
        {
            get
            {
                var result = string.Empty;
                if (Source.Count == 0 || LastIndex == -1) return result;
                Method.GetDataItem(ref result, Source[LastIndex]);
                return result;
            }
        }

        public int DataIndex { get; set; }

        public int DetailIndex { get; set; }

        public int ExtendIndex { get; set; }

        public System.Data.DataRow[] DataCache
        {
            get
            {
                var value = CacheDeviceID.GetCache();
                if (string.IsNullOrEmpty(value)) return null;
                try
                {
                    var data = value.ToDataSet().Tables[DataIndex];
                    return Settings.Filter.Equals("") ? data.Select() : data.Select(Settings.Filter);
                }
                catch { return null; }
            }
        }

        public FData DataFilter { get; set; }

        public Dictionary<string, object> ExtData { get; set; }

        public DataTable ExtenderData { get; set; }

        public string CommentText => hasComment ? E.Value : string.Empty;

        public FViews DetailView => Settings.Views.Find(x => x.Id == "Detail");

        public FGridBase(string controller)
        {
            isLock = false;
            isFirst = true;

            LastIndex = -1;
            TotalItem = -1;
            LastPage = 0;
            Method = new FReportMethod(this);
            Command = new FCommand(this);
            Toolbar = new FReportToolbar(this);
            Approval = new FApproval(this);
            ExtData = new Dictionary<string, object>();
            Source = new FDataObservation();
            BindingContext = this;
            GridStyle = new FGridStyle();
            WidthRequest = FSetting.ScreenWidth;
            Controller = controller;
            E = new FInputTextUnderline();

            this.SetBinding(ContentView.ContentProperty, ListViewProperty.PropertyName);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(GridType):
                    Root.SetView();
                    if (GridType == GridType.ListView) this.SetBinding(ContentView.ContentProperty, ListViewProperty.PropertyName);
                    else if (GridType == GridType.GridView) this.SetBinding(ContentView.ContentProperty, GridViewProperty.PropertyName);
                    else this.SetBinding(ContentView.ContentProperty, WebViewProperty.PropertyName);
                    break;

                case nameof(TriggerRefresh):
                    Refresh();
                    break;

                default:
                    break;
            }
        }

        #region Public

        public async void Refresh()
        {
            await Root.SetBusy(true);
            await AddNewData(TargetType, false);
            await InitAttachment();
            await InitComment();
            LastPage = PageIndex - 1;
            LastIndex = Source.Count - 1;
            await Root.SetBusy(false);
        }

        public async Task LoadingGrid(FTargetType target)
        {
            await Root.SetBusy(true);
            isFirst = true;
            TotalItem = -1;
            LastPage = 0;
            ExtData = new Dictionary<string, object>();
            Source = new FDataObservation();
            PageIndex = 1;
            await AddNewData(target, true);
            await InitAttachment();
            await InitComment();
            LastPage = PageIndex - 1;
            LastIndex = Source.Count - 1;
            TargetType = target;
            await Root.SetBusy(false);
        }

        public async Task<bool> UpdateItem(FFormType type)
        {
            var rs = await Toolbar.SaveRecordGrid(type);
            return rs.Result;
        }

        public async Task<bool> EditInDirItem(FData data)
        {
            var rs = await Toolbar.ScatterRecordGrid(new FToolbar { Command = "Edit", FormType = 1 }, data);
            return rs.Result;
        }

        public async Task<bool> NewInDirItem(FData data)
        {
            var rs = await Toolbar.ScatterRecordGrid(new FToolbar { Command = "New", FormType = 1 }, data);
            return rs.Result;
        }

        public async Task<bool> NewItem(object obj)
        {
            (obj as FToolbar).CommandArgument = null;
            var rs = await Toolbar.NewRecordGrid(obj);
            return rs.Result;
        }

        public async Task<bool> ReleaseItem(object obj)
        {
            var rs = await Toolbar.ReleaseRecordGrid(obj, new FData());
            return rs.Result;
        }

        public async Task<bool> CancelItem(object obj)
        {
            var rs = await Toolbar.CancelRecordGrid(obj, null);
            return rs.Result;
        }

        public async Task<bool> AcceptApprovalItem(object obj)
        {
            var rs = await Toolbar.AcceptApproval(obj, new FData());
            return rs.Result;
        }

        public async Task<bool> CancelApprovalItem(object obj)
        {
            var rs = await Toolbar.CancelApproval(obj, new FData());
            return rs.Result;
        }

        public async Task<bool> UndoApprovalItem(object obj)
        {
            var rs = await Toolbar.UndoApproval(obj, new FData());
            return rs.Result;
        }

        public bool CheckNextBackToobarEnable(bool isNext)
        {
            if (Root.BeforePage == null) return false;
            var grid = Root.BeforePage.Grid;
            var index = grid.GridView.SelectedIndex;
            if (index < 0 || index > grid.Source.Count || index == (isNext ? grid.Source.Count : 1)) return false;
            return true;
        }

        public async Task<bool> NextToolbarItem(object obj)
        {
            return await Next(false);
        }

        public async Task<bool> BackToolbarItem(object obj)
        {
            return await Back(false);
        }

        public async Task<bool> CustomToolbarItem(object obj)
        {
            FData data;
            FCommnadValue rs;
            if (!Settings.SelectMode)
            {
                rs = await Toolbar.CustomRecordOther(obj, null);
                return rs.Result;
            }
            if (Source.Count == 0) return false;
            else if (Source.Count == 1) data = Source[0];
            else
            {
                var selected = GetSelectedItems();
                if (selected.Count > 1)
                {
                    MessagingCenter.Send(new FMessage(0, 145, ""), FChannel.ALERT_BY_MESSAGE);
                    return false;
                }
                if (selected.Count == 0) return false;
                data = selected[0];
            }
            rs = await Toolbar.CustomRecordGrid(obj, data);
            return rs.Result;
        }

        public async Task<bool> DownLoadAttachment(object obj)
        {
            var rs = await Toolbar.DownloadAttachment(obj, null);
            return rs.Result;
        }

        public async Task SelectButton(object obj)
        {
            await Root.SetBusy(true);
            await Task.Delay(10);
            var ds = CacheDeviceID.GetCache().ToDataSet();
            Source.ForEach(x => { SelectItem(ref ds, Source.IndexOf(x), true); });
            ds.Encode().SetCache(CacheDeviceID);
            await Root.SetBusy(false);
            await Task.CompletedTask;
        }

        public void DeleteItem(ref DataSet ds, int index)
        {
            if (CheckType == ReportCacheType.None) Source.RemoveAt(index - 1);
            else
            {
                if (Source.Count > index - ItemFrom && index - ItemFrom >= 0)
                {
                    Source.RemoveAt(index - ItemFrom);
                    ItemTo -= 1;
                }
                ds.Tables[DataIndex].Rows.RemoveAt(index - 1);
                TotalItem -= 1;
            }
            if (index <= LastIndex + 1) LastIndex--;
        }

        public void EditItem(ref DataSet ds, int index, System.Data.DataRow dr)
        {
            if (CheckType == ReportCacheType.None) Source[index - 1] = FData.NewItem(dr, Settings.Fields);
            else
            {
                if (Source.Count > index - ItemFrom && index - ItemFrom >= 0) Source[index - ItemFrom] = FData.NewItem(dr, Settings.Fields);
                foreach (System.Data.DataColumn x in ds.Tables[DataIndex].Columns)
                    if (dr.Table.Columns.Contains(x.ColumnName)) ds.Tables[DataIndex].Rows[index - 1][x.ColumnName] = dr[x.ColumnName];
            }
        }

        public void AddItem(System.Data.DataRow dr)
        {
            Source.Add(FData.NewItem(dr, Settings.Fields));
            if (CheckType != ReportCacheType.None)
            {
                var ds = CacheDeviceID.GetCache().ToDataSet();
                ds.Tables[DataIndex].ImportRow(dr);
                ds.Encode().SetCache(CacheDeviceID);
                ItemTo += 1;
                TotalItem += 1;
            }
        }

        public void SelectItem(ref DataSet ds, int index, bool isChanged)
        {
            if (isChanged) Source.CheckItem(index);
            var columns = ds.Tables[DataIndex].Columns;
            if (CheckType != ReportCacheType.None && columns.Contains(FData.CheckStatusName) && columns.Contains(FData.LineNumberRowName))
            {
                ds.Tables[DataIndex].Rows[ItemFrom + index - 1][FData.CheckStatusName] = Source[index][FData.CheckStatusName];
                ds.Tables[DataIndex].Rows[ItemFrom + index - 1][FData.LineNumberRowName] = ItemFrom + index;
            }
        }

        public List<FData> GetSelectedItems()
        {
            if (CheckType == ReportCacheType.None) return Source.SelectedsItem;
            else
            {
                var ld = new List<FData>();
                var ds = CacheDeviceID.GetCache().ToDataSet();
                if (!ds.Tables[DataIndex].Columns.Contains(FData.CheckStatusName)) return ld;
                foreach (System.Data.DataRow row in ds.Tables[DataIndex].Rows)
                    if (FFunc.StringToBoolean(row[FData.CheckStatusName].ToString())) ld.Add(FData.NewItem(row, Settings.Fields));
                return ld;
            }
        }

        public async void OpenDetail(DataTable table)
        {
            try
            {
                if (table == null || table.Rows.Count == 0) return;
                var view = DetailView;
                if (view == null) return;
                var mess = await FCommand.Showing(Settings.Fields, FData.NewItem(table.Rows[0], Settings.Fields), Controller);
                if (mess.Success == 1) await FPageReport.SetReportByAction(Root, "Inquiry", view.Controller, Root, mess.ToDataSet(), view.Reference, TargetType);
                else MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
            }
            catch (Exception ex) { MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE); }
        }

        public async Task<bool> Next(bool isDelete)
        {
            return await NextBackRecord(true, isDelete);
        }

        public async Task<bool> Back(bool isDelete)
        {
            return await NextBackRecord(false, isDelete);
        }

        #endregion Public

        #region Event

        private async void OnItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            lock (sender) { if (isLock) return; isLock = true; }
            var item = e.ItemData as FData;
            GridView.SelectedIndex = Source.IndexOf(item) + 1;
            if (IsAlowSelect)
            {
                var ds = CacheDeviceID.GetCache().ToDataSet();
                SelectItem(ref ds, Source.IndexOf(item), true);
                ds.Encode().SetCache(CacheDeviceID);
            }
            else await OpenedDetail(item);
            isLock = false;
        }

        private async void OnItemTapped(object sender, GridTappedEventArgs e)
        {
            lock (sender) { if (isLock) return; isLock = true; }
            if (e.RowColumnIndex.RowIndex <= 0)
            {
                isLock = false;
                return;
            }
            GridView.SelectedIndex = e.RowColumnIndex.RowIndex;
            if (IsAlowSelect)
            {
                var ds = CacheDeviceID.GetCache().ToDataSet();
                SelectItem(ref ds, e.RowColumnIndex.RowIndex - 1, true);
                ds.Encode().SetCache(CacheDeviceID);
            }
            else await OpenedDetail(e.RowData as FData);
            isLock = false;
        }

        public void OnClearCacheComment(object sender, EventArgs e)
        {
            FApprovalComment.RemoveSetting(E.CacheInput);
        }

        #endregion Event

        #region Data

        private async Task AddNewData(FTargetType target, bool isLog)
        {
            if (Settings == null) return;
            ListView.ResetSwipe();
            GridView.ResetSwipeOffset();
            switch (GetTypeReportData(target))
            {
                case ReportCacheType.Device:
                    await AddDataGridCache(target, isLog);
                    break;

                case ReportCacheType.None:
                    await AddDataGrid(target, isLog);
                    break;

                case ReportCacheType.IIS:
                    await AddDataGridCache(target, isLog);
                    break;
            }
        }

        private async Task OpenedDetail(FData item)
        {
            try
            {
                var view = DetailView;
                switch (view.Type)
                {
                    case "Inquiry":
                        await Root.SetBusy(true);
                        var mess = await FCommand.Showing(Settings.Fields, item, Controller);
                        if (mess.Success == 1) await FPageReport.SetReportByAction(Root, "Inquiry", view.Controller, Root, mess.ToDataSet(), view.Reference, TargetType);
                        else MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                        await Root.SetBusy(false);
                        break;

                    case "Dir":
                        await Toolbar.ViewRecordGrid(new FToolbar { Command = "View", FormType = 1 }, item);
                        break;

                    case "Edit":
                        await Toolbar.EditRecordGrid(new FToolbar { Command = "Edit", FormType = 1, Title = "", MenuItem = new List<FItem>() }, item);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                await Root.SetBusy(false);
            }
        }

        private async Task AddDataGrid(FTargetType target, bool isLog)
        {
            try
            {
                if (Root.InputData != null)
                {
                    DataIndex = Settings.TableData;
                    DetailIndex = Settings.TableDetails;
                    ExtendIndex = Settings.TableExtend;

                    var data = Root.InputData;
                    var total = GetTotalRecord(data);

                    Method.AddDataDetails(data, DetailIndex, DetailIndex != -1 && totalItem == -1);
                    Method.AddDataExtend(data, ExtendIndex);
                    Method.AddDataSource(data, DataIndex, total, this);
                    TotalItem = total;
                    return;
                }
                if (DataFilter == null && target == FTargetType.Filter) return;
                var mess = await GetDataSet(target, isLog);
                if (mess.Success == 1)
                {
                    var data = mess.ToDataSet();
                    await FReportToolbar.TryCatchMessage(Root, Root, data, 1,
                        async (dt) =>
                        {
                            CacheDeviceID.RemoveCache();
                            var total = GetTotalRecord(data);
                            Method.AddDataDetails(data, DetailIndex, DetailIndex != -1 && totalItem == -1);
                            Method.AddDataExtend(data, ExtendIndex);
                            Method.AddDataSource(data, DataIndex, total, this);
                            TotalItem = total;
                            isFirst = false;
                            await Task.CompletedTask;
                        },
                         async () => await Navigation.PopAsync()
                    );
                }
                else
                {
                    await Navigation.PopAsync();
                    MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private async Task AddDataGridCache(FTargetType target, bool isLog)
        {
            try
            {
                if (Root.InputData != null)
                {
                    DataIndex = Settings.TableData;
                    DetailIndex = Settings.TableDetails;
                    ExtendIndex = Settings.TableExtend;

                    CacheDeviceID.RemoveCache();
                    Root.InputData.Encode().SetCache(CacheDeviceID);

                    Method.AddDataDetails(Root.InputData, DetailIndex, DetailIndex != -1);
                    Method.AddDataExtend(Root.InputData, ExtendIndex);
                    Method.AddDataSource(this);
                    TotalItem = DataCache.Length;
                    return;
                }
                if (DataFilter == null && target == FTargetType.Filter) return;
                if (isFirst)
                {
                    var mess = await GetDataSet(target, isLog);
                    if (mess.Success == 1)
                    {
                        var dataString = mess.Message.AESDecrypt(FSetting.NetWorkKey);
                        var data = dataString.ToDataSet();

                        await FReportToolbar.TryCatchMessage(Root, Root, data, 1,
                           async (dt) =>
                           {
                               dataString.SetCache(CacheDeviceID);
                               Method.AddDataDetails(data, DetailIndex, DetailIndex != -1);
                               Method.AddDataExtend(data, ExtendIndex);
                               Source = new FDataObservation();
                               Method.AddDataSource(this);
                               TotalItem = DataCache.Length;
                               isFirst = false;
                               await Task.CompletedTask;
                           },
                           async () => await Navigation.PopAsync()
                       );
                    }
                    else
                    {
                        await Navigation.PopAsync();
                        MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                    }
                }
                else Method.AddDataSource(this);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        #endregion Data

        #region Create

        public async Task InitView()
        {
            try
            {
                HeightRequest = ListView.HeightRequest = GridView.HeightRequest = FSetting.ScreenHeight;

                ListView = GridStyle.InitListView();
                ListView.LoadMoreOption = Settings.ReportType == ReportType.Loadmore ? LoadMoreOption.AutoOnScroll : LoadMoreOption.None;
                ListView.LoadMoreCommand = new Command<object>(Method.LoadMoreItems(AddNewData), Method.CanLoadMoreItems);
                ListView.LoadMoreTemplate = GridStyle.TemplateBusyLoadMore(this);
                ListView.BindingContext = this;
                ListView.SetBinding(SfListView.ItemsSourceProperty, SourceProperty.PropertyName);
                ListView.SetBinding(SfListView.IsBusyProperty, IsBusyGridProperty.PropertyName);

                GridView = GridStyle.InitGridView();
                GridView.FrozenColumnsCount = Settings.Freeze;
                GridView.BindingContext = this;
                GridView.SetBinding(SfDataGrid.IsBusyProperty, IsBusyGridProperty.PropertyName);
                GridView.SetBinding(SfDataGrid.ItemsSourceProperty, SourceProperty.PropertyName);

                WebView = GridStyle.GridCustomWebView(this, HtmlProperty.PropertyName, Approval.PageDetailOnNavigating);
                E.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;
                E.Rendering();

                InitEvent();
                InitItemView();
                InitToolbar();
                Success = true;
            }
            catch (Exception ex)
            {
                await Navigation.PopAsync();
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                Success = false;
            }
        }

        protected virtual void InitEvent()
        {
            if (DetailView != null || IsAlowSelect)
            {
                ListView.ItemTapped += OnItemTapped;
                GridView.GridTapped += OnItemTapped;
            }
        }

        protected virtual void InitItemView()
        {
            var view = Settings.Views.Find(x => x.Id == "Item");
            GridType = view == null ? GridType.GridView : view.Type == "WebView" ? GridType.WebView : GridType;
            InitTemplateListView(view);
            InitColumnGridView();
        }

        protected virtual void InitToolbar()
        {
            var button = Settings.Toolbars.FindAll(t => t.IsRecord);
            if (button.Count > 0)
            {
                var count = button.Count;
                var action = new List<Func<object, FData, Task>>();

                button.ForEach(x =>
                {
                    switch (x.Command)
                    {
                        case "Edit":
                            action.Add(Toolbar.EditRecordGrid);
                            break;

                        case "Delete":
                            action.Add(Toolbar.DeleteRecordGrid);
                            break;

                        case "Print":
                            action.Add(Toolbar.PrintRecordGrid);
                            break;

                        case "PrintVoucher":
                            action.Add(Toolbar.PrintVoucherRecordGrid);
                            break;

                        case "XmlDownload":
                            action.Add(Toolbar.XmlDownLoadRecordGrid);
                            break;

                        case "Command":
                            action.Add(Toolbar.CommandRecordGrid);
                            break;
                    }
                });

                ListView.AllowSwiping = true;
                ListView.SwipeOffset = count * 85;
                ListView.RightSwipeTemplate = GridStyle.TemplateGridSwipe(button.ToArray(), action.ToArray(), true);

                GridView.FrozenColumnsCount = Settings.Freeze;
                GridView.Columns.Insert(0, new FCustomColumn
                {
                    MappingName = FData.GetBindingName(FData.CheckStatusName),
                    CellTemplate = GridStyle.TemplateGridSwipe(button.ToArray(), action.ToArray(), false),
                    Width = count * FSetting.HeightRowGrid,
                    MinimumWidth = count * FSetting.HeightRowGrid,
                    MaximumWidth = count * FSetting.HeightRowGrid
                });
            }
        }

        protected virtual void InitColumnGridView()
        {
            Settings.Fields.ForEach(f => { if (!f.Hidden) GridView.Columns.Add(GridStyle.GridColumnView(Settings.Details, f)); });
        }

        protected virtual void InitTemplateListView(FViews itemView)
        {
            ListView.ItemTemplate = new DataTemplate(() =>
            {
                var t = new StackLayout();
                var f = new SfEffectsView();
                var i = itemView != null ? GridStyle.GridCustomView(Settings.Fields, itemView.Row, IsAlowSelect ? FSetting.ScreenWidth - 50 : FSetting.ScreenWidth) : new StackLayout();

                f.TouchUp += (s, e) => f.Reset();
                f.TouchUpEffects = SfEffects.Ripple;
                f.Content = i;

                t.Margin = 0;
                t.Spacing = 0;
                t.Children.Add(DeviceInfo.Platform == DevicePlatform.iOS ? f : i);
                t.Children.Add(new FLine());

                if (IsAlowSelect)
                {
                    var b = new FCheckBox(true);
                    var g = new Grid();

                    b.SetBinding(FCheckBox.IsCheckedProperty, FData.GetBindingName(FData.CheckStatusName));

                    g.ColumnSpacing = 0;
                    g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    g.Children.Add(b, 0, 0);
                    g.Children.Add(t, 1, 0);
                    return g;
                }
                return t;
            });
        }

        protected virtual Task InitAttachment()
        {
            Root.Attachment = new ObservableCollection<FMenuButtonReport>();
            if (ExtenderData == null || ExtenderData.Rows.Count == 0)
            {
                Root.IsVisibleAttachmentView = false;
                Root.AttachmentView = null;
                return Task.CompletedTask;
            }
            var column = ExtenderData.Columns;
            ExtenderData.Rows.ForEach<System.Data.DataRow>(x =>
            {
                var toolbar = new FToolbar();
                var file = new FMenuButtonReport();

                if (column.Contains("file_ext")) toolbar.Command = FUtility.GetContentType(x["file_ext"].ToString());
                if (column.Contains("file_name")) toolbar.Title = x["file_name"].ToString();
                var table = ExtenderData.Clone();
                table.ImportRow(x);
                toolbar.Data = table;
                file.Action = DownLoadAttachment; file.Toolbar = toolbar; file.Visible = true; file.Enable = true;
                Root.Attachment.Add(file);
            });
            Root.IsVisibleAttachmentView = Root.Attachment.Count > 0;
            Root.AttachmentView = Root.ReportStyle.InitAttachmentView(Root.Attachment, 50);
            return Task.CompletedTask;
        }

        protected virtual Task InitComment()
        {
            var view = Settings.Views.Find(v => v.Id == "Comment");
            if (view == null || view.Row.Count == 0)
            {
                Root.IsVisibleCommentView = false;
                hasComment = false;
                return Task.CompletedTask;
            }
            var comment = Settings.Fields.Find(x => x.Name == FFunc.ReplaceBinding(view.Row[0].Value));
            if (comment == null || Source.Count == 0)
            {
                Root.IsVisibleCommentView = false;
                hasComment = false;
                return Task.CompletedTask;
            }
            var rootController = Root.BeforePage == null ? "" : Root.BeforePage.Controller;
            var cacheText = Source[0][comment.CacheName];

            E.Title = comment.Title;
            E.TitleColor = string.IsNullOrEmpty(comment.TitleColor) ? FSetting.TextColorTitle : Color.FromHex(comment.TitleColor);
            E.MaxLength = comment.MaxLength;
            E.CacheName = cacheText == null ? string.Empty : $"FastMobile.FXamarin.Core.GridBase.{rootController}.{Controller}.{cacheText}";
            if (cacheText != null) FApprovalComment.AddSetting(E.CacheInput);
            E.RequestAction = string.Empty;
            E.HandleField = new List<string>();
            E.ScriptReference = new List<string>();
            E.InitValue(true);
            E.Unfocus();

            Root.IsVisibleCommentView = true;
            hasComment = true;
            Root.CommentView = E;
            return Task.CompletedTask;
        }

        #endregion Create

        #region private

        private int GetTotalRecord(DataSet data)
        {
            return Int32.Parse(data.Tables[DetailIndex].Rows[0][TotalRecordName].ToString());
        }

        private async Task<FMessage> GetDataSet(FTargetType target, bool isLog)
        {
            return target == FTargetType.Grid ? await Command.GetDataSet(this, isLog) : target == FTargetType.SearchBar ? await Command.Searching(this, isLog) : await Command.Prossessing(this, isLog);
        }

        private ReportCacheType GetTypeReportData(FTargetType target)
        {
            if (target == FTargetType.Grid)
            {
                return Settings.ReportCacheType == ReportCacheType.Dynamic ? ReportCacheType.None : Settings.ReportCacheType;
            }
            if (target == FTargetType.Filter)
            {
                var fCache = Root.Filter.Settings.ReportCacheType;
                var gCache = Settings.ReportCacheType;
                return fCache == ReportCacheType.Dynamic ? gCache == ReportCacheType.Dynamic ? ReportCacheType.None : gCache : fCache;
            }
            return Settings.SearchCache;
        }

        private async Task<bool> NextBackRecord(bool isNext, bool isDelete)
        {
            if (Root.BeforePage == null) return false;
            var grid = Root.BeforePage.Grid;
            var index = grid.GridView.SelectedIndex;
            if (index < 0 || index > grid.Source.Count) return false;
            if (isDelete)
            {
                var ds = grid.CacheDeviceID.GetCache().ToDataSet();
                grid.DeleteItem(ref ds, index);
                ds.Encode().SetCache(grid.CacheDeviceID);
            }
            else grid.GridView.SelectedIndex = index = isNext ? index + 1 : index - 1;
            if (index < 0 || index > grid.Source.Count) return false;
            await Root.SetBusy(true);
            var mess = await FCommand.Showing(grid.Settings.Fields, grid.Source[index - 1], grid.Controller);
            if (mess.Success == 1)
            {
                Root.InputData = mess.ToDataSet();
                Refresh();
                FFunc.CatchScriptMethod(Root, Root.InputData);
                NextBackUpdateView();
            }
            else MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
            await Root.SetBusy(false);
            return true;
        }

        private void NextBackUpdateView()
        {
            Root.MenuButton.ForEach(x =>
            {
                if (x.Toolbar.Command == "Next" || x.Toolbar.Command == "Back") x.Enable = CheckNextBackToobarEnable(x.Toolbar.Command == "Next");
            });
            Root.RefreshMenuView();
        }

        #endregion private
    }
}