using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using xkcd.DataModel;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace xkcd_windows_8
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage
    {
        private readonly ObservableCollection<Comic> _comics = new ObservableCollection<Comic>();
        private string _queryText;
        bool _handleDataEvent;

        public SearchResultsPage()
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
            if (!_handleDataEvent)
            {
                ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                SearchPane.GetForCurrentView().QuerySubmitted += OnQuerySubmitted;
                _handleDataEvent = true;
            }

            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("QueryText"))
            {
                navigationParameter = pageState["QueryText"];
            }

            GetQueryResults(navigationParameter as string);

            DefaultViewModel["Results"] = _comics;
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            pageState["QueryText"] = _queryText;
        }

        private void GetQueryResults(string queryText)
        {
            _queryText = queryText;
            DefaultViewModel["QueryText"] = '\u201c' + _queryText + '\u201d';
            RefreshCollection();
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetComics(_queryText);
            _comics.Clear();
            foreach (Comic c in newCollection)
            {
                _comics.Add(c);
            }

            string stateName = _comics.Any() ? "ResultsFound" : "NoResultsFound";
            VisualStateManager.GoToState(this, stateName, true);
        }

        private void OnQuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            GetQueryResults(args.QueryText);
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var comic = (Comic)e.ClickedItem;
            Frame.Navigate(typeof(ComicPage), comic._num);
        }
    }
}
