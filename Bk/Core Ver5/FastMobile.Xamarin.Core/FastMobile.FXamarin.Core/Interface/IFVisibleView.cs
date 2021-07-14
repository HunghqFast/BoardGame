namespace FastMobile.FXamarin.Core
{
    public interface IFVisibleView
    {
        bool IsShouldVisible();

        bool IsShouldVisible(object value);
    }
}