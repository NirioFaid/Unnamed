using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;

namespace Unnamed
{
    public partial class Storage : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        MainWindow mainwindow = null;
        [Magic]
        public ObservableCollection<Item> CharItems { get; set; } = new ObservableCollection<Item> { };
        public Storage(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
            namesList.ItemsSource = mainwindow.SelectedGroup.Crew;
            storageItems.ItemsSource = mainwindow.SelectedGroup.Storage;
            memItems.ItemsSource = CharItems;
        }

        private void giveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = storageItems.SelectedItem as Item;
            var crewMember = namesList.SelectedItem as Unit;
            if (item != null && storageItems.SelectedItem != null && namesList.SelectedItem != null)
            {
                mainwindow.SelectedGroup.Crew.Where(x => x == crewMember).FirstOrDefault().Inventory.Add(item);
                mainwindow.SelectedGroup.Storage.Remove(mainwindow.SelectedGroup.Storage.Where(x => x == item).FirstOrDefault());
                storageItems.Items.Refresh();
                memItems.Items.Refresh();
            }
        }

        private async void sellButton_Click(object sender, RoutedEventArgs e)
        {
            var item = storageItems.SelectedItem as Item;
            var seller = namesList.SelectedItem as Unit;
            if (item != null && seller != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync($"Sell {item.Name}", $"Sell it for {(int)(item.TotalCost * seller.Stat("CHR") / 100.0)} coins?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    mainwindow.SelectedGroup.Coins += (int)(item.TotalCost * seller.Stat("CHR") / 100.0);
                    mainwindow.SelectedGroup.Storage.Remove(mainwindow.SelectedGroup.Storage.Where(x => x == item).FirstOrDefault());
                    storageItems.Items.Refresh();
                }
            }
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            var item = storageItems.SelectedItem as Item;
            if (storageItems.SelectedItem != null)
            {
                string result = this.ShowModalInputExternal("Confirm", $"Are you sure about that? Type \"yes\" if so.");
                if (result != null && result.ToLower() == "yes")
                {
                    mainwindow.SelectedGroup.Storage.Remove(mainwindow.SelectedGroup.Storage.Where(x => x == item).FirstOrDefault());
                    storageItems.Items.Refresh();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = namesList.SelectedItem as Unit;
            var item = memItems.SelectedItem as Item;
            if (crewMember != null && memItems.SelectedItem != null)
            {
                mainwindow.SelectedGroup.Storage.Add(item);
                mainwindow.SelectedGroup.Crew.Where(x => x == crewMember).FirstOrDefault().Inventory.Remove(item);
                memItems.Items.Refresh();
                storageItems.Items.Refresh();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var crewMember = namesList.SelectedItem as Unit;
            var item = memItems.SelectedItem as Item;
            if (crewMember != null && memItems.SelectedItem != null)
            {
                AddOrChangeItem form = new AddOrChangeItem(mainwindow, item, crewMember);
                form.ShowDialog();
            }
        }

        private void namesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var crewMember = namesList.SelectedItem as Unit;
            var item = storageItems.SelectedItem as Item;
            if (crewMember != null)
            {
                CharItems = crewMember.Inventory;
                memItems.ItemsSource = CharItems;
                if (item != null)
                {
                    sellButton.Content = $"SELL (+{(int)(item.TotalCost * crewMember.Stat("CHR") / 100.0)} coins)";
                } else
                {
                    sellButton.Content = $"SELL";
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            memItems.ItemsSource = CharItems;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindow.save();
        }

        private void storageItems_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var crewMember = namesList.SelectedItem as Unit;
            var selectedItem = storageItems.SelectedItem as Item;
            if (crewMember != null && selectedItem != null)
            {
                sellButton.Content = $"SELL (+{(int)(selectedItem.TotalCost * crewMember.Stat("CHR") / 100.0)} coins)";
            } else {
                sellButton.Content = $"SELL";
            }
        }
    }
}
