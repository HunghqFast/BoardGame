using FastMobile.FXamarin.Core.FAndroid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Core")]
[assembly: ExportEffect(typeof(FPressedEffect), "FPressedEffect")]
namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FPressedEffect : PlatformEffect
    {
        private bool attached;

        public static void Initialize() { }

        public FPressedEffect() : base()
        {
        }

        protected override void OnAttached()
        {
            if (!attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick += ControlLongClick;
                    Control.Click += ControlClick;
                }
                else if (Container != null)
                {
                    Container.LongClickable = true;
                    Container.LongClick += ControlLongClick;
                    Container.Click += ControlClick;
                }
                attached = true;
            }
        }

        protected override void OnDetached()
        {
            if (attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.LongClick -= ControlLongClick;
                    Control.Click -= ControlClick;
                }
                else if (Container != null)
                {
                    Container.LongClickable = true;
                    Container.LongClick -= ControlLongClick;
                    Container.Click -= ControlClick;
                }
                attached = false;
            }
        }

        private void ControlClick(object sender, EventArgs e)
        {
            var command = Core.FPressedEffect.GetTapCommand(Element);
            command?.Execute(Core.FPressedEffect.GetTapParameter(Element));
        }

        private void ControlLongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            var command = Core.FPressedEffect.GetLongTapCommand(Element);
            command?.Execute(Core.FPressedEffect.GetLongParameter(Element));
        }
    }
}