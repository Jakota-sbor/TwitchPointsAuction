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
using TwitchPointsAuction.Classes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Логика взаимодействия для AuctionLotControl.xaml
    /// </summary>
    public partial class AuctionLotControl : UserControl
    {
        public static readonly DependencyProperty LotContentProperty = DependencyProperty.Register("LotContent", typeof(LotData), typeof(AuctionLotControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsShowPosterProperty = DependencyProperty.Register("IsShowPoster", typeof(bool), typeof(AuctionLotControl), new PropertyMetadata(false));
        public static readonly DependencyProperty TitlePositionProperty = DependencyProperty.Register("TitlePosition", typeof(Dock), typeof(AuctionLotControl), new PropertyMetadata(Dock.Top));

        public LotData LotContent
        {
            get { return (LotData)GetValue(LotContentProperty); }
            set { SetValue(LotContentProperty, value); }
        }

        public bool IsShowPoster
        {
            get { return (bool)GetValue(IsShowPosterProperty); }
            set { SetValue(IsShowPosterProperty, value); }
        }

        public Dock TitlePosition
        {
            get { return (Dock)GetValue(TitlePositionProperty); }
            set { SetValue(TitlePositionProperty, value); System.Diagnostics.Trace.WriteLine("DOCK CHANGED"); }
        }

        public AuctionLotControl()
        {
            InitializeComponent();
        }
        /*
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
        */
    }
}
