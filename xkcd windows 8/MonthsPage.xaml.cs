using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using xkcd.DataModel;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace xkcd_windows_8
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MonthsPage
    {
        private readonly ObservableCollection<DateTime> _months = new ObservableCollection<DateTime>();
        private int _year;
        bool _handleDataEvent;

        public MonthsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _year = (int)navigationParameter;

            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                _handleDataEvent = true;
            }

            RefreshCollection();

            DefaultViewModel["Items"] = _months;
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
            Frame.Navigate(typeof(DaysPage), month.ToString());
        }
    }
}
