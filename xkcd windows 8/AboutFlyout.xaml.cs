using System.Reflection;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace xkcd_windows_8
{
    public sealed partial class AboutFlyout
    {
        const int ContentAnimationOffset = 100;

        public AboutFlyout()
        {
            InitializeComponent();
            FlyoutContent.Transitions = new TransitionCollection
                {
                    new EntranceThemeTransition
                        {
                            FromHorizontalOffset =
                                (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                    ? ContentAnimationOffset
                                    : (ContentAnimationOffset*-1)
                        }
                };


            Version.Text = "Version " + GetPackageVersion();
            Copyright.Text = GetAssemblyCopyright();
        }

        private string GetAssemblyCopyright()
        {
            Assembly asm = typeof(App).GetTypeInfo().Assembly;
            
            var attr = CustomAttributeExtensions.GetCustomAttribute<AssemblyCopyrightAttribute>(asm);
            if (attr != null)
            {
                return attr.Copyright;
            }
            else
            {
                return "";
            }
        }

        private static string GetPackageVersion()
        {
            var ver = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString();
            return version;
        }

        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            var parent = Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }
    }
}
