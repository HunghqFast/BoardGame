namespace FastMobile.FXamarin.Core
{
    public interface IFNotifyParent
    {
        void Read(string id);

        void ReadAll();

        void UnRead(string id);

        void UnReadAll();
    }
}