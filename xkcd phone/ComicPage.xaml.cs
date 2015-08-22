using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Navigation;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class ComicPage
    {
        // TODO: fix zoom for large pics
        // TODO: research crashes
        // TODO: live tile to know when new comics are available

        private const string NumberKey = "number";
        private int _currentComicNumber;

        public ComicPage()
        {
            InitializeComponent();
            DataContext = ComicDataSource.AllItems;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string number;
            Comic comic;

            if (NavigationContext.QueryString.TryGetValue(NumberKey, out number))
            {
                _currentComicNumber = Int32.Parse(number);
                comic = ComicDataSource.GetComic(_currentComicNumber);
            }
            else if (State.ContainsKey(NumberKey))
            {
                _currentComicNumber = (int)State[NumberKey];
                comic = ComicDataSource.GetComic(_currentComicNumber);
            }
            else
            {
                // On app load...
                comic = ComicDataSource.GetLatestComic();
                _currentComicNumber = comic.Number;
            }

            SlideView.SelectedItem = comic;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (State.ContainsKey(NumberKey))
            {
                State[NumberKey] = _currentComicNumber;
            }
            else
            {
                State.Add(NumberKey, _currentComicNumber);
            }
        }

        private void PanAndZoomImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedComic = (Comic)SlideView.SelectedItem;
            MessageBox.Show(selectedComic.AltText);
        }

        private void RandomClick(object sender, EventArgs e)
        {
            SlideView.SelectedItem = ComicDataSource.GetRandomComic();
        }

        private void ByDateClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YearsPage.xaml", UriKind.Relative));
        }

        private void LatestClick(object sender, EventArgs e)
        {
            SlideView.SelectedItem = ComicDataSource.GetLatestComic();
        }

        private void WebsiteClick(object sender, EventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var webBrowserTask = new WebBrowserTask { Uri = selected.Uri };
            webBrowserTask.Show();
        }

        private void ExplanationClick(object sender, EventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var webBrowserTask = new WebBrowserTask { Uri = selected.ExplanationUri };
            webBrowserTask.Show();
        }

        private void RateAndReviewClick(object sender, EventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void ShareLinkClick(object sender, EventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var shareLinkTask = new ShareLinkTask {Title = "xkcd", LinkUri = selected.Uri, Message = selected.Title};
            shareLinkTask.Show();
        }

        private void EmailLinkClick(object sender, EventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var emailComposeTask = new EmailComposeTask
                {
                    Subject = "xkcd - " + selected.Title,
                    Body = selected.Uri.ToString()
                };
            emailComposeTask.Show();
        }

        private void TextLinkClick(object sender, EventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var smsComposeTask = new SmsComposeTask
                {
                    Body = "xkcd - " + selected.Title + Environment.NewLine + selected.Uri
                };
            smsComposeTask.Show();
        }

        private void SlideView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            _currentComicNumber = selected.Number;
        }

        private void SearchClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void PrivacyClick(object sender, EventArgs e)
        {
            var webBrowserTask = new WebBrowserTask { Uri = new Uri("http://tadams1138.blogspot.com/p/xkcd-the.html") };
            webBrowserTask.Show();
        }
    }
}