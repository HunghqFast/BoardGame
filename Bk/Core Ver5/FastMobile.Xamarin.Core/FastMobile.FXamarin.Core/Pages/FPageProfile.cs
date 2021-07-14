using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace FastMobile.FXamarin.Core
{
    public class FPageProfile : FPage
    {
        public static readonly BindableProperty CurrentServiceNameProperty = BindableProperty.Create("CurrentServiceName", typeof(string), typeof(FPageProfile), string.Empty);

        public string CurrentServiceName
        {
            get => (string)GetValue(CurrentServiceNameProperty);
            set => SetValue(CurrentServiceNameProperty, value);
        }

        public static bool IsOldProfile
        {
            get => Convert.ToBoolean("FastMobile.FXamarin.Core.FPageProfile.IsOldProfile".GetCache(bool.FalseString));
            set => value.SetCache("FastMobile.FXamarin.Core.FPageProfile.IsOldProfile");
        }

        public FProfileModel Model { get; }
        public List<FItemProfile> PrimarySource { get; }
        public ToolbarItem ToolbarAdd { get; }
        public FPageProfileInput Form { get; }

        public event EventHandler<IFDataEvent> ItemTapped;

        private bool IsEdit;
        private readonly SfListView ListView;
        private FItemProfile CurrentSwipe, CurrentProfile;

        public FPageProfile(bool IsHasPullToRefresh = false, bool enalbleScrolling = false) : base(IsHasPullToRefresh, enalbleScrolling)
        {
            Model = new FProfileModel(Path.Combine(DependencyService.Get<IFEnvironment>().PersionalPath, $"{Regex.Replace(FCrypto.AESEncrypt(AppInfo.PackageName, FCrypto.Key), @"[^A-Z]+", "")}.db3"));
            ListView = new SfListView();
            Form = new FPageProfileInput();
            PrimarySource = new List<FItemProfile>();
            ToolbarAdd = new ToolbarItem();
            Base();
            Update(FSetting.V);
        }

        public override async void Init()
        {
            await SetBusy(true);
            await InitDataSource();
            await SetBusy(false);
        }

        public void Update(bool v)
        {
            Title = FText.ProfileDeclare;
            Form.Update(v);
        }

        private void Base()
        {
            Form.Submiter.Clicked += AcceptClicked;
            ListView.BindingContext = Model;
            ListView.ItemSpacing = 0;
            ListView.AllowSwiping = true;
            ListView.IsScrollingEnabled = true;
            ListView.IsScrollBarVisible = false;
            ListView.SwipeOffset = 140;
            ListView.AutoFitMode = AutoFitMode.DynamicHeight;
            ListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.None;
            ListView.VerticalOptions = ListView.HorizontalOptions = LayoutOptions.Fill;
            ListView.DragStartMode = DragStartMode.OnHold;
            ListView.ItemTemplate = new DataTemplate(typeof(FTLItemProfile));
            ListView.RightSwipeTemplate = new DataTemplate(() => new FTLSwipeAll(Edit, Remove, FIcons.PencilOutline.ToFontImageSource(Color.White, FSetting.SizeIconButton), FIcons.TrashCanOutline.ToFontImageSource(Color.White, FSetting.SizeIconButton), "", ""));
            ListView.ItemTapped += OnItemtabbed;
            ListView.ItemDragging += OnItemDrag;
            ListView.SetBinding(SfListView.ItemsSourceProperty, FProfileModel.DataSourceProperty.PropertyName);
            CreateSorting();

            ToolbarAdd.Clicked += ToolbarAddItemClicked;
            ToolbarAdd.IconImageSource = FIcons.LinkVariantPlus.ToFontImageSource(Color.White, FSetting.SizeIconToolbar);
            ToolbarItems.Add(ToolbarAdd);
            Content = ListView;
        }

        private async Task InitDataSource()
        {
            await Model.InitPrimary(PrimarySource);
            await Model.InitItemsAsync();
            await DefaultCurrent(true);
        }

        private async Task DefaultCurrent(bool setCurrent = false)
        {
            if (!string.IsNullOrEmpty(FString.ServiceUrl) || FString.ServiceUrl.IsUrl())
            {
                CurrentProfile = await Model.GetCurrentProfile(FString.ServiceID);
                if (CurrentProfile == null)
                    return;
                await Model.UnMarkAllAsync();
                CurrentProfile.Mark();
                if (setCurrent)
                {
                    await SaveCurrent(CurrentProfile);
                    await Model.UpdateAllAsync();
                }
                CurrentServiceName = CurrentProfile.Name;
            }
            else
            {
                if (Model.DataSource.Count > 0)
                {
                    CurrentProfile = Model.DataSource[0];
                    CurrentProfile.Mark();
                    await SaveCurrent(CurrentProfile);
                    CurrentServiceName = CurrentProfile.Name;
                }
                else
                    CurrentServiceName = FText.ProfileDeclare;
            }
        }

        private async void Edit(object obj)
        {
            if (obj is not FItemProfile current)
                return;
            CurrentSwipe = current;
            if (IsPrimaryID(CurrentSwipe.ID))
            {
                MessagingCenter.Send(new FMessage(0, 1200, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            IsEdit = true;
            Form.Link.Value = CurrentSwipe.Link;
            Form.Name.Value = CurrentSwipe.Name;
            Form.Database.Value = CurrentSwipe.DatabaseName;
            Form.UpdateIcon(true);
            await Navigation.PushAsync(Form, true);
        }

        private async void Remove(object obj)
        {
            if (obj is not FItemProfile current)
                return;
            CurrentSwipe = current;
            if (IsPrimaryID(CurrentSwipe.ID))
            {
                MessagingCenter.Send(new FMessage(0, 1200, ""), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            if (await FAlertHelper.Confirm("805"))
            {
                if (Model.DataSource.Count < 2)
                {
                    CurrentServiceName = FText.ProfileDeclare;
                    FString.RemoveCurrentProrfile();
                    await Model.ClearItemAsync();
                }
                else
                {
                    if (FString.ServiceID == CurrentSwipe.ID.ToString())
                        FString.RemoveCurrentProrfile();
                    await Model.RemoveItemAsync(CurrentSwipe);
                }
                CreateSorting();
            }
            await DefaultCurrent();
        }

        private async Task SaveWhenClick()
        {
            if (IsEdit)
            {
                await EditItem();
                return;
            }

            var randomID = new Random().Next(1000, 100000);
            while (await Model.Exists(randomID))
                randomID = new Random().Next(1000, 100000);
            await Model.AddItemAsync(new FItemProfile { Link = Form.Link.Value, Name = Form.Name.Value, DatabaseName = Form.Database.Value, IsInternal = "0", ID = randomID, Stt = FindMax() + 1 });
        }

        private bool IsPrimaryID(int id)
        {
            return PrimarySource.Find(x => x.ID == id) != null;
        }

        private async Task EditItem()
        {
            CurrentSwipe.Link = Form.Link.Value;
            CurrentSwipe.Name = Form.Name.Value;
            CurrentSwipe.DatabaseName = Form.Database.Value;

            if (FString.ServiceID == CurrentSwipe.ID.ToString())
            {
                CurrentServiceName = CurrentSwipe.Name;
                await SaveCurrent(CurrentSwipe);
            }

            await Model.EditItemAsync(CurrentSwipe);
        }

        private Task SaveCurrent(FItemProfile item)
        {
            if (item == null)
                return Task.CompletedTask;
            FString.SetCurrentProfile(item);
            return Model.EditItemAsync(item);
        }

        private void CreateSorting()
        {
            ListView.DataSource.SortDescriptors.Clear();
            ListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "Stt",
                Direction = ListSortDirection.Ascending,
                Comparer = FItemProfileSttComparer.Instance
            });
        }

        private async void ToolbarAddItemClicked(object sender, EventArgs e)
        {
            IsEdit = false;
            Form.Link.Value = Form.Name.Value = Form.Database.Value = "";
            Form.UpdateIcon(false);
            await Navigation.PushAsync(Form, true);
        }

        private void ProfileFormClosed(object sender, EventArgs e)
        {
            ListView.ResetSwipe(true);
        }

        private async void OnItemtabbed(object sender, ItemTappedEventArgs e)
        {
            if (CurrentProfile != null)
                CurrentProfile.UnMark();
            CurrentProfile = e.ItemData as FItemProfile;
            if (CurrentProfile == null)
                return;
            IsOldProfile = CurrentProfile.ID.ToString() == FString.ServiceID && CurrentProfile.Link == FString.ServiceUrl;
            CurrentServiceName = CurrentProfile.Name;
            CurrentProfile.Mark();
            await SaveCurrent(CurrentProfile);
            FInterface.IFFirebase?.RefreshToken();
            ItemTapped?.Invoke(sender, new FDataEventArgs(e.ItemData));
        }

        private async void AcceptClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Form.Name.Value))
            {
                Form.Name.Focus();
                MessagingCenter.Send(new FMessage(0, 606, Form.Name.Title), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            if (string.IsNullOrEmpty(Form.Link.Value) || !Form.Link.Value.IsUrl())
            {
                Form.Link.Focus();
                MessagingCenter.Send(new FMessage(0, 606, Form.Link.Title), FChannel.ALERT_BY_MESSAGE);
                return;
            }
            if (FSetting.AppMode == FAppMode.FBO && string.IsNullOrEmpty(Form.Database.Value))
            {
                Form.Database.Focus();
                MessagingCenter.Send(new FMessage(0, 606, Form.Database.Title), FChannel.ALERT_BY_MESSAGE);
                return;
            }

            if ((IsEdit && CurrentSwipe.Name != Form.Name.Value) || !IsEdit)
            {
                var item2 = Model.DataSource.ToList().Find(s => s.Name.ToLower() == Form.Name.Value?.Trim().ToLower());
                if (item2 != null)
                {
                    Form.Name.Focus();
                    MessagingCenter.Send(new FMessage(0, 614, Form.Name.Title + (char)254 + Form.Name.Value), FChannel.ALERT_BY_MESSAGE);
                    return;
                }
            }

            await SaveWhenClick();
            CreateSorting();
            await Form.Navigation.PopAsync(true);
        }

        private void OnItemDrag(object sender, ItemDraggingEventArgs e)
        {
            if (e.Action == DragAction.Drop)
                FUtility.RunAfter(UpdateSource, TimeSpan.FromSeconds(1));
        }

        void UpdateSource()
        {
            ListView.DataSource.DisplayItems.ForIndex((x, i) => (x as FItemProfile).Stt = i);
        }

        int FindMax()
        {
            int max = 0;
            Model.DataSource.ForEach(x =>
            {
                if (x.Stt > max)
                    max = x.Stt;
            });
            return max;
        }
    }
}