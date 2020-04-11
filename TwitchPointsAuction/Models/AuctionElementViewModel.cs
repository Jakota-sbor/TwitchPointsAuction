using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionElementViewModel : INotifyPropertyChanged
    {
        private readonly int index=0;
        private readonly string id;
        private readonly AnimeData animeData;
        private uint totalBet = 0;
        private NotifyCollection<Bet> bets = new NotifyCollection<Bet>();
        private bool isShowPoster = false;
        public int Index => index;
        public string Id { get => id;}
        public AnimeData AnimeData { get => animeData;}
        public uint TotalBet { get => totalBet; set { totalBet = value; OnPropertyChanged(); } }
        public NotifyCollection<Bet> Bets { get => bets; set { bets = value; OnPropertyChanged(); } }
        public bool IsShowPoster { get => isShowPoster; set { isShowPoster = value; OnPropertyChanged(); } }

        public AuctionElementViewModel(int index, string id, AnimeData animeData)
        {
            Debug.WriteLine("Element created!");
            this.index = index;
            this.id = id;
            this.animeData = animeData;
            Bets.CollectionChanged += Bets_CollectionChanged;
        }

        public AuctionElementViewModel()
        {
            this.animeData = new AnimeData();
        }

        private void Bets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                var bet = item as Bet;
                TotalBet += bet.Cost;
            }   
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
