using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFNotificationManager
    {
        Task ShowHome(Func<Task> afterInvoke);

        Task ShowNotify();

        void ClearNotifyData();

        Task OpenDetail();

        bool IsOpenReport(string action);

        void LoadReportByAction(string action);

        void ReadById(string id, string group);

        Task LoadById(string id, string group);
    }
}