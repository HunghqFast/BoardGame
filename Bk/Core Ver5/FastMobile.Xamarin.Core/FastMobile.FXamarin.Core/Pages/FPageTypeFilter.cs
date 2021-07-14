using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeFilter : FPageTypeReport
    {
        public FPageTypeFilter(string controller) : base(controller, false, null)
        {
        }

        protected override async Task FilterSuccessed()
        {
            await Filter.SetBusy(true);
            Grid.DataFilter = Filter.FDataDirForm();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(new FMessage(0, 403, string.Empty), FChannel.ALERT_BY_MESSAGE);
                await Filter.SetBusy(false);
                return;
            }
            await Grid.LoadingGrid(FTargetType.Filter);
            IsFirst = false;
            await Filter.SetBusy(false);
        }

        protected override async Task FilterCanceled()
        {
            await Navigation.PopToRootAsync(true);
        }

        protected override async Task FilterBackButton()
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}