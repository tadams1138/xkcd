using Microsoft.Phone.Tasks;
using System;
using System.Windows;

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
            throw new NotImplementedException();
        }

        private void ByDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/YearsPage.xaml", UriKind.Relative));
        }

        private void RandomButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WebsiteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask {Uri = new Uri("http://xkcd.com", UriKind.Absolute)};
            webBrowserTask.Show();
        }
    }
}