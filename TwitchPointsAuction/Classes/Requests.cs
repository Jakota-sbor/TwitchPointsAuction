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

        static Requests()
        {
            var shikiServicePoint = ServicePointManager.FindServicePoint(new Uri("https://shikimori.one"));
            var twitchApiServicePoint = ServicePointManager.FindServicePoint(new Uri("https://api.twitch.tv"));

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 10;
            ServicePointManager.DnsRefreshTimeout = 10000;
            shikiServicePoint.ConnectionLeaseTimeout = 10000;
            shikiServicePoint.MaxIdleTime = 10000;
            twitchApiServicePoint.ConnectionLeaseTimeout = 10000;
            twitchApiServicePoint.MaxIdleTime = 10000;
        }

        public static async Task<(string, HttpStatusCode)> GetUserID(string username)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in _TwitchHeaders)
                {
                    client.DefaultRequestHeaders.Add(item.Item1, item.Item2);
                }
                HttpResponseMessage response = await client.GetAsync(new Uri("https://api.twitch.tv/kraken/users?login=" + username));
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

        public static async Task<(AnimeData, HttpStatusCode)> GetAnimeData(string animeid)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.GetAsync(new Uri("https://shikimori.one/api/animes/" + animeid));
                string jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("JSON: " + jsonString);
                return (JsonParser.ParseAnimeData(jsonString), response.StatusCode);
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

        public static async Task<(ICollection<string>, HttpStatusCode)> GetCompletedAnimeData(string userid= "378254")
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.GetAsync(new Uri("https://shikimori.one/api/users/"+ userid+"/anime_rates?limit=5000"));
                string jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("JSON: " + jsonString);
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
