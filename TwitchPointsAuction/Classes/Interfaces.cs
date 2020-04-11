using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TwitchPointsAuction.Classes
{
    public delegate void OnRewardHandler(object sender, TwitchPointsAuction.Models.Reward newReward);
    public delegate void OnAuctionEventHandler(object sender, AuctionEvent newState);

    public interface IChat
    {
        //event OnMessageHandler OnMessage;

        bool IsConnected { get; }

        Task<bool> Connect();
        Task<bool> JoinChannel();
        void Disconnect();
        void SendMessage(string msg);
    }
}
