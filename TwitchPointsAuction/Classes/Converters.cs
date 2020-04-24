using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TwitchPointsAuction.Models;

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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var collectionType = (CollectionType)parameter;
            switch (collectionType)
            {
                case CollectionType.Genres:
                    return Settings.Instance.AuctionRules.ForbiddenGenres.Any(x => x == (Genres)value);
                case CollectionType.Kinds:
                    return Settings.Instance.AuctionRules.ForbiddenKinds.Any(x => x == (Kind)value);
                case CollectionType.Titles:
                    return Settings.Instance.AuctionRules.ForbiddenTitles.Any(x => x == (string)value);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
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
                return TimeSpan.TryParseExact((string)value, @"mm\:ss", null, out var timespan) ? timespan : TimeSpan.Zero;
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

    public class RandomBrushConverter : IValueConverter
    {
        private List<GradientStopCollection> gradStopCollectionList = new List<GradientStopCollection>()
        {
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFFF9600"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFAA5000"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF87E100"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF326400"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF00CCF9"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF0059C5"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFE6007D"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF8B0044"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFE00000"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFA21600"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF00F9D7"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF00A7C1"),1)
            },
            new GradientStopCollection() {
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFD8D8D8"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF7E7E7E"),1)
            }
        };

        private Random rnd = new Random();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new LinearGradientBrush(gradStopCollectionList[rnd.Next(0, 7)], new Point(0, 0), new Point(0, 1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class RulesToStringMultivalueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() == 6)
            {
                var text = (string)values[0];
                var yearFrom = (int?)values[1];
                var yearTo = (int?)values[2];
                var genres = (NotifyCollection<Genres>)values[3];
                var kinds = (NotifyCollection<Kind>)values[4];
                var titles = (NotifyCollection<string>)values[5];

                string years = "";
                if (yearFrom.HasValue && yearTo.HasValue)
                    years = $"{yearFrom.Value} - {yearTo.Value}";
                else if (yearFrom.HasValue)
                    years = yearFrom.Value.ToString();
                else if (yearTo.HasValue)
                    years = yearTo.Value.ToString();
                if (!string.IsNullOrEmpty(text))
                    return text.Replace("(years)", years).Replace("(genres)", genres.ToString()).Replace("(kinds)", kinds.ToString()).Replace("(titles)", titles.ToString());
                else
                    return DependencyProperty.UnsetValue;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /*
    class NotifyCollectionConverter<T> : System.Text.Json.Serialization.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(NotifyCollection<T>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            PagedResult<T> result = (PagedResult<T>)value;
            JObject jo = new JObject();
            jo.Add("PageSize", result.PageSize);
            jo.Add("PageIndex", result.PageIndex);
            jo.Add("TotalItems", result.TotalItems);
            jo.Add("TotalPages", result.TotalPages);
            jo.Add("Items", JArray.FromObject(result.ToArray(), serializer));
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            PagedResult<T> result = new PagedResult<T>();
            result.PageSize = (int)jo["PageSize"];
            result.PageIndex = (int)jo["PageIndex"];
            result.TotalItems = (int)jo["TotalItems"];
            result.TotalPages = (int)jo["TotalPages"];
            result.AddRange(jo["Items"].ToObject<T[]>(serializer));
            return result;
        }
    }
    */
}
