﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace TwitchPointsAuction.Classes
{
    class PubSub
    {
        public static event OnRewardHandler OnReward;
        private static Timer PubSubPingTimer;
        private static readonly Uri PubSubUri = new Uri("wss://pubsub-edge.twitch.tv");
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private static readonly IWebsocketClient client = new WebsocketClient(PubSubUri, WebSocketFactory);
        private static Func<ClientWebSocket> WebSocketFactory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                    }
                };
                //client.Options.SetRequestHeader("Origin", "xxx");
                return client;
            });

        public static async Task Initialize()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            client.Name = "PubSub";
            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(type =>
            {
                Debug.WriteLine($"Reconnection happened, type: {type}, url: {client.Url}");
            });
            client.DisconnectionHappened.Subscribe(info =>
                Debug.WriteLine($"Disconnection happened, type: {info.Type}"));

            client.MessageReceived.Subscribe(msg =>
            {
                Debug.WriteLine($"Message received: {msg}");
                var jObject = JObject.Parse(msg.Text);
                if ((string)jObject["type"] == "MESSAGE" && jObject["data"] != null)
                {
                    var newReward = JsonParser.ParseReward(((string)jObject["data"]["message"]).Replace('\\', Char.MinValue));
                    OnReward?.Invoke("twitch", newReward);
                }
            });

            Debug.WriteLine("Starting...");
            await client.Start();
            Debug.WriteLine("Started.");
            PubSubPingTimer = new Timer(new TimerCallback(SendingPing), client, 0, 10000);
            //Task.Run(() => StartSendingPing(client));
            await ListenWhispers(client);
        }

        private static void SendingPing(object state)
        {
            var client = state as IWebsocketClient;
            if (client.IsRunning)
                client.Send((new JObject() { ["type"] = "PING" }).ToString());
        }

        private static async Task ListenWhispers(IWebsocketClient client)
        {
            if (client.IsRunning)
            {
                string userid = (await Requests.GetUserID("sinedd92")).Item1;
                client.Send((JObject.FromObject(new
                {
                    type = "LISTEN",
                    data = new
                    {
                        topics = new string[] { "channel-points-channel-v1." + userid },
                        auth_token = "lwous17y267p47nk8o9dng6h2g68ur" //sinedd92
                    }
                })).ToString());
            }
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            ExitEvent.Set();
        }

        private static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
        {
            ExitEvent.Set();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}