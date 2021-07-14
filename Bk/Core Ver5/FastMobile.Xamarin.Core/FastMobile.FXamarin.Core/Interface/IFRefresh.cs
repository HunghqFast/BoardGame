using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFRefresh
    {
        bool IsRefreshing { get; set; }

        event EventHandler Refreshing;
        Task SetRefresh(bool value, int miliseconds = 400);

    }
}