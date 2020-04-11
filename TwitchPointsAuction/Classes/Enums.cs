using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TwitchPointsAuction.Classes
{

    public enum AuctionEvent : int
    {
        [Description("ВЫКЛ")]
        Stoped = 0,
        [Description("ВКЛ")]
        Started
    }

    public enum AuctionState : int
    {
        [Description("ВЫКЛ")]
        Off = 0,
        [Description("ВКЛ")]
        On
    }

    public enum Status : int
    {
        [Description("Законченное")]
        Released = 0,
        [Description("Незаконченное (онгоинг)")]
        Ongoing
    }

    public enum Kind : int
    {
        [Description("ТВ сериал")]
        TV = 0,
        [Description("Фильм")]
        Movie,
        [Description("OVA")]
        OVA,
        [Description("ONA")]
        ONA,
        [Description("Спешл")]
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
