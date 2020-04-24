using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwitchPointsAuction.Classes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Логика взаимодействия для AuctionSettingsWindow.xaml
    /// </summary>
    public partial class AuctionSettingsWindow : Window
    {
        Authorize authorizeWindow;

        AuthorizeType AuthorizeType { get; set; } = AuthorizeType.TwitchChat;

        const string ClientID = "hfgz4sbkqm4m5tjkvtqmr5bpzwiknd";
        const string RedirectURL = @"http://localhost";
        const string ScopesChat = "chat:read chat:edit";
        const string ScopesPubSub = "channel:read:redemptions";

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

        public Settings UserSettings
        {
            get { return Settings.Instance; }
        }

        public AuctionSettingsWindow()
        {
            InitializeComponent();
            this.DataContext = UserSettings;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Instance.Save();
        }

        private void button_ChatLogin_Click(object sender, RoutedEventArgs e)
        {
            AuthorizeType = AuthorizeType.TwitchChat;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", TwitchAuthLink);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", TwitchAuthLink);
            }
            else
            {
                var psi = new ProcessStartInfo
                {
                    FileName = TwitchAuthLink,
                    UseShellExecute = true,
                    Verb = "open",
                };
                Process.Start(psi);
            }

        }

        private void button_PubSubLogin_Click(object sender, RoutedEventArgs e)
        {
            //authorizeWindow = new Authorize(AuthorizeType.TwitchPubSub);
            //authorizeWindow.PropertyChanged += AuthorizeWindow_PropertyChanged;
            //authorizeWindow.Show();
            AuthorizeType = AuthorizeType.TwitchPubSub;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", TwitchAuthLink);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", TwitchAuthLink);
            }
            else
            {
                var psi = new ProcessStartInfo
                {
                    FileName = TwitchAuthLink,
                    UseShellExecute = true,
                    Verb = "open",
                };
                Process.Start(psi);
            }
        }

        private void AuthorizeWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            /*
            switch (authorizeWindow.AuthorizeType)
            {
                case AuthorizeType.TwitchChat:
                    UserSettings.IrcSettings.Token = authorizeWindow.AccessToken;
                    break;
                case AuthorizeType.TwitchPubSub:
                    UserSettings.PubSettings.Token = authorizeWindow.AccessToken;
                    break;
                default:
                    break;
            }
            
            authorizeWindow.PropertyChanged -= AuthorizeWindow_PropertyChanged;
            authorizeWindow.Close();
            */
        }
    }
}
