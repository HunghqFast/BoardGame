using Firebase.CloudMessaging;
using Firebase.InstanceID;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FFirebase : IFFirebase
    {
        public void Subscribe(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
                Messaging.SharedInstance.Subscribe(topic);
        }

        public void UnSubscribe(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
                Messaging.SharedInstance.Unsubscribe(topic);
        }

        public void GetToken(Action<string> invoke) => invoke?.Invoke(Messaging.SharedInstance?.FcmToken);

        public void GetToken(string senderID, Action<string> invoke)
        {
            var handler = new MessagingFcmTokenFetchCompletionHandler((fcmToken, error) => invoke?.Invoke(fcmToken ?? string.Empty));
            Messaging.SharedInstance.RetrieveFcmToken(senderID, handler);
        }

        public string GetToken() => Messaging.SharedInstance?.FcmToken;

        public string GetToken(string senderID)
        {
            var token = "";
            var handler = new MessagingFcmTokenFetchCompletionHandler((fcmToken, error) =>
            {
                token = error == null ? fcmToken : error.Description;
            });
            Task.Run(() => Messaging.SharedInstance.RetrieveFcmToken(senderID, handler));
            return token;
        }

        public async Task<string> GetTokenAsync(string senderID, int timeoutSeconds = -1)
        {
            var token = string.Empty;
            var tcs = new TaskCompletionSource<string>();

            if (timeoutSeconds != -1)
            {
                var cancel = Core.FUtility.TimeoutToken(CancellationToken.None, TimeSpan.FromSeconds(30));
                cancel.Register(Cancel);
            }

            var handler = new MessagingFcmTokenFetchCompletionHandler((fcmToken, error) => Completed(fcmToken ?? string.Empty));

            _ = Task.Run(() => Messaging.SharedInstance.RetrieveFcmToken(senderID, handler));

            var result = await tcs.Task;
            return result;

            void Completed(string result)
            {
                tcs.TrySetResult(result);
            }

            void Cancel()
            {
                tcs.TrySetResult(string.Empty);
            }
        }

        public void RefreshToken() => InstanceId.SharedInstance?.DeleteId(new InstanceIdDeleteHandler((error) => { }));

        public void DeleteToken(string senderID) => Messaging.SharedInstance?.DeleteFcmToken(senderID, new MessagingDeleteFcmTokenCompletionHandler((error) => { }));
    }
}