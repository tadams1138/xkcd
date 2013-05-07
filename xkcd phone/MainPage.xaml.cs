using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void LatestButton_OnClick(object sender, RoutedEventArgs e)
        {
            int numberOfLatestComic = ComicDataSource.GetNumberOfLatestComic();
            NavigationService.Navigate(new Uri("/ComicPage.xaml?number=" + numberOfLatestComic.ToString(), UriKind.Relative));
        }

        private void ByDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/YearsPage.xaml", UriKind.Relative));
        }

        private void RandomButton_OnClick(object sender, RoutedEventArgs e)
        {
            int randomComicNumber = ComicDataSource.GetRandomComicNumber();
            NavigationService.Navigate(new Uri("/ComicPage.xaml?number=" + randomComicNumber.ToString(), UriKind.Relative));
        }

        private void WebsiteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask {Uri = new Uri("http://xkcd.com", UriKind.Absolute)};
            webBrowserTask.Show();
        }
    }
}