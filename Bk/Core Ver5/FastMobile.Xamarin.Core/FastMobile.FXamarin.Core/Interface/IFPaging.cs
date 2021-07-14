using System.Collections.ObjectModel;

namespace FastMobile.FXamarin.Core
{
    public interface IFPaging
    {
        ObservableCollection<int> ListItem { get; set; }
        ObservableCollection<int> ListPaging { get; set; }
        int ItemTo { get; set; }
        int ItemFrom { get; set; }
        int ItemPerPage { get; set; }
        int PageIndex { get; set; }
        int TotalItem { get; set; }
        bool TriggerRefresh { get; set; }
    }
}