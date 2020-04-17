using System;
using System.Collections.Generic;
using System.Linq;
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
        public AuctionSettingsWindow()
        {
            InitializeComponent();
        }

        ~AuctionSettingsWindow()
        {
            Properties.UserSettings.Default.DefaultAuctionRules.SaveRules();
            Properties.UserSettings.Default.Save();
        }
    }
}
