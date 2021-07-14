using Android.Content;
using Android.Text;
using Android.Text.Style;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FEntryBase), typeof(FEntryBaseRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FEntryBaseRenderer : EntryRenderer
    {
        private FEntryBase Current => Element as FEntryBase;

        public FEntryBaseRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Current == null)
                return;
            ModifyTextColor();
            Remove();
            UpdatePlaceholderFont();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Entry.IsEnabledProperty.PropertyName)
            {
                ModifyTextColor();
                return;
            }

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

        private void ModifyTextColor()
        {
            if (!Element.IsEnabled)
                Control?.SetTextColor(Element.TextColor.ToAndroid());
        }

        private void Remove()
        {
            Control.SetPadding(0, Control.PaddingTop, 0, Control.PaddingBottom);
            Control.Background = null;
        }

        private void UpdatePlaceholderFont()
        {
            if (Current == null || string.IsNullOrEmpty(Current.Placeholder))
                return;
            var placeholderFontSize = (int)Current.FontSize;
            var placeholderSpan = new SpannableString(Current.Placeholder);
            placeholderSpan.SetSpan(new AbsoluteSizeSpan(placeholderFontSize, true), 0, placeholderSpan.Length(), SpanTypes.InclusiveExclusive);
            placeholderSpan.SetSpan(new FTypefaceSpan(FUtility.FindFont(Current.PlaceholderFontFamily, FontExtensions.ToTypefaceStyle(Current.PlaceholderFontAttributes))), 0, placeholderSpan.Length(), SpanTypes.InclusiveExclusive);
            Control.HintFormatted = placeholderSpan;
        }
    }
}