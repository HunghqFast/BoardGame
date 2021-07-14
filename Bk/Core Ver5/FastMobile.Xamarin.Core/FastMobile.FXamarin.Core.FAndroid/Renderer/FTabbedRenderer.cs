using Android.Content;
using Android.Views;
using Android.Widget;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Internal;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

[assembly: ExportRenderer(typeof(FTabbedPage), typeof(FTabbedRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FTabbedRenderer : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemReselectedListener
    {
        private LinearLayout topTabStrip;
        private TabLayout topTabLayout;
        private ViewGroup bottomTabStrip;
        private BottomNavigationView Control;
        FTabbedPage Current => Element as FTabbedPage;

        private const int DeleayBeforeTabAdded = 10;
        protected readonly Dictionary<Element, FBadgeView> BadgeViews = new Dictionary<Element, FBadgeView>();
        private bool isShiftModeSet;

        public FTabbedRenderer(Context context) : base(context)
        {
        }

        public void OnNavigationItemReselected(IMenuItem item)
        {
            if (Current != null)
                Current.OnTabReselected(Current, new FTabbedPageReselectedEventArgs(Current.CurrentPage as FPage));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TabbedPage> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
                Control = (GetChildAt(0) as Android.Widget.RelativeLayout)?.GetChildAt(1) as BottomNavigationView;

            Cleanup(e.OldElement);
            Cleanup(Element);

            var tabCount = InitLayout();
            for (var i = 0; i < tabCount; i++)
                AddTabBadge(i);

            Element.ChildAdded += OnTabAdded;
            Element.ChildRemoved += OnTabRemoved;

            if (Element != null)
            {
                switch (this.Element.OnThisPlatform().GetToolbarPlacement())
                {
                    case ToolbarPlacement.Default:
                        OnTopChange();
                        break;

                    case ToolbarPlacement.Top:
                        OnTopChange();
                        break;

                    case ToolbarPlacement.Bottom:
                        OnBottomChange();
                        break;

                    default:
                        break;
                }
            }
        }

        protected virtual void OnTabbedPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Element element)
                return;

            if (BadgeViews.TryGetValue(element, out var badgeView))
                badgeView.UpdateFromPropertyChangedEvent(element, e);
        }

        protected override void Dispose(bool disposing)
        {
            Cleanup(Element);
            base.Dispose(disposing);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (!isShiftModeSet && Control is BottomNavigationView)
            {
                Control.SetOnNavigationItemReselectedListener(this);
                isShiftModeSet = true;
            }
        }

        private void OnBottomChange()
        {
            if (Control?.GetChildAt(0) is not BottomNavigationMenuView bottomNavMenuView)
                return;
            for (int i = 0; i < bottomNavMenuView.ChildCount; i++)
            {
                if (bottomNavMenuView.GetChildAt(i) is not BottomNavigationItemView item)
                    continue;
                var itemTitle = item.GetChildAt(1);
                var smallTextView = (TextView)((BaselineLayout)itemTitle).GetChildAt(0);
                var largeTextView = (TextView)((BaselineLayout)itemTitle).GetChildAt(1);
                smallTextView.TextSize = Convert.ToSingle(FSetting.FontSizeLabelHint - 3);
                largeTextView.TextSize = Convert.ToSingle(FSetting.FontSizeLabelHint - 2);
                largeTextView.Ellipsize = smallTextView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            }
        }

        private void OnTopChange()
        {
            var topNavMenuView = topTabLayout?.GetChildAt(0);
            if (topNavMenuView is not ViewGroup group)
                return;
            for (int i = 0; i < group.ChildCount; i++)
            {
                if (group.GetChildAt(i) is not ViewGroup vgTab)
                    continue;

                for (int j = 0; j < vgTab.ChildCount; j++)
                {
                    if (vgTab.GetChildAt(j) is not TextView textView)
                        continue;
                    textView.TextSize = Convert.ToSingle(FSetting.FontSizeLabelHint - 3);
                    textView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
                }
            }
        }

        private int InitLayout()
        {
            switch (this.Element.OnThisPlatform().GetToolbarPlacement())
            {
                case ToolbarPlacement.Default:
                    topTabLayout = ViewGroup.FindChildOfType<TabLayout>();
                    if (topTabLayout == null)
                        return 0;
                    topTabStrip = topTabLayout.FindChildOfType<LinearLayout>();
                    return topTabLayout.TabCount;

                case ToolbarPlacement.Top:
                    topTabLayout = ViewGroup.FindChildOfType<TabLayout>();
                    if (topTabLayout == null)
                        return 0;
                    topTabStrip = topTabLayout.FindChildOfType<LinearLayout>();
                    return topTabLayout.TabCount;

                case ToolbarPlacement.Bottom:
                    bottomTabStrip = ViewGroup.FindChildOfType<BottomNavigationView>()?.GetChildAt(0) as ViewGroup;
                    if (bottomTabStrip == null)
                        return 0;
                    return bottomTabStrip.ChildCount;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddTabBadge(int tabIndex)
        {
            var page = Element.GetChildPageWithBadge(tabIndex);
            var placement = Element.OnThisPlatform().GetToolbarPlacement();
            var targetView = placement == ToolbarPlacement.Bottom ? bottomTabStrip?.GetChildAt(tabIndex) : topTabLayout?.GetTabAt(tabIndex).CustomView ?? topTabStrip?.GetChildAt(tabIndex);
            if (targetView is not ViewGroup targetLayout)
                return;

            var badgeView = targetLayout.FindChildOfType<FBadgeView>();
            if (badgeView == null)
            {
                var imageView = targetLayout.FindChildOfType<ImageView>();
                if (placement == ToolbarPlacement.Bottom)
                    badgeView = FBadgeView.ForTargetLayout(Context, imageView);
                else
                    badgeView = FBadgeView.ForTarget(Context, imageView?.Drawable != null ? imageView : targetLayout.FindChildOfType<TextView>());
            }
            BadgeViews[page] = badgeView;
            badgeView.UpdateFromElement(page);

            page.PropertyChanged -= OnTabbedPagePropertyChanged;
            page.PropertyChanged += OnTabbedPagePropertyChanged;
        }

        private void OnTabRemoved(object sender, ElementEventArgs e)
        {
            e.Element.PropertyChanged -= OnTabbedPagePropertyChanged;
            BadgeViews.Remove(e.Element);
        }

        private void OnTabAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is not Page page)
                return;
            AddTabBadge(Element.Children.IndexOf(page));
        }

        private void Cleanup(Xamarin.Forms.TabbedPage page)
        {
            if (page == null)
                return;

            foreach (var tab in page.Children.Select(c => c.GetPageWithBadge()))
                tab.PropertyChanged -= OnTabbedPagePropertyChanged;

            page.ChildRemoved -= OnTabRemoved;
            page.ChildAdded -= OnTabAdded;

            BadgeViews.Clear();
            topTabLayout = null;
            topTabStrip = null;
            bottomTabStrip = null;
        }
    }
}