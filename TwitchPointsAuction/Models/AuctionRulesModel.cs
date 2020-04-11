using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class AuctionRulesModel : INotifyPropertyChanged
    {
        private int? yearFrom = null;
        private int? yearTo = null;
        private NotifyDictionary<string,bool> forbiddenGenres = new NotifyDictionary<string, bool>();
        private NotifyDictionary<Kind,bool> forbiddenTypes = new NotifyDictionary<Kind,bool>();
        private NotifyDictionary<string, bool> forbiddenTitles = new NotifyDictionary<string, bool>();

        public int? YearFrom { get => yearFrom; set => yearFrom = value; }
        public int? YearTo { get => yearTo; set => yearTo = value; }
        public NotifyDictionary<string,bool> ForbiddenGenres { get => forbiddenGenres; set => forbiddenGenres = value; }
        public NotifyDictionary<Kind,bool> ForbiddenTypes { get => forbiddenTypes; set => forbiddenTypes = value; }
        public NotifyDictionary<string,bool> ForbiddenTitles { get => forbiddenTitles; set => forbiddenTitles = value; }
        public bool IsEnabled { get; set; } = false;

        public (bool, BetError) IsInvalid(AnimeData anime)
        {
            if (!IsEnabled)
                return (false, BetError.None);
            else
            {
                bool InvalidName, InvalidDate, InvalidKind, InvalidGenres;
                InvalidDate = (YearFrom.HasValue && anime.AiredDate.Year < YearFrom) || (YearTo.HasValue && anime.AiredDate.Year > YearTo);
                InvalidName = ForbiddenTitles.Where(x=> x.Value).Any(x => x.Key.ToUpper().Contains(anime.NameRus.ToUpper()));
                InvalidKind = ForbiddenTypes.Where(x => x.Value).Any(x => x.Key == anime.Kind);
                InvalidGenres = ForbiddenGenres.Where(x => x.Value).Any(x => anime.Genres.Contains(x.Key));
                return (InvalidDate || InvalidName || InvalidKind || InvalidGenres,
                        BetError.None);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
