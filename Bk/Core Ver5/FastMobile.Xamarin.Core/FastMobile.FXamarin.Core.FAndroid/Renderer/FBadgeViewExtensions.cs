using Android.Views;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

namespace FastMobile.FXamarin.Core.FAndroid
{
    internal static class BadgeViewExtensions
    {
        public static void UpdateFromElement(this FBadgeView badgeView, Page element)
        {
            var badgeText = FTabBadge.GetBadgeText(element);
            badgeView.Text = badgeText;

            var tabColor = FTabBadge.GetBadgeColor(element);
            if (tabColor != Color.Default)
            {
                badgeView.BadgeColor = tabColor.ToAndroid();
            }

            var tabTextColor = FTabBadge.GetBadgeTextColor(element);
            if (tabTextColor != Color.Default)
            {
                badgeView.TextColor = tabTextColor.ToAndroid();
            }

            var font = FTabBadge.GetBadgeFont(element);
            if (font != Font.Default)
            {
                badgeView.Typeface = font.ToTypeface();
            }

            var margin = FTabBadge.GetBadgeMargin(element);
            badgeView.SetMargins((float)margin.Left, (float)margin.Top, (float)margin.Right, (float)margin.Bottom);
            badgeView.Postion = FTabBadge.GetBadgePosition(element);
        }

        public static void UpdateFromPropertyChangedEvent(this FBadgeView badgeView, Element element, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FTabBadge.BadgeTextProperty.PropertyName)
            {
                badgeView.Text = FTabBadge.GetBadgeText(element);
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeColorProperty.PropertyName)
            {
                badgeView.BadgeColor = FTabBadge.GetBadgeColor(element).ToAndroid();
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeTextColorProperty.PropertyName)
            {
                badgeView.TextColor = FTabBadge.GetBadgeTextColor(element).ToAndroid();
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeFontProperty.PropertyName)
            {
                badgeView.Typeface = FTabBadge.GetBadgeFont(element).ToTypeface();
                return;
            }
            if (e.PropertyName == FTabBadge.BadgePositionProperty.PropertyName)
            {
                badgeView.Postion = FTabBadge.GetBadgePosition(element);
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeMarginProperty.PropertyName)
            {
                var margin = FTabBadge.GetBadgeMargin(element);
                badgeView.SetMargins((float)margin.Left, (float)margin.Top, (float)margin.Right, (float)margin.Bottom);
                return;
            }
        }

        public static T FindChildOfType<T>(this ViewGroup parent) where T : View
        {
            if (parent == null)
                return null;
            if (parent.ChildCount == 0)
                return null;
            for (var i = 0; i < parent.ChildCount; i++)
            {
                var child = parent.GetChildAt(i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                if (child is not ViewGroup)
                {
                    continue;
                }
                var result = FindChildOfType<T>(child as ViewGroup);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}