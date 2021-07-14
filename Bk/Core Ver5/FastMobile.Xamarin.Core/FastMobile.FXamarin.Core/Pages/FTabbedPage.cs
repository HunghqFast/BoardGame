using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Page = Xamarin.Forms.Page;
using TabbedPage = Xamarin.Forms.TabbedPage;

namespace FastMobile.FXamarin.Core
{
    public class FTabbedPage : TabbedPage
    {
        public event EventHandler<FTabbedPageReselectedEventArgs> DoubleTabbed;

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create("IconSize", typeof(double), typeof(FTabbedPage), 18d);
        public static readonly BindableProperty Tab0IconNameProperty = BindableProperty.Create("Tab0IconName", typeof(string), typeof(FTabbedPage), "HomeOutline");
        public static readonly BindableProperty Tab1IconNameProperty = BindableProperty.Create("Tab1IconName", typeof(string), typeof(FTabbedPage), "FileDocumentOutline");
        public static readonly BindableProperty Tab2IconNameProperty = BindableProperty.Create("Tab2IconName", typeof(string), typeof(FTabbedPage), "FileDocumentEditOutline");
        public static readonly BindableProperty Tab3IconNameProperty = BindableProperty.Create("Tab3IconName", typeof(string), typeof(FTabbedPage), "BellOutline");
        public static readonly BindableProperty Tab4IconNameProperty = BindableProperty.Create("Tab4IconName", typeof(string), typeof(FTabbedPage), "Menu");
        public static readonly BindableProperty Tab0SelectedIconNameProperty = BindableProperty.Create("Tab0SelectedIconName", typeof(string), typeof(FTabbedPage), "Home");
        public static readonly BindableProperty Tab1SelectedIconNameProperty = BindableProperty.Create("Tab1SelectedIconName", typeof(string), typeof(FTabbedPage), "FileDocument");
        public static readonly BindableProperty Tab2SelectedIconNameProperty = BindableProperty.Create("Tab2SelectedIconName", typeof(string), typeof(FTabbedPage), "FileDocumentEdit");
        public static readonly BindableProperty Tab3SelectedIconNameProperty = BindableProperty.Create("Tab3SelectedIconName", typeof(string), typeof(FTabbedPage), "Bell");
        public static readonly BindableProperty Tab4SelectedIconNameProperty = BindableProperty.Create("Tab4SelectedIconName", typeof(string), typeof(FTabbedPage), "Menu");

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public string Tab0IconName
        {
            get => (string)GetValue(Tab0IconNameProperty);
            set => SetValue(Tab0IconNameProperty, value);
        }

        public string Tab1IconName
        {
            get => (string)GetValue(Tab1IconNameProperty);
            set => SetValue(Tab1IconNameProperty, value);
        }

        public string Tab2IconName
        {
            get => (string)GetValue(Tab2IconNameProperty);
            set => SetValue(Tab2IconNameProperty, value);
        }

        public string Tab3IconName
        {
            get => (string)GetValue(Tab3IconNameProperty);
            set => SetValue(Tab3IconNameProperty, value);
        }

        public string Tab4IconName
        {
            get => (string)GetValue(Tab4IconNameProperty);
            set => SetValue(Tab4IconNameProperty, value);
        }

        public string Tab0SelectedIconName
        {
            get => (string)GetValue(Tab0SelectedIconNameProperty);
            set => SetValue(Tab0SelectedIconNameProperty, value);
        }

        public string Tab1SelectedIconName
        {
            get => (string)GetValue(Tab1SelectedIconNameProperty);
            set => SetValue(Tab1SelectedIconNameProperty, value);
        }

        public string Tab2SelectedIconName
        {
            get => (string)GetValue(Tab2SelectedIconNameProperty);
            set => SetValue(Tab2SelectedIconNameProperty, value);
        }

        public string Tab3SelectedIconName
        {
            get => (string)GetValue(Tab3SelectedIconNameProperty);
            set => SetValue(Tab3SelectedIconNameProperty, value);
        }

        public string Tab4SelectedIconName
        {
            get => (string)GetValue(Tab4SelectedIconNameProperty);
            set => SetValue(Tab4SelectedIconNameProperty, value);
        }

        public static Type Type { get; }
        private Page BeforePage;

        static FTabbedPage()
        {
            Type = typeof(FTabbedPage);
        }

        public FTabbedPage() : base()
        {
            BarBackgroundColor = Color.White;
            FNavigationPage.SetBackButtonTitle(this, "");
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetOffscreenPageLimit(5);
            CurrentPageChanged += OnCurrentPageChanged;
        }

        public void LoadIcons()
        {
            int k = 0;
            Children.ForEach((x) =>
            {
                InitPageIcon(x, CurrentPage == x ? Type.GetPropValue(this, $"Tab{x}SelectedIconName").ToString() : Type.GetPropValue(this, $"Tab{x}IconName").ToString());
                k++;
            });
        }

        public void Add(params Page[] childrensPage)
        {
            childrensPage.ForIndex((x, i) =>
            {
                Children.Add(x);
                InitPageIcon(x, x == CurrentPage ? Type.GetPropValue(this, $"Tab{i}SelectedIconName").ToString() : Type.GetPropValue(this, $"Tab{i}IconName").ToString());
            });
        }

        public virtual async void OnTabReselected(object sender, FTabbedPageReselectedEventArgs e)
        {
            await e.Current?.ScrollToTop();
            DoubleTabbed?.Invoke(sender, e);
        }

        protected virtual void OnCurrentPageChanged(object sender, EventArgs e)
        {
            if (BeforePage != null)
                BeforePage.IconImageSource = Type.GetPropValue(this, $"Tab{Children.IndexOf(BeforePage)}IconName")?.ToString()?.ToImageSource(UnselectedTabColor, FSetting.SizeIconMenu);
            CurrentPage.IconImageSource = Type.GetPropValue(this, $"Tab{Children.IndexOf(CurrentPage)}SelectedIconName")?.ToString()?.ToImageSource(SelectedTabColor, FSetting.SizeIconMenu);
            BeforePage = CurrentPage;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsBusyProperty.PropertyName)
            {
                CurrentPage.IsBusy = IsBusy;
                return;
            }

            if (propertyName == Tab0IconNameProperty.PropertyName)
            {
                if (Children.Count < 1)
                    return;
                InitPageIcon(Children[0], Tab0IconName);
                return;
            }

            if (propertyName == Tab1IconNameProperty.PropertyName)
            {
                if (Children.Count < 2)
                    return;
                InitPageIcon(Children[1], Tab0IconName);
                return;
            }

            if (propertyName == Tab2IconNameProperty.PropertyName)
            {
                if (Children.Count < 3)
                    return;
                InitPageIcon(Children[2], Tab0IconName);
                return;
            }

            if (propertyName == Tab3IconNameProperty.PropertyName)
            {
                if (Children.Count < 4)
                    return;
                InitPageIcon(Children[3], Tab0IconName);
                return;
            }

            if (propertyName == Tab4IconNameProperty.PropertyName)
            {
                if (Children.Count < 5)
                    return;
                InitPageIcon(Children[4], Tab0IconName);
                return;
            }

            if (propertyName == Tab0SelectedIconNameProperty.PropertyName)
            {
                if (Children.Count < 1)
                    return;
                InitPageIcon(Children[0], Tab0SelectedIconName);
                return;
            }

            if (propertyName == Tab1SelectedIconNameProperty.PropertyName)
            {
                if (Children.Count < 2)
                    return;
                InitPageIcon(Children[1], Tab0SelectedIconName);
                return;
            }

            if (propertyName == Tab2SelectedIconNameProperty.PropertyName)
            {
                if (Children.Count < 3)
                    return;
                InitPageIcon(Children[2], Tab0SelectedIconName);
                return;
            }

            if (propertyName == Tab3SelectedIconNameProperty.PropertyName)
            {
                if (Children.Count < 4)
                    return;
                InitPageIcon(Children[3], Tab0SelectedIconName);
                return;
            }

            if (propertyName == Tab4SelectedIconNameProperty.PropertyName)
            {
                if (Children.Count < 5)
                    return;
                InitPageIcon(Children[4], Tab0SelectedIconName);
                return;
            }
        }

        private void InitPageIcon(Page page, string icon)
        {
            page.IconImageSource = icon.ToImageSource(CurrentPage == page ? SelectedTabColor : UnselectedTabColor, FSetting.SizeIconMenu);
        }
    }
}