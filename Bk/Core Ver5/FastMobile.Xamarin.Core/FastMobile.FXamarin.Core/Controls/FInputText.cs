using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputText : FInputTextBase
    {
        public FInputText() : base()
        {
        }

        public FInputText(FField field) : base(field)
        {
        }

        protected override View SetContentView()
        {
            E.HeightRequest = FSetting.FilterInputHeight;
            return base.SetContentView();
        }
    }
}