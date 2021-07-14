using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public partial class FPageWebView : FPage
    {
        public event EventHandler<EventArgs> OkClicked, CancelClicked, ClosedClicked, NextClicked, BackClicked;

        protected DataSet Result { get; private set; }

        protected bool MessageWhenFailed { get; private set; } = true;
        protected bool MessageWhenSuccess { get; private set; } = true;
        protected bool NextWhenSuccess { get; private set; } = false;
        protected bool DeleteWhenSuccess { get; private set; } = false;
        protected bool NextWhenFailed { get; private set; } = false;
        protected bool DeleteWhenFailed { get; private set; } = false;
        protected bool HasComment { get; private set; } = false;
        protected bool HasBio { get; private set; } = false;
        protected bool BioNative { get; private set; } = false;
        protected string CommnetText => HasComment ? E.Value : string.Empty;

        protected readonly string Action, Controller, Method, ID;
        protected readonly FButton Ok, Cancel, Closed, Next, Back;
        protected string CommentTitle, CommentCacheText;
        protected int CommentMaxLength;
        protected bool CommentCache;

        protected DataRow R0 => Result.Tables[0].Rows[0];
        protected DataRow R1 => Result.Tables[1].Rows[0];

        private readonly bool IsApproval;
        private readonly DataSet Data;

        private readonly ScrollView S;
        private readonly Grid V, B, D;
        private readonly WebView WebView;
        private readonly FInputTextUnderline E;
        private readonly FLine L;

        public FPageWebView(FWebViewType type, bool isHasPullToRefresh) : base(isHasPullToRefresh, false)
        {
            IsApproval = type == FWebViewType.Approval;
            NothingText = FText.NoData;

            V = new Grid();
            B = new Grid();
            S = new ScrollView();
            D = new Grid();
            E = new FInputTextUnderline();
            L = new FLine();
            WebView = new WebView();

            Ok = new FButton(FText.Approve, FIcons.Check);
            Cancel = new FButton(FText.Cancel, FIcons.Close);
            Closed = new FButton(FText.Close, FIcons.ChevronLeft);
            Next = new FButton("", FIcons.ArrowRight, FIcons.ArrowRight);
            Back = new FButton("", FIcons.ArrowLeft, FIcons.ArrowLeft);
            InitView();
        }

        public FPageWebView(FWebViewType type, string action, string controller, string iDContent, DataSet dataRequest, string method, bool isHasPullToRefresh) : base(isHasPullToRefresh, false)
        {
            Title = FText.Detail;
            Action = action;
            Controller = controller;
            Method = method;
            ID = iDContent;
            Data = dataRequest;
            IsApproval = type == FWebViewType.Approval;
            NothingText = FText.NoData;

            V = new Grid();
            B = new Grid();
            S = new ScrollView();
            D = new Grid();
            E = new FInputTextUnderline();
            L = new FLine();
            WebView = new WebView();

            Ok = new FButton(FText.Approve, FIcons.Check);
            Cancel = new FButton(FText.Cancel, FIcons.Close);
            Closed = new FButton(FText.Close, FIcons.ChevronLeft);
            Next = new FButton("", FIcons.ArrowRight, FIcons.ArrowRight);
            Back = new FButton("", FIcons.ArrowLeft, FIcons.ArrowLeft);
            InitView();
        }

        public override async void Init()
        {
            base.Init();
            if (!HasNetwork)
                return;
            await OnLayout();
        }

        public void SetHtml(string html)
        {
            WebView.Source = new HtmlWebViewSource { Html = html.Replace("@@isApprove", IsApproval ? "1" : "0") };
        }

        private async Task OnLayout()
        {
            await SetBusy(true);
            E.IsVisible = L.IsVisible = false;
            var message = await FServices.ExecuteCommand(Action, Controller, Data, Method, null, ID);
            if (message.Success != 1)
            {
                MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                ShowNothing = true;
                await SetBusy(false);
                return;
            }
            try
            {
                Result = message.ToDataSet();
                if (Result.Tables.Count == 0 || Result.Tables[0].Rows.Count == 0)
                {
                    ShowNothing = true;
                    await SetBusy(false);
                    return;
                }
                var htmlSource = GetHtml();
                if (string.IsNullOrEmpty(htmlSource))
                {
                    ShowNothing = true;
                    await SetBusy(false);
                    return;
                }
                UpdateIsApproval();
                SetHtml(htmlSource);
                UpdateBadge();
                UpdateComment();
                ShowNothing = false;
                await SetBusy(false);
            }
            catch (Exception ex)
            {
                await SetBusy(false);
                ShowNothing = true;
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        protected virtual string GetHtml()
        {
            return FHtml.DataSetToHtml(Result)?.Replace("@@isApprove", IsApproval ? "1" : "0");
        }

        protected override void OnTabbedTryConnect(object sender, EventArgs e)
        {
            Init();
            base.OnTabbedTryConnect(sender, e);
        }

        protected virtual void OnAcceptClicked(object sender, EventArgs e)
        {
            OkClicked?.Invoke(sender, e);
        }

        protected virtual void OnCancelClicked(object sender, EventArgs e)
        {
            CancelClicked?.Invoke(sender, e);
        }

        protected virtual void OnClosedClicked(object sender, EventArgs e)
        {
            ClosedClicked?.Invoke(sender, e);
        }

        protected virtual void OnNextClicked(object sender, EventArgs e)
        {
            NextClicked?.Invoke(sender, e);
        }

        protected virtual void OnBackClicked(object sender, EventArgs e)
        {
            BackClicked?.Invoke(sender, e);
        }

        private void InitView()
        {
            S.Content = B;
            S.Orientation = ScrollOrientation.Horizontal;
            S.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

            D.ColumnSpacing = FSetting.SpacingButtons;
            D.HorizontalOptions = B.HorizontalOptions = LayoutOptions.Fill;

            V.RowSpacing = 0;
            V.HorizontalOptions = LayoutOptions.Fill;
            V.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            V.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            V.Children.Add(WebView, 0, 0);

            if (IsApproval)
            {
                B.ColumnSpacing = FSetting.SpacingButtons;
                B.Padding = new Thickness(10, 0);

                Ok.Clicked += OnAcceptClicked;
                Cancel.Clicked += OnCancelClicked;
                Closed.Clicked += OnClosedClicked;
                Next.Clicked += OnNextClicked;
                Back.Clicked += OnBackClicked;

                E.ClearButtonVisibility = ClearButtonVisibility.WhileEditing;
                E.Rendering();
                V.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                V.Children.Add(L, 0, V.RowDefinitions.Count - 1);
                V.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                V.Children.Add(E, 0, V.RowDefinitions.Count - 1);
                E.IsVisible = L.IsVisible = false;
            }

            WebView.Navigating += OnNavigatingWeb;
            Content = V;
        }

        private async void OnNavigatingWeb(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.IsUrl())
                return;

            e.Cancel = true;
            await e.Url.OpenBrowser();
        }

        protected virtual void UpdateBadge()
        {
        }

        public void OnClearCacheComment()
        {
            if (!HasComment) return;
            FApprovalComment.RemoveSetting(E.CacheInput);
        }

        private void UpdateIsApproval()
        {
            if (!IsApproval || Result.Tables.Count < 1 || Result.Tables[1].Rows.Count == 0) return;

            if (R1.Table.Columns.Contains("alertSuccess"))
                MessageWhenSuccess = R1["alertSuccess"].Equals(1);

            if (R1.Table.Columns.Contains("alertFail"))
                MessageWhenFailed = R1["alertFail"].Equals(1);

            if (R1.Table.Columns.Contains("nextSuccess"))
                NextWhenSuccess = R1["nextSuccess"].Equals(1);

            if (R1.Table.Columns.Contains("nextFail"))
                NextWhenFailed = R1["nextFail"].Equals(1);

            if (R1.Table.Columns.Contains("deleteSuccess"))
                DeleteWhenSuccess = R1["deleteSuccess"].Equals(1);

            if (R1.Table.Columns.Contains("deleteFail"))
                DeleteWhenFailed = R1["deleteFail"].Equals(1);

            if (R1.Table.Columns.Contains("approvalText"))
                Ok.Text = R1["approvalText"].ToString();

            if (R1.Table.Columns.Contains("cancelText"))
                Cancel.Text = R1["cancelText"].ToString();

            if (R1.Table.Columns.Contains("closeText"))
                Closed.Text = R1["closeText"].ToString();

            if (R1.Table.Columns.Contains("nextText"))
                Next.Text = R1["nextText"].ToString();

            if (R1.Table.Columns.Contains("backText"))
                Back.Text = R1["backText"].ToString();

            if (R1.Table.Columns.Contains("buttons"))
                UpdateButton(R1["buttons"].ToString());
            else UpdateButton("1,2,3");

            if (R1.Table.Columns.Contains("comment"))
                HasComment = R1["comment"].Equals(1);

            if (R1.Table.Columns.Contains("bio"))
                HasBio = Convert.ToBoolean(R1["bio"]);

            if (R1.Table.Columns.Contains("bio_native"))
                BioNative = Convert.ToBoolean(R1["bio_native"]);

            if (R1.Table.Columns.Contains("commentTitle"))
                CommentTitle = R1["commentTitle"].ToString();

            if (R1.Table.Columns.Contains("commentMaxLength"))
                CommentMaxLength = Convert.ToInt32(R1["commentMaxLength"]);
            else
                CommentMaxLength = 256;

            if (R1.Table.Columns.Contains("cacheName"))
            {
                CommentCache = true;
                CommentCacheText = R1["cacheName"].ToString();
            }
            else
            {
                CommentCache = false;
                CommentCacheText = string.Empty;
            }
        }

        private void UpdateComment()
        {
            E.IsVisible = L.IsVisible = HasComment;
            if (!IsApproval || !HasComment) return;
            E.Title = CommentTitle;
            E.TitleColor = FSetting.TextColorTitle;
            E.MaxLength = CommentMaxLength;
            E.CacheName = CommentCache ? $"FastMobile.FXamarin.Core.FPageWebView.IsApproval.{ID}.{CommentCacheText}" : string.Empty;
            if (CommentCache) FApprovalComment.AddSetting(E.CacheInput);
            E.RequestAction = string.Empty;
            E.HandleField = new List<string>();
            E.ScriptReference = new List<string>();
            E.InitValue(true);
            E.Unfocus();
        }

        private void UpdateButton(string position)
        {
            if (string.IsNullOrEmpty(position))
                return;
            var arr = new List<string>(position.Split(","));
            int indexBack = arr.IndexOf("5"), indexNext = arr.IndexOf("4"), count = arr.Count;

            if ((indexNext == count - 1 && indexBack == -1) || (indexBack == count - 1 && indexNext == -1) || (indexNext >= count - 2 && indexBack >= count - 2))
            {
                AddButton(D, S, 0, GridLength.Star);
                AddOkButtons(B, arr);
                if (indexNext >= 0)
                    AddButton(D, Next, indexNext < indexBack ? 1 : indexBack < 0 ? 1 : 2, GridLength.Auto);
                if (indexBack >= 0)
                    AddButton(D, Back, indexBack < indexNext ? 1 : indexNext < 0 ? 1 : 2, GridLength.Auto);
                AfterInitButton();
                return;
            }

            AddOkButtons(D, arr);
            AddButton(D, Next, indexNext, GridLength.Auto);
            AddButton(D, Back, indexBack, GridLength.Auto);
            AfterInitButton();
            return;
        }

        private void AddOkButtons(Grid view, List<string> arr)
        {
            AddButton(view, Ok, arr.IndexOf("1"), GridLength.Auto);
            AddButton(view, Cancel, arr.IndexOf("2"), GridLength.Auto);
            AddButton(view, Closed, arr.IndexOf("3"), GridLength.Auto);
        }

        private void AddButton(Grid views, View v, int index, GridLength length)
        {
            if (index < 0)
                return;
            views.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
            views.Children.Add(v, index, 0);
        }

        private void AfterInitButton()
        {
            V.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            V.RowDefinitions.Add(new RowDefinition { Height = 50 });
            V.Children.Add(new FLine(), 0, V.RowDefinitions.Count - 2);
            V.Children.Add(D, 0, V.RowDefinitions.Count - 1);
        }
    }
}