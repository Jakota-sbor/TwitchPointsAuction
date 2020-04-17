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
    public enum Genres : int
    {
        [Description("Сёнен")]
        Shonen = 0,
        [Description("Сёнен-ай")]
        Shounen_ai,
        [Description("Сейнен")]
        Seinen,
        [Description("Сёдзё")]
        Shoujo,
        [Description("Сёдзё-ай")]
        Shoujo_ai,
        [Description("Дзёсей")]
        Josei,
        [Description("Комедия")]
        Comedy,
        [Description("Романтика")]
        Romance,
        [Description("Школа")]
        School,
        [Description("Безумие")]
        Madness,
        [Description("Боевые искусства")]
        MartialArts,
        [Description("Вампиры")]
        Vampire,
        [Description("Военное")]
        Military,
        [Description("Гарем")]
        Harem,
        [Description("Демоны")]
        Demons,
        [Description("Детектив")]
        Detective,
        [Description("Детское")]
        Kids,
        [Description("Драма")]
        Drama,
        [Description("Игры")]
        Game,
        [Description("Исторический")]
        Historical,   
        [Description("Космос")]
        Space,
        [Description("Магия")]
        Magic,
        [Description("Машины")]
        Vehicles,
        [Description("Меха")]
        Mecha,
        [Description("Музыка")]
        Music,
        [Description("Пародия")]
        Parody,
        [Description("Повседневность")]
        SliceOfLife,
        [Description("Полиция")]
        Police,
        [Description("Приключения")]
        Adventure,
        [Description("Психологическое")]
        Psychological,
        [Description("Самураи")]
        Samurai,
        [Description("Сверхъестественное")]
        Supernatural,
        [Description("Спорт")]
        Sport,
        [Description("Супер сила")]
        SuperPower,
        [Description("Ужасы")]
        Horror,
        [Description("Фантастика")]
        Sci_Fi,
        [Description("Фэнтези")]
        Fantasy,
        [Description("Экшен")]
        Action,
        [Description("Этти")]
        Ecchi,
        [Description("Триллер")]
        Thriller,
        [Description("Хентай")]
        Hentai,
        [Description("Яой")]
        Yaoi,
        [Description("Юри")]
        Yuri
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
        Completed,
        InvalidName,
        InvalidDate,
        InvalidKind,
        InvalidGenre
    }
}
