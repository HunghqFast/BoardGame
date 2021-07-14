using FastMobile.Core;
using FastMobile.FXamarin.Core;
using Syncfusion.XForms.TabView;

namespace FastMobile.Device
{
    public class PageNotifyGroup : CPageNotifyGroup
    {
        public const string SystemGroup = "S";
        public const string NewsGroup = "N";
        public const string PromotionGroup = "P";
        public readonly TabContent System, News, Promotion;
        private readonly SfTabItem SystemItem, NewsItem, PromotionItem;

        public PageNotifyGroup() : base()
        {
            System = new TabContent(this);
            News = new TabContent(this);
            Promotion = new TabContent(this);
            SystemItem = new SfTabItem();
            NewsItem = new SfTabItem();
            PromotionItem = new SfTabItem();
            System.InitModelProperty("GetNotify", "System", FChannel.NOTIFYRECEIVED + SystemGroup, SystemGroup, 20, 0, FWebViewType.Default, this);
            News.InitModelProperty("GetNotify", "System", FChannel.NOTIFYRECEIVED + NewsGroup, NewsGroup, 20, 0, FWebViewType.Default, this);
            Promotion.InitModelProperty("GetNotify", "System", FChannel.NOTIFYRECEIVED + PromotionGroup, PromotionGroup, 20, 0, FWebViewType.Default, this);

            InitEvent(System, News, Promotion);
        }

        public override async void Init()
        {
            base.Init();
            if (!HasNetwork)
                return;
            IsBusy = true;
            await FServices.ForAllAsync(x => x.Init(), System, News, Promotion);
            InitLayout();
            IsBusy = false;
        }

        private void InitLayout()
        {
            SystemItem.TitleFontFamily = NewsItem.TitleFontFamily = PromotionItem.TitleFontFamily = FSetting.FontText;
            SystemItem.TitleFontSize = NewsItem.TitleFontSize = PromotionItem.TitleFontSize = FSetting.FontSizeLabelContent;

            SystemItem.Title = FText.System;
            SystemItem.Content = System;

            NewsItem.Title = FText.News;
            NewsItem.Content = News;

            PromotionItem.Title = FText.Promotion;
            PromotionItem.Content = Promotion;

            TabView.Items = new TabItemCollection { SystemItem, NewsItem, PromotionItem };
            TabView.VisibleHeaderCount = TabView.Items.Count;
            Content = TabView;
        }
    }
}