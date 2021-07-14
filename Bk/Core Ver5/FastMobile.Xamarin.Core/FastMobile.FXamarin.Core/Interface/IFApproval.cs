using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFApproval
    {
        void PageDetailOnNavigating(object sender, WebNavigatingEventArgs e);

        Task<bool> Execute(string type);

        Task<FInvokeResult> ApprovalServices(string type);

        public event EventHandler Executing;
    }
}