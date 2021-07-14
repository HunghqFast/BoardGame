using Android.Graphics.Drawables;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Syncfusion.SfNumericTextBox.XForms;
using Syncfusion.SfNumericTextBox.XForms.Droid;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FSfNumericTextBox), typeof(FSfNumericTextBoxRenderer))]
namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FSfNumericTextBoxRenderer : SfNumericTextBoxRenderer
    {
        public FSfNumericTextBoxRenderer() : base()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SfNumericTextBox> e)
        {
            base.OnElementChanged(e);
            Remove();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FSfNumericTextBox.CornerRadiusProperty.PropertyName || e.PropertyName == FSfNumericTextBox.BorderWidthProperty.PropertyName)
            {
                UpdateRadius();
            }
        }

        private void Remove()
        {
            if (Control == null)
                return;

            Control.Background = null;
            Element.Margin = new Thickness(Element.Margin.Left - (Control.PaddingLeft / DeviceDisplay.MainDisplayInfo.Density), Element.Margin.Top, Element.Margin.Right, Element.Margin.Bottom);
        }

        private void UpdateRadius()
        {
            if (Control == null && Element == null)
                return;
            GradientDrawable gd = new GradientDrawable();
            gd.SetColor(Android.Graphics.Color.Transparent);
            gd.SetCornerRadius(((FSfNumericTextBox)Element).CornerRadius);
            gd.SetStroke(Convert.ToInt32(((FSfNumericTextBox)Element).BorderWidth), Element.BorderColor.ToAndroid());
            Control.SetBackgroundDrawable(gd);
        }
    }
}