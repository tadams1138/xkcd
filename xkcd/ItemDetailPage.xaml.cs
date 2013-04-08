using xkcd.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace xkcd
{
    public sealed partial class ItemDetailPage : xkcd.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var comic = ComicDataSource.GetItem((int)navigationParameter);
            this.DefaultViewModel["Group"] = ComicDataSource.GetComics(comic.Date.Year, comic.Date.Month);
            this.DefaultViewModel["Items"] = ComicDataSource.AllItems;
            this.flipView.SelectedItem = comic;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var currentComic = (Comic)this.flipView.SelectedItem;
            pageState["SelectedItem"] = currentComic.Number;
        }

        private async void ImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentComic = (Comic)this.flipView.SelectedItem;
            MessageDialog dialog = new MessageDialog(currentComic.AltText);
            await dialog.ShowAsync();
        }
    }
}
