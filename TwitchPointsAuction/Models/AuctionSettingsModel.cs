using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwitchPointsAuction.Models
{
    public class AuctionSettingsModel : INotifyPropertyChanged
    {
        private TimeSpan auctionTime = TimeSpan.FromMinutes(10);
        private bool allowDifferentBets = false;
        private TimeSpan defaultAuctionTime = TimeSpan.FromMinutes(10);

        public TimeSpan AuctionTime { get => auctionTime; set { auctionTime = value; OnPropertyChanged(); } }
        public bool AllowDifferentBets { get => allowDifferentBets; set { allowDifferentBets = value; OnPropertyChanged(); } }

        public AuctionSettingsModel() { }
        public AuctionSettingsModel(TimeSpan time, bool alldiffbets)
        {
            auctionTime = time;
            allowDifferentBets = alldiffbets;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
