using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageImage : FPage
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource), typeof(FPageImage), null, BindingMode.TwoWay);

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private readonly Image Image;
        private readonly FZoomView View;
        private readonly ToolbarItem Rotater;

        public FPageImage() : base(false, false)
        {
            Rotater = new ToolbarItem();
            View = new FZoomView();
            Image = new Image() { BindingContext = this };
            Image.HorizontalOptions = Image.VerticalOptions = LayoutOptions.Fill;
            Image.SetBinding(Image.SourceProperty, SourceProperty.PropertyName);

            View.Content = Image;
            Content = View;

            Rotater.IconImageSource = FIcons.RotateRight.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconToolbar);
            Rotater.Clicked += OnRotation;
            ToolbarItems.Add(Rotater);
        }

        private void OnRotation(object sender, EventArgs e)
        {
            Image.RotateTo(Image.Rotation + 90);
        }

        public void SetImageSourceFromMediaUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            Source = url.Replace("t=show&", "t=showfull&") + (url.Contains("&w=") ? "" : "&w=4320&h=7680");
        }
    }
}