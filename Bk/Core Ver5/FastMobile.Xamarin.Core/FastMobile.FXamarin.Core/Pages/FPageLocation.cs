using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public class FPageLocation : FPageBackdrop, IFLocationPage
    {
        public static readonly BindableProperty PlacesProperty = BindableProperty.Create("Places", typeof(ObservableCollection<FPlace>), typeof(FPageLocation));

        public ObservableCollection<FPlace> Places
        {
            get => (ObservableCollection<FPlace>)GetValue(PlacesProperty);
            set => SetValue(PlacesProperty, value);
        }

        public ToolbarItem Refresh { get; }
        public IFLocationControl Control { get; set; }
        public IFLocation Location { get; set; }

        public event EventHandler<FObjectPropertyArgs<FPlace>> ItemClicked;

        private readonly SfListView ListPlaces;
        private readonly Label HeaderTitle;

        public FPageLocation()
        {
            Control = new FMapContainer();
            ListPlaces = new SfListView();
            HeaderTitle = new Label { BindingContext = this };
            Refresh = new ToolbarItem();
            Places = new ObservableCollection<FPlace>();
            Base();
        }

        public FPageLocation(MapSpan span)
        {
            Control = new FMapContainer(span);
            ListPlaces = new SfListView { BindingContext = this };
            HeaderTitle = new Label { BindingContext = this };
            Refresh = new ToolbarItem { BindingContext = this };
            Places = new ObservableCollection<FPlace>();
            Base();
        }

        public string GetPosition()
        {
            return string.Empty;
        }

        public string GetPositionNear()
        {
            return string.Empty;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Appear();
        }

        private void Base()
        {
            ShowLineHeader = false;
            BackContent = Control as View;

            HeaderTitle.Text = FText.LocationSelect;
            HeaderTitle.FontFamily = FSetting.FontText;
            HeaderTitle.TextColor = FSetting.TextColorTitle;
            HeaderTitle.FontSize = FSetting.FontSizeLabelTitle;
            HeaderTitle.Padding = new Thickness(10);

            ListPlaces.BindingContext = this;
            ListPlaces.ItemSpacing = 10;
            ListPlaces.AutoFitMode = AutoFitMode.DynamicHeight;
            ListPlaces.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            ListPlaces.ItemTemplate = new DataTemplate(typeof(FTLLocationPlace));
            ListPlaces.ItemTapped += OnItemClicked;
            ListPlaces.SetBinding(SfListView.ItemsSourceProperty, PlacesProperty.PropertyName);

            Refresh.IconImageSource = FIcons.Refresh.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconToolbar);
            ToolbarItems.Insert(0, Refresh);

            HeaderContent = HeaderTitle;
            FrontContent = ListPlaces;
        }

        public void OpenList()
        {
            IsBackLayerRevealed = false;
        }

        public void CloseList()
        {
            IsBackLayerRevealed = true;
        }

        private void OnItemClicked(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            ItemClicked?.Invoke(this, new FObjectPropertyArgs<FPlace>(e.ItemData as FPlace));
        }

        private void Appear()
        {
            if (Location == null || Location.IsGpsEnable())
                return;
            MessagingCenter.Send(new FMessageConfirm(OpenSettingConfirmed, 1, 1300, ""), FChannel.ALERT_BY_MESSAGE);
        }

        private void OpenSettingConfirmed(bool value)
        {
            if (value) Location.OpenSetting();
        }
    }
}