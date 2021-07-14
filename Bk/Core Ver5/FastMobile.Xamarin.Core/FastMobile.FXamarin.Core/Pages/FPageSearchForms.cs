using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageSearchForms : FPage, IFSearch
    {
        public static readonly BindableProperty TurnOnSearchProperty = BindableProperty.Create("TurnOnSearch", typeof(bool), typeof(FPageSearchForms), true);
        public static readonly BindableProperty SearchTextProperty = BindableProperty.Create("SearchText", typeof(string), typeof(FPageSearchForms), string.Empty, BindingMode.TwoWay);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder", typeof(string), typeof(FPageSearchForms), string.Empty);
        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create("PlaceholderColor", typeof(Color), typeof(FPageSearchForms), Color.White);

        public bool TurnOnSearch
        {
            get => (bool)GetValue(TurnOnSearchProperty);
            set => SetValue(TurnOnSearchProperty, value);
        }

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public View SearchContent
        {
            get => V.Content;
            set => V.Content = value;
        }

        public event EventHandler<FSearchEventArgs> SearchBarTextChanged;

        public event EventHandler<FSearchEventArgs> SearchBarTextSubmit;

        private readonly FSearchView Search;
        private readonly StackLayout S;
        private readonly ContentView V;

        public FPageSearchForms(bool IsHasPullToRefresh, bool enableScroll = false) : base(IsHasPullToRefresh, enableScroll)
        {
            Search = new FSearchView();
            V = new ContentView();
            S = new StackLayout();
            Content = S;
            Base();
        }

        private void Base()
        {
            Search.BindingContext = this;
            Search.Field.SetBinding(Entry.TextProperty, SearchTextProperty.PropertyName, BindingMode.TwoWay);
            Search.Field.SetBinding(Entry.PlaceholderProperty, PlaceholderProperty.PropertyName, BindingMode.TwoWay);
            Search.Field.SetBinding(Entry.PlaceholderColorProperty, PlaceholderColorProperty.PropertyName, BindingMode.TwoWay);
            Search.SetBinding(View.IsVisibleProperty, TurnOnSearchProperty.PropertyName);
            Search.SearchBarTextChanged += OnSearchBarTextChanged;
            Search.SearchBarTextSubmit += OnSearchBarTextSubmit;

            if (FApplication.Current.MainPage is FNavigationPage nav)
            {
                UpdateBackground(nav);
                nav.PropertyChanged += OnNavigationParentPropertyChanged;
            }

            S.VerticalOptions = LayoutOptions.Fill;
            S.Spacing = 0;
            S.Children.Add(Search);
            S.Children.Add(V);
        }

        protected virtual void OnSearchBarTextChanged(object sender, FSearchEventArgs e)
        {
            SearchBarTextChanged?.Invoke(this, e);
        }

        protected virtual void OnSearchBarTextSubmit(object sender, FSearchEventArgs e)
        {
            SearchBarTextSubmit?.Invoke(this, e);
        }

        private void OnNavigationParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FNavigationPage.StartColorProperty.PropertyName || e.PropertyName == FNavigationPage.EndColorProperty.PropertyName)
            {
                UpdateBackground(sender as FNavigationPage);
            }
        }

        private void UpdateBackground(FNavigationPage nav)
        {
            if (nav == null)
                return;
            Search.Background = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { new GradientStop { Offset = 0.0f, Color = FSetting.StartColor }, new GradientStop { Offset = 1.0f, Color = FSetting.EndColor } } };
        }
    }
}