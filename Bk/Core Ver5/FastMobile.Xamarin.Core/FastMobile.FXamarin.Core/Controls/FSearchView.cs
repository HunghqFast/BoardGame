using Syncfusion.XForms.Border;
using Syncfusion.XForms.Buttons;
using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSearchView : StackLayout
    {
        public readonly View Icon, Cancel, Field;
        public event EventHandler<FSearchEventArgs> SearchBarTextChanged;
        public event EventHandler<FSearchEventArgs> SearchBarTextSubmit;

        private ImageButton icon => Icon as ImageButton;
        private SfButton cancel => Cancel as SfButton;
        private FEntryBase field => Field as FEntryBase;
        private readonly SfBorder Border;
        string placeHolder = string.Empty;
        public FSearchView() : base()
        {
            Icon = new ImageButton();
            Cancel = new SfButton();
            Field = new FEntryBase();
            Border = new SfBorder();
            Base();
        }

        private void Base()
        {
            icon.Margin = new Thickness(5, 0);
            icon.Source = FIcons.Magnify.ToFontImageSource(FSetting.BackgroundMain, FSetting.SizeIconLegend);
            icon.BackgroundColor = Color.Transparent;
            icon.Clicked += OnSearchTextSubmit;

            cancel.CornerRadius = 5;
            cancel.FontFamily = FSetting.FontText;
            cancel.FontSize = FSetting.FontSizeLabelTitle;
            cancel.HeightRequest = 40;
            cancel.Margin = new Thickness(10, 0);
            cancel.IsVisible = false;
            cancel.Text = FText.Cancel;
            cancel.BackgroundColor = Color.Transparent;
            cancel.TextColor = FSetting.BackgroundMain;
            cancel.Clicked += OnCancelClicked;

            field.Margin = new Thickness(5, 0);
            field.TextColor = FSetting.BackgroundMain;
            field.FontFamily = FSetting.FontText;
            field.FontSize = FSetting.FontSizeLabelContent;
            field.ReturnType = ReturnType.Search;
            field.Focused += OnFieldFocus;
            field.Unfocused += OnFieldUnFocus;
            field.TextChanged += OnSearchTextChanged;
            field.Completed += OnSearchTextSubmit;

            Border.Content = field;
            Border.BorderWidth = 0;
            Border.BorderColor = Color.Transparent;
            Border.CornerRadius = 8;
            Border.HorizontalOptions = LayoutOptions.FillAndExpand;
            Border.BackgroundColor = Color.FromHex("#25b3b3b3");

            Spacing = 10;
            Padding = new Thickness(10);
            Background = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { new GradientStop { Offset = 0.0f, Color = FSetting.StartColor }, new GradientStop { Offset = 1.0f, Color = FSetting.EndColor } } };
            Orientation = StackOrientation.Horizontal;
            Children.Add(icon);
            Children.Add(Border);
            Children.Add(cancel);
        }

        private void OnSearchTextSubmit(object sender, EventArgs e)
        {
            SearchBarTextSubmit?.Invoke(this, new FSearchEventArgs(field.Text));
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBarTextChanged?.Invoke(this, new FSearchEventArgs(e.NewTextValue));
        }

        private void OnFieldFocus(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(field.Text))
            {
                placeHolder = field.Placeholder;
                field.Placeholder = string.Empty;
            }
            cancel.TranslateTo(0, 0, easing: Easing.Linear);
            cancel.IsVisible = true;
        }

        private void OnFieldUnFocus(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(field.Text))
            {
                cancel.TranslateTo(cancel.Width, 0, easing: Easing.Linear);
                cancel.IsVisible = false;
                field.Placeholder = placeHolder;
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            cancel.TranslateTo(cancel.Width, 0, easing: Easing.Linear);
            cancel.IsVisible = false;
            field.Unfocus();
            field.Text = "";
            field.Placeholder = placeHolder;
        }
    }
}
