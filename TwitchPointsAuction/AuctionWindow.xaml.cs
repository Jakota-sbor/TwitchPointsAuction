using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwitchPointsAuction.Models;

namespace TwitchPointsAuction
{
    /// <summary>
    /// Логика взаимодействия для AuctionWindow.xaml
    /// </summary>
    public partial class AuctionWindow : Window
    {
        AuctionViewModel viewModel = new AuctionViewModel();

        public AuctionWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void list_LotListPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is Bet bet)
            {
               
            }
        }

        private void list_BetList_MouseMove(object sender, MouseEventArgs e)
        {
            Point mPos = e.GetPosition(null);

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(mPos.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(mPos.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                try
                {
                    ListBox listbox = (ListBox)sender;
                    ListBoxItem selectedItem = (ListBoxItem)listbox.SelectedItem;

                    listbox.Items.Remove(selectedItem);

                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, selectedItem), DragDropEffects.Move);

                    if (selectedItem.Parent == null)
                    {
                        listbox.Items.Add(selectedItem);
                    }
                }
                catch { }
            }
        }

        private void button_BetDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var bet = button.DataContext as Bet;
                bet.IsVisible = false;
            }
        }
    }
}
