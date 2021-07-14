using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeDefault : FPageReport
    {
        public FPageTypeDefault(string controller, bool isFilter, DataTable resTable) : base(controller, isFilter, resTable: resTable)
        {
            Target = isFilter ? "111" : "011";
        }

        protected override async Task Run(Page parent, bool openDetail = false)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(new FMessage(0, 403, ""), FChannel.ALERT_BY_MESSAGE);
                parent.IsBusy = false;
                return;
            }
            IsLoading = true;
            if (await InitSetting())
            {
                await parent.Navigation.PushAsync(this, true);
                await Grid.LoadingGrid(FTargetType.Grid);
                await UpdateMasterView();
                await UpdateFooterView();
                await Task.Delay(25);
                if (openDetail) Grid.OpenDetail(DetailData);
            }
            IsLoading = false;
        }
    }
}