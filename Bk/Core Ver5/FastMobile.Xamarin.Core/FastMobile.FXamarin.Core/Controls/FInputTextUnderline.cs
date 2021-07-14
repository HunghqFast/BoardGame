using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputTextUnderline : FInputTextBase
    {
        public ClearButtonVisibility ClearButtonVisibility { get; set; }

        public FInputTextUnderline() : base()
        {
            ClearButtonVisibility = ClearButtonVisibility.Never;
        }

        public FInputTextUnderline(FField field) : base(field)
        {
            ClearButtonVisibility = ClearButtonVisibility.Never;
        }

        protected override View SetContentView()
        {
            var t = new FInputLayoutDashed();
            var e = base.SetContentView() as FEntryBase;

            e.ClearButtonVisibility = ClearButtonVisibility;

            t.InputViewPadding = new Thickness(0, (FSetting.FilterInputHeight - FSetting.FontSizeLabelContent) * 0.5 - 2, 0, 1);
            t.HeightRequest = FSetting.FilterInputHeight;
            t.ContainerBackgroundColor = Color.Transparent;
            t.InputView = e;

            return t;
        }
    }
}