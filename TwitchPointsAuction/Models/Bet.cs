using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchPointsAuction.Models
{
    public class Bet
    {
        readonly string _User;
        readonly uint _Cost;

        public string User { get => _User; }
        public uint Cost { get => _Cost;  }

        public Bet(string user, uint cost)
        {
            _User = user;
            _Cost = cost;
        }
    }
}
