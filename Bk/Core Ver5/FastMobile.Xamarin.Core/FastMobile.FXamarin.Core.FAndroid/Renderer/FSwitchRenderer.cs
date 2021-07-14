using Android.Content;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FSwitch), typeof(FSwitchRenderer))]
namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FSwitchRenderer : SwitchRenderer
    {
        FSwitch Current => Element as FSwitch;
        public FSwitchRenderer(Context context) : base(context)
        {

        }

        protected override void Dispose(bool disposing)
        {
            OnElementChanged(new ElementChangedEventArgs<Switch>(Current, null));
            Control.CheckedChange -= OnCheckedChanged;
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnControlPropertyChanged;
            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnControlPropertyChanged;
                UpdateAll(Control.Checked);
                Control.CheckedChange -= OnCheckedChanged;
                Control.CheckedChange += OnCheckedChanged;
            }
        }


        protected virtual void OnControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FSwitch.OnColorProperty.PropertyName)
            {
                if (Current.IsToggled) UpdateOnColor();
                return;
            }

            if (e.PropertyName == FSwitch.OffColorProperty.PropertyName)
            {
                if (!Current.IsToggled) UpdateOffColor();
                return;
            }

            if (e.PropertyName == FSwitch.OnThumbColorProperty.PropertyName)
            {
                if (Current.IsToggled) UpdateThumOnColor();
                return;
            }

            if (e.PropertyName == FSwitch.OffThumbColorProperty.PropertyName)
            {
                if (!Current.IsToggled) UpdateThumOffColor();
                return;
            }

            if (e.PropertyName == FSwitch.IsEnabledProperty.PropertyName || e.PropertyName == FSwitch.IsToggledProperty.PropertyName)
            {
                Control.Checked = Current.IsToggled;
                UpdateAll(Control.Checked);
                return;
            }
        }

        private void UpdateAll(bool isCheck)
        {
            if (isCheck)
            {
                UpdateOnColor();
                UpdateThumOnColor();
                return;
            }
            UpdateOffColor();
            UpdateThumOffColor();
        }

        private void UpdateOnColor()
        {
            Control.TrackDrawable.SetColorFilter(Current.OnColor.ToAndroid(), FilterMode.SrcAtop);
        }

        private void UpdateOffColor()
        {
            Control.TrackDrawable.SetColorFilter(Current.OffColor.ToAndroid(), FilterMode.SrcAtop);
        }

        private void UpdateThumOnColor()
        {
            Control.ThumbDrawable.SetColorFilter(Current.OnThumbColor, FilterMode.SrcAtop);
        }

        private void UpdateThumOffColor()
        {
            Control.ThumbDrawable.SetColorFilter(Current.OffThumbColor, FilterMode.SrcAtop);
        }

        private void OnCheckedChanged(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs e)
        {
            Current.IsToggled = e.IsChecked;
        }
    }
}