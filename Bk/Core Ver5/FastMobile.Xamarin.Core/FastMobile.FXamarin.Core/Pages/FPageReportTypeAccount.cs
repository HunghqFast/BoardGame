namespace FastMobile.FXamarin.Core
{
    public class FPageReportTypeAccount : FPageTypeDefault
    {
        public FPageReportTypeAccount(string controller) : base(controller, false, null)
        {
            Target = "010";
        }

        public override void SetView()
        {
            IsVisibleSubTitleView = false;
            IsVisibleLoadmoreTitleView = false;
            IsVisiblePagingTitleView = false;
            base.SetView();
        }
    }
}