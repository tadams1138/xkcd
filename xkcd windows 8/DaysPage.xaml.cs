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
    public sealed partial class DaysPage
    {
        private readonly ObservableCollection<Comic> _comics = new ObservableCollection<Comic>();
        private DateTime _month;
        bool _handleDataEvent;

        public DaysPage()
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
            _month = DateTime.Parse((string)navigationParameter);

            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                _handleDataEvent = true;
            }

            RefreshCollection();

            DefaultViewModel["Items"] = _comics;
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
            Frame.Navigate(typeof(ComicPage), comic._num);
        }
    }
}
