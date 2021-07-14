using Syncfusion.SfImageEditor.XForms;

namespace FastMobile.FXamarin.Core
{
    public class FImageEditor : SfImageEditor
    {
        public FImageEditor()
        {
            Base();
        }

        private void Base()
        {
            EnableZooming = true;
            MaximumZoomLevel = 8;
            PanningMode = PanningMode.TwoFinger;
        }
    }
}