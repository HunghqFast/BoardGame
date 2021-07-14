using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FSwitch), typeof(FSwitchRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FSwitchRenderer : SwitchRenderer
    {
        private FSwitch Current => Element as FSwitch;

        public FSwitchRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);
            if (Control == null || Current == null)
                return;
            UpdateLayer();
            UpdateAll();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
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
                UpdateAll();
                return;
            }
        }

        private void UpdateAll()
        {
            if (Current.IsToggled)
            {
                UpdateOnColor();
                UpdateThumOnColor();
                Control.BackgroundColor = Color.Transparent.ToUIColor();
                return;
            }
            UpdateOffColor();
            UpdateThumOffColor();
            Control.BackgroundColor = Current.OffColor.ToUIColor();
        }

        private void UpdateLayer()
        {
            if (Control.Layer != null)
                Control.Layer.CornerRadius = 16.0f;
        }

        private void UpdateOnColor()
        {
            Control.OnTintColor = Current.OnColor.ToUIColor();
        }

        private void UpdateOffColor()
        {
            var color = Current.OffColor.ToUIColor();
            Control.TintColor = color;
            Control.BackgroundColor = color;
        }

        private void UpdateThumOnColor()
        {
            Control.ThumbTintColor = Current.OnThumbColor.ToUIColor();
            Control.BackgroundColor = Color.Transparent.ToUIColor();
        }

        private void UpdateThumOffColor()
        {
            Control.ThumbTintColor = Current.OffThumbColor.ToUIColor();
        }
    }
}