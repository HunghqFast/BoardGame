using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FActionSheetArguments
    {
        public FActionSheetArguments(string title, string cancel, string destruction, IEnumerable<string> buttons)
        {
            Title = title;
            Cancel = cancel;
            Destruction = destruction;
            Buttons = buttons?.Where(c => c != null);
            Result = new TaskCompletionSource<string>();
        }

        public IEnumerable<string> Buttons { get; private set; }

        public string Cancel { get; private set; }

        public string Destruction { get; private set; }

        public TaskCompletionSource<string> Result { get; }

        public string Title { get; private set; }

        public void SetResult(string result)
        {
            Result.TrySetResult(result);
        }
    }
}