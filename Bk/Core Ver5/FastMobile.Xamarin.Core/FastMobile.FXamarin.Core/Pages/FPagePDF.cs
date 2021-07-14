using Syncfusion.SfPdfViewer.XForms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPagePDF : FPage
    {
        private Stream pdfStream;
        private SfPdfViewer pdfView;
        private string link;
        private bool isXml;

        public Stream PdfStream { get => pdfStream; set { pdfStream = value; OnPropertyChanged("PdfStream"); } }

        public FPagePDF(string url, bool xml = true) : base(false, false)
        {
            link = GetUrl(url);
            isXml = xml;
            SetPDFView();
            InitToolbar();
        }

        private async void InitToolbar()
        {
            if (isXml)
                ToolbarItems.Add(new ToolbarItem { IconImageSource = FIcons.Download.ToFontImageSource(Color.White, FSetting.SizeIconToolbar), Command = new Command(DownloadXml) });
            ToolbarItems.Add(new ToolbarItem { IconImageSource = FIcons.ContentSave.ToFontImageSource(Color.White, FSetting.SizeIconToolbar), Command = new Command(OnSaveDocument) });
            ToolbarItems.Add(new ToolbarItem { IconImageSource = FIcons.Printer.ToFontImageSource(Color.White, FSetting.SizeIconToolbar), Command = new Command(OnPrint) });
            await Task.CompletedTask;
        }

        public async Task Show(FPage parent)
        {
            await parent.SetBusy(true);
            PdfStream = FInterface.IFDownloader?.DownloadPdf(link);
            if (PdfStream.Length == 0) MessagingCenter.Send(new FMessage(0, 408, ""), FChannel.ALERT_BY_MESSAGE);
            else
            {
                Content = null;
                await parent.Navigation.PushAsync(this, true);
                await SetBusy(true);
                pdfView.LoadDocument(PdfStream);
                Content = pdfView;
                await SetBusy(false);
            }
            await parent.SetBusy(false);
        }

        public void SetPDFView()
        {
            pdfView = new SfPdfViewer();
            pdfView.IsToolbarVisible = false;
            pdfView.PageViewMode = PageViewMode.Continuous;
            pdfView.WidthRequest = FSetting.ScreenWidth;
            pdfView.HeightRequest = FSetting.ScreenHeight;
            pdfView.VerticalOptions = LayoutOptions.StartAndExpand;
            pdfView.HorizontalOptions = LayoutOptions.CenterAndExpand;
            pdfView.ViewMode = ViewMode.FitWidth;
            pdfView.IsPasswordViewEnabled = false;
            pdfView.BookmarkNavigationEnabled = false;
            pdfView.BookmarkPaneVisible = false;
            pdfView.ShowPageNumber = false;
            pdfView.EnableFormFilling = false;
            pdfView.IsTextSelectionEnabled = true;
            pdfView.EnableScrollHead = false;
        }

        private async void OnSaveDocument()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true) && !await FPermissions.HasPermission<Permissions.StorageRead>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            await SetBusy(true);
            var dl = await FInterface.IFDownloader?.SaveFile($"{DateTime.Now:yyyyMMddHHmmss}.pdf");
            MessagingCenter.Send(new FMessage(0, dl.Code, dl.Message), FChannel.ALERT_BY_MESSAGE);
            await SetBusy(false);
        }

        private async void DownloadXml()
        {
            try
            {
                if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
                {
                    FPermissions.ShowMessage();
                    return;
                }

                await SetBusy(true);
                var query = HttpUtility.ParseQueryString(link.Replace("type=3", "type=2").ToQuery());
                FInterface.IFDownload.OnFileDownloaded += FUtility.DownloadCompleted;
                await FInterface.IFDownload.DownloadFileWithText(query.ToString(), $"{DateTime.Now:yyyyMMddHHmmss}.xml");
                FInterface.IFDownload.OnFileDownloaded -= FUtility.DownloadCompleted;
                await SetBusy(false);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                await SetBusy(false);
            }
        }

        private void OnPrint()
        {
            try { pdfView.Print(); }
            catch
            {
                MessagingCenter.Send(new FMessage(0, 1250, string.Empty), FChannel.ALERT_BY_MESSAGE);
            }
        }

        private string GetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            var urls = url.Split(";");
            foreach (string u in urls)
            {
                if (u.IsUrl()) return u;
            }
            return urls[0];
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PdfStream.Flush();
            PdfStream.Close();
        }
    }
}