using Firebase.Iid;
using Firebase.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FFirebase : IFFirebase
    {
        public void Subscribe(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
                FirebaseMessaging.Instance.SubscribeToTopic(topic);
        }

        public void UnSubscribe(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
                FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
        }

        public void GetToken(Action<string> invoke) => invoke?.Invoke(FirebaseInstanceId.Instance.Token);

        public void GetToken(string senderID, Action<string> invoke)
        {
            try
            {
                Task.Run(() =>
                {
                    invoke?.Invoke(FirebaseInstanceId.Instance.GetToken(senderID, "FCM"));
                });
            }
            catch
            {
                invoke?.Invoke("");
            }
        }

        public string GetToken() => FirebaseInstanceId.Instance.Token;

        public string GetToken(string senderID)
        {
            var token = "";
            Task.Run(() =>
            {
                token = FirebaseInstanceId.Instance.GetToken(senderID, "FCM");
            });
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

            _ = Task.Run(() => Completed(FirebaseInstanceId.Instance.GetToken(senderID, "FCM")));

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

        public void RefreshToken() => Task.Run(FirebaseInstanceId.Instance.DeleteInstanceId);

        public void DeleteToken(string senderID) => Task.Run(() => FirebaseInstanceId.Instance.DeleteToken(senderID, "FCM"));
    }
}