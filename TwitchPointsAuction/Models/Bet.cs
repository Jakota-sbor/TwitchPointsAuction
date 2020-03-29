using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchPointsAuction.Models
{
    public class Bet
    {
        string _User;
        uint _Cost;

        public string User { get => _User; private set => _User = value; }
        public uint Cost { get => _Cost; private set => _Cost = value; }

        public Bet(string user, uint cost)
        {
            this.User = user;
            this.Cost = cost;
        }
    }
}
