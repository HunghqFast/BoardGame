using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFFirebase
    {
        void Subscribe(string topic);

        void UnSubscribe(string topic);

        void GetToken(Action<string> invoke);

        void GetToken(string senderID, Action<string> invoke);

        string GetToken();

        string GetToken(string senderID);

        Task<string> GetTokenAsync(string senderID, int timeoutSeconds = -1);

        void DeleteToken(string senderID);

        void RefreshToken();
    }
}