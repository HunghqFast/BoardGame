using Android.Content;
using Android.Graphics;
using Android.Widget;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Platform = Xamarin.Essentials.Platform;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

[assembly: ExportRenderer(typeof(FNavigationPage), typeof(FNavigationPageRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FNavigationPageRenderer : NavigationPageRenderer
    {
        private TextView Title;
        private Toolbar Toolbar;
        private FNavigationPage Current => (FNavigationPage)Element;

        private static int EnterBottom, EnterLeft, EnterRight, EnterTop, ExitBottom, ExitLeft, ExitRight, ExitTop, FadeIn, FadeOut, FlipIn, FlipOut, ScaleIn, ScaleOut;

        public FNavigationPageRenderer(Context context) : base(context)
        {
        }

        public static void Init(string enterBottom = "EnterBottom", string enterLeft = "EnterLeft", string enterRight = "EnterRight", string enterTop = "EnterTop",
            string exitBottom = "ExitBottom", string exitLeft = "ExitLeft", string exitRight = "ExitRight", string exitTop = "ExitTop", string fadeIn = "FadeIn",
            string fadeOut = "FadeOut", string flipIn = "FlipIn", string flipOut = "FlipOut", string scaleIn = "ScaleIn", string scaleOut = "ScaleOut")
        {
            try
            {
                var T = Assembly.GetCallingAssembly().TypeByAssemply("Resource", "Animation");
                EnterBottom = Convert.ToInt32(T.GetStaticFieldValue(enterBottom));
                EnterLeft = Convert.ToInt32(T.GetStaticFieldValue(enterLeft));
                EnterRight = Convert.ToInt32(T.GetStaticFieldValue(enterRight));
                EnterTop = Convert.ToInt32(T.GetStaticFieldValue(enterTop));
                ExitBottom = Convert.ToInt32(T.GetStaticFieldValue(exitBottom));
                ExitLeft = Convert.ToInt32(T.GetStaticFieldValue(exitLeft));
                ExitRight = Convert.ToInt32(T.GetStaticFieldValue(exitRight));
                ExitTop = Convert.ToInt32(T.GetStaticFieldValue(exitTop));
                FadeIn = Convert.ToInt32(T.GetStaticFieldValue(fadeIn));
                FadeOut = Convert.ToInt32(T.GetStaticFieldValue(fadeOut));
                FlipIn = Convert.ToInt32(T.GetStaticFieldValue(flipIn));
                FlipOut = Convert.ToInt32(T.GetStaticFieldValue(flipOut));
                ScaleIn = Convert.ToInt32(T.GetStaticFieldValue(scaleIn));
                ScaleOut = Convert.ToInt32(T.GetStaticFieldValue(scaleOut));
            }
            catch { }
        }

        protected override void OnAttachedToWindow()
        {
            Current?.OnAppreared();
            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            Current?.OnDisappreared();
            base.OnDetachedFromWindow();
        }

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
            if (child is Toolbar toolbar)
            {
                Toolbar = toolbar;
                Toolbar.ChildViewAdded += ToolbarChildViewAdded;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FNavigationPage.TitleFontFamilyProperty.PropertyName)
            {
                UpdateFontFamily();
                return;
            }
            if (e.PropertyName == FNavigationPage.TitleTextAlignmentProperty.PropertyName)
            {
                UpdateAlignment();
                return;
            }
            if (e.PropertyName == FNavigationPage.TitleFontAttributesProperty.PropertyName)
            {
                UpdateFontFamily();
                return;
            }
            if (e.PropertyName == FNavigationPage.TitleFontSizeProperty.PropertyName)
            {
                UpdateFontSize();
                return;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            UpdateAlignment();
            UpdateFontSize();
            UpdateFontFamily();
        }

        protected override void SetupPageTransition(AndroidX.Fragment.App.FragmentTransaction transaction, bool isPush)
        {
            base.SetupPageTransition(transaction, isPush);
            if (FadeIn == 0)
                return;

            switch (Current.TransitionType)
            {
                case FTransitionType.None:
                    return;

                case FTransitionType.Default:
                    return;

                case FTransitionType.Fade:
                    transaction.SetCustomAnimations(FadeIn, FadeOut, FadeOut, FadeIn);
                    break;

                case FTransitionType.Flip:
                    transaction.SetCustomAnimations(FlipIn, FlipOut, FlipOut, FlipIn);
                    break;

                case FTransitionType.Scale:
                    transaction.SetCustomAnimations(ScaleIn, ScaleOut, ScaleOut, ScaleIn);
                    break;

                case FTransitionType.SlideFromLeft:
                    if (isPush)
                        transaction.SetCustomAnimations(EnterLeft, ExitRight, EnterRight, ExitLeft);
                    else
                        transaction.SetCustomAnimations(EnterRight, ExitLeft, EnterLeft, ExitRight);
                    break;

                case FTransitionType.SlideFromRight:
                    if (isPush)
                        transaction.SetCustomAnimations(EnterRight, ExitLeft, EnterLeft, ExitRight);
                    else
                        transaction.SetCustomAnimations(EnterLeft, ExitRight, EnterRight, ExitLeft);
                    break;

                case FTransitionType.SlideFromTop:
                    if (isPush)
                        transaction.SetCustomAnimations(EnterTop, ExitBottom, EnterBottom, ExitTop);
                    else
                        transaction.SetCustomAnimations(EnterBottom, ExitTop, EnterTop, ExitBottom);
                    break;

                case FTransitionType.SlideFromBottom:
                    if (isPush)
                        transaction.SetCustomAnimations(EnterBottom, ExitTop, EnterTop, ExitBottom);
                    else
                        transaction.SetCustomAnimations(EnterTop, ExitBottom, EnterBottom, ExitTop);
                    break;

                default:
                    return;
            }
        }

        private void UpdateFontFamily()
        {
            if (Title == null)
                InitTitle();
            if (Title == null)
                return;

            if (!string.IsNullOrEmpty(Current.TitleFontFamily))
                Title.Typeface = Typeface.Create(Current.TitleFontFamily, ConvertFontAttributesToTypefaceStyle(Current.TitleFontAttributes));
        }

        private void UpdateFontSize()
        {
            if (Title == null)
                InitTitle();
            if (Title == null)
                return;

            if (Current.TitleFontSize != 0)
                Title.TextSize = Current.TitleFontSize;
        }

        private void UpdateAlignment()
        {
            if (Title == null)
                InitTitle();
            if (Title == null)
                return;

            if (Current.TitleTextAlignment == Xamarin.Forms.TextAlignment.Center)
            {
                Title.SetX(Toolbar.Width / 2 - Title.Width / 2);
                return;
            }

            if (Current.TitleTextAlignment == Xamarin.Forms.TextAlignment.Start)
            {
                Title.SetX(10);
                return;
            }

            Title.SetX(Toolbar.Width - Title.Width);
        }

        private void InitTitle()
        {
            if (Toolbar == null)
                Toolbar = GetToolbar();
            if (Toolbar == null)
                return;

            for (var i = 0; i < Toolbar.ChildCount; i++)
            {
                if (Toolbar.GetChildAt(i) is TextView t)
                    Title = t;
            }
        }

        private TypefaceStyle ConvertFontAttributesToTypefaceStyle(FontAttributes fontAttributes)
        {
            if (fontAttributes == FontAttributes.Bold)
                return TypefaceStyle.Bold;
            if (fontAttributes == FontAttributes.Italic)
                return TypefaceStyle.Italic;
            return TypefaceStyle.Normal;
        }

        private void ToolbarChildViewAdded(object sender, ChildViewAddedEventArgs e)
        {
            var view = e.Child.GetType();
            if (e.Child is TextView child)
            {
                child.Typeface = Font.OfSize(FSetting.FontText, FSetting.FontSizeLabelTitle).ToTypeface();
                Toolbar.ChildViewAdded -= ToolbarChildViewAdded;
            }
        }

        private Toolbar GetToolbar()
        {
            return Platform.CurrentActivity.FindViewById<Toolbar>(Resources.GetIdentifier("toolbar", "id", Context.PackageName));
        }
    }
}