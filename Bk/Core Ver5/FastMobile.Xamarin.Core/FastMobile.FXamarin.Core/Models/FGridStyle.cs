using Syncfusion.ListView.XForms;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FGridStyle
    {
        public static FGridStyle Instance { get; }

        static FGridStyle()
        {
            Instance = new FGridStyle();
        }

        public virtual SfListView InitListView()
        {
            var listView = new SfListView
            {
                AutoFitMode = AutoFitMode.Height,
                IsScrollBarVisible = true,
                LoadMorePosition = LoadMorePosition.Bottom,
                SelectionGesture = TouchGesture.Tap,
                Orientation = Orientation.Vertical,
                LayoutManager = new GridLayout { SpanCount = 1, ItemsCacheLimit = 50 },
                BackgroundColor = Color.White,
                SelectionBackgroundColor = Color.Default,
                Padding = new Thickness(0, -2, 0, 0)
            };
            return listView;
        }

        public virtual SfDataGrid InitGridView()
        {
            var gridView = new SfDataGrid
            {
                AllowLoadMore = false,
                AutoGenerateColumns = false,
                AllowResizingColumn = true,
                ResizingMode = ResizingMode.OnTouchUp,
                ColumnSizer = ColumnSizer.SizeToHeader,
                ScrollingMode = ScrollingMode.Pixel,
                VerticalOverScrollMode = VerticalOverScrollMode.None,
                GridStyle = new FGridSettings(),
                AllowDiagonalScrolling = false,
                EnableDataVirtualization = true,
                RowHeight = FSetting.HeightRowGrid,
                HeaderRowHeight = FSetting.HeightRowGrid - 5,
                SelectionMode = Syncfusion.SfDataGrid.XForms.SelectionMode.SingleDeselect
            };
            return gridView;
        }

        static public ObservableCollection<int> UpdatePaging(int total, int pageIndex, int itemPerPage)
        {
            var ne = new ObservableCollection<int>();
            var np = (itemPerPage + total - 1) / itemPerPage;
            var ck = pageIndex - 2 > 0 ? pageIndex - 2 : 1;
            var fr = np <= 5 ? 1 : ck + 4 > np ? np - 4 : ck;
            var to = np <= 5 ? np : fr + 4;

            ne.Add(1);
            Enumerable.Range(fr, to - fr + 1).ForEach(i => ne.Add(i));
            ne.Add(np);
            return ne;
        }

        public virtual DataTemplate TemplateBusyLoadMore(object binding)
        {
            return new DataTemplate(() =>
            {
                var grid = new Grid();
                var busyIndicator = new ActivityIndicator();

                busyIndicator.Color = FSetting.BusyColor;
                busyIndicator.WidthRequest = 40;
                busyIndicator.HeightRequest = 40;
                busyIndicator.VerticalOptions = LayoutOptions.Center;
                busyIndicator.HorizontalOptions = LayoutOptions.Fill;
                busyIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusyGrid");
                busyIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusyGrid");

                grid.HeightRequest = FSetting.HeightRowView;
                grid.VerticalOptions = LayoutOptions.CenterAndExpand;
                grid.SetBinding(Grid.IsVisibleProperty, "IsBusyGrid");
                grid.Children.Add(busyIndicator);
                grid.BindingContext = binding;
                return grid;
            });
        }

        public virtual DataTemplate TemplateGridSwipe(FToolbar[] toolbars, Func<object, FData, Task>[] actions, bool isLabel)
        {
            return new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    ColumnSpacing = 0,
                    RowSpacing = 0,
                    Margin = 0,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    RowDefinitions = { new RowDefinition { Height = GridLength.Star } }
                };

                for (int i = 0; i < toolbars.Length; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    grid.Children.Add(SwipeView(toolbars[i], actions[i], isLabel), i, 0);
                }
                return grid;
            });
        }

        public virtual View SwipeView(FToolbar toolbar, Func<object, FData, Task> action, bool isLabel)
        {
            var bt = new SfButton();
            var ic = new Image();
            var st = new StackLayout();
            var lc = false;

            ic.HorizontalOptions = LayoutOptions.Center;
            ic.VerticalOptions = LayoutOptions.Center;
            ic.WidthRequest = FSetting.SizeIconButton;

            bt.HorizontalOptions = LayoutOptions.FillAndExpand;
            bt.VerticalOptions = LayoutOptions.FillAndExpand;

            st.Padding = new Thickness(5, 5, 5, 5);
            st.Spacing = 5;
            st.HorizontalOptions = LayoutOptions.CenterAndExpand;
            st.VerticalOptions = LayoutOptions.CenterAndExpand;
            st.Children.Add(ic);
            if (isLabel)
            {
                var lb = new Label();
                ic.Source = toolbar.GetIcon(Color.White, FSetting.SizeIconButton);
                bt.BackgroundColor = toolbar.GetColor();

                lb.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
                lb.FontSize = FSetting.FontSizeLabelContent;
                lb.FontFamily = FSetting.FontText;
                lb.HorizontalTextAlignment = TextAlignment.Center;
                lb.VerticalTextAlignment = TextAlignment.Center;
                lb.HorizontalOptions = LayoutOptions.CenterAndExpand;
                lb.VerticalOptions = LayoutOptions.Center;
                lb.MaxLines = 2;
                lb.TextColor = Color.White;
                lb.Text = toolbar.Title;
                st.Children.Add(lb);
            }
            else
            {
                ic.Source = toolbar.GetIcon(FSetting.PrimaryColor, FSetting.SizeIconButton - 5);
                bt.BackgroundColor = Color.Transparent;
            }

            bt.Content = st;
            bt.Clicked += async (s, e) =>
            {
                lock (s) { if (lc) return; lc = true; }
                await action.Invoke(toolbar, bt.BindingContext as FData);
                lc = false;
            };
            return bt;
        }

        public virtual GridColumn GridColumnView(List<FField> detail, FField field)
        {
            var title = field.Title;
            var format = field.DataFormatString;

            GridColumn columns = field.FieldType switch { FieldType.Bool => new FCheckBoxColumn(title), FieldType.DateTime => new FDateColumn(title), FieldType.Number => new FNumericColumn(title), _ => new FTextColumn(title) };
            columns.Width = field.Width;
            columns.MappingName = FData.GetBindingName(field.Name);
            columns.TextAlignment = field.Align;

            if (!string.IsNullOrWhiteSpace(format))
            {
                if (FFunc.IsBinding(format)) columns.SetBinding(GridColumn.FormatProperty, "Details." + FData.GetBindingName(FFunc.ReplaceBinding(format)));
                else columns.Format = format;
            }

            if (FFunc.IsBinding(field.HiddenByKey)) columns.SetBinding(GridColumn.IsHiddenProperty, "Details." + FData.GetBindingName(FFunc.ReplaceBinding(field.HiddenByKey)));
            return columns;
        }

        public virtual View GridCustomWebView(object binding, string html, EventHandler<WebNavigatingEventArgs> OnNavigating)
        {
            var st = new StackLayout { Margin = 0, Padding = 0, Spacing = 0, BackgroundColor = Color.White };
            var we = new WebView();
            var ht = new HtmlWebViewSource { BindingContext = binding };

            ht.SetBinding(HtmlWebViewSource.HtmlProperty, html);
            we.Source = ht;
            we.HeightRequest = FSetting.ScreenHeight;
            we.WidthRequest = FSetting.ScreenWidth;
            we.Navigating += OnNavigating;
            st.Children.Add(we);
            return st;
        }

        public virtual View GridCustomView(List<FField> fields, List<FRow> rows, double maxWidth)
        {
            var it = new StackLayout();

            for (int i = 0; i < rows.Count; i++)
            {
                try
                {
                    var gr = new Grid();
                    var ro = rows[i];

                    gr.Margin = gr.RowSpacing = gr.ColumnSpacing = 0;
                    gr.Padding = new Thickness(0, 2);
                    gr.VerticalOptions = LayoutOptions.CenterAndExpand;
                    gr.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    if (ro.Value.Equals("@@line"))
                    {
                        it.Children.Add(new FLine(ro.Color));
                        continue;
                    }
                    var wi = FFunc.GetArrayString(ro.Width);
                    var al = FFunc.GetArrayString(ro.Align);
                    var va = ro.VAlign.Trim().ToLower() switch { "top" => TextAlignment.Start, "center" => TextAlignment.Center, "bottom" => TextAlignment.End, _ => TextAlignment.Start };
                    var st = FFunc.GetArrayString(ro.Style);
                    var co = FFunc.GetArrayString(ro.Color);
                    var fs = FFunc.GetArrayString(ro.FontSize);
                    var he = ro.Height;
                    var vs = FFunc.GetArrayString(ro.Value);
                    var li = ro.Line;
                    var hi = ro.Hidden;

                    if (FFunc.IsBinding(hi)) gr.SetBinding(Grid.IsVisibleProperty, FData.GetBindingName(FFunc.ReplaceBinding(hi)));

                    for (int j = 0; j < wi.Count; j++)
                    {
                        try
                        {
                            var rw = wi[j] switch { "*" => -1, "_" => -2, _ => double.Parse(wi[j]) };
                            var ra = al[j].ToLower() switch { "*" => TextAlignment.Start, "left" => TextAlignment.Start, "right" => TextAlignment.End, _ => TextAlignment.Center };
                            var ft = st[j].ToLower() switch { "*" => FSetting.FontText, "n" => FSetting.FontText, "b" => FSetting.FontTextBold, "u" => "@Underline", "s" => "@Strike", "m" => FSetting.FontTextMedium, _ => FSetting.FontTextItalic };
                            var cb = FFunc.IsBinding(co[j]) ? fields.Find(x => x.Name == FFunc.ReplaceBinding(co[j])) : null;
                            var rc = co[j] switch { "*" => FSetting.TextColorContent, _ => Color.FromHex(co[j]) };
                            var rf = fs[j] switch { "*" => FSetting.FontSizeLabelContent, _ => FSetting.FontSizeLabelContent + int.Parse(fs[j]) };
                            var vl = vs[j];
                            var tt = vl.Contains("Title");
                            var fo = vl.Contains("Footer");
                            var na = FFunc.ReplaceBinding(tt || fo ? vl.Split(".")[0] : vl);
                            var fi = fields.Find(x => x.Name == na);
                            var te = tt ? fi.Title : fo ? fi.SubTitle : "";
                            var bd = tt || fo ? "" : fi.Name;
                            var fm = tt || fo ? "" : fi.DataFormatString;
                            var fb = FFunc.IsBinding(fm) ? fields.Find(x => x.Name == FFunc.ReplaceBinding(fm)) : null;

                            AddLabel(ref gr, maxWidth, rw, ra, va, rc, cb, rf, he, ft, li, te, bd, fb, fm, j);
                        }
                        catch (Exception ex) { MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE); }
                    }
                    it.Spacing = 8; it.Margin = 0; it.Padding = 8; it.VerticalOptions = LayoutOptions.StartAndExpand; it.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    it.Children.Add(gr);
                }
                catch (Exception ex) { MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE); }
            }
            return it;
        }

        private void AddLabel(ref Grid grid, double maxWidth, double width, TextAlignment align, TextAlignment valign, Color textColor, FField color, int fontSize, double height, string font, int line, string text, string binding, FField format, string formatString, int index)
        {
            var lb = new FLabelListView
            {
                FontFamily = FSetting.FontText,
                FontSize = fontSize,
                HorizontalTextAlignment = align,
                VerticalTextAlignment = valign,
                MaxLines = line == -1 ? int.MaxValue : line,
                HeightRequest = height != 25 ? height : -1,
                Text = text,
                BindingName = binding
            };
            if (font.Equals("@Underline")) lb.TextDecorations = TextDecorations.Underline;
            else if (font.Equals("@Strike")) lb.TextDecorations = TextDecorations.Strikethrough;
            else lb.FontFamily = font;

            if (color == null) lb.TextColor = textColor;
            else lb.SetBinding(FLabelListView.TextColorProperty, FData.GetBindingName(color.Name), converter: new FColorConvert());

            if (format == null) lb.Format = formatString;
            else lb.SetBinding(FLabelListView.FormatProperty, FData.GetBindingName(format.Name));

            if (width == -1) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            else if (width == -2) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            else { var w = width * (maxWidth - 16) * 0.01; grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w) }); lb.WidthRequest = w; }
            grid.Children.Add(new StackLayout { Margin = 0, Padding = 0, VerticalOptions = LayoutOptions.StartAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, Children = { lb } }, index, 0);
        }
    }
}