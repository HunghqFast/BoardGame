using Android.Content;
using Android.Graphics.Drawables;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Syncfusion.XForms.Android.ComboBox;
using Syncfusion.XForms.ComboBox;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FSfComboBox), typeof(FSfComboboxRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FSfComboboxRenderer : SfComboBoxRenderer
    {
        private FSfComboBox Current => Element as FSfComboBox;

        public FSfComboboxRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SfComboBox> e)
        {
            base.OnElementChanged(e);
            Remove();
            UpdateRadius();
            UpdateReturnType();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FSfComboBox.CornerRadiusProperty.PropertyName || e.PropertyName == FSfComboBox.BorderWidthProperty.PropertyName)
            {
                UpdateRadius();
                return;
            }

            if (e.PropertyName == FSfComboBox.ReturnTypeProperty.PropertyName)
            {
                UpdateReturnType();
                return;
            }
        }

        private void Remove()
        {
            if (Control == null)
                return;
            Control.SetPadding(0, 0, 0, 0);
            Control.Background = null;

            var t = Control.GetAutoEditText();
            t.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            t.SetPadding(0, 0, 0, 0);
        }

        private void UpdateRadius()
        {
            if (Control == null && Element == null)
                return;
            GradientDrawable gd = new GradientDrawable();
            gd.SetColor(Android.Graphics.Color.Transparent);
            gd.SetCornerRadius(((FSfComboBox)Element).CornerRadius);
            gd.SetStroke(Convert.ToInt32(((FSfComboBox)Element).BorderWidth), Element.BorderColor.ToAndroid());
            Control.SetBackgroundDrawable(gd);
        }

        private void UpdateReturnType()
        {
            var t = Control.GetAutoEditText();
            t.ImeOptions = Current.ReturnType.ToAndroidImeAction();
        }
    }
}