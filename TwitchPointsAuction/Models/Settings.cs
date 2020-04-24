using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class Settings : INotifyPropertyChanged
    {
        private static Settings instance;
        public static Settings Instance
        {
            get { return instance ?? (instance = new Settings()); }
        }

        private IrcChatSettings ircSettings;
        private PubSubSettings pubSettings;
        private AuctionRulesModel auctionRules;
        private AuctionSettingsModel auctionSettings;

        public IrcChatSettings IrcSettings { get => ircSettings; set { ircSettings = value; OnPropertyChanged(); }  }
        public PubSubSettings PubSettings { get => pubSettings; set { pubSettings = value; OnPropertyChanged(); } }
        public AuctionRulesModel AuctionRules { get => auctionRules; set { auctionRules = value; OnPropertyChanged(); } }
        public AuctionSettingsModel AuctionSettings { get => auctionSettings; set { auctionSettings = value; OnPropertyChanged(); } }

        public Settings()
        {
            IrcSettings = Properties.UserSettings.Default.TwitchIrcSettings != null ? JsonConvert.DeserializeObject<IrcChatSettings>(Properties.UserSettings.Default.TwitchIrcSettings) : new IrcChatSettings();
            PubSettings = Properties.UserSettings.Default.TwitchPubSubSettings != null ? JsonConvert.DeserializeObject<PubSubSettings>(Properties.UserSettings.Default.TwitchPubSubSettings) : new PubSubSettings();
            AuctionSettings = Properties.UserSettings.Default.DefaultAuctionSettings != null ? JsonConvert.DeserializeObject<AuctionSettingsModel>(Properties.UserSettings.Default.DefaultAuctionSettings) : new AuctionSettingsModel();
            AuctionRules = Properties.UserSettings.Default.DefaultAuctionRules != null ? JsonConvert.DeserializeObject<AuctionRulesModel>(Properties.UserSettings.Default.DefaultAuctionRules) : new AuctionRulesModel();
        }

        public void Save()
        {
            Debug.WriteLine("SAVE");
            Properties.UserSettings.Default.TwitchIrcSettings = JsonConvert.SerializeObject(IrcSettings);
            Properties.UserSettings.Default.TwitchPubSubSettings = JsonConvert.SerializeObject(PubSettings);
            Properties.UserSettings.Default.DefaultAuctionSettings = JsonConvert.SerializeObject(AuctionSettings);
            Properties.UserSettings.Default.DefaultAuctionRules = JsonConvert.SerializeObject(AuctionRules);
            Properties.UserSettings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(prop));

    }
}
