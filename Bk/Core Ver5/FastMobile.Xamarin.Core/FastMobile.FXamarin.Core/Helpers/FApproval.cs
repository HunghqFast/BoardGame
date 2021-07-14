using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FApproval : IFApproval
    {
        private readonly IFReportSettings report;

        public event EventHandler Executing;

        public FApproval(IFReportSettings report)
        {
            this.report = report;
        }

        public async void PageDetailOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.IsUrl()) return;
            e.Cancel = true;
            await e.Url.OpenBrowser();
        }

        public virtual async Task<bool> Execute(string type)
        {
            Executing?.Invoke(this, EventArgs.Empty);
            var message = await ApprovalServices(type);
            if (message == null)
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
            if (message.Success == "1")
            {
                var success = report?.Source[0]["success"]?.ToString();
                if (success == null || string.IsNullOrWhiteSpace(success))
                {
                    await report.Root.BeforePage.Grid.LoadingGrid(report.Root.DataTarget);
                    MessagingCenter.Send(new FMessage(0, 901, string.Empty), FChannel.ALERT_BY_MESSAGE);
                }
                else FFunc.CatchScriptMethod(report.Root, success.Replace("@@type", type));
                return true;
            }
            if (message.Success == "0")
            {
                if (string.IsNullOrEmpty(message.Message)) MessagingCenter.Send(new FMessage(0, 902, string.Empty), FChannel.ALERT_BY_MESSAGE);
                else MessagingCenter.Send(new FMessage(0, 0, message.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
            return false;
        }

        public virtual async Task<FInvokeResult> ApprovalServices(string type)
        {
            var source = report.Source[0];
            return await new FInvoke().OnApprove(type, source["service_url"].ToString(), source["nameSpace"].ToString(), source["method"].ToString(), source["approveMailID"].ToString(), report.Source[0]["ref_code"].ToString(), source["s1"].ToString(), report.CommentText, "");
        }
    }
}