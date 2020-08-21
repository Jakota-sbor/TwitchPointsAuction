using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TwitchPointsAuction.Classes;

namespace TwitchPointsAuction.Models
{
    public class Bet : INotifyPropertyChanged
    {
        readonly string _User;
        readonly string _UserInput;
        readonly uint _Cost;
        readonly ApiType _ApiType;
        bool isVisible = true;
        bool isBroken = false;

        public string User => _User;
        public string UserInput => _UserInput;
        public uint Cost => _Cost;
        public ApiType ApiType => _ApiType;
        public bool IsVisible { get => isVisible; set { isVisible = value; OnPropertyChanged(); } }
        public bool IsBroken { get => isBroken; set => isBroken = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public Bet()
        {
            _User = "Тестовый чел";
            _UserInput = "Строка с реварда";
            _Cost = 1000;
            _ApiType = ApiType.Text;
        }

        public Bet(string user, string userinput, uint cost, ApiType api = ApiType.Text)
        {
            _User = user;
            _UserInput = userinput;
            _Cost = cost;
            _ApiType = api;
        }
    }
}
