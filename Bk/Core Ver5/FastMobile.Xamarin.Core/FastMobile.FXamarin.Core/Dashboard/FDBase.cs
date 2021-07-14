using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Syncfusion.XForms.ComboBox.SfComboBox;

namespace FastMobile.FXamarin.Core
{
    public abstract class FDBase : Grid, IFLayout, IFBusy
    {
        public static readonly BindableProperty NothingTextProperty = BindableProperty.Create("NothingText", typeof(string), typeof(FDBase), string.Empty);
        public static readonly BindableProperty CurrentTextLeftProperty = BindableProperty.Create("CurrentTextLeft", typeof(string), typeof(FDBase), string.Empty);
        public static readonly BindableProperty CurrentTextRightProperty = BindableProperty.Create("CurrentTextRight", typeof(string), typeof(FDBase), string.Empty);
        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create("IsBusy", typeof(bool), typeof(FDBase), false);
        public static readonly BindableProperty ShowHeaderProperty = BindableProperty.Create("ShowHeader", typeof(bool), typeof(FDBase), false);
        public static readonly BindableProperty ShowLineFooterProperty = BindableProperty.Create("ShowLineFooter", typeof(bool), typeof(FDBase), false);
        public static readonly BindableProperty ShowFooterProperty = BindableProperty.Create("ShowFooter", typeof(bool), typeof(FDBase), false);
        public static readonly BindableProperty ShowNothingProperty = BindableProperty.Create("ShowNothing", typeof(bool), typeof(FDBase), false);
        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create("TitleText", typeof(string), typeof(FDGrid), string.Empty);

        public string CurrentTextLeft
        {
            get => (string)GetValue(CurrentTextLeftProperty);
            set => SetValue(CurrentTextLeftProperty, value);
        }

        public string CurrentTextRight
        {
            get => (string)GetValue(CurrentTextRightProperty);
            set => SetValue(CurrentTextRightProperty, value);
        }

        public string NothingText
        {
            get => (string)GetValue(NothingTextProperty);
            set => SetValue(NothingTextProperty, value);
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        public bool ShowFooter
        {
            get => (bool)GetValue(ShowFooterProperty);
            set => SetValue(ShowFooterProperty, value);
        }

        public bool ShowLineFooter
        {
            get => (bool)GetValue(ShowLineFooterProperty);
            set => SetValue(ShowLineFooterProperty, value);
        }

        public bool ShowNothing
        {
            get => (bool)GetValue(ShowNothingProperty);
            set => SetValue(ShowNothingProperty, value);
        }

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public View Content
        {
            get => SubView.Content;
            set => SubView.Content = value;
        }

        public ContentView SubView { get; }

        protected string CurrentCcodeLeft { get; private set; }
        protected string CurrentCcodeRight { get; private set; }
        protected DataSet SumaryResult { get; set; }
        protected DataTable DataResult { get; set; }
        protected FBDashboard ViewModel { get; set; }

        protected ObservableCollection<FItemCustom> ListSourceLeft { get; private set; }
        protected ObservableCollection<FItemCustom> ListSourceRight { get; private set; }

        protected event SelectionChangedEventHandler SelectionLeftChanged, SelectionRightChanged;

        protected const string LeftCharacter = "{?left}", RightCharacter = "{?right}";

        private readonly Dictionary<string, Label> ls;
        private readonly ActivityIndicator bs;
        private readonly FLabel ln;
        private readonly Label ti;
        private readonly StackLayout bl, hl, hdl, hdr;
        private readonly ImageButton btl, btr;
        private readonly SfComboBox cl, cr;
        private readonly BoxView bv;
        private bool ifl = false, ifr = false;

        public FDBase() : base()
        {
            ListSourceRight = new ObservableCollection<FItemCustom>();
            ListSourceLeft = new ObservableCollection<FItemCustom>();
            ls = new Dictionary<string, Label>();
            bl = new StackLayout();
            hl = new StackLayout();
            hdl = new StackLayout();
            hdr = new StackLayout();
            btl = new ImageButton();
            btr = new ImageButton();
            SumaryResult = new DataSet();
            bs = new ActivityIndicator();
            ln = new FLabel();
            ti = new Label();
            cl = new SfComboBox();
            cr = new SfComboBox();
            bv = new BoxView();
            SubView = new ContentView();
        }

        public virtual Task OnLoaded()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnChanged()
        {
            return Task.CompletedTask;
        }

        public virtual void UpdateHeight(double height = -1, bool setHeight = false)
        {

        }

        public async Task SetBusy(bool value)
        {
            IsBusy = value;
            await Task.Delay(1);
        }

        protected virtual void Init()
        {
            ti.BindingContext = this;
            ti.HorizontalOptions = LayoutOptions.CenterAndExpand;
            ti.VerticalOptions = LayoutOptions.Center;
            ti.FontFamily = FSetting.FontTextMedium;
            ti.FontAttributes = FontAttributes.Bold;
            ti.TextColor = FSetting.TextColorTitle;
            ti.FontSize = FSetting.FontSizeLabelTitle;
            ti.Padding = new Thickness(5, 10, 5, 0);
            ti.MaxLines = 5;
            ti.LineBreakMode = LineBreakMode.TailTruncation;
            ti.SetBinding(Label.TextProperty, TitleTextProperty.PropertyName);

            ln.BindingContext = this;
            ln.SetBinding(Label.TextProperty, NothingTextProperty.PropertyName);
            ln.SetBinding(Label.IsVisibleProperty, ShowNothingProperty.PropertyName);
            ln.Init(LayoutOptions.Fill, LayoutOptions.CenterAndExpand, TextAlignment.Center, TextAlignment.Center);

            hl.BindingContext = this;
            hl.Spacing = 0;
            hl.Orientation = StackOrientation.Horizontal;
            hl.HorizontalOptions = hl.VerticalOptions = LayoutOptions.Fill;
            hl.Margin = new Thickness(15, 10, 14, 0);
            hl.HeightRequest = 50;
            hl.SetBinding(VisualElement.WidthRequestProperty, VisualElement.WidthRequestProperty.PropertyName);
            hl.SetBinding(VisualElement.IsVisibleProperty, ShowHeaderProperty.PropertyName);

            bs.BindingContext = this;
            bs.VerticalOptions = bs.HorizontalOptions = LayoutOptions.CenterAndExpand;
            bs.Color = FSetting.BusyColor;
            bs.WidthRequest = 30;
            bs.HeightRequest = 30;
            bs.SetBinding(ActivityIndicator.IsRunningProperty, IsBusyProperty.PropertyName);
            bs.SetBinding(VisualElement.IsVisibleProperty, IsBusyProperty.PropertyName);

            bl.BindingContext = this;
            bl.BackgroundColor = FSetting.BackgroundMain;
            bl.HorizontalOptions = bl.VerticalOptions = LayoutOptions.Fill;
            bl.SetBinding(VisualElement.IsVisibleProperty, IsBusyProperty.PropertyName);

            bv.BindingContext = this;
            bv.VerticalOptions = bv.HorizontalOptions = LayoutOptions.Fill;
            bv.BackgroundColor = FSetting.LineBoxReportColor;
            bv.SetBinding(VisualElement.IsVisibleProperty, ShowLineFooterProperty.PropertyName);

            bl.Children.Add(bs);

            HorizontalOptions = VerticalOptions = LayoutOptions.Fill;
            RowSpacing = 0;

            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = 1 });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Children.Add(hl, 0, 0);
            Children.Add(ti, 0, 1);
            Children.Add(SubView, 0, 2);
            Children.Add(ln, 0, 2);
            Children.Add(bl, 0, 2);
            Children.Add(bv, 0, 3);
            UpdateHeight(FSetting.HeightChart, true);
        }

        protected virtual void InitHeader()
        {
            cr.WidthRequest = cl.WidthRequest = 0;
            cr.TextColor = cl.TextColor = Color.Transparent;
            cr.ShowBorder = cl.ShowBorder = false;
            cr.DropDownItemFontFamily = cl.DropDownItemFontFamily = FSetting.FontText;
            cr.DropDownTextSize = cl.DropDownTextSize = FSetting.FontSizeLabelContent;
            cr.SelectedValuePath = cl.SelectedValuePath = FItemCustom.IDProperty.PropertyName;
            cr.DisplayMemberPath = cl.DisplayMemberPath = FItemCustom.ValueProperty.PropertyName;
            cr.MultiSelectMode = cl.MultiSelectMode = MultiSelectMode.None;
            cr.DropDownWidth = cl.DropDownWidth = Convert.ToInt32(FSetting.ScreenWidth * 0.6);
            cr.HorizontalOptions = cl.HorizontalOptions = LayoutOptions.Center;

            cl.BindingContext = this;
            cl.DataSource = ListSourceLeft;
            cl.SetBinding(SfComboBox.TextProperty, CurrentTextLeftProperty.PropertyName);
            cl.SetBinding(SfComboBox.IsEnabledProperty, IsBusyProperty.PropertyName, BindingMode.Default, FInvertBool.Instance);

            cr.BindingContext = this;
            cr.DataSource = ListSourceRight;
            cr.SetBinding(SfComboBox.TextProperty, CurrentTextRightProperty.PropertyName);
            cr.SetBinding(SfComboBox.IsEnabledProperty, IsBusyProperty.PropertyName, BindingMode.Default, FInvertBool.Instance);

            btl.BackgroundColor = btr.BackgroundColor = Color.Transparent;
            btl.BorderWidth = btr.BorderWidth = 0;
            btl.HeightRequest = btr.HeightRequest = btl.WidthRequest = btr.WidthRequest = FSetting.SizeButtonIcon;
            btl.CornerRadius = btr.CornerRadius = 5;

            btl.Clicked += LeftClicked;
            btl.HorizontalOptions = LayoutOptions.StartAndExpand;

            btr.Clicked += RightClicked;
            btr.HorizontalOptions = LayoutOptions.EndAndExpand;

            hdl.HorizontalOptions = LayoutOptions.StartAndExpand;
            hdl.Orientation = StackOrientation.Horizontal;
            hdl.Spacing = 0;
            hdl.Children.Add(cl);
            hdl.Children.Add(btl);

            hdr.WidthRequest = FSetting.ScreenWidth * 0.6;
            hdr.HorizontalOptions = LayoutOptions.EndAndExpand;
            hdr.Orientation = StackOrientation.Horizontal;
            hdr.Spacing = 0;
            hdr.Children.Add(cr);
            hdr.Children.Add(btr);

            hl.Children.Add(hdl);
            hl.Children.Add(hdr);
        }

        protected virtual void InitHeaderItemSource()
        {
            if (ViewModel == null)
                return;

            if (ViewModel.Header == null)
            {
                cl.IsVisible = cr.IsVisible = false;
                return;
            }

            string left = "", right = "";
            if (ViewModel.Header.Left != null && ViewModel.Header.Right == null)
            {
                CurrentCcodeLeft = GetDefaultSelection(SumaryResult, ViewModel.Header.Left);
                CreateSource(SumaryResult, ViewModel.Header.Left.Table, ViewModel.Header.Left.TextKey, ViewModel.Header.Left.TextValue, CurrentCcodeLeft, ListSourceLeft, cl, ref left);
                CurrentTextLeft = left;
                cr.IsVisible = false;
            }
            else if (ViewModel.Header.Right != null && ViewModel.Header.Left == null)
            {
                CurrentCcodeRight = GetDefaultSelection(SumaryResult, ViewModel.Header.Right);
                CreateSource(SumaryResult, ViewModel.Header.Right.Table, ViewModel.Header.Right.TextKey, ViewModel.Header.Right.TextValue, CurrentCcodeRight, ListSourceRight, cr, ref right);
                CurrentTextRight = right;
                cl.IsVisible = false;
            }
            else if (ViewModel.Header.Right != null && ViewModel.Header.Left != null)
            {
                CurrentCcodeLeft = GetDefaultSelection(SumaryResult, ViewModel.Header.Left);
                CurrentCcodeRight = GetDefaultSelection(SumaryResult, ViewModel.Header.Right);
                CreateSource(SumaryResult, ViewModel.Header.Left.Table, ViewModel.Header.Left.TextKey, ViewModel.Header.Left.TextValue, CurrentCcodeLeft, ListSourceLeft, cl, ref left);
                CreateSource(SumaryResult, ViewModel.Header.Right.Table, ViewModel.Header.Right.TextKey, ViewModel.Header.Right.TextValue, CurrentCcodeRight, ListSourceRight, cr, ref right);
                CurrentTextLeft = left;
                CurrentTextRight = right;
            }
            if (ListSourceLeft.Count == 0)
                cl.IsVisible = false;
            if (ListSourceRight.Count == 0)
                cr.IsVisible = false;
            ReplaceLeftRight(CurrentTextLeft, CurrentTextRight, true);
        }

        protected virtual void InitModel()
        {
            InitHeaderModel();
            InitFooterModel();
            DataResult = SumaryResult.Tables[ViewModel.LoadingTable];
        }

        protected virtual void Error()
        {
            ShowNothing = true;
        }

        protected virtual void InitLabelStyle(Label label, FBLabelStyle style)
        {
            if (style == null)
            {
                label.FontFamily = FSetting.FontText;
                label.TextColor = Color.White;
                label.FontSize = FSetting.FontSizeLabelHint;
                return;
            }
            label.MaxLines = style.MaxLine;
            label.LineBreakMode = LineBreakMode.TailTruncation;
            label.FontAttributes = style.FontAttributes;
            label.FontFamily = style.FontAttributes == FontAttributes.Bold ? FSetting.FontTextBold : style.FontAttributes == FontAttributes.Italic ? FSetting.FontTextItalic : FSetting.FontText;
            label.TextColor = Color.FromHex(style.TextColor);
            label.BackgroundColor = Color.FromHex(style.BackgroundColor);
            label.Margin = GetThickness(label.Margin, style.Margin);
            label.Padding = GetThickness(label.Padding, style.Padding);
            label.FontSize = style.FontSize != -1 ? style.FontSize : FSetting.FontSizeLabelHint;
            label.WidthRequest = FSetting.ArrayToDoubleByPercentAndScreenWidth(style.Width, style.WidthType);
        }

        protected virtual void RefreshFooter(bool isLoading)
        {
            ReplaceFooter(CurrentTextLeft, CurrentTextRight, isLoading);
        }

        protected virtual Thickness GetThickness(Thickness reff, double[] input)
        {
            if (input == null)
                return reff;
            return input.Length == 1 ? new Thickness(input[0]) : input.Length == 2 ? new Thickness(input[0], input[1]) : input.Length == 4 ? new Thickness(input[0], input[1], input[2], input[3]) : reff;
        }

        protected virtual void OnLeftSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SelectionLeftChanged?.Invoke(sender, e);
        }

        protected virtual void OnRightSelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SelectionRightChanged?.Invoke(sender, e);
        }

        protected virtual void ReplaceLeftRight(string left, string right, bool isLoading)
        {
            NothingText = ViewModel.DefaultData?.Replace(LeftCharacter, left).Replace(RightCharacter, right);
            ReplaceFooter(left, right, isLoading);
        }

        private void InitHeaderModel()
        {
            if (ViewModel.Header == null)
            {
                ShowHeader = false;
                return;
            }
            if (ViewModel.Header.Left == null)
                cl.IsVisible = false;
            else
            {
                string code = ViewModel.Header.Left.Icon != null ? FIcons.Type.GetStaticFieldValue(ViewModel.Header.Left.Icon.Path)?.ToString() : "";
                btl.Source = (ViewModel.Header.Left.Icon != null ? (string.IsNullOrEmpty(code) ? FIcons.CalendarMonth : code) : FIcons.CalendarMonth).ToFontImageSource(ViewModel.Header.Left.Icon != null ? Color.FromHex(ViewModel.Header.Left.Icon.Color) : Color.FromHex("#a7a7a7"), FSetting.SizeButtonIcon);
                cl.IsVisible = ViewModel.Header.Left.IsVisible;
                CurrentCcodeLeft = ViewModel.Header.Left.DefaultCode;
            }
            if (ViewModel.Header.Right == null)
                cr.IsVisible = false;
            else
            {
                string code = ViewModel.Header.Right.Icon != null ? FIcons.Type.GetStaticFieldValue(ViewModel.Header.Right.Icon.Path)?.ToString() : "";
                btr.Source = (ViewModel.Header.Right.Icon != null ? (string.IsNullOrEmpty(code) ? FIcons.CogOutline : code) : FIcons.CogOutline).ToFontImageSource(ViewModel.Header.Right.Icon != null ? Color.FromHex(ViewModel.Header.Right.Icon.Color) : Color.FromHex("#a7a7a7"), FSetting.SizeButtonIcon);
                cr.IsVisible = ViewModel.Header.Right.IsVisible;
                CurrentCcodeRight = ViewModel.Header.Right.DefaultCode;
            }
            ShowHeader = ViewModel.Header.IsVisible;
        }

        private void InitFooterModel()
        {
            if (ViewModel.Footer == null || ViewModel.Footer.Rows == null || ViewModel.Footer.Rows.Count == 0)
                return;
            ShowLineFooter = ViewModel.Footer.ShowLine;
            RenderFooter(ViewModel.Footer.Rows);
        }

        private void ReplaceFooter(string left, string right, bool isLoading)
        {
            if (ViewModel.Footer == null || ViewModel.Footer.Rows == null || ViewModel.Footer.Rows.Count == 0)
            {
                ShowFooter = false;
                return;
            }

            ShowFooter = ViewModel.Footer.IsVisible;
            RefreshFooter(SumaryResult, left, right, isLoading);
        }

        private void CreateSource(DataSet ds, int table, string textKey, string textValue, string defaultCode, ObservableCollection<FItemCustom> source, SfComboBox cb, ref string text)
        {
            source.Clear();
            ds.Tables[table].Rows.ForEach<DataRow>((x) => AddRow(x, textKey, textValue, source));
            var selectedObj = source.ToList().Find((x) => x.ID == defaultCode);
            text = selectedObj?.Value;
            cb.Text = selectedObj?.Value;
            if (string.IsNullOrEmpty(text))
                cb.Text = source.Count > 0 ? source[0].Value : null;
        }

        private void AddRow(DataRow row, string textKey, string textValue, ObservableCollection<FItemCustom> source)
        {
            source.Add(new FItemCustom { ID = row[textKey].ToString().Trim(), Value = row[textValue].ToString().Trim() });
        }

        private string GetDefaultSelection(DataSet dataSet, FBOptions options)
        {
            if (options != null && string.IsNullOrEmpty(options.DefaultCode) && !string.IsNullOrEmpty(options.DefaultKey) && dataSet.Tables[options.DefaultTable] != null && dataSet.Tables[options.DefaultTable].Columns.Contains(options.DefaultKey) && dataSet.Tables[options.DefaultTable].Rows.Count > 0)
                return dataSet.Tables[options.DefaultTable].Rows[0][options.DefaultKey].ToString().Trim();
            return options.DefaultCode;
        }

        private void ChangingHeader(object sender, SelectionChangingEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                MessagingCenter.Send(new FMessage(0, 403, ""), FChannel.ALERT_BY_MESSAGE);
                var cb = sender as SfComboBox;
                cb.IsDropDownOpen = false;
                e.Cancel = true;
            }
        }

        private void ChangedHeader(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            CurrentTextRight = cr.Text;
            CurrentTextLeft = cl.Text;
            CurrentCcodeLeft = cl.SelectedValue?.ToString();
            CurrentCcodeRight = cr.SelectedValue?.ToString();
            ReplaceLeftRight(CurrentTextLeft, CurrentTextRight, false);
        }

        private void LeftClicked(object sender, EventArgs e)
        {
            cl.IsDropDownOpen = !cl.IsDropDownOpen;
            if (!ifl)
            {
                ifl = true;
                cl.SelectionChanging += ChangingHeader;
                cl.SelectionChanged += ChangedHeader;
                cl.SelectionChanged += OnLeftSelectionChanged;
            }
        }

        private void RightClicked(object sender, EventArgs e)
        {
            cr.IsDropDownOpen = !cr.IsDropDownOpen;
            if (!ifr)
            {
                ifr = true;
                cr.SelectionChanging += ChangingHeader;
                cr.SelectionChanged += ChangedHeader;
                cr.SelectionChanged += OnRightSelectionChanged;
            }
        }

        private void RefreshFooter(DataSet data, string left, string right, bool isLoading)
        {
            if (ls.Count < 1)
                return;
            ViewModel.Footer.Rows?.ForEach(r => r.Views.ForEach(t => ls[t.Name].Text = LabelText(data, t.Name, t.Text, left, right, t.Table, t.RefreshTable, t.Row, t.RefreshRow, isLoading)));
        }

        private void RenderFooter(List<BFooterRow> texts)
        {
            if (ls.Count > 0)
                return;
            texts?.ForEach(t =>
            {
                RowDefinitions.Add(new RowDefinition { Height = t.Height == -1 ? GridLength.Auto : t.Height });
                Children.Add(FooterRow(t), 0, Children.Count - 1);
            });
        }

        private Grid FooterRow(BFooterRow r)
        {
            var gr = new Grid();
            gr.RowSpacing = 0;
            gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gr.Padding = GetThickness(gr.Padding, r.Padding);
            gr.SetBinding(Grid.IsVisibleProperty, ShowFooterProperty.PropertyName);

            r.Views?.ForEach(t =>
            {
                gr.ColumnDefinitions.Add(new ColumnDefinition { Width = GLength(t.Width) });
                gr.Children.Add(FooterView(t), gr.ColumnDefinitions.Count - 1, 0);
            });
            return gr;
        }

        private View FooterView(BTextView t)
        {
            var lb = LabelFromStyle(t);
            ls.TryAdd(t.Name, lb);
            return lb;
        }

        private string LabelText(DataSet data, string name, string text, string left, string right, int table, int table2, int row, int row2, bool isLoading)
        {
            return table == -1 || row == -1 || !string.IsNullOrEmpty(text) ? text.Replace(LeftCharacter, left).Replace(RightCharacter, right) : data.Tables[isLoading ? table : table2 == -1 ? table : table2].Rows[isLoading ? row : row2 == -1 ? row : row2][name].ToString();
        }

        private GridLength GLength(string width)
        {
            return width switch
            {
                "_" => GridLength.Auto,
                "*" => GridLength.Star,
                _ => Convert.ToDouble(width) * FSetting.ScreenWidth * 0.01
            };
        }

        private Label LabelFromStyle(BTextView style)
        {
            var lb = new Label();
            if (style == null)
            {
                lb.FontFamily = FSetting.FontText;
                lb.FontSize = FSetting.FontSizeLabelContent;
                lb.TextColor = FSetting.TextColorContent;
                return lb;
            }
            lb.FontSize = style.FontSize == -1 ? FSetting.FontSizeLabelContent : style.FontSize;
            lb.MaxLines = style.MaxLine;
            lb.LineBreakMode = LineBreakMode.TailTruncation;
            lb.TextColor = Color.FromHex(style.TextColor);
            lb.FontAttributes = style.FontAttributes;
            lb.FontFamily = style.FontAttributes == FontAttributes.Bold ? FSetting.FontTextBold : style.FontAttributes == FontAttributes.Italic ? FSetting.FontTextItalic : FSetting.FontText;
            lb.VerticalOptions = LayoutOptions.Start;
            lb.VerticalTextAlignment = TextAlignment.Center;
            lb.HorizontalTextAlignment = style.TextAlignment;
            lb.HorizontalOptions = new LayoutOptions(style.Alignment, true);
            return lb;
        }
    }
}