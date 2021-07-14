using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageSearch : FPage, IFSearch
    {
        public static readonly BindableProperty TurnOnSearchProperty = BindableProperty.Create("TurnOnSearch", typeof(bool), typeof(FPageSearch), true);
        public static readonly BindableProperty SearchBoxColorProperty = BindableProperty.Create("SearchBoxColor", typeof(Color), typeof(FPageSearch), Color.Default);
        public static readonly BindableProperty SearchTextColorProperty = BindableProperty.Create("SearchTextColor", typeof(Color), typeof(FPageSearch), Color.White);
        public static readonly BindableProperty SearchPlaceHolderColorProperty = BindableProperty.Create("SearchPlaceHolderColor", typeof(Color), typeof(FPageSearch), Color.White);
        public static readonly BindableProperty SearchBackgroundColorProperty = BindableProperty.Create("SearchBackgroundColor", typeof(Color), typeof(FPageSearch), Color.Default);
        public static readonly BindableProperty SearchPlaceHolderProperty = BindableProperty.Create("SearchPlaceHolder", typeof(string), typeof(FPageSearch), string.Empty);
        public static readonly BindableProperty SearchFontProperty = BindableProperty.Create("SearchFont", typeof(Font), typeof(FPageSearch), Font.Default);
        public static readonly BindableProperty SearchTextProperty = BindableProperty.Create("SearchText", typeof(string), typeof(FPageSearch), string.Empty);
        public static readonly BindableProperty IOSSearchCancelTextProperty = BindableProperty.Create("IOSSearchCancelText", typeof(string), typeof(FPageSearch), string.Empty);
        public static readonly BindableProperty IOSShowCancelButtonProperty = BindableProperty.Create("IOSShowCancelButton", typeof(bool), typeof(FPageSearch), true);

        public bool TurnOnSearch
        {
            get => (bool)GetValue(TurnOnSearchProperty);
            set => SetValue(TurnOnSearchProperty, value);
        }

        public Color SearchBoxColor
        {
            get => (Color)GetValue(SearchBoxColorProperty);
            set => SetValue(SearchBoxColorProperty, value);
        }

        public Color SearchPlaceHolderColor
        {
            get => (Color)GetValue(SearchPlaceHolderColorProperty);
            set => SetValue(SearchPlaceHolderColorProperty, value);
        }

        public Color SearchTextColor
        {
            get => (Color)GetValue(SearchTextColorProperty);
            set => SetValue(SearchTextColorProperty, value);
        }

        public Color SearchBackgroundColor
        {
            get => (Color)GetValue(SearchBackgroundColorProperty);
            set => SetValue(SearchBackgroundColorProperty, value);
        }

        public string SearchPlaceHolder
        {
            get => (string)GetValue(SearchPlaceHolderProperty);
            set => SetValue(SearchPlaceHolderProperty, value);
        }

        public string IOSSearchCancelText
        {
            get => (string)GetValue(IOSSearchCancelTextProperty);
            set => SetValue(IOSSearchCancelTextProperty, value);
        }

        public bool IOSShowCancelButton
        {
            get => (bool)GetValue(IOSShowCancelButtonProperty);
            set => SetValue(IOSShowCancelButtonProperty, value);
        }

        public Font SearchFont
        {
            get => (Font)GetValue(SearchFontProperty);
            set => SetValue(SearchFontProperty, value);
        }

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public event EventHandler<FSearchEventArgs> SearchBarTextChanged;

        public event EventHandler<FSearchEventArgs> SearchBarTextSubmit;

        public FPageSearch(bool IsHasPullToRefresh, bool enableScroll = false) : base(IsHasPullToRefresh, enableScroll)
        {
            IOSSearchCancelText = FText.Cancel;
            SearchFont = Font.OfSize(FSetting.FontText, FSetting.FontSizeLabelContent);
            SearchBoxColor = FSetting.IsAndroid ? Color.Transparent : Color.FromHex("#106e6e6e");
            SearchBackgroundColor = Color.Transparent;
        }

        public virtual void OnSearchChanged(object sender, FSearchEventArgs e)
        {
            SearchBarTextChanged?.Invoke(sender, e);
        }

        public virtual void OnSearchSubmit(object sender, FSearchEventArgs e)
        {
            SearchBarTextSubmit?.Invoke(sender, e);
        }
    }
}