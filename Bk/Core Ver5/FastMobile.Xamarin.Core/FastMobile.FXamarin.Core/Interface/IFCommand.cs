using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFCommand
    {
        Task<FMessage> Prossessing(IFPaging paging, bool isLog);

        Task<FMessage> Loading(IFPaging paging, bool isLog);

        Task<FMessage> Searching(IFPaging paging, bool isLog);

        Task<FMessage> GetDataSet(IFPaging paging, bool isLog);
    }
}