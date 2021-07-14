using SQLite;
using System;

namespace FastMobile.FXamarin.Core
{
    public class FItemProfile : FObjectBase
    {
        private int id, stt;
        private string name, link, database, check, avatar, datecreate, isInternal;

        public FItemProfile()
        {
            Avatar = FIcons.Server;
            DateCreate = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => id;
            set
            {
                id = value;
                stt = Convert.ToInt32(FUtility.GetCache($"FastMobile.FXamarin.Core.FItemProfile.Stt.ID={value}", value.ToString()));
                OnPropertyChanged();
            }
        }

        [Ignore]
        public int Stt
        {
            get => stt;
            set
            {
                stt = value;
                FUtility.SetCache(value, $"FastMobile.FXamarin.Core.FItemProfile.Stt.ID={ID}");
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public string Link
        {
            get => link;
            set { link = value; OnPropertyChanged(); }
        }

        public string DatabaseName
        {
            get => database;
            set { database = value; OnPropertyChanged(); }
        }

        public string Current
        {
            get => check;
            set { check = value; OnPropertyChanged(); }
        }

        public string Avatar
        {
            get => avatar;
            private set { avatar = value; OnPropertyChanged(); }
        }

        public string DateCreate
        {
            get => datecreate;
            set { datecreate = value; OnPropertyChanged(); }
        }

        public string IsInternal
        {
            get => isInternal;
            set { isInternal = value; OnPropertyChanged(); }
        }

        public void Mark()
        {
            Current = FIcons.Check;
        }

        public void UnMark()
        {
            Current = "";
        }
    }
}