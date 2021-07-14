using Syncfusion.XForms.Core;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FAlertEntry : FAlertBase
    {
        protected readonly FInputTextUnderline Text;
        public bool AllowNull { get; set; }

        public FAlertEntry() : base()
        {
            Text = new FInputTextUnderline();
            Base();
        }

        protected override void BeforeBase()
        {
            MessageRow.Height = GridLength.Auto;
            SubViewRow.Height = GridLength.Star;
        }

        private void Base()
        {
            Text.Margin = new Thickness(10, 0);
            Text.IsShowTitle = false;
            Text.Rendering();
            Text.TextChanged += OnTextChanged;
            MessageLabel.TextColor = FSetting.TextColorContent;
            MessageLabel.Padding = new Thickness(10, 0);
            MessageLabel.TextType = TextType.Text;
            SubView = Text;
            Alert.Closing += OnAlertClosing;
        }

        public async Task<(bool OK, string Message)> ShowTextbox(bool allowNull, bool isPassword, string message, int maxLength = 256)
        {
            Text.IsPassword = isPassword;
            Text.MaxLength = maxLength;
            AllowNull = allowNull;
            if (IsShowedOrCanotAlert())
                return (false, string.Empty);
            BeforeLoadConfirm();
            Load(false, "", message, FText.Accept, FText.Cancel);
            var result = await WaitConfirm();
            return (result, Text.Value);
        }

        public async Task<(bool, string)> ShowTextbox(bool allowNull, bool isPassword, string message, string acceptText, string cancelText, int maxLength = 256)
        {
            Text.IsPassword = isPassword;
            Text.MaxLength = maxLength;
            AllowNull = allowNull;
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return (false, string.Empty);
            BeforeLoadConfirm();
            Load(false, "", message, acceptText, cancelText);
            var result = await WaitConfirm();
            return (result, Text.Value);
        }

        public async Task<(bool, string)> ShowTextbox(bool allowNull, bool isPassword, string title, string message, string acceptText, string cancelText, int maxLength = 256)
        {
            Text.IsPassword = isPassword;
            Text.MaxLength = maxLength;
            AllowNull = allowNull;
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return (false, string.Empty);
            BeforeLoadConfirm();
            Load(false, title, message, acceptText, cancelText);
            var result = await WaitConfirm();
            return (result, Text.Value);
        }

        protected override string Config(string message)
        {
            return message;
        }

        private void OnAlertClosing(object sender, CancelEventArgs e)
        {
            if (!AllowNull && string.IsNullOrEmpty(Text.Value) && ResultConfirm)
            {
                e.Cancel = true;
                Bring();
                ResultConfirm = false;
            }
        }

        private async void Bring()
        {
            MessageLabel.TextColor = FSetting.DangerColor;
            await MessageLabel.TranslateTo(-20, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(20, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(-10, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(10, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(-5, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(5, 0, 20, Easing.Linear);
            await MessageLabel.TranslateTo(0, 0, 20, Easing.Linear);
        }

        private void OnFocusEntry(object sender, FocusEventArgs e)
        {
            MessageLabel.TextColor = FSetting.TextColorContent;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0) MessageLabel.TextColor = FSetting.TextColorContent;
        }
    }
}