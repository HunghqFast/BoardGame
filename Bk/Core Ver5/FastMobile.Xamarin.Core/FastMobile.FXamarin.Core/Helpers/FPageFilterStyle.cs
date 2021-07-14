using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using static Xamarin.Forms.Grid;

namespace FastMobile.FXamarin.Core
{
    public class FPageFilterStyle
    {
        private readonly IFPageFilter page;

        public FPageFilterStyle(IFPageFilter filter)
        {
            this.page = filter;
        }

        #region Public

        public virtual void InitInputView()
        {
            var v = page.Settings.Views.Count > 0;
            page.InputView.Content ??= v ? new StackLayout { Padding = 10, Spacing = 10 } : new StackLayout { Padding = 5, Spacing = 0 };
            var s = page.InputView.Content as StackLayout;
            s.Children.Clear();

            if (v) InitInputViewBySettings(s, 43.5);
            else InitInputViewAuto(s);
        }

        public virtual void InitInputViewAuto(StackLayout form)
        {
            page.Settings.Fields.ForEach(f => GetInputAuto(form, f));
        }

        public virtual void InitInputViewBySettings(StackLayout f, double removeSize)
        {
            page.Settings.Views.ForIndex((view, i) =>
            {
                var bd = new FBorderVisibleByInputs() { BorderColor = FSetting.LineBoxReportColor, CornerRadius = 5 };
                var ct = new StackLayout() { Spacing = 0, Orientation = StackOrientation.Vertical };
                if (string.IsNullOrEmpty(view.Title)) bd.Content = ct;
                else
                {
                    var ex = new FExpander { Index = i };
                    ex.Header.Text = view.Title;
                    if (page.Settings.ExpandCache) ex.IsExpanded = GetExpanderStatus(i);
                    else
                    {
                        ex.BindingContext = view;
                        ex.SetBinding(FExpander.IsExpandedProperty, "IsExpand", BindingMode.OneWay, new FIsExpanderConvert(page));
                    }

                    ex.Tapped += ExpandedCollapsed;
                    ex.ContentTemplate = new DataTemplate(() => ct);
                    bd.Content = ex;
                    if (ex.IsExpanded) ex.RenderContent();
                }

                if (view.Line && !view.Title.Equals("")) ct.Children.Add(new FLine());

                GetRowInputByRows(ct.Children, view.Row, removeSize, bd);

                bd.BindingContext = view;
                bd.SetBinding(View.IsVisibleProperty, "Hidden", converter: new FHiddenFilterExpanderConvert(page));
                f.Children.Add(bd);
            });
        }

        public virtual View InitButtonsView(List<FButton> buttons, List<EventHandler<EventArgs>> events)
        {
            var sr = new ScrollView();
            var gr = new Grid();

            buttons.ForIndex((b, i) =>
            {
                b.Clicked += events[i];
                gr.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gr.Children.Add(b, i, 0);
            });

            gr.ColumnSpacing = 10;
            gr.Padding = new Thickness(10, 0);
            gr.RowDefinitions.Add(new RowDefinition { Height = 48 });

            sr.Content = gr;
            sr.Orientation = ScrollOrientation.Horizontal;
            return sr;
        }

        #endregion Public

        #region Private

        private void ExpandedCollapsed(object sender, EventArgs e)
        {
            if (sender is FExpander ex) ex.IsExpanded.SetCache(page.ExpanderStatus.Replace("[index]", ex.Index.ToString()));
        }

        private bool GetExpanderStatus(int index)
        {
            var st = page.ExpanderStatus.Replace("[index]", index.ToString()).GetCache();
            return string.IsNullOrEmpty(st) || FFunc.StringToBoolean(st);
        }

        private void GetRowInputByRows(IList<View> f, List<FRow> rows, double removeSize, FBorderVisibleByInputs bd)
        {
            rows.ForEach(row =>
            {
                try
                {
                    var gr = new Grid() { Margin = 0, ColumnSpacing = 0, RowSpacing = 0, RowDefinitions = { new RowDefinition { Height = GridLength.Auto } } };
                    var wi = FFunc.GetArrayString(row.Width);
                    var vl = FFunc.GetArrayString(row.Value);

                    wi.ForIndex((s, j) =>
                    {
                        try
                        {
                            var rw = wi[j].Trim() switch { "*" => -1, "_" => -2, _ => double.Parse(wi[j].Trim()) };
                            var fi = page.Settings.Fields.Find(x => x.Name == FFunc.ReplaceBinding(vl[j].Trim()));

                            GetInputBySettings(gr, fi, rw, j, wi.Count == 1, removeSize, bd);
                        }
                        catch (Exception ex) { MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE); }
                    });

                    if (gr.Children.Count > 0)
                    {
                        f.Add(gr);
                        f.Add(new FLine());
                        SetVisibleLine(gr.Children, f[^1], gr.Children.Count == 1);
                    }
                }
                catch (Exception ex) { MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE); }
            });

            f.RemoveAt(f.Count - 1);
        }

        private void SetVisibleLine(IGridList<View> childs, VisualElement li, bool single)
        {
            if (single)
            {
                li.IsVisible = childs.ElementAt(0).IsVisible;
                li.BindingContext = childs.ElementAt(0);
                li.SetBinding(VisualElement.IsVisibleProperty, new MultiBinding { Bindings = { new Binding(VisualElement.IsVisibleProperty.PropertyName), new Binding(FInput.IsShowLineProperty.PropertyName) }, Converter = new FMultiBoolConvert() });
                return;
            }

            li.IsVisible = childs.ToList().Find(x => x.IsVisible) != null;
            childs.ForEach(x =>
            {
                x.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
                        li.IsVisible = childs.ToList().Find(x => x.IsVisible) != null;
                };
            });
        }

        private void GetInputAuto(StackLayout form, FField field)
        {
            var ip = page.Method.GetFInput(page.Input[field.Name]);
            ip.IsVisible = !field.Hidden && field.OnFilter;
            SetKeyBoard(field, ip);
            form.Children.Add(ip);
            form.Children.Add(new FLine() { BindingContext = ip });
            form.Children[^1].SetBinding(VisualElement.IsVisibleProperty, new MultiBinding { Bindings = { new Binding(VisualElement.IsVisibleProperty.PropertyName), new Binding(FInput.IsShowLineProperty.PropertyName) }, Converter = new FMultiBoolConvert() });
        }

        private void GetInputBySettings(Grid grid, FField field, double width, int index, bool isSignle, double removeSize, FBorderVisibleByInputs bd)
        {
            var i = page.Method.GetFInput(page.Input[field.Name]);
            var c = width == -1 || width == -2;
            var v = width * (FSetting.ScreenWidth - ((i.HasPadding ? (i.PaddingSize * 2) : 0) + removeSize)) * 0.01;

            i.Expander = bd;
            SetKeyBoard(field, i);
            if (isSignle)
            {
                i.ColumnDefinitions[1].Width = c ? GridLength.Star : new GridLength(v, GridUnitType.Absolute);
                bd.AddInput(i);
                i.IsVisible = !field.Hidden && field.OnFilter;
                grid.Children.Add(i, index, 0);
                return;
            }

            if (c)
            {
                if (page.IsMaster)
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, i.IsVisible ? GridUnitType.Star : GridUnitType.Auto) });
                else
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, !field.Hidden ? GridUnitType.Star : GridUnitType.Auto) });

                i.IsVisibleChanged += (s, e) =>
                {
                    grid.ColumnDefinitions[index].Width = new GridLength(1, e.Value ? GridUnitType.Star : GridUnitType.Auto);
                };
            }
            else
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                i.ColumnDefinitions[1].Width = v;
            }
            bd.AddInput(i);
            i.IsVisible = !field.Hidden && field.OnFilter;
            grid.Children.Add(i, index, 0);
        }

        private void SetKeyBoard(FField f, FInput i)
        {
            switch (f.ItemStyle)
            {
                case FItemStyle.AutoComplete: i.Keyboard = Keyboard.Create(KeyboardFlags.All); break;
            }
        }

        #endregion Private
    }
}