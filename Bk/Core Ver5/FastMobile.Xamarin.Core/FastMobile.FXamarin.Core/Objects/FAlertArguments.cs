using System.ComponentModel;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FAlertArguments
    {
        public static bool IsShowing { get; private set; }

        public FAlertArguments(string title, string message, string accept, string cancel)
        {
            Title = title;
            Message = message;
            Accept = accept;
            Cancel = cancel;
            Result = new TaskCompletionSource<bool>();
        }

        public string Accept { get; private set; }

        public string Cancel { get; private set; }

        public string Message { get; private set; }

        public TaskCompletionSource<bool> Result { get; }

        public string Title { get; private set; }

        public void SetResult(bool result)
        {
            Result.TrySetResult(result);
        }

        public void SetShowing(bool value)
        {
            IsShowing = value;
        }
    }
}