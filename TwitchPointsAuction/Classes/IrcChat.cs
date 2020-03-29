using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchPointsAuction.Classes
{
    public class IrcChat : IChat
    {
        //public event OnMessageHandler OnMessage;

        public bool IsConnected
        {
            get
            {
                return tcpclient != null && tcpclient.Connected && ns != null && sr != null && sw != null;
            }
        }

        private IrcChatSettings chatSettings;

        public TcpClient tcpclient { get; set; }
        public NetworkStream ns { get; set; }
        public StreamReader sr { get; set; }
        public StreamWriter sw { get; set; }

        public IrcChatSettings ChatSettings
        {
            get
            {
                return chatSettings;
            }

            set
            {
                chatSettings = value;
            }
        }

        Task ReadMsgThread;

        CancellationTokenSource cancelReadSource;
        CancellationToken cancelReadToken;

        public IrcChat(IrcChatSettings settings)
        {
            ChatSettings = settings;
            cancelReadSource = new CancellationTokenSource();
            cancelReadToken = cancelReadSource.Token;
        }

        public async Task<bool> Connect()
        {
            try
            {
                return await Task.Run<bool>(() =>
                {
                    tcpclient = new TcpClient(ChatSettings.IrcUrl, ChatSettings.IrcPort.Value);
                    ns = tcpclient.GetStream();
                    sr = new StreamReader(ns);
                    sw = new StreamWriter(ns) { AutoFlush = true } ;
                    sw?.WriteLine("PASS " + ChatSettings.Token);
                    sw?.WriteLine("NICK " + ChatSettings.Name);
                    sw?.WriteLine("USER " + ChatSettings.Name);
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
                        sw?.WriteLine("JOIN #" + ChatSettings.Channel);
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

        public async Task ExitFromChannel()
        {
            await Task.Run(() =>
            {
                if (IsConnected)
                    sw?.WriteLine("PART " + ChatSettings.Channel);
            });
        }

        public void SendMessage(string message)
        {
            bool status = false;
            try
            {
                if (IsConnected)
                {
                    sw?.WriteLine("PRIVMSG #" + ChatSettings.Name + " :" + message);
                }
                status = true;
            }
            catch (Exception e)
            {
                status = false;
            }
        }


        public void Disconnect()
        {
            try
            {
                if (cancelReadSource != null)
                    cancelReadSource.Cancel();

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
            }
            catch
            { }
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
                    if (inputLine != null && (inputLine.Contains("PRIVMSG") || inputLine.Contains("WHISPER")))
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
