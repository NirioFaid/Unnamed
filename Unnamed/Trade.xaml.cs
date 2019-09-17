using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Unnamed
{
    public partial class Trade : MetroWindow
    {
        MainWindow mainwindow = null;
        public ObservableCollection<Unit> TradeList1 { get; set; } = new ObservableCollection<Unit> { };
        public ObservableCollection<Unit> TradeList2 { get; set; } = new ObservableCollection<Unit> { };
        public Unit Selected1 { get; set; } = null;
        public Unit Selected2 { get; set; } = null;
        public List<Item> GiveItemList1 { get; set; } = new List<Item> { };
        public List<Item> GiveItemList2 { get; set; } = new List<Item> { };
        public int BP { get; set; } = 0;
        Random r = new Random();

        public Trade(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
            traderList1.ItemsSource = TradeList1;
            traderList2.ItemsSource = TradeList2;
            giveItemsList1.ItemsSource = GiveItemList1;
            giveItemsList2.ItemsSource = GiveItemList2;
            FillStatThrow();
            Update();
        }

        public Trade(MainWindow mw, ObservableCollection<Unit> tl1, ObservableCollection<Unit> tl2)
        {
            InitializeComponent();
            mainwindow = mw;
            TradeList1 = tl1;
            TradeList2 = tl2;
            traderList1.ItemsSource = TradeList1;
            traderList2.ItemsSource = TradeList2;
            giveItemsList1.ItemsSource = GiveItemList1;
            giveItemsList2.ItemsSource = GiveItemList2;
            if (tl1.Count() > 0) traderList1.SelectedItem = TradeList1[0];
            if (tl2.Count() > 0) traderList2.SelectedItem = TradeList2[0];
            FillStatThrow();
            Update();
        }

        void FillStatThrow()
        {
            statThrow1.Items.Clear(); statThrow2.Items.Clear();
            List<StatData> all = (new Unit(true)).AllStats();
            foreach (StatData a in all) { statThrow1.Items.Add(a.Name); statThrow2.Items.Add(a.Name); }
        }

        void Update()
        {
            if (Selected1 != null)
            {
                itemsList1.ItemsSource = Selected1.Inventory;
            }
            if (Selected2 != null)
            {
                itemsList2.ItemsSource = Selected2.Inventory;
            }
            DataContext = new
            {
                TradeList1,
                TradeList2,
                Selected1,
                Selected2,
                BP
            };
            traderList1.Items.Refresh();
            traderList2.Items.Refresh();
            itemsList1.Items.Refresh();
            itemsList2.Items.Refresh();
            giveItemsList1.Items.Refresh();
            giveItemsList2.Items.Refresh();
        }

        private void traderList1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var trader1 = traderList1.SelectedItem as Unit;
            if (trader1 != null)
            {
                Selected1 = trader1;
                itemsList1.ItemsSource = Selected1.Inventory;
                givenCoins1.HideUpDownButtons = false;
                statThrow1.IsEnabled = true;
                Update();
            }
            else givenCoins1.HideUpDownButtons = true;
        }

        private void traderList2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var trader2 = traderList2.SelectedItem as Unit;
            if (trader2 != null)
            {
                Selected2 = trader2;
                itemsList2.ItemsSource = Selected2.Inventory;
                givenCoins2.HideUpDownButtons = false;
                statThrow2.IsEnabled = true;
                Update();
            }
            else givenCoins2.HideUpDownButtons = true;
        }

        private void addGuildButton1_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add guildian", $"Type his name");
            if (result != null && mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                TradeList1.Add(mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void addWorldButton1_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add someone from world", $"Type his name");
            if (result != null && mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                TradeList1.Add(mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void deleteButton1_Click(object sender, RoutedEventArgs e)
        {
            var trader = traderList1.SelectedItem as Unit;
            if (trader != null)
            {
                TradeList1.Remove(trader);
                Selected1 = null;
                itemsList1.ItemsSource = null;
                Update();
            }
        }

        private void move(Unit u, bool IsFirst)
        {
            if (IsFirst)
            {
                TradeList1.Remove(u);
                TradeList2.Add(u);
                if (u == Selected1)
                {
                    Selected1 = null;
                    traderList1.SelectedItem = null;
                    itemsList1.ItemsSource = null;
                }
            }
            else
            {
                TradeList2.Remove(u);
                TradeList1.Add(u);
                if (u == Selected2)
                {
                    Selected2 = null;
                    traderList2.SelectedItem = null;
                    itemsList2.ItemsSource = null;
                }
            }
            Update();
        }

        private void moveButton1_Click(object sender, RoutedEventArgs e)
        {
            move(Selected1, true);
        }

        private void addGuildButton2_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add guildian", $"Type his name");
            if (result != null && mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                TradeList2.Add(mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void addWorldButton2_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add someone from world", $"Type his name");
            if (result != null && mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                TradeList2.Add(mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void deleteButton2_Click(object sender, RoutedEventArgs e)
        {
            var trader = traderList2.SelectedItem as Unit;
            if (trader != null)
            {
                TradeList2.Remove(trader);
                Selected2 = null;
                itemsList2.ItemsSource = null;
                Update();
            }
        }

        private void moveButton2_Click(object sender, RoutedEventArgs e)
        {
            move(Selected2, false);
        }

        private void statThrow1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statThrow1.Text != null)
                statThrowButton1.IsEnabled = true;
        }

        private void statThrow2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statThrow2.Text != null)
                statThrowButton2.IsEnabled = true;
        }

        private void statThrowButton1_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            int value = r.Next(0, 100);
            statThrowResultBox1.Text = $"{Selected1.Stat(statThrow1.Text)}/{value + 1}";
            if (Selected1.Stat(statThrow1.Text) > value) statThrowResultBox1.Foreground = new SolidColorBrush(Colors.Lime);
            else statThrowResultBox1.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void statThrowButton2_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            int value = r.Next(0, 100);
            statThrowResultBox2.Text = $"{Selected2.Stat(statThrow2.Text)}/{value + 1}";
            if (Selected2.Stat(statThrow2.Text) > value) statThrowResultBox2.Foreground = new SolidColorBrush(Colors.Lime);
            else statThrowResultBox2.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void giveButton1_Click(object sender, RoutedEventArgs e)
        {
            var item = itemsList1.SelectedItem as Item;
            if (item != null)
            {
                GiveItemList1.Add(item);
                Update();
            }
        }

        private void giveButton2_Click(object sender, RoutedEventArgs e)
        {
            var item = itemsList2.SelectedItem as Item;
            if (item != null)
            {
                GiveItemList2.Add(item);
                Update();
            }
        }

        private void removeButton2_Click(object sender, RoutedEventArgs e)
        {
            var item = giveItemsList2.SelectedItem as Item;
            if (item != null)
            {
                GiveItemList2.Remove(item);
                Update();
            }
        }

        private void removeButton1_Click(object sender, RoutedEventArgs e)
        {
            var item = giveItemsList1.SelectedItem as Item;
            if (item != null)
            {
                GiveItemList1.Remove(item);
                Update();
            }
        }

        private void tradeButton_Click(object sender, RoutedEventArgs e)
        {
            Selected1.Coins += (int)givenCoins2.Value;
            Selected2.Coins += (int)givenCoins1.Value;
            Selected1.Coins -= (int)givenCoins1.Value;
            Selected2.Coins -= (int)givenCoins2.Value;

            foreach (Item i in GiveItemList1) { Selected2.Inventory.Add(i); Selected1.Inventory.Remove(i); }
            foreach (Item i in GiveItemList2) { Selected1.Inventory.Add(i); Selected2.Inventory.Remove(i); }
            Selected1.InventoryChanged(); Selected2.InventoryChanged();

            cancel();
        }

        void cancel()
        {
            GiveItemList1.Clear();
            GiveItemList2.Clear();
            givenCoins1.Value = 0;
            givenCoins2.Value = 0;
            Update();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancel();
        }

        private void leaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void giveAllButton1_Click(object sender, RoutedEventArgs e)
        {
            GiveItemList1.Clear();
            if (Selected1 != null) { GiveItemList1.AddRange(Selected1.Inventory); givenCoins1.Value = Selected1.Coins; }
            Update();
        }

        private void giveAllButton2_Click(object sender, RoutedEventArgs e)
        {
            GiveItemList2.Clear();
            if (Selected2 != null) { GiveItemList2.AddRange(Selected2.Inventory); givenCoins2.Value = Selected2.Coins; }
            Update();
        }
    }
}
