using System.Data;

namespace FastMobile.FXamarin.Core
{
    public class FPageTypeApproval : FPageTypeDefault
    {
        public FPageTypeApproval(string controller, DataTable resTable) : base(controller, true, resTable)
        {
            Target = "110";
        }

        public override void SetView()
        {
            IsVisibleSubTitleView = false;
            base.SetView();
        }
    }
}