using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public interface IFLocation
    {
        event EventHandler<FObjectPropertyArgs<(Position Position, double Accuracy)>> LocationChanged;

        event EventHandler<FObjectPropertyArgs<bool>> GPSEnabled;

        bool IsUpdating { get; }
        Action<bool> Updated { get; set; }

        bool IsGpsEnable();

        void OpenSetting();

        void StopUpdating();

        Task<bool> StartUpdating(FLocationAccuracy accuracy, CancellationToken cancellation);

        Task<(Position Position, double Accuracy)> GetCurrentPosition(FLocationAccuracy accuracy, CancellationToken cancellation, bool requirePermission);
    }
}