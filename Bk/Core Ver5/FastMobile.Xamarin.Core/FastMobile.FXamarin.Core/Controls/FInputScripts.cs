using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public class FInputScripts : FInputText
    {
        public FInputScripts() : base()
        {
        }

        public FInputScripts(FField field) : base(field)
        {
        }

        protected override void SetInput(List<string> value, bool isCompleted = false, bool isDisable = false)
        {
            if (Disable && isDisable) return;
            Value = string.Join(Seperate, value);
            if (isCompleted) OnCompleteValue(this, new FInputChangeValueEventArgs(Value));
        }
    }
}