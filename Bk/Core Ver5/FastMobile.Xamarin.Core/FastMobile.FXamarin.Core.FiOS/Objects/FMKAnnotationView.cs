using CoreGraphics;
using Foundation;
using MapKit;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FMKAnnotationView : MKAnnotationView
    {
        public FMKAnnotationView() : base()
        {
        }

        public FMKAnnotationView(NSCoder coder) : base(coder)
        {
        }

        public FMKAnnotationView(CGRect frame) : base(frame)
        {
        }

        public FMKAnnotationView(IMKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
        {
        }

        protected FMKAnnotationView(NSObjectFlag t) : base(t)
        {
        }
    }
}