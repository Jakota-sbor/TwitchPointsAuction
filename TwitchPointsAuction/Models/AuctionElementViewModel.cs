using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionElementViewModel : INotifyPropertyChanged
    {
        private int index = 0;
        private LotData lotData;
        private ApiType apitype = ApiType.Text;
        //private Task<(LotData, HttpStatusCode)> LoadLotDataTask;
        private uint totalBet = 0;
        private NotifyCollection<Bet> bets = new NotifyCollection<Bet>();
        private bool isShowPoster = false;
        public int Index { get => index; set { index = value; OnPropertyChanged(); } }
        public LotData LotData { get => lotData; set { lotData = value; OnPropertyChanged(); } }
        public uint TotalBet { get => totalBet; set { totalBet = value; OnPropertyChanged(); } }
        public NotifyCollection<Bet> Bets { get => bets; set { bets = value; OnPropertyChanged(); } }
        public bool IsShowPoster { get => isShowPoster; set { isShowPoster = value; OnPropertyChanged(); } }
        public ApiType Apitype { get => apitype; set { apitype = value; OnPropertyChanged(); } }

        public AuctionElementViewModel()
        {
            this.LotData = new LotData()
            {
                ID = "12345",
                Name = "Тестовый лот",
            };
        }

        public AuctionElementViewModel(Bet bet)
        {
            Bets.CollectionChanged += Bets_CollectionChanged;
            Bets.Add(bet);
        }

        public AuctionElementViewModel(int index, Bet bet) : this(bet)
        {
            this.Index = index;
            this.LotData = new LotData() { Name = bet.UserInput };
        }

        public AuctionElementViewModel(int index, Bet bet, string ID = "") : this(index, bet)
        {
            if ((bet.ApiType != ApiType.Text && bet.ApiType != ApiType.ID) && !string.IsNullOrEmpty(ID))
                this.LotData = Requests.GetContentData(bet.ApiType, ID).Result.Item1;
            else
                this.LotData = new LotData() { ID = ID, Name = bet.UserInput };
        }
        /*
        public AuctionElementViewModel(int index, LotData lotData)
        {
            this.index = index;
            this.lotData = lotData;
            Bets.CollectionChanged += Bets_CollectionChanged;
        }
        */
        private void Bets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                var bet = item as Bet;
                TotalBet += bet.Cost;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
