using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeReport : FPageReport
    {
        public FPageTypeReport(string controller, bool isDir, DataTable resTable) : base(controller, resTable: resTable)
        {
            Target = isDir ? "111" : "110";
        }

        protected override async Task Run(Page parent, bool openDetail = false)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(new FMessage(0, 403, ""), FChannel.ALERT_BY_MESSAGE);
                parent.IsBusy = false;
                return;
            }
            if (await InitSetting())
            {
                await FilterCustom(parent);
                if (Filter.Success) parent.Navigation.InsertPageBefore(this, Filter);
                if (openDetail) Grid.OpenDetail(DetailData);
            }
        }

        protected override async Task FilterCanceled()
        {
            await BackEvent();
        }

        protected override async Task FilterBackButton()
        {
            await BackEvent();
        }

        private async Task BackEvent()
        {
            if (IsFirst) await Navigation.PopToRootAsync(true);
            else await Navigation.PopAsync(true);
        }
    }
}