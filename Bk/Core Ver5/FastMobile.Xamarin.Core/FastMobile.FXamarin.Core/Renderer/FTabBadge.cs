using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FTabBadge
    {
        public static BindableProperty BadgeTextProperty = BindableProperty.CreateAttached("BadgeText", typeof(string), typeof(FTabBadge), default(string));
        public static BindableProperty BadgeColorProperty = BindableProperty.CreateAttached("BadgeColor", typeof(Color), typeof(FTabBadge), Color.Default);
        public static BindableProperty BadgeTextColorProperty = BindableProperty.CreateAttached("BadgeTextColor", typeof(Color), typeof(FTabBadge), Color.Default);
        public static BindableProperty BadgeFontProperty = BindableProperty.CreateAttached("BadgeFont", typeof(Font), typeof(FTabBadge), Font.Default);
        public static BindableProperty BadgePositionProperty = BindableProperty.CreateAttached("FBadgePosition", typeof(FBadgePosition), typeof(FTabBadge), FBadgePosition.TopRight);
        public static BindableProperty BadgeMarginProperty = BindableProperty.CreateAttached("BadgeMargin", typeof(Thickness), typeof(FTabBadge), DefaultMargins);

        public static string GetBadgeText(BindableObject view) => (string)view.GetValue(BadgeTextProperty);

        public static void SetBadgeText(BindableObject view, string value) => view.SetValue(BadgeTextProperty, value);

        public static Color GetBadgeColor(BindableObject view) => (Color)view.GetValue(BadgeColorProperty);

        public static void SetBadgeColor(BindableObject view, Color value) => view.SetValue(BadgeColorProperty, value);

        public static Color GetBadgeTextColor(BindableObject view) => (Color)view.GetValue(BadgeTextColorProperty);

        public static void SetBadgeTextColor(BindableObject view, Color value) => view.SetValue(BadgeTextColorProperty, value);

        public static Font GetBadgeFont(BindableObject view) => (Font)view.GetValue(BadgeFontProperty);

        public static void SetBadgeFont(BindableObject view, Font value) => view.SetValue(BadgeFontProperty, value);

        public static FBadgePosition GetBadgePosition(BindableObject view) => (FBadgePosition)view.GetValue(BadgePositionProperty);

        public static void SetBadgePosition(BindableObject view, FBadgePosition value) => view.SetValue(BadgePositionProperty, value);

        public static Thickness GetBadgeMargin(BindableObject view) => (Thickness)view.GetValue(BadgeMarginProperty);

        public static void SetBadgeMargin(BindableObject view, Thickness value) => view.SetValue(BadgeMarginProperty, value);

        public static Thickness DefaultMargins
        {
            get => Device.RuntimePlatform switch
            {
                Device.Android => new Thickness(-10, -5),
                Device.iOS => new Thickness(0),
                _ => new Thickness(0),
            };
        }

        public static Page GetChildPageWithBadge(this TabbedPage parentTabbedPage, int tabIndex)
        {
            var element = parentTabbedPage.Children[tabIndex];
            return GetPageWithBadge(element);
        }

        public static Page GetPageWithBadge(this Page element)
        {
            if (GetBadgeText(element) != (string)BadgeTextProperty.DefaultValue)
            {
                return element;
            }

            if (element is NavigationPage navigationPage)
            {
                return navigationPage.RootPage;
            }

            return element;
        }
    }
}