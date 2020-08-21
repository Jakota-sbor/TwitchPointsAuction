using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    [JsonObject]
    public class AuctionSettingsModel : INotifyPropertyChanged
    {
        private bool allowDifferentBets = false;
        private int? maxAuctionElements = null;
        private TimeSpan defaultAuctionTime = TimeSpan.FromMinutes(10);
        private TimeSpan defaultAuctionThresholdTime = TimeSpan.FromMinutes(2);
        private TimeSpan defaultAdditionalTime = TimeSpan.FromMinutes(2);
        private NotifyCollection<ApiType> allowApiTypes = new NotifyCollection<ApiType>();

        public TimeSpan AuctionTime { get => defaultAuctionTime; set {  defaultAuctionTime =  value != TimeSpan.Zero ? value : TimeSpan.FromMinutes(30); OnPropertyChanged(); } }
        public TimeSpan DefaultAuctionThresholdTime { get => defaultAuctionThresholdTime; set { defaultAuctionThresholdTime = value != TimeSpan.Zero ? value : TimeSpan.FromMinutes(5); OnPropertyChanged(); } }
        public TimeSpan DefaultAdditionalTime { get => defaultAdditionalTime; set { defaultAdditionalTime = value != TimeSpan.Zero ? value : TimeSpan.FromMinutes(2); OnPropertyChanged(); } }
        public bool AllowDifferentBets { get => allowDifferentBets; set { allowDifferentBets = value; OnPropertyChanged(); } }
        public int? MaxAuctionElements { get => maxAuctionElements; set { maxAuctionElements = value; OnPropertyChanged(); } }
        public NotifyCollection<ApiType> AllowApiTypes { get => allowApiTypes; set { allowApiTypes = value; OnPropertyChanged(); } }

        public AuctionSettingsModel() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
