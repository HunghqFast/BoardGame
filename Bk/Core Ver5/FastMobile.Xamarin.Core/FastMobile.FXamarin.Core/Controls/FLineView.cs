using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace FastMobile.FXamarin.Core
{
    public class FLineView : StackLayout
    {
        public static readonly BindableProperty StrokeDashArrayProperty = BindableProperty.Create("StrokeDashArray", typeof(DoubleCollection), typeof(FLineView), null, BindingMode.Default, null, StrokeDashArrayValueChanged);
        public static readonly BindableProperty StrokeBrushProperty = BindableProperty.Create("StrokeBrush", typeof(Brush), typeof(FLineView), Brush.Black);
        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create("StrokeThickness", typeof(double), typeof(FLineView), 1d);

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        public Brush StrokeBrush
        {
            get => (Brush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        private readonly Line Line;

        public FLineView()
        {
            Line = new Line();
            Base();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width <= 0 || height <= 0) return;
            Line.X1 = Line.Y1 = Line.Y2 = 0;
            Line.X2 = width;
            if (!Children.Contains(Line))
                Children.Add(Line);
        }

        private void Base()
        {
            Line.BindingContext = this;
            Line.SetBinding(Line.StrokeProperty, StrokeBrushProperty.PropertyName);
            Line.SetBinding(Line.StrokeThicknessProperty, StrokeThicknessProperty.PropertyName);
        }

        private static void StrokeDashArrayValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var lineView = bindable as FLineView;
            if (lineView.Line != null)
            {
                lineView.Line.StrokeDashArray = newValue as DoubleCollection;
            }
        }
    }
}