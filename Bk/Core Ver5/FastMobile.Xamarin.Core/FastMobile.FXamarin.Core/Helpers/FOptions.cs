using System;

namespace FastMobile.FXamarin.Core
{
    public static class FOptions
    {
        public static int TicketMinute
        {
            get => Convert.ToInt32("FastMobile.FXamarin.Core.FOptions.TicketMinute".GetCache("55"));
            set => (value - 5).ToString().SetCache("FastMobile.FXamarin.Core.FOptions.TicketMinute");
        }
    }
}