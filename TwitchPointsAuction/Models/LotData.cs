using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class LotData
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Uri PosterUri { get; set; } = null;

        public LotData() { }
    }

    public class SteamLot : LotData
    {
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public Status Status { get; set; } = Status.Released;
        public IEnumerable<SteamGenres> Genres { get; set; } = new List<SteamGenres>();

        public SteamLot()
        {
            PosterUri = new Uri($@"https://steamcdn-a.akamaihd.net/steam/apps/{ID}/capsule_231x87.jpg");
        }

        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{Name}\nОписание:{Description}\nПостер: {PosterUri}\nДата выхода:{ReleaseDate.ToShortDateString()}\nЖанры:{string.Join(", ", Genres)}";
        }
    }

    public class PsStoreLot : LotData
    {
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public Status Status { get; set; } = Status.Released;

        public PsStoreLot()
        {
            PosterUri = new Uri($@"https://store.playstation.com/store/api/chihiro/00_09_000/container/RU/ru/999/{ID}/image?w=240&h=240");
        }
        //https://store.playstation.com/store/api/chihiro/00_09_000/container/RU/ru/999/EP1004-CUSA00411_00-PREMIUMPACKOG001/image?w=240&h=240
        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{Name}\nОписание:{Description}\nПостер: {PosterUri}\nДата выхода:{ReleaseDate.ToShortDateString()}";
        }
    }

    public class KinopoiskLot : LotData
    {
        public KinopoiskLot()
        {
            PosterUri = new Uri($@"");
        }
    }

    public class ShikimoriLot : LotData
    {
        public string NameEng { get; set; } = "";
        public string StudioName { get; set; } = "";
        public int Episodes { get; set; } = 0;
        public Status Status { get; set; } = Status.Released;
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public Kind Kind { get; set; } = Kind.TV;
        public IEnumerable<AnimeGenres> Genres { get; set; } = new List<AnimeGenres>();

        public ShikimoriLot() { }

        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{Name}\nОписание:{Description}\nКол-во серий:{Episodes}\nПостер: {PosterUri}\nДата выхода:{ReleaseDate.ToShortDateString()}\nЖанры:{string.Join(", ", Genres)}";
        }
    }
}
