using Foundation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;
using RectangleF = CoreGraphics.CGRect;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FDisplayAlert : IFDisplayAlert
    {
        public Task DisplayAlert(string title, string message, string acceptText)
        {
            return DisplayConfirm(title, message, acceptText, null);
        }

        public Task<bool> DisplayConfirm(string title, string message, string acceptText, string cancelText)
        {
            if (string.IsNullOrEmpty(acceptText))
                return Task.FromResult(false);
            var args = new FAlertArguments(title, message, acceptText, cancelText);
            PresentAlert(args);
            return args.Result.Task;
        }

        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            var args = new FActionSheetArguments(title, cancel, destruction, buttons);
            PresentActionSheet(args);
            return args.Result.Task;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder, int maxLength, Keyboard keyboard)
        {
            return DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, "");
        }

        public Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder = null, int maxLength = -1, Keyboard keyboard = default(Keyboard), string initialValue = "")
        {
            var args = new FPromptArguments(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            PresentPrompt(args);
            return args.Result.Task;
        }

        private void PresentAlert(FAlertArguments arguments)
        {
            if (FAlertArguments.IsShowing)
                return;
            var window = new UIWindow { BackgroundColor = ToUIColor(Color.Transparent) };
            var alert = UIAlertController.Create(arguments.Title, arguments.Message, UIAlertControllerStyle.Alert);
            var oldFrame = alert.View.Frame;
            alert.View.Frame = new RectangleF(oldFrame.X, oldFrame.Y, oldFrame.Width, oldFrame.Height - 20);
            if (arguments.Accept != null)
            {
                alert.AddAction(CreateActionWithWindowHide(arguments.Accept, UIAlertActionStyle.Cancel, () => { arguments.SetResult(true); arguments.SetShowing(false); }, window));
            }

            if (arguments.Cancel != null)
            {
                alert.AddAction(CreateActionWithWindowHide(arguments.Cancel, UIAlertActionStyle.Default, () => { arguments.SetResult(false); arguments.SetShowing(false); }, window));
            }
            PresentPopUp(window, alert);
            arguments.SetShowing(true);
        }

        private void PresentActionSheet(FActionSheetArguments arguments)
        {
            var alert = UIAlertController.Create(arguments.Title, null, UIAlertControllerStyle.ActionSheet);
            var window = new UIWindow { BackgroundColor = ToUIColor(Color.Transparent) };
            if (arguments.Cancel != null || UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                alert.AddAction(CreateActionWithWindowHide(arguments.Cancel ?? "", UIAlertActionStyle.Cancel, () => arguments.SetResult(arguments.Cancel), window));
            }
            if (arguments.Destruction != null)
            {
                alert.AddAction(CreateActionWithWindowHide(arguments.Destruction, UIAlertActionStyle.Destructive, () => arguments.SetResult(arguments.Destruction), window));
            }
            foreach (var label in arguments.Buttons)
            {
                if (label == null)
                    continue;
                var blabel = label;
                alert.AddAction(CreateActionWithWindowHide(blabel, UIAlertActionStyle.Default, () => arguments.SetResult(blabel), window));
            }
            PresentPopUp(window, alert, arguments);
        }

        private void PresentPrompt(FPromptArguments arguments)
        {
            var window = new UIWindow { BackgroundColor = ToUIColor(Color.Transparent) };
            var alert = UIAlertController.Create(arguments.Title, arguments.Message, UIAlertControllerStyle.Alert);
            alert.AddTextField(uiTextField =>
            {
                uiTextField.Placeholder = arguments.Placeholder;
                uiTextField.Text = arguments.InitialValue;
                uiTextField.ShouldChangeCharacters = (field, range, replacementString) => arguments.MaxLength <= -1 || field.Text.Length + replacementString.Length - range.Length <= arguments.MaxLength;
                uiTextField.ApplyKeyboard(arguments.Keyboard);
            });
            var oldFrame = alert.View.Frame;
            alert.View.Frame = new RectangleF(oldFrame.X, oldFrame.Y, oldFrame.Width, oldFrame.Height - 20);
            alert.AddAction(CreateActionWithWindowHide(arguments.Cancel, UIAlertActionStyle.Cancel, () => arguments.SetResult(null), window));
            alert.AddAction(CreateActionWithWindowHide(arguments.Accept, UIAlertActionStyle.Default, () => arguments.SetResult(alert.TextFields[0].Text), window));
            PresentPopUp(window, alert);
        }

        private UIAlertAction CreateActionWithWindowHide(string text, UIAlertActionStyle style, Action setResult, UIWindow window)
        {
            return UIAlertAction.Create(text, style, a => { window.Hidden = true; setResult(); });
        }

        private void PresentPopUp(UIWindow window, UIAlertController alert, FActionSheetArguments arguments = null)
        {
            window.RootViewController = new UIViewController();
            window.RootViewController.View.BackgroundColor = ToUIColor(Color.Transparent);
            window.WindowLevel = UIWindowLevel.Alert + 1;
            window.MakeKeyAndVisible();

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && arguments != null)
            {
                UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
                var observer = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification,
                    n => { alert.PopoverPresentationController.SourceRect = window.RootViewController.View.Bounds; });

                arguments.Result.Task.ContinueWith(t =>
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
                    UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
                }, TaskScheduler.FromCurrentSynchronizationContext());

                alert.PopoverPresentationController.SourceView = window.RootViewController.View;
                alert.PopoverPresentationController.SourceRect = window.RootViewController.View.Bounds;
                alert.PopoverPresentationController.PermittedArrowDirections = 0;
            }
            if (!UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                window.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            }
            window.RootViewController.PresentViewController(alert, true, null);
        }

        private UIColor ToUIColor(Xamarin.Forms.Color color)
        {
            return new UIColor((float)color.R, (float)color.G, (float)color.B, (float)color.A);
        }
    }
}