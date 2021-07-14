using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFPageFilter : IFBusy
    {
        static ImageSource SaveFileIcon;
        static ImageSource DownFileIcon;
        static ImageSource TipsIcon;
        static ImageSource ClearIcon;

        string Controller { get; set; }
        FPage Page { get; set; }
        DataSet InitData { get; set; }
        FViewPage Settings { get; set; }
        Dictionary<string, FInput> Input { get; set; }
        List<string> InputEdited { get; set; }
        FFormTarget Target { get; set; }
        FFormType FormType { get; set; }
        bool IsMaster { get; set; }
        bool IsBusy { get; }
        bool IsOpening { get; set; }
        string SettingsDeviceID { get; }
        string DataSetDeviceID { get; }
        string SaveToolbarDeviceID { get; }
        string ExpanderStatus { get; }
        bool IsEditable { get; }
        string Action { get; }
        string ClientScript { get; set; }
        FData InputData { get; set; }
        FData OldData { get; set; }
        FData GridData { get; set; }
        Action Script { get; set; }
        object Root { get; set; }
        CheckCommand EditCommand { get; set; }
        CheckCommand NewCommand { get; set; }
        FPageFilterStyle FormStyle { get; set; }
        FPageFilterMethod Method { get; set; }
        Grid Form { get; }
        ScrollView InputView { get; set; }
        ContentView ButtonView { get; set; }

        event EventHandler OkClick;
        event EventHandler SaveClick;
        event EventHandler NewClick;
        event EventHandler EditClick;
        event EventHandler CancelClick;
        event EventHandler CloseClick;
        event EventHandler BackButtonClicked;

        void ClearAll();
        void FillAll(bool isDisable);
        void InitToobar();
        FData FDataDirForm(int mode = 0);
        Task Request(string requestAction, List<string> requestField);
        object InvokeScript(string id, string scriptDetail, string input = null);
        Task<object> InvokeScriptAsync(string id, string scriptDetail, string input = null);
        Task InitInput();
        Task InitContent();
        Task InitByController();
        Task InitBySetting();
        Task ExcuteButton();

        delegate Task<bool> CheckCommand(FData data);
    }
}
