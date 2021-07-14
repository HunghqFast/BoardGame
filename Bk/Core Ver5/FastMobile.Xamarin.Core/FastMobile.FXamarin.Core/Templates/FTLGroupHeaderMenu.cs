using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLGroupHeaderMenu : StackLayout
    {
        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create("IsExpanded", typeof(bool), typeof(FTLGroupHeaderMenu), true);

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private readonly SfListView ViewParent;
        private readonly FPageMenu Root;
        private readonly Label Title;
        private readonly Image Icon;

        public FTLGroupHeaderMenu(FPageMenu root, SfListView parent) : base()
        {
            Title = new Label();
            Icon = new Image();
            ViewParent = parent;
            Root = root;

            Spacing = 0;
            Orientation = StackOrientation.Horizontal;
            BackgroundColor = FSetting.BackgroundSpacing;
            WidthRequest = FSetting.ScreenWidth;

            Title.FontFamily = FSetting.FontText;
            Title.TextColor = FSetting.TextColorTitle;
            Title.FontSize = FSetting.FontSizeLabelTitle;
            Title.PropertyChanged += OnTitlePropertyChanged;
            Title.SetBinding(Label.TextProperty, "Key");

            Icon.IsVisible = Root.TurnOnExpand;
            Icon.BackgroundColor = Color.Transparent;
            Icon.Source = FIcons.ChevronUp.ToFontImageSource(FSetting.TextColorTitle, FSetting.SizeIconLegend - 3);
            Icon.HorizontalOptions = LayoutOptions.EndAndExpand;

            Children.Add(Title);
            Children.Add(Icon);
            if (Root == null)
                return;
            Root.PropertyChanged += OnRootPropertyChanged;
            GestureRecognizers.Add(new TapGestureRecognizer());
            (GestureRecognizers[0] as TapGestureRecognizer).Tapped += OnClicked;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (!Root.TurnOnExpand)
                return;
            if (BindingContext is not GroupResult group)
                return;

            IsExpanded = GetExpand($"FastMobile.FXamarin.Core.FTLGroupHeaderMenu.{Root.Group}.{group.Key}");
            UpdateView(group);
        }

        private void OnTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                HeightRequest = string.IsNullOrEmpty(Title.Text) ? 0 : -1;
                Padding = string.IsNullOrEmpty(Title.Text) ? 0 : new Thickness(10, 8);
            }
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            if (!Root.TurnOnExpand)
                return;
            if (BindingContext is not GroupResult group)
                return;

            await Root.SetBusy(true);
            await Task.Delay(150);
            IsExpanded = !IsExpanded;
            SetExpand($"FastMobile.FXamarin.Core.FTLGroupHeaderMenu.{Root.Group}.{group.Key}", IsExpanded);
            UpdateView(group);
            await Root.SetBusy(false);
        }

        private async void UpdateView(GroupResult group)
        {
            await Icon.RotateTo(IsExpanded ? 0 : -180, 150, Easing.Linear);
            if (IsExpanded) ViewParent.ExpandGroup(group);
            else ViewParent.CollapseGroup(group);
        }

        private void OnRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FPageMenu.TurnOnExpandProperty.PropertyName)
            {
                Icon.IsVisible = Root.TurnOnExpand;
                return;
            }
        }

        private static bool GetExpand(string path)
        {
            return Convert.ToBoolean(path.GetCache(bool.TrueString));
        }

        private static void SetExpand(string path, bool value)
        {
            (value ? bool.TrueString : bool.FalseString).SetCache(path);
        }
    }
}