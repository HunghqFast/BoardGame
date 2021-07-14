using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSfChart : SfChart
    {
        public static readonly BindableProperty TypeProperty = BindableProperty.Create("Type", typeof(FChartType), typeof(FSfChart), FChartType.Bar);

        public FChartType Type
        {
            get => (FChartType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
    }
}