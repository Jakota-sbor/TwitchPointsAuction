using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TwitchPointsAuction.Classes
{
    [ContentProperty("Filters")]
    class FilterExtension : MarkupExtension
    {
        private readonly Collection<IFilter> _filters = new Collection<IFilter>();
        public ICollection<IFilter> Filters { get { return _filters; } }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new FilterEventHandler((s, e) =>
            {
                foreach (var filter in Filters)
                {
                    var res = filter.Filter(e.Item);
                    if (!res)
                    {
                        e.Accepted = false;
                        return;
                    }
                }
                e.Accepted = true;
            });
        }
    }

    public interface IFilter
    {
        bool Filter(object item);
    }

    public class PropertyFilter : DependencyObject, IFilter
    {
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(PropertyFilter), new UIPropertyMetadata(null));
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(PropertyFilter), new UIPropertyMetadata(null));
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty RegexPatternProperty =
            DependencyProperty.Register("RegexPattern", typeof(string), typeof(PropertyFilter), new UIPropertyMetadata(null));
        public string RegexPattern
        {
            get { return (string)GetValue(RegexPatternProperty); }
            set { SetValue(RegexPatternProperty, value); }
        }

        public bool Filter(object item)
        {
            var type = item.GetType();
            var itemValue = type.GetProperty(PropertyName).GetValue(item, null);
            if (RegexPattern == null)
            {
                return (object.Equals(itemValue, Value));
            }
            else
            {
                if (itemValue is string == false)
                    return false;
                else if (string.IsNullOrEmpty((string)itemValue))
                    return true;
                else
                {
                    return Regex.Match((string)itemValue, RegexPattern).Success;
                }
            }
        }
    }
}
