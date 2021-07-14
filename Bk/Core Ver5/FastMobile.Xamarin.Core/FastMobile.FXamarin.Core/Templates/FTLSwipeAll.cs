using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLSwipeAll : Grid
    {
        public FTLSwipeAll(Func<object, Task> tabbedFirst, Func<object, Task> tabbedLast, ImageSource iconImageFirst = null, ImageSource iconImageLast = null, string textFirst = null, string textLast = null, string leftBackground = "#C8C7CF", string rightBackground = "#F96267", bool isBinding = false) : base()
        {
            Init();
            Children.Add(SwipeView(tabbedFirst, iconImageFirst, true, textFirst, leftBackground, rightBackground, isBinding), 0, 0);
            Children.Add(SwipeView(tabbedLast, iconImageLast, false, textLast, leftBackground, rightBackground, isBinding), 1, 0);
        }

        public FTLSwipeAll(Action<object> tabbedFirst, Action<object> tabbedLast, ImageSource iconImageFirst = null, ImageSource iconImageLast = null, string textFirst = null, string textLast = null, string leftBackground = "#C8C7CF", string rightBackground = "#F96267", bool isBinding = false) : base()
        {
            Init();
            Children.Add(SwipeView(tabbedFirst, iconImageFirst, true, textFirst, leftBackground, rightBackground, isBinding), 0, 0);
            Children.Add(SwipeView(tabbedLast, iconImageLast, false, textLast, leftBackground, rightBackground, isBinding), 1, 0);
        }

        private void Init()
        {
            CompressedLayout.SetIsHeadless(this, true);
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnSpacing = 0;
        }

        private View SwipeView(Func<object, Task> tabbed, ImageSource iconImage, bool isLeft, string text, string leftBackground, string rightBackground, bool isBinding)
        {
            return InitSwipeView(0, iconImage, isLeft, text, leftBackground, rightBackground, null, tabbed, null, isBinding);
        }

        private View SwipeView(Action<object> tabbed, ImageSource iconImage, bool isLeft, string text, string leftBackground, string rightBackground, bool isBinding)
        {
            return InitSwipeView(0, iconImage, isLeft, text, leftBackground, rightBackground, null, null, tabbed, isBinding);
        }

        private View InitSwipeView(int type, ImageSource iconImage, bool isLeft, string text, string leftBackground, string rightBackground, Func<Task> task, Func<object, Task> obj, Action<object> invoke, bool isBinding)
        {
            var gr = new Grid();
            var st = new StackLayout();
            var ic = new Image();

            gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            gr.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gr.BackgroundColor = st.BackgroundColor = isLeft ? Color.FromHex(leftBackground) : Color.FromHex(rightBackground);
            gr.HorizontalOptions = gr.VerticalOptions = LayoutOptions.Fill;

            st.Orientation = StackOrientation.Vertical;
            st.VerticalOptions = LayoutOptions.Center;
            st.HorizontalOptions = LayoutOptions.Fill;
            st.Padding = new Thickness(3);

            if (!isBinding)
                ic.Source = iconImage;
            else
                ic.SetBinding(Image.SourceProperty, isLeft ? FItemNotify.CheckIconProperty.PropertyName : FItemNotify.RemoveIconProperty.PropertyName);

            ic.HorizontalOptions = LayoutOptions.Center;
            ic.VerticalOptions = LayoutOptions.Center;
            if (type == 0) ic.WidthRequest = FSetting.SizeIconButton;
            st.Children.Add(ic);

            if (!string.IsNullOrEmpty(text) || isBinding)
            {
                var tb = new Label();
                tb.LineBreakMode = LineBreakMode.TailTruncation;
                tb.FontSize = FSetting.FontSizeLabelContent;
                tb.FontFamily = FSetting.FontText;
                tb.HorizontalTextAlignment = TextAlignment.Center;
                tb.VerticalTextAlignment = TextAlignment.Center;
                tb.HorizontalOptions = LayoutOptions.CenterAndExpand;
                tb.VerticalOptions = LayoutOptions.Center;
                tb.MaxLines = 2;
                tb.TextColor = Color.White;
                if (!isBinding)
                    tb.Text = text;
                else
                    tb.SetBinding(Label.TextProperty, isLeft ? FItemNotify.CheckTextProperty.PropertyName : FItemNotify.RemoveTextProperty.PropertyName);
                st.Children.Add(tb);
            }
            gr.Children.Add(new FButtonEffect(gr, (x, y) => Invoke(x, y, task, obj, invoke)) { Content = st }, 0, 0);
            return gr;
        }

        private async void Invoke(object sender, IFDataEvent e, Func<Task> task, Func<object, Task> obj, Action<object> invoke)
        {
            if (task != null)
            {
                await task.Invoke();
                return;
            }

            if (obj != null)
            {
                await obj.Invoke(e.ItemData);
                return;
            }

            invoke.Invoke(e.ItemData);
        }
    }
}