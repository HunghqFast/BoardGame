namespace FastMobile.FXamarin.Core
{
    public class FMessageToast : FMessage
    {
        public int Milisecond { get; set; }

        public FMessageToast() : base()
        {
            Milisecond = 2000;
        }

        public FMessageToast(int success, int code, string message, int miliseconds) : base(success, code, message)
        {
            Milisecond = miliseconds;
        }
    }
}