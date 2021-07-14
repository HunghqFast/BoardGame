using Syncfusion.ListView.XForms;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using SelectionMode = Syncfusion.ListView.XForms.SelectionMode;

namespace FastMobile.FXamarin.Core
{
    public class FPageLookup : FPageSearchForms, IFDataHandler
    {
        private bool isFirst;
        private int totalItems;

        public static readonly BindableProperty LookupTypeProperty = BindableProperty.Create("LookupType", typeof(FLookupType), typeof(FPageLookup), FLookupType.None);
        public static readonly BindableProperty DataSourceProperty = BindableProperty.Create("DataSource", typeof(ObservableCollection<FData>), typeof(FPageLookup));
        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create("IsRunning", typeof(bool), typeof(FPageLookup), false);

        public static bool AutoComplete
        {
            get => Convert.ToBoolean("FastMobile.FXamarin.Core.FPageLookup.AutoComplete".GetCache(bool.TrueString));
            set => value.SetCache("FastMobile.FXamarin.Core.FPageLookup.AutoComplete");
        }

        public FInputLookup Root { get; set; }

        public ObservableCollection<FData> DataSource { get => (ObservableCollection<FData>)GetValue(DataSourceProperty); set => SetValue(DataSourceProperty, value); }

        public FLookupType LookupType
        {
            get { return (FLookupType)GetValue(LookupTypeProperty); }
            set { SetValue(LookupTypeProperty, value); List.SelectionMode = LookupType == FLookupType.None ? SelectionMode.Single : SelectionMode.Multiple; }
        }

        public bool IsRunning { get => (bool)GetValue(IsRunningProperty); set => SetValue(IsRunningProperty, value); }

        public int TableData { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Method { get; set; }

        public int ItemPerPage { get; set; }

        public FData LastItem => DataSource[^1];

        public SfListView List => SearchContent as SfListView;

        public List<FData> SelectedsItem { get; set; }

        public event EventHandler<IFDataEvent> ItemTapped;

        public event EventHandler SourceChanged;

        public FPageLookup(string controller, string value, FFormTarget target) : base(true, false)
        {
            SelectedsItem = new List<FData>();
            isFirst = true;
            totalItems = -1;

            InitLookup(controller, value, target);

            SearchContent = new SfListView();
            DataSource = new ObservableCollection<FData>();
            List.VerticalOptions = LayoutOptions.StartAndExpand;
            List.SelectionBackgroundColor = Color.Transparent;
            List.SelectionGesture = TouchGesture.Tap;
            List.AutoFitMode = AutoFitMode.Height;
            List.ItemTemplate = ItemTemplate();
            List.IsScrollBarVisible = false;
            List.LoadMoreOption = LoadMoreOption.AutoOnScroll;
            List.LoadMorePosition = LoadMorePosition.Bottom;
            List.BindingContext = this;
            List.LayoutManager = new GridLayout { SpanCount = 1, ItemsCacheLimit = 60 };
            List.LoadMoreCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);
            List.LoadMoreTemplate = TemplateBusyLoadMore();
            List.SetBinding(SfListView.ItemsSourceProperty, DataSourceProperty.PropertyName);
            List.SetBinding(SfListView.IsBusyProperty, IsRunningProperty.PropertyName);
            SearchBarTextSubmit += SearchLookup;
        }

        #region Public

        public async Task ResetSelected(string searchText = null)
        {
            SearchText = searchText ?? string.Empty;
            SelectedsItem.Clear();
            await Start(false);
        }

        public async void RefreshView()
        {
            await SetBusy(true);
            DataSource.ForEach(x => { x.IsCheck = SetSelected(x); });
            await SetBusy(false);
            await Task.CompletedTask;
        }

        public async Task<bool> CheckExist(string searchText)
        {
            SearchText = searchText;
            var message = await GetData(1);
            if (message.Success == 1)
            {
                try
                {
                    var data = message.ToDataSet();
                    if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                    {
                        Root.Value = new FItem(searchText, string.Empty);
                        return false;
                    }
                    var row = data.Tables[0].Rows[0];
                    Root.Value = new FItem(row[0], row[1]);
                    Root.SetReference(row);
                    return true;
                }
                catch
                {
                    Root.Value = new FItem(searchText, string.Empty);
                    return false;
                }
            }
            Root.Value = new FItem(searchText, string.Empty);
            return false;
        }

        #endregion Public

        #region Private

        private async Task Start(bool isRefreshing = false)
        {
            if (isRefreshing) await SetRefresh(true);
            await SetBusy(true);
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(FMessage.FromFail(403), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
                if (isRefreshing) await SetRefresh(false);
                return;
            }
            totalItems = -1;
            DataSource = new ObservableCollection<FData>();
            await AddNewLookup();
            isFirst = false;
            await SetBusy(false);
            if (isRefreshing) await SetRefresh(false);
        }

        private void SearchLookup(object sender, EventArgs e)
        {
            _ = Start(false);
        }

        private bool CanLoadMoreItems(object arg)
        {
            return !(totalItems <= DataSource.Count || Connectivity.NetworkAccess != NetworkAccess.Internet);
        }

        private async void LoadMoreItems(object obj)
        {
            try
            {
                IsRunning = true;
                await AddNewLookup();
            }
            catch { MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE); }
            finally { IsRunning = false; }
        }

        private async Task AddNewLookup()
        {
            var message = await GetData();
            if (message.Success == 1)
            {
                try
                {
                    var data = message.ToDataSet();
                    AddDataSource(data.Tables[TableData]);
                    if (totalItems == -1) totalItems = Convert.ToInt32(data.Tables[0].Rows[0]["count"]);
                    SourceChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex) { MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE); }
            }
            else MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
        }

        public async void OnItemTapped(object sender, IFDataEvent e)
        {
            ItemTapped?.Invoke(sender, e);
            if (e.ItemData is not FData item) return;
            if (LookupType == FLookupType.None)
            {
                if (SelectedsItem.Count == 0)
                {
                    item.IsCheck = true;
                    SelectedsItem.Add(item);
                    if (AutoComplete) await Navigation.PopAsync();
                }
                else if (SelectedsItem[0][Root.TargetName].ClearString() == item[Root.TargetName].ClearString())
                {
                    item.IsCheck = false;
                    SelectedsItem.Clear();
                }
                else
                {
                    SelectedsItem[0].IsCheck = false;
                    item.IsCheck = true;
                    SelectedsItem[0] = item;
                    if (AutoComplete) await Navigation.PopAsync();
                }
                return;
            }
            var check = item.IsCheck;
            item.IsCheck = !check;
            if (check)
            {
                var du = SelectedsItem.Find(s => s[Root.TargetName] == item[Root.TargetName]);
                if (du != null) SelectedsItem.Remove(du);
            }
            else SelectedsItem.Add(item);
        }

        private void AddDataSource(DataTable data)
        {
            var source = DataSource;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                var ro = data.Rows[i];
                var it = new FData();

                it[Root.TargetName, FieldType.String] = ro[Root.TargetName];
                it[Root.Reference[0], FieldType.String] = ro[1];
                for (int j = 1; j < Root.Reference.Count; j++)
                    it[Root.Reference[j], FieldType.String] = ro[j + 1];
                it.IsCheck = SetSelected(it);
                source.Add(it);
            }
            DataSource = source;
        }

        private bool SetSelected(FData item)
        {
            var value = item[Root.TargetName].ClearString();
            if (LookupType == FLookupType.None)
            {
                if (Root.Value.I.ClearString() == value)
                {
                    if (SelectedsItem.Count > 0) SelectedsItem[0] = item;
                    else SelectedsItem.Add(item);
                    return true;
                }
            }
            else
            {
                var values = FFunc.GetArrayString(Root.Value.I.ToLower());
                var value2 = SelectedsItem.Select(s => s[Root.TargetName].ClearString()).ToList();
                if (values.Contains(value) || value2.Contains(value))
                {
                    if (value2.Contains(value)) SelectedsItem[value2.IndexOf(value)] = item;
                    else SelectedsItem.Add(item);
                    return true;
                }
            }
            return false;
        }

        private DataTemplate TemplateBusyLoadMore()
        {
            return new DataTemplate(() =>
            {
                var gr = new FTLBusyLoadMore().GetView(IsRunningProperty.PropertyName);
                gr.BindingContext = this;
                return gr;
            });
        }

        private DataTemplate ItemTemplate()
        {
            return new DataTemplate(() =>
            {
                var none = LookupType == FLookupType.None;
                var view = new FButtonEffect(List, OnItemTapped);
                var grid = new Grid();
                var cbox = new FCheckBox(!none);
                var line = new FLine();

                cbox.SetBinding(ToggleButton.IsCheckedProperty, FData.GetBindingName(FData.CheckStatusName), mode: BindingMode.TwoWay);
                if (none) cbox.SetBinding(IsVisibleProperty, FData.GetBindingName(FData.CheckStatusName));

                grid.ColumnSpacing = 0;
                grid.RowSpacing = 0;
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(none ? 0 : 45) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(none ? 30 : 0) });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(SetItemView(), 1, 0);
                grid.Children.Add(cbox, none ? 2 : 0, 0);
                grid.Children.Add(line, 0, 1);
                Grid.SetColumnSpan(line, 3);

                view.Content = grid;
                return view;
            });
        }

        private async Task<FMessage> GetData(int isCheck = 0)
        {
            var ds = new DataSet();
            ds.AddTable(new DataTable().AddRowValue("numberItems", ItemPerPage).AddRowValue(0, "lastItem", DataSource.Count == 0 ? "" : LastItem[Root.TargetName]).AddRowValue(0, "searchValue", SearchText.Trim()).AddRowValue(0, "isCheck", isCheck));
            if (Root.ListInput.Count > 0)
            {
                var dt2 = new DataTable();
                foreach (var i in Root.ListInput)
                {
                    var root = FInput.IsRootParam(i) ? (Root.Root.Root as FInputGrid).Root.Input : Root.Root.Input;
                    var name = FInput.RootParam(i);
                    dt2.AddRowValue(0, name, root.TryGetValue(name, out FInput input) ? GetInputLookup(input) : string.Empty);
                }
                ds.Tables.Add(dt2);
            }

            var mess = await FServices.ExecuteCommand(Action, Controller, ds, Method, null, true);
            ds.Dispose();
            return mess;
        }

        private object GetInputLookup(FInput input)
        {
            var value = input.GetInput(0);
            if (input.Type == FieldType.DateTime) return FInputDate.GetRequestValue(value);
            return value;
        }

        #endregion Private

        #region Protected

        protected override void OnRefreshing(object sender, EventArgs e)
        {
            base.OnRefreshing(sender, e);
            _ = Start(true);
        }

        protected virtual void InitLookup(string controller, string value, FFormTarget target)
        {
            Title = FText.FilterCondition;
            Placeholder = FText.QuickFind;
            Controller = $"{controller}.Lookup";
            Action = $"{value}.Lookup.{target}";
            Method = "600";
            ItemPerPage = 20;
        }

        protected virtual View SetItemView()
        {
            var st = new StackLayout();
            var l1 = new Label();
            var l2 = new Label();

            l1.FontFamily = FSetting.FontTextMedium;
            l1.WidthRequest = FSetting.ScreenWidth * 0.3;
            l1.FontSize = FSetting.FontSizeLabelContent;
            l1.TextColor = FSetting.TextColorTitle;
            l1.VerticalTextAlignment = TextAlignment.Center;
            l1.VerticalOptions = LayoutOptions.Center;
            l1.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            l1.SetBinding(Label.TextProperty, FData.GetBindingName(Root.TargetName));
            l1.HeightRequest = 25;

            l2.FontFamily = FSetting.FontText;
            l2.WidthRequest = FSetting.ScreenWidth * 0.7;
            l2.MaxLines = 5;
            l2.FontSize = FSetting.FontSizeLabelContent;
            l2.TextColor = FSetting.TextColorTitle;
            l2.VerticalTextAlignment = TextAlignment.Center;
            l2.VerticalOptions = LayoutOptions.Center;
            l2.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            l2.SetBinding(Label.TextProperty, FData.GetBindingName(Root.Reference[0]), converter: new FStringNoNullConvert());

            st.Margin = 0;
            st.Padding = new Thickness(10, 10, 0, 10);
            st.VerticalOptions = LayoutOptions.CenterAndExpand;
            st.HorizontalOptions = LayoutOptions.CenterAndExpand;
            st.Spacing = 5;
            st.Orientation = StackOrientation.Horizontal;
            st.Children.Add(l1);
            st.Children.Add(l2);
            return st;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (isFirst) _ = Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (SelectedsItem.Count == 0)
            {
                if (LookupType == FLookupType.None && !DataSource.Any(x => x[Root.TargetName].ClearString() == Root.Value.I.ClearString())) return;
                Root.Value = FItem.Empty;
                Root.IsValidValue = true;
                return;
            }

            FData data;
            if (LookupType == FLookupType.None) data = SelectedsItem[0];
            else
            {
                data = new FData();
                data[Root.TargetName, FieldType.String] = string.Join(',', SelectedsItem.Select(e => e[Root.TargetName].ToString().Trim()));
                data[Root.Reference[0], FieldType.String] = string.Empty;
            }

            Root.IsValidValue = true;
            if (Root.Value.I.ClearString() == data[Root.TargetName].ClearString())
            {
                Root.IsCommpleted = false;
                Root.Annotation = data[Root.Reference[0]]?.ToString();
                return;
            }
            Root.Value = new FItem(data[Root.TargetName], data[Root.Reference[0]]);
            Root.SetReference(data);
            Root.IsCommpleted = true;
        }

        #endregion Protected
    }
}