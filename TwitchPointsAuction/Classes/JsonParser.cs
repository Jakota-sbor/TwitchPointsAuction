using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction.Classes
{
    class JsonParser
    {
        private static Regex ShikiBracketsRegex = new Regex(@"\[[^\]]*\]", RegexOptions.Compiled);
        private static CultureInfo Culture = CultureInfo.CreateSpecificCulture("ru-RU");

        public static ShikimoriLot ParseAnimeData(string jsonString)
        {
            JObject jsonO = JObject.Parse(jsonString);

            var id = ((string)jsonO["id"]).Replace("z","");
            var nameEng = (string)jsonO["name"];
            var nameRus = (string)jsonO["russian"];
            var description =  string.Empty;
            // var description = jsonO["description"] != null ? ShikiBracketsRegex.Replace(((string)jsonO["description"]).Replace(@"[[", "").Replace(@"]]", "").Replace(@"\r\n",""), "") : string.Empty;
            int episodes = int.TryParse((string)jsonO["episodes"], out episodes) ? episodes : 0;
            var posterUri = new Uri("https://shikimori.one" + (string)jsonO["image"]["original"], UriKind.Absolute);
            var status = (string)jsonO["status"] == "released" ? Status.Released : Status.Ongoing;
            var kind = (string)jsonO["kind"] switch
            {
                "tv" => Kind.TV,
                "movie" => Kind.Movie,
                "ova" => Kind.OVA,
                "ona" => Kind.ONA,
                "special" => Kind.Special,
                _ => Kind.Special
            };
            DateTime airedDate = DateTime.TryParse((string)jsonO["aired_on"], out airedDate) ? airedDate : DateTime.Now;
            var genres = jsonO["genres"] != null ? (from genre in jsonO["genres"].Children() select int.TryParse((string)genre["id"], out var genreID) ? (AnimeGenres)genreID : AnimeGenres.None) : null;
            //Enum.TryParse(typeof(AnimeGenres), (string)genre["russian"], out var genreenum) ? (AnimeGenres)genreenum : AnimeGenres.Shonen)
            return new ShikimoriLot
            {
                ID = id,
                NameEng = nameEng,
                Name = nameRus,
                Description = description,
                Episodes = episodes,
                PosterUri = posterUri,
                Status = status,
                ReleaseDate = airedDate,
                Genres = genres,
                Kind = kind
            };

        }

        public static SteamLot ParseSteamGameData(string jsonString)
        {
            var jsonO = JObject.Parse(System.Uri.UnescapeDataString(jsonString)).Children().First().Children().First();

            //System.Text.RegularExpressions.Regex.Unescape(String)

            var success = bool.TryParse((string)jsonO["success"], out var suc) ? suc : false;

            if (success && jsonO["data"] != null)
            {
                Debug.WriteLine("Success");
                var id = (string)jsonO["data"]["steam_appid"];
                var name = (string)jsonO["data"]["name"];
                var description = (string)jsonO["data"]["short_description"];
                //var posterUri = new Uri(((string)jsonO["data"]["header_image"]).Replace("\\", ""), UriKind.Absolute);
                DateTime releaseDate = DateTime.TryParse(((string)jsonO["data"]["release_date"]["date"]), Culture, DateTimeStyles.None, out releaseDate) ? releaseDate : DateTime.Now;
                //var status = (string)jsonO["data"]["release_date"]["coming_soon"] == "false" ? Status.Released : Status.NotReleased;
                var status = releaseDate <= DateTime.Now ? Status.Released : Status.NotReleased;
                var genres = jsonO["data"]["genres"] != null ? (from genre in jsonO["data"]["genres"].Children() select int.TryParse((string)genre["id"], out var genreID) ? (SteamGenres)genreID : SteamGenres.None) : null;

                return new SteamLot
                {
                    ID = id,
                    Name = name,
                    Description = description,
                    //PosterUri = posterUri,
                    Status = status,
                    ReleaseDate = releaseDate,
                    Genres = genres
                };
            }
            return null;
        }

        public static PsStoreLot ParsePsStoreGameData(string jsonString)
        {
            var jsonO = JObject.Parse(jsonString);

            //System.Text.RegularExpressions.Regex.Unescape(String)
            //Debug.WriteLine("PS Store Name: " + (string)jsonO["name"]);          
            if (jsonO["short_name"] != null)
            {
                var id = (string)jsonO["id"];

                var name = (string)jsonO["short_name"];
                var description = (string)jsonO["long_desc"];
                DateTime releaseDate = DateTime.TryParse(((string)jsonO["release_date"]), Culture, DateTimeStyles.None, out releaseDate) ? releaseDate : DateTime.Now;
                var status = releaseDate <= DateTime.Now ? Status.Released : Status.NotReleased;
                //var genres = jsonO["data"]["genres"] != null ? (from genre in jsonO["data"]["genres"].Children() select int.TryParse((string)genre["id"], out var genreID) ? (SteamGenres)genreID : SteamGenres.None) : null;

                var psstorelot = new PsStoreLot
                {
                    ID = id,
                    Name = name,
                    Description = description,
                    Status = status,
                    ReleaseDate = releaseDate
                };
                Debug.WriteLine(psstorelot.ToString());
                return psstorelot;
            }
            
            return null;
        }

        public static IList<string> ParseListAnimeData(string jsonString)
        {
            List<string> AnimeList = new List<string>();
            var results = JArray.Parse(jsonString);

            foreach (var result in results) 
            {
                if ((string)result["status"] == "completed")
                {
                    //Debug.WriteLine("TITLE: "+result["anime"]["russian"].ToString()+" | ID: "+result["anime"]["id"].ToString());
                    AnimeList.Add(result["anime"]["id"].ToString());
                }
            }
               
            return AnimeList;
        }

        public static IList<SteamLot> ParseListGamesData(string jsonString)
        {
            List<SteamLot> GamesList = new List<SteamLot>();
            var appsObject = JObject.Parse(jsonString);
            //Debug.WriteLine(appsObject["applist"]["apps"]);
            var gamesArray = JsonConvert.DeserializeAnonymousType((appsObject["applist"]["apps"]).ToString(), GamesList);
            //Debug.WriteLine(gamesArray);
            //foreach (var game in gamesArray)
            //{
            //    Debug.WriteLine(game);
            //}

            return GamesList;
        }

        public static Reward ParseReward(string jsonString)
        {
            JObject jsonO = JObject.Parse(jsonString);

            var id = (string)jsonO["data"]["redemption"]["reward"]["id"];
            var title = (string)jsonO["data"]["redemption"]["reward"]["title"];
            var user = (string)jsonO["data"]["redemption"]["user"]["display_name"];
            var userInput = (string)jsonO["data"]["redemption"]["user_input"];
            uint cost = uint.TryParse((string)jsonO["data"]["redemption"]["reward"]["cost"], out cost) ? cost : 0;
            bool userInputRequired = bool.TryParse((string)jsonO["data"]["redemption"]["reward"]["is_user_input_required"], out bool uIR) ? uIR : false;
            Debug.WriteLine("Parsing reward...");
            return new Reward(id, title.ToLower(), user.ToLower(), userInput, cost, userInputRequired);
        }
    }
}
