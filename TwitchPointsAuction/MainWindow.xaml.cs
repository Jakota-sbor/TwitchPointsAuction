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
        public AuctionViewModel viewModel = new AuctionViewModel();
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
            Properties.UserSettings.Default.Reset();
            this.DataContext = viewModel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await viewModel.Initialize();
                /*
                List<string> animes = new List<string>() { "19", "37991", "z11741", "33486", "33255", "z31043", "16498", "9756", "z13601" };

                foreach (var item in animes)
                {
                    viewModel.AnimeBetList.Add((await Requests.GetAnimeData(item)).Item1);
                }
                */

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in viewModel.AuctionElements)
            {
                item.IsShowPoster = !item.IsShowPoster;
            }
        }

        private void togglebutton_Settings_Click(object sender, RoutedEventArgs e)
        {
            AuctionSettingsWindow wind = new AuctionSettingsWindow();
            wind.Show();
        }

        private async void Button_LoadCompletedAnime(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Properties.UserSettings.Default.DefaultAuctionRules.CompletedAnime == null ||
                   (Properties.UserSettings.Default.DefaultAuctionRules.CompletedAnime != null
                   && Properties.UserSettings.Default.DefaultAuctionRules.CompletedAnime.Count() == 0))
                {
                    var AnimeList = await Requests.GetCompletedAnimeData();
                    Properties.UserSettings.Default.DefaultAuctionRules.CompletedAnime = (NotifyCollection<string>)AnimeList.Item1;
                    Properties.UserSettings.Default.Save();
                    MessageBox.Show("Загружено " + AnimeList.Item1.Count + " просмотренных тайтлов!");
                }
                else
                {
                    MessageBox.Show("Список просмотренных тайтлов уже загружен!");
                }
            }
            catch
            {
                MessageBox.Show("Ошибочка! Не удалось загрузить список просмотренных тайтлов");
            }
        }
    }
}
