using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Threading;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionModel : IDisposable, INotifyPropertyChanged
    {
        public event OnAuctionEventHandler OnAuctionEventChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        //private readonly DispatcherTimer Timer;
        private readonly System.Threading.Timer Timer;
        private readonly TimeSpan TimeSpanTick = TimeSpan.FromMilliseconds(100);
        private TimeSpan currentTimeLeft = TimeSpan.Zero;
        private TimeSpan addTime = TimeSpan.Zero;
        private AuctionState currentState = AuctionState.Off;

        public TimeSpan CurrentTimeLeft { get => currentTimeLeft; private set { currentTimeLeft = value; OnPropertyChanged(); } }
        public TimeSpan AddTime { get => addTime; private set { addTime = value; OnPropertyChanged(); } }

        public AuctionState CurrentState
        {
            get { return currentState; }
            set
            {
                currentState = value;
                OnPropertyChanged();
                switch (currentState)
                {
                    case AuctionState.On:
                        OnAuctionEventChanged?.Invoke(this, AuctionEvent.Started);
                        CurrentTimeLeft = Settings.Instance.AuctionSettings.AuctionTime;
                        break;
                    case AuctionState.Off:
                        OnAuctionEventChanged?.Invoke(this, AuctionEvent.Stoped);
                        CurrentTimeLeft = TimeSpan.Zero;
                        break;
                }
            }
        }

        public bool CanBet => !(currentState == AuctionState.Off || currentTimeLeft == TimeSpan.Zero);


        public AuctionModel()
        {
            //Timer = new DispatcherTimer() { Interval = TimeSpanTick };
            Timer = new System.Threading.Timer(new System.Threading.TimerCallback(Timer_Tick), null, 0, 100);
        }

        public void AddAuctionTime(TimeSpan time)
        {
            AddTime = time; 
        }

        private void Timer_Tick(object state)
        {
            if (AddTime != TimeSpan.Zero)
            {
                CurrentTimeLeft += AddTime;
                AddTime = TimeSpan.Zero;
            }
            else
                CurrentTimeLeft = CurrentTimeLeft > TimeSpanTick ? CurrentTimeLeft -= TimeSpanTick : TimeSpan.Zero;
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
