using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using xkcd_windows_8.Common;
using xkcd.DataModel;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226
namespace xkcd_windows_8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private const string ComicsDataFile = "comics.xml";
        private readonly StorageFolder _storageFolder = ApplicationData.Current.LocalFolder;

        private Popup _settingsPopup;
        private Rect _windowBounds;
        private const double SettingsWidth = 646;

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            LoadComicDataSource();
            UpdateComicDataSource();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), "Main"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();

            _windowBounds = Window.Current.Bounds;

            // Added to listen for events when the window size is updated.
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            var aboutCommand = new SettingsCommand("AboutId", "About", OnAboutCommand);
            var privacyCommand = new SettingsCommand("PrivacyId", "Privacy Statement", OnPrivacyCommand);
            var rateAndReviewCommand = new SettingsCommand("RateAndReviewId", "Rate and Review", OnRateAndReviewCommand);
            eventArgs.Request.ApplicationCommands.Add(aboutCommand);
            eventArgs.Request.ApplicationCommands.Add(privacyCommand);
            eventArgs.Request.ApplicationCommands.Add(rateAndReviewCommand);
        }

        void OnAboutCommand(IUICommand command)
        {
            var mypane = new AboutFlyout();
            ShowFlyout(mypane);
        }

        void OnRateAndReviewCommand(IUICommand command)
        {
            var uri = new Uri("ms-windows-store:REVIEW?PFN=9525TISoftware.xkcdTheBrowser_bc0wgwa5kk60m");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        void OnPrivacyCommand(IUICommand command)
        {
            var uri = new Uri("http://tadams1138.blogspot.com/p/xkcd-the.html");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void ShowFlyout(LayoutAwarePage mypane)
        {
            _settingsPopup = new Popup();
            _settingsPopup.Closed += OnPopupClosed;
            Window.Current.Activated += OnWindowActivated;
            _settingsPopup.IsLightDismissEnabled = true;
            _settingsPopup.Width = SettingsWidth;
            _settingsPopup.Height = _windowBounds.Height;

            // Add the proper animation for the panel.
            _settingsPopup.ChildTransitions = new TransitionCollection
                {
                    new PaneThemeTransition
                        {
                            Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                       ? EdgeTransitionLocation.Right
                                       : EdgeTransitionLocation.Left
                        }
                };

            // Create a SettingsFlyout the same dimenssions as the Popup.
            mypane.Width = SettingsWidth;
            mypane.Height = _windowBounds.Height;

            // Place the SettingsFlyout inside our Popup window.
            _settingsPopup.Child = mypane;

            // Let's define the location of our Popup.
            _settingsPopup.SetValue(Canvas.LeftProperty,
                                    SettingsPane.Edge == SettingsEdgeLocation.Right ? (_windowBounds.Width - SettingsWidth) : 0);
            _settingsPopup.SetValue(Canvas.TopProperty, 0);
            _settingsPopup.IsOpen = true;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private void LoadComicDataSource()
        {
            Task result = LoadComicsDataFile();
            result.Wait();
        }

        private async void UpdateComicDataSource()
        {
            bool failedUpdate = false;

            try
            {
                await ComicDataSource.UpdateComicData();
            }
            catch (Exception)
            {
                failedUpdate = true;
            }

            if (!failedUpdate)
            {
                await SaveComicData();
            }
            else
            {
                var dialog = new MessageDialog("Failed to get update form web.");
                await dialog.ShowAsync();
            }
        }

        private async Task SaveComicData()
        {
            var outFile = await _storageFolder.CreateFileAsync(ComicsDataFile, CreationCollisionOption.ReplaceExisting);
            using (var saveStream = await outFile.OpenStreamForWriteAsync())
            {
                ComicDataSource.SaveComicData(saveStream);
            }
        }

        private async Task LoadComicsDataFile()
        {
            bool loadOriginalComicsData = false;

            try
            {
                await LoadComicsDataFile(_storageFolder).ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                loadOriginalComicsData = true;
            }

            if (loadOriginalComicsData)
            {
                await LoadComicsDataFile(Package.Current.InstalledLocation).ConfigureAwait(false);
            }
        }

        private async Task LoadComicsDataFile(StorageFolder folder)
        {
            var inFile = await folder.GetFileAsync(ComicsDataFile).AsTask().ConfigureAwait(false);
            using (var readStream = await inFile.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                ComicDataSource.LoadDataFromFile(readStream);
            }
        }
    }
}
