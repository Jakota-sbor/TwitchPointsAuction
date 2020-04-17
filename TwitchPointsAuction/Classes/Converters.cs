using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TwitchPointsAuction.Classes
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var aucState = (AuctionState)value;
            return aucState == AuctionState.On;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolvalue = (bool)value;
            return boolvalue ? AuctionState.On : AuctionState.Off;
        }
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            return ((Enum)value).GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ListEnumToBoolConverter : IValueConverter
    {
        Genres genre;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is NotifyCollection<Genres> && value is Genres)
            {
                var collection = (NotifyCollection<Genres>)parameter;
                genre = (Genres)value;
                return collection.Any(x=> x == genre);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                var collection = (NotifyCollection<Genres>)parameter;
                var boolvalue = (bool)value;
                if (boolvalue)
                    collection.Add(genre);
                else
                    collection.Remove(genre);
            }
            return DependencyProperty.UnsetValue;
        }
    }

    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty((string)value))
                return int.TryParse((string)value, out var val) ? val : 0;
            else return null;
        }
    }

    public class GenresToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[])
                return string.Join(", ", (string[])value);
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class StringToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
                return ((TimeSpan)value).ToString(@"mm\:ss");
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                return TimeSpan.TryParseExact((string)value, @"mm\:ss", null, out var timespan) ? timespan : TimeSpan.FromMinutes(10);
            return DependencyProperty.UnsetValue;
        }
    }

    public class StringToTitlesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotifyDictionary<string, bool>)
            {
                var titles = (NotifyDictionary<string, bool>)value;
                return string.Join(Environment.NewLine, titles.Select(x => x.Key));
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var titlesdict = new NotifyDictionary<string, bool>();
                var titles = ((string)value).Split(Environment.NewLine).ToList();
                foreach (var item in titles)
                    titlesdict.Add(item, true);
                return titlesdict;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
                return ((SolidColorBrush)value).Color.ToString();
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)value));
            return DependencyProperty.UnsetValue;
        }
    }
}
