using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class MainwindowViewModel : INotifyPropertyChanged
    {
        //PubSub pubsub;
        public MainwindowViewModel()
        {
            //pubsub = new PubSub();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private RelayCommand toggleChatCommand;
        public RelayCommand ToggleChatCommand => toggleChatCommand ??
                    (toggleChatCommand = new RelayCommand(param =>
                    {
                        var isChecked = (bool)param;
                        if (isChecked)
                        {
                            Task.Run(async () =>
                            {
                                await IrcChat.Instance?.Connect();
                                await IrcChat.Instance?.JoinChannel();
                            });
                        }
                        else
                        {
                            Task.Run(async () =>
                            {
                                await IrcChat.Instance?.LeaveChannel();
                                await IrcChat.Instance?.Disconnect();
                            });
                        }
                    }));

        private RelayCommand togglePubSubCommand;
        public RelayCommand TogglePubSubCommand => togglePubSubCommand ??
                    (togglePubSubCommand = new RelayCommand(param =>
                    {
                        var isChecked = (bool)param;
                        if (isChecked)
                        {
                            Task.Run(async () =>
                            {
                                await PubSub.Instance.Connect();
                                await PubSub.Instance.JoinChannel();
                                //await pubsub.Connect();
                                //await pubsub.JoinChannel();
                                //pubsub.OnReward += Pubsub_OnReward;
                            });
                        }
                        else
                        {
                            Task.Run(async () =>
                            {
                                await PubSub.Instance.LeaveChannel();
                                await PubSub.Instance.Disconnect();
                            });
                        }
                    }));

        private void Pubsub_OnReward(object sender, Reward newReward)
        {
            Debug.WriteLine("Reward!");
        }
    }
}
