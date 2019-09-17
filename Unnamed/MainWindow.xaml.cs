using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace Unnamed
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        [Magic]
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group> { new Group(4400, "Team 0", "My group"), new Group(4400, "Rivals", "Rivals group") };
        [Magic]
        public int SelectedGroupID { get; set; } = 0;
        [Magic]
        public Group SelectedGroup => Groups[SelectedGroupID];
        [Magic]
        public ObservableCollection<Unit> World { get; set; } = new ObservableCollection<Unit> { };
        [Magic]
        public ObservableCollection<Unit> Graveyard { get; set; } = new ObservableCollection<Unit> { };
        [Magic]
        public DataGrid SelectedGrid { get; set; }

        [Magic]
        public class Group: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged([CallerMemberName]string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }

            public long Coins { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public ObservableCollection<Unit> Crew { get; set; } = new ObservableCollection<Unit> { };
            public int Count => Crew.Count;
            public ObservableCollection<Unit> Prisoners { get; set; } = new ObservableCollection<Unit> { };
            public ObservableCollection<Item> Storage { get; set; } = new ObservableCollection<Item> { };

            public Group() { }
            public Group(long coins, string name, string descr)
            {
                Coins = coins;
                Name = name;
                Description = descr;
            }
        }

        public class Data
        {
            public ObservableCollection<Group> Groups { get; set; }
            public int SelectedGroupID { get; set; }
            public ObservableCollection<Unit> World { get; set; }
            public ObservableCollection<Unit> Graveyard { get; set; }

            public Data() { }
            public Data(ObservableCollection<Group> groups, int selectedGroupID, ObservableCollection<Unit> world, ObservableCollection<Unit> grave)
            {
                Groups = groups;
                SelectedGroupID = selectedGroupID;
                World = world;
                Graveyard = grave;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("savefile.eq"))
            {
                string s = File.ReadAllText("savefile.eq");
                s = App.Crypt(s, "KuriGohan & Kamehameha");
                File.WriteAllText("savefile.eq", s);
                var loadData = new Data(Groups, SelectedGroupID, World, Graveyard);
                XmlSerializer xs = new XmlSerializer(typeof(Data));
                using (var sr = new StreamReader("savefile.eq"))
                {
                    loadData = (Data)xs.Deserialize(sr);
                }
                Groups = loadData.Groups;
                SelectedGroupID = loadData.SelectedGroupID;
                World = loadData.World;
                Graveyard = loadData.Graveyard;
            }
            save();
            unitsDisplay.DataContext = SelectedGroup;
            namesList.ItemsSource = SelectedGroup.Crew;
            prisonersList.ItemsSource = SelectedGroup.Prisoners;
            factionTab.Header = SelectedGroup.Name;
            groupsList.ItemsSource = Groups;
            groupsList.SelectedItem = SelectedGroup;
            worldList.ItemsSource = World;
            graveyardList.ItemsSource = Graveyard;
            SelectedGrid = namesList;
            foreach (Group f in Groups)
            {
                MenuItem newMenuItem = new MenuItem();
                newMenuItem.Header = f.Name;
                newMenuItem.Click += new RoutedEventHandler(moveUnitButton_Click);
                groupToMove.Items.Add(newMenuItem);
            }
        }

        public void shake()
        {
            if (groupToMove != null)
                groupToMove.Items.Clear();
            foreach (Group f in Groups)
            {
                MenuItem newMenuItem = new MenuItem();
                newMenuItem.Header = f.Name;
                newMenuItem.Click += new RoutedEventHandler(moveUnitButton_Click);
                groupToMove.Items.Add(newMenuItem);
            }
            try { groupsList.Items.Refresh(); } catch { groupsList.CommitEdit(); groupsList.CancelEdit(); } finally { groupsList.Items.Refresh(); }
            var faction = groupsList.SelectedItem as Group;
            if (faction != null)
            {
                SelectedGroupID = Groups.IndexOf(Groups.Where(x => x == faction).FirstOrDefault());
                unitsDisplay.DataContext = SelectedGroup;
                namesList.ItemsSource = SelectedGroup.Crew;
                prisonersList.ItemsSource = SelectedGroup.Prisoners;
                factionTab.Header = SelectedGroup.Name;
                groupsList.ItemsSource = Groups;
                RaisePropertyChanged("SelectedGroup");
            }
            try { namesList.Items.Refresh(); } catch { namesList.CommitEdit(); namesList.CancelEdit(); } finally { namesList.Items.Refresh(); }
            try { prisonersList.Items.Refresh(); } catch { prisonersList.CommitEdit(); prisonersList.CancelEdit(); } finally { prisonersList.Items.Refresh(); }
            try { worldList.Items.Refresh(); } catch { worldList.CommitEdit(); worldList.CancelEdit(); } finally { worldList.Items.Refresh(); }
            try { graveyardList.Items.Refresh(); } catch { graveyardList.CommitEdit(); graveyardList.CancelEdit(); } finally { graveyardList.Items.Refresh(); }
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                DataContext = crewMember;
                lvlCircle.Visibility = Visibility.Visible;
                memMoves.Visibility = Visibility.Visible;
                if (crewMember.skPoints1 > 0 || crewMember.atPoints > 0) pointIndicator.Visibility = Visibility.Visible;
                else pointIndicator.Visibility = Visibility.Collapsed;
                memMoves.ItemsSource = crewMember.MoveList;
                //if (crewMember != null) for (int i = 0; i < crewMember.Inventory.Count; i++) memItems.Items.Add(crewMember.Inventory[i]);
                /*try { photo.Source = new BitmapImage(new Uri(crewMember.Avatar, UriKind.Absolute)); }
                catch { try { photo.Source = new BitmapImage(new Uri(crewMember.Avatar, UriKind.Relative)); }
                    catch {crewMember.checkAvatar(); photo.Source = new BitmapImage(new Uri("Resource/empty.png", UriKind.Relative)); }
                }*/
            }
            //photo.Source = new BitmapImage(new Uri($"resources/{crewMember.Avatar}.png", UriKind.Relative));
            else
            {
                lvlCircle.Visibility = Visibility.Collapsed;
                memMoves.Visibility = Visibility.Collapsed;
                pointIndicator.Visibility = Visibility.Collapsed;
            }
        }

        public void save()
        {
            var saveData = new Data(Groups, SelectedGroupID, World, Graveyard);
            XmlSerializer xs = new XmlSerializer(typeof(Data));
            TextWriter tw = new StreamWriter("savefile.eq");
            xs.Serialize(tw, saveData);
            tw.Close();
            string s = File.ReadAllText("savefile.eq");
            s = App.Crypt(s, "KuriGohan & Kamehameha");
            File.WriteAllText("savefile.eq", s);
        }

        public void promote(int exp)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                /*if (crewMember.IsTraitor && new Random().Next(0, (crewMember.Stat("Wisdom") + crewMember.Stat("Intelligence") + crewMember.Stat("Dexterity")) / 30) == 0)
                {
                    this.ShowMessageAsync("Warning!", $"Police just received leaked info about us!\nIt looks like {crewMember.Name} ({crewMember.Subclass}) is saboteur.\nThis one's further fate is at your disposal.");
                    SelectedFaction.Crew.Find(x => x == crewMember).Comment += "\nSaboteur!";
                    SelectedFaction.Crew.Find(x => x == crewMember).IsTraitor = false;
                }*/
                SelectedGroup.Crew.Where(x => x == crewMember).FirstOrDefault().Promote(exp);
                //memName.Content = $"{crewMember.Name} ({crewMember.Promotions})";
                shake();
                save();
            }
        }

        public void dismiss()
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null) {
                if (crewMember.HP <= 0) Graveyard.Add(crewMember);
                switch (SelectedGrid.Name)
                {
                    case "namesList":
                        if (crewMember.HP > 0) World.Add(crewMember);
                        SelectedGroup.Crew.Remove(crewMember);
                        break;
                    case "prisonersList":
                        if (crewMember.HP > 0) World.Add(crewMember);
                        SelectedGroup.Prisoners.Remove(crewMember);
                        break;
                    case "worldList":
                        World.Remove(crewMember);
                        break;
                    case "graveyardList":
                        Graveyard.Remove(crewMember);
                        break;
                    default: break;
                }
            }
            shake();
            save();
        }

        private void HireButton_Click(object sender, RoutedEventArgs e)
        {
            HireWindow hire = new HireWindow(this);
            hire.ShowDialog();
        }

        private async void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync($"Dismiss {crewMember.Name}", $"Are you sure?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative) dismiss();
            }
        }

        private void PromoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGrid.SelectedItem as Unit != null)
            {
                Promotion prom = new Promotion(this);
                prom.ShowDialog();
            }
        }

        private void ReceiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (inc.Value != null) SelectedGroup.Coins += (int)inc.Value;
            inc.Value = 0;
            save();
        }

        private void MissionButton_Click(object sender, RoutedEventArgs e)
        {
            Mission mission = new Mission(this);
            mission.Show();
            save();
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            Log log = new Log();
            log.Show();
        }

        private void selectedList_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            var datagrid = (DataGrid)sender;
            string name = datagrid.Name.ToString();
            if (datagrid.SelectedItem != null)
                switch (name)
                {
                    case "namesList":
                        SelectedGrid = namesList;
                        prisonersList.SelectedItem = null;
                        worldList.SelectedItem = null;
                        graveyardList.SelectedItem = null;
                        break;
                    case "prisonersList":
                        SelectedGrid = prisonersList;
                        namesList.SelectedItem = null;
                        worldList.SelectedItem = null;
                        graveyardList.SelectedItem = null;
                        break;
                    case "worldList":
                        SelectedGrid = worldList;
                        prisonersList.SelectedItem = null;
                        namesList.SelectedItem = null;
                        graveyardList.SelectedItem = null;
                        break;
                    case "graveyardList":
                        SelectedGrid = graveyardList;
                        prisonersList.SelectedItem = null;
                        worldList.SelectedItem = null;
                        namesList.SelectedItem = null;
                        break;
                    default: break;
                }
            shake();
            save();
        }

        private void StorageButton_Click(object sender, RoutedEventArgs e)
        {
            Storage stor = new Storage(this);
            stor.ShowDialog();
        }

        private void ShopButton_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                Shop shop = new Shop(this, crewMember);
                shop.ShowDialog();
            }
            else this.ShowMessageAsync("Choose someone", "You need to have anyone in order to send him to the shop and buy things");
        }

        private void MetroWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if ((key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt)) || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S))
            {
                save();
            }

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            save();
            Environment.Exit(0);
        }

        private void namesList_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            var row = e.Row;
            var unit = row.DataContext as Unit;
            if (unit.HP <= 0) row.Foreground = new SolidColorBrush(Colors.Black);
            else if (unit.HP < unit.maxHP) row.Foreground = new SolidColorBrush(Colors.Red);
            else if (unit.skPoints1 > 0 || unit.atPoints > 0) row.Foreground = new SolidColorBrush(Colors.Gold);
        }

        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGrid.SelectedItem as Unit != null)
            {
                CharWindow profile = new CharWindow(this, SelectedGrid.SelectedItem as Unit);
                profile.Show();
            }
        }

        private void AddFaction_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("New faction!", $"Type your new faction name:");
            if (result != null)
            {
                Groups.Add(new Group(4000, result, ""));
                shake();
            }
        }

        private async void DeleteFaction_Click(object sender, RoutedEventArgs e)
        {
            var group = groupsList.SelectedItem as Group;
            if (group != null)
            {
                if (Groups.Count > 1)
                {
                    MessageDialogResult result = await this.ShowMessageAsync($"Disband {group.Name}", $"Are you fucking sure about that?", MessageDialogStyle.AffirmativeAndNegative);
                    if (result == MessageDialogResult.Affirmative) { SelectedGroupID = 0; Groups.Remove(group); shake(); }
                }
                else {
                    MessageDialogResult result = await this.ShowMessageAsync("Disbanding last faction", "You can't disband the last faction! Don't you dare! Factionless world is so.. boring. Doing it anyway, huh?");
                    if (result == MessageDialogResult.Affirmative) { await this.ShowMessageAsync("Nice try!", "Well... You just can't. DEAL WITH IT!"); }
                }
            }
        }

        private void moveUnitButton_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                var menuitem = (MenuItem)sender;
                string header = menuitem.Header.ToString();
                if (Groups.Contains(Groups.Where(x => x.Name == header).FirstOrDefault())) {
                    Groups.Where(x => x.Name == header).FirstOrDefault().Crew.Add(crewMember);
                    switch (SelectedGrid.Name)
                    {
                        case "namesList":
                            SelectedGroup.Crew.Remove(crewMember);
                            break;
                        case "prisonersList":
                            SelectedGroup.Prisoners.Remove(crewMember);
                            break;
                        case "worldList":
                            World.Remove(crewMember);
                            break;
                        case "graveyardList":
                            Graveyard.Remove(crewMember);
                            break;
                        default: break;
                    }
                    shake();
                }
                else if (header == "Prison")
                {
                    SelectedGroup.Prisoners.Add(crewMember);
                    switch (SelectedGrid.Name)
                    {
                        case "namesList":
                            SelectedGroup.Crew.Remove(crewMember);
                            break;
                        case "prisonersList":
                            SelectedGroup.Prisoners.Remove(crewMember);
                            break;
                        case "worldList":
                            World.Remove(crewMember);
                            break;
                        case "graveyardList":
                            Graveyard.Remove(crewMember);
                            break;
                        default: break;
                    }
                    shake();
                }
                else if (header == "World")
                {
                    World.Add(crewMember);
                    switch (SelectedGrid.Name)
                    {
                        case "namesList":
                            SelectedGroup.Crew.Remove(crewMember);
                            break;
                        case "prisonersList":
                            SelectedGroup.Prisoners.Remove(crewMember);
                            break;
                        case "worldList":
                            World.Remove(crewMember);
                            break;
                        case "graveyardList":
                            Graveyard.Remove(crewMember);
                            break;
                        default: break;
                    }
                    shake();
                }
            }
        }

        private void TradeButton_Click(object sender, RoutedEventArgs e)
        {
            Trade trade = new Trade(this);
            trade.ShowDialog();
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            Journal jrnl = new Journal();
            jrnl.Show();
        }

        private void CloneButton_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                World.Add(crewMember.Copy());
                shake();
                save();
            }
        }

        private void ImportUnitButton_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Unit));
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.Title = "Import from...";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && File.Exists(openFileDialog1.FileName))
            {
                var loadData = new Unit();
                using (var sr = new StreamReader(openFileDialog1.FileName))
                {
                    loadData = (Unit)xs.Deserialize(sr);
                }
                World.Add(loadData.Copy());
            }
            shake();
            save();
        }

        private void ExportUnitButton_Click(object sender, RoutedEventArgs e)
        {
            var crewMember = SelectedGrid.SelectedItem as Unit;
            if (crewMember != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(Unit));
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                saveFileDialog1.Title = "Export in...";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    TextWriter tw = new StreamWriter(saveFileDialog1.FileName);
                    xs.Serialize(tw, crewMember);
                    tw.Close();
                }
            }
        }

        private void ImportGroupButton_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Group));
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.Title = "Import from...";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && File.Exists(openFileDialog1.FileName))
            {
                var loadData = new Group();
                using (var sr = new StreamReader(openFileDialog1.FileName))
                {
                    loadData = (Group)xs.Deserialize(sr);
                }
                Groups.Add(loadData.Copy());
            }
            shake();
            save();
        }

        private void ExportGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroup != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(Group));
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                saveFileDialog1.Title = "Export in...";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    TextWriter tw = new StreamWriter(saveFileDialog1.FileName);
                    xs.Serialize(tw, SelectedGroup);
                    tw.Close();
                }
            }
        }

        private void WorldButton_Click(object sender, RoutedEventArgs e)
        {
            World world = new World(this);
            world.ShowDialog();
        }
    }
}
