﻿using MahApps.Metro.Controls;
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
            new Item("Knuckles", 25, 2, "STR\nBrawl\ncSP -10\ntHP -5\ntSP -5"),
            new Item("Steel Gauntlet", 125, 5, "STR\nBrawl\ncSP -12\ntHP -6\ntSP -6\nSteel"),
            new Item("Metal Claws", 125, 4, "STR\nBrawl\ncSP -12\ntHP -12\nSteel"),
            new Item("Sword", 70, 10, "STR\nLongBlade\ncSP -10\ntHP -10"),
            new Item("War Axe", 80, 12, "STR\nAxe\ncSP -12\ntHP -12"),
            new Item("Mace", 90, 14, "STR\nBluntWeapon\ncSP -14\ntHP -14"),
            new Item("Greatsword", 140, 17, "STR\nLongBlade\ncSP -17\ntHP -17"),
            new Item("Battleaxe", 160, 21, "STR\nAxe\ncSP -21\ntHP -21"),
            new Item("Warhammer", 180, 25, "STR\nBluntWeapon\ncSP -25\ntHP -25"),
            new Item("Katana", 2800, 7, "STR\nLongBlade\ncSP -11\ntHP -11"),
            new Item("Combat Oil Umbrella", 125, 6, "STR\nBluntWeapon\ncSP -10\ntHP -3\ntSP -7\nNormal\n[Block] chance"),
        };

        List<Item> dexShop = new List<Item> {
            new Item("Dagger", 25, 2.5, "DEX\nShortBlade\ncSP -5\ntHP -5"),
            new Item("Sickle", 42, 3, "DEX\nShortBlade\ncSP -8\ntHP -6\ntSP -1\ntSP -1"),
            new Item("Short Bow", 70, 9, "DEX\nAccuracy\ncSP -10\ntHP -10\nUses Arrow"),
            new Item("Long Bow", 80, 11, "DEX\nAccuracy\ncSP -12\ntHP -12\nUses Arrow"),
            new Item("Hand Crossbow", 60, 7, "DEX\nAccuracy\ncSP -8\ntHP -8\nUses Bolt(Small)"),
            new Item("Crossbow", 90, 14, "DEX\nAccuracy\ncSP -14\ntHP -14\nUses Bolt(Medium)"),
            new Item("Arrow", 1, 0.025, 20, "DEX\nShortBlade\ncSP -4\ntHP -4"),
            new Item("Bolt(Small)", 1, 0.23, 20, "DEX\nShortBlade\ncSP -3\ntHP -3"),
            new Item("Bolt(Medium)", 1, 0.45, 20, "DEX\nShortBlade\ncSP -5\ntHP -5"),
            new Item("Boomerang", 30, 0.5, "DEX\nThrowing\ncSP -8\ntSP -4\ntHP -4"),
            new Item("Bladed Boomerang", 60, 1, "DEX\nThrowing\ncSP -8\ntHP -8"),
            new Item("Tessen", 25, 3, "DEX\nShortBlade\ncSP -8\ntHP -8\n[Block] chance"),
            new Item("Shuriken", 1, 0.1, 10, "DEX\nThrowing\ncSP -1\ntSP -3\ntHP -3\nConsumable"),
            new Item("Kunai", 1, 0.2, 10, "DEX\nThrowing\ncSP -4\ntHP -4\nConsumable"),
            new Item("Throwing Knife", 1, 1, 4, "DEX\nThrowing\ncSP -5\ntHP -5\nConsumable"),
            new Item("Needle", 1, 0.05, 10, "DEX\nThrowing\ncSP -1\ntHP -1\nConsumable"),
            new Item("Dart", 1, 0.1, 10, "DEX\nThrowing\ncSP -2\ntHP -2\nConsumable"),
            new Item("Poison Dart", 2, 0.1, 10, "DEX\nThrowing\ncSP -6\ntHP -2\ntSP -4\nConsumable")
        };

        List<Item> magicShop = new List<Item> {
            new Item("Green wear", 75, 1, "SPR +3\nMPR +3"),
            new Item("Robe of innocence", 125, 1, "MPR +6\nWPR +4"),
            new Item("Novice Robe", 125, 2, "MA +6\nMPR +3"),
            new Item("Apprentice Robe", 350, 2, "MA +10\nMPR +5"),
            new Item("Adept Robe", 725, 2, "MA +14\nMPR +7"),
            new Item("Expert Robe", 2480, 2, "MA +20\nMPR +10"),
            new Item("Master Robe", 4275, 2, "MA +30\nMPR +15"),
        };

        List<Item> alchemistsShop = new List<Item> {
            new Item("Healing Potion", 25, 0.5, "END\nAlchemy\ncSP -10\ncHP +8\nConsumable"),
            new Item("Mana Potion", 25, 0.5, "INT\nAlchemy\ncSP -10\ncMP +8\nConsumable"),
            new Item("Will Potion", 25, 0.5, "WIS\nAlchemy\ncSP -10\ncWP +8\nConsumable"),
            new Item("Flashbang", 45, 0.5, "DEX\nThrowing\ncSP -12\ncaSP -2\ntaSP -6\nLight\nConsumable"),
            new Item("Freeze Bomb", 55, 1, "DEX\nThrowing\ncSP -20\ntaSP -6\nIce\nConsumable"),
            new Item("Charming Bomb", 135, 1, "DEX\nThrowing\ncSP -16\ncWP -2\ntaWP -6\nPsy\nConsumable"),
            new Item("Alchemist's Fire", 55, 1, "DEX\nThrowing\ncSP -16\ntHP -7\ntaHP -3\nFire\nConsumable"),
            new Item("Storm bomb", 55, 1, "DEX\nThrowing\ncSP -15\ntHP -6\ntaMP -3\nStorm\nConsumable"),
            new Item("Rock Bomb", 55, 1, "DEX\nThrowing\ncSP -16\ntHP -7\ntaHP -3\nRock\nConsumable"),
            new Item("Poison Bomb", 55, 1, "DEX\nThrowing\ncSP -20\ntaHP -3\ntaSP -3\nPoison\nConsumable"),
            new Item("Void Bomb", 55, 1, "DEX\nThrowing\ncSP -18\ncHP -2\ntHP -6\ntaHP -4\nVoid\nConsumable"),
        };

        List<Item> armorShop = new List<Item> {
            new Item("Padded Armor", 45, 3, "PA +4"),
            new Item("Leather Armor", 75, 5, "PA +8"),
            new Item("Hide Armor", 75, 7, "PA +12"),
            new Item("Studded Leather Armor", 338, 10, "PA +18"),
            new Item("Chain Shirt", 375, 12, "PA +24"),
            new Item("Breastplate", 1500, 12, "PA +28"),
            new Item("Scale Mail", 715, 16, "PA +36"),
            new Item("Spiked Armor", 1040, 16, "END\nMelee\ncSP -10\ntHP -5\ntSP -5\nPA +36"),
            new Item("Half Plate", 5625, 20, "PA +50"),
            new Item("Chain Mail", 1040, 25, "PA +56"),
            new Item("Plate Armor", 11250, 29, "PA +64"),
            new Item("Shield", 75, 7, "Chance to [Block] incoming attack")
        };

        List<Item> travelShop = new List<Item> {
            new Item("Small Backpack", 15, 3, "CW +15"),
            new Item("Medium Backpack", 80, 3, "CW +30"),
            new Item("Large Backpack", 100, 3, "CW +45"),
            new Item("Spacious Backpack", 150, 3, "CW +60"),
            new Item("Advanced Backpack", 250, 3, "CW +90"),
            new Item("Camping Supplies", 15, 3, 4, "Need to rest outdoors, consumable"),
        };

        List<Item> blackMarket = new List<Item> {
            new Item("Sacrificial Dagger", 135, 3, "DEX\nShortBlade\ncSP -6\ntHP -3\nDrain\nSteel"),
            new Item("Chillrend", 4600, 7, "STR\nLongBlade\ncSP -10\ntMP -5\ntHP -11\ntSP -4\nIce"),
            new Item("Benizakura", 6666, 8, "STR\nLongBlade\ncSP -10\ntWP -16\ntHP -13\nDrain\nVoid"),
            new Item("Dawnbreaker", 4740, 7, "STR\nLongBlade\ncSP -10\ntWP -5\ntHP -15\nLight"),
            new Item("Healing stone", 4444, 2, "HPR +4"),
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
