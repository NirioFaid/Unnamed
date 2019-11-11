using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Unnamed
{
    public partial class CharWindow : MetroWindow
    {
        MainWindow mainwindow = null;
        public Unit currentlyShownUnit { get; set; } = new Unit();
        public List<StatData> skillData = new List<StatData> { };
        public int startSkillPoints = 0;
        public int startSkillPoints2 = 0;
        public ObservableCollection<Move> moveList = new ObservableCollection<Move> { };
        public ObservableCollection<Item> inventory = new ObservableCollection<Item> { };
        DispatcherTimer updater; int counter = 0;

        public CharWindow(MainWindow mw, Unit unit)
        {
            InitializeComponent();
            mainwindow = mw;
            bio.Document.Blocks.Clear();
            currentlyShownUnit = unit;
            DataContext = currentlyShownUnit;
            Title = $"{currentlyShownUnit.Name} | Rating: {currentlyShownUnit.Rating}";
            moveList = currentlyShownUnit.MoveList;
            inventory = currentlyShownUnit.Inventory;
            startSkillPoints = currentlyShownUnit.skPoints1;
            skillsList.ItemsSource = currentlyShownUnit.CombatSkills;
            itemsList.ItemsSource = inventory;
            bio.AppendText(currentlyShownUnit.Bio);
            mutationsList.ItemsSource = currentlyShownUnit.Mutations;
            passiveList.ItemsSource = currentlyShownUnit.Passives;
            personList.ItemsSource = currentlyShownUnit.CharTraits;
            moves.ItemsSource = moveList; if (moveList.Count > 0) delMove.IsEnabled = true;

            updater = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 20), DispatcherPriority.Normal, delegate
            {
                if (counter >= 2) { currentlyShownUnit.Jump(); updater.Stop(); }
                counter++;
            }, this.Dispatcher); updater.Stop();
        }

        public void shake()
        {
            try { skillsList.Items.Refresh(); } catch { skillsList.CommitEdit(); skillsList.CancelEdit(); } finally { skillsList.Items.Refresh(); }
            try { skillsList2.Items.Refresh(); } catch { skillsList2.CommitEdit(); skillsList2.CancelEdit(); } finally { skillsList2.Items.Refresh(); }
            try { itemsList.Items.Refresh(); } catch { itemsList.CommitEdit(); itemsList.CancelEdit(); } finally { itemsList.Items.Refresh(); }
            try { personList.Items.Refresh(); } catch { personList.CommitEdit(); personList.CancelEdit(); } finally { personList.Items.Refresh(); }
            try { mutationsList.Items.Refresh(); } catch { mutationsList.CommitEdit(); mutationsList.CancelEdit(); } finally { mutationsList.Items.Refresh(); }
            try { passiveList.Items.Refresh(); } catch { passiveList.CommitEdit(); passiveList.CancelEdit(); } finally { passiveList.Items.Refresh(); }
            Title = $"{currentlyShownUnit.Name} | Rating: {currentlyShownUnit.Rating}";
            DataContext = currentlyShownUnit;
            gateway(); moves.Items.Refresh();
            mainwindow.shake(); mainwindow.save();
        }

        public void gateway()
        {
            if (currentlyShownUnit.atPoints > 0)
            {
                strength.HideUpDownButtons = false;
                endurance.HideUpDownButtons = false;
                wisdom.HideUpDownButtons = false;
                intelligence.HideUpDownButtons = false;
                dexterity.HideUpDownButtons = false;
                charisma.HideUpDownButtons = false;
            } else
            {
                strength.HideUpDownButtons = true;
                endurance.HideUpDownButtons = true;
                wisdom.HideUpDownButtons = true;
                intelligence.HideUpDownButtons = true;
                dexterity.HideUpDownButtons = true;
                charisma.HideUpDownButtons = true;
            }
            if (currentlyShownUnit.skPoints1 > 0)
            {
                incSkill.IsEnabled = true;
            } else
            {
                incSkill.IsEnabled = false;
            }
        }

        public void generate()
        {
            currentlyShownUnit = null;
            bio.Document.Blocks.Clear();
            currentlyShownUnit = new Unit(true);
            moveList = currentlyShownUnit.MoveList;
            inventory = currentlyShownUnit.Inventory;
            itemsList.ItemsSource = inventory;
            moves.ItemsSource = moveList;
            skillsList.ItemsSource = currentlyShownUnit.CombatSkills;
            personList.ItemsSource = currentlyShownUnit.CharTraits;
            bio.AppendText(currentlyShownUnit.Bio);
            mutationsList.ItemsSource = currentlyShownUnit.Mutations;
            passiveList.ItemsSource = currentlyShownUnit.Passives;
            shake();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void randButton_Click(object sender, RoutedEventArgs e)
        {
            generate();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindow.save();
        }

        private void avatarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true && openFileDialog.FileName != null)
            {
                /*string oldAvatar = currentlyShownUnit.Avatar;
                if (oldAvatar.Contains("pack://siteoforigin:,,,")) {
                    currentlyShownUnit.Avatar = "Resources/empty.png";
                    currentlyShownUnit.RaisePropertyChanged("Avatar");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}{oldAvatar.Replace("pack://siteoforigin:,,,", "")}");
                }*/
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/avatars");
                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/avatars/{Path.GetFileName(openFileDialog.FileName)}"))
                    File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}/avatars/{Path.GetFileName(openFileDialog.FileName)}");
                File.Copy(openFileDialog.FileName, $"{AppDomain.CurrentDomain.BaseDirectory}/avatars/{Path.GetFileName(openFileDialog.FileName)}");
                int i = 0;
                while(true)
                {
                    if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/avatars/a{i}{Path.GetFileName(openFileDialog.FileName).Replace(Path.GetFileNameWithoutExtension(openFileDialog.FileName), "")}"))
                    {
                        File.Move($"{AppDomain.CurrentDomain.BaseDirectory}/avatars/{Path.GetFileName(openFileDialog.FileName)}", $"{AppDomain.CurrentDomain.BaseDirectory}/avatars/a{i}{Path.GetFileName(openFileDialog.FileName).Replace(Path.GetFileNameWithoutExtension(openFileDialog.FileName), "")}");
                        break;
                    }
                    i++;
                }
                currentlyShownUnit.Avatar = $"pack://siteoforigin:,,,/avatars/a{i}{Path.GetFileName(openFileDialog.FileName).Replace(Path.GetFileNameWithoutExtension(openFileDialog.FileName), "")}";
            }
        }

        private void NUDValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            shake();
        }

        private void attrIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            currentlyShownUnit.atPoints--; shake();
            counter = 0; updater.Start();
        }

        private void attrDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            currentlyShownUnit.atPoints++; shake();
            counter = 0; updater.Start();
        }

        private void incSkill_Click(object sender, RoutedEventArgs e)
        {
            var skill = skillsList.SelectedItem as StatData;
            if (skill != null)
            {
                skill.Value++;
                currentlyShownUnit.skPoints1--;
                decSkill.IsEnabled = true; confirmSkill.IsEnabled = true;
                if (currentlyShownUnit.skPoints1 == 0) incSkill.IsEnabled = false;
                shake();
            }
        }

        private void decSkill_Click(object sender, RoutedEventArgs e)
        {
            var skill = skillsList.SelectedItem as StatData;
            if (skill != null)
            {
                skill.Value--;
                currentlyShownUnit.skPoints1++;
                if (currentlyShownUnit.skPoints1 == startSkillPoints) decSkill.IsEnabled = false;
                shake();
            }
        }

        private void confirmSkill_Click(object sender, RoutedEventArgs e)
        {
            startSkillPoints = currentlyShownUnit.skPoints1;
            decSkill.IsEnabled = false;
            if (startSkillPoints == 0) incSkill.IsEnabled = false;
            confirmSkill.IsEnabled = false;
            shake();
        }

        private void addMove_Click(object sender, RoutedEventArgs e)
        {
            AddOrChangeItem wnd = new AddOrChangeItem(this, currentlyShownUnit, "new_move");
            wnd.Show();
            moves.Items.Refresh();
        }

        private void editMove_Click(object sender, RoutedEventArgs e)
        {
            var move = moves.SelectedItem as Move;
            if (move != null)
            {
                AddOrChangeItem wnd = new AddOrChangeItem(this, move, currentlyShownUnit);
                delMove.IsEnabled = false; editMove.IsEnabled = false;
                wnd.Show();
            }
            moves.Items.Refresh();
        }

        private async void delMove_Click(object sender, RoutedEventArgs e)
        {
            var move = moves.SelectedItem as Move;
            if (move != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync($"Forget {move.Name}", $"Are you sure?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    currentlyShownUnit.MoveList.Remove(move);
                }
            }
            moves.Items.Refresh();
        }

        private void moves_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var move = moves.SelectedItem as Move;
            if (move != null)
            {
                this.ShowMessageAsync(move.Name, move.Descr);
            }
        }

        private void moves_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var move = moves.SelectedItem as Move;
            if (move != null)
            {
                display.Document.Blocks.Clear();
                display.AppendText(move.Descr);
                editMove.IsEnabled = true;
                delMove.IsEnabled = true;
            }
            else { display.Document.Blocks.Clear(); editMove.IsEnabled = false; }
        }

        private void addItem_Click(object sender, RoutedEventArgs e)
        {
            AddOrChangeItem wnd = new AddOrChangeItem(this, currentlyShownUnit, "new_item");
            wnd.Show();
        }

        private async void delItem_Click(object sender, RoutedEventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync($"Destroy {item.Name}", $"Are you sure?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    currentlyShownUnit.Inventory.Remove(item);
                }
            }
            shake();
        }

        private void itemsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null)
            {
                display.Document.Blocks.Clear();
                display.AppendText(item.Description);
                delItem.IsEnabled = true;
            }
            else { display.Document.Blocks.Clear(); }
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (cName.IsReadOnly)
            {
                cName.IsReadOnly = false;
                race.IsReadOnly = false;
                cClass.IsReadOnly = false;
                exp.IsReadOnly = false;
                gender.IsReadOnly = false;
                type.IsReadOnly = false;
                promote.Visibility = Visibility.Visible;
                atPoints.IsReadOnly = false;
                skPoints.IsReadOnly = false;
                bio.IsReadOnly = false;
                background.IsReadOnly = false;
                habitat.IsReadOnly = false;
                mutationsList.IsReadOnly = false;
                passiveList.IsReadOnly = false;
                addPassive.IsEnabled = true;
                delPassive.IsEnabled = true;
                incMutLVL.IsEnabled = true;
                decMutLVL.IsEnabled = true;
                incPersLVL.IsEnabled = true;
                decPersLVL.IsEnabled = true;
            } else {
                cName.IsReadOnly = true;
                race.IsReadOnly = true;
                cClass.IsReadOnly = true;
                exp.IsReadOnly = true;
                gender.IsReadOnly = true;
                type.IsReadOnly = true;
                promote.Visibility = Visibility.Collapsed;
                atPoints.IsReadOnly = true;
                skPoints.IsReadOnly = true;
                bio.IsReadOnly = true;
                background.IsReadOnly = true;
                habitat.IsReadOnly = true;
                mutationsList.IsReadOnly = true;
                passiveList.IsReadOnly = true;
                addPassive.IsEnabled = false;
                delPassive.IsEnabled = false;
                incMutLVL.IsEnabled = false;
                decMutLVL.IsEnabled = false;
                incPersLVL.IsEnabled = false;
                decPersLVL.IsEnabled = false;
            }
        }

        private void promote_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyShownUnit.EXP >= currentlyShownUnit.EXP4NextLVL)
                mainwindow.promote(0);
        }

        private void incPersLVL_Click(object sender, RoutedEventArgs e)
        {
            var stat = personList.SelectedItem as StatData;
            if (stat != null && stat.Value<6)
            {
                stat.Value++;
                shake();
            }
        }

        private void decPersLVL_Click(object sender, RoutedEventArgs e)
        {
            var stat = personList.SelectedItem as StatData;
            if (stat != null && stat.Value > 0)
            {
                stat.Value--;
                shake();
            }
        }

        private void incMutLVL_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyShownUnit != null)
            {
                currentlyShownUnit.Mutations.Add(new StatData("Common",0));
                shake();
            }
        }

        private void decMutLVL_Click(object sender, RoutedEventArgs e)
        {
            var mutation = mutationsList.SelectedItem as StatData;
            if (mutation != null)
            {
                currentlyShownUnit.Mutations.Remove(mutation);
                shake();
            }
        }

        private void addPassive_Click(object sender, RoutedEventArgs e)
        {
            /*if (currentlyShownUnit != null)
            {
                currentlyShownUnit.Passives.Add(new Move("",""));
                shake();
            }*/
        }

        private void delPassive_Click(object sender, RoutedEventArgs e)
        {
            var passive = passiveList.SelectedItem as Move;
            if (passive != null)
            {
                currentlyShownUnit.Passives.Remove(passive);
                shake();
            }
        }

        private void bio_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (currentlyShownUnit != null && new TextRange(bio.Document.ContentStart, bio.Document.ContentEnd).Text != null && new TextRange(bio.Document.ContentStart, bio.Document.ContentEnd).Text != "")
                currentlyShownUnit.Bio = new TextRange(bio.Document.ContentStart, bio.Document.ContentEnd).Text;
            mainwindow.save();
        }

        private void camp_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyShownUnit.Inventory.Contains(currentlyShownUnit.Inventory.Where(x => x.Name.Equals("Bedroll")).FirstOrDefault()))
            {
                var supply = currentlyShownUnit.Inventory.Where(x => x.Name.Equals("Camping Supplies")).FirstOrDefault();
                if (supply != null)
                {
                    if (supply.Stack > 1) supply.Stack--;
                    else currentlyShownUnit.Inventory.Remove(supply);
                    FullRestore();
                }
                else this.ShowMessageAsync("Not enough camping supplies","So you can't camp now");
            }
            else this.ShowMessageAsync("Can't rest now", "You need bedroll to make a camp rest");
        }

        void FullRestore()
        {
            currentlyShownUnit.HP = currentlyShownUnit.maxHP;
            currentlyShownUnit.SP = currentlyShownUnit.maxSP;
            currentlyShownUnit.MP = currentlyShownUnit.maxMP;
            currentlyShownUnit.WP = currentlyShownUnit.maxWP;
        }

        private void restore_Click(object sender, RoutedEventArgs e)
        {
            FullRestore();
        }

        private void editItem_Click(object sender, RoutedEventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (item != null)
            {
                AddOrChangeItem wnd = new AddOrChangeItem(mainwindow, item, currentlyShownUnit);
                wnd.Show();
            }
        }

        private void shop_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyShownUnit != null)
            {
                Shop shop = new Shop(mainwindow, currentlyShownUnit);
                shop.ShowDialog();
            }
        }
    }
}
