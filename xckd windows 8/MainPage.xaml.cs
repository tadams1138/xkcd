﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using xckd_windows_8.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace xckd_windows_8
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void GoToWebsite(object sender, RoutedEventArgs e)
        {
            try
            {
                const string uriToLaunch = @"http://xkcd.com/";
                var uri = new Uri(uriToLaunch);
                Windows.System.Launcher.LaunchUriAsync(uri);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.ToString());
                dialog.ShowAsync();
            }
        }

        private void GoToRandom(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ViewByDate(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(YearsPage), "AllYears");
        }

        private void ViewLatest(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
