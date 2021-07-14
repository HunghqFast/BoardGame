using CoreAnimation;
using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using Syncfusion.XForms.iOS.ComboBox;
using System;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FSfComboBox), typeof(FSfComboboxRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FSfComboboxRenderer : SfComboBoxRenderer
    {
        private readonly CALayer CALayer;
        private FSfComboBox Current => Element as FSfComboBox;

        public FSfComboboxRenderer() : base()
        {
            CALayer = new CALayer();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FSfComboBox.CornerRadiusProperty.PropertyName || e.PropertyName == FSfComboBox.BorderWidthProperty.PropertyName)
            {
                UpdateRadius();
                return;
            }

            if(e.PropertyName == FSfComboBox.ReturnTypeProperty.PropertyName)
            {
                UpdateReturnType();
                return;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Syncfusion.XForms.ComboBox.SfComboBox> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
                e.OldElement.SizeChanged -= OnElementSizeChanged;
            if (e.NewElement != null)
                e.NewElement.SizeChanged += OnElementSizeChanged;

            if (Control == null)
                return;
            Control.TextField.LeftView = new UIView(new CGRect(0, 0, 0, 0));
            Control.TextField.LeftViewMode = UITextFieldViewMode.Always;

            UpdateInput();
            UpdateKeyboard();
            UpdateReturnType();
        }

        private void OnElementSizeChanged(object sender, EventArgs e)
        {
            UpdateRadius();
            if (Control == null)
                return;
            if (!Control.Layer.Sublayers.Contains(CALayer))
                Control.Layer.AddSublayer(CALayer);
            Control.TextField.BorderStyle = UITextBorderStyle.None;
        }

        private void UpdateRadius()
        {
            if (Control == null || Element is not FSfComboBox E)
                return;
            CALayer.Frame = new CGRect(0, 0, Control.Frame.Width, Control.Frame.Height);
            CALayer.BackgroundColor = UIColor.Clear.CGColor;
            CALayer.BorderColor = E.BorderColor.ToCGColor();
            CALayer.BorderWidth = E.BorderWidth;
            CALayer.CornerRadius = E.CornerRadius;
        }

        private void UpdateKeyboard()
        {
            //Control.TextField.ApplyKeyboard(Element.Keyboard);
            //Control.TextField.SpellCheckingType = UITextSpellCheckingType.No;
            //Control.TextField.AutocorrectionType = UITextAutocorrectionType.No;
            //Control.ReloadInputViews();
        }

        private void UpdateInput()
        {
            //Control.TextField.ShouldChangeCharacters = TextFieldChange;
        }

        private void UpdateReturnType()
        {
            Control.TextField.ReturnKeyType = Current.ReturnType.ToUIReturnKeyType();
        }
    }
}