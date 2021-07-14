using Syncfusion.SfImageEditor.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using SPath = System.IO.Path;

namespace FastMobile.FXamarin.Core
{
    public class FInputMediaImage : FInputMedia
    {
        //private readonly FButton E;
        private FPageImageEditor Editor;

        private bool IsOpen;

        #region Public

        public FInputMediaImage(string controller, string sysField, string lineNumber, List<string> allow) : base(controller, sysField, lineNumber, allow)
        {
            //E = new FButton(FText.Edit, FIcons.ImageEditOutline);
            //E.Clicked += OnEditImage;
            //V.Clicked += OnEditImage;
        }

        public FInputMediaImage(FField field) : base(field)
        {
        }

        #endregion Public

        #region Protected

        protected override FButton CaptureButton()
        {
            return new FButton(FText.CaptureImage, FIcons.CameraOutline);
        }

        protected override string UploadTitle()
        {
            return FText.SelectImage;
        }

        protected override string DownloadTitle()
        {
            return FText.Download;
        }

        protected override ImageSource FileImage(string path)
        {
            return path;
        }

        protected override async Task<FileResult> TakeMedia()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true) || !await FPermissions.HasPermission<Permissions.Camera>(true))
            {
                FPermissions.ShowMessage();
                return null;
            }

            if (!MediaPicker.IsCaptureSupported)
            {
                MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                return null;
            }

            return await MediaPicker.CapturePhotoAsync();
        }

        protected override async Task<FileResult> PickMedia()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageRead>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true))
            {
                FPermissions.ShowMessage();
                return null;
            }

            return await MediaPicker.PickPhotoAsync();
        }

        protected override IEnumerable<FButton> ExtendButtons()
        {
            //E.SetBinding(View.IsVisibleProperty, new MultiBinding { Bindings = new List<BindingBase> { new Binding(IsModifierPropertyName), new Binding(ValueProperty.PropertyName, converter: new FValueToVisible(this)) }, Converter = FMultiBoolConvert.Instance });
            return new List<FButton>();
        }

        protected override async void OnImageClicked(object sender, EventArgs e)
        {
            try
            {
                if (!IsModifier)
                {
                    if (IsOpen)
                        return;
                    IsOpen = true;
                    var page = new FPageImage();
                    if (FUtility.IsUrl(MediaPath))
                        page.SetImageSourceFromMediaUrl(MediaPath);
                    else page.Source = V.Source;

                    await Navigation.PushAsync(page, true);
                    IsOpen = false;
                    return;
                }

                if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
                {
                    FPermissions.ShowMessage();
                    return;
                }

                await Root.SetBusy(true);
                if (Editor == null)
                {
                    Editor = new FPageImageEditor();
                    Editor.Title = FText.EditImage;
                    Editor.Appearing -= OnEditAppearing;
                    Editor.Disappearing += OnEditDisAppearing;
                }
                if (IsOpen)
                {
                    await Root.SetBusy(false);
                    return;
                }

                IsOpen = true;
                Editor.Editor.Source = V.Source;
                Editor.Editor.ImageSaving += OnImageSaving;
                Editor.Editor.ImageSaved += OnImageSaved;
                await Navigation.PushAsync(Editor, true);
                await Root.SetBusy(false);
            }
            catch
            {
                await Root.SetBusy(false);
            }
        }

        #endregion Protected

        #region Private

        private void OnImageSaving(object sender, ImageSavingEventArgs e)
        {
            Editor.Editor.ImageSaving -= OnImageSaving;
            _ = Editor.SetBusy(true);
            var fileExt = SPath.GetExtension(MediaPath);
            if (!string.IsNullOrEmpty(fileExt))
                SaveFileExt = fileExt;

            if (!AllowExtensions.Contains("*") && !AllowExtensions.Contains(SaveFileExt))
            {
                e.Cancel = true;
                FPermissions.ShowMessage();
                return;
            }

            SaveFileName = FUtility.NewFileName(SaveFileExt);
            MediaData = FUtility.GetFileData(e.Stream);
        }

        private void OnImageSaved(object sender, ImageSavedEventArgs args)
        {
            Editor.Editor.ImageSaved -= OnImageSaved;
            if (File.Exists(args.Location))
                MediaPath = args.Location;
            if (IsOpen)
            {
                IsOpen = false;
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync(true));
            }
            _ = Editor.SetBusy(false);
            FUtility.RunAfter(Save, TimeSpan.FromMilliseconds(200));
        }

        private void OnEditDisAppearing(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        private void OnEditAppearing(object sender, EventArgs e)
        {
            IsOpen = true;
        }

        private void Save()
        {
            Device.BeginInvokeOnMainThread(() => SetValue(null, IsEdit ? "1" : "0", true, ImageSource.FromStream(() => new MemoryStream(MediaData))));
        }

        #endregion Private
    }
}