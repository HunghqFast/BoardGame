using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputFile : FInputTable, IFInputRequest
    {
        public static readonly BindableProperty AllowMediaProperty = BindableProperty.Create("AllowMedia", typeof(bool), typeof(FInputFile));
        private readonly FViewPage settings;
        private readonly string controller;
        private readonly List<FFileInfo> files;
        private readonly List<FData> delData;
        private string sysKey;
        private string ticket;
        private int fileMaxSize;

        public string SettingsDeviceID => $"FastMobile.FXamarin.Core.FInputFile.device_settings_grid_{Controller}";
        public const string Key = "syskey", Ticket = "ticket", FormatDate = "dd/MM/yyyy HH:mm", FileName = "file_name", FileEnc = "file_enc", FileData = "file_data", FileSize = "file_size", FileExt = "file_ext", FileTimeCreated = "file_time_created", FileTimeEdited = "file_time_edited", FilePath = "file_path", FileClient = "file_client", FileTimeUpload = "datetime0", FileMaxSize = "file_max_size";

        public bool AllowMedia
        {
            get => (bool)GetValue(AllowMediaProperty);
            set => SetValue(AllowMediaProperty, value);
        }

        public override bool HasPadding => false;

        public override FViewPage Settings => settings;

        public override string Controller => controller;

        public override string Target => "Grid";

        public override bool IncreaseLnr => true;

        public override FInputGridValue Value
        {
            get
            {
                var table = new DataTable();
                if (Settings.Fields == null || Settings.Fields.Count == 0) return new FInputGridValue(table, IsEdited);
                FFunc.AddColumnTable(table, Settings.Fields);
                delData.ForEach(del => FFunc.AddRowTable(table, del, Settings.Fields, true, delData.IndexOf(del)));
                table.TableName = Name;
                return new FInputGridValue(table, IsEdited);
            }
        }

        public FInputFile(string controller, JObject settings) : base()
        {
            Type = FieldType.Table;
            IsEdited = "0";
            this.controller = controller;
            this.settings = new FViewPage(SettingsDeviceID, settings);
            this.files = new List<FFileInfo>();
            this.delData = new List<FData>();
            InitGridView();
        }

        public FInputFile(FField field) : base(field)
        {
            Type = FieldType.Table;
            IsEdited = "0";
            this.controller = field.ItemController;
            this.settings = new FViewPage(SettingsDeviceID, field.ItemTemplate);
            this.files = new List<FFileInfo>();
            this.delData = new List<FData>();
            InitGridView();
        }

        #region Public

        public override void InitColumn()
        {
            base.InitColumn();
            var d = new FToolbar { Command = "Delete" };
            var x = new FToolbar { Command = "XmlDownload" };
            var c = new FCustomColumn
            {
                MappingName = FData.GetBindingName(FileClient),
                Width = 2 * FSetting.HeightRowGrid,
                MaximumWidth = 2 * FSetting.HeightRowGrid,
                MinimumWidth = 2 * FSetting.HeightRowGrid
            };
            c.CellTemplate = new DataTemplate(() =>
            {
                var template = new Grid();
                var delete = GridStyle.SwipeView(d, OnDeleteItem, false);
                var download = GridStyle.SwipeView(x, OnDownloadItem, false);

                template.ColumnSpacing = template.RowSpacing = 0;
                template.Margin = 0;
                template.VerticalOptions = LayoutOptions.FillAndExpand;
                template.HorizontalOptions = LayoutOptions.FillAndExpand;
                template.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                template.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                template.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                template.Children.Add(delete, 0, 0);
                template.Children.Add(download, 1, 0);

                download.SetBinding(View.IsVisibleProperty, FData.GetBindingName(FileClient), converter: FInvertBool.Instance);
                return template;
            });
            Grid.Columns.Insert(0, c);
        }

        public async Task InvokeService()
        {
            if (files.Count == 0) return;
            await FServices.Attachment(Root.Controller, sysKey.TrimEnd(), ticket.TrimEnd(), "0", files);
        }

        public override bool IsEqual(object oldValue)
        {
            return delData.Count == 0 && files.Count == 0;
        }

        #endregion Public

        #region Protected

        protected override View SetContentView()
        {
            HeightGridView = FSetting.HeightRowGrid * 4;

            var g = new Grid();
            var r = new RowDefinition();
            var f = new FMenuButtonReport { Toolbar = new FToolbar { Command = "SelectFile", Title = FText.SelectFile }, Action = OnPickFile, Visible = true, Enable = true, BindingContext = this };
            var i = new FMenuButtonReport { Toolbar = new FToolbar { Command = "SelectImage", Title = FText.SelectImage }, Action = OnPickImage, Visible = AllowMedia, Enable = true, BindingContext = this };
            var v = new FMenuButtonReport { Toolbar = new FToolbar { Command = "SelectVideo", Title = FText.SelectVideo }, Action = OnPickVideo, Visible = AllowMedia, Enable = true, BindingContext = this };
            var n = new FMenuButtonReport { Toolbar = new FToolbar { Command = "CaptureImage", Title = FText.CaptureImage }, Action = OnCaptureImage, Visible = AllowMedia, Enable = true, BindingContext = this };
            var l = new FMenuButtonReport { Toolbar = new FToolbar { Command = "CaptureVideo", Title = FText.CaptureVideo }, Action = OnCaptureVideo, Visible = AllowMedia, Enable = true, BindingContext = this };
            var c = new FMenuButtonReport { Toolbar = new FToolbar { Command = "ClearAll", Title = FText.ClearAll }, Action = OnClearItem, Enable = true, BindingContext = this };
            var m = new ObservableCollection<FMenuButtonReport> { f, i, v, n, l, c };

            r.SetBinding(RowDefinition.HeightProperty, HeightGridViewProperty.PropertyName);
            c.SetBinding(FMenuButtonReport.VisibleProperty, ClearModeProperty.PropertyName);
            i.SetBinding(FMenuButtonReport.VisibleProperty, AllowMediaProperty.PropertyName);
            v.SetBinding(FMenuButtonReport.VisibleProperty, AllowMediaProperty.PropertyName);
            n.SetBinding(FMenuButtonReport.VisibleProperty, AllowMediaProperty.PropertyName);
            l.SetBinding(FMenuButtonReport.VisibleProperty, AllowMediaProperty.PropertyName);

            g.RowSpacing = 0;
            g.VerticalOptions = LayoutOptions.StartAndExpand;
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.RowDefinitions.Add(r);
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            g.Children.Add(Grid, 0, 0);
            g.Children.Add(new FLine(), 0, 1);
            g.Children.Add(new FReportStyle().InitToolbarView(m, FSetting.HeightRowGrid, -1, FSetting.SizeIconMenu, true, true, false), 0, 2);
            return g;
        }

        protected override void UpdateDetail(DataTable table)
        {
            if (table.Rows.Count == 0)
            {
                sysKey = ticket = string.Empty;
                fileMaxSize = -1;
                return;
            }
            var row = table.Rows[0];
            sysKey = !table.Columns.Contains(Key) ? string.Empty : row[Key].ToString();
            ticket = !table.Columns.Contains(Ticket) ? string.Empty : row[Ticket].ToString();
            fileMaxSize = !table.Columns.Contains(FileMaxSize) ? -1 : Convert.ToInt32(row[FileMaxSize]);
        }

        protected override void UpdateDetail(FData data)
        {
            if (data == null)
            {
                sysKey = ticket = string.Empty;
                fileMaxSize = -1;
                return;
            }
            sysKey = data.CheckName(Key) ? data[Key].ToString() : string.Empty;
            ticket = data.CheckName(Ticket) ? data[Ticket].ToString() : string.Empty;
            fileMaxSize = data.CheckName(FileMaxSize) ? Convert.ToInt32(data[FileMaxSize]) : -1;
        }

        protected override void InitPropertyByField(FField f)
        {
            AllowMedia = f.ItemAllowMedia;
            base.InitPropertyByField(f);
        }

        #endregion Protected

        #region private

        private async Task OnPickFile(object obj)
        {
            await OnFile(FFileType.File);
        }

        private async Task OnPickImage(object obj)
        {
            await OnFile(FFileType.PickPhoto);
        }

        private async Task OnPickVideo(object obj)
        {
            await OnFile(FFileType.PickVideo);
        }

        private async Task OnCaptureImage(object obj)
        {
            await OnFile(FFileType.CapturePhoto);
        }

        private async Task OnCaptureVideo(object obj)
        {
            await OnFile(FFileType.CaptureVideo);
        }

        private async Task OnFile(FFileType type)
        {
            if (!IsModifier) return;
            await Lock(this, async () =>
            {
                try
                {
                    await Root.SetBusy(true);
                    switch (type)
                    {
                        case FFileType.File:
                            await PickFile();
                            break;

                        case FFileType.PickPhoto:
                            await PickPhoto();
                            break;

                        case FFileType.PickVideo:
                            await PickVideo();
                            break;

                        case FFileType.CapturePhoto:
                            await CapturePhoto();
                            break;

                        case FFileType.CaptureVideo:
                            await CaptureVideo();
                            break;

                        default: break;
                    }
                    await Root.SetBusy(false);
                }
                catch (Exception ex)
                {
                    if (FSetting.IsDebug)
                        MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                    else MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                    await Root.SetBusy(false);
                }
            });
        }

        private async Task PickFile()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageRead>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            var ms = await FilePicker.PickMultipleAsync();
            if (ms == null || ms.Count() == 0)
                return;
            await ms.ForEach(async x => await AddNewItem(x));
            await Root.SetBusy(false);
        }

        private async Task PickPhoto()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageRead>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            await AddNewItem(await MediaPicker.PickPhotoAsync());
        }

        private async Task PickVideo()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageRead>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true) || !await FPermissions.HasPermission<Permissions.Microphone>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            await AddNewItem(await MediaPicker.PickVideoAsync());
        }

        private async Task CapturePhoto()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true) || !await FPermissions.HasPermission<Permissions.Camera>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            if (!MediaPicker.IsCaptureSupported)
            {
                MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            await AddNewItem(await MediaPicker.CapturePhotoAsync());
        }

        private async Task CaptureVideo()
        {
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true) || !await FPermissions.HasPermission<Permissions.Photos>(true) || !await FPermissions.HasPermission<Permissions.Camera>(true) || !await FPermissions.HasPermission<Permissions.Microphone>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            if (!MediaPicker.IsCaptureSupported)
            {
                MessagingCenter.Send(FMessage.FromFail(1250), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            await AddNewItem(await MediaPicker.CaptureVideoAsync());
        }

        private async Task AddNewItem(FileResult file)
        {
            if (file == null) return;

            if (Source.ToList().Find(x => x[FileName].ToString() == file.FileName) is not null)
            {
                MessagingCenter.Send(new FMessage(0, 1101, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            var fileInfo = new FileInfo(await FUtility.GetFilePath(file));
            if (fileInfo == null || (fileMaxSize != -1 && (fileInfo.Length == 0 || (fileInfo.Length / 1024) > fileMaxSize)))
            {
                MessagingCenter.Send(new FMessage(0, 1102, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if (await NewFromFile(file, fileInfo) is not FData data)
            {
                MessagingCenter.Send(new FMessage("File not exists."), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            Source.Add(data);
            AddFiles(data[FilePath], data.LineNumberRow, (byte[])data[FileData], "Insert", false);
            await ChangedView();
        }

        private async Task OnDeleteItem(object obj, FData data)
        {
            Grid.SelectedIndex = Source.IndexOf(data) + 1;
            await Root.SetBusy(true);
            if (IsModifier && (Settings.ClearMode || await FAlertHelper.Confirm("805")))
            {
                await Task.Delay(100);
                await DeleteItem();
                if (FFunc.StringToBoolean(data[FileClient].ToString()))
                    AddFiles(data[FilePath], data.LineNumberRow, null, "Delete", true);
                else
                    delData.Add(data);
            }
            await Root.SetBusy(false);
        }

        private async Task OnDownloadItem(object obj, FData data)
        {
            if (data[FileClient].Equals(true))
            {
                MessagingCenter.Send(FMessage.FromFail(900), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if(!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
            {
                FPermissions.ShowMessage();
                return;
            }

            await Root.SetBusy(true);

            var mess = await Downloading();
            if (!mess.Success.Equals(1))
            {
                MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                await Root.SetBusy(false);
                return;
            }

            await FReportToolbar.TryCatchMessage(Root, Root.Page, mess.ToDataSet(), 1, async (dt) =>
            {
                FInterface.IFDownload.OnFileDownloaded += FUtility.DownloadCompleted;
                await FInterface.IFDownload.DownloadFile(FServices.DownloadUrl(Root.Controller, "", data[FileEnc].ToString(), data[Key].ToString(), data.LineNumberRow.ToString(), dt.Rows[0][Ticket].ToString()), data[FileName].ToString());
                FInterface.IFDownload.OnFileDownloaded -= FUtility.DownloadCompleted;
            });
            await Root.SetBusy(false);
        }

        private async Task<FData> NewFromFile(FileResult file, FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return null;

            var data = new FData();
            data.Data.Add(FileName, fileInfo.Name);
            data.Data.Add(FileData, await FUtility.GetFileData(file));
            data.Data.Add(FileSize, FUtility.GetFileSize(fileInfo.Length, 2));
            data.Data.Add(FileExt, fileInfo.Extension);
            data.Data.Add(FileTimeCreated, fileInfo.CreationTime.ToDate(FInputDate.FormatText));
            data.Data.Add(FileTimeEdited, fileInfo.LastWriteTime.ToDate(FInputDate.FormatText));
            data.Data.Add(FilePath, fileInfo.FullName);
            data.Data.Add(FileClient, true);
            data.Data.Add(FileTimeUpload, DateTime.Now.ToDate(FormatDate));
            return data;
        }

        private void AddFiles(object filePath, object line, byte[] fileData, string type, bool remove)
        {
            if (remove)
                files.RemoveAll(f => f.LineNbrMode == $"{line}.Insert");
            else
                files.Add(new FFileInfo(filePath?.ToString(), $"{line}.{type}", fileData, null));
        }

        protected virtual async Task<FMessage> Downloading()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", Target, controllerParent: Root.Controller);
            FFunc.AddFieldInput(ref ds, new DataTable(), Settings.Code, Root.InputData, true, Settings.Reference);
            FMessage result = await FServices.ExecuteCommand("Download", Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        #endregion private
    }
}