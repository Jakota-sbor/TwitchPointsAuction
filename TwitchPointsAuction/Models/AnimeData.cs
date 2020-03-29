using System;
using System.Collections.Generic;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AnimeData
    {
        public string ID { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string Description { get; set; }
        public string StudioName { get; set; }
        public int Episodes { get; set; }
        public Uri PosterUri { get; set; }
        public Status Status { get; set; }
        public Kind Kind { get; set; }
        public DateTime AiredDate { get; set; }
        public List<string> Genres { get; set; }

        public AnimeData() { }

        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{NameRus}\nОписание:{Description}\nКол-во серий:{Episodes}\nПостер: {PosterUri}\nДата выхода:{AiredDate.ToShortDateString()}\nЖанры:{string.Join(", ", Genres)}";
        }

    }
}
