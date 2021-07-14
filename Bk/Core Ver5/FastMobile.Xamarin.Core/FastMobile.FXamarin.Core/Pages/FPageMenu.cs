using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageMenu : FPage, IFDataHandler
    {
        public static readonly BindableProperty GridIconSourceProperty = BindableProperty.Create("GridIconSource", typeof(ImageSource), typeof(FPageMenu));
        public static readonly BindableProperty ListIconSourceProperty = BindableProperty.Create("ListIconSource", typeof(ImageSource), typeof(FPageMenu));
        public static readonly BindableProperty ViewTypeProperty = BindableProperty.Create("ViewType", typeof(FMenuViewType), typeof(FPageMenu));
        public static readonly BindableProperty TurnOnExpandProperty = BindableProperty.Create("TurnOnExpand", typeof(bool), typeof(FPageMenu), false);

        public ImageSource GridIconSource
        {
            get => (ImageSource)GetValue(GridIconSourceProperty);
            set => SetValue(GridIconSourceProperty, value);
        }

        public ImageSource ListIconSource
        {
            get => (ImageSource)GetValue(ListIconSourceProperty);
            set => SetValue(ListIconSourceProperty, value);
        }

        public FMenuViewType ViewType
        {
            get => (FMenuViewType)GetValue(ViewTypeProperty);
            protected set => SetValue(ViewTypeProperty, value);
        }

        public bool TurnOnExpand
        {
            get => (bool)GetValue(TurnOnExpandProperty);
            set => SetValue(TurnOnExpandProperty, value);
        }

        public event EventHandler<EventArgs> ViewTypeClicked;

        public event EventHandler<IFDataEvent> ItemTapped;

        public readonly string Group;

        public ObservableCollection<FItemMenu> ItemsSource
        {
            get => MenuLayout.ItemsSource as ObservableCollection<FItemMenu>;
            set
            {
                value.CollectionChanged += ItemSourceCollectionChanged;
                MenuLayout.ItemsSource = value;
            }
        }

        private readonly FGridMenu MenuLayout;
        private readonly ToolbarItem Viewer;
        private DateTime LastTabbed;

        public FPageMenu(string group, bool isHasPullToRefresh, bool enableScroll = true) : base(isHasPullToRefresh, enableScroll)
        {
            Group = group;
            LastTabbed = DateTime.Now;
            Viewer = new ToolbarItem();
            MenuLayout = new FGridMenu(this) { BindingContext = this };
            MenuLayout.SetBinding(SfListView.HeightRequestProperty, HeightProperty.PropertyName);
            GridIconSource = FIcons.ViewGridOutline.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
            ListIconSource = FIcons.FormatListBulleted.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
            MessagingCenter.Subscribe<FMessage>(this, FChannel.NOTIFYRECEIVED, NotifyReceived);
        }

        public void OnLayout()
        {
            MenuLayout.InitMenu(ViewType, OnItemTapped);
        }

        public void RefreshView()
        {
            MenuLayout.RefreshView();
        }

        public void UpdateMenuView()
        {
            ShowNothing = false;
            Content = MenuLayout;
        }

        public override async Task<bool> ScrollToTop()
        {
            if (await base.ScrollToTop()) MenuLayout.ScrollTo(0);
            return false;
        }

        public void CacheViewType(FMenuViewType type, string keyWord)
        {
            type.SetCache($"FastMobile.FXamarin.Core.FPageMenu.ViewType: {keyWord}");
        }

        public FMenuViewType MenuViewType(string keyWord, FMenuViewType defaultType)
        {
            var type = $"FastMobile.FXamarin.Core.FPageMenu.ViewType: {keyWord}".GetCache();
            return string.IsNullOrEmpty(type) ? defaultType : (FMenuViewType)Enum.Parse(typeof(FMenuViewType), type);
        }

        public void OnItemTapped(object sender, IFDataEvent e)
        {
            if (e.ItemData is not FItemMenu item)
                return;
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(new FMessage(0, 403, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            if (LastTabbed.AddSeconds(1) > DateTime.Now)
                return;
            LastTabbed = DateTime.Now;
            ItemTapped?.Invoke(sender, e);
        }

        public virtual void NotifyReceived(FMessage message)
        {
        }

        protected virtual void OnViewTypeClicked(object sender, EventArgs e)
        {
            ViewType = ViewType == FMenuViewType.Grid ? FMenuViewType.List : FMenuViewType.Grid;
            ViewTypeClicked?.Invoke(sender, e);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ViewTypeProperty.PropertyName)
            {
                Viewer.IconImageSource = ViewType == FMenuViewType.Grid ? ListIconSource : GridIconSource;
                return;
            }
        }

        protected void InitToolbar()
        {
            Viewer.IconImageSource = ViewType == FMenuViewType.Grid ? ListIconSource : GridIconSource;
            Viewer.Clicked += OnViewTypeClicked;
        }

        private void ItemSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource.Count == 0)
                ToolbarItems.Clear();
            else if (ItemsSource.Count > 0 && !ToolbarItems.Contains(Viewer))
                ToolbarItems.Add(Viewer);
        }
    }
}