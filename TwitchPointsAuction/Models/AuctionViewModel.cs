using System;
using System.Collections.Generic;
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
    public class AuctionElementValidation
    {
        public int YearFrom { get; set; } 
        public int YearTo { get; set; }
        public List<string> Genres { get; set; }
        public Kind Type { get; set; }
        public List<string> NamesBlackList { get; set; }
        public bool IsEnabled { get; set; } = false;

        public (bool, BetError) IsInvalid(AnimeData anime)
        {
            if (!IsEnabled)
                return (false,BetError.None);
            else
            {
                bool InvalidName, InvalidDate, InvalidKind, InvalidGenres;
                InvalidName = NamesBlackList.Any(x => x.ToUpper().Contains(anime.NameRus.ToUpper()));
                InvalidDate = anime.AiredDate.Year < YearFrom || anime.AiredDate.Year > YearTo;
                InvalidKind = Type != anime.Kind;
                InvalidGenres = false;
                return (InvalidName || InvalidDate || InvalidKind || InvalidGenres, BetError.None);
            }
        }
    }

    public class AuctionSettings
    {
        private uint _AuctionTime = 36000;
        private bool _AllowDifferentBets = true;

        public uint AuctionTime { get => _AuctionTime; set => _AuctionTime = value; }
        public bool AllowDifferentBets { get => _AllowDifferentBets; set => _AllowDifferentBets = value; }
    }

    public class AuctionViewModel : INotifyPropertyChanged
    {
        private Regex ShikiAnimeIDRegex = new Regex(@"shikimori\.one\/animes\/(z?\d+)\S*", RegexOptions.Compiled);
        private object itemslock = new object();
        private AuctionSettings AucSettings;
        private AuctionElementValidation AuctionElemValidation;
        private IrcChat TwitchChat;
        private DispatcherTimer Timer;
        public NotifyCollection<AuctionElementViewModel> AuctionElements { get; set; }

        private uint CurrentTimeLeft { get; set; } = 600000;
        public AuctionState CurrentState { get; set; } = AuctionState.Going;

        public AuctionViewModel(IrcChatSettings ircSettings, AuctionSettings auctionSettings, AuctionElementValidation aucElemValid)
        {
            AuctionElements = new NotifyCollection<AuctionElementViewModel>();
            TwitchChat = new IrcChat(ircSettings);
            AucSettings = auctionSettings;
            AuctionElemValidation = aucElemValid;
            BindingOperations.EnableCollectionSynchronization(AuctionElements, itemslock);
            Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        ~AuctionViewModel()
        {
            TwitchChat.Disconnect();
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (CurrentState == AuctionState.Going)
                CurrentTimeLeft = CurrentTimeLeft - 100 > 0 ? CurrentTimeLeft -= 100 : CurrentTimeLeft;
        }

        public async Task Initialize()
        {
            //await Parsing.Initialize();
            await TwitchChat.Connect();
            await TwitchChat.JoinChannel();
            await PubSub.Initialize();
            PubSub.OnReward += PubSub_OnReward;
        }

        private async void PubSub_OnReward(object sender, Reward newReward)
        {
            Debug.WriteLine("NEW REWARD! STATES: {0}, {1}, {2}", CurrentState, CurrentTimeLeft, newReward.UserInputRequired);
            if (CurrentState == AuctionState.Going && CurrentTimeLeft > 0 && newReward.UserInputRequired)
            {
                //lock (itemslock)
                Debug.WriteLine("Finding match in reward... " + newReward.UserInput);
                var match = ShikiAnimeIDRegex.Match(newReward.UserInput);
                if (match.Success)
                {
                    Debug.WriteLine("SUCCESS SHIKI MATCH!");
                    string animeId = match.Groups[1].Value;
                    var elem = AuctionElements.FirstOrDefault(x => x.Id == animeId);
                    Debug.WriteLine("Element exist?! " + elem != null);
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
            var IsInvalid = AuctionElemValidation.IsInvalid(animeData.Item1).Item1;
            if (!IsInvalid)
            {
                AuctionElements.Add(new AuctionElementViewModel(animeId, animeData.Item1));
                return AuctionElements[^1];
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
