using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public class FSpinWheelEventArgs
    {
        public SpinActionAsync SpinAsync { get; }

        public FSpinWheelEventArgs(SpinActionAsync action)
        {
            SpinAsync = action;
        }

        public delegate Task<int> SpinActionAsync(Func<Task<(bool OK, string Result)>> Action);
    }
}
