using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AnimeData
    {
        public string ID { get; set; } = "0";
        public string NameEng { get; set; } = "";
        public string NameRus { get; set; } = "";
        public string Description { get; set; } = "";
        public string StudioName { get; set; } = "";
        public int Episodes { get; set; } = 12;
        public Uri PosterUri { get; set; } = new Uri("pack://application:,,,/Resources/poster.jpg", UriKind.RelativeOrAbsolute);
        public Status Status { get; set; } = Status.Released;
        public Kind Kind { get; set; } = Kind.TV;
        public DateTime AiredDate { get; set; } = DateTime.Now;
        public IEnumerable<Genres> Genres { get; set; }

        public AnimeData() { }

        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{NameRus}\nОписание:{Description}\nКол-во серий:{Episodes}\nПостер: {PosterUri}\nДата выхода:{AiredDate.ToShortDateString()}\nЖанры:{string.Join(", ", Genres)}";
        }

    }
}
