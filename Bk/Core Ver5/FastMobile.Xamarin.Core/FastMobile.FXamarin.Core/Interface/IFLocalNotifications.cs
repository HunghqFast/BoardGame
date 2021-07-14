using System;

namespace FastMobile.FXamarin.Core
{
    public interface IFLocalNotifications
    {
        void Show(string title, string body, int id = 0);

        void Show(string title, string body, int id, DateTime notifyTime);

        void Cancel(int id);
    }
}