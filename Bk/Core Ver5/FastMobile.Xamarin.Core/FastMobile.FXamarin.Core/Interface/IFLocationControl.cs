using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public interface IFLocationControl
    {
        string ApiKey { get; set; }
        bool HasAdress { get; set; }
        bool HasNear { get; set; }
        ObservableCollection<FPlace> PinsPlaces { get; set; }

        void GoTo(Position position, Distance radius);

        string GetPosition();

        string GetPositionNear();

        FPlace NewCurrentPlace(Position position, string address);

        Task UpdateCurrentLocation(Position position, bool clearPins);

        Task<(ObservableCollection<FPlace> Places, FMessage Message)> GetNearPlaces(Position position, double radius);

        event EventHandler<FObjectPropertyArgs<Position>> MapClicked;

        event EventHandler<FObjectPropertyArgs<Position>> CurrentLocationUpdated;
    }
}