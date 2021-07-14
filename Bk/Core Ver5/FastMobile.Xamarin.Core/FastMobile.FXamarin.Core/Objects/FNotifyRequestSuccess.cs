using System;
using System.Data;

namespace FastMobile.FXamarin.Core
{
    public class FNotifyRequestSuccessEventArgs : EventArgs
    {
        public string Badge { get; }
        public int ServerCount { get; }
        public int ClientCount { get; }
        public DataSet Data { get; }

        public FNotifyRequestSuccessEventArgs(int total, string badge, int clientCount, DataSet data)
        {
            ClientCount = clientCount;
            ServerCount = total;
            Badge = badge;
            Data = data;
        }
    }
}