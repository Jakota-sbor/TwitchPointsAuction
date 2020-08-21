using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction.Classes
{
    public class IrcChat : IChat
    {
        //public event OnMessageHandler OnMessage;

        private static IrcChat instance;
        public static IrcChat Instance
        {
            get { return instance ?? (instance = new IrcChat()); }
        }

        public bool IsConnected
        {
            get
            {
                return tcpclient != null && tcpclient.Connected && ns != null && sr != null && sw != null;
            }
        }

        TcpClient tcpclient { get; set; }
        NetworkStream ns { get; set; }
        StreamReader sr { get; set; }
        StreamWriter sw { get; set; }

        Task ReadMsgThread;

        CancellationTokenSource cancelReadSource;
        CancellationToken cancelReadToken;

        public IrcChat()
        {
        }

        public async Task<bool> Connect()
        {
            try
            {
                return await Task.Run<bool>(() =>
                {
                    cancelReadSource = new CancellationTokenSource();
                    cancelReadToken = cancelReadSource.Token;
                    tcpclient = new TcpClient(Settings.Instance.IrcSettings.IrcUrl, Settings.Instance.IrcSettings.IrcPort.Value);
                    ns = tcpclient.GetStream();
                    sr = new StreamReader(ns);
                    sw = new StreamWriter(ns) { AutoFlush = true } ;
                    sw?.WriteLine("PASS oauth:" + Settings.Instance.IrcSettings.Token);
                    sw?.WriteLine("NICK " + Settings.Instance.IrcSettings.Name);
                    ReadMsgThread = ReadStreamTask(cancelReadToken);
                    return true;
                });
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public async Task<bool> JoinChannel()
        {
            try
            {
                return await Task.Run<bool>(() =>
                {
                    if (IsConnected)
                    {
                        sw?.WriteLine("JOIN #" + Settings.Instance.IrcSettings.Channel);
                        sw?.WriteLine("CAP REQ :twitch.tv/commands");
                        sw?.WriteLine("CAP REQ :twitch.tv/tags");
                        sw?.WriteLine("CAP REQ :twitch.tv/tags twitch.tv/commands");
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> LeaveChannel()
        {
            try
            {
                return await Task.Run<bool>(() =>
                {
                    if (IsConnected)
                        sw?.WriteLine("PART " + Settings.Instance.IrcSettings.Channel);
                    return true;
                });
            }
            catch
            {
                return false;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (IsConnected)
                    sw?.WriteLine("PRIVMSG #" + Settings.Instance.IrcSettings.Channel + " :" + message);
            }
            catch (Exception e)
            {
            }
        }


        public async Task<bool> Disconnect()
        {
            try
            {
                return await Task<bool>.Run(() =>
                {
                    cancelReadSource?.Cancel();

                    while (!ReadMsgThread.IsCanceled)
                    {
                    }

                    sw?.Close();
                    sr?.Close();
                    ns?.Close();
                    tcpclient?.Close();

                    sw = null;
                    sr = null;
                    ns = null;
                    tcpclient = null;
                    return true;
                });
            }
            catch
            {
                return false;
            }
        }


        public async Task ReadStreamTask(CancellationToken token)
        {
            try
            {
                if (IsConnected)
                {
                    string inputLine = await sr.ReadLineAsync();
                    if (inputLine != null)
                        Trace.WriteLine(inputLine);
                    if (inputLine != null && (inputLine.Contains("PRIVMSG")))
                        ParseIrcMessage(inputLine);
                    else if (inputLine != null && inputLine.Contains("PING"))
                    {
                        Trace.WriteLine("!PING");
                        await sw?.WriteLineAsync("PONG :tmi.twitch.tv");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                if (token.IsCancellationRequested)
                    Debug.WriteLine("CANCEL TASK REQUESTED");
                else
                {
                    await Task.Delay(50);
                    ReadMsgThread = ReadStreamTask(token);
                }
            }
        }


        void ParseIrcMessage(string msgLine)
        {

            //if (OnMessage != null)
            //{
            //    var lines = msgLine.Remove(0, 1).Split(new char[] { ' ' }, 2);
            //var twitchMsg = Parsing.ParseTwitchMsg(lines[1], lines[0]);
            //if (twitchMsg != null)
            //    OnMessage("twitch", twitchMsg);
            //}
        }
    }
}
