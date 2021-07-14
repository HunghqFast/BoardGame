using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using System;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FTabbedPage), typeof(FTabbedRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    [Preserve]
    public class FTabbedRenderer : TabbedRenderer
    {
        private FTabbedPage Current => Element as FTabbedPage;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Cleanup(Tabbed);
            for (var i = 0; i < TabBar.Items.Length; i++)
            {
                AddTabBadge(i);
            }
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
            Tabbed.ChildAdded += OnTabAdded;
            Tabbed.ChildRemoved += OnTabRemoved;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            Cleanup(e.OldElement as TabbedPage);
            ReselectTabPage();
        }

        protected bool CheckValidTabIndex(Page page, out int tabIndex)
        {
            tabIndex = Tabbed.Children.IndexOf(page);
            if (tabIndex == -1 && page.Parent != null)
                tabIndex = Tabbed.Children.IndexOf(page.Parent);
            return tabIndex >= 0 && tabIndex < TabBar.Items.Length;
        }

        protected override void Dispose(bool disposing)
        {
            Cleanup(Tabbed);
            base.Dispose(disposing);
        }

        private void UpdateIcon()
        {
            if (TabBar?.Items == null)
                return;
            var tabs = Element as FTabbedPage;
            if (tabs != null)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    TabBar.Items[i].Image = MaxResizeImage(TabBar.Items[i].Image, FSetting.SizeIconMenu, FSetting.SizeIconMenu);
                    TabBar.Items[i].SelectedImage = MaxResizeImage(TabBar.Items[i].SelectedImage, FSetting.SizeIconMenu, FSetting.SizeIconMenu);
                }
            }
        }

        private void UpdateFont()
        {
            if (TabBar?.Items == null)
                return;
            if (Element is FTabbedPage)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    TabBar.Items[i].SetTitleTextAttributes(new UITextAttributes()
                    {
                        Font = UIFont.FromName(FSetting.FontText, FSetting.FontSizeLabelContent - 3),
                    }, UIControlState.Normal);
                }
            }
        }

        private UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            if (sourceImage == null)
                return sourceImage;
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1)
                return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new System.Drawing.SizeF((float)width, (float)height));
            sourceImage.Draw(new System.Drawing.RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        private void ReselectTabPage()
        {
            try
            {
                var tabbarController = (UITabBarController)this.ViewController;
                if (tabbarController != null)
                {
                    tabbarController.ViewControllerSelected -= OnTabbarControllerItemSelected;
                    tabbarController.ViewControllerSelected += OnTabbarControllerItemSelected;
                }
            }
            catch { }
        }

        private void OnTabbarControllerItemSelected(object sender, UITabBarSelectionEventArgs e)
        {
            if (Current != null)
                Current.OnTabReselected(Current, new FTabbedPageReselectedEventArgs(Current.CurrentPage as FPage));
        }

        private void AddTabBadge(int tabIndex)
        {
            var element = Tabbed.GetChildPageWithBadge(tabIndex);
            element.PropertyChanged += OnChildPropertyChanged;

            if (TabBar.Items.Length > tabIndex)
            {
                var tabBarItem = TabBar.Items[tabIndex];
                UpdateTabBadgeText(tabBarItem, element);
                UpdateTabBadgeColor(tabBarItem, element);
                UpdateTabBadgeTextAttributes(tabBarItem, element);
            }
        }

        private void UpdateTabBadgeText(UITabBarItem tabBarItem, Element element)
        {
            var text = FTabBadge.GetBadgeText(element);
            tabBarItem.BadgeValue = string.IsNullOrEmpty(text) ? null : text;
        }

        private void UpdateTabBadgeTextAttributes(UITabBarItem tabBarItem, Element element)
        {
            if (!tabBarItem.RespondsToSelector(new ObjCRuntime.Selector("setBadgeTextAttributes:forState:")))
            {
                return;
            }
            var attrs = new UIStringAttributes();
            var textColor = FTabBadge.GetBadgeTextColor(element);
            if (textColor != Color.Default)
            {
                attrs.ForegroundColor = textColor.ToUIColor();
            }
            var font = FTabBadge.GetBadgeFont(element);
            if (font != Font.Default)
            {
                attrs.Font = font.ToUIFont();
            }
            attrs.Font = UIFont.SystemFontOfSize(9);
            tabBarItem.SetBadgeTextAttributes(attrs, UIControlState.Normal);
        }

        private void UpdateTabBadgeColor(UITabBarItem tabBarItem, Element element)
        {
            if (!tabBarItem.RespondsToSelector(new ObjCRuntime.Selector("setBadgeColor:")))
                return;
            var tabColor = FTabBadge.GetBadgeColor(element);
            if (tabColor != Color.Default)
            {
                tabBarItem.BadgeColor = tabColor.ToUIColor();
            }
        }

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var page = sender as Page;
            if (page == null)
                return;
            if (e.PropertyName == Page.IconImageSourceProperty.PropertyName)
            {
                if (CheckValidTabIndex(page, out int tabIndex))
                {
                    UpdateTabBadgeText(TabBar.Items[tabIndex], page);
                    UpdateTabBadgeColor(TabBar.Items[tabIndex], page);
                    UpdateTabBadgeTextAttributes(TabBar.Items[tabIndex], page);
                }
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeTextProperty.PropertyName)
            {
                if (CheckValidTabIndex(page, out int tabIndex))
                    UpdateTabBadgeText(TabBar.Items[tabIndex], page);
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeColorProperty.PropertyName)
            {
                if (CheckValidTabIndex(page, out int tabIndex))
                    UpdateTabBadgeColor(TabBar.Items[tabIndex], page);
                return;
            }
            if (e.PropertyName == FTabBadge.BadgeTextColorProperty.PropertyName || e.PropertyName == FTabBadge.BadgeFontProperty.PropertyName)
            {
                if (CheckValidTabIndex(page, out int tabIndex))
                    UpdateTabBadgeTextAttributes(TabBar.Items[tabIndex], page);
                return;
            }
        }

        private void OnTabAdded(object sender, ElementEventArgs e)
        {
            var page = e.Element as Page;
            if (page == null)
                return;
            var tabIndex = Tabbed.Children.IndexOf(page);
            AddTabBadge(tabIndex);
        }

        private void OnTabRemoved(object sender, ElementEventArgs e)
        {
            e.Element.PropertyChanged -= OnChildPropertyChanged;
        }

        private void Cleanup(TabbedPage tabbedPage)
        {
            if (tabbedPage == null)
                return;
            foreach (var tab in tabbedPage.Children.Select(c => c.GetPageWithBadge()))
                tab.PropertyChanged -= OnChildPropertyChanged;
            tabbedPage.ChildAdded -= OnTabAdded;
            tabbedPage.ChildRemoved -= OnTabRemoved;
        }
    }
}