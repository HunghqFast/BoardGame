using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//using FastMobile.FXamarin.Core.FAndroid;
//using FastMobile.FXamarin.Core;

//[assembly: ExportRenderer(typeof(FTabbedPage), typeof(FLegacyBadgedTabbedRenderer))]
namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FLegacyBadgedTabbedRenderer : TabbedRenderer
    {
        protected const int DelayBeforeTabAdded = 10;
        protected readonly Dictionary<Element, FBadgeView> BadgeViews = new Dictionary<Element, FBadgeView>();
        protected LinearLayout tabLinearLayout;
        public static int InitializationDelayInMiliseconds = 600;

        public FLegacyBadgedTabbedRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            Cleanup(e.OldElement);
            Cleanup(Element);

            Element.ChildAdded += OnTabAdded;
            Element.ChildRemoved += OnTabRemoved;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Initialize();
        }

        protected virtual void Initialize()
        {
            IViewParent root = ViewGroup;
            while (root.Parent?.Parent != null)
            {
                root = root.Parent;
            }

            if (root is not ViewGroup rootGroup)
                return;

            for (var i = 0; i < rootGroup.ChildCount; i++)
                tabLinearLayout = tabLinearLayout ?? (rootGroup.GetChildAt(i) as ViewGroup)?.FindChildOfType<HorizontalScrollView>()?.FindChildOfType<LinearLayout>();

            if (tabLinearLayout == null || tabLinearLayout.ChildCount == 0)
                return;

            for (var i = 0; i < tabLinearLayout.ChildCount; i++)
                AddTabBadge(i);
        }

        private void AddTabBadge(int tabIndex)
        {
            if (tabLinearLayout.GetChildAt(tabIndex) is not ViewGroup view || tabIndex >= Element.Children.Count)
                return;

            var page = Element.GetChildPageWithBadge(tabIndex);
            var badgeView = view.FindChildOfType<FBadgeView>();
            if (badgeView == null)
            {
                var badgeTarget = view.FindChildOfType<TextView>();
                if (badgeTarget == null)
                    return;

                badgeView = FBadgeView.ForTarget(Context, badgeTarget);
            }
            BadgeViews[page] = badgeView;
            badgeView.UpdateFromElement(page);
            page.PropertyChanged += OnTabbedPagePropertyChanged;
        }

        protected virtual void OnTabbedPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Element element)
                return;

            if (BadgeViews.TryGetValue(element, out var badgeView))
                badgeView.UpdateFromPropertyChangedEvent(element, e);
        }

        private void OnTabRemoved(object sender, ElementEventArgs e)
        {
            e.Element.PropertyChanged -= OnTabbedPagePropertyChanged;
            BadgeViews.Remove(e.Element);
        }

        private void OnTabAdded(object sender, ElementEventArgs e)
        {
            if (!(e.Element is Page page))
                return;
            AddTabBadge(Element.Children.IndexOf(page));
        }

        protected override void Dispose(bool disposing)
        {
            Cleanup(Element);
            base.Dispose(disposing);
        }

        private void Cleanup(TabbedPage page)
        {
            if (page == null)
                return;

            foreach (var tab in page.Children.Select(c => c.GetPageWithBadge()))
                tab.PropertyChanged -= OnTabbedPagePropertyChanged;

            page.ChildRemoved -= OnTabRemoved;
            page.ChildAdded -= OnTabAdded;

            BadgeViews.Clear();
            tabLinearLayout = null;
        }
    }
}