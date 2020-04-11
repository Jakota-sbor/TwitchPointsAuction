using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionViewModel : INotifyPropertyChanged
    {
        private Regex ShikiAnimeIDRegex = new Regex(@"shikimori\.one\/animes\/(z?\d+)\S*", RegexOptions.Compiled);
        private object itemslock = new object();
        private IrcChat TwitchChat;

        public AuctionModel Auction { get; set; }
        public AuctionSettingsModel AucSettings { get; set; }
        public AuctionRulesModel AuctionRules { get; set; }
        public NotifyCollection<AuctionElementViewModel> AuctionElements { get; set; }

        bool IsInitialized = false;

        public AuctionViewModel()
        {
            AuctionElements = new NotifyCollection<AuctionElementViewModel>()
            {
                new AuctionElementViewModel(),
                new AuctionElementViewModel(),
                new AuctionElementViewModel(),
                new AuctionElementViewModel(),
                new AuctionElementViewModel()
            };
            Auction = new AuctionModel(TimeSpan.FromMinutes(10));
            AucSettings = new AuctionSettingsModel();
            AuctionRules = new AuctionRulesModel();
        }

        public AuctionViewModel(IrcChatSettings ircSettings, AuctionSettingsModel auctionSettings, AuctionRulesModel auctionRules)
        {
            TwitchChat = new IrcChat(ircSettings);
            AucSettings = auctionSettings;
            AuctionRules = auctionRules;
            BindingOperations.EnableCollectionSynchronization(AuctionElements, itemslock);
            AuctionElements = new NotifyCollection<AuctionElementViewModel>();
            Auction = new AuctionModel(AucSettings.AuctionTime);
        }

        ~AuctionViewModel()
        {
            TwitchChat?.Disconnect();
            Auction?.Dispose();
        }

        public async Task Initialize()
        {
            //await Parsing.Initialize();
            await TwitchChat.Connect();
            await TwitchChat.JoinChannel();
            await PubSub.Initialize();
            PubSub.OnReward += PubSub_OnReward;
            IsInitialized = true;
            /*
            var animeData = await Requests.GetAnimeData("38668");
            var IsInvalid = AuctionElemValidation.IsInvalid(animeData.Item1).Item1;
            if (!IsInvalid)
            {
                lock (itemslock)
                    AuctionElements.Add(new AuctionElementViewModel("38668", animeData.Item1));
            }
            */
        }

        private async void PubSub_OnReward(object sender, Reward newReward)
        {
            if (Auction.CanBet && newReward.UserInputRequired)
            {
                var match = ShikiAnimeIDRegex.Match(newReward.UserInput);
                if (match.Success)
                {
                    string animeId = match.Groups[1].Value;
                    var elem = AuctionElements.FirstOrDefault(x => x.Id == animeId);
                    if (elem != null)
                        elem.Bets.Add(new Bet(newReward.User, newReward.Cost));
                    else
                    {
                        elem = await AddAuctionElement(animeId);
                        elem.Bets.Add(new Bet(newReward.User, newReward.Cost));
                    }
                }
            }
        }

        public async Task<AuctionElementViewModel> AddAuctionElement(string animeId)
        {
            Debug.WriteLine("Create new element!");
            var animeData = await Requests.GetAnimeData(animeId);
            var IsInvalid = AuctionRules.IsInvalid(animeData.Item1).Item1;
            if (!IsInvalid)
            {
                AuctionElements.Add(new AuctionElementViewModel(AuctionElements.Count+1, animeId, animeData.Item1));
                return AuctionElements[^1];
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
