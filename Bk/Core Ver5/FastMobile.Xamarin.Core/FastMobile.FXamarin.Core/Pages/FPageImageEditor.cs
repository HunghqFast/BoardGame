namespace FastMobile.FXamarin.Core
{
    public class FPageImageEditor : FPage
    {
        public FImageEditor Editor { get; }

        public FPageImageEditor() : base(false, false)
        {
            Editor = new FImageEditor();
            Content = Editor;
        }
    }
}