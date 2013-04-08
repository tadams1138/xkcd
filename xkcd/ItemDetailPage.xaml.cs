using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;
using xkcd.Common;
using xkcd.DataModel;

namespace xkcd
{
    public sealed partial class ItemDetailPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var comic = ComicDataSource.GetItem((int)navigationParameter);
            DefaultViewModel["Group"] = ComicDataSource.GetComics(comic.Date.Year, comic.Date.Month);
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
            var currentComic = (Comic)FlipView.SelectedItem;
            pageState["SelectedItem"] = currentComic.Number;
        }

        private async void ImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentComic = (Comic)FlipView.SelectedItem;
            var dialog = new MessageDialog(currentComic.AltText);
            await dialog.ShowAsync();
        }
    }
}
