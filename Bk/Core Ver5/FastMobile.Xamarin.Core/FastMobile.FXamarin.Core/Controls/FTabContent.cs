using Syncfusion.ListView.XForms;
using Syncfusion.SfPullToRefresh.XForms;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xamarin.Forms;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace FastMobile.FXamarin.Core
{
    public class FTabContent : ContentView, IFScroll, IFRefresh
    {
        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create("IsRefreshing", typeof(bool), typeof(FTabContent), false);

        public bool IsRefreshing
        {
            get => (bool)GetValue(IsRefreshingProperty);
            set => SetValue(IsRefreshingProperty, value);
        }

        public int Table { get; set; }
        public int CountPerPage { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Group { get; set; }
        public FWebViewType Type { get; set; }
        public IFBadge Badge { get; set; }
        public FItemNotify CurrentSwipe { get; private set; }

        public FNotifyGroupModel Model
        {
            get => L.BindingContext as FNotifyGroupModel;
            private set => L.BindingContext = value;
        }

        public bool IsBottom { get; private set; }
        public bool IsTop { get; private set; }

        public event EventHandler<ItemTappedEventArgs> ItemTabbed;

        public event EventHandler<SwipeStartedEventArgs> SwipeStarted;

        public event EventHandler Refreshing;

        public event EventHandler ContentScrolled;

        public event EventHandler Removing, Removed, Reading, Readed;

        private readonly Grid G;
        private readonly FPullToRefresh P;
        private readonly SfListView L;
        private readonly View E;

        public FTabContent() : base()
        {
            G = new Grid();
            P = new FPullToRefresh();
            L = new SfListView();
            E = new FTLEmptyView(FText.NoData);
            Model = new FNotifyGroupModel();
            InitBase();
        }

        public virtual Task Init()
        {
            return InitModel();
        }

        public Task Refresh()
        {
            Model = new FNotifyGroupModel();
            InitLoadMore();
            return InitModel();
        }

        public async Task SetRefresh(bool value, int miliseconds = 400)
        {
            IsRefreshing = value;
            if (value) await Task.Delay(miliseconds);
        }

        public void InitModelProperty(string action, string controller, string loadChannel, string group, int countPerPage, int table, FWebViewType type, IFBadge badge)
        {
            Action = action;
            Controller = controller;
            Group = group;
            CountPerPage = countPerPage;
            Table = table;
            Type = type;
            Badge = badge;
            InitChannel(loadChannel);
            Model.InitProperty(action, controller, group, countPerPage, table, type, badge);
        }

        public void ResetSwipe(bool animation = true)
        {
            L.ResetSwipe(animation);
        }

        public void SetSwipeTemplate(DataTemplate template)
        {
            L.RightSwipeTemplate = template;
        }

        public virtual void InitChannel(string loadChannel)
        {
            MessagingCenter.Subscribe<FNotifyInformation>(this, loadChannel, (sender) => Device.BeginInvokeOnMainThread(async () => await Model.LoadOne(sender)));
        }

        public virtual Task<bool> ScrollToTop()
        {
            if (IsTop)
                return Task.FromResult(false);
            IsTop = true;
            L.ScrollTo(0);
            IsTop = false;
            return Task.FromResult(true);
        }

        public virtual Task<bool> ScrollToBot()
        {
            if (IsBottom)
                return Task.FromResult(false);
            IsBottom = true;
            L.ScrollTo(L.Height);
            IsBottom = false;
            return Task.FromResult(true);
        }

        protected virtual void OnRefreshing(object sender, EventArgs e)
        {
            Refreshing?.Invoke(this, e);
        }

        protected virtual ImageSource Icon(string group)
        {
            return "BullhornOutline".ToImageSource(FSetting.PrimaryColor, FSetting.SizeButtonIcon);
        }

        protected virtual void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            ItemTabbed?.Invoke(sender, e);
        }

        protected virtual void SwipeStart(object sender, SwipeStartedEventArgs e)
        {
            CurrentSwipe = e.ItemData as FItemNotify;
            SwipeStarted?.Invoke(sender, e);
        }

        protected virtual void OnContentScrolled(object sender, ScrollStateChangedEventArgs e)
        {
            ContentScrolled?.Invoke(sender, e);
        }

        protected virtual void OnRemoving(object sender, EventArgs e)
        {
            Removing?.Invoke(sender, e);
        }

        protected virtual void OnRemoved(object sender, EventArgs e)
        {
            Removed?.Invoke(sender, e);
        }

        protected virtual void OnReading(object sender, EventArgs e)
        {
            Reading?.Invoke(sender, e);
        }

        protected virtual void OnReaded(object sender, EventArgs e)
        {
            Readed?.Invoke(sender, e);
        }

        private async Task InitModel()
        {
            Model.InitProperty(Action, Controller, Group, CountPerPage, Table, Type, Badge);
            Model.DataSource.CollectionChanged += ModelCollectionChanged;
            Model.RequestSuccess += ModelRequestSuccess;
            Model.Icon = Icon;
            await Model.Load();
            L.ItemsSource = Model.DataSource;
            L.LoadMoreCommand = Model.LoadMoreItemsCommand;
        }

        private void InitBase()
        {
            InitEmptyView();
            InitListView();
            InitLoadMore();
            InitPullToRefresh();
            InitGrid();
            Content = P;
        }

        private void InitEmptyView()
        {
        }

        private void InitListView()
        {
            L.LayoutManager = new GridLayout { SpanCount = 1, ItemsCacheLimit = 20 };
            L.ItemSpacing = 0;
            L.AllowSwiping = true;
            L.IsScrollBarVisible = false;
            L.SwipeOffset = 200;
            L.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            L.AutoFitMode = AutoFitMode.DynamicHeight;
            L.ItemTemplate = new DataTemplate(typeof(FTLNotify));
            L.ItemTapped += OnItemTapped;
            L.SwipeStarted += SwipeStart;
            L.ScrollStateChanged += OnContentScrolled;
        }

        private void InitLoadMore()
        {
            L.LoadMoreOption = LoadMoreOption.AutoOnScroll;
            L.LoadMorePosition = LoadMorePosition.Bottom;
            L.LoadMoreTemplate = new DataTemplate(typeof(FTLBusyLoadMore));
            L.SetBinding(SfListView.IsBusyProperty, FModelBase.IsBusyProperty.PropertyName, BindingMode.TwoWay);
        }

        private void InitPullToRefresh()
        {
            P.BindingContext = this;
            P.PullableContent = G;
            P.Refreshing += OnRefreshing;
            P.SetBinding(FPullToRefresh.IsRefreshingProperty, IsRefreshingProperty.PropertyName, BindingMode.TwoWay);
        }

        private void InitGrid()
        {
            G.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            G.Children.Add(L, 0, 0);
            G.Children.Add(E, 0, 0);
        }

        private void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            E.IsVisible = Model.ClientCount < 1;
        }

        private void ModelRequestSuccess(object sender, FNotifyRequestSuccessEventArgs e)
        {
            E.IsVisible = e.ClientCount < 1;

            if (e.Data == null || e.Data.Tables.Count < 3 || e.Data.Tables[2].Rows.Count == 0 || !e.Data.Tables[2].Columns.Contains("badge") || !e.Data.Tables[2].Columns.Contains("controller"))
                return;

            Badge.OnReceived(e.Data.Tables[2].Rows[0]["badge"].ToString(), e.Data.Tables[2].Rows[0]["controller"].ToString());
        }

        private void Pulling(object sender, PullingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}