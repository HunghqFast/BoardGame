using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFScroll
    {
        bool IsBottom { get; }
        bool IsTop { get; }

        event EventHandler ContentScrolled;

        Task<bool> ScrollToTop();

        Task<bool> ScrollToBot();
    }
}