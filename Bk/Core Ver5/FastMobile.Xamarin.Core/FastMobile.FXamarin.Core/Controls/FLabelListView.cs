using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLabelListView : Label
    {
        public static readonly BindableProperty FormatProperty = BindableProperty.Create("Format", typeof(string), typeof(Label));
        public static readonly BindableProperty BindingNameProperty = BindableProperty.Create("BindingName", typeof(string), typeof(Label));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public string BindingName
        {
            get { return (string)GetValue(BindingNameProperty); }
            set { SetValue(BindingNameProperty, value); }
        }

        public FLabelListView()
        {
            Margin = 0;
            VerticalOptions = LayoutOptions.StartAndExpand;
            LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            Padding = new Thickness(0, 0, 1, 0);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Format):
                    if (!string.IsNullOrEmpty(BindingName))
                    {
                        if (Format == "") this.SetBinding(FLabelListView.TextProperty, FData.GetBindingName(BindingName));
                        else if (Format == "X" || Format == "x" || Format == "U" || Format == "u") this.SetBinding(FLabelListView.TextProperty, FData.GetBindingName(BindingName), converter: new FStringStyleConvert(Format));
                        else this.SetBinding(FLabelListView.TextProperty, FData.GetBindingName(BindingName), stringFormat: "{0:" + Format + "}");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}