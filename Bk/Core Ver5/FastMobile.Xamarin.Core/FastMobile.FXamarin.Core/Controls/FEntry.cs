using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FEntry : SfTextInputLayout
    {
        public new Entry InputView
        {
            get => base.InputView as Entry;
            set => base.InputView = value;
        }

        public FEntry()
        {
            InputView = new Entry();
            InputView.Text = string.Empty;
            InputView.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;
            InputView.Keyboard = Keyboard.Text;
            InputView.FontFamily = FSetting.FontText;
            InputView.TextColor = FSetting.TextColorContent;
            InputView.IsTextPredictionEnabled = false;
            InputView.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeNone);
            InputView.HeightRequest = FSetting.IsAndroid ? 50 : InputView.HeightRequest;
            InputView.FontSize = FSetting.FontSizeLabelTitle + 2;

            ShowHint = true;
            FocusedColor = FSetting.PrimaryColor;
            UnfocusedColor = FSetting.DisableColor;
            HintLabelStyle.FontFamily = FSetting.FontText;
            HintLabelStyle.FontSize = FSetting.FontSizeLabelHint + 4;
            IsHintAlwaysFloated = true;
            EnableHintAnimation = false;
            ContainerType = ContainerType.None;
            OutlineCornerRadius = 8;
            FocusedStrokeWidth = UnfocusedStrokeWidth = 1;
        }
    }
}