using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageAbout : FPage, IFDataHandler
    {
        public event EventHandler<IFDataEvent> ItemTapped;

        public readonly ObservableCollection<FItemAbout> Items;
        public readonly FLabel Version;
        public readonly FLabel Copyright;

        protected Action<IFDataEvent> InvokeWhenTapped { get; set; }

        private readonly SfListView C;
        private readonly Grid AboutView;

        private DateTime LastTabbed;

        public FPageAbout(bool isHasPullToRefresh) : base(isHasPullToRefresh, false)
        {
            LastTabbed = DateTime.MinValue;
            Items = new ObservableCollection<FItemAbout>();
            C = new SfListView();
            AboutView = new Grid();
            Version = new FLabel();
            Copyright = new FLabel();
            BackgroundColor = FSetting.BackgroundSpacing;
        }

        public override void Init()
        {
            base.Init();
            InitHeader();
            InitList();
            InitMainView();
            Content = AboutView;
        }

        public virtual void OnItemTapped(object sender, IFDataEvent e)
        {
            if (LastTabbed.AddSeconds(1) > DateTime.Now)
                return;
            LastTabbed = DateTime.Now;
            ItemTapped?.Invoke(sender, e);
            InvokeWhenTapped?.Invoke(e);
        }

        protected virtual void InitList()
        {
            C.ItemTemplate = new DataTemplate(() => new FTLAbout(C, OnItemTapped));
            C.LayoutManager = new GridLayout { SpanCount = 1, ItemsCacheLimit = 20 };
            C.ItemSpacing = 0;
            C.AllowSwiping = true;
            C.IsScrollBarVisible = false;
            C.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            C.AutoFitMode = AutoFitMode.DynamicHeight;
            C.ItemsSource = Items;
        }

        protected virtual void InitHeader()
        {
            Version.TextColor = Copyright.TextColor = FSetting.DisableColor;
            Version.Init(LayoutOptions.EndAndExpand, Version.VerticalOptions, TextAlignment.End, TextAlignment.Center);
            Version.Text = FSetting.V ? $"{FText.Version}: {FInterface.IFVersion?.InstalledVersionNumber}" : $"{FText.Version}: {FInterface.IFVersion?.InstalledVersionNumber}";
            Version.Margin = new Thickness(0, 0, 11, 0);
            Version.MaxLines = 2;
            Copyright.Init(LayoutOptions.StartAndExpand, Version.VerticalOptions, TextAlignment.Center, TextAlignment.Center);
            Copyright.MaxLines = 2;
            Copyright.Text = string.Format(FText.Copyright, DateTime.Now.Year.ToString());
            Copyright.Margin = new Thickness(11, 0, 0, 0);
        }

        protected void AddItem(string imagePath, string color, string action, string controller, string titlePage, string title, string subTitle)
        {
            Items.Add(new FItemAbout { ImageSource = imagePath.ToImageSource(color), Action = action, Controller = controller, TitlePage = titlePage, Title = FHtml.ReplaceTab(FHtml.ReplaceEnter(title)), Subtitle = FHtml.ReplaceTab(FHtml.ReplaceEnter(subTitle)) });
        }

        private void InitMainView()
        {
            AboutView.RowSpacing = 5;
            AboutView.Padding = new Thickness(0, 5);
            AboutView.BackgroundColor = FSetting.BackgroundSpacing;
            AboutView.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            AboutView.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AboutView.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            AboutView.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AboutView.Children.Add(Version, 0, 0);
            AboutView.Children.Add(C, 0, 1);
            AboutView.Children.Add(Copyright, 0, 2);
        }
    }
}