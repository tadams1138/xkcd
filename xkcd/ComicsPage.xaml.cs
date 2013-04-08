namespace xkcd
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Windows.UI.Xaml.Controls;
    using DataModel;

    public sealed partial class ComicsPage
    {
        private readonly ObservableCollection<Comic> _comics = new ObservableCollection<Comic>();
        private DateTime _month;
        bool _handleDataEvent;

        public ComicsPage()
        {
            InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _month = DateTime.Parse((string)navigationParameter);

            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                _handleDataEvent = true;
            }

            RefreshCollection();

            DefaultViewModel["Comics"] = _comics;
            DefaultViewModel["Month"] = _month.ToString("MMMM yyyy");
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetComics(_month.Year, _month.Month);
            _comics.Clear();
            foreach (Comic c in newCollection)
            {
                _comics.Add(c);
            }
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var comic = (Comic)e.ClickedItem;
            Frame.Navigate(typeof(ItemDetailPage), comic.Number);
        }
    }
}
