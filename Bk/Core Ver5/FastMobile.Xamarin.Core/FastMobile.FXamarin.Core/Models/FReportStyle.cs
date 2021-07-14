using Syncfusion.ListView.XForms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FReportStyle
    {
        public virtual View InitHtmlView(object binding, string name, bool isLabel = true, double height = 40d)
        {
            if (isLabel)
            {
                var lb = new Label();
                var sr = new ScrollView();

                lb.FontSize = FSetting.FontSizeLabelContent;
                lb.VerticalOptions = LayoutOptions.CenterAndExpand;
                lb.HorizontalOptions = LayoutOptions.StartAndExpand;
                lb.VerticalTextAlignment = TextAlignment.Center;
                lb.TextType = TextType.Html;
                lb.HeightRequest = height;
                lb.MaxLines = 1;
                lb.BindingContext = binding;
                lb.SetBinding(Label.TextProperty, name);

                sr.Content = lb;
                sr.Orientation = ScrollOrientation.Horizontal;
                sr.Padding = new Thickness(10, 0);
                return sr;
            }

            var gr = new Grid();
            var we = new WebView();
            var hs = new HtmlWebViewSource();

            hs.SetBinding(HtmlWebViewSource.HtmlProperty, name);
            hs.BindingContext = binding;

            we.Source = hs;
            we.WidthRequest = FSetting.ScreenWidth;

            gr.ColumnSpacing = 0;
            gr.RowSpacing = 0;
            gr.Margin = 25;
            gr.Padding = 25;
            gr.MinimumHeightRequest = 0;
            gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gr.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gr.Children.Add(we, 0, 0);
            return gr;
        }

        public virtual void InitBaseView(ref ContentView baseView, object binding, string bindingView, string bindingVisible)
        {
            baseView = new ContentView
            {
                BindingContext = binding
            };
            baseView.SetBinding(ContentView.ContentProperty, bindingView);
            baseView.SetBinding(ContentView.IsVisibleProperty, bindingVisible);
        }

        public virtual View InitPagingDefaultView(FGridBase grid)
        {
            var pg = new FPaging
            {
                BindingContext = grid
            };
            pg.SetBinding(FPaging.TriggerRefreshProperty, "TriggerRefresh", mode: BindingMode.TwoWay);
            pg.SetBinding(FPaging.ListItemProperty, "ListItem");
            pg.SetBinding(FPaging.ListPagingProperty, "ListPaging");
            pg.SetBinding(FPaging.ItemPickerProperty, "ItemPerPage", mode: BindingMode.TwoWay);
            pg.SetBinding(FPaging.PageStatePickerProperty, "PageIndex", mode: BindingMode.TwoWay);
            return pg;
        }

        public virtual View InitPagingLoadmoreView(FGridBase grid)
        {
            static Span Sp(string text, Color force, string binding = "")
            {
                var span = new Span
                {
                    Text = text,
                    FontSize = FSetting.FontSizeLabelContent,
                    ForegroundColor = force,
                    FontFamily = FSetting.FontText
                };
                if (binding != "") span.SetBinding(Span.TextProperty, binding);
                return span;
            }
            var st = new StackLayout();
            var lb = new Label();
            var fs = new FormattedString();

            fs.Spans.Add(Sp(FText.ReportTotal, Color.FromHex("#545555")));
            fs.Spans.Add(Sp("", Color.FromHex("#000080"), "TotalItem"));
            fs.Spans.Add(Sp(FText.ReportRecord, Color.FromHex("#545555")));

            lb.FormattedText = fs;
            lb.FontSize = FSetting.FontSizeLabelContent;
            lb.FontFamily = FSetting.FontText;
            lb.VerticalOptions = LayoutOptions.CenterAndExpand;
            lb.VerticalTextAlignment = TextAlignment.Center;
            lb.LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            lb.BindingContext = grid;

            st.Margin = 0;
            st.Spacing = 0;
            st.Padding = new Thickness(10, 0, 10, 0);
            st.BackgroundColor = Color.FromHex("#FEFFFF");
            st.HeightRequest = 40;
            st.Orientation = StackOrientation.Horizontal;
            st.Children.Add(lb);
            return st;
        }

        public virtual View InitToolbarView(ObservableCollection<FMenuButtonReport> menu, double height, double width, int iconWidth, bool isTitle = true, bool isTitleLeft = false, bool hasPadding = true, int titleSize = -1, Color titleColor = default)
        {
            var lt = menu.Where(x => x.Toolbar.IsLeft)?.ToList();
            var rt = menu.Where(x => x.Toolbar.IsRight)?.ToList();

            if (rt.Count == 0)
            {
                return CreateToolbar(menu, height, width, iconWidth, isTitle, isTitleLeft, hasPadding, titleSize, titleColor);
            }
            else
            {
                var tb = new Grid();
                var le = CreateToolbar(lt, height, width, iconWidth, isTitle, isTitleLeft, hasPadding, titleSize, titleColor);
                var ri = CreateToolbar(rt, height, width, iconWidth, isTitle, isTitleLeft, hasPadding, titleSize, titleColor);
                tb.Margin = tb.Padding = tb.ColumnSpacing = 0;
                tb.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                tb.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                tb.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                tb.Children.Add(le, 0, 0);
                tb.Children.Add(ri, 1, 0);
                return tb;
            }
        }

        public virtual View InitAttachmentView(ObservableCollection<FMenuButtonReport> menu, int height)
        {
            var view = new SfListView();
            var busy = false;
            view.ItemsSource = menu;
            view.HeightRequest = height;
            view.IsScrollBarVisible = false;
            view.AutoFitMode = AutoFitMode.DynamicHeight;
            view.Orientation = Orientation.Horizontal;
            view.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            view.ItemTemplate = new DataTemplate(typeof(FTLAttachment));
            view.ItemSpacing = new Thickness(5, 0);
            view.ItemTapped += async (s, e) =>
            {
                lock (s) { if (busy) return; busy = true; }
                busy = false;

                if (e.ItemData is not FMenuButtonReport btn)
                    return;
                await btn.Action?.Invoke(btn.Toolbar);
            };
            return view;
        }

        private View CreateToolbar(IEnumerable<FMenuButtonReport> menu, double height, double width, int iconWidth, bool isTitle = true, bool isTitleLeft = false, bool hasPadding = true, int titleSize = -1, Color titleColor = default)
        {
            var sr = new ScrollView();
            var st = new StackLayout();

            st.Spacing = FSetting.SpacingButtons;
            st.Padding = hasPadding ? new Thickness(10, 0) : st.Padding;
            st.HeightRequest = height;
            st.Orientation = StackOrientation.Horizontal;

            menu.ForEach(x => CreateButton(st, x, width, iconWidth, isTitle, isTitleLeft, titleSize, titleColor == default ? FSetting.PrimaryColor : titleColor));

            sr.Content = st;
            sr.Orientation = ScrollOrientation.Horizontal;
            sr.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
            return sr;
        }

        private void CreateButton(StackLayout stack, FMenuButtonReport button, double width, int iconWidth, bool isTitle, bool isTitleLeft, int titleSize, Color titleColor)
        {
            var il = false;
            var btn = NewFButton(button.Toolbar.Title, button.Toolbar.GetIcon(button.Enable ? FSetting.PrimaryColor : FSetting.DisableColor, iconWidth), isTitle, isTitleLeft);
            btn.WidthRequest = width;
            btn.TextColor = titleColor;
            btn.IsVisible = button.Visible;
            btn.IsEnabled = button.Enable;
            if (titleSize != -1) btn.FontSize = titleSize;
            if (button.Toolbar.IsBorder)
            {
                btn.BorderThickness = 1;
                btn.BorderColor = FSetting.SecondaryColor;
                btn.CornerRadius = 5;
                btn.Margin = new Thickness(0, 3);
            }
            btn.Clicked += async (s, e) =>
            {
                lock (s) { if (il) return; il = true; }
                if (button.Enable) await button.Action?.Invoke(button.Toolbar);
                il = false;
            };
            stack.Children.Add(btn);
        }

        private FButton NewFButton(string text, ImageSource icon, bool isTitle, bool isTitleLeft)
        {
            return isTitleLeft ? new FButton(isTitle ? text : string.Empty, icon) : new FButton(isTitle ? text : string.Empty, icon) { ImageAlignment = Syncfusion.XForms.Buttons.Alignment.Top, HorizontalTextAlignment = TextAlignment.Center };
        }
    }
}