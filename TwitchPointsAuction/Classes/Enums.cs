using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    public enum AuctionState: int
    {
        Stoped=0,
        Going,
        Done
    }

    public enum Status : int
    {
        Released = 0,
        Ongoing
    }

    public enum Kind : int
    {
        TV = 0,
        Movie,
        OVA,
        ONA,
        Special
    }

    public enum BetError : int
    {
        None = 0,
        NotFound,
        InvalidName,
        InvalidDate,
        InvalidKind,
        InvalidGenre
    }
}
