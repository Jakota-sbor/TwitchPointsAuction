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
        public AuctionViewModel viewModel = new AuctionViewModel(
            new IrcChatSettings("jakota_sbor", "oauth:l6czzcm0brb26k42a1a3d1cgqkqyop", "happasc2", "irc.chat.twitch.tv", 6667),
            new AuctionSettings(),
            new AuctionElementValidation()
            );

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
