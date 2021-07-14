using System.Data;

namespace FastMobile.FXamarin.Core
{
    public class FCommnadValue
    {
        public DataTable Table;
        public bool Result;

        public FCommnadValue()
        {
            Table = null;
            Result = false;
        }

        public FCommnadValue(DataTable table, bool result)
        {
            Table = table;
            Result = result;
        }
    }
}