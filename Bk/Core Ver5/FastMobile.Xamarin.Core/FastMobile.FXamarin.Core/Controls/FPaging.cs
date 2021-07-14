using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPaging : Grid
    {
        private FPicker picker;
        public static readonly BindableProperty TriggerRefreshProperty = BindableProperty.Create("TriggerRefresh", typeof(bool), typeof(FGridBase), false);
        public static readonly BindableProperty ListItemProperty = BindableProperty.Create("ListItem", typeof(ObservableCollection<int>), typeof(FPaging), new ObservableCollection<int>());
        public static readonly BindableProperty ListPagingProperty = BindableProperty.Create("ListPaging", typeof(ObservableCollection<int>), typeof(FPaging), new ObservableCollection<int>());
        public static readonly BindableProperty IsVisiblePagingProperty = BindableProperty.Create("IsVisiblePaging", typeof(bool), typeof(FPaging), true);
        public static readonly BindableProperty ItemPickerProperty = BindableProperty.Create("ItemPicker", typeof(int), typeof(FPaging), 20);
        public static readonly BindableProperty PageStatePickerProperty = BindableProperty.Create("PageState", typeof(int), typeof(FPaging), 1);
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create("SelectedColor", typeof(Color), typeof(FPaging), FSetting.DisableColor);
        public static readonly BindableProperty UnSelectedColorProperty = BindableProperty.Create("UnSelectedColor", typeof(Color), typeof(FPaging), Color.Default);

        public bool TriggerRefresh { get => (bool)GetValue(TriggerRefreshProperty); set => SetValue(TriggerRefreshProperty, value); }

        public ObservableCollection<int> ListItem { get => (ObservableCollection<int>)GetValue(ListItemProperty); set => SetValue(ListItemProperty, value); }

        public ObservableCollection<int> ListPaging { get => (ObservableCollection<int>)GetValue(ListPagingProperty); set => SetValue(ListPagingProperty, value); }

        public bool IsVisiblePaging { get => (bool)GetValue(IsVisiblePagingProperty); set => SetValue(IsVisiblePagingProperty, value); }

        public int ItemPicker { get => (int)GetValue(ItemPickerProperty); set => SetValue(ItemPickerProperty, value); }

        public int PageState { get => (int)GetValue(PageStatePickerProperty); set => SetValue(PageStatePickerProperty, value); }

        public Color SelectedColor { get => (Color)GetValue(SelectedColorProperty); set => SetValue(SelectedColorProperty, value); }

        public Color UnSelectedColor { get => (Color)GetValue(UnSelectedColorProperty); set => SetValue(UnSelectedColorProperty, value); }

        public FPickerMode PickerMode { get; set; }

        public FPaging()
        {
            Padding = new Thickness(5, 0, 5, 0);
            HeightRequest = 40;
            Margin = 0;
            ColumnSpacing = 2;
            PickerMode = FPickerMode.Default;
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            UnSelectedColor = FSetting.PrimaryColor;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(ListPaging):
                    InitView();
                    break;
                default:
                    break;
            }
        }

        private void ChangePaging(object sender, EventArgs e)
        {
            var text = (sender as Button).Text;

            switch (text)
            {
                case ">":
                    PageState++;
                    TriggerRefresh = !TriggerRefresh;
                    break;
                case "<":
                    PageState--;
                    TriggerRefresh = !TriggerRefresh;
                    break;
                default:
                    PageState = Int32.Parse(text);
                    TriggerRefresh = !TriggerRefresh;
                    break;
            }
        }

        private Button PagingItem(string text, Color textColor, bool isPaging = true)
        {
            var btn = new Button();
            if (isPaging) btn.Clicked += ChangePaging;
            btn.Text = text;
            btn.TextColor = textColor;
            btn.HeightRequest = 40;
            btn.FontSize = FSetting.FontSizeLabelContent;
            btn.FontFamily = FSetting.FontText;
            btn.BackgroundColor = Color.Transparent;
            btn.HorizontalOptions = LayoutOptions.CenterAndExpand;
            btn.BorderWidth = 0;
            return btn;
        }

        private void AddLabel(ref StackLayout stack, string text, string bindingName = null)
        {
            var label = new Label
            {
                Text = text,
                HeightRequest = 40,
                FontSize = FSetting.FontSizeLabelContent,
                FontFamily = FSetting.FontText,
                BackgroundColor = Color.Transparent,
                TextColor = FSetting.TextColorContent,
                VerticalTextAlignment = TextAlignment.Center
            };
            if (bindingName != null) label.SetBinding(Label.TextProperty, bindingName);
            stack.Children.Add(label);
        }

        private void AddNewCoumn(ref int i, GridLength width, View view, bool disible = false)
        {
            ColumnDefinitions.Add(new ColumnDefinition { Width = width });
            if (view != null)
            {
                if (disible)
                {
                    (view as Button).TextColor = SelectedColor;
                    (view as Button).Clicked -= ChangePaging;
                }
                Children.Add(view, i, 0);
            }
            i++;
        }

        public void InitSettings()
        {
            picker = new FPicker();
            picker.Title = FText.Done;
            picker.PickerMode = PickerMode;
            picker.ItemSource.Clear();

            ListItem.ForEach((x) => picker.ItemSource.Add(new FPickerItem { Value = x }));
            picker.BindingContext = this;
            picker.SetBinding(FPicker.ValueProperty, ItemPickerProperty.PropertyName);
            picker.Sucessed += (s, e) =>
            {
                ItemPicker = Convert.ToInt32(picker.Value);
                PageState = 1;
                TriggerRefresh = !TriggerRefresh;
            };
            picker.InitView();
        }

        public void InitView()
        {
            this.Children.Clear();
            this.ColumnDefinitions.Clear();

            var count = ListPaging.Count;
            var width = 30;
            var selectCountPerPage = PagingItem(string.Format("{0} ▼", ItemPicker), FSetting.TextColorContent, false);
            var stackLayout = new StackLayout { Margin = 0, Spacing = 0, Padding = 0, Orientation = StackOrientation.Horizontal, BindingContext = BindingContext };
            var scroll = new ScrollView { Content = stackLayout, HorizontalScrollBarVisibility = ScrollBarVisibility.Never, Orientation = ScrollOrientation.Horizontal };
            var id = 0;

            selectCountPerPage.BindingContext = this;
            selectCountPerPage.SetBinding(Button.TextProperty, ItemPickerProperty.PropertyName, stringFormat: "{0:0 ▼}");
            selectCountPerPage.Clicked += OnPicking;

            AddLabel(ref stackLayout, "0", "ItemFrom");
            AddLabel(ref stackLayout, "-");
            AddLabel(ref stackLayout, "0", "ItemTo");
            AddLabel(ref stackLayout, "/");
            AddLabel(ref stackLayout, "0", "TotalItem");

            if (count > 3)
            {
                if (ListPaging[1] > ListPaging[0]) AddNewCoumn(ref id, width - 10, PagingItem(ListPaging[0].ToString(), UnSelectedColor));
                AddNewCoumn(ref id, width - 10, PagingItem("<", UnSelectedColor), ListPaging[0] == PageState);
                for (int i = 1; i < count - 1; i++) AddNewCoumn(ref id, width, PagingItem(ListPaging[i].ToString(), UnSelectedColor), ListPaging[i] == PageState);
                AddNewCoumn(ref id, width - 10, PagingItem(">", UnSelectedColor), ListPaging[count - 1] == PageState);
                if (ListPaging[count - 2] < ListPaging[count - 1]) AddNewCoumn(ref id, width, PagingItem(ListPaging[count - 1].ToString(), UnSelectedColor));
            }
            AddNewCoumn(ref id, 5, null);
            AddNewCoumn(ref id, 40, selectCountPerPage);
            AddNewCoumn(ref id, 5, null);
            AddNewCoumn(ref id, GridLength.Auto, scroll);
        }

        private void OnPicking(object sender, EventArgs e)
        {
            InitSettings();
            picker.Show();
        }
    }
}