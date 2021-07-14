using MediaManager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputMediaVideo : FInputMedia
    {
        #region Public

        public FInputMediaVideo(string controller, string sysField, string lineNumber, List<string> allow) : base(controller, sysField, lineNumber, allow)
        {
            //V.Clicked += OnPlayVideo;
        }

        public FInputMediaVideo(FField field) : base(field)
        {
        }

        #endregion Public

        #region Protected

        protected override FButton CaptureButton()
        {
            return new FButton(FText.CaptureVideo, FIcons.VideoOutline);
        }

        protected override string UploadTitle()
        {
            return FText.SelectVideo;
        }

        protected override string DownloadTitle()
        {
            return FText.Download;
        }

        protected override ImageSource FileImage(string path)
        {
            return FInterface.IFEnvironment.GetThumbImage(path, 1);
        }

        protected override async Task<FileResult> PickMedia()
        {
            return await MediaPicker.PickVideoAsync();
        }

        protected override async Task<FileResult> TakeMedia()
        {
            return await MediaPicker.CaptureVideoAsync();
        }

        protected override IEnumerable<FButton> ExtendButtons()
        {
            return new List<FButton>();
        }

        protected override void OnImageClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(MediaPath))
                return;
            Device.InvokeOnMainThreadAsync(async () => await CrossMediaManager.Current.Play(MediaPath));
        }

        #endregion Protected
    }
}