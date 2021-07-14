namespace FastMobile.FXamarin.Core
{
    public class FInputPassword : FInputText
    {
        public FInputPassword() : base()
        {
        }

        public FInputPassword(FField field) : base(field)
        {
        }

        protected override void InitPropertyByField(FField f)
        {
            IsPassword = true;
            base.InitPropertyByField(f);
        }
    }
}