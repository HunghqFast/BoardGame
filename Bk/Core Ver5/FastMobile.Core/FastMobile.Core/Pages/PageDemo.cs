using FastMobile.FXamarin.Core;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Core
{
    public class PageDemo : FPage
    {
        private readonly FCombobox button;

        public PageDemo() : base(true, true)
        {
            button = new FCombobox();
            Content = new StackLayout { Children = { button } };
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ShowNothing = true;
        }

        protected override async void OnRefreshing(object sender, EventArgs e)
        {
            base.OnRefreshing(sender, e);
            await SetRefresh(true);
            ShowNothing = false;
            await SetRefresh(false);
        }
    }

    public class FCombobox : StackLayout
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(FCombobox), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty DisplayItemPathProperty = BindableProperty.Create("DisplayItemPath", typeof(string), typeof(FCombobox));
        public static readonly BindableProperty SelectedValuePathProperty = BindableProperty.Create("SelectedValuePath", typeof(string), typeof(FCombobox));
        public static readonly BindableProperty IsSuggestProperty = BindableProperty.Create("IsSuggest", typeof(bool), typeof(FCombobox), true);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string DisplayItemPath
        {
            get => (string)GetValue(DisplayItemPathProperty);
            set => SetValue(DisplayItemPathProperty, value);
        }

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        public bool IsSuggest
        {
            get => (bool)GetValue(IsSuggestProperty);
            set => SetValue(IsSuggestProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => listView.ItemTemplate;
            set => listView.ItemTemplate = value;
        }

        public object ItemsSource
        {
            get => listView.ItemsSource;
            set => listView.ItemsSource = value;
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;

        public event EventHandler<FChangingObjectArgs<string>> SelectionChanging;

        public event EventHandler<FObjectPropertyArgs<string>> SelectionChanged;

        private readonly SfPopupLayout dropdown;
        private readonly FEntryBase input;
        private readonly SfListView listView;
        private object CurrentItem;

        public FCombobox() : base()
        {
            dropdown = new SfPopupLayout();
            input = new FEntryBase();
            listView = new SfListView();
            Base();
            ItemsSource = Data();
        }

        protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
            if (IsSuggest)
                OpenDropdown();
        }

        private void Base()
        {
            input.BindingContext = this;
            input.SetBinding(Entry.TextProperty, TextProperty.PropertyName, BindingMode.TwoWay);
            Orientation = StackOrientation.Horizontal;
            input.HeightRequest = 45;
            Children.Add(input);
            BackgroundColor = Color.FromHex("#abbad1");

            input.TextChanged += OnTextChanged;
            dropdown.PopupView.ShowHeader = false;
            dropdown.PopupView.ShowFooter = false;
            dropdown.PopupView.ContentTemplate = new DataTemplate(() => listView);
            dropdown.PopupView.AutoSizeMode = AutoSizeMode.Height;
            dropdown.PopupView.PopupStyle.CornerRadius = 5;
            dropdown.PopupView.PopupStyle.OverlayColor = Color.Transparent;
            dropdown.PopupView.BindingContext = this;
            dropdown.PopupView.SetBinding(PopupView.WidthRequestProperty, WidthProperty.PropertyName);
            dropdown.PopupView.AnimationDuration = 20;

            listView.ItemTapped += OnSelectionChanged;
            listView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            listView.AutoFitMode = AutoFitMode.Height;
            ItemTemplate = new DataTemplate(() => Template());
            Margin = new Thickness(10);
        }

        private void OnSelectionChanged(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var type = CurrentItem.GetType();
            var cancel = new FChangingObjectArgs<string>(CurrentItem == null ? null : type.GetPropValue(CurrentItem, string.IsNullOrEmpty(SelectedValuePath) ? FItem.ItemID : SelectedValuePath)?.ToString());
            SelectionChanging?.Invoke(this, cancel);
            if (cancel.Cancel)
                return;
            CurrentItem = e.ItemData;
            Text = type.GetPropValue(CurrentItem, string.IsNullOrEmpty(DisplayItemPath) ? FItem.ItemValue : DisplayItemPath)?.ToString();
            SelectionChanged?.Invoke(this, new FObjectPropertyArgs<string>(type.GetPropValue(CurrentItem, string.IsNullOrEmpty(SelectedValuePath) ? FItem.ItemID : SelectedValuePath)?.ToString()));
            CloseDropdown();
        }

        private void OpenDropdown()
        {
            dropdown.ShowRelativeToView(input, RelativePosition.AlignBottom, input.X, input.Y - input.Height);
        }

        private void CloseDropdown()
        {
            dropdown.IsOpen = false;
        }

        private View Template()
        {
            var g = new Grid();
            var i = new Label();
            var n = new Label();
            var h = new FLine();
            var v = new FLine();

            i.VerticalOptions = n.VerticalOptions = LayoutOptions.CenterAndExpand;
            i.VerticalTextAlignment = n.VerticalTextAlignment = TextAlignment.Center;
            i.FontSize = n.FontSize = FSetting.FontSizeLabelContent;
            i.TextColor = n.TextColor = FSetting.TextColorContent;
            i.FontFamily = n.FontFamily = FSetting.FontText;
            i.LineBreakMode = n.LineBreakMode = LineBreakMode.TailTruncation;
            i.Margin = n.Margin = 10;

            i.SetBinding(Label.TextProperty, FItem.ItemID);
            n.SetBinding(Label.TextProperty, FItem.ItemValue, converter: new FStringNoNullConvert());

            g.RowSpacing = g.RowSpacing = 0;
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = 120 });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.Children.Add(i, 0, 0);
            g.Children.Add(v, 1, 0);
            g.Children.Add(n, 2, 0);
            g.Children.Add(h, 0, 1);
            g.SetBinding(Grid.BackgroundColorProperty, DisplayItemPathProperty.PropertyName, converter: new FConvertEntryTextToBackground(input, FSetting.DisableColor, Color.Transparent));
            Grid.SetColumnSpan(h, 3);
            return g;
        }

        private ObservableCollection<FItem> Data()
        {
            var resul = new ObservableCollection<FItem>
            {
                {new FItem("1", "val 1")},
                {new FItem("2", "val 2")},
                {new FItem("3", "val 3")}
            };
            return resul;
        }
    }
}