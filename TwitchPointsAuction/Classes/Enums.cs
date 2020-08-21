using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    public enum ApiType : int
    {
        [Description("Обычный текст")]
        Text = 0,
        [Description("Номер лота")]
        ID,
        [Description("Ссылка Steam")]
        Steam,
        [Description("Ссылка PS Store")]
        PSStore,
        [Description("Ссылка Kinopoisk")]
        Kinopoisk,
        [Description("Ссылка Shikimori")]
        Shikimori
    }

    public enum CollectionType : int
    {
        [Description("Жанры")]
        Genres = 0,
        [Description("Форматы")]
        Kinds,
        [Description("Рейтинг")]
        Rating,
        [Description("Тайтлы")]
        Titles,
        [Description("Доступные API")]
        Apis
    }

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
        [Description("Не вышло")]
        NotReleased = 0,
        [Description("Законченное")]
        Released,
        [Description("Незаконченное (онгоинг)")]
        Ongoing
    }

    public enum AnimeGenres : int
    {
        [Description("Все")]
        None = 0,
        [Description("Экшен")]
        Action,
        [Description("Приключения")]
        Adventure,
        [Description("Машины")]
        Vehicles,
        [Description("Комедия")]
        Comedy,
        [Description("Безумие")]
        Madness,
        [Description("Демоны")]
        Demons,
        [Description("Детектив")]
        Detective,
        [Description("Драма")]
        Drama,
        [Description("Этти")]
        Ecchi,
        [Description("Фэнтези")]
        Fantasy,
        [Description("Игры")]
        Game,
        [Description("Хентай")]
        Hentai,
        [Description("Исторический")]
        Historical,
        [Description("Ужасы")]
        Horror,
        [Description("Детское")]
        Kids,
        [Description("Магия")]
        Magic,
        [Description("Боевые искусства")]
        MartialArts,
        [Description("Военное")]
        Military,
        [Description("Меха")]
        Mecha,
        [Description("Музыка")]
        Music,
        [Description("Пародия")]
        Parody,
        [Description("Самураи")]
        Samurai,

        [Description("Сёдзё-ай")]
        Shoujo_ai,
        [Description("Дзёсей")]
        Josei,
        [Description("Романтика")]
        Romance,
        [Description("Школа")]
        School,
        [Description("Вампиры")]
        Vampire,
        [Description("Гарем")]
        Harem,
        [Description("Космос")]
        Space,
        [Description("Сёдзё")]
        Shoujo,
        [Description("Повседневность")]
        SliceOfLife,
        [Description("Полиция")]
        Police,
        [Description("Сёнен")]
        Shonen,
        [Description("Сёнен-ай")]
        Shounen_ai,
        [Description("Психологическое")]
        Psychological,
        [Description("Сверхъестественное")]
        Supernatural,
        [Description("Спорт")]
        Sport,
        [Description("Супер сила")]
        SuperPower,
        [Description("Фантастика")]
        Sci_Fi,
        [Description("Триллер")]
        Thriller,
        [Description("Яой")]
        Yaoi,
        [Description("Сейнен")]
        Seinen,
        [Description("Юри")]
        Yuri
    }

    public enum SteamGenres : int
    {
        [Description("Нет")]
        None = 0,
        [Description("Экшены")]
        Actions=1,
        [Description("Стратегии")]
        Strategy,
        [Description("")]
        E2,
        [Description("Казуальные")]
        Casual,
        [Description("")]
        E3,
        [Description("")]
        E4,
        [Description("")]
        E5,
        [Description("")]
        E6,
        [Description("Гонки")]
        Racing,
        [Description("")]
        E8,
        [Description("")]
        E9,
        [Description("")]
        E10,
        [Description("")]
        E11,
        [Description("")]
        E12,
        [Description("")]
        E13,
        [Description("")]
        E14,
        [Description("")]
        E15,
        [Description("Спортивные")]
        Sports,
        [Description("")]
        E17,
        [Description("")]
        E18,
        [Description("")]
        E19,
        [Description("")]
        E20,
        [Description("")]
        E21,
        [Description("")]
        E22,
        [Description("Приключения")]
        Adventure,
        [Description("")]
        SliceOfLife,
        [Description("")]
        Police,
        [Description("Симуляторы")]
        Simulators,
        [Description("")]
        Psychological,
        [Description("")]
        Samurai,
        [Description("")]
        Supernatural,
        [Description("")]
        Sport,
        [Description("")]
        SuperPower,
        [Description("")]
        Horror,
        [Description("")]
        Sci_Fi,
        [Description("")]
        Fantasy,
        [Description("Бесплатно")]
        FreeToPlay,
        [Description("")]
        Ecchi,
        [Description("")]
        Thriller,
        [Description("")]
        Hentai,
        [Description("")]
        Yaoi,
        [Description("")]
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
