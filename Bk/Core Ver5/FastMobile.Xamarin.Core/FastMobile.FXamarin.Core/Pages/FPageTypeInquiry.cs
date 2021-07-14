using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeInquiry : FPageReport
    {
        public FPageTypeInquiry(string controller, FPageReport before, DataSet data, FViewPage grid, FTargetType target) : base(controller, false, before, data, grid, target)
        {
            Target = "010";
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
            }
            IsLoading = false;
        }

        public override void SetView()
        {
            if (Grid.GridType == GridType.WebView)
            {
                IsVisibleSubTitleView = false;
                IsVisibleLoadmoreTitleView = false;
                IsVisibleTitleTableView = false;
                IsVisiblePagingTitleView = false;
                return;
            }
            base.SetView();
        }
    }
}