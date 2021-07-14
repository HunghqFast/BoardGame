using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using Android.Views.InputMethods;
using Android.Widget;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FSearchBar), typeof(FSearchBarRenderer))]
namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FSearchBarRenderer : SearchBarRenderer
    {
        private EditText EditText;
        private FSearchBar Current => Element as FSearchBar;
        private const float DefaultCornerRadiusDip = 10;
        public FSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;
            Contructor();

            UpdateIcon();

            if (EditText == null)
                return;

            EditText.EditorAction += OnEdittorAction;
            UpdateSearchBox();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == FSearchBar.TextColorProperty.PropertyName)
            {
                UpdateIcon();
                return;
            }

            if (e.PropertyName == FSearchBar.SearchBoxColorProperty.PropertyName)
            {
                UpdateSearchBox();
                return;
            }
        }

        private void Contructor()
        {
            EditText ??= Control.GetChildrenOfType<EditText>().FirstOrDefault();
        }

        private void UpdateIcon()
        {
            var searchIconId = Control.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            int searchViewCloseButtonId = Control.Resources.GetIdentifier("android:id/search_close_btn", null, null);

            if (searchIconId > 0)
            {
                var searchPlateIcon = Control.FindViewById(searchIconId);
                (searchPlateIcon as ImageView)?.SetColorFilter(Current.TextColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            }

            if (searchViewCloseButtonId > 0)
            {
                var closeIcon = Control.FindViewById(searchViewCloseButtonId);
                (closeIcon as ImageView)?.SetColorFilter(Current.TextColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            }
        }

        private void UpdateSearchBox()
        {
            var t = CreateBackgroundShape();
            t.SetColorFilter(Current.SearchBoxColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            EditText.SetBackground(t);
        }

        private void OnEdittorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Search)
            {
                Current.OnSearchButtonPressed();
                SClearFocus();
            }
        }

        private void SClearFocus()
        {
            Control?.ClearFocus();
            EditText?.ClearFocus();
        }

        private ShapeDrawable CreateBackgroundShape()
        {
            var radius = DipToPixels(DefaultCornerRadiusDip);
            var outerR = new float[] { radius, radius, radius, radius, radius, radius, radius, radius };
            return new ShapeDrawable(new RoundRectShape(outerR, null, null));
        }

        private int DipToPixels(float dip)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, Resources.DisplayMetrics);
        }
    }
}