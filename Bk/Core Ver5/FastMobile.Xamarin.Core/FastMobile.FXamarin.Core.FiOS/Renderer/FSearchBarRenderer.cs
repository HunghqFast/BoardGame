using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FSearchBar), typeof(FSearchBarRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FSearchBarRenderer : SearchBarRenderer
    {
        private readonly UIView SearchFieldBackground;
        private UITextField SearchField;
        private UIButton CancelButton;

        private FSearchBar Current => Element as FSearchBar;
        private UIImageView Icon => SearchField.LeftView as UIImageView;

        public FSearchBarRenderer() : base()
        {
            SearchFieldBackground = new UIView();
        }

        public override void UpdateCancelButton()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            Control.EnablesReturnKeyAutomatically = false;
            Control.SearchBarStyle = UISearchBarStyle.Minimal;
            Control.Translucent = true;
            Control.BarStyle = UIBarStyle.Black;

            Control.OnEditingStarted -= OnEditingStarted;
            Control.OnEditingStarted += OnEditingStarted;

            Control.OnEditingStopped -= OnEditingStoped;
            Control.OnEditingStopped += OnEditingStoped;

            Contructor();
            UpdateDefaultTextField();
            //UpdateSearchBox();
            UpdateFieldTextColor();
            UpdateBackground();
            UpdateTextCancel();
            UpdateIcon();
            UpdateShowCancel(false);
            UpdateClearButton();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FSearchBar.StartColorProperty.PropertyName || e.PropertyName == FSearchBar.EndColorProperty.PropertyName)
            {
                UpdateBackground();
                return;
            }

            if (e.PropertyName == FSearchBar.SearchBoxColorProperty.PropertyName)
            {
                //UpdateSearchBox();
                return;
            }

            if (e.PropertyName == FSearchBar.IconProperty.PropertyName || e.PropertyName == FSearchBar.TextColorProperty.PropertyName)
            {
                UpdateIcon();
                return;
            }

            if (e.PropertyName == FSearchBar.IOSShowCancelButtonProperty.PropertyName)
            {
                UpdateShowCancel(Current.IOSShowCancelButton);
                return;
            }

            if (e.PropertyName == FSearchBar.IOSCancelTextProperty.PropertyName)
            {
                UpdateTextCancel();
                return;
            }

            if (e.PropertyName == FSearchBar.TextColorProperty.PropertyName)
            {
                UpdateFieldTextColor();
                return;
            }
        }

        private void Contructor()
        {
            SearchField ??= Control.ValueForKey(new NSString("searchField")) as UITextField;
            CancelButton ??= Control.ValueForKey(new NSString("cancelButton")) as UIButton;
        }

        private void UpdateDefaultTextField()
        {
            Control.AutocapitalizationType = UITextAutocapitalizationType.None;
        }

        private void UpdateFieldTextColor()
        {
            if (SearchFieldIsNull)
                return;
            SearchField.TintColor = SearchField.TextColor = Current.TextColor.ToUIColor();
        }

        private void UpdateTextCancel()
        {
            if (CancelButtonIsNull)
                return;
            CancelButton.SetTitle(Current.IOSCancelText, UIControlState.Normal);
            CancelButton.SetTitleColor(Current.TextColor.ToUIColor(), UIControlState.Normal);
            CancelButton.TintColor = Current.TextColor.ToUIColor();
        }

        private void UpdateShowCancel(bool show)
        {
            if (Control == null || string.IsNullOrWhiteSpace(Current.IOSCancelText))
                return;
            Control.SetShowsCancelButton(show, true);
        }

        private async void UpdateIcon()
        {
            Icon.TintColor = Current.TextColor.ToUIColor();
            Icon.Image = await FUtility.ToImageFromImageSource(FIcons.Magnify.ToFontImageSource(Current.TextColor, FSetting.SizeIconShow));
            Control.SetImageforSearchBarIcon(Icon.ConvertToImage(), UISearchBarIcon.Search, UIControlState.Normal);
        }

        private void UpdateSearchBox()
        {
            if (SearchFieldIsNull)
                return;
            if (Current.SearchBoxColor != Color.Default)
                SearchField.BackgroundColor = Current.SearchBoxColor.ToUIColor();

            SearchFieldBackground.Layer.CornerRadius = 10;
            SearchFieldBackground.Frame = new CGRect(SearchField.Frame.X, SearchField.Frame.Y, SearchField.Frame.Width, 36);
            SearchFieldBackground.BackgroundColor = Current.SearchBoxColor.ToUIColor();
            SearchFieldBackground.ClipsToBounds = true;
            Control.SetSearchFieldBackgroundImage(SearchFieldBackground.ConvertToImage(), UIControlState.Normal);
        }

        private void UpdateBackground()
        {
            Control.UpdateBackground(new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { new GradientStop { Offset = 0.0f, Color = Current.StartColor }, new GradientStop { Offset = 1.0f, Color = Current.EndColor } } });
        }

        private void UpdateClearButton()
        {
            if (SearchFieldIsNull)
                return;
            SearchField.ClearButtonMode = UITextFieldViewMode.Never;
        }

        private void OnEditingStoped(object sender, EventArgs e)
        {
            UpdateShowCancel(false);
        }

        private void OnEditingStarted(object sender, EventArgs e)
        {
            UpdateShowCancel(Current.IOSShowCancelButton);
            UpdateTextCancel();
            UpdateClearButton();
        }

        private bool SearchFieldIsNull
        {
            get
            {
                SearchField ??= Control.ValueForKey(new NSString("searchField")) as UITextField;
                return SearchField == null;
            }
        }

        private bool CancelButtonIsNull
        {
            get
            {
                CancelButton ??= Control.ValueForKey(new NSString("cancelButton")) as UIButton;
                return CancelButton == null;
            }
        }
    }
}