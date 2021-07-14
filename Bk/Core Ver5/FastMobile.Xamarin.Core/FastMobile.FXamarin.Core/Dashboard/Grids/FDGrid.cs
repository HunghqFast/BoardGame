using Syncfusion.SfDataGrid.XForms;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FDGrid : FDBase
    {
        protected FGDashboard Model => ViewModel as FGDashboard;

        protected ObservableCollection<FBData> DataSource
        {
            get => DataGrid.ItemsSource as ObservableCollection<FBData>;
            set => DataGrid.ItemsSource = value;
        }

        protected ObservableCollection<int> ListItem { get; set; }

        public static readonly BindableProperty ListPagingProperty = BindableProperty.Create("ListPaging", typeof(ObservableCollection<int>), typeof(FDGrid));
        public static readonly BindableProperty TriggerRefreshProperty = BindableProperty.Create("TriggerRefresh", typeof(bool), typeof(FDGrid), false);
        public static readonly BindableProperty ItemPerPageProperty = BindableProperty.Create("ItemPerPage", typeof(int), typeof(FDGrid), 10);
        public static readonly BindableProperty PageIndexProperty = BindableProperty.Create("PageIndex", typeof(int), typeof(FDGrid), 1);
        public static readonly BindableProperty ItemToProperty = BindableProperty.Create("ItemTo", typeof(int), typeof(FDGrid), 0);
        public static readonly BindableProperty ItemFromProperty = BindableProperty.Create("ItemFrom", typeof(int), typeof(FDGrid), 0);
        private static readonly BindableProperty TotalItemProperty = BindableProperty.Create("TotalItem", typeof(int), typeof(FDGrid), 0);

        public ObservableCollection<int> ListPaging
        {
            get => (ObservableCollection<int>)GetValue(ListPagingProperty);
            set => SetValue(ListPagingProperty, value);
        }

        public bool TriggerRefresh
        {
            get => (bool)GetValue(TriggerRefreshProperty);
            set => SetValue(TriggerRefreshProperty, value);
        }

        public int ItemTo
        {
            get => (int)GetValue(ItemToProperty);
            set => SetValue(ItemToProperty, value);
        }

        public int ItemFrom
        {
            get => (int)GetValue(ItemFromProperty);
            set => SetValue(ItemFromProperty, value);
        }

        public int ItemPerPage
        {
            get => (int)GetValue(ItemPerPageProperty);
            set => SetValue(ItemPerPageProperty, value);
        }

        public int PageIndex
        {
            get => (int)GetValue(PageIndexProperty);
            set => SetValue(PageIndexProperty, value);
        }

        public int TotalItem
        {
            get => (int)GetValue(TotalItemProperty);
            set => SetValue(TotalItemProperty, value);
        }

        protected SfDataGrid DataGrid { get; private set; }
        protected FPaging Paging { get; private set; }

        private readonly ContentView V;
        private readonly Grid Grid;

        public FDGrid() : base()
        {
            DataGrid = new SfDataGrid();
            DataSource = new ObservableCollection<FBData>();
            Grid = new Grid();
            Paging = new FPaging();
            ListItem = new ObservableCollection<int>();
            ListPaging = new ObservableCollection<int>();
            V = new ContentView();
        }

        public override void UpdateHeight(double height = -1, bool setHeight = false)
        {
            if (setHeight)
            {
                DataGrid.HeightRequest = V.HeightRequest = height;
                return;
            }

            DataGrid.HeightRequest = V.HeightRequest = FSetting.HeightRowGrid * (DataSource.Count > 10 ? 10 : DataSource.Count) + DataGrid.HeaderRowHeight;
        }

        protected override void Init()
        {
            InitPaging();
            InitGrid();
            InitView();
            base.Init();
        }

        protected override void InitModel()
        {
            base.ViewModel = SumaryResult.Tables[SumaryResult.Tables.Count - 1].Rows[0]["dashboard"].ToString().ToObject<FGDashboard>();
            if (Model == null)
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            base.InitModel();
            UpdateTitle();
            InitBindingName();
            InitColumn();
            Grid.Margin = GetThickness(V.Margin, Model.Margin);
        }

        protected void InitDataSource(bool refresh = true)
        {
            TotalItem = DataResult.Rows.Count;
            ShowNothing = TotalItem == 0;
            DataGrid.IsVisible = !ShowNothing;
            PageIndex = refresh ? 1 : PageIndex;

            UpdatePaging(TotalItem);
            if (TotalItem == 0)
            {
                DataSource = new ObservableCollection<FBData>();
                return;
            }

            var datasource = new ObservableCollection<FBData>();
            for (int i = ItemFrom - 1; i <= ItemTo - 1; i++)
                datasource.Add(FBData.NewItem(DataResult.Rows[i], Model.Fields));
            DataSource = datasource;
            UpdateHeight();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == TriggerRefreshProperty.PropertyName)
            {
                InitDataSource(false);
                return;
            }

            if (propertyName == ItemPerPageProperty.PropertyName)
            {
                UpdateHeight();
                return;
            }
        }

        private void UpdateTitle()
        {
            TitleText = Model.Title;
        }

        private void InitListItem()
        {
            ListItem.Add(5);
            ListItem.Add(10);
            ListItem.Add(15);
            ListItem.Add(20);
            ListItem.Add(25);
        }

        private void InitPaging()
        {
            InitListItem();
            Paging.BindingContext = this;
            Paging.HorizontalOptions = LayoutOptions.EndAndExpand;
            Paging.PickerMode = FPickerMode.AutoCheck;
            Paging.ListItem = this.ListItem;
            Paging.SetBinding(FPaging.ListPagingProperty, ListPagingProperty.PropertyName);
            Paging.SetBinding(FPaging.TriggerRefreshProperty, TriggerRefreshProperty.PropertyName, BindingMode.TwoWay);
            Paging.SetBinding(FPaging.ItemPickerProperty, ItemPerPageProperty.PropertyName, BindingMode.TwoWay);
            Paging.SetBinding(FPaging.PageStatePickerProperty, PageIndexProperty.PropertyName, BindingMode.TwoWay);
        }

        private void InitGrid()
        {
            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.Children.Add(V, 0, 1);
            Grid.Children.Add(Paging, 0, 2);
            Content = Grid;
        }

        private void InitView()
        {
            DataGrid.GridStyle = new FGridSettings();
            DataGrid.SelectionMode = Syncfusion.SfDataGrid.XForms.SelectionMode.SingleDeselect;
            DataGrid.HeaderRowHeight = FSetting.HeightRowGrid - 5;
            DataGrid.RowHeight = FSetting.HeightRowGrid;
            DataGrid.AllowLoadMore = false;
            DataGrid.AutoGenerateColumns = false;
            DataGrid.AllowDiagonalScrolling = false;
            DataGrid.AllowResizingColumn = true;
            DataGrid.EnableDataVirtualization = true;
            DataGrid.VerticalOptions = LayoutOptions.Fill;
            DataGrid.ResizingMode = ResizingMode.OnTouchUp;
            DataGrid.ColumnSizer = ColumnSizer.SizeToHeader;
            DataGrid.ScrollingMode = ScrollingMode.Pixel;
            DataGrid.VerticalOverScrollMode = VerticalOverScrollMode.None;
            V.Content = DataGrid;
            UpdateHeight();
        }

        private void UpdatePaging(int total)
        {
            if (total == 0)
            {
                ItemFrom = 0;
                ItemTo = 0;
            }
            else
            {
                ItemFrom = 1 + (PageIndex - 1) * ItemPerPage > total ? ItemFrom : 1 + (PageIndex - 1) * ItemPerPage;
                ItemTo = PageIndex * ItemPerPage > total ? total : PageIndex * ItemPerPage;
            }
            ListPaging = UpdatePaging(total, PageIndex, ItemPerPage);
        }

        private void InitBindingName()
        {
            Model.Fields.ForIndex((x, i) => x.BindingName = $"F{i + 1}");
        }

        private void InitColumn()
        {
            Model.Fields.ForEach((x) => DataGrid.Columns.Add(GetColumn(x)));
        }

        private GridColumn GetColumn(FDField field)
        {
            return field.Type switch
            {
                FDFieldType.DateTime => new FDateColumn(field.Header) { Width = field.Width, MappingName = field.BindingName, TextAlignment = field.Align, Format = string.IsNullOrEmpty(field.DataFormatString) ? null : field.DataFormatString },
                FDFieldType.Decimal => new FNumericColumn(field.Header) { Width = field.Width, MappingName = field.BindingName, TextAlignment = field.Align, Format = string.IsNullOrEmpty(field.DataFormatString) ? null : field.DataFormatString },
                _ => new FTextColumn(field.Header) { Width = field.Width, MappingName = field.BindingName, TextAlignment = field.Align, Format = string.IsNullOrEmpty(field.DataFormatString) ? null : field.DataFormatString }
            };
        }

        private ObservableCollection<int> UpdatePaging(int total, int pageIndex, int itemPerPage)
        {
            var numPages = (itemPerPage + total - 1) / itemPerPage;
            int from, to;
            var result = new ObservableCollection<int>();

            if (numPages <= 5)
            {
                from = 1;
                to = numPages;
            }
            else
            {
                from = pageIndex - 2 > 0 ? pageIndex - 2 : 1;

                if (from + 4 > numPages)
                {
                    from = numPages - 4;
                    to = numPages;
                }
                else
                {
                    to = from + 4;
                }
            }
            result.Add(1);
            for (int i = from; i <= to; i++)
            {
                result.Add(i);
            }
            result.Add(numPages);
            return result;
        }
    }
}