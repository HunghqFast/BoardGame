using FastMobile.FXamarin.Core;
using Xamarin.Forms;

namespace FastMobile.Core
{
    internal class OverViewModel : BindableObject
    {
        public static readonly BindableProperty ViewProperty = BindableProperty.Create("View", typeof(FDBase), typeof(OverViewModel));

        public FDBase View
        {
            get => (FDBase)GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public string ID { get; } = FUtility.GetRandomString();
        public string Controller { get; set; }
        public FChartType ChartType { get; set; }
        public FOverviewType ViewType { get; set; }
    }
}