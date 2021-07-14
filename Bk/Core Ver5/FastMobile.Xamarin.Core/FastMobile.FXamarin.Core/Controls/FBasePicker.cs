using Syncfusion.SfPicker.XForms;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBasePicker<T> : SfPicker
    {
        public Button OKButton;
        public Button CancelButton;
        public ObservableCollection<object> Data
        {
            get => ItemsSource as ObservableCollection<object>;
            set => ItemsSource = value;
        }

        public FBasePicker()
        {
            Data = new ObservableCollection<object>();
            OKButton = new Button();
            CancelButton = new Button();
            OKButton.Text = FText.Accept;
            CancelButton.Text = FText.Cancel;
            CancelButton.FontFamily = FSetting.FontText;
            OKButton.VerticalOptions = CancelButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            OKButton.HorizontalOptions = CancelButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
            OKButton.WidthRequest = CancelButton.WidthRequest = FSetting.FilterDatePickerWidth * 0.5;
            OKButton.BackgroundColor = CancelButton.BackgroundColor = FSetting.BackgroundMain;
            OKButton.TextColor = CancelButton.TextColor = FSetting.TextColorTitle;
            OKButton.FontSize = CancelButton.FontSize = FSetting.FontSizeLabelContent;
            OKButton.CornerRadius = CancelButton.CornerRadius = 0;
            OKButton.FontFamily = FSetting.FontText;
            FooterView = new StackLayout
            {
                Margin = 0,
                Padding = 0,
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                Children = { OKButton, CancelButton }
            };

            PopulateDataCollection();

            BackgroundColor = FSetting.BackgroundMain;
            HeaderTextColor = Color.White;
            HeaderFontSize = FSetting.FontSizeLabelTitle;
            HeaderHeight = 40;
            ShowFooter = true;
            ShowHeader = false;
            ShowColumnHeader = true;
            ColumnHeaderHeight = 40;
            HeaderBackgroundColor = FSetting.PrimaryColor;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            PickerHeight = 350;
            PickerMode = PickerMode.Dialog;
            ItemHeight = 50;
            PickerWidth = FSetting.FilterDatePickerWidth;
            SelectedItemTextColor = FSetting.PrimaryColor;
            UnSelectedItemTextColor = FSetting.TextColorContent;
            SelectedItemFontSize = FSetting.FontSizeLabelSelectedDate;
            UnSelectedItemFontSize = DeviceInfo.Platform == DevicePlatform.Android ? FSetting.FontSizeLabelSelectedDate - FSetting.FontSizePercent : FSetting.FontSizeLabelTitle;
            EnableLooping = true;
            ColumnHeaderText = InitHeader();
            ColumnHeaderTextColor = FSetting.TextColorTitle;
            ColumnHeaderFontSize = FSetting.FontSizeLabelTitle;
            ColumnHeaderBackgroundColor = FSetting.BackgroundMain;
            SelectionChanged += CustomDatePickerSelectionChanged;
        }

        public virtual void SetDataPicker(string value)
        {
        }

        public virtual T GetDataPicker()
        {
            return default;
        }

        protected virtual ObservableCollection<string> InitHeader()
        {
            return new ObservableCollection<string>();
        }

        protected virtual void UpdateData(Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
        }

        protected virtual void PopulateDataCollection()
        {
        }

        private void CustomDatePickerSelectionChanged(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            UpdateData(e);
        }
    }
}
