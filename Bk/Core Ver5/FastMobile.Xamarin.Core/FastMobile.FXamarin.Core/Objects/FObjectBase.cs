using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FastMobile.FXamarin.Core
{
    public abstract class FObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}