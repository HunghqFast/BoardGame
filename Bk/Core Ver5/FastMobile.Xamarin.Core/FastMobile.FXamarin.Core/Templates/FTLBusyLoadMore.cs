using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLBusyLoadMore : ViewCell
    {
        public FTLBusyLoadMore() : base()
        {
            View = GetView();
        }

        public View GetView(string path = "IsBusy")
        {
            var grid = new Grid();
            var busyIndicator = new ActivityIndicator
            {
                Color = FSetting.BusyColor,
                WidthRequest = FSetting.BusySize,
                HeightRequest = FSetting.BusySize,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill
            };
            busyIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, path);
            busyIndicator.SetBinding(ActivityIndicator.IsRunningProperty, path);
            grid.HeightRequest = FSetting.HeightRowView;
            grid.VerticalOptions = LayoutOptions.CenterAndExpand;
            grid.Children.Add(busyIndicator);
            grid.SetBinding(Grid.IsVisibleProperty, path);
            return grid;
        }
    }
}