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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionViewModel : INotifyPropertyChanged
    { 

        public Settings UserSettings
        {
            get { return Settings.Instance; }
        }

        private Regex ShikiAnimeIDRegex = new Regex(@"shikimori\.one\/animes\/(z?\d+)\S*", RegexOptions.Compiled);
        private object itemslock = new object();
        private IrcChat TwitchChat;
        private PubSub TwitchPubSub;
        private string auctionElementLeader = null;

        public AuctionModel Auction { get; set; }
        public NotifyCollection<AuctionElementViewModel> AuctionElements { get; set; }
        public string AuctionElementLeader { get => auctionElementLeader; set { auctionElementLeader = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public event EventHandler<BetError> OnBetErrorEvent;

        private RelayCommand toggleChatCommand;
        public RelayCommand ToggleChatCommand
        {
            get
            {
                return toggleChatCommand ??
                    (toggleChatCommand = new RelayCommand(param =>
                    {
                        var isChecked = (bool)param;
                        if (isChecked)
                        {
                            Task.Run(async () =>
                            {
                                await TwitchChat.Connect();
                                await TwitchChat.JoinChannel();

                            });
                        }
                        else
                        {
                            Task.Run(async () =>
                            {
                                await TwitchChat.LeaveChannel();
                                await TwitchChat.Disconnect();
                            });
                        }
                    }));
            }
        }

        private RelayCommand togglePubSubCommand;
        public RelayCommand TogglePubSubCommand
        {
            get
            {
                return togglePubSubCommand ??
                    (togglePubSubCommand = new RelayCommand(param =>
                    {
                        var isChecked = (bool)param;
                        if (isChecked)
                        {
                            Task.Run(async () =>
                            {
                                TwitchPubSub.OnReward += PubSub_OnReward;
                                await TwitchPubSub.Connect();
                                await TwitchPubSub.JoinChannel();
                            });
                        }
                        else
                        {
                            Task.Run(async () =>
                            {
                                TwitchPubSub.OnReward -= PubSub_OnReward;
                                await TwitchPubSub.LeaveChannel();
                                await TwitchPubSub.Disconnect();
                            });
                        }
                    }));
            }
        }

        bool IsInitialized = false;

        public AuctionViewModel()
        {
            TwitchChat = new IrcChat();
            TwitchPubSub = new PubSub();
            AuctionElements = new NotifyCollection<AuctionElementViewModel>();
            Auction = new AuctionModel();
            OnBetErrorEvent += AuctionViewModel_OnBetErrorEvent;
            BindingOperations.EnableCollectionSynchronization(AuctionElements, itemslock);
        }

        private void AuctionViewModel_OnBetErrorEvent(object sender, BetError e)
        {
            if (TwitchChat.IsConnected)
            {
                var BetErrorString = "";
                switch (e)
                {
                    case BetError.NotFound:
                        BetErrorString = "Ошибочка! Тайтл с таким номером не найден в списке аукциона!";
                        break;
                    case BetError.Completed:
                        BetErrorString = "Ошибочка! Тайтл уже просмотрен";
                        break;
                    case BetError.InvalidDate:
                        BetErrorString = "Ошибочка: дата выхода тайтла не соответствует правилам!";
                        break;
                    case BetError.InvalidGenre:
                        BetErrorString = "Ошибочка: один из жанров аниме не соответствует правилам!";
                        break;
                    case BetError.InvalidKind:
                        BetErrorString = "Ошибочка: формат тайтла не соответствует правилам!";
                        break;
                    case BetError.InvalidName:
                        BetErrorString = "Ошибочка: название тайтла не соответствует правилам!";
                        break;
                }
                Debug.WriteLine(BetErrorString);
                TwitchChat.SendMessage(BetErrorString);
            }
        }

        ~AuctionViewModel()
        {
            TwitchChat?.Disconnect();
            Auction?.Dispose();
        }

        private async void PubSub_OnReward(object sender, Reward newReward)
        {
            try
            {
                Debug.WriteLine("NEW REWARD: " + newReward.ToString());
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
                            if (elem != null)
                            {
                                elem.Bets.Add(new Bet(newReward.User, newReward.Cost));
                                elem.PropertyChanged += Elem_PropertyChanged;
                            }
                        }
                    }
                    else
                    {
                        int index = int.TryParse(newReward.UserInput.Trim(), out index) ? index : 0;
                        var elem = AuctionElements.FirstOrDefault(x => x.Index == index);
                        if (elem != null)
                            elem.Bets.Add(new Bet(newReward.User, newReward.Cost));
                        else
                            OnBetErrorEvent?.Invoke(this, BetError.NotFound);
                    }
                }
            }
            catch
            { }
        }

        private void Elem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "TotalBet")
                {
                    if (Auction.CurrentTimeLeft <= Settings.Instance.AuctionSettings.DefaultAuctionThresholdTime)
                    {
                        var elem = (AuctionElementViewModel)sender;
                        var otherelems = AuctionElements.Where(x => x.Index != elem.Index);
                        if (otherelems.Count() > 0 && elem.TotalBet > otherelems.Max(x => x.TotalBet))
                        {
                            Debug.WriteLine("ADD TIME!");
                            Auction.AddAuctionTime(Settings.Instance.AuctionSettings.DefaultAdditionalTime);
                        }
                    }
                }
            }
            catch { }
        }

        public async Task<AuctionElementViewModel> AddAuctionElement(string animeId)
        {
            try
            {
                Debug.WriteLine("Create new element!");
                if (!Settings.Instance.AuctionSettings.MaxAuctionElements.HasValue
                    || (Settings.Instance.AuctionSettings.MaxAuctionElements.HasValue && AuctionElements.Count < Settings.Instance.AuctionSettings.MaxAuctionElements.Value))
                {
                    var animeData = await Requests.GetAnimeData(animeId);
                    var IsInvalid = Settings.Instance.AuctionRules.IsInvalid(animeData.Item1);
                    Debug.WriteLine("IsInValid? " + IsInvalid);
                    if (!IsInvalid.Item1)
                    {
                        Debug.WriteLine("Add new item!");
                        AuctionElements.Add(new AuctionElementViewModel(AuctionElements.Count + 1, animeId, animeData.Item1));
                        return AuctionElements[^1];
                    }
                    else
                        OnBetErrorEvent?.Invoke(this, IsInvalid.Item2);
                    return null;
                }
                return null;
            }
            catch
            { return null; }
        }
    }
}
