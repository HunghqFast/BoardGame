namespace FastMobile.FXamarin.Core
{
    public class FMessage
    {
        public int Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public bool OK => Success == 1;

        public FMessage()
        {
            Success = 0;
            Code = 100;
            Message = "";
        }

        public FMessage(string message)
        {
            Success = 0;
            Code = 100;
            Message = message;
        }

        public FMessage(int success, int code, string message)
        {
            Success = success;
            Code = code;
            Message = message;
        }

        public static FMessage FromSuccess() => new(1, 0, "");

        public static FMessage FromSuccess(int code) => new(1, code, "");

        public static FMessage FromSuccess(int code, string message) => new(1, code, message);

        public static FMessage FromFail(int code) => new(0, code, "");

        public static FMessage FromFail(int code, string message) => new(0, code, message);
    }
}