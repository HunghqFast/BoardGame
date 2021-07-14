using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPicker : SfPopupLayout
    {
        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create("ItemSource", typeof(List<FPickerItem>), typeof(FPicker), new List<FPickerItem>());
        public static readonly BindableProperty SelectedProperty = BindableProperty.Create("Selected", typeof(object), typeof(FPicker), null);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(FPicker), null);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(FPicker));
        public static readonly BindableProperty PickerModeProperty = BindableProperty.Create("PickerMode", typeof(FPickerMode), typeof(FPicker), FPickerMode.Default);

        public List<FPickerItem> ItemSource { get => (List<FPickerItem>)GetValue(ItemSourceProperty); set => SetValue(ItemSourceProperty, value); }

        public object Selected { get => GetValue(SelectedProperty); set => SetValue(SelectedProperty, value); }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public FPickerMode PickerMode { get => (FPickerMode)GetValue(PickerModeProperty); set => SetValue(PickerModeProperty, value); }

        public event EventHandler Sucessed;

        public FPicker()
        {
            InitSettings();
        }

        private async void InitSettings()
        {
            Title = FText.Done;
            PopupView.ShowHeader = false;
            PopupView.ShowFooter = false;
            PopupView.WidthRequest = FSetting.ScreenWidth;
            PopupView.AnimationMode = AnimationMode.SlideOnBottom;
            PopupView.AnimationDuration = 200d;
            PopupView.PopupStyle.CornerRadius = 0;
            PopupView.PopupStyle.BorderThickness = 0;
            PopupView.PopupStyle.HasShadow = false;
            PopupView.ContentTemplate = PopupView.ContentTemplate = new DataTemplate(() =>
            {
                return new StackLayout();
            });
            Opened += (s, e) => { Selected = Value; };
            await Task.CompletedTask;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Selected):
                    var da = ItemSource.Find(x => x.Value.Equals(Selected));
                    if (da == null) break;
                    var db = ItemSource.Find(x => x.IsCheck == true);
                    if (db != null) db.IsCheck = false;
                    da.IsCheck = true;
                    break;
                default:
                    break;
            }
        }

        private StackLayout Stack(Thickness padd, double height)
        {
            return new StackLayout
            {
                Margin = 0,
                Spacing = 0,
                Padding = padd,
                HeightRequest = height,
                BackgroundColor = Color.White
            };
        }

        private void TitleClicked(object sender, EventArgs e)
        {
            Value = Selected;
            FFunc.CreateEventArgs(this, Sucessed, e);
            IsOpen = false;
        }

        private Grid StackItem(FPickerItem data)
        {
            var g = new Grid();
            var b = new Button();
            var c = new FCheckBox(false);

            g.BackgroundColor = Color.White;
            g.Padding = 0;
            g.HeightRequest = 45;
            g.Margin = 0;
            g.ColumnSpacing = 0;
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

            b.FontSize = FSetting.FontSizeButton;
            b.TextColor = FSetting.TextColorContent;
            b.FontFamily = FSetting.FontText;
            b.VerticalOptions = LayoutOptions.Center;
            b.BackgroundColor = Color.Transparent;
            b.BorderWidth = 0;
            b.HeightRequest = 40;
            b.BindingContext = data;
            b.SetBinding(Button.TextProperty, ValueProperty.PropertyName);
            b.Clicked += (s, e) =>
            {
                if (data.IsCheck) return;
                Selected = data.Value;
                if (PickerMode == FPickerMode.AutoCheck) TitleClicked(s, e);

            };

            c.BindingContext = data;
            c.SetBinding(FCheckBox.IsCheckedProperty, FData.GetBindingName(FData.CheckStatusName));
            c.SetBinding(FCheckBox.IsVisibleProperty, FData.GetBindingName(FData.CheckStatusName));

            g.Children.Add(b, 1, 0);
            g.Children.Add(c, 2, 0);
            return g;
        }

        public async void InitView()
        {
            PopupView.ContentTemplate = new DataTemplate(() => { return InitContent(); });
            await Task.CompletedTask;
        }

        private View InitContent()
        {
            var l = new StackLayout { Margin = 0, Spacing = 0, Padding = 0 };
            var s = Stack(new Thickness(10, 0, 10, 0), 40d);
            var c = Stack(0, ItemSource.Count * 46);
            var t = new Button();

            t.SetBinding(Button.TextProperty, TitleProperty.PropertyName);
            t.FontSize = FSetting.FontSizeButton;
            t.FontFamily = FSetting.FontText;
            t.VerticalOptions = LayoutOptions.Center;
            t.HorizontalOptions = LayoutOptions.End;
            t.BackgroundColor = Color.Transparent;
            t.BorderWidth = 0;
            t.WidthRequest = 80;
            t.HeightRequest = 40;
            t.BindingContext = this;

            if (PickerMode == FPickerMode.Default) t.Clicked += TitleClicked;
            else t.Clicked += Close;

            s.Children.Add(t);
            ItemSource.ForEach(i => { c.Children.Add(StackItem(i)); c.Children.Add(new FLine()); });
            PopupView.HeightRequest = s.HeightRequest + c.HeightRequest + 1;

            l.Children.Add(new FLine());
            l.Children.Add(s);
            l.Children.Add(new FLine());
            l.Children.Add(c);
            return l;
        }

        public async void Show()
        {
            Show(0, FSetting.ScreenHeight);
            await Task.CompletedTask;
        }

        private void Close(object sender, EventArgs e)
        {
            IsOpen = false;
        }
    }
}