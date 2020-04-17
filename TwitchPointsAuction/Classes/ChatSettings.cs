using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    public abstract class ChatSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string _Name;
        private string _Token;
        private string _Channel;

        public abstract bool IsEmpty
        {
            get;
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Token
        {
            get
            {
                return _Token;
            }

            set
            {
                _Token = value;
                OnPropertyChanged("Token");

            }
        }

        public string Channel
        {
            get
            {
                return _Channel;
            }

            set
            {
                _Channel = value;
                OnPropertyChanged("Channel");

            }
        }

        public ChatSettings(string accName, string accToken, string joinChannel)
        {
            this.Name = accName;
            this.Token = accToken;
            this.Channel = joinChannel;
        }


        public ChatSettings() { }
    }

    public class IrcChatSettings : ChatSettings
    {
        string ircUrl = "irc.chat.twitch.tv";
        int? ircPort = 6667;

        public override bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Channel) || string.IsNullOrEmpty(IrcUrl); }
        }

        public string IrcUrl
        {
            get
            {
                return ircUrl;
            }

            set
            {
                ircUrl = value;
            }
        }

        public int? IrcPort
        {
            get
            {
                return ircPort;
            }

            set
            {
                ircPort = value;
            }
        }

        public IrcChatSettings(string accName, string accToken, string joinChannel, string ircUrl, int? ircPort = null) : base(accName, accToken, joinChannel)
        {
            this.IrcUrl = ircUrl;
            this.IrcPort = ircPort;
        }
        public IrcChatSettings() : base() { }
    }

    public class PubSubSettings : ChatSettings
    {
        string uri = "wss://pubsub-edge.twitch.tv";

        public override bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Channel) || string.IsNullOrEmpty(Uri); }
        }

        public string Uri { get => uri; set => uri = value; }

        public PubSubSettings(string accName, string accToken, string joinChannel, string uri) : base(accName, accToken, joinChannel)
        {
            this.Uri = uri;
        }

        public PubSubSettings() : base() { }
    }
}
