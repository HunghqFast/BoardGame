using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageNotify : FPage, IFBadge
    {
        public string BadgeValue
        {
            get => FTabBadge.GetBadgeText(this);
            set
            {
                FTabBadge.SetBadgeText(this, string.IsNullOrWhiteSpace(value) || value.Trim() == "0" ? "" : value);
                if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0") HideReader();
                else ShowReader();
            }
        }

        public event EventHandler<EventArgs> ReadAllClicked;

        public event EventHandler<FBadgeMenuArgs> ReceivedBadgeForMenu;

        private readonly ToolbarItem Reader;

        public FPageNotify(bool isHasPullToRefresh, bool enableScrolling = false) : base(isHasPullToRefresh, enableScrolling)
        {
            Reader = new ToolbarItem();
            InitToolbar();
            FTabBadge.SetBadgeTextColor(this, Color.White);
            FTabBadge.SetBadgeColor(this, FSetting.DangerColor);
            //FTabBadge.SetBadgeFont(this, Font.SystemFontOfSize(FSetting.FontSizeBadge * 3));
        }

        public void Clear()
        {
            FTabBadge.SetBadgeText(this, "");
        }

        public void OnReceived(string badge, string controller)
        {
            ReceivedBadgeForMenu?.Invoke(this, new FBadgeMenuArgs(badge, controller));
        }

        protected virtual void OnReadAllClicked(object sender, EventArgs e)
        {
            ReadAllClicked?.Invoke(sender, e);
        }

        private void InitToolbar()
        {
            Reader.IconImageSource = FIcons.CheckAll.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconToolbar);
            Reader.Clicked += OnReadAllClicked;
        }

        private void ShowReader()
        {
            ToolbarItems.Clear();
            ToolbarItems.Add(Reader);
        }

        private void HideReader()
        {
            ToolbarItems.Clear();
        }
    }
}