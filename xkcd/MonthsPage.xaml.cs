using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using xkcd.DataModel;

namespace xkcd
{
    public sealed partial class MonthsPage
    {
        private readonly ObservableCollection<DateTime> _months = new ObservableCollection<DateTime>();
        private int _year;
        bool _handleDataEvent;

        public MonthsPage()
        {
            InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _year = (int)navigationParameter;

            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                _handleDataEvent = true;
            }

            RefreshCollection();

            DefaultViewModel["Months"] = _months;
            DefaultViewModel["Year"] = _year;
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetMonths(_year);
            _months.Clear();
            foreach (DateTime month in newCollection)
            {
                _months.Add(month);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var month = (DateTime)e.ClickedItem;
            Frame.Navigate(typeof(ComicsPage), month.ToString());
        }
    }
}
