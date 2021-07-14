using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FProfileModel : BindableObject
    {
        public static readonly BindableProperty DataSourceProperty = BindableProperty.Create("DataSource", typeof(ObservableCollection<FItemProfile>), typeof(FProfileModel), null);

        public ObservableCollection<FItemProfile> DataSource
        {
            get => (ObservableCollection<FItemProfile>)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        private readonly SQLiteAsyncConnection database;

        public FProfileModel(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            Init();
        }

        public async Task InitPrimary(IEnumerable<FItemProfile> sources)
        {
            await sources.ForEachAsync(async x =>
             {
                 try
                 {
                     var profiles = await database.Table<FItemProfile>().Where((z) => z.ID == x.ID).ToListAsync();
                     if (profiles == null || profiles.Count < 1)
                         await database.InsertAsync(x);
                     else
                         await database.UpdateAsync(x);
                 }
                 catch { }
             });
        }

        public async Task InitItemsAsync()
        {
            DataSource = new ObservableCollection<FItemProfile>(await database.Table<FItemProfile>().ToListAsync());
        }

        public Task<int> AddItemAsync(FItemProfile item)
        {
            DataSource.Add(item);
            return database.InsertAsync(item);
        }

        public Task<int> EditItemAsync(FItemProfile item)
        {
            return database.UpdateAsync(item);
        }

        public Task<int> RemoveItemAsync(FItemProfile item)
        {
            DataSource.Remove(item);
            return database.DeleteAsync(item);
        }

        public Task<int> ClearItemAsync()
        {
            DataSource.Clear();
            return database.DeleteAllAsync<FItemProfile>();
        }

        public Task<int> UpdateItemAsync(FItemProfile item)
        {
            return database.UpdateAsync(item);
        }

        public Task<int> UpdateAllAsync()
        {
            return database.UpdateAllAsync(DataSource);
        }

        public Task UnMarkAllAsync()
        {
            DataSource.ForEach(x => x.UnMark());
            return Task.CompletedTask;
        }

        public async Task<bool> Exists(int id)
        {
            try
            {
                return await database.GetAsync<FItemProfile>(id) != null;
            }
            catch
            {
                return false;
            }
        }

        public Task<FItemProfile> GetCurrentProfile(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = "1";
            var item = DataSource.ToList().Find(X => X.ID.ToString() == id);
            if (item != null)
                return Task.FromResult(item);
            if (DataSource.Count > 0)
                return Task.FromResult(DataSource[0]);
            return null;
        }

        private async void Init()
        {
            await database.CreateTableAsync<FItemProfile>();
        }
    }
}