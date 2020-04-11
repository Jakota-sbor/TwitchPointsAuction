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
        public string NameRus { get; set; } = "Этот глупый свин не понимает мечту девочки-зайки";
        public string Description { get; set; } = "Ничем не примечательный школьник Сакута Адзусагава проводит всё своё свободное время в библиотеке. Однажды он встречает девушку в костюме кролика, которая старше его на год. Она представляется как Маи Сакурадзима — прославленная актриса и первая красавица школы. Девушка рассказывает, что все вокруг перестали «видеть» её, поэтому она оделась так вызывающе. По мнению Маи, всё это из-за загадочного «подросткового синдрома». Адзусагава, недолго думая, решает помочь Сакурадзиме в решении этой странной проблемы. Однако кто мог знать, чем это обернётся...";
        public string StudioName { get; set; } = "";
        public int Episodes { get; set; } = 12;
        public Uri PosterUri { get; set; } = new Uri("pack://application:,,,/Resources/poster.jpg", UriKind.RelativeOrAbsolute);
        public Status Status { get; set; } = Status.Released;
        public Kind Kind { get; set; } = Kind.TV;
        public DateTime AiredDate { get; set; } = DateTime.Now;
        public string[] Genres { get; set; } = new string[4] { "Комедия", "Школа", "Романтика", "Сверхъестественное" };

        public AnimeData() { }

        public override string ToString()
        {
            return $"ИД: {ID}\nНазвание:{NameRus}\nОписание:{Description}\nКол-во серий:{Episodes}\nПостер: {PosterUri}\nДата выхода:{AiredDate.ToShortDateString()}\nЖанры:{string.Join(", ", Genres)}";
        }

    }
}
