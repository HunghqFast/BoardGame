using Syncfusion.XForms.Core;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ScrollView = Xamarin.Forms.ScrollView;

namespace FastMobile.FXamarin.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class FPage : ContentPage, IFLayout, IFScroll, IFNetwork, IFRefresh, IFBusy
    {
        public static new readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(FPage));
        public static readonly BindableProperty BusyBackgroundOpacityProperty = BindableProperty.Create("BusyBackgroundOpacity", typeof(double), typeof(FPage), 0d);
        public static readonly BindableProperty BusyBackgroundColorProperty = BindableProperty.Create("BusyBackgroundColor", typeof(Color), typeof(FPage), Color.Black);
        public static readonly BindableProperty BusyColorProperty = BindableProperty.Create("BusyColor", typeof(Color), typeof(FPage), Color.Default);
        public static readonly BindableProperty NothingBackgroundColorProperty = BindableProperty.Create("NothingBackgroundColor", typeof(Color), typeof(FPage), Color.Default);
        public static readonly BindableProperty BusySizeProperty = BindableProperty.Create("BusySize", typeof(double), typeof(FPage), 40d);
        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create("IsRefreshing", typeof(bool), typeof(FPage), false);
        public static readonly BindableProperty PopupAutoSizeModeProperty = BindableProperty.Create("PopupAutoSizeMode", typeof(AutoSizeMode), typeof(FPage), AutoSizeMode.Height);
        public static readonly BindableProperty PopupWidthProperty = BindableProperty.Create("PopupWidth", typeof(double), typeof(FPage), -1d);
        public static readonly BindableProperty PopupHeightProperty = BindableProperty.Create("PopupHeight", typeof(double), typeof(FPage), -1d);
        public static readonly BindableProperty PopupTemplateProperty = BindableProperty.Create("PopupTemplate", typeof(DataTemplate), typeof(FPage));
        public static readonly BindableProperty ShowNothingProperty = BindableProperty.Create("ShowNothing", typeof(bool), typeof(FPage), false);
        public static readonly BindableProperty NothingTextProperty = BindableProperty.Create("NothingText", typeof(string), typeof(FPage));
        public static readonly BindableProperty NothingContentProperty = BindableProperty.Create("NothingContent", typeof(View), typeof(FPage));
        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create("TitleColor", typeof(Color), typeof(FPage), Color.White);

        public new View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public double BusyBackgroundOpacity
        {
            get => (double)GetValue(BusyBackgroundOpacityProperty);
            set => SetValue(BusyBackgroundOpacityProperty, value);
        }

        public Color BusyBackgroundColor
        {
            get => (Color)GetValue(BusyBackgroundColorProperty);
            set => SetValue(BusyBackgroundColorProperty, value);
        }

        public Color BusyColor
        {
            get => (Color)GetValue(BusyColorProperty);
            set => SetValue(BusyColorProperty, value);
        }

        public Color NothingBackgroundColor
        {
            get => (Color)GetValue(NothingBackgroundColorProperty);
            set => SetValue(NothingBackgroundColorProperty, value);
        }

        public double BusySize
        {
            get => (double)GetValue(BusySizeProperty);
            set => SetValue(BusySizeProperty, value);
        }

        public bool IsRefreshing
        {
            get => (bool)GetValue(IsRefreshingProperty);
            set => SetValue(IsRefreshingProperty, value);
        }

        public AutoSizeMode PopupAutoSizeMode
        {
            get => (AutoSizeMode)GetValue(PopupAutoSizeModeProperty);
            set => SetValue(PopupAutoSizeModeProperty, value);
        }

        public double PopupWidth
        {
            get => (double)GetValue(PopupWidthProperty);
            set => SetValue(PopupWidthProperty, value);
        }

        public double PopupHeight
        {
            get => (double)GetValue(PopupHeightProperty);
            set => SetValue(PopupHeightProperty, value);
        }

        public DataTemplate PopupTemplate
        {
            get => (DataTemplate)GetValue(PopupTemplateProperty);
            set => SetValue(PopupTemplateProperty, value);
        }

        public bool ShowNothing
        {
            get => (bool)GetValue(ShowNothingProperty);
            set => SetValue(ShowNothingProperty, value);
        }

        public string NothingText
        {
            get => (string)GetValue(NothingTextProperty);
            set => SetValue(NothingTextProperty, value);
        }

        public View NothingContent
        {
            get => (View)GetValue(NothingContentProperty);
            set => SetValue(NothingContentProperty, value);
        }

        public Color TitleColor
        {
            get => (Color)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        public bool HasInitialized { get; private set; }
        public bool IsBottom { get; protected set; }
        public bool IsTop { get; protected set; }

        public bool HasNetwork
        {
            get
            {
                if (!FUtility.HasNetwork)
                {
                    Content = new FTLNoInternet(TryConnect);
                    IsBusy = false;
                    return false;
                }
                return true;
            }
        }

        public event EventHandler Refreshing;

        public event EventHandler ConnectNetworkClicked;

        public event EventHandler ContentScrolled;

        public event EventHandler PopupLoaded;

        public event EventHandler PopupOpened;

        public event EventHandler PopupClosed;

        public event EventHandler<CancelEventArgs> PopupOpening;

        public event EventHandler<CancelEventArgs> PopupClosing;

        public event EventHandler Appeared;

        public event EventHandler Disappeared;

        protected readonly bool EnableScroll;

        private readonly ContentView CurrentContent;
        private readonly FPullToRefresh PullToRefresh;
        private readonly ActivityIndicator Busy;
        private readonly StackLayout BusyBackground;
        private readonly SfPopupLayout PopupLayout;
        private readonly ScrollView ScrollContent;
        private readonly Grid GridContent;
        private readonly ContentView NothingView;
        private readonly Label TitleLabel;

        public FPage(bool IsHasPullToRefresh, bool enableScroll) : base()
        {
            EnableScroll = enableScroll;
            FNavigationPage.SetBackButtonTitle(this, "");
            GridContent = new Grid();
            PopupLayout = new SfPopupLayout();
            BusyBackground = new StackLayout();
            Busy = new ActivityIndicator();
            ScrollContent = new ScrollView();
            CurrentContent = new ContentView();
            PullToRefresh = new FPullToRefresh();
            NothingView = new ContentView();
            NothingContent = new Label();
            TitleLabel = new Label();

            BusyColor = FSetting.BusyColor;
            BackgroundColor = FSetting.BackgroundMain;
            NothingText = FText.NoDataNoAccess;
            NothingBackgroundColor = FSetting.BackgroundMain;

            ScrollContent.BindingContext = this;
            ScrollContent.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            ScrollContent.Scrolled += OnContentScrolled;

            GridContent.BindingContext = this;
            GridContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            GridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            GridContent.SetBinding(Grid.BackgroundColorProperty, BackgroundColorProperty.PropertyName);

            CurrentContent.BindingContext = this;
            CurrentContent.SetBinding(ContentView.ContentProperty, ContentProperty.PropertyName);

            BusyBackground.BindingContext = this;
            BusyBackground.HorizontalOptions = BusyBackground.VerticalOptions = LayoutOptions.Fill;
            BusyBackground.SetBinding(StackLayout.BackgroundColorProperty, BusyBackgroundColorProperty.PropertyName);
            BusyBackground.SetBinding(StackLayout.IsVisibleProperty, IsBusyProperty.PropertyName);
            BusyBackground.SetBinding(StackLayout.OpacityProperty, BusyBackgroundOpacityProperty.PropertyName);

            Busy.BindingContext = this;
            Busy.VerticalOptions = LayoutOptions.CenterAndExpand;
            Busy.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Busy.SetBinding(ActivityIndicator.ColorProperty, BusyColorProperty.PropertyName);
            Busy.SetBinding(ActivityIndicator.WidthRequestProperty, BusySizeProperty.PropertyName);
            Busy.SetBinding(ActivityIndicator.HeightRequestProperty, BusySizeProperty.PropertyName);
            Busy.SetBinding(ActivityIndicator.IsRunningProperty, IsBusyProperty.PropertyName);
            Busy.SetBinding(ActivityIndicator.IsVisibleProperty, IsBusyProperty.PropertyName);

            PopupWidth = FSetting.ScreenWidth < 400 ? FSetting.ScreenWidth - 30 : 370;
            PopupLayout.StaysOpen = true;
            PopupLayout.PopupView.AnimationDuration = 100;
            PopupLayout.PopupView.ShowFooter = PopupLayout.PopupView.ShowHeader = PopupLayout.PopupView.ShowCloseButton = false;

            PopupLayout.PopupView.BindingContext = this;
            PopupLayout.PopupView.PopupStyle.CornerRadius = FSetting.IsAndroid ? FSetting.RadiusPopup : PopupLayout.PopupView.PopupStyle.CornerRadius;
            PopupLayout.PopupLayoutLoaded += OnPopupLoaded;
            PopupLayout.Opening += OnPopupOpening;
            PopupLayout.Opened += OnPopupOpened;
            PopupLayout.Closing += OnPopupClosing;
            PopupLayout.Closed += OnPopupClosed;
            PopupLayout.PopupView.SetBinding(PopupView.AutoSizeModeProperty, PopupAutoSizeModeProperty.PropertyName);
            PopupLayout.PopupView.SetBinding(PopupView.HeightRequestProperty, PopupHeightProperty.PropertyName);
            PopupLayout.PopupView.SetBinding(PopupView.WidthRequestProperty, PopupWidthProperty.PropertyName);
            PopupLayout.PopupView.SetBinding(PopupView.ContentTemplateProperty, PopupTemplateProperty.PropertyName);

            if (NothingContent is Label L)
            {
                L.BindingContext = this;
                L.HorizontalOptions = L.VerticalOptions = LayoutOptions.CenterAndExpand;
                L.TextColor = FSetting.TextColorContent;
                L.FontFamily = FSetting.FontText;
                L.FontSize = FSetting.FontSizeLabelContent;
                L.HorizontalTextAlignment = L.VerticalTextAlignment = TextAlignment.Center;
                L.SetBinding(Label.TextProperty, NothingTextProperty.PropertyName);
            }

            NothingView.BindingContext = this;
            NothingView.Padding = new Thickness(20);
            NothingView.HorizontalOptions = NothingView.VerticalOptions = LayoutOptions.Fill;
            NothingView.SetBinding(View.IsVisibleProperty, ShowNothingProperty.PropertyName);
            NothingView.SetBinding(ContentView.ContentProperty, NothingContentProperty.PropertyName);
            NothingView.SetBinding(ContentView.BackgroundColorProperty, NothingBackgroundColorProperty.PropertyName);

            TitleLabel.BindingContext = this;
            TitleLabel.MaxLines = 1;
            TitleLabel.LineBreakMode = LineBreakMode.TailTruncation;
            TitleLabel.VerticalOptions = TitleLabel.HorizontalOptions = LayoutOptions.Center;
            TitleLabel.FontFamily = FSetting.FontTextBold;
            TitleLabel.FontAttributes = FontAttributes.Bold;
            TitleLabel.FontSize = FSetting.IsAndroid ? FSetting.FontSizeLabelTitle + 3 : FSetting.FontSizeLabelTitle + 2;
            TitleLabel.SetBinding(Label.TextColorProperty, TitleColorProperty.PropertyName);
            FNavigationPage.SetTitleView(this, new StackLayout { Children = { TitleLabel }, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center });

            Base(IsHasPullToRefresh);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        public virtual void Init()
        {
            HasInitialized = true;
        }

        public virtual FPage Init(bool setInit)
        {
            HasInitialized = setInit;
            return this;
        }

        public virtual Task OnLoaded()
        {
            if (!HasInitialized)
                Init();
            return Task.CompletedTask;
        }

        public virtual Task OnChanged()
        {
            return Task.CompletedTask;
        }

        public virtual async Task<bool> ScrollToTop()
        {
            if (IsTop)
                return false;
            IsTop = true;
            if (EnableScroll) await ScrollContent?.ScrollToAsync(0, 0, true);
            IsTop = false;
            return true;
        }

        public virtual async Task<bool> ScrollToBot()
        {
            if (IsBottom)
                return false;
            IsBottom = true;
            if (EnableScroll) await ScrollContent?.ScrollToAsync(0, ScrollContent.Height, true);
            IsBottom = false;
            return true;
        }

        public virtual void OnAppeared()
        {
            Appeared?.Invoke(this, EventArgs.Empty);
            TitleLabel.SetBinding(Label.TextProperty, TitleProperty.PropertyName);
        }

        public virtual void OnDisappeared()
        {
            Disappeared?.Invoke(this, EventArgs.Empty);
        }

        public void ShowPopup(bool isFullScreen = false)
        {
            if (PopupTemplate != null)
                PopupLayout.Show(isFullScreen);
        }

        public void HidePopup()
        {
            PopupLayout.IsOpen = false;
        }

        public async Task SetBusy(bool value)
        {
            IsBusy = value;
            await Task.Delay(1);
        }

        public async Task SetRefresh(bool value, int miliseconds = 400)
        {
            IsRefreshing = value;
            if (value) await Task.Delay(miliseconds);
        }

        protected void SetBindingHeightScroll(bool confirm)
        {
            if (confirm)
                ScrollContent.SetBinding(ScrollView.HeightRequestProperty, HeightProperty.PropertyName);
        }

        protected virtual void OnRefreshing(object sender, EventArgs e)
        {
            Refreshing?.Invoke(sender, e);
        }

        protected virtual void OnTabbedTryConnect(object sender, EventArgs e)
        {
            ConnectNetworkClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnContentScrolled(object sender, ScrolledEventArgs e)
        {
            ContentScrolled?.Invoke(sender, e);
            IsTop = e.ScrollY == 0;
            IsBottom = e.ScrollY == ScrollContent.Height;
        }

        protected virtual void OnPopupLoaded(object sender, EventArgs e)
        {
            PopupLoaded?.Invoke(this, e);
        }

        protected virtual void OnPopupClosed(object sender, EventArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        protected virtual void OnPopupOpened(object sender, EventArgs e)
        {
            PopupOpened?.Invoke(this, e);
        }

        protected virtual void OnPopupOpening(object sender, CancelEventArgs e)
        {
            PopupOpening?.Invoke(this, e);
        }

        protected virtual void OnPopupClosing(object sender, CancelEventArgs e)
        {
            PopupClosing?.Invoke(this, e);
        }

        protected override void OnAppearing()
        {
            (ToolbarItems as ObservableCollection<ToolbarItem>).CollectionChanged += OnToolbarItemCollectionChanged;
            UpdateTitleMargin();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (ToolbarItems as ObservableCollection<ToolbarItem>).CollectionChanged -= OnToolbarItemCollectionChanged;
            base.OnDisappearing();
        }

        private void Base(bool isHasPullToRefresh)
        {
            if (EnableScroll)
            {
                ScrollContent.Content = CurrentContent;
                GridContent.Children.Add(ScrollContent, 0, 0);
            }
            else GridContent.Children.Add(CurrentContent, 0, 0);

            GridContent.Children.Add(NothingView, 0, 0);
            GridContent.Children.Add(BusyBackground, 0, 0);
            GridContent.Children.Add(Busy, 0, 0);

            if (isHasPullToRefresh)
            {
                PullToRefresh.BindingContext = this;
                PullToRefresh.Refreshing += OnRefreshing;
                PullToRefresh.PullableContent = GridContent;
                PullToRefresh.SetBinding(FPullToRefresh.IsRefreshingProperty, IsRefreshingProperty.PropertyName);

                PopupLayout.Content = PullToRefresh;
            }
            else PopupLayout.Content = GridContent;
            base.Content = PopupLayout;
        }

        private void TryConnect()
        {
            OnTabbedTryConnect(this, EventArgs.Empty);
        }

        private void OnToolbarItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTitleMargin();
        }

        private void UpdateTitleMargin()
        {
            if (FNavigationPage.GetHasBackButton(this) && ToolbarItems.Count == 0)
            {
                TitleLabel.Margin = new Thickness(0, 0, FSetting.IsAndroid ? 65 : 35, 0);
                return;
            }
            if (!FNavigationPage.GetHasBackButton(this) && ToolbarItems.Count == 1)
            {
                TitleLabel.Margin = new Thickness(ToolbarItems.Count * (FSetting.IsAndroid ? 50 : 35), 0, 0, 0);
                return;
            }
            TitleLabel.Margin = FSetting.IsAndroid ? new Thickness(0, 0, 17, 0) : default;
        }
    }
}