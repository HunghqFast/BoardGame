using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FToolbar
    {
        public const string ParentPreferenceName = "PARENT";

        private static readonly string[] onRecord = new string[]
        {
             "Edit", "Delete", "Print", "PrintVoucher", "XmlDownload", "Command"
        };

        public string Command { get; set; }
        public string CommandSuccessScript { get; set; }
        public string CommandSuccess { get; set; }
        public string Title { get; set; }
        public Color Color { get; set; }
        public string Icon { get; set; }
        public string Check { get; set; }
        public string ShowForm { get; set; }
        public int FormType { get; set; }
        public List<string> Fields { get; set; }
        public string MenuCheck { get; set; }
        public List<FItem> MenuItem { get; set; }
        public string CommandArgument { get; set; }
        public string Align { get; set; }
        public bool IsBorder { get; set; }
        public string Bio { get; set; }
        public string BioPassword { get; set; }
        public string Password { get; set; }
        public bool BioNative { get; set; }
        public object Data { get; set; }

        public ImageSource GetIcon(Color color, double size)
        {
            if (string.IsNullOrEmpty(Icon))
                return Command switch
                {
                    "PDF" => FIcons.FilePdfOutline.ToFontImageSource(Color.FromHex("#e7756e"), size),
                    "WORD" => FIcons.FileWordOutline.ToFontImageSource(Color.FromHex("#185ABD"), size),
                    "EXCEL" => FIcons.FileExcelOutline.ToFontImageSource(Color.FromHex("#107C41"), size),
                    "IMG" => FIcons.FileImageOutline.ToFontImageSource(Color.FromHex("#ffc000"), size),
                    "CODE" => FIcons.FileCodeOutline.ToFontImageSource(Color.FromHex("#4472c4"), size),
                    "TEXT" => FIcons.FileDocumentOutline.ToFontImageSource(Color.FromHex("#a5a5a5"), size),
                    "FILE" => FIcons.FileDownloadOutline.ToFontImageSource(Color.FromHex("#636363"), size),
                    _ => GetStringIcon()?.ToFontImageSource(color, size),
                };
            else return Icon.ToImageSource(color, size);
        }

        public Color GetIconColor()
        {
            if (string.IsNullOrEmpty(Icon))
            {
                return Command switch
                {
                    "PDF" => Color.FromHex("#e7756e"),
                    "WORD" => Color.FromHex("#185ABD"),
                    "EXCEL" => Color.FromHex("#107C41"),
                    "IMG" => Color.FromHex("#ffc000"),
                    "CODE" => Color.FromHex("#4472c4"),
                    "TEXT" => Color.FromHex("#a5a5a5"),
                    "FILE" => Color.FromHex("#636363"),
                    _ => FSetting.PrimaryColor,
                };
            }
            return FSetting.PrimaryColor;
        }

        public string GetStringIcon()
        {
            if (string.IsNullOrEmpty(Icon))
            {
                return Command switch
                {
                    "New" => FIcons.Plus,
                    "Release" => FIcons.FileSendOutline,
                    "Select" => FIcons.FormatListChecks,
                    "Close" => FIcons.ChevronLeft,
                    "Cancel" => FIcons.Close,
                    "AcceptApproval" => FIcons.Check,
                    "CancelApproval" => FIcons.Close,
                    "UndoApproval" => FIcons.RestartOff,
                    "ClearAll" => FIcons.PlaylistRemove,
                    "Next" => FIcons.ArrowRight,
                    "Back" => FIcons.ArrowLeft,
                    "Download" => FIcons.Download,

                    "SelectFile" => FIcons.UploadOutline,
                    "SelectImage" => FIcons.ImageOutline,
                    "SelectVideo" => FIcons.VideoOutline,
                    "CaptureImage" => FIcons.CameraOutline,
                    "CaptureVideo" => FIcons.VideoPlusOutline,
                    "EditImage" => FIcons.ImageEditOutline,

                    "PDF" => FIcons.FilePdfOutline,
                    "WORD" => FIcons.FileWordOutline,
                    "EXCEL" => FIcons.FileExcelOutline,
                    "IMG" => FIcons.FileImageOutline,
                    "CODE" => FIcons.FileCodeOutline,
                    "TEXT" => FIcons.FileDocumentOutline,
                    "FILE" => FIcons.FileDownloadOutline,

                    "Edit" => FIcons.PencilOutline,
                    "Delete" => FIcons.TrashCanOutline,
                    "Print" => FIcons.Printer,
                    "PrintVoucher" => FIcons.Printer,
                    "XmlDownload" => FIcons.Download,
                    "Command" => FIcons.Sync,
                    _ => null,
                };
            }
            return Icon;
        }

        public Color GetColor()
        {
            if (Color == Color.Default)
            {
                return Command switch
                {
                    "Edit" => Color.FromHex("#EDD013"),
                    "Delete" => Color.FromHex("#F96267"),
                    "Print" => Color.FromHex("#5BC0DE"),
                    "PrintVoucher" => Color.FromHex("#5BC0DE"),
                    "XmlDownload" => Color.FromHex("#C8C7CF"),
                    "Command" => Color.FromHex("#F0AD4E"),
                    _ => Color.White,
                };
            }
            return Color;
        }

        public bool IsToolbar => !IsRecord;

        public bool IsRecord => onRecord.Contains(Command);

        public bool IsRight => !string.IsNullOrWhiteSpace(Align) && Align.ToLower().Equals("right");

        public bool IsLeft => !IsRight;

        public IEnumerable<FItem> GetMenuItem => MenuItem.Where(x =>
        {
            if (string.IsNullOrWhiteSpace(MenuCheck)) return true;
            try
            {
                return (bool)FFunc.Compute(null, MenuCheck.Replace("[commandArgument]", x.I));
            }
            catch
            {
                return true;
            }
        });

        public FToolbar()
        {
        }

        public FToolbar(JObject obj)
        {
            var color = FFunc.GetStringValue(obj, "Color").Trim();
            Command = FFunc.GetStringValue(obj, "Command").Trim();
            CommandSuccess = null;
            CommandSuccessScript = FFunc.GetStringValue(obj, "CommandSuccess");
            Title = FFunc.GetStringValue(obj, "Title");
            Icon = FFunc.GetStringValue(obj, "Icon").Trim();
            Color = string.IsNullOrEmpty(color) ? Color.Default : Color.FromHex(color);
            Check = FFunc.GetStringValue(obj, "Check");
            ShowForm = FFunc.GetStringValue(obj, "ShowForm").Trim();
            FormType = (int)FFunc.GetNumberValue(obj, "FormType", 1);
            Fields = FFunc.GetArrayString(FFunc.GetStringValue(obj, "Fields"));
            MenuCheck = FFunc.GetStringValue(obj, "MenuCheck");
            Align = FFunc.GetStringValue(obj, "Align");
            IsBorder = FFunc.GetBooleanValue(obj, "IsBorder");
            Bio = FFunc.GetStringValue(obj, "Bio");
            BioPassword = FFunc.GetStringValue(obj, "BioPassword");
            Password = FFunc.GetStringValue(obj, "Password");
            BioNative = FFunc.GetBooleanValue(obj, "BioNative");
            MenuItem = FFunc<FItem>.GetFListObject(obj.SelectToken("MenuItem"));
            Data = null;
        }
    }
}