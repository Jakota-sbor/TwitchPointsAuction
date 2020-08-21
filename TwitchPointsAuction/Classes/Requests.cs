using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction.Classes
{
    class Requests
    {
        static HttpClient client = new HttpClient();
        static string _TwitchClientID { get; set; }
        static List<(string, string)> _TwitchHeaders = new List<(string, string)>()
            {
                ("Accept", "application/vnd.twitchtv.v5+json"),
                ("Client-ID", "hfgz4sbkqm4m5tjkvtqmr5bpzwiknd")
            };
        static List<(string, string)> _ShikimoriHeaders = new List<(string, string)>()
            {
                ("User-Agent", "J-_YKQ0Xn1FTXNwLW8x5rLy5ujUzTKmqOBgNY15tDcU"),
            };
        //Twitch получение логина пользователя (канала) по ID
        static readonly string _TwitchGetUserByIdUrl = @"https://api.twitch.tv/kraken/users?login={0}";
        //Steam API
        static readonly string _SteamGetGameDataUrl = @"https://store.steampowered.com/api/appdetails?appids={0}";
        static readonly string _SteamGetAllGamesListUrl = @"http://api.steampowered.com/ISteamApps/GetAppList/v2";
        //PS Store API
        static readonly string _PSStoreGameDataUrl = @"https://store.playstation.com/store/api/chihiro/00_09_000/titlecontainer/RU/ru/999/{0}";
        //Shikimori API, Doc: https://shikimori.one/api/doc
        static readonly string _ShikimoriGetAnimeDataUrl = @"https://shikimori.one/api/animes/{0}";
        static readonly string _ShikimoriGetUserAnimeListUrl = @"https://shikimori.one/api/users/{0}/anime_rates?limit=5000";

        static Requests()
        {
            var steamServicePoint = ServicePointManager.FindServicePoint(new Uri("https://store.steampowered.com"));
            var psstoreServicePoint = ServicePointManager.FindServicePoint(new Uri("https://store.playstation.com"));
            var shikiServicePoint = ServicePointManager.FindServicePoint(new Uri("https://shikimori.one"));
            var twitchApiServicePoint = ServicePointManager.FindServicePoint(new Uri("https://api.twitch.tv"));

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 10;
            ServicePointManager.DnsRefreshTimeout = 10000;
            steamServicePoint.ConnectionLeaseTimeout = 0;
            steamServicePoint.MaxIdleTime = int.MaxValue;
            psstoreServicePoint.ConnectionLeaseTimeout = 0;
            psstoreServicePoint.MaxIdleTime = int.MaxValue;
            shikiServicePoint.ConnectionLeaseTimeout = 0;
            shikiServicePoint.MaxIdleTime = int.MaxValue;
            twitchApiServicePoint.ConnectionLeaseTimeout = -1;
            twitchApiServicePoint.MaxIdleTime = int.MaxValue;
        }

        public static async Task<(LotData, HttpStatusCode)> GetContentData(ApiType apiType,string ID)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in _ShikimoriHeaders)
                {
                    client.DefaultRequestHeaders.Add(item.Item1, item.Item2);
                }

                string ApiUri = apiType switch
                {
                    ApiType.Text => string.Empty,
                    ApiType.Steam => _SteamGetGameDataUrl,
                    ApiType.PSStore => _PSStoreGameDataUrl,
                    ApiType.Kinopoisk => string.Empty,
                    ApiType.Shikimori => _ShikimoriGetAnimeDataUrl,
                    _ => string.Empty
                };

                HttpResponseMessage response = await client.GetAsync(new Uri(string.Format(ApiUri, ID)));
                string jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(jsonString);
                return apiType switch
                {
                    ApiType.Text => (null, HttpStatusCode.OK),
                    ApiType.Steam => (JsonParser.ParseSteamGameData(jsonString), response.StatusCode),
                    ApiType.PSStore => (JsonParser.ParsePsStoreGameData(jsonString), response.StatusCode),
                    ApiType.Kinopoisk => (null,HttpStatusCode.OK),
                    ApiType.Shikimori => (JsonParser.ParseAnimeData(jsonString), response.StatusCode),
                    _ => (null, HttpStatusCode.OK)
                };
            }
            catch (HttpRequestException e)
            {
                return (null, HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return (null, HttpStatusCode.InternalServerError);
            }
        }

        #region Shikimori
        public static async Task<(string, HttpStatusCode)> GetUserID(string userName)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in _TwitchHeaders)
                {
                    client.DefaultRequestHeaders.Add(item.Item1, item.Item2);
                }
                HttpResponseMessage response = await client.GetAsync(new Uri(string.Format(_TwitchGetUserByIdUrl, userName)));
                string jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(jsonString);
                JObject o = JObject.Parse(jsonString);

                return ((string)o["users"][0]["_id"], response.StatusCode);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.ToString());
                return (string.Empty, HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return (string.Empty, HttpStatusCode.InternalServerError);
            }
        }

        public static async Task<(IList<string>, HttpStatusCode)> LoadCompletedAnimeData(string userId= "378254")
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.GetAsync(new Uri(string.Format(_ShikimoriGetUserAnimeListUrl, userId)));
                string jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("JSON: " + jsonString);
                return (JsonParser.ParseListAnimeData(jsonString), response.StatusCode);
            }
            catch (HttpRequestException e)
            {
                return (null, HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return (null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Steam
        public static async Task<(IList<SteamLot>, HttpStatusCode)> LoadAllGames()
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.GetAsync(new Uri(_SteamGetAllGamesListUrl));
                string jsonString = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine("JSON: "+jsonString);
                return (JsonParser.ParseListGamesData(jsonString), response.StatusCode);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.ToString());
                return (null, HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return (null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        /*
        public static async Task<(AnimeData, HttpStatusCode)> SearchAnime(string name, int yearfrom, int yearto, Kind type = Kind.TV)
        {
            //https://shikimori.one/api/animes?search="Моя%20геройская%20академия%203"&order=aired_on&limit=10&kind=tv
            client.DefaultRequestHeaders.Clear();
            var searchStr = $"https://shikimori.one/api/animes?search=\"{name}\"&season={yearfrom}_{yearto}&kind={type.ToString().ToLower()}&limit=10";
            HttpResponseMessage response = await client.GetAsync(new Uri(searchStr));
            string jsonString = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("JSON: " + jsonString);
            return (JsonParser.ParseAnimeData(jsonString), response.StatusCode);
        }
        */
    }
}
