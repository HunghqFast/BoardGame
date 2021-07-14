using FastMobile.Core;
using FastMobile.FXamarin.Core;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.Device
{
    public partial class App : CApp
    {
        public App() : base()
        {
        }

        public override void ReadById(string id, string group)
        {
            (Notify as PageNotifyGroup)?.System?.Model?.Read(id);
            (Notify as PageNotifyGroup)?.News?.Model?.Read(id);
            (Notify as PageNotifyGroup)?.Promotion?.Model?.Read(id);
        }

        public override Task LoadById(string id, string group)
        {
            return group switch
            {
                PageNotifyGroup.SystemGroup => (Notify as PageNotifyGroup)?.System?.Model?.LoadOne(id),
                PageNotifyGroup.NewsGroup => (Notify as PageNotifyGroup)?.News?.Model?.LoadOne(id),
                PageNotifyGroup.PromotionGroup => (Notify as PageNotifyGroup)?.Promotion?.Model?.LoadOne(id),
                _ => Task.CompletedTask,
            };
        }

        protected override void Init()
        {
            FServices.Init("2.3", "01");
            FSetting.Init(FAppMode.FE, FConfigMode.Publish);
        }

        protected override void SetNotify()
        {
            Notify = new PageNotifyGroup() { Title = FText.Notify.ToUpper() };
        }

        //protected override void SetLogin(bool showBiometric = true)
        //{
        //    base.SetLogin();
        //    var login = new PageLogin(showBiometric);
        //    login.Init();
        //    MainPage = new FNavigationPage(login);
        //}

        //protected override void SetLoginWhenTimeOut()
        //{
        //    base.SetLoginWhenTimeOut();
        //    MainPage = new FNavigationPage(new PageLogin(false).Init(true));
        //}

        protected override Page GetLoginWhenTimeOut()
        {
            return new FNavigationPage(new PageLogin(true).Init(true));
        }
    }
}