using System;
using System.Data;

namespace FastMobile.FXamarin.Core
{
    public interface IFSelectDetail
    {
        public FDataObservation Source { get; set; }

        public event EventHandler<(FInputGridValue Checks, FInputGridValue UnChecks)> AcceptClicked;

        public event EventHandler<EventArgs> CancelClicked;

        void UpdateSource(DataTable source, bool checkDisable, bool isNew);

        void Open();

        void Close();
    }
}