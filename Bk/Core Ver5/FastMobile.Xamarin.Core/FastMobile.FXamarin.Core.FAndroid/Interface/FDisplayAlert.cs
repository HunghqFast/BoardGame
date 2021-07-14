using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AppCompatActivity = global::AndroidX.AppCompat.App.AppCompatActivity;
using AppCompatAlertDialog = global::AndroidX.AppCompat.App.AlertDialog;
using String = Java.Lang.String;

namespace FastMobile.FXamarin.Core.FAndroid
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
            OnAlertRequested(args);
            return args.Result.Task;
        }

        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            var args = new FActionSheetArguments(title, cancel, destruction, buttons);
            OnActionSheetRequested(args);
            return args.Result.Task;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder, int maxLength, Keyboard keyboard)
        {
            return DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, "");
        }

        [Obsolete]
        public Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            var args = new FPromptArguments(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            OnPromptRequested(args);
            return args.Result.Task;
        }

        #region Helper

        private void OnAlertRequested(FAlertArguments arguments)
        {
            if (FAlertArguments.IsShowing)
                return;
            var alert = new DialogBuilder(Xamarin.Essentials.Platform.CurrentActivity).Create();
            alert.SetTitle(arguments.Title);
            alert.SetMessage(arguments.Message);
            if (arguments.Accept != null)
                alert.SetButton((int)DialogButtonType.Negative, arguments.Accept, (o, args) => { arguments.SetResult(true); arguments.SetShowing(false); });
            alert.SetButton((int)DialogButtonType.Positive, arguments.Cancel, (o, args) => { arguments.SetResult(false); arguments.SetShowing(false); });
            alert.SetCancelEvent((o, args) => { arguments.SetResult(false); arguments.SetShowing(false); });
            alert.Show();
            arguments.SetShowing(true);
        }

        private void OnActionSheetRequested(FActionSheetArguments arguments)
        {
            var builder = new DialogBuilder(Xamarin.Essentials.Platform.CurrentActivity);

            builder.SetTitle(arguments.Title);
            string[] items = arguments.Buttons.ToArray();
            builder.SetItems(items, (o, args) => arguments.Result.TrySetResult(items[args.Which]));

            if (arguments.Cancel != null)
                builder.SetPositiveButton(arguments.Cancel, (o, args) => arguments.Result.TrySetResult(arguments.Cancel));

            if (arguments.Destruction != null)
                builder.SetNegativeButton(arguments.Destruction, (o, args) => arguments.Result.TrySetResult(arguments.Destruction));

            var dialog = builder.Create();
            builder.Dispose();
            dialog.SetCanceledOnTouchOutside(true);
            dialog.SetCancelEvent((o, e) => arguments.SetResult(null));
            dialog.Show();
        }

        [Obsolete]
        private void OnPromptRequested(FPromptArguments arguments)
        {
            var alertDialog = new DialogBuilder(Xamarin.Essentials.Platform.CurrentActivity).Create();
            alertDialog.SetTitle(arguments.Title);
            alertDialog.SetMessage(arguments.Message);

            var frameLayout = new FrameLayout(Xamarin.Essentials.Platform.CurrentActivity);
            var editText = new EditText(Xamarin.Essentials.Platform.CurrentActivity) { Hint = arguments.Placeholder, Text = arguments.InitialValue };
            var layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = (int)(22 * Xamarin.Essentials.Platform.CurrentActivity.Resources.DisplayMetrics.Density),
                RightMargin = (int)(22 * Xamarin.Essentials.Platform.CurrentActivity.Resources.DisplayMetrics.Density)
            };

            editText.LayoutParameters = layoutParams;
            editText.InputType = arguments.Keyboard.ToInputType();
            if (arguments.Keyboard == Keyboard.Numeric)
                editText.KeyListener = LocalizedDigitsKeyListener.Create(editText.InputType);

            if (arguments.MaxLength > -1)
                editText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(arguments.MaxLength) });

            frameLayout.AddView(editText);
            alertDialog.SetView(frameLayout);

            alertDialog.SetButton((int)DialogButtonType.Positive, arguments.Accept, (o, args) => arguments.SetResult(editText.Text));
            alertDialog.SetButton((int)DialogButtonType.Negative, arguments.Cancel, (o, args) => arguments.SetResult(null));
            alertDialog.SetCancelEvent((o, args) => { arguments.SetResult(null); });

            alertDialog.Window.SetSoftInputMode(SoftInput.StateVisible);
            alertDialog.Show();
            editText.RequestFocus();
        }

        #endregion Helper
    }

    internal sealed class DialogBuilder
    {
        private AppCompatAlertDialog.Builder appcompatBuilder;
        private AlertDialog.Builder legacyBuilder;
        private bool useAppCompat;

        public DialogBuilder(Activity activity)
        {
            if (activity is AppCompatActivity)
            {
                appcompatBuilder = new AppCompatAlertDialog.Builder(activity);
                useAppCompat = true;
            }
            else
            {
                legacyBuilder = new AlertDialog.Builder(activity);
            }
        }

        public void SetTitle(string title)
        {
            if (useAppCompat)
            {
                appcompatBuilder.SetTitle(title);
            }
            else
            {
                legacyBuilder.SetTitle(title);
            }
        }

        public void SetItems(string[] items, EventHandler<DialogClickEventArgs> handler)
        {
            if (useAppCompat)
            {
                appcompatBuilder.SetItems(items, handler);
            }
            else
            {
                legacyBuilder.SetItems(items, handler);
            }
        }

        public void SetPositiveButton(string text, EventHandler<DialogClickEventArgs> handler)
        {
            if (useAppCompat)
            {
                appcompatBuilder.SetPositiveButton(text, handler);
            }
            else
            {
                legacyBuilder.SetPositiveButton(text, handler);
            }
        }

        public void SetNegativeButton(string text, EventHandler<DialogClickEventArgs> handler)
        {
            if (useAppCompat)
            {
                appcompatBuilder.SetNegativeButton(text, handler);
            }
            else
            {
                legacyBuilder.SetNegativeButton(text, handler);
            }
        }

        public FlexibleAlertDialog Create()
        {
            if (useAppCompat)
            {
                return new FlexibleAlertDialog(appcompatBuilder.Create());
            }

            return new FlexibleAlertDialog(legacyBuilder.Create());
        }

        public void Dispose()
        {
            if (useAppCompat)
            {
                appcompatBuilder.Dispose();
            }
            else
            {
                legacyBuilder.Dispose();
            }
        }
    }

    internal sealed class FlexibleAlertDialog
    {
        private readonly AppCompatAlertDialog appcompatAlertDialog;
        private readonly AlertDialog legacyAlertDialog;
        private bool useAppCompat;

        public FlexibleAlertDialog(AlertDialog alertDialog)
        {
            legacyAlertDialog = alertDialog;
        }

        public FlexibleAlertDialog(AppCompatAlertDialog alertDialog)
        {
            appcompatAlertDialog = alertDialog;
            useAppCompat = true;
        }

        public void SetTitle(string title)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.SetTitle(title);
            }
            else
            {
                legacyAlertDialog.SetTitle(title);
            }
        }

        public void SetMessage(string message)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.SetMessage(message);
            }
            else
            {
                legacyAlertDialog.SetMessage(message);
            }
        }

        public void SetButton(int whichButton, string text, EventHandler<DialogClickEventArgs> handler)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.SetButton(whichButton, text, handler);
            }
            else
            {
                legacyAlertDialog.SetButton(whichButton, text, handler);
            }
        }

        public void SetCancelEvent(EventHandler cancel)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.CancelEvent += cancel;
            }
            else
            {
                legacyAlertDialog.CancelEvent += cancel;
            }
        }

        public void SetCanceledOnTouchOutside(bool canceledOnTouchOutSide)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.SetCanceledOnTouchOutside(canceledOnTouchOutSide);
            }
            else
            {
                legacyAlertDialog.SetCanceledOnTouchOutside(canceledOnTouchOutSide);
            }
        }

        public void SetView(global::Android.Views.View view)
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.SetView(view);
            }
            else
            {
                legacyAlertDialog.SetView(view);
            }
        }

        public Window Window => useAppCompat ? appcompatAlertDialog.Window : legacyAlertDialog.Window;

        public void Show()
        {
            if (useAppCompat)
            {
                appcompatAlertDialog.Show();
            }
            else
            {
                legacyAlertDialog.Show();
            }
        }
    }

    internal class LocalizedDigitsKeyListener : NumberKeyListener
    {
        private readonly char decimalSeparator;
        private const char SignCharacter = '-';
        private static Dictionary<char, LocalizedDigitsKeyListener> sunsignedCache;
        private static Dictionary<char, LocalizedDigitsKeyListener> ssignedCache;

        private static char GetDecimalSeparator()
        {
            var format = NumberFormat.Instance as DecimalFormat;
            if (format == null)
            {
                return '.';
            }

            DecimalFormatSymbols sym = format.DecimalFormatSymbols;
            return sym.DecimalSeparator;
        }

        [Obsolete]
        public static NumberKeyListener Create(InputTypes inputTypes)
        {
            if ((inputTypes & InputTypes.NumberFlagDecimal) == 0)
            {
                return DigitsKeyListener.GetInstance(inputTypes.HasFlag(InputTypes.NumberFlagSigned), false);
            }

            char decimalSeparator = GetDecimalSeparator();
            if (decimalSeparator == '.')
            {
                return DigitsKeyListener.GetInstance(inputTypes.HasFlag(InputTypes.NumberFlagSigned), true);
            }
            return GetInstance(inputTypes, decimalSeparator);
        }

        public static LocalizedDigitsKeyListener GetInstance(InputTypes inputTypes, char decimalSeparator)
        {
            if ((inputTypes & InputTypes.NumberFlagSigned) != 0)
            {
                return GetInstance(inputTypes, decimalSeparator, ref ssignedCache);
            }
            return GetInstance(inputTypes, decimalSeparator, ref sunsignedCache);
        }

        private static LocalizedDigitsKeyListener GetInstance(InputTypes inputTypes, char decimalSeparator, ref Dictionary<char, LocalizedDigitsKeyListener> cache)
        {
            if (cache == null)
            {
                cache = new Dictionary<char, LocalizedDigitsKeyListener>(1);
            }
            if (!cache.ContainsKey(decimalSeparator))
            {
                cache.Add(decimalSeparator, new LocalizedDigitsKeyListener(inputTypes, decimalSeparator));
            }
            return cache[decimalSeparator];
        }

        protected LocalizedDigitsKeyListener(InputTypes inputTypes, char decimalSeparatorr)
        {
            decimalSeparator = decimalSeparatorr;
            InputType = inputTypes;
        }

        public override InputTypes InputType { get; }

        private char[] acceptedChars;

        protected override char[] GetAcceptedChars()
        {
            if ((InputType & InputTypes.NumberFlagSigned) == 0)
            {
                return acceptedChars ?? (acceptedChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', decimalSeparator });
            }
            return acceptedChars ?? (acceptedChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', SignCharacter, decimalSeparator });
        }

        private static bool IsSignChar(char c)
        {
            return c == SignCharacter;
        }

        private bool IsDecimalPointChar(char c)
        {
            return c == decimalSeparator;
        }

        public override ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            ICharSequence filterFormatted = base.FilterFormatted(source, start, end, dest, dstart, dend);

            if (filterFormatted != null)
            {
                source = filterFormatted;
                start = 0;
                end = filterFormatted.Length();
            }

            int sign = -1;
            int dec = -1;
            int dlen = dest.Length();

            for (var i = 0; i < dstart; i++)
            {
                char c = dest.CharAt(i);
                if (IsSignChar(c))
                {
                    sign = i;
                }
                else if (IsDecimalPointChar(c))
                {
                    dec = i;
                }
            }
            for (int i = dend; i < dlen; i++)
            {
                char c = dest.CharAt(i);
                if (IsSignChar(c))
                {
                    return new String("");
                }

                if (IsDecimalPointChar(c))
                {
                    dec = i;
                }
            }

            SpannableStringBuilder stripped = null;
            for (int i = end - 1; i >= start; i--)
            {
                char c = source.CharAt(i);
                var strip = false;

                if (IsSignChar(c))
                {
                    if (i != start || dstart != 0)
                    {
                        strip = true;
                    }
                    else if (sign >= 0)
                    {
                        strip = true;
                    }
                    else
                    {
                        sign = i;
                    }
                }
                else if (IsDecimalPointChar(c))
                {
                    if (dec >= 0)
                    {
                        strip = true;
                    }
                    else
                    {
                        dec = i;
                    }
                }

                if (strip)
                {
                    if (end == start + 1)
                    {
                        return new String("");
                    }
                    if (stripped == null)
                    {
                        stripped = new SpannableStringBuilder(source, start, end);
                    }
                    stripped.Delete(i - start, i + 1 - start);
                }
            }
            return stripped ?? filterFormatted;
        }
    }
}