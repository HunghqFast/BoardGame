using System.Collections.ObjectModel;

namespace FastMobile.FXamarin.Core
{
    public class FCacheArray : ObservableCollection<string>
    {
        private readonly string Key;
        private readonly char Sperator;

        private bool Inited;

        public FCacheArray(string keyWord, char sperator = ',')
        {
            Key = keyWord;
            Sperator = sperator;
            Inited = false;
            Init();
        }

        public override string ToString()
        {
            return string.Join(Sperator, this);
        }

        protected override void InsertItem(int index, string item)
        {
            if (!Contains(item)) base.InsertItem(index, item);
            if (Inited) FUtility.SetCache(string.Join(Sperator, this), Key);
        }

        protected override void ClearItems()
        {
            this.ForEach(x => x.RemoveCache());
            FUtility.SetCache("", Key);
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this[index].RemoveCache();
            base.RemoveItem(index);
            FUtility.SetCache(string.Join(Sperator, this), Key);
        }

        protected override void SetItem(int index, string item)
        {
            if (!Contains(item)) base.SetItem(index, item);
            FUtility.SetCache(string.Join(Sperator, this), Key);
        }

        private void Init()
        {
            var cache = Key.GetCache();
            if (string.IsNullOrWhiteSpace(cache))
            {
                Inited = true;
                return;
            }

            var list = cache.Split(Sperator);
            list.ForEach(x => Add(x));
            Inited = true;
        }
    }
}