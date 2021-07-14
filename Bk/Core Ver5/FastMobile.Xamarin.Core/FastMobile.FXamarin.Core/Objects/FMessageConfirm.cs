using System;

namespace FastMobile.FXamarin.Core
{
    public class FMessageConfirm : FMessage
    {
        public Action<bool> Completed { get; set; }

        public FMessageConfirm(Action<bool> completed)
        {
            Completed = completed;
        }

        public FMessageConfirm(Action<bool> completed, string message) : base(message)
        {
            Completed = completed;
        }

        public FMessageConfirm(Action<bool> completed, int success, int code, string message) : base(success, code, message)
        {
            Completed = completed;
        }
    }
}