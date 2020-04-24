using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    [JsonObject]
    public class AuctionRulesModel : INotifyPropertyChanged
    {
        /*
        private readonly List<string> DefaultGenres = new List<string>()
        {
            "Сёнен",
            "Сёнен-ай",
            "Сейнен",
            "Сёдзё",
            "Сёдзё-ай",
            "Дзёсей",
            "Комедия",
            "Романтика",
            "Школа",
            "Безумие",
            "Боевые искусства",
            "Вампиры",
            "Военное",
            "Гарем",
            "Демоны",
            "Детектив",
            "Детское",
            "Драма",
            "Игры",
            "Исторический",
            "Космос",
            "Магия",
            "Машины",
            "Меха",
            "Музыка",
            "Пародия",
            "Повседневность",
            "Полиция",
            "Приключения",
            "Психологическое",
            "Самураи",
            "Сверхъестественное",
            "Спорт",
            "Супер сила",
            "Ужасы",
            "Фантастика",
            "Фэнтези",
            "Экшен",
            "Этти",
            "Триллер",
            "Хентай",
            "Яой",
            "Юри"
        };
        */
        private int? yearFrom = null;
        private int? yearTo = null;
        private NotifyCollection<string> completedAnime = new NotifyCollection<string>();
        private NotifyCollection<Genres> forbiddenGenres = new NotifyCollection<Genres>();
        private NotifyCollection<Kind> forbiddenKinds = new NotifyCollection<Kind>();
        private NotifyCollection<string> forbiddenTitles = new NotifyCollection<string>();
        private string auctionRulesText = null;

        public int? YearFrom { get => yearFrom; set {
                if (!value.HasValue || (value.HasValue && value.Value == 0))
                    yearFrom = value;
                else if (value > yearTo)
                    yearFrom = yearTo;
                else if (value >= 0 && value <= DateTime.Now.Year)
                    yearFrom = value;
                else
                    yearFrom = DateTime.MinValue.Year;

                OnPropertyChanged(); } }
        public int? YearTo { get => yearTo; set {
                if (!value.HasValue || (value.HasValue && value.Value == 0))
                    yearTo = value;
                else if (value < yearFrom)
                    yearTo = yearFrom;
                else if (value >= 0 && value <= DateTime.Now.Year)
                    yearTo = value;
                else
                    yearTo = DateTime.Now.Year;
                OnPropertyChanged(); } }
        public NotifyCollection<string> CompletedAnime { get => completedAnime; set { completedAnime = value; OnPropertyChanged(); } }
        public NotifyCollection<Genres> ForbiddenGenres { get => forbiddenGenres;  set { forbiddenGenres = value; OnPropertyChanged(); } }
        public NotifyCollection<Kind> ForbiddenKinds { get => forbiddenKinds; set { forbiddenKinds = value; OnPropertyChanged(); } }
        public NotifyCollection<string> ForbiddenTitles { get => forbiddenTitles; set { forbiddenTitles = value; OnPropertyChanged(); } }
        public string AuctionRulesText { get => auctionRulesText; set { auctionRulesText = value; OnPropertyChanged(); } }

        public bool IsEnabled { get; set; } = true;

        public AuctionRulesModel()
        {
            /*
            ForbiddenGenres = new NotifyDictionary<string, bool>(DefaultGenres.Select(x => new WritableKeyValuePair<string, bool>(x, false))) : JsonConvert.DeserializeObject<NotifyDictionary<string, bool>>(forbiddenGenresJson);
            ForbiddenTypes = new NotifyDictionary<Classes.Kind, bool>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x => new WritableKeyValuePair<Kind, bool>(x, false))) : JsonConvert.DeserializeObject<NotifyDictionary<Kind, bool>>(forbiddenTypesJson);
            ForbiddenTitles = new NotifyDictionary<string, bool>();
            */

        }

        public void SaveRules()
        {
            //forbiddenGenresJson = JsonConvert.SerializeObject(ForbiddenGenres);
            //forbiddenTypesJson = JsonConvert.SerializeObject(ForbiddenTypes);
            //forbiddenTitlesJson = JsonConvert.SerializeObject(ForbiddenTitles);
            //Debug.WriteLine(forbiddenGenresJson);
        }

        public (bool, BetError) IsInvalid(AnimeData anime)
        {
            if (!IsEnabled)
                return (false, BetError.None);
            else
            {
                bool InvalidName, InvalidDate, InvalidKind, InvalidGenres, Completed;
                Completed = CompletedAnime.Any(x => x == anime.ID);
                if (Completed)
                    return (true, BetError.Completed);
                InvalidDate = (YearFrom.HasValue && anime.AiredDate.Year < YearFrom) || (YearTo.HasValue && anime.AiredDate.Year > YearTo);
                if (InvalidDate)
                    return (true,BetError.InvalidDate);
                foreach (var item in ForbiddenKinds)
                {
                    Debug.WriteLine("FORB KIND: " + item.ToString());
                }
                Debug.WriteLine("ANIME KIND: " + anime.Kind.ToString());
                InvalidKind = ForbiddenKinds.Any(x => x.Equals(anime.Kind));
                if (InvalidKind)
                    return (true, BetError.InvalidKind);
                InvalidGenres = ForbiddenGenres.Any(x => anime.Genres.Contains(x));
                if (InvalidGenres)
                    return (true, BetError.InvalidGenre);
                InvalidName = ForbiddenTitles.Any(x => anime.NameRus.ToUpper().Split(" ").Contains(x.ToUpper()));
                if (InvalidName)
                    return (true, BetError.InvalidName);

                return (false, BetError.None);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
