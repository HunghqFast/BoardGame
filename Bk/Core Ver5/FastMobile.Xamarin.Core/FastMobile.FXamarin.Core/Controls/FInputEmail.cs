using System.Collections.Generic;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputEmail : FInputText
    {
        public FInputEmail() : base()
        {
        }

        public FInputEmail(FField field) : base(field)
        {
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = value[0].TrimEnd(' ').Replace(";", ",");
            base.SetInput(value, isCompleted);
        }

        protected override void OnUnfocused(object sender, FocusEventArgs e)
        {
            if (Value.Contains(";")) Value = Value.Replace(";", ",");
            base.OnUnfocused(sender, e);
        }
    }
}