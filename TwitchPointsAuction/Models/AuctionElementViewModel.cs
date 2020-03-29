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
        private readonly string id;
        private readonly AnimeData animeData;
        private uint totalBet = 0;
        private NotifyCollection<Bet> bets = new NotifyCollection<Bet>();

        public string Uid { get; } = Guid.NewGuid().ToString();
        public string Id { get => id;}
        public AnimeData AnimeData { get => animeData;}
        public uint TotalBet { get => totalBet; set { totalBet = value; OnPropertyChanged("TotalBet"); } }
        public NotifyCollection<Bet> Bets { get => bets; set { bets = value; OnPropertyChanged("Bets"); } }    

        public AuctionElementViewModel(string id, AnimeData animeData)
        {
            Debug.WriteLine("Element created!");
            this.id = id;
            this.animeData = animeData;
            Bets.CollectionChanged += Bets_CollectionChanged;
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
        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

}
