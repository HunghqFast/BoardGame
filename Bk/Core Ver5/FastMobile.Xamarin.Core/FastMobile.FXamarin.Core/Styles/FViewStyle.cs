using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FViewStyle : BindableObject, IFViewStyle
    {
        public LayoutOptions HorizontalOptions
        {
            get => (LayoutOptions)GetValue(IFViewStyle.HorizontalOptionsProperty);
            set => SetValue(IFViewStyle.HorizontalOptionsProperty, value);
        }

        public LayoutOptions VerticalOptions
        {
            get => (LayoutOptions)GetValue(IFViewStyle.VerticalOptionsProperty);
            set => SetValue(IFViewStyle.VerticalOptionsProperty, value);
        }

        public Color BackgroundColor
        {
            get => (Color)GetValue(IFViewStyle.BackgroundColorProperty);
            set => SetValue(IFViewStyle.BackgroundColorProperty, value);
        }

        public Thickness Margin
        {
            get => (Thickness)GetValue(IFViewStyle.MarginProperty);
            set => SetValue(IFViewStyle.MarginProperty, value);
        }

        public double HeightRequest
        {
            get => (double)GetValue(IFViewStyle.HeightRequestProperty);
            set => SetValue(IFViewStyle.HeightRequestProperty, value);
        }

        public double WidthRequest
        {
            get => (double)GetValue(IFViewStyle.WidthRequestProperty);
            set => SetValue(IFViewStyle.WidthRequestProperty, value);
        }

        public virtual void SetBindingStyle(View view)
        {
            view.SetBinding(View.HorizontalOptionsProperty, IFViewStyle.HorizontalOptionsProperty.PropertyName);
            view.SetBinding(View.BackgroundColorProperty, IFViewStyle.BackgroundColorProperty.PropertyName);
            view.SetBinding(View.HorizontalOptionsProperty, IFViewStyle.HorizontalOptionsProperty.PropertyName);
            view.SetBinding(View.VerticalOptionsProperty, IFViewStyle.VerticalOptionsProperty.PropertyName);
            view.SetBinding(View.MarginProperty, IFViewStyle.MarginProperty.PropertyName);
            view.SetBinding(View.HeightRequestProperty, IFViewStyle.HeightRequestProperty.PropertyName);
            view.SetBinding(View.WidthRequestProperty, IFViewStyle.WidthRequestProperty.PropertyName);
        }
    }
}