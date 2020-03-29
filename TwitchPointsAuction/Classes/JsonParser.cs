using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction.Classes
{
    class JsonParser
    {
        public static AnimeData ParseAnimeData(string jsonString)
        {
            //List<AnimeData> AnimeList = new List<AnimeData>();
            //foreach (var jsonString in jsonStrings)
            //{
                JObject jsonO = JObject.Parse(jsonString);

                var id = (string)jsonO["id"];
                var nameEng = (string)jsonO["name"];
                var nameRus = (string)jsonO["russian"];
                var description = (string)jsonO["description"];
                int episodes = int.TryParse((string)jsonO["episodes"], out episodes) ? episodes : 0;
                var posterUri = new Uri("https://shikimori.one" + (string)jsonO["image"]["original"], UriKind.Absolute);
                var status = (string)jsonO["status"] == "released" ? Status.Released : Status.Ongoing;
                DateTime airedDate = DateTime.TryParse((string)jsonO["aired_on"], out airedDate) ? airedDate : DateTime.Now;
                var genres = (from genre in jsonO["genres"].Children() select (string)genre["russian"]).ToList();
                return new AnimeData
                {
                    ID = id,
                    NameEng = nameEng,
                    NameRus = nameRus,
                    Description = description,
                    Episodes = episodes,
                    PosterUri = posterUri,
                    Status = status,
                    AiredDate = airedDate,
                    Genres = genres
                };
            //}
            //return AnimeList;
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
            return new Reward(id, title, user, userInput, cost, userInputRequired);
        }
    }
}
