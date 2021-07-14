using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public class FPlace : BindableObject
    {
        public static readonly BindableProperty PlaceNameProperty = BindableProperty.Create("PlaceName", typeof(string), typeof(FPlace));
        public static readonly BindableProperty AddressProperty = BindableProperty.Create("Address", typeof(string), typeof(FPlace));
        public static readonly BindableProperty MarkerIconProperty = BindableProperty.Create("MarkerIcon", typeof(ImageSource), typeof(FPlace));
        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(ImageSource), typeof(FPlace));
        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Position", typeof(Position), typeof(FPlace));
        public static readonly BindableProperty IconBackgroundColorProperty = BindableProperty.Create("IconBackgroundColor", typeof(Color), typeof(FPlace), Color.Default);

        public string PlaceName
        {
            get => (string)GetValue(PlaceNameProperty);
            set => SetValue(PlaceNameProperty, value);
        }

        public string Address
        {
            get => (string)GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ImageSource MarkerIcon
        {
            get => (ImageSource)GetValue(MarkerIconProperty);
            set => SetValue(MarkerIconProperty, value);
        }

        public Position Position
        {
            get => (Position)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public Color IconBackgroundColor
        {
            get => (Color)GetValue(IconBackgroundColorProperty);
            set => SetValue(IconBackgroundColorProperty, value);
        }

        public FPlace(string placeName)
        {
            PlaceName = placeName;
        }
    }
}