using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    internal interface IFSpinWheelService
    {
        Task<FMessage> GetStruct();

        Task<FMessage> GetData();

        Task<FMessage> GetRoll();

    }
}