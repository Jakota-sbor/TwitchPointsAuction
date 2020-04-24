using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TwitchPointsAuction
{
    public enum AuthorizeType : int
    {
        TwitchChat = 0,
        TwitchPubSub
    }
    /// <summary>
    /// Логика взаимодействия для Authorize.xaml
    /// </summary>
    public partial class Authorize : Window
    {
        public Authorize(AuthorizeType type = AuthorizeType.TwitchChat)
        {
            InitializeComponent();
            AuthorizeType = type;
        }

        public AuthorizeType AuthorizeType { get; private set; }

        const string ClientID = "hfgz4sbkqm4m5tjkvtqmr5bpzwiknd";
        const string RedirectURL = @"http://localhost";
        const string ScopesChat = "chat:read chat:edit";
        const string ScopesPubSub = "channel:read:redemptions";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string accessToken;
        public string AccessToken { get => accessToken; set { accessToken = value; OnPropertyChanged("AccessToken"); } }

        public string TwitchAuthLink
        {
            get
            {
                string Scopes;
                switch (AuthorizeType)
                {
                    case AuthorizeType.TwitchChat:
                        Scopes = ScopesChat;
                        break;
                    case AuthorizeType.TwitchPubSub:
                        Scopes = ScopesPubSub;
                        break;
                    default:
                        Scopes = ScopesChat;
                        break;
                }
                return string.Format(@"https://id.twitch.tv/oauth2/authorize?client_id={0}&redirect_uri={1}&response_type=token&scope={2}&force_verify=true", ClientID, RedirectURL, Scopes);
            }
        }

        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }


        private void buttonHideAuthorize_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void webBrowser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            label_Loading.Visibility = System.Windows.Visibility.Visible;
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            label_Loading.Visibility = System.Windows.Visibility.Collapsed;
            txtUrl.Text = e.Uri.ToString();

            if (e.Uri.ToString().Contains("#access_token"))
            {
                Match access_token = Regex.Match(e.Uri.ToString().Split('#')[1], @"access_token=(.*)&");
                if (access_token.Success)
                    AccessToken = access_token.Groups[1].Value;
                else MessageBox.Show("Не удалось авторизоваться");
            }
        }

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            SetIE8KeyforWebBrowserControl(appName);
            webBrowser.Navigate(TwitchAuthLink);
        }

        private void SetIE8KeyforWebBrowserControl(string appName)
        {
            RegistryKey Regkey = null;
            try
            {
                // For 64 bit machine
                if (Environment.Is64BitOperatingSystem)
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                else  //For 32 bit machine
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);

                // If the path is not correct or
                // if the user haven't priviledges to access the registry
                if (Regkey == null)
                {
                    MessageBox.Show("Application Settings Failed - Address Not found");
                    return;
                }

                string FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                // Check if key is already present
                if (FindAppkey == "8000")
                {
                    MessageBox.Show("Required Application Settings Present");
                    Regkey.Close();
                    return;
                }

                // If a key is not present add the key, Key value 8000 (decimal)
                if (string.IsNullOrEmpty(FindAppkey))
                    Regkey.SetValue(appName, unchecked((int)0x1F40), RegistryValueKind.DWord);

                // Check for the key after adding
                FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                if (FindAppkey == "8000")
                    MessageBox.Show("Application Settings Applied Successfully");
                else
                    MessageBox.Show("Application Settings Failed, Ref: " + FindAppkey);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application Settings Failed");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close the Registry
                if (Regkey != null)
                    Regkey.Close();
            }
        }
    }
}
