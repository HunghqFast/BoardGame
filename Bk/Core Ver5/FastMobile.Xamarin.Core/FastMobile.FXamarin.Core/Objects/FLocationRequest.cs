using System;

namespace FastMobile.FXamarin.Core
{
    public partial class FLocationRequest
    {
        public FLocationRequest()
        {
            Timeout = TimeSpan.Zero;
            DesiredAccuracy = FLocationAccuracy.Default;
        }

        public FLocationRequest(FLocationAccuracy accuracy)
        {
            Timeout = TimeSpan.Zero;
            DesiredAccuracy = accuracy;
        }

        public FLocationRequest(FLocationAccuracy accuracy, TimeSpan timeout)
        {
            Timeout = timeout;
            DesiredAccuracy = accuracy;
        }

        public TimeSpan Timeout { get; set; }

        public FLocationAccuracy DesiredAccuracy { get; set; }

        public override string ToString() => $"{nameof(DesiredAccuracy)}: {DesiredAccuracy}, {nameof(Timeout)}: {Timeout}";
    }
}