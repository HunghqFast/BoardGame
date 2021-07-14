using FastMobile.FXamarin.Core.FiOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Core")]
[assembly: ExportEffect(typeof(FPressedEffect), "FPressedEffect")]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FPressedEffect : PlatformEffect
    {
        private bool attached;
        private readonly UILongPressGestureRecognizer longPressRecognizer;
        private readonly UITapGestureRecognizer tapRecognizer;

        public FPressedEffect() : base()
        {
            longPressRecognizer = new UILongPressGestureRecognizer(HandleLongClick);
            tapRecognizer = new UITapGestureRecognizer(HandleClick);
        }

        protected override void OnAttached()
        {
            if (!attached)
            {
                Container?.AddGestureRecognizer(longPressRecognizer);
                Container?.AddGestureRecognizer(tapRecognizer);
                attached = true;
            }
        }

        protected override void OnDetached()
        {
            if (attached)
            {
                Container?.RemoveGestureRecognizer(longPressRecognizer);
                Container?.RemoveGestureRecognizer(tapRecognizer);
                attached = false;
            }
        }

        private void HandleClick()
        {
            var command = Core.FPressedEffect.GetTapCommand(Element);
            command?.Execute(Core.FPressedEffect.GetTapParameter(Element));
        }

        private void HandleLongClick(UILongPressGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                var command = Core.FPressedEffect.GetLongTapCommand(Element);
                command?.Execute(Core.FPressedEffect.GetLongParameter(Element));
            }
        }
    }
}