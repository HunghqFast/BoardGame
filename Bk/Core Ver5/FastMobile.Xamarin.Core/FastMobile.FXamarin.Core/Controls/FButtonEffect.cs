using Syncfusion.XForms.EffectsView;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FButtonEffect : SfEffectsView
    {

        public FButtonEffect() : base()
        {
            TouchDownEffects = SfEffects.Ripple;
            CornerRadius = 5;
            RippleAnimationDuration = 150;
            RippleColor = Color.FromHex("#50808080");
        }

        public FButtonEffect(object sender, Action<object, IFDataEvent> action) : base()
        {
            TouchDownEffects = SfEffects.Ripple;
            CornerRadius = 5;
            RippleAnimationDuration = 150;
            RippleColor = Color.FromHex("#50808080");
            TouchUp += OnTouchUp;
            async void OnTouchUp(object s, EventArgs e)
            {
                await Task.Delay(Convert.ToInt32(RippleAnimationDuration * 1.5));
                action?.Invoke(sender, new FDataEventArgs(BindingContext));
            }
        }
    }
}