using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchPointsAuction.Models;
using Websocket.Client;

namespace TwitchPointsAuction.Classes
{
    public class PubSub
    {
        private static PubSub instance;
        public static PubSub Instance
        {
            get { Debug.WriteLine("Instanse null? {0}", instance == null); return instance ?? (instance = new PubSub()); }
        }

        public event OnRewardHandler OnReward;

        private Timer PubSubPingTimer;
        private readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private IWebsocketClient Client;

        private Func<ClientWebSocket> WebSocketFactory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(60),
                    }
                };
                //client.Options.SetRequestHeader("Origin", "xxx");
                return client;
            });
        //Tests
        /*
        Random rnd = new Random();
        string[] users = { "Billy", "Willy", "Pepega", "Kappa", "PepeLaugh", "PepeHands", "Rikardo", "Gaben", "Megumin", "Aqua" };
        string[] animes = { @"https://shikimori.one/animes/z5114-fullmetal-alchemist-brotherhood",
            @"https://shikimori.one/animes/z9253-steins-gate",
            @"https://shikimori.one/animes/32281-kimi-no-na-wa",
            @"https://shikimori.one/animes/37991-jojo-no-kimyou-na-bouken-part-5-ougon-no-kaze",
            @"https://shikimori.one/animes/1575-code-geass",
            @"https://shikimori.one/animes/33486-boku-no-hero-academia-2nd-season",
            @"https://shikimori.one/animes/z38040-kono-subarashii-sekai-ni-shukufuku-wo-kurenai-densetsu",
            @"https://shikimori.one/animes/z33674-no-game-no-life-zero",
            @"https://shikimori.one/animes/z31043-boku-dake-ga-inai-machi",
            @"https://shikimori.one/animes/z38691-dr-stone",
        };
        */
        public PubSub()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
        }

        public async Task<bool> Connect()
        {
            try
            {
                Client = new WebsocketClient(new Uri(Settings.Instance.PubSettings.Uri), WebSocketFactory)
                {
                    Name = "PubSub",
                    ReconnectTimeout = TimeSpan.FromSeconds(5),
                    ErrorReconnectTimeout = TimeSpan.FromSeconds(5)
                };

                PubSubPingTimer = new Timer(new TimerCallback(SendingPing), Client, 0, 5000);
                await Client.Start();
                Subscribe();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> JoinChannel()
        {
            try
            {
                await ListenChannelPoints(Settings.Instance.PubSettings.Channel, Settings.Instance.PubSettings.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LeaveChannel()
        {
            try
            {
                await ListenChannelPoints(Settings.Instance.PubSettings.Channel, Settings.Instance.PubSettings.Token, false);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> Disconnect()
        {
            try
            {
                await Client.Stop(WebSocketCloseStatus.Empty, string.Empty);
                PubSubPingTimer.Dispose();
                Client.Dispose();
                return true;
            }
            catch { return false; }
        }

        private void SendingPing(object state)
        {
            try
            {
                var client = state as IWebsocketClient;
                if (client != null && client.IsRunning)
                {
                    //Tests
                    /*
                    var user = users[rnd.Next(10)];
                    var shikiurl = animes[rnd.Next(10)];
                    var cost = (uint)rnd.Next(1, 10) * 1000;
                    OnReward?.Invoke("twitch", new Models.Reward("1","1",user,shikiurl,cost,true));
                    */
                    client.Send((new JObject() { ["type"] = "PING" }).ToString());
                }
            }
            catch { }
        }

        private async Task ListenChannelPoints(string channel, string token, bool listen = true)
        {
            try
            {
                if (Client != null && Client.IsRunning)
                {
                    string userid = (await Requests.GetUserID(channel)).Item1;
                    Client.Send((JObject.FromObject(new
                    {
                        type = listen ? "LISTEN" : "UNLISTEN",
                        data = new
                        {
                            topics = new string[] { "channel-points-channel-v1." + userid },
                            auth_token = token
                        }
                    })).ToString());
                    Debug.WriteLine("Listen Send!");
                }
            }
            catch { }
        }

        private void Subscribe()
        {
            try
            {
                if (Client != null && Client.IsRunning)
                {
                    Client.ReconnectionHappened.Subscribe(type =>
                    {
                        Debug.WriteLine($"Reconnection happened, type: {type}, url: {Client.Url}");
                    });

                    Client.DisconnectionHappened.Subscribe(info =>
                    {
                        Debug.WriteLine($"Disconnection happened, type: {info.Type}");
                    });

                    Client.MessageReceived.Subscribe(msg =>
                    {
                        Debug.WriteLine($"Message received: {msg}");
                        var jObject = JObject.Parse(msg.Text);
                        if ((string)jObject["type"] == "MESSAGE" && jObject["data"] != null)
                        {
                            var newReward = JsonParser.ParseReward(((string)jObject["data"]["message"]).Replace('\\', Char.MinValue));
                            OnReward?.Invoke("twitch", newReward);
                        }
                    });
                }
            }
            catch { }
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            ExitEvent.Set();
        }

        private void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
        {
            ExitEvent.Set();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}
