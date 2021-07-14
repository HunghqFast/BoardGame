using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XMap = Xamarin.Forms.Maps.Map;

namespace FastMobile.FXamarin.Core
{
    public class FMapContainer : ContentView, IFLocationControl
    {
        public static readonly BindableProperty CurrentPositionProperty = BindableProperty.Create("CurrentPosition", typeof(Position), typeof(FMapContainer));
        public static readonly BindableProperty CurrentLocationProperty = BindableProperty.Create("CurrentLocation", typeof(Location), typeof(FMapContainer));
        public static readonly BindableProperty PinsPlacesProperty = BindableProperty.Create("PinsPlaces", typeof(ObservableCollection<FPlace>), typeof(FMapContainer));

        public event EventHandler<FObjectPropertyArgs<Position>> MapClicked;

        public event EventHandler<FObjectPropertyArgs<Position>> CurrentLocationUpdated;

        public ObservableCollection<FPlace> PinsPlaces
        {
            get => (ObservableCollection<FPlace>)GetValue(PinsPlacesProperty);
            set => SetValue(PinsPlacesProperty, value);
        }

        public string ApiKey { get; set; }
        public bool HasAdress { get; set; }
        public bool HasNear { get; set; }

        private readonly FMap Map;

        public FMapContainer()
        {
            Map = new();
            PinsPlaces = new ObservableCollection<FPlace>();
            Base();
        }

        public FMapContainer(MapSpan region)
        {
            Map = new(region);
            PinsPlaces = new ObservableCollection<FPlace>();
            Base();
        }

        private void Base()
        {
            HorizontalOptions = VerticalOptions = Map.HorizontalOptions = Map.VerticalOptions = LayoutOptions.Fill;
            Map.BindingContext = this;
            Map.MapClicked += OnMapClicked;
            Map.ItemTemplate = new DataTemplate(Template);
            Map.SetBinding(XMap.ItemsSourceProperty, PinsPlacesProperty.PropertyName);
            Content = Map;
        }

        public string GetPosition()
        {
            throw new NotImplementedException();
        }

        public string GetPositionNear()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            MapClicked?.Invoke(this, new FObjectPropertyArgs<Position>(e.Position));
        }

        private Element Template()
        {
            var pin = new FPin();
            pin.SetBinding(FPin.AddressProperty, FPlace.AddressProperty.PropertyName);
            pin.SetBinding(FPin.PositionProperty, FPlace.PositionProperty.PropertyName);
            pin.SetBinding(FPin.LabelProperty, FPlace.PlaceNameProperty.PropertyName);
            pin.SetBinding(FPin.IconProperty, FPlace.MarkerIconProperty.PropertyName);
            return pin;
        }

        public virtual void GoTo(Position position, Distance radius)
        {
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, radius));
        }

        public async Task UpdateCurrentLocation(Position position, bool clearPins)
        {
            var adr = string.Empty;
            if (HasAdress)
            {
                var message = await FMapService.Instance.PlaceDetail(ApiKey, position.Latitude, position.Longitude, FMapService.Instance.LanguageText(FSetting.V), true);
                if (message.OK.Success != 1)
                {
                    MessagingCenter.Send(message.OK, FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                if (message.results.Length > 0)
                    adr = message.results[0].formatted_address;
            }

            if (clearPins) PinsPlaces.Clear();
            PinsPlaces.Add(NewCurrentPlace(position, adr));
            GoTo(position, new Distance(100));
            CurrentLocationUpdated?.Invoke(this, new FObjectPropertyArgs<Position>(position));
        }

        public FPlace NewCurrentPlace(Position position, string address)
        {
            return new FPlace(FText.LocationCurrent)
            {
                Position = position,
                Icon = FIcons.MapMarker.ToFontImageSource(FSetting.LightColor, FSetting.SizeIconButton),
                MarkerIcon = FIcons.MapMarker.ToFontImageSource(FSetting.DangerColor, FSetting.SizeIconButton * 1.5),
                Address = address,
                IconBackgroundColor = Color.FromHex("#1194ff")
            };
        }

        public async Task<(ObservableCollection<FPlace> Places, FMessage Message)> GetNearPlaces(Position position, double radius)
        {
            var source = new ObservableCollection<FPlace>();
            if (!HasNear || string.IsNullOrWhiteSpace(ApiKey)) return (source, FMessage.FromSuccess());
            var message = await FMapService.Instance.SearchByNear(ApiKey, position.Latitude, position.Longitude, radius, FMapService.Instance.LanguageText(FSetting.V));
            if (message.OK.Success != 1)
                return (source, message.OK);
            foreach (var result in message.results)
            {
                if (result.types.Contains("locality") || result.types.Contains("sublocality"))
                    continue;
                source.Add(new FPlace(result.name)
                {
                    Address = result.vicinity,
                    Icon = FMapService.Instance.IconFromType(result.types.Length > 0 ? result.types[0] : string.Empty).ToFontImageSource(FSetting.LightColor, FSetting.SizeIconButton),
                    IconBackgroundColor = Color.FromHex(FMapService.Instance.HexFromType(result.types.Length > 0 ? result.types[0] : string.Empty)),
                    Position = new Position(result.geometry.location.lat, result.geometry.location.lng)
                });
            }
            return (source, message.OK);
        }
    }
}