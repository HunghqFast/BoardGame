using CoreAnimation;
using CoreGraphics;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FNavigationPage), typeof(FNavigationPageRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FNavigationPageRenderer : NavigationRenderer
    {
        private FNavigationPage Current => Element as FNavigationPage;
        private FTransitionType TransitionType => Current.TransitionType;

        public FNavigationPageRenderer() : base()
        {
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            if (TransitionType == FTransitionType.None || TransitionType == FTransitionType.Default || !animated)
            {
                base.PushViewController(viewController, animated);
                return;
            }

            if (TransitionType == FTransitionType.Fade)
                FadeAnimation(View);
            else if (TransitionType == FTransitionType.Flip)
                FlipAnimation(View);
            else if (TransitionType == FTransitionType.Scale)
                ScaleAnimation(View);
            else
            {
                var transition = CATransition.CreateAnimation();
                transition.Duration = 0.2f;
                transition.Type = CAAnimation.TransitionPush;

                switch (TransitionType)
                {
                    case FTransitionType.SlideFromBottom:
                        transition.Subtype = CAAnimation.TransitionFromBottom;
                        break;

                    case FTransitionType.SlideFromLeft:
                        transition.Subtype = CAAnimation.TransitionFromLeft;
                        break;

                    case FTransitionType.SlideFromRight:
                        transition.Subtype = CAAnimation.TransitionFromRight;
                        break;

                    case FTransitionType.SlideFromTop:
                        transition.Subtype = CAAnimation.TransitionFromTop;
                        break;
                }

                View.Layer.AddAnimation(transition, null);
            }

            base.PushViewController(viewController, false);
        }

        public override UIViewController PopViewController(bool animated)
        {
            if (TransitionType == FTransitionType.None || TransitionType == FTransitionType.Default || !animated)
                return base.PopViewController(animated);

            if (TransitionType == FTransitionType.Fade)
                FadeAnimation(View);
            else if (TransitionType == FTransitionType.Flip)
                FlipAnimation(View);
            else if (TransitionType == FTransitionType.Scale)
                ScaleAnimation(View);
            else
            {
                var transition = CATransition.CreateAnimation();
                transition.Duration = 0.5f;
                transition.Type = CAAnimation.TransitionPush;

                switch (TransitionType)
                {
                    case FTransitionType.SlideFromBottom:
                        transition.Subtype = CAAnimation.TransitionFromTop;
                        break;

                    case FTransitionType.SlideFromLeft:
                        transition.Subtype = CAAnimation.TransitionFromRight;
                        break;

                    case FTransitionType.SlideFromRight:
                        transition.Subtype = CAAnimation.TransitionFromLeft;
                        break;

                    case FTransitionType.SlideFromTop:
                        transition.Subtype = CAAnimation.TransitionFromBottom;
                        break;
                }

                View.Layer.AddAnimation(transition, null);
            }

            return base.PopViewController(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateBarColor();
        }

        public override void ViewDidAppear(bool animated)
        {
            Current?.OnAppreared();
            base.ViewDidAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            Current?.OnDisappreared();
            base.ViewDidDisappear(animated);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            if (e.NewElement != null)
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
        }

        private void UpdateBarColor()
        {
            var gradientLayer = new CAGradientLayer
            {
                Frame = new CGRect(0, 0, NavigationBar.Frame.Width, UIApplication.SharedApplication.StatusBarFrame.Height + NavigationBar.Frame.Height),
                Colors = new CGColor[] { Current.StartColor.ToCGColor(), Current.EndColor.ToCGColor() },
                StartPoint = new CGPoint(0.0, 0.5),
                EndPoint = new CGPoint(1.0, 0.5)
            };

            UIGraphics.BeginImageContext(gradientLayer.Bounds.Size);
            gradientLayer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.SetBackgroundImage(image, UIBarMetrics.Default);
        }

        private void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FNavigationPage.StartColorProperty.PropertyName || e.PropertyName == FNavigationPage.EndColorProperty.PropertyName)
            {
                UpdateBarColor();
                return;
            }
        }

        private void FadeAnimation(UIView view, double duration = 0.3)
        {
            view.Alpha = 0.0f;
            view.Transform = CGAffineTransform.MakeIdentity();
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => view.Alpha = 1.0f, null);
        }

        private void FlipAnimation(UIView view, double duration = 0.2)
        {
            var m34 = (nfloat)(-1 * 0.001);
            var initialTransform = CATransform3D.Identity;
            initialTransform.m34 = m34;
            initialTransform = initialTransform.Rotate((nfloat)(1 * Math.PI * 0.5), 0.0f, 1.0f, 0.0f);

            view.Alpha = 0.0f;
            view.Layer.Transform = initialTransform;
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    view.Layer.AnchorPoint = new CGPoint((nfloat)0.5, 0.5f);
                    var newTransform = CATransform3D.Identity;
                    newTransform.m34 = m34;
                    view.Layer.Transform = newTransform;
                    view.Alpha = 1.0f;
                },
                null
            );
        }

        private void ScaleAnimation(UIView view, double duration = 0.2)
        {
            view.Alpha = 0.0f;
            view.Transform = CGAffineTransform.MakeScale((nfloat)0.5, (nfloat)0.5);

            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    view.Alpha = 1.0f;
                    view.Transform = CGAffineTransform.MakeScale((nfloat)1.0, (nfloat)1.0);
                },
                null
            );
        }
    }
}