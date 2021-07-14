using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FTypefaceSpan : TypefaceSpan
    {
        private readonly Typeface typeface;

        public FTypefaceSpan(string family) : base(family)
        {
        }

        public FTypefaceSpan(Typeface type) : base("")
        {
            typeface = type;
        }

        public FTypefaceSpan(string family, Typeface type) : base(family)
        {
            typeface = type;
        }

        public override void UpdateDrawState(TextPaint ds)
        {
            ApplyCustomTypeFace(ds, typeface);
        }

        public override void UpdateMeasureState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint, typeface);
        }

        private static void ApplyCustomTypeFace(Paint paint, Typeface tf)
        {
            paint.SetTypeface(tf);
        }
    }
}