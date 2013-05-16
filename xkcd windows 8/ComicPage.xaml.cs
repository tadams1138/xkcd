using System;
using System.Collections.Generic;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Printing;
using xkcd.DataModel;
using xkcd_windows_8.Common;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace xkcd_windows_8
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ComicPage : xkcd_windows_8.Common.LayoutAwarePage
    {
        public ComicPage()
        {
            this.InitializeComponent();
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
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var comic = ComicDataSource.GetItem((int)navigationParameter);;
            DefaultViewModel["Items"] = ComicDataSource.AllItems;
            FlipView.SelectedItem = comic;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (Comic)FlipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.Number;
        }

        private void WebsiteOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (Comic)FlipView.SelectedItem;
                Windows.System.Launcher.LaunchUriAsync(selectedItem.Uri);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.ToString());
                dialog.ShowAsync();
            }
        }

        private void ExplainationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (Comic)FlipView.SelectedItem;
                Windows.System.Launcher.LaunchUriAsync(selectedItem.ExplanationUri);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.ToString());
                dialog.ShowAsync();
            }
        }

        private async void ImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentComic = (Comic)FlipView.SelectedItem;
            var dialog = new MessageDialog(currentComic.AltText);
            await dialog.ShowAsync();
        }
    }
}
