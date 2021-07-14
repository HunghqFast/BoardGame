using System.Windows.Input;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPressedEffect : RoutingEffect
    {
        public FPressedEffect() : base("Core.FPressedEffect")
        {
        }

        public static readonly BindableProperty LongTapCommandProperty = BindableProperty.CreateAttached("LongTapCommand", typeof(ICommand), typeof(FPressedEffect), (object)null);

        public static ICommand GetLongTapCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(LongTapCommandProperty);
        }

        public static void SetLongTapCommand(BindableObject view, ICommand value)
        {
            view.SetValue(LongTapCommandProperty, value);
        }

        public static readonly BindableProperty LongParameterProperty = BindableProperty.CreateAttached("LongParameter", typeof(object), typeof(FPressedEffect), (object)null);

        public static object GetLongParameter(BindableObject view)
        {
            return view.GetValue(LongParameterProperty);
        }

        public static void SetLongParameter(BindableObject view, object value)
        {
            view.SetValue(LongParameterProperty, value);
        }

        public static readonly BindableProperty TapCommandProperty = BindableProperty.CreateAttached("TapCommand", typeof(ICommand), typeof(FPressedEffect), (object)null);

        public static ICommand GetTapCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(TapCommandProperty);
        }

        public static void SetTapCommand(BindableObject view, ICommand value)
        {
            view.SetValue(TapCommandProperty, value);
        }

        public static readonly BindableProperty TapParameterProperty = BindableProperty.CreateAttached("TapParameter", typeof(object), typeof(FPressedEffect), (object)null);

        public static object GetTapParameter(BindableObject view)
        {
            return view.GetValue(TapParameterProperty);
        }

        public static void SetTapParameter(BindableObject view, object value)
        {
            view.SetValue(TapParameterProperty, value);
        }
    }
}