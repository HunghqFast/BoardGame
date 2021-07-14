using System.Collections.Generic;

namespace FastMobile.FXamarin.Core
{
    public sealed class FExtensionParam
    {
        public bool IsUpdate { get; set; }
        public string Key { get; set; }
        public string UserID { get; set; }
        public string AppMode { get; set; }
        public string DataName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public FDeviceInformation DeviceInfo { get; set; }
        public List<string> OptionsName { get; set; }

        //Release
        public string H { get; set; }

        //Log
        public bool WriteLog { get; set; }

        public string LogMode { get; set; }
        public string LogContent { get; set; }
        public string LogContent2 { get; set; }
        public string LogAction { get; set; }
        public string LogController { get; set; }

        public static FExtensionParam New(bool writeLog, string controller, FAction action)
        {
            return new FExtensionParam
            {
                WriteLog = FSetting.AppMode == FAppMode.FE ? writeLog : writeLog && action == FAction.Initialize,
                LogAction = action.ToString(),
                LogController = controller,
                LogMode = "01"
            };
        }

        public static FExtensionParam New(bool writeLog, string content, string content2, FAction action)
        {
            return new FExtensionParam
            {
                WriteLog = FSetting.AppMode == FAppMode.FE ? writeLog : writeLog && action == FAction.Initialize,
                LogAction = action.ToString(),
                LogContent = content,
                LogContent2 = content2,
                LogMode = "01"
            };
        }
    }
}