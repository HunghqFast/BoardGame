using Syncfusion.XForms.TabView;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageNotifyTab : FPageNotify
    {
        public readonly SfTabView TabView;

        public FPageNotifyTab(bool hasPull, bool hasScroll = false) : base(hasPull, hasScroll)
        {
            TabView = new SfTabView();
            TabView.SelectionIndicatorSettings.AnimationDuration = 500;
            TabView.EnableSwiping = false;
            TabView.OverflowMode = OverflowMode.Scroll;
            TabView.HorizontalOptions = TabView.VerticalOptions = LayoutOptions.Fill;
        }

        public override async Task<bool> ScrollToTop()
        {
            if (IsTop)
                return false;
            IsTop = true;
            if (TabView.Items != null && TabView.Items.Count > 0) await (TabView.Items[TabView.SelectedIndex].Content as IFScroll)?.ScrollToTop();
            IsTop = false;
            return true;
        }
    }
}