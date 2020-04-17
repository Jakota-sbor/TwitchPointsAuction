using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwitchPointsAuction.Models
{
    public class AuctionSettingsModel : INotifyPropertyChanged
    {
        private bool allowDifferentBets = false;
        private TimeSpan defaultAuctionTime = TimeSpan.FromMinutes(10);
        private TimeSpan defaultAuctionMinAddTime = TimeSpan.FromMinutes(2);
        private TimeSpan defaultAdditionalTime = TimeSpan.FromMinutes(2);
        public TimeSpan AuctionTime { get => defaultAuctionTime; set { defaultAuctionTime = value; OnPropertyChanged(); } }
        public TimeSpan DefaultAuctionMinAddTime { get => defaultAuctionMinAddTime; set { defaultAuctionMinAddTime = value; OnPropertyChanged(); } }
        public TimeSpan DefaultAdditionalTime { get => defaultAdditionalTime; set { defaultAdditionalTime = value; OnPropertyChanged(); } }
        public bool AllowDifferentBets { get => allowDifferentBets; set { allowDifferentBets = value; OnPropertyChanged(); } }

        public AuctionSettingsModel() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
