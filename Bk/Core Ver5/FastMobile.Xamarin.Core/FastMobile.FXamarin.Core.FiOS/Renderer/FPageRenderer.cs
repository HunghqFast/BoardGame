using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FPage), typeof(FPageRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FPageRenderer : PageRenderer
    {
        private FPage Curent => Element as FPage;

        public FPageRenderer() : base()
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Curent?.OnAppeared();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Curent?.OnDisappeared();
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnPropertyChanged;
                if (e.NewElement.Parent is FNavigationPage nav)
                    nav.PropertyChanged += OnNavigationPagePropertyChanged;
            }

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnPropertyChanged;
                if (e.NewElement.Parent is FNavigationPage nav)
                    nav.PropertyChanged -= OnNavigationPagePropertyChanged;
            }
        }

        protected virtual void OnNavigationPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}