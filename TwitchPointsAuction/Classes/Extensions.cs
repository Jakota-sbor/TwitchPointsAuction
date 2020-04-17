using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TwitchPointsAuction.Classes
{
    public static class Extensions
    {
        public static string GetDescription(this Enum en)
        {
            return en.GetType()
                       .GetMember(en.ToString())
                       .First()
                       .GetCustomAttribute<DescriptionAttribute>()?
                       .Description ?? string.Empty;
        }
    }
}
