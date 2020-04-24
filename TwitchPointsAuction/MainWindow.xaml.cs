using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchPointsAuction.Classes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AuctionViewModel viewModel = new AuctionViewModel();
        AuctionSettingsWindow windowSettings;
            /*
             * new AuctionViewModel(
            new IrcChatSettings("jakota_sbor", "oauth:l6czzcm0brb26k42a1a3d1cgqkqyop", "happasc2", "irc.chat.twitch.tv", 6667),
            new AuctionSettings(),
            new AuctionElementValidation()
            );
            */
        public MainWindow()
        {
            InitializeComponent();
            //Properties.UserSettings.Default.Reset();
            this.DataContext = viewModel;
        }

        private void togglebutton_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (windowSettings != null && windowSettings.WindowState == WindowState.Minimized)
            {
                windowSettings.WindowState = WindowState.Normal;
                windowSettings.Activate();
            }
            else
                windowSettings = new AuctionSettingsWindow();

            windowSettings.Show();
        }

        private async void Button_LoadCompletedAnime(object sender, RoutedEventArgs e)
        {
            try
            {
                button_LoadCompletedAnimeList.IsEnabled = false;
                var AnimeList = await Requests.GetCompletedAnimeData();
                Settings.Instance.AuctionRules.CompletedAnime = new NotifyCollection<string>(AnimeList.Item1);
                MessageBox.Show("Загружено " + AnimeList.Item1.Count + " просмотренных тайтлов!");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибочка! Не удалось загрузить список просмотренных тайтлов"+exc.ToString());
            }
            finally
            {
                button_LoadCompletedAnimeList.IsEnabled = true;
            }
        }
    }
}
