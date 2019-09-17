using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Unnamed
{
    public partial class HireWindow : MetroWindow
    {
        MainWindow mainwindow = null;
        public List<Unit> squad = new List<Unit> { };
        public int totalCost = 0;
        public HireWindow(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
            Title = $"Hire Menu | Coins: {mainwindow.SelectedGroup.Coins}";
            generate();
        }

        public void generate()
        {
            squad.Clear();
            squad.Add(new Unit(true));
            memInfo1.Content = $"{squad[0].Name}, {squad[0].Gender}, {squad[0].Race}, {squad[0].Class}";
            memCost1.Content = $"COST: {squad[0].Cost}";
            try { avatar1.Source = new BitmapImage(new Uri(squad[0].Avatar, UriKind.Absolute)); }
            catch { avatar1.Source = new BitmapImage(new Uri(squad[0].Avatar, UriKind.Relative)); }
            memStrength1.Content = squad[0].Stat("STR");
            memDexterity1.Content = squad[0].Stat("DEX");
            memEndurance1.Content = squad[0].Stat("END");
            memIntelligence1.Content = squad[0].Stat("INT");
            memWisdom1.Content = squad[0].Stat("WIS");
            memCharisma1.Content = squad[0].Stat("CHR");
            Thread.Sleep(228);
            squad.Add(new Unit(true));
            memInfo2.Content = $"{squad[1].Name}, {squad[1].Gender}, {squad[1].Race}, {squad[1].Class}";
            memCost2.Content = $"COST: {squad[1].Cost}";
            try { avatar2.Source = new BitmapImage(new Uri(squad[1].Avatar, UriKind.Absolute)); }
            catch { avatar2.Source = new BitmapImage(new Uri(squad[1].Avatar, UriKind.Relative)); }
            memStrength2.Content = squad[1].Stat("STR");
            memDexterity2.Content = squad[1].Stat("DEX");
            memEndurance2.Content = squad[1].Stat("END");
            memIntelligence2.Content = squad[1].Stat("INT");
            memWisdom2.Content = squad[1].Stat("WIS");
            memCharisma2.Content = squad[1].Stat("CHR");
            Thread.Sleep(228);
            squad.Add(new Unit(true));
            memInfo3.Content = $"{squad[2].Name}, {squad[2].Gender}, {squad[2].Race}, {squad[2].Class}";
            memCost3.Content = $"COST: {squad[2].Cost}";
            try { avatar3.Source = new BitmapImage(new Uri(squad[2].Avatar, UriKind.Absolute)); }
            catch { avatar3.Source = new BitmapImage(new Uri(squad[2].Avatar, UriKind.Relative)); }
            memStrength3.Content = squad[2].Stat("STR");
            memDexterity3.Content = squad[2].Stat("DEX");
            memEndurance3.Content = squad[2].Stat("END");
            memIntelligence3.Content = squad[2].Stat("INT");
            memWisdom3.Content = squad[2].Stat("WIS");
            memCharisma3.Content = squad[2].Stat("CHR");

            if (squad[0].Race == "Dragonborn" || squad[0].Race == "Demonborn" || squad[0].Race == "Void Elf" || squad[0].Race == "Ascended" || squad[0].Race == "Metalborn") memInfo1.Foreground = new SolidColorBrush(Colors.Orange);
            else memInfo1.Foreground = new SolidColorBrush(Colors.Lime);
            if (squad[1].Race == "Dragonborn" || squad[1].Race == "Demonborn" || squad[1].Race == "Void Elf" || squad[1].Race == "Ascended" || squad[0].Race == "Metalborn") memInfo2.Foreground = new SolidColorBrush(Colors.Orange);
            else memInfo2.Foreground = new SolidColorBrush(Colors.Lime);
            if (squad[2].Race == "Dragonborn" || squad[2].Race == "Demonborn" || squad[2].Race == "Void Elf" || squad[2].Race == "Ascended" || squad[0].Race == "Metalborn") memInfo3.Foreground = new SolidColorBrush(Colors.Orange);
            else memInfo3.Foreground = new SolidColorBrush(Colors.Lime);
        }

        private void hireButton_Click(object sender, RoutedEventArgs e)
        {
            if (totalCost > 0)
                if (mainwindow.SelectedGroup.Coins - totalCost >= 0)
                {
                    if (checkBox1.IsChecked == true) { mainwindow.SelectedGroup.Crew.Add(squad[0]); }
                    if (checkBox2.IsChecked == true) { mainwindow.SelectedGroup.Crew.Add(squad[1]); }
                    if (checkBox3.IsChecked == true) { mainwindow.SelectedGroup.Crew.Add(squad[2]); }
                    mainwindow.SelectedGroup.Coins -= totalCost;
                    mainwindow.shake();
                    Close();
                }
                else
                {
                    this.ShowMessageAsync("Em...", "Not enough coins for that!");
                }
            else Close();
        }

        private void checkBox_Touched(object sender, RoutedEventArgs e)
        {
            if (!(checkBox1.IsChecked == true || checkBox2.IsChecked == true || checkBox3.IsChecked == true)) { hireButton.Content = "LEAVE"; totalCost = 0; }
            else
            {
                totalCost=0;
                if (checkBox1.IsChecked == true) { totalCost += squad[0].Cost; }
                if (checkBox2.IsChecked == true) { totalCost += squad[1].Cost; }
                if (checkBox3.IsChecked == true) { totalCost += squad[2].Cost; }
                hireButton.Content = $"HIRE (-{totalCost} coins)";
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            totalCost = 0;
            checkBox1.IsChecked = false;
            checkBox2.IsChecked = false;
            checkBox3.IsChecked = false;
            generate();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindow.save();
        }
    }
}
