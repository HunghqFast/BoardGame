using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLayoutStyle : FViewStyle, IFLayoutStyle
    {
        public Thickness Padding
        {
            get => (Thickness)GetValue(IFLayoutStyle.PaddingProperty);
            set => SetValue(IFLayoutStyle.PaddingProperty, value);
        }

        public override void SetBindingStyle(View view)
        {
            base.SetBindingStyle(view);
            if (view is Layout)
                view.SetBinding(Layout.PaddingProperty, IFLayoutStyle.PaddingProperty.PropertyName);
        }
    }
}