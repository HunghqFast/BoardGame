using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLNoInternet : ContentView
    {
        public FTLNoInternet(Action action) : base()
        {
            var s = new StackLayout();
            var img = new Image();
            var btn = new FButton(FText.ReloadThisPage, FIcons.Reload);
            Base(s, img, btn);
            btn.Clicked += (s, e) => { if (FUtility.HasNetwork) action(); };
            Content = s;
        }

        private void Base(StackLayout s, Image img, FButton btn)
        {
            var lbTitle = NewLabel(FText.NoInternet);
            var lbHD = NewLabel(FText.TryConnectInternet);

            HorizontalOptions = VerticalOptions = LayoutOptions.Fill;

            s.Spacing = 10;
            img.HorizontalOptions = s.VerticalOptions = s.HorizontalOptions = LayoutOptions.Center;
            img.Source = FIcons.AccessPointNetworkOff.ToFontImageSource(Color.LightGray);

            btn.CornerRadius = 5;
            btn.HeightRequest = 50;
            btn.HorizontalOptions = LayoutOptions.CenterAndExpand;
            AddLayoutChildren(s.Children, img, lbTitle, lbHD, new ContentView { Content = btn });
        }

        private Label NewLabel(string text)
        {
            return new Label
            {
                Text = text,
                TextColor = FSetting.TextColorContent,
                FontSize = FSetting.FontSizeLabelContent,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = FSetting.FontText,
                HorizontalTextAlignment = TextAlignment.Center,
            };
        }

        private void AddLayoutChildren<T>(IList<T> childs, params T[] views)
        {
            views.ForEach((x) => childs.Add(x));
        }
    }
}