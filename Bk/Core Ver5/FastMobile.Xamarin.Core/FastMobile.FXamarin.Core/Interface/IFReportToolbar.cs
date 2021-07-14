using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFReportToolbar
    {
        Task<FCommnadValue> ViewRecordGrid(object obj, FData data);

        Task<FCommnadValue> ScatterRecordGrid(object obj, FData data);

        Task<FCommnadValue> EditRecordGrid(object obj, FData data);

        Task<FCommnadValue> NewRecordGrid(object obj);

        Task<FCommnadValue> DeleteRecordGrid(object obj, FData data);

        Task<FCommnadValue> SaveRecordGrid(FFormType type);

        Task<FCommnadValue> PrintRecordGrid(object obj, FData data);

        Task<FCommnadValue> PrintVoucherRecordGrid(object obj, FData data);

        Task<FCommnadValue> XmlDownLoadRecordGrid(object obj, FData data);

        Task<FCommnadValue> ReleaseRecordGrid(object obj, FData data);

        Task<FCommnadValue> CancelRecordGrid(object obj, FData data);

        Task<FCommnadValue> CustomRecordGrid(object obj, FData data);

        Task<FCommnadValue> CustomRecordOther(object obj, FData data);

        Task<FCommnadValue> DownloadAttachment(object obj, FData data);

        Task<FCommnadValue> CommandRecordGrid(object obj, FData data);

        Task<FCommnadValue> AcceptApproval(object obj, FData data);

        Task<FCommnadValue> CancelApproval(object obj, FData data);

        Task<FCommnadValue> UndoApproval(object obj, FData data);
    }
}