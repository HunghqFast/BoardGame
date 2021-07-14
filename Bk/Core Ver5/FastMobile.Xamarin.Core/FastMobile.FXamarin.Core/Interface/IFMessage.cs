namespace FastMobile.FXamarin.Core
{
    public interface IFMessage
    {
        public bool OK { get; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}