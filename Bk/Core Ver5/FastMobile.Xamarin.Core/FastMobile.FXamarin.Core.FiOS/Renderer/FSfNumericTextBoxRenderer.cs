using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FSfNumericTextBox), typeof(FSfNumericTextBoxRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FSfNumericTextBoxRenderer : SfNumericTextBoxRenderer
    {
        public FSfNumericTextBoxRenderer() : base()
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Control.Frame = new CGRect(Control.Frame.X, Control.Frame.Y, Control.Frame.Width + 10, Control.Frame.Height);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Syncfusion.SfNumericTextBox.XForms.SfNumericTextBox> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;
            Remove();
            UpdateRadius();
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
            Control.SetStaticPropValue("EditTextLeftPadding", 0);
        }

        private void UpdateRadius()
        {
            if (Element is not FSfNumericTextBox E || Control == null)
                return;
            Control.BorderStyle = UITextBorderStyle.None;
            Control.Layer.BorderColor = E.BorderColor.ToCGColor();
            Control.Layer.CornerRadius = E.CornerRadius;
            Control.Layer.BorderWidth = E.BorderWidth;
        }
    }
}