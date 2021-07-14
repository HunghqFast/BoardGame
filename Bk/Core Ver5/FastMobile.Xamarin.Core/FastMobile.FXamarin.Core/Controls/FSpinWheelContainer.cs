using Syncfusion.SfChart.XForms;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace FastMobile.FXamarin.Core
{
    public class FSpinWheelContainer : SfChart, IFSpinWheelControl
    {
        private bool isSpin, isSpinOn;

        public const int minRound = 2, maxRound = 5;
        public const double spinAngle = 4, initAngle = 270;

        public static readonly BindableProperty DataProperty = BindableProperty.Create("Data", typeof(FSpinWheelListData), typeof(FSpinWheelContainer));

        public FSpinWheelListData Data { get => (FSpinWheelListData)GetValue(DataProperty); set => SetValue(DataProperty, value); }

        public DoughnutSeries SpinSeries { get; set; }
        public AbsoluteLayout CenterView;
        public Button Button;
        public Polygon Pointer;

        public event EventHandler<FSpinWheelEventArgs> SpinClicked;

        public FSpinWheelContainer()
        {
            BackgroundColor = Color.WhiteSmoke;
            SpinSeries = new DoughnutSeries();
            CenterView = new AbsoluteLayout();
            Button = new Button();
            Pointer = new Polygon();
            SeriesRendered -= FSpinWheelContainerSeriesRendered;
            SeriesRendered += FSpinWheelContainerSeriesRendered;
        }

        public void Render()
        {
            InitSpin();
            InitSeries();
        }

        #region Private

        private void InitSpin()
        {
            Title.FontSize = 20;
            Title.TextAlignment = TextAlignment.Center;
        }

        private void InitSeries()
        {
            var data = new FSpinWheelListData(Data.Reverse());
            SpinSeries.ItemsSource = data;
            SpinSeries.ColorModel = new ChartColorModel
            {
                Palette = ChartColorPalette.Custom,
                CustomBrushes = data.GetColor()
            };
            SpinSeries.DataMarker = new ChartDataMarker
            {
                ShowLabel = true,
                LabelStyle = new DataMarkerLabelStyle
                {
                    LabelPosition = DataMarkerLabelPosition.Auto
                }
            };
            SpinSeries.XBindingPath = FSpinWheelData.NameProperty.PropertyName;
            SpinSeries.YBindingPath = FSpinWheelData.ValueProperty.PropertyName;
            SpinSeries.DataMarkerPosition = CircularSeriesDataMarkerPosition.Outside;
            SpinSeries.DataMarkerLabelCreated -= FSpinWheelContainerDataMarkerLabelCreated;
            SpinSeries.DataMarkerLabelCreated += FSpinWheelContainerDataMarkerLabelCreated;
            SpinCirle(initAngle);
            Series.Add(SpinSeries);
        }

        private void FSpinWheelContainerSeriesRendered(object sender, EventArgs e)
        {
            InitCenterView();
        }

        private void FSpinWheelContainerDataMarkerLabelCreated(object sender, ChartDataMarkerLabelCreatedEventArgs e)
        {
            e.DataMarkerLabel.Label = (e.DataMarkerLabel.Data as FSpinWheelData).Name;
        }

        private void CenterViewClicked(object sender, EventArgs e)
        {
            lock (sender) if (isSpin) return;
            //SpinSeries.DataMarker.ShowLabel = false;
            SpinClicked?.Invoke(this, new FSpinWheelEventArgs(OnSpinAsync));
            //SpinSeries.DataMarker.ShowLabel = true;
        }

        private void InitCenterView()
        {
            var size = Width * SpinSeries.DoughnutCoefficient * SpinSeries.CircularCoefficient;
            var yPointer = size * 0.2;
            var xPointer = size * 0.2;

            Button.FontAttributes = FontAttributes.Bold;
            Button.HeightRequest = size;
            Button.WidthRequest = size;
            Button.CornerRadius = (int)Math.Round(size * 0.5);
            Button.Clicked -= CenterViewClicked;
            Button.Clicked += CenterViewClicked;

            Pointer.RotationX = 180;
            Pointer.Points = new PointCollection { new Point((size - xPointer) * 0.5, 0), new Point(size * 0.5, yPointer), new Point((size + xPointer) * 0.5, 0) };
            Pointer.Margin = new Thickness(0, -yPointer + Button.BorderWidth + 1, 0, 0);
            Pointer.BindingContext = this;

            CenterView.Children.Add(Button);
            CenterView.Children.Add(Pointer);
            SpinSeries.CenterView = CenterView;
        }

        private async Task<int> OnSpinAsync(Func<Task<(bool Ok, string Award)>> getAward)
        {
            if (Data.Count == 0)
                return -1;

            isSpin = true;

            SpinOn();
            await Task.Delay(200);
            var result = await getAward();
            SpinOff();

            if (!result.Ok)
            {
                isSpin = false;
                return -2;
            }

            var award = Data.GetItem(result.Award);
            var rAward = Data.IndexOf(award);

            var angle = 360d / Data.Count;
            var start = SpinSeries.StartAngle;
            var extend = 270 - start % 360;

            var rRound = new Random().Next(minRound, maxRound);
            var rPosition = new Random().NextDouble() * angle;
            var end = start + rRound * 360 + angle * rAward + rPosition + (extend >= 0 ? extend : 360 + extend);

            await AutoSpin(start, end);
            isSpin = false;
            return await Task.FromResult(rAward);
        }

        private async Task AutoSpin(double start, double end)
        {
            double spin;
            while (start < end)
            {
                spin = start + spinAngle <= end ? spinAngle : end - start;
                await Task.Delay(3);
                SpinCirle(spin);
                start += spin;
            }
        }

        private void SpinCirle(double angle)
        {
            SpinSeries.StartAngle += angle;
            SpinSeries.EndAngle += angle;
        }

        private async void SpinOn()
        {
            isSpinOn = true;
            while (isSpinOn)
            {
                SpinCirle(spinAngle);
                await Task.Delay(3);
            }
        }

        private void SpinOff()
        {
            isSpinOn = false;
        }

        #endregion Private
    }
}