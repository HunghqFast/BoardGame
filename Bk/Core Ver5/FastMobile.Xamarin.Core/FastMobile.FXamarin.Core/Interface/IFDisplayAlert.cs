using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFDisplayAlert
    {
        Task DisplayAlert(string title, string message, string acceptText);

        Task<bool> DisplayConfirm(string title, string message, string acceptText, string cancelText);

        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);

        [EditorBrowsable(EditorBrowsableState.Never)]
        Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder, int maxLength, Keyboard keyboard);

        Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel, string placeholder = null, int maxLength = -1, Keyboard keyboard = default(Keyboard), string initialValue = "");
    }
}