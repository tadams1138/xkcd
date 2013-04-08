namespace xkcd
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Windows.UI.Xaml.Controls;
    using DataModel;

    public sealed partial class YearsPage
    {
        private readonly ObservableCollection<int> _years = new ObservableCollection<int>();
        bool _handleDataEvent;

        public YearsPage()
        {
            InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                _handleDataEvent = true;
            }

            RefreshYearsCollection();

            DefaultViewModel["Years"] = _years;
        }

        private void RefreshYearsCollection()
        {
            var sampleDataGroups = ComicDataSource.GetYears();
            _years.Clear();
            foreach (int year in sampleDataGroups)
            {
                _years.Add(year);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshYearsCollection();
        }       
                
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var year = (int)e.ClickedItem;
            Frame.Navigate(typeof(MonthsPage), year);
        }
    }
}
