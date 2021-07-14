namespace FastMobile.FXamarin.Core
{
    public class FApprovalComment
    {
        private static FCacheArray ApprovalReference { get; }

        static FApprovalComment()
        {
            ApprovalReference = new FCacheArray("FastMobile.FXamarin.Core.FApprovalComment.CommentID");
        }

        static public void ClearAll()
        {
            ApprovalReference.Clear();
        }

        static public void AddSetting(string id)
        {
            ApprovalReference.Add(id);
        }

        static public void RemoveSetting(string id)
        {
            ApprovalReference.Remove(id);
        }

        static public string GetSetting(string id)
        {
            if (string.IsNullOrEmpty(id.GetCache()))
                return string.Empty;
            return id.GetCache();
        }
    }
}