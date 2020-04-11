using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwitchPointsAuction.Classes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Логика взаимодействия для AuctionSettingsWindow.xaml
    /// </summary>
    public partial class AuctionSettingsWindow : Window
    {
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

        public AuctionRulesModel AuctionRules { get; set; }
        public AuctionSettingsModel AuctionSettings { get; set; }

        public AuctionSettingsWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            AuctionSettings = new AuctionSettingsModel();
            AuctionRules = new AuctionRulesModel() { ForbiddenGenres = new NotifyDictionary<string, bool>(DefaultGenres.Select(x=> new WritableKeyValuePair<string, bool>(x,false))),
                                                     ForbiddenTypes = new NotifyDictionary<Classes.Kind, bool>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x=> new WritableKeyValuePair<Kind, bool>(x,false)))}; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AuctionRules.ForbiddenTypes[Kind.Movie] = true;
        }
    }
}
