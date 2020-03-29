using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchPointsAuction.Models
{
    public class Reward
    {
        string _ID;
        string _Title;
        string _User;
        string _UserInput;
        uint _Cost;
        bool _UserInputRequired;

        public string ID { get => _ID; private set => _ID = value; }
        public string Title { get => _Title; private set => _Title = value; }
        public string User { get => _User; private set => _User = value; }
        public string UserInput { get => _UserInput; private set => _UserInput = value; }
        public uint Cost { get => _Cost; private set => _Cost = value; }
        public bool UserInputRequired { get => _UserInputRequired; private set => _UserInputRequired = value; }

        public Reward(string id,string title,string user,string userInput,uint cost,bool userinputrequired)
        {
            ID = id;
            Title = title;
            User = user;
            UserInput = userInput;
            Cost = cost;
            UserInputRequired = userinputrequired;
        }

        public override string ToString()
        {
            return $"ID: {ID}\nTitle: {Title}\nUser: {User}\nUserInput: {UserInput}\nCost: {Cost}\nInputRequired: {UserInputRequired}";
        }
    }
}
