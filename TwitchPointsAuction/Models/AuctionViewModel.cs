using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

        #region Regex
        private Regex ShikiAnimeIDRegex = new Regex(@"shikimori\.one\/animes\/(\w?\d+)\S*", RegexOptions.Compiled);
        private Regex SteamIDRegex = new Regex(@"store\.steampowered\.com\/app\/(\d+)", RegexOptions.Compiled);
        private Regex PSStoreRegex = new Regex(@"store\.playstation\.com\/.*(.{6}-(CUSA.*)-[^\/]*)", RegexOptions.Compiled);
        #endregion

        private object itemslock = new object();
        private object betslock = new object();

        private string auctionElementLeader = null;
        private string auctionLotFilterByname = string.Empty;

        public AuctionModel Auction { get; set; }
        public NotifyCollection<AuctionElementViewModel> AuctionElements { get; set; }

        private readonly System.Threading.Timer Timer;

        public NotifyCollection<Bet> BetsQueue { get; set; }
        public ConcurrentQueue<AuctionElementViewModel> AuctionElementsQueue { get; set; }
        public ConcurrentQueue<Task<(LotData, HttpStatusCode)>> TasksQueue { get; set; }

        public string AuctionElementLeader { get => auctionElementLeader; set { auctionElementLeader = value; OnPropertyChanged(); } }

        public string AuctionLotFilterByname { get => auctionLotFilterByname; set { auctionLotFilterByname = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public event EventHandler<BetError> OnBetErrorEvent;

        bool IsInitialized = false;

        public AuctionViewModel()
        {
            BetsQueue = new NotifyCollection<Bet>();
            AuctionElements = new NotifyCollection<AuctionElementViewModel>();
            Auction = new AuctionModel();
            OnBetErrorEvent += AuctionViewModel_OnBetErrorEvent;
            PubSub.Instance.OnReward += PubSub_OnReward;
            BindingOperations.EnableCollectionSynchronization(AuctionElements, itemslock);
            BindingOperations.EnableCollectionSynchronization(BetsQueue, betslock);
            Timer = new System.Threading.Timer(new System.Threading.TimerCallback(Timer_Tick), null, 0, 10000);
        }

        ~AuctionViewModel()
        {
            PubSub.Instance.OnReward -= PubSub_OnReward;
            Auction?.Dispose();
        }

        private void Timer_Tick(object state)
        {
            lock (betslock)
            {
                try
                {
                    if (Auction.CurrentState == AuctionState.On)
                    {
                        Debug.WriteLine("Bets: " + BetsQueue.Count);
                        // || BetsQueue.Count == BetsQueue.Select(bet => bet.ApiType == (ApiType.Text | ApiType.ID)).Count()
                        while (BetsQueue.Count != 0 && BetsQueue.Count == BetsQueue.Select(bet => bet.IsBroken).Count())
                        {
                            Debug.WriteLine("Trying Dequeue...");
                            bool IsSuccess = false;
                            var newBet = BetsQueue.LastOrDefault(bet => !bet.IsBroken);
                            if (newBet == null)
                                break;
                            if (newBet.ApiType != ApiType.Text && newBet.ApiType != ApiType.ID)
                            {
                                if (Settings.Instance.AuctionSettings.AllowApiTypes.Contains(newBet.ApiType))
                                {
                                    string ID = string.Empty;
                                    string FullID = string.Empty;
                                    Match match;
                                    //Ищем ID в строке
                                    switch (newBet.ApiType)
                                    {
                                        case ApiType.Shikimori:
                                            match = ShikiAnimeIDRegex.Match(newBet.UserInput);
                                            ID = match.Success ? match.Groups[1].Value : string.Empty;
                                            break;
                                        case ApiType.Steam:
                                            match = SteamIDRegex.Match(newBet.UserInput);
                                            ID = match.Success ? match.Groups[1].Value : string.Empty;
                                            break;
                                        case ApiType.PSStore:
                                            match = PSStoreRegex.Match(newBet.UserInput);
                                            ID = match.Success ? match.Groups[2].Value : string.Empty;
                                            FullID = match.Success ? match.Groups[1].Value : string.Empty;
                                            break;
                                        default:
                                            break;
                                    }
                                    Debug.WriteLine("ID: " + ID);
                                    //Если ID не пустой
                                    if (!string.IsNullOrEmpty(ID))
                                    {
                                        //Проверяем есть ли элемент с таким ID и API
                                        var elem = AuctionElements.FirstOrDefault(element => element.LotData != null && element.LotData.ID == (newBet.ApiType != ApiType.PSStore ? ID : FullID));
                                        //Если есть отправляем ставку в него, если нет создаем новый элемент, загружаем данные и отправляем ставку
                                        if (elem != null)
                                            elem.Bets.Add(newBet);
                                        else
                                        {
                                            Debug.WriteLine("Add new lot: " + newBet.UserInput);
                                            AuctionElements.Add(new AuctionElementViewModel(AuctionElements.Count + 1, newBet, ID));
                                        }
                                        IsSuccess = true;
                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine("TEXT INPUT");
                                //Ищем лот с таким же ID или текстом
                                var elem = AuctionElements.FirstOrDefault(element => element.LotData != null && element.LotData.Name.ToLower() == newBet.UserInput.Trim().ToLower());
                                //Добавляем ставку если такой лот есть
                                if (elem != null)
                                    elem.Bets.Add(newBet);
                                else
                                    AuctionElements.Add(new AuctionElementViewModel(AuctionElements.Count + 1, newBet));
                                IsSuccess = true;
                                //else
                                //Если нет, то возвращаем ставку в очередь
                                //BetsQueue.Enqueue(newBet);
                            }

                            if (newBet != null && (IsSuccess || !newBet.IsVisible))
                                BetsQueue.Remove(newBet);
                            else if (newBet != null && !IsSuccess)
                                newBet.IsBroken = true;
                        }
                    }
                    /*
                    var animeData = await Requests.GetAnimeData(animeId);
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
                    */
                }
                catch { }
            }
        }

        private void AuctionViewModel_OnBetErrorEvent(object sender, BetError e)
        {
            if (IrcChat.Instance.IsConnected)
            {
                var BetErrorString = "";
                switch (e)
                {
                    case BetError.NotFound:
                        BetErrorString = "Ошибочка: лот с таким номером не найден в списке аукциона!";
                        break;
                    case BetError.Completed:
                        BetErrorString = "Ошибочка: уже просмотрено";
                        break;
                    case BetError.InvalidDate:
                        BetErrorString = "Ошибочка: дата выхода не соответствует правилам аукциона!";
                        break;
                    case BetError.InvalidGenre:
                        BetErrorString = "Ошибочка: один из жанров не соответствует правилам аукциона!";
                        break;
                    case BetError.InvalidKind:
                        BetErrorString = "Ошибочка: формат не соответствует правилам аукциона!";
                        break;
                    case BetError.InvalidName:
                        BetErrorString = "Ошибочка: название не соответствует правилам аукциона!";
                        break;
                }
                Debug.WriteLine(BetErrorString);
                IrcChat.Instance.SendMessage(BetErrorString);
            }
        }


        private void PubSub_OnReward(object sender, Reward newReward)
        {
            try
            {
                //Преобразует награду (Reward) в ставку (Bet) и добавляет в очередь ставок
                Debug.WriteLine("NEW REWARD: " + newReward.ToString());
                if (Auction.CanBet && newReward.UserInputRequired)
                {
                    ApiType type = ApiType.Text;

                    if (newReward.UserInput.Contains("store.steampowered.com"))
                        type = ApiType.Steam;
                    else if (newReward.UserInput.Contains("store.playstation.com"))
                        type = ApiType.PSStore;
                    else if (newReward.UserInput.Contains("kinopoisk.ru"))
                        type = ApiType.Kinopoisk;
                    else if (newReward.UserInput.Contains("shikimori.one"))
                        type = ApiType.Shikimori;

                    BetsQueue.Add(new Bet(newReward.User, newReward.UserInput, newReward.Cost, type));
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
        /*
        public AuctionElementViewModel AddAuctionElement(LotData data)
        {
            try
            {
                Debug.WriteLine("Create new element!");
                if (!Settings.Instance.AuctionSettings.MaxAuctionElements.HasValue
                    || (Settings.Instance.AuctionSettings.MaxAuctionElements.HasValue && AuctionElements.Count < Settings.Instance.AuctionSettings.MaxAuctionElements.Value))
                {
                    //var IsInvalid = Settings.Instance.AuctionRules.IsInvalid(data);
                    var IsInvalid = (false, BetError.None);
                    if (!IsInvalid.Item1)
                    {
                        Debug.WriteLine("Add new item!");
                        AuctionElements.Add(new AuctionElementViewModel(AuctionElements.Count + 1, data));
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
        */
    }
}
