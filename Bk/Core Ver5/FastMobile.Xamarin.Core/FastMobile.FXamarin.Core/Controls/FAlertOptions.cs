using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FAlertOptions : FAlertBase
    {
        public static readonly BindableProperty OptionsSourceProperty = BindableProperty.Create("OptionsSource", typeof(IEnumerable<object>), typeof(FAlert));
        public static readonly BindableProperty ValuePathProperty = BindableProperty.Create("ValuePath", typeof(string), typeof(FAlert), "ID");
        public static readonly BindableProperty DisplayPathProperty = BindableProperty.Create("DisplayPath", typeof(string), typeof(FAlert), "Value");

        public IEnumerable<object> OptionsSource
        {
            get => (IEnumerable<object>)GetValue(OptionsSourceProperty);
            set => SetValue(OptionsSourceProperty, value);
        }

        public string ValuePath
        {
            get => (string)GetValue(ValuePathProperty);
            set => SetValue(ValuePathProperty, value);
        }

        public string DisplayPath
        {
            get => (string)GetValue(DisplayPathProperty);
            set => SetValue(DisplayPathProperty, value);
        }

        readonly StackLayout OptionsView;
        readonly FSfComboBox Dropdown;
        readonly FLine NewLine;

        public FAlertOptions() : base()
        {
            OptionsView = new StackLayout { BindingContext = this };
            Dropdown = new FSfComboBox { BindingContext = this };
            NewLine = new FLine();
            Base();
        }

        void Base()
        {
            MessageRow.Height = GridLength.Auto;
            SubViewRow.Height = GridLength.Star;
            Dropdown.SelectedIndex = 0;
            Dropdown.ShowBorder = false;
            Dropdown.DropDownTextSize = FSetting.FontSizeLabelContent;
            Dropdown.TextSize = FSetting.FontSizeLabelContent;
            Dropdown.FontFamily = FSetting.FontText;
            Dropdown.DropDownItemHeight = 40;
            Dropdown.DropDownCornerRadius = 2;
            Dropdown.SetBinding(FSfComboBox.DataSourceProperty, OptionsSourceProperty.PropertyName);
            Dropdown.SetBinding(FSfComboBox.SelectedValuePathProperty, ValuePathProperty.PropertyName);
            Dropdown.SetBinding(FSfComboBox.DisplayMemberPathProperty, DisplayPathProperty.PropertyName);

            OptionsView.Children.Add(Dropdown);
            OptionsView.Children.Add(NewLine);
            OptionsView.Padding = new Thickness(20, 0, 20, 10);
            SubView = OptionsView;
        }

        public async Task<string> ShowOptions(string message, IEnumerable<object> dataSource, string valuePath = "ID", string displayPath = "Value")
        {
            if (IsShowedOrCanotAlert())
                return string.Empty;
            BeforeLoadConfirm();
            ValuePath = valuePath;
            DisplayPath = displayPath;
            OptionsSource = dataSource;
            Load(false, "", message, FText.Yes, FText.No);
            var result = await WaitConfirm();
            return result ? Dropdown.SelectedValue?.ToString() : string.Empty;
        }

        public async Task<string> ShowOptions(string message, string acceptText, string cancelText, IEnumerable<object> dataSource, string valuePath = "ID", string displayPath = "Value")
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return string.Empty;
            BeforeLoadConfirm();
            ValuePath = valuePath;
            DisplayPath = displayPath;
            OptionsSource = dataSource;
            Load(false, "", message, acceptText, cancelText);
            var result = await WaitConfirm();
            return result ? Dropdown.SelectedValue?.ToString() : string.Empty;
        }

        public async Task<string> ShowOptions(string title, string message, string acceptText, string cancelText, IEnumerable<object> dataSource, string valuePath = "ID", string displayPath = "Value")
        {
            if (IsShowedOrCanotAlert() || this.IsNullOrEmpty(message, acceptText, cancelText))
                return string.Empty;
            BeforeLoadConfirm();
            ValuePath = valuePath;
            DisplayPath = displayPath;
            OptionsSource = dataSource;
            Load(false, title, message, acceptText, cancelText);
            var result = await WaitConfirm();
            return result ? Dropdown.SelectedValue?.ToString() : string.Empty;
        }

        protected override void Load(bool single, string title, string message, string accept, string cancel)
        {

            base.Load(single, title, message, accept, cancel);
        }
    }
}
