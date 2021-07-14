using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FModelBase : BindableObject, IFBusy
    {
        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create("IsBusy", typeof(bool), typeof(FModelBase), false);

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public async Task SetBusy(bool value)
        {
            IsBusy = value;
            await Task.Delay(1);
        }
    }
}