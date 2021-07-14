using Syncfusion.XForms.BadgeView;
using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLItemMenu : ViewCell
    {
        public FTLItemMenu(FMenuViewType type, object sender, Action<object, IFDataEvent> invoke) : base()
        {
            if (type == FMenuViewType.Grid)
            {
                View = HorizontalView(sender, invoke);
                return;
            }
            View = VerticalView(sender, invoke);
        }

        private View VerticalView(object sender, Action<object, IFDataEvent> action)
        {
            var stack = new StackLayout();
            var badge = new SfBadgeView();
            var icon = new Image();
            var label = new Label();
            var res = new FButtonEffect(sender, action) { Content = stack };

            badge.BadgeSettings.StrokeWidth = 2;
            badge.BadgeSettings.FontFamily = FSetting.FontText;
            badge.BadgeSettings.FontSize = FSetting.FontSizeBadge;
            badge.BadgeSettings.Stroke = FSetting.LightColor;
            badge.BadgeSettings.BadgeType = BadgeType.None;
            badge.BadgeSettings.SetBinding(BadgeSetting.BackgroundColorProperty, FItemMenu.BadgeColorProperty.PropertyName);
            badge.SetBinding(SfBadgeView.BadgeTextProperty, FItemMenu.BadgeTextProperty.PropertyName);

            label.MaxLines = 1;
            label.FontSize = FSetting.FontSizeLabelTitle;
            label.TextColor = FSetting.TextColorContent;
            label.LineBreakMode = LineBreakMode.TailTruncation;
            label.HorizontalOptions = LayoutOptions.StartAndExpand;
            label.VerticalOptions = LayoutOptions.Center;
            label.SetBinding(Label.TextProperty, FItemMenu.BarProperty.PropertyName);

            icon.HeightRequest = icon.WidthRequest = FSetting.SizeButtonIcon;
            icon.VerticalOptions = LayoutOptions.Center;
            icon.SetBinding(Image.SourceProperty, FItemMenu.IconUrlProperty.PropertyName);

            stack.HorizontalOptions = stack.VerticalOptions = LayoutOptions.FillAndExpand;
            stack.Orientation = StackOrientation.Horizontal;

            stack.Spacing = 10;
            stack.Children.Add(icon);
            stack.Children.Add(label);
            stack.Children.Add(badge);
            res.HeightRequest = 50;
            res.Padding = new Thickness(10, 0);
            return res;
        }

        private View HorizontalView(object sender, Action<object, IFDataEvent> action)
        {
            var icon = new Image();
            var label = new Label();
            var r = new RelativeLayout();
            var badge = new SfBadgeView();
            var res = new FButtonEffect(sender, action);

            badge.BadgeSettings.StrokeWidth = 2;
            badge.BadgeSettings.FontFamily = FSetting.FontText;
            badge.BadgeSettings.FontSize = FSetting.FontSizeBadge;
            badge.BadgeSettings.Stroke = FSetting.LightColor;
            badge.BadgeSettings.BadgeType = BadgeType.None;
            badge.BadgeSettings.SetBinding(BadgeSetting.BackgroundColorProperty, FItemMenu.BadgeColorProperty.PropertyName);
            badge.SetBinding(SfBadgeView.BadgeTextProperty, FItemMenu.BadgeTextProperty.PropertyName);

            label.MaxLines = 2;
            label.FontSize = FSetting.FontSizeLabelTitle;
            label.TextColor = FSetting.TextColorContent;
            label.LineBreakMode = LineBreakMode.TailTruncation;
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.SetBinding(Label.TextProperty, FItemMenu.BarProperty.PropertyName);

            icon.HeightRequest = icon.WidthRequest = FSetting.SizeButtonIcon;
            icon.HorizontalOptions = icon.VerticalOptions = LayoutOptions.Start;
            icon.SetBinding(Image.SourceProperty, FItemMenu.IconUrlProperty.PropertyName);

            r.HeightRequest = FSetting.SizeButtonIcon + FSetting.FontSizeLabelTitle * 2 + 20;
            r.Children.Add(icon, Constraint.RelativeToParent((parent) => parent.Width / 2 - (FSetting.SizeButtonIcon) / 2), Constraint.Constant(5), Constraint.Constant(FSetting.SizeButtonIcon));
            r.Children.Add(badge, Constraint.RelativeToParent((parent) => parent.Width / 2), Constraint.Constant(1));
            r.Children.Add(label, Constraint.Constant(0), Constraint.RelativeToParent((parent) => parent.Height / 2 - 5), Constraint.RelativeToParent((parent) => parent.Width));

            res.Content = r;
            res.Padding = new Thickness(10, 8, 10, 5);
            return res;
        }
    }
}