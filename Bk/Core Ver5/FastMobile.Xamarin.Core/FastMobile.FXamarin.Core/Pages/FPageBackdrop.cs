using Syncfusion.XForms.Backdrop;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace FastMobile.FXamarin.Core
{
    public class FPageBackdrop : SfBackdropPage
    {
        public static readonly BindableProperty ShowLineHeaderProperty = BindableProperty.Create("ShowLineHeader", typeof(bool), typeof(FPageBackdrop), true);
        public static readonly BindableProperty FrontContentProperty = BindableProperty.Create("FrontContent", typeof(View), typeof(FPageBackdrop));
        public static readonly BindableProperty BackContentProperty = BindableProperty.Create("BackContent", typeof(View), typeof(FPageBackdrop));
        public static readonly BindableProperty HeaderContentProperty = BindableProperty.Create("HeaderContent", typeof(View), typeof(FPageBackdrop));

        public bool ShowLineHeader
        {
            get => (bool)GetValue(ShowLineHeaderProperty);
            set => SetValue(ShowLineHeaderProperty, value);
        }

        public View FrontContent
        {
            get => (View)GetValue(FrontContentProperty);
            set => SetValue(FrontContentProperty, value);
        }

        public View BackContent
        {
            get => (View)GetValue(BackContentProperty);
            set => SetValue(BackContentProperty, value);
        }

        public View HeaderContent
        {
            get => (View)GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        private readonly BackdropBackLayer backLayer;
        private readonly BackdropFrontLayer frontLayer;
        private readonly ToolbarItem ToolbarItem;
        private readonly ContentView backParent, frontParent, frontHeader;
        private readonly StackLayout frontLayout;
        private readonly FLine line;

        public FPageBackdrop()
        {
            line = new FLine { BindingContext = this };
            frontHeader = new ContentView { BindingContext = this };
            backParent = new ContentView() { BindingContext = this };
            frontParent = new ContentView() { BindingContext = this };
            backLayer = new BackdropBackLayer() { BindingContext = this };
            frontLayer = new BackdropFrontLayer() { BindingContext = this };
            frontLayout = new StackLayout { BindingContext = this };
            ToolbarItem = new ToolbarItem();
            Base();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        private void Base()
        {
            BackLayerRevealOption = RevealOption.Auto;
            IsBackLayerRevealed = true;

            backParent.HeightRequest = FSetting.ScreenHeight;
            backParent.WidthRequest = FSetting.ScreenWidth;
            backParent.SetBinding(ContentView.ContentProperty, BackContentProperty.PropertyName);

            backLayer.Content = backParent;

            frontLayout.Spacing = 0;
            frontLayout.Children.Add(frontHeader);
            frontLayout.Children.Add(line);
            frontLayout.Children.Add(frontParent);

            frontLayer.EnableSwiping = true;
            frontLayer.RevealedHeight = 150;
            frontLayer.LeftCornerRadius = frontLayer.RightCornerRadius = 0;
            frontLayer.Content = frontLayout;

            line.SetBinding(FLine.IsVisibleProperty, ShowLineHeaderProperty.PropertyName);
            frontParent.SetBinding(ContentView.ContentProperty, FrontContentProperty.PropertyName);
            frontHeader.SetBinding(ContentView.ContentProperty, HeaderContentProperty.PropertyName);

            BackLayer = backLayer;
            FrontLayer = frontLayer;
            UpdateToolbar();
        }

        private void UpdateToolbar()
        {
            ToolbarItems.Clear();
            ToolbarItem.BindingContext = this;
            ToolbarItem.Clicked += OnStateChanged;
            ToolbarItem.SetBinding(ToolbarItem.IconImageSourceProperty, IsBackLayerRevealedProperty.PropertyName, converter: new FBoolToObject(FIcons.ChevronUp.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconToolbar), FIcons.ChevronDown.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconToolbar)));
            ToolbarItems.Add(ToolbarItem);
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            IsBackLayerRevealed = !IsBackLayerRevealed;
        }
    }
}