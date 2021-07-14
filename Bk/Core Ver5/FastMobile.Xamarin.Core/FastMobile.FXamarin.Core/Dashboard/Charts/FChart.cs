using Syncfusion.SfChart.XForms;
using Syncfusion.XForms.Border;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public abstract class FChart : FDBase
    {
        protected FCDashboard Model => ViewModel as FCDashboard;
        protected FSfChart Chart { get; private set; }

        protected ChartLegend Legend { get; private set; }
        protected ChartTooltipBehavior ToolTip { get; private set; }
        protected ChartZoomPanBehavior Zoom { get; private set; }
        protected ChartTrackballBehavior TrackBall { get; private set; }
        internal ObservableCollection<bool> ListIsVisibleSeries { get; }

        public FChart() : base()
        {
            ListIsVisibleSeries = new ObservableCollection<bool>();
            Legend = new ChartLegend();
            ToolTip = new ChartTooltipBehavior();
            Zoom = new ChartZoomPanBehavior();
            TrackBall = new ChartTrackballBehavior();
            Chart = new FSfChart() { Legend = Legend };
            Chart.ChartBehaviors.Add(ToolTip);
            Chart.ChartBehaviors.Add(Zoom);
            Chart.ChartBehaviors.Add(TrackBall);
        }

        #region Helper

        public override void UpdateHeight(double height = -1, bool setHeight = false)
        {
            if (!setHeight)
                return;
            SubView.HeightRequest = height;
        }

        protected override void Init()
        {
            InitToolTip();
            InitZoom();
            InitTrackBall();

            Legend.IconHeight = Legend.IconWidth = FSetting.SizeIconLegend;
            Legend.LabelStyle.FontFamily = FSetting.FontText;
            Legend.LabelStyle.FontSize = FSetting.FontSizeLabelContent;
            Legend.Title.FontFamily = FSetting.FontText;
            Legend.Title.FontSize = FSetting.FontSizeLabelContent;

            Chart.BindingContext = this;
            Chart.SideBySideSeriesPlacement = false;
            Chart.VerticalOptions = Chart.HorizontalOptions = LayoutOptions.Fill;
            Chart.Legend.IsVisible = false;

            Content = Chart;
            base.Init();
        }

        protected virtual void SeriesDataMarkerLabelCreated(object sender, ChartDataMarkerLabelCreatedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.DataMarkerLabel.Label))
                e.DataMarkerLabel.Label = Double.Parse(e.DataMarkerLabel.Label).ToString("### ### ### ##0");
        }

        protected virtual Task RefreshSeries(bool isLoading)
        {
            return Task.CompletedTask;
        }

        protected override void InitModel()
        {
            base.ViewModel = SumaryResult.Tables[SumaryResult.Tables.Count - 1].Rows[0]["dashboard"].ToString().ToObject<FCDashboard>();
            if (Model == null)
            {
                MessagingCenter.Send(new FMessage("FChart model is null"), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            InitLegendModel();

            SubView.HeightRequest = Model.Height == -1 ? FSetting.HeightChart : SizeByType(FSetting.ScreenHeight, Model.Height);
            SubView.WidthRequest = Model.Width == -1 ? SubView.WidthRequest : SizeByType(FSetting.ScreenWidth, Model.Width);
            SubView.Margin = GetThickness(SubView.Margin, Model.Margin);
            base.InitModel();
        }

        protected virtual void InitXAxis(CategoryAxis axis, FXAxis model)
        {
            if (model == null)
                return;
            InitAxis(axis, model);
            axis.ArrangeByIndex = model.ArrangeByIndex;
            axis.Interval = model.Interval;
        }

        protected virtual void InitYAxis(NumericalAxis axis, FYAxis model)
        {
            if (model == null)
                return;
            InitAxis(axis, model);
            axis.ShowMinorGridLines = model.ShowMinorGridLines;
        }

        protected virtual ChartColorCollection ChartColors()
        {
            var colors = new ChartColorCollection();
            Model.Colors.ForEach((x) => colors.Add(Color.FromHex(x)));
            return colors;
        }

        protected virtual ChartDataMarker DataMarker(DataMarkerLabelPosition position)
        {
            return InitMaker(FSetting.FontSizeLabelHint - 3, FSetting.FontText, position, Color.Transparent);
        }

        protected virtual ChartDataMarker DataMarker(DataMarkerLabelPosition position, int seriesIndex, bool hasBackground)
        {
            var maker = InitMaker(FSetting.FontSizeLabelHint, FSetting.FontText, position, Color.Transparent);
            maker.LabelContent = LabelContent.YValue;
            maker.LabelTemplate = DataMakerTemplate(seriesIndex, hasBackground);
            return maker;
        }

        protected virtual ChartDataMarker InitMaker(float fontSize, string fontFamily, DataMarkerLabelPosition position, Color borderColor)
        {
            return new ChartDataMarker()
            {
                LabelStyle = new DataMarkerLabelStyle()
                {
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    LabelPosition = position,
                    BorderColor = borderColor
                }
            };
        }

        protected virtual DataTemplate DataMakerTemplate(int position, bool hasBackground)
        {
            return new DataTemplate(() =>
            {
                var rs = new SfBorder();
                var lb = new Label();
                rs.CornerRadius = 3;
                rs.BorderColor = Color.Transparent;
                if (hasBackground)
                    rs.SetBinding(SfBorder.BackgroundColorProperty, "ColorMaker" + position.ToString());
                else
                    rs.BackgroundColor = Color.FromHex("#f5f3eb");
                lb.TextColor = hasBackground ? Color.White : FSetting.TextColorContent;
                lb.Padding = new Thickness(3, 0);
                lb.FontSize = FSetting.FontSizeLabelHint;
                lb.FontFamily = FSetting.FontText;
                lb.SetBinding(Label.TextProperty, "StringValue" + position.ToString());
                rs.Content = lb;
                return rs;
            });
        }

        protected virtual DataTemplate TooltipTemplate(int position)
        {
            return new DataTemplate(() =>
            {
                var yValue = new Label
                {
                    TextColor = Color.White,
                    FontFamily = FSetting.FontText,
                    FontSize = FSetting.FontSizeLabelTitle,
                    Margin = new Thickness(3, 0),
                    MaxLines = 2,
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                yValue.SetBinding(Label.TextProperty, $"StringValue{position}");
                return new StackLayout { Children = { yValue } };
            });
        }

        protected virtual DataTemplate TooltipTemplate(FBLabelStyle xdes, FBLabelStyle xvalue, FBLabelStyle ydes, FBLabelStyle yvalue, string xdescription, string ydescription, string index, string name = "Name", string value = "StringValue")
        {
            return new DataTemplate(() =>
            {
                var grid = new Grid();
                grid.Padding = new Thickness(5);
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var xdesL = new FLabel { Text = xdescription };
                var xval = new FLabel();
                var ydesL = new FLabel { Text = ydescription };
                var yval = new FLabel();

                InitLabelStyle(xdesL, xdes);
                InitLabelStyle(xval, xvalue);
                InitLabelStyle(ydesL, ydes);
                InitLabelStyle(yval, yvalue);

                xval.SetBinding(Label.TextProperty, name);
                yval.SetBinding(Label.TextProperty, $"{value}{index}");

                grid.Children.Add(xdesL, 0, 0);
                grid.Children.Add(xval, 1, 0);
                grid.Children.Add(ydesL, 0, 1);
                grid.Children.Add(yval, 1, 1);

                return grid;
            });
        }

        protected virtual void InitSerie(ChartSeries series, FChartSeries model, object itemSource, bool isLoading, int index = 1, bool isSector = false)
        {
            series.ItemsSource = itemSource;
            series.XBindingPath = "Name";
            series.EnableTooltip = true;
            series.EnableAnimation = model.EnableAnimation;
            series.Label = model.Header;
            series.IsVisibleOnLegend = Model.SeriesIsVisibleOnLegend;
            series.LegendIcon = ChartLegendIcon.SeriesType;
            series.StrokeWidth = (model.StrokeWidth != -1) ? model.StrokeWidth : series.StrokeWidth;
            series.DataMarker = (model.DataMaker != null && model.DataMaker.IsVisible) ? (model.DataMaker.EnableCustomMode) ? DataMarker(model.DataMaker.Position, 1, model.EnableColor) : DataMarker(model.DataMaker.Position) : series.DataMarker;
            series.ColorModel = (model.EnableColor) ? new ChartColorModel { Palette = ChartColorPalette.Custom, CustomBrushes = ChartColors() } : series.ColorModel;
            series.SelectedDataPointColor = (!String.IsNullOrEmpty(model.SelectedColor)) ? Color.FromHex(model.SelectedColor) : series.SelectedDataPointColor;
            series.IsVisible = Model.IsVisibleMultipleSeries || model.IsVisible;
            series.TooltipTemplate = model.EnableTooltipTemplate ? TooltipTemplate(model.XDescriptionStyle, model.XValueStyle, model.YDescriptionStyle, model.YValueStyle, model.XTooltipDescription, model.YTooltipDescription, index.ToString()) : TooltipTemplate(index);
            series.DataMarkerLabelCreated += SeriesDataMarkerLabelCreated;
        }

        protected virtual void InitAxis(ChartAxis axis, FAxis model)
        {
            axis.LabelStyle.BorderColor = Color.FromHex(model.LabelStyle.BorderColor);
            axis.LabelStyle.BackgroundColor = Color.FromHex(model.LabelStyle.BackgroundColor);
            axis.LabelStyle.TextColor = Color.FromHex(model.LabelStyle.TextColor);
            axis.LabelStyle.CornerRadius = new ChartCornerRadius(model.LabelStyle.CornerRadius);
            axis.LabelStyle.LabelAlignment = model.LabelAlignment;
            axis.LabelStyle.LabelsPosition = model.TickPosition;
            axis.TickPosition = model.TickPosition;
            axis.LabelStyle.Margin = GetThickness(axis.LabelStyle.Margin, model.LabelStyle.Margin);
            axis.ShowMajorGridLines = model.ShowMajorGridLines;
            axis.IsInversed = model.IsInversed;
            axis.CrossesAt = model.CrossesAt != -100 ? model.CrossesAt : axis.CrossesAt;
            axis.OpposedPosition = model.OpposedPosition;
            axis.IsVisible = model.IsVisible;
            if (model.Title != null)
            {
                axis.Title.Text = model.Title.Text;
                axis.Title.BorderWidth = model.Title.BorderWidth;
                axis.Title.BorderColor = Color.FromHex(model.Title.BorderColor);
                axis.Title.BackgroundColor = Color.FromHex(model.Title.BackgroundColor);
                axis.Title.TextColor = Color.FromHex(model.Title.TextColor);
                axis.Title.Margin = GetThickness(axis.Title.Margin, model.Title.Margin);
            }
            if (model.Titles != null && model.Titles.Count > 0)
            {
                axis.Title.Text = model.Titles[0];
            }
        }

        protected virtual void AddSerie(ChartSeries series)
        {
            Chart.Series.Add(series);
        }

        protected virtual void UpdateHeight(bool isNodata)
        {
            SubView.HeightRequest = isNodata ? SubView.HeightRequest : Model.Height != -1 ? SizeByType(FSetting.ScreenHeight, Model.Height) : DataResult.Rows.Count < 6 ? SizeByType(FSetting.ScreenHeight, Model.MinHeight) : SizeByType(FSetting.ScreenHeight, Model.MaxHeight);
        }

        protected virtual void UpdateWidth(bool isNodata)
        {
            SubView.WidthRequest = isNodata ? SubView.WidthRequest : Model.Width != -1 ? SizeByType(FSetting.ScreenWidth, Model.Width) : DataResult.Rows.Count < 6 ? SizeByType(FSetting.ScreenWidth, Model.MinWidth) : SizeByType(FSetting.ScreenWidth, Model.MaxWidth);
        }

        protected override void ReplaceLeftRight(string left, string right, bool isLoading)
        {
            Legend.Title.Text = Model.Legend?.Title?.Text?.Replace(LeftCharacter, left).Replace(RightCharacter, right);
            TitleText = Model.Title?.Replace(LeftCharacter, left).Replace(RightCharacter, right);
            base.ReplaceLeftRight(left, right, isLoading);
        }

        #endregion Helper

        #region Private

        private void InitToolTip()
        {
            ToolTip.FontFamily = FSetting.FontText;
            ToolTip.FontSize = FSetting.FontSizeLabelHint;
        }

        private void InitZoom()
        {
            Zoom.EnableDirectionalZooming = true;
        }

        private void InitTrackBall()
        {
            TrackBall.ShowLabel = true;
            TrackBall.ShowLine = true;
        }

        private void InitLegendModel()
        {
            if (Model.Legend == null)
                return;
            if (Model.Legend.Background != null)
            {
                Legend.BackgroundColor = Color.FromHex(Model.Legend.Background.BackgroundColor);
                Legend.StrokeColor = Color.FromHex(Model.Legend.Background.StrokeColor);
                Legend.StrokeWidth = Model.Legend.Background.StrokeWidth;
                Legend.Margin = GetThickness(Legend.Margin, Model.Legend.Background.Margin);
                Legend.CornerRadius = GetChartCornerRadius(Legend.CornerRadius, Model.Legend.Background.CornerRadius);
                Legend.StrokeDashArray = Model.Legend.Background.StrokeDashArray;
            }
            if (Model.Legend.Title != null)
            {
                Legend.Title.BorderColor = Color.FromHex(Model.Legend.Title.BorderColor);
                Legend.Title.BorderWidth = Model.Legend.Title.BorderWidth;
                Legend.Title.BackgroundColor = Color.FromHex(Model.Legend.Title.BackgroundColor);
                Legend.Title.TextColor = Color.FromHex(Model.Legend.Title.TextColor);
                Legend.Title.TextAlignment = Model.Legend.TextAlignment;
                Legend.Title.Margin = GetThickness(Legend.Title.Margin, Model.Legend.Title.Margin);
            }
            Legend.ToggleSeriesVisibility = Model.IsVisibleMultipleSeries;
            Legend.ItemMargin = GetThickness(Legend.ItemMargin, Model.Legend.ItemMargin);
            Legend.OverflowMode = Model.Legend.OverflowMode;
            Legend.Orientation = Model.Legend.Orientation != ChartOrientation.Default ? Model.Legend.Orientation : Legend.Orientation;
            Legend.IsVisible = Model.Legend.IsVisible;
            Legend.IsIconVisible = Model.Legend.ShowIcon;
            Legend.DockPosition = Model.Legend.DockPosition;
            Legend.MaxWidth = Model.Legend.Width != -1 ? FSetting.ScreenWidth * Model.Legend.Width : Legend.MaxWidth;
            Legend.LabelStyle.TextColor = Color.FromHex(Model.Legend.LabelColor);
        }

        private ChartCornerRadius GetChartCornerRadius(ChartCornerRadius reff, double[] input)
        {
            if (input == null)
                return reff;
            return input.Length == 1 ? new ChartCornerRadius(input[0]) : input.Length == 2 ? new ChartCornerRadius(input[0], input[1]) : input.Length == 4 ? new ChartCornerRadius(input[0], input[1], input[2], input[3]) : reff;
        }

        private double SizeByType(double bounds, double value)
        {
            return value == -1 ? bounds : Model.SizeType == FSizeType.Ratio ? bounds * value : value;
        }

        #endregion Private
    }
}