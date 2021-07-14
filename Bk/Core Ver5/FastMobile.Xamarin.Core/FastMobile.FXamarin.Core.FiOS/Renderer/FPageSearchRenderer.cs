using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UISearchBarStyle = UIKit.UISearchBarStyle;

[assembly: ExportRenderer(typeof(FPageSearch), typeof(FPageSearchRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FPageSearchRenderer : FPageRenderer, IShellContentInsetObserver
    {
        private readonly UIView SearchFieldBackground;
        private readonly UISearchController SearchController;
        private UITextField SearchField;
        private UILabel PlaceHolder;
        private UIButton CancelButton;
        private bool FirstTurnOn = true;

        private FPageSearch Current => Element as FPageSearch;
        private UISearchBar SearchBar => SearchController.SearchBar;
        private UIImageView Icon => SearchField.LeftView as UIImageView;
        private bool Inited = false;

        public FPageSearchRenderer() : base()
        {
            SearchController = new UISearchController(searchResultsController: null)
            {
                DimsBackgroundDuringPresentation = false,
                HidesNavigationBarDuringPresentation = false,
                ObscuresBackgroundDuringPresentation = false,
                DefinesPresentationContext = true,
                HidesBottomBarWhenPushed = true
            };
            SearchBar.CancelButtonClicked += SearchBarCancelButtonClicked;
            SearchBar.SearchButtonClicked += SearchBarSearchButtonClicked;
            SearchBar.OnEditingStarted += OnEditingStarted;
            SearchBar.OnEditingStopped += OnEditingStoped;
            SearchBar.TextChanged += SearchBarTextChanged;
            SearchFieldBackground = new UIView();
            Contructor();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AddSearchToToolbar();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Contructor();
            UpdateSafeArea();
        }

        public override void ViewSafeAreaInsetsDidChange()
        {
            base.ViewSafeAreaInsetsDidChange();
            UpdateSafeArea();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            UpdateSafeArea();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UpdateSearchBoxColor();
        }

        protected override void OnNavigationPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnNavigationPagePropertyChanged(sender, e);

            if (e.PropertyName == FNavigationPage.StartColorProperty.PropertyName || e.PropertyName == FNavigationPage.EndColorProperty.PropertyName)
            {
                UpdateSearchBackgroundColor();
                return;
            }
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(sender, e);
            if (Current == null)
                return;
            if (e.PropertyName == FPageSearch.SearchTextColorProperty.PropertyName)
            {
                UpdateSearchTextColor();
                return;
            }

            if (e.PropertyName == FPageSearch.SearchBackgroundColorProperty.PropertyName)
            {
                UpdateSearchBackgroundColor();
                return;
            }

            if (e.PropertyName == FPageSearch.IOSSearchCancelTextProperty.PropertyName)
            {
                UpdateCancelButtonText();
                return;
            }

            if (e.PropertyName == FPageSearch.SearchPlaceHolderProperty.PropertyName)
            {
                UpdatePlaceHolderText();
                return;
            }

            if (e.PropertyName == FPageSearch.IOSShowCancelButtonProperty.PropertyName)
            {
                UpdateShowCancelButton(Current.IOSShowCancelButton);
                return;
            }

            if (e.PropertyName == FPageSearch.SearchPlaceHolderColorProperty.PropertyName)
            {
                UpdatePlaceholderColor();
                return;
            }

            if (e.PropertyName == FPageSearch.SearchBoxColorProperty.PropertyName)
            {
                UpdateSearchBoxColor();
                return;
            }

            if (e.PropertyName == FPageSearch.SearchTextProperty.PropertyName)
            {
                SearchBar.Text = Current.SearchText;
                return;
            }

            if (e.PropertyName == FPageSearch.TurnOnSearchProperty.PropertyName)
            {
                UpdateTurnOnSearch();
                UpdateSafeArea();
                return;
            }
        }

        #region Private

        private void AddSearchToToolbar()
        {
            if (Inited)
                return;
            Inited = true;

            if (Current == null)
                return;

            if (!(ParentViewController is UIViewController))
                return;

            UpdateTurnOnSearch();
            ParentViewController.NavigationItem.HidesSearchBarWhenScrolling = false;
            DefinesPresentationContext = true;

            SearchBar.EnablesReturnKeyAutomatically = false;
            SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            SearchBar.Translucent = true;
            SearchBar.BarStyle = UIBarStyle.Black;
            SearchBar.Text = Current.SearchText;

            UpdateDefaultTextField();
            UpdateSearchBoxColor();
            UpdateSearchTextColor();
            UpdatePlaceholderColor();
            UpdateSearchBackgroundColor();
            UpdatePlaceHolderText();
            UpdateFont();
            UpdateCancelButtonText();
            UpdateShowCancelButton(false);
        }

        private void UpdateTurnOnSearch()
        {
            if (!Current.TurnOnSearch)
            {
                ParentViewController.NavigationItem.SearchController = null;
                return;
            }
            ParentViewController.NavigationItem.SearchController = SearchController;
            ParentViewController.NavigationItem.SearchController.Active = true;
            ParentViewController.NavigationItem.SearchController.Active = false;
        }

        private void UpdateSafeArea()
        {
            var safeBottom = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
            if (Current.TurnOnSearch && FirstTurnOn && safeBottom > 0 && Current.Padding.Bottom != safeBottom)
            {
                AdditionalSafeAreaInsets = new UIEdgeInsets(0, 0, SearchBar.Frame.Height - 3 + safeBottom, 0);
                FirstTurnOn = false;
                return;
            }
            AdditionalSafeAreaInsets = new UIEdgeInsets(0, 0, Current.TurnOnSearch && Current.TurnOnSearch ? (SearchBar.Frame.Height - 3 + safeBottom) : 0, 0);
        }

        private void Contructor()
        {
            SearchField ??= SearchBar.ValueForKey(new NSString("searchField")) as UITextField;
            CancelButton ??= SearchBar.ValueForKey(new NSString("cancelButton")) as UIButton;
            PlaceHolder ??= SearchField?.ValueForKey(new NSString("placeholderLabel")) as UILabel;
        }

        private void SearchBarTextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            Current.SearchText = e.SearchText;
            Current?.OnSearchChanged(Current, new FSearchEventArgs(e.SearchText));
        }

        private void UpdateDefaultTextField()
        {
            if (SearchFieldIsNull)
                return;
            SearchField.AutocapitalizationType = UITextAutocapitalizationType.None;
        }

        private void UpdateSearchBoxColor()
        {
            if (SearchFieldIsNull)
                return;
            if (Current.SearchBoxColor != Color.Default)
                SearchField.BackgroundColor = Current.SearchBoxColor.ToUIColor();

            SearchFieldBackground.Layer.CornerRadius = 10;
            SearchFieldBackground.Frame = new CGRect(SearchField.Frame.X, SearchField.Frame.Y, SearchField.Frame.Width, 36);
            SearchFieldBackground.BackgroundColor = Current.SearchBoxColor.ToUIColor();
            SearchFieldBackground.ClipsToBounds = true;
            SearchBar.SetSearchFieldBackgroundImage(SearchFieldBackground.ConvertToImage(), UIControlState.Normal);
        }

        private async void UpdateSearchTextColor()
        {
            UpdateSearchBarTintColor();
            UpdateClearButton();

            Icon.TintColor = SearchField.TextColor = SearchField.TintColor = Current.SearchTextColor.ToUIColor();
            Icon.Image = await FUtility.ToImageFromImageSource(FIcons.Magnify.ToFontImageSource(Current.SearchTextColor, FSetting.SizeIconShow));
            SearchBar.SetImageforSearchBarIcon(Icon.ConvertToImage(), UISearchBarIcon.Search, UIControlState.Normal);
        }

        private void UpdatePlaceholderColor()
        {
            if (PlaceHolderIsNull)
                return;
            PlaceHolder.TextColor = Current.SearchPlaceHolderColor.ToUIColor();
            PlaceHolder.TintColor = Current.SearchPlaceHolderColor.ToUIColor();
        }

        private void UpdateSearchBackgroundColor()
        {
            if (SearchBar == null)
                return;
            if (Current.SearchBackgroundColor != Color.Transparent && Current.SearchBackgroundColor != Color.Default)
            {
                SearchBar.BackgroundColor = Current.SearchBackgroundColor.ToUIColor();
                return;
            }

            if (Current.Parent is FNavigationPage nav)
                SearchBar.UpdateBackground(new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { new GradientStop { Offset = 0.0f, Color = nav.StartColor }, new GradientStop { Offset = 1.0f, Color = nav.EndColor } } });
        }

        private void UpdatePlaceHolderText()
        {
            if (SearchBar == null)
                return;
            SearchBar.Placeholder = Current.SearchPlaceHolder;
        }

        private void UpdateFont()
        {
            if (Current.SearchFont.IsDefault)
                return;

            if (SearchFieldIsNull)
                return;
            SearchField.Font = Current.SearchFont.ToUIFont();

            if (!PlaceHolderIsNull)
                PlaceHolder.Font = Current.SearchFont.ToUIFont();

            if (!CancelButtonIsNull)
                CancelButton.Font = Current.SearchFont.ToUIFont();
        }

        private void UpdateCancelButtonText()
        {
            if (string.IsNullOrEmpty(Current.IOSSearchCancelText))
                return;
            if (CancelButtonIsNull)
                return;
            CancelButton.SetTitle(Current.IOSSearchCancelText, UIControlState.Normal);
        }

        private void UpdateShowCancelButton(bool show)
        {
            if (SearchBar == null)
                return;
            SearchBar.SetShowsCancelButton(show, true);
            UpdateSearchBarTintColor();
        }

        private void SearchBarCancelButtonClicked(object sender, EventArgs e)
        {
            if (Current != null && !String.IsNullOrEmpty(SearchBar.Text))
            {
                Current.SearchText = "";
                Current?.OnSearchChanged(Current, new FSearchEventArgs(""));
            }
            UpdateShowCancelButton(false);
        }

        private void UpdateSearchBarTintColor()
        {
            if (SearchBar == null)
                return;
            SearchBar.TintColor = SearchBar.BarTintColor = Current.SearchTextColor.ToUIColor();
        }

        private void UpdateClearButton()
        {
            if (SearchFieldIsNull)
                return;
            SearchField.ClearButtonMode = UITextFieldViewMode.Never;
        }

        private void SearchBarSearchButtonClicked(object sender, EventArgs e)
        {
            Current?.OnSearchSubmit(Current, new FSearchEventArgs(SearchBar.Text));
        }

        private void OnEditingStoped(object sender, EventArgs e)
        {
            UpdateShowCancelButton(false);
        }

        private void OnEditingStarted(object sender, EventArgs e)
        {
            UpdateShowCancelButton(Current.IOSShowCancelButton);
            UpdateCancelButtonText();
            UpdateClearButton();
        }

        private bool SearchFieldIsNull
        {
            get
            {
                SearchField ??= SearchBar.ValueForKey(new NSString("searchField")) as UITextField;
                return SearchField == null;
            }
        }

        private bool CancelButtonIsNull
        {
            get
            {
                CancelButton ??= SearchBar.ValueForKey(new NSString("cancelButton")) as UIButton;
                return CancelButton == null;
            }
        }

        private bool PlaceHolderIsNull
        {
            get
            {
                if (SearchFieldIsNull)
                    return true;
                PlaceHolder ??= SearchField.ValueForKey(new NSString("placeholderLabel")) as UILabel;
                return PlaceHolder == null;
            }
        }

        #endregion Private
    }
}