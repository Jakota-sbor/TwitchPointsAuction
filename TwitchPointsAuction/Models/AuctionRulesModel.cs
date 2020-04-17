using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;
using TwitchPointsAuction.Properties;

namespace TwitchPointsAuction.Models
{
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
        private NotifyCollection<Kind> forbiddenTypes = new NotifyCollection<Kind>();
        private NotifyCollection<string> forbiddenTitles = new NotifyCollection<string>();

        public int? YearFrom { get => yearFrom; set { yearFrom = value; OnPropertyChanged(); } }
        public int? YearTo { get => yearTo; set { yearTo = value; OnPropertyChanged(); } }
        public NotifyCollection<string> CompletedAnime { get => completedAnime; set { completedAnime = value; OnPropertyChanged(); } }
        public NotifyCollection<Genres> ForbiddenGenres { get => forbiddenGenres; set { forbiddenGenres = value; OnPropertyChanged(); } }
        public NotifyCollection<Kind> ForbiddenTypes { get => forbiddenTypes; set { forbiddenTypes = value; OnPropertyChanged(); } }
        public NotifyCollection<string> ForbiddenTitles { get => forbiddenTitles; set { forbiddenTitles = value; OnPropertyChanged(); } }
        
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

        private RelayCommand addGenreCommand;
        public RelayCommand AddGenreCommand
        {
            get
            {
                return addGenreCommand ??
                    (addGenreCommand = new RelayCommand(param =>
                    {
                        Debug.WriteLine("add forb genre: " + ((Genres)param).ToString());
                        var genre = (Genres)param;
                        ForbiddenGenres.Add(genre);
                    }));
            }
        }

        private RelayCommand removeGenreCommand;
        public RelayCommand RemoveGenreCommand
        {
            get
            {
                return removeGenreCommand ??
                    (removeGenreCommand = new RelayCommand(param =>
                    {
                        Debug.WriteLine("remove forb genre: " + ((Genres)param).ToString());
                        var genre = (Genres)param;
                        ForbiddenGenres.Remove(genre);
                    }));
            }
        }

        public (bool, BetError) IsInvalid(AnimeData anime)
        {
            if (!IsEnabled)
                return (false, BetError.None);
            else
            {
                bool InvalidName, InvalidDate, InvalidKind, InvalidGenres, Completed;
                var error = BetError.None;
                Debug.WriteLine("ANIME ID: " + anime.ID);
                Debug.WriteLine("COMPLETED ANIME: " + CompletedAnime.Count);
                Completed = CompletedAnime.Any(x => x == anime.ID);
                InvalidDate = (YearFrom.HasValue && anime.AiredDate.Year < YearFrom) || (YearTo.HasValue && anime.AiredDate.Year > YearTo);
                InvalidName = ForbiddenTitles.Any(x => anime.NameRus.ToUpper().Contains(x.ToUpper()));
                InvalidKind = ForbiddenTypes.Any(x => x == anime.Kind);
                InvalidGenres = ForbiddenGenres.Any(x => anime.Genres.Contains(x));
                if (Completed)
                    error = BetError.Completed;
                else if (InvalidDate)
                    error = BetError.InvalidDate;
                else if (InvalidName)
                    error = BetError.InvalidName;
                else if (InvalidKind)
                    error = BetError.InvalidKind;
                else if (InvalidGenres)
                    error = BetError.InvalidGenre;
                return (Completed || InvalidDate || InvalidName || InvalidKind || InvalidGenres,
                        error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
