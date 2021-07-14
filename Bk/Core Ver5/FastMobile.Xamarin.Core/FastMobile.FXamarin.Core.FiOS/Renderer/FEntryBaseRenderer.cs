using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FEntryBase), typeof(FEntryBaseRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FEntryBaseRenderer : EntryRenderer
    {
        private FEntryBase Current => Element as FEntryBase;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Current == null)
                return;

            Control.BorderStyle = UITextBorderStyle.None;
            UpdatePlaceholderFont();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == FEntryBase.PlaceholderFontFamilyProperty.PropertyName)
            {
                UpdatePlaceholderFont();
                return;
            }

            if (e.PropertyName == FEntryBase.PlaceholderFontAttributesProperty.PropertyName)
            {
                UpdatePlaceholderFont();
                return;
            }

            if (e.PropertyName == FEntryBase.PlaceholderProperty.PropertyName)
            {
                UpdatePlaceholderFont();
                return;
            }
        }

        private void UpdatePlaceholderFont()
        {
            if (Current == null || string.IsNullOrEmpty(Current.Placeholder))
                return;
            Control.AttributedPlaceholder = new NSAttributedString(Current.Placeholder, FUtility.FindFont(Current.PlaceholderFontFamily, (float)Current.FontSize), foregroundColor: Current.PlaceholderColor.ToUIColor(), backgroundColor: Current.BackgroundColor.ToUIColor(), paragraphStyle: new NSMutableParagraphStyle { Alignment = UITextAlignment.Left });
        }
    }
}