using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Unnamed
{
    public partial class Shop : MetroWindow
    {
        List<Item> warShop = new List<Item> {
            Item.Find("Knuckles"),
            Item.Find("Steel Gauntlet"),
            Item.Find("Metal Claws"),
            Item.Find("Sword"),
            Item.Find("War Axe"),
            Item.Find("Mace"),
            Item.Find("Greatsword"),
            Item.Find("Battleaxe"),
            Item.Find("Warhammer"),
            Item.Find("Katana"),
            //Item.Find("Combat Oil Umbrella"),
        };

        List<Item> dexShop = new List<Item> {
            Item.Find("Dagger"),
            Item.Find("Sickle"),
            Item.Find("Short Bow"),
            Item.Find("Long Bow"),
            Item.Find("Hand Crossbow"),
            Item.Find("Crossbow"),
            Item.Find("Arrow"),
            Item.Find("Bolt(Small)"),
            Item.Find("Bolt(Medium)"),
            Item.Find("Bladed Boomerang"),
            Item.Find("Tessen"),
            Item.Find("Shuriken"),
            Item.Find("Knife"),
            Item.Find("Kunai"),
        };

        List<Item> magicShop = new List<Item> {
            Item.Find("Green wear"),
            Item.Find("Robe of innocence"),
            Item.Find("Novice Robe"),
            Item.Find("Apprentice Robe"),
            Item.Find("Adept Robe"),
            Item.Find("Expert Robe"),
            Item.Find("Master Robe"),
        };

        List<Item> alchemistsShop = new List<Item> {
            Item.Find("Healing Potion"),
            Item.Find("Mana Potion"),
            Item.Find("Will Potion"),
            Item.Find("Freeze Bomb"),
            Item.Find("Charming Bomb"),
            Item.Find("Alchemist's Fire"),
            Item.Find("Storm bomb"),
            Item.Find("Rock Bomb"),
            Item.Find("Poison Bomb"),
            Item.Find("Void Bomb"),
        };

        List<Item> armorShop = new List<Item> {
            Item.Find("Padded Armor"),
            Item.Find("Leather Armor"),
            Item.Find("Hide Armor"),
            Item.Find("Studded Leather Armor"),
            Item.Find("Chain Shirt"),
            Item.Find("Breastplate"),
            Item.Find("Scale Mail"),
            //Item.Find("Spiked Armor"),
            Item.Find("Half Plate"),
            Item.Find("Chain Mail"),
            Item.Find("Plate Armor"),
            Item.Find("Shield")
        };

        List<Item> travelShop = new List<Item> {
            Item.Find("Small Backpack"),
            Item.Find("Medium Backpack"),
            Item.Find("Large Backpack"),
            Item.Find("Spacious Backpack"),
            Item.Find("Advanced Backpack"),
            Item.Find("Camping Supplies"),
        };

        List<Item> blackMarket = new List<Item> {
            Item.Find("Sacrificial Dagger"),
            Item.Find("Chillrend"),
            Item.Find("Benizakura"),
            Item.Find("Dawnbreaker"),
            Item.Find("Healing stone"),
        };

        List<Item> activeList = new List<Item> { };

        MainWindow mainwindow = null;
        Unit Client = null;
        public Shop(MainWindow mw, Unit client)
        {
            InitializeComponent();
            mainwindow = mw;
            Client = client;
            itemsList.ItemsSource = activeList;
            charItemsList.ItemsSource = Client.Inventory;
            unitsCounter.Content = $"COINS: {Client.Coins}";
        }

        private void buyButton_Click(object sender, RoutedEventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null)
            {
                if (Client.Coins - ((int)(item.TotalCost * (3.0 - Client.Stat("CHR") / 10.0)) * (int)itemMult.Value) >= 0)
                {
                    for (int i = 0; i < itemMult.Value; i++) Client.Inventory.Add(item.Copy());
                    Client.Coins -= (int)(item.TotalCost * (3.0 - Client.Stat("CHR") / 10.0)) * (int)itemMult.Value;
                    charItemsList.Items.Refresh(); Client.InventoryChanged();
                    unitsCounter.Content = $"COINS: {Client.Coins}";
                }
                else
                {
                    this.ShowMessageAsync("Em...", "Not enough coins for that!");
                }
            }
        }

        private void leaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void shopsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = (shopsList.SelectedItem as ListBoxItem).Content.ToString();
            switch (item)
            {
                case "Warrior Shop": activeList.Clear(); foreach (Item i in warShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Armor Shop": activeList.Clear(); foreach (Item i in armorShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Assassin Shop": activeList.Clear(); foreach (Item i in dexShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Magic Shop": activeList.Clear(); foreach (Item i in magicShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Alchemist's Shop": activeList.Clear(); foreach (Item i in alchemistsShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Traveler Shop": activeList.Clear(); foreach (Item i in travelShop) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                case "Black Market": activeList.Clear(); foreach (Item i in blackMarket) { activeList.Add(i); } itemsList.Items.Refresh(); break;
                default: break;
            }
        }

        private void itemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null) { buyButton.IsEnabled = true; buyButton.Content = $"BUY (-{(int)(item.TotalCost * (3.0 - Client.Stat("CHR") / 10.0)) * (int)itemMult.Value} coins)"; }
            else { buyButton.IsEnabled = false; buyButton.Content = "BUY"; }
        }

        private void itemMult_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null) { buyButton.Content = $"BUY (-{(int)(item.TotalCost * (3.0 - Client.Stat("CHR") / 10.0)) * (int)itemMult.Value} coins)"; }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindow.save();
        }

        private void sellButton_Click(object sender, RoutedEventArgs e)
        {
            var item = charItemsList.SelectedItem as Item;
            if (item != null)
            {
                Client.Inventory.Remove(item);
                Client.Coins += (int)(item.TotalCost * Client.Stat("CHR") / 20.0);
                unitsCounter.Content = $"COINS: {Client.Coins}";
                sellButton.Content = $"SELL"; Client.InventoryChanged();
                charItemsList.Items.Refresh();
            }
        }

        private void charItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var item = charItemsList.SelectedItem as Item;
            if (item != null) { sellButton.Content = $"SELL (+{(int)(item.TotalCost * Client.Stat("CHR") / 20.0)} coins)"; }

        }
    }
}
