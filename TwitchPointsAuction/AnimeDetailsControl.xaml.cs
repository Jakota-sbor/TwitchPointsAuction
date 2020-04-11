using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Логика взаимодействия для AnimeDetailsControl.xaml
    /// </summary>
    public partial class AnimeDetailsControl : UserControl
    {
        int Delay = 0;
        bool IsDelay = false;

        public static readonly DependencyProperty AnimeContentProperty = DependencyProperty.Register("AnimeContent", typeof(AnimeData), typeof(AnimeDetailsControl), new PropertyMetadata(new AnimeData()));
        public static readonly DependencyProperty IsShowPosterProperty = DependencyProperty.Register("IsShowPoster", typeof(bool), typeof(AnimeDetailsControl), new PropertyMetadata(false));

        public AnimeData AnimeContent
        {
            get { return (AnimeData)GetValue(AnimeContentProperty); }
            set { SetValue(AnimeContentProperty, value); }
        }

        public bool IsShowPoster
        {
            get { return (bool)GetValue(IsShowPosterProperty); }
            set { SetValue(IsShowPosterProperty, value); }
        }

        public AnimeDetailsControl()
        {
            InitializeComponent();
        }

        public void Scroll()
        {
            if ((scroll_Description.VerticalOffset == scroll_Description.ScrollableHeight || scroll_Description.VerticalOffset == 0) && !IsDelay)
            {
                IsDelay = true;
                Delay = 0;
            }
            else if ((scroll_Description.VerticalOffset == scroll_Description.ScrollableHeight || scroll_Description.VerticalOffset == 0) && IsDelay && Delay < 10)
                Delay++;
            else if ((scroll_Description.VerticalOffset == scroll_Description.ScrollableHeight || scroll_Description.VerticalOffset == 0) && IsDelay && Delay >= 10)
            {
                IsDelay = false;
                Delay = 0;
                if (scroll_Description.VerticalOffset == scroll_Description.ScrollableHeight)
                    scroll_Description.ScrollToTop();
                else
                    scroll_Description.ScrollToVerticalOffset(scroll_Description.VerticalOffset + 5);
            }
            else
                scroll_Description.ScrollToVerticalOffset(scroll_Description.VerticalOffset + 5);
        }
    }
}
