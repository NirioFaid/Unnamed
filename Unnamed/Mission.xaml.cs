using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Unnamed
{
    [Magic]
    public partial class Mission : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        MainWindow mainwindow = null;
        public ObservableCollection<Unit> AllyList { get; set; } = new ObservableCollection<Unit> { };
        public ObservableCollection<Unit> EnemyList { get; set; } = new ObservableCollection<Unit> { };
        public Unit SelectedAlly { get; set; } = null;
        public Unit SelectedEnemy { get; set; } = null;
        public int BP { get; set; } = 0;
        public int AllySPdec { get; set; }
        public Item BufItem = null;
        public Unit BufSummon = null;
        public string BufElementType = null;
        public Item BufCraftItem = null;
        public bool ItemRequired = false;
        public int EnemySPdec { get; set; }

        Random r = new Random();
        DispatcherTimer enemyAutoTimer;

        public Mission(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
            allyList.ItemsSource = AllyList;
            enemyList.ItemsSource = EnemyList;
            Update();

            enemyAutoTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (turnDisplay.Content.ToString() != "ENEMY TURN" || EnemyList.Count == 0 || AllyList.Count == 0) { enemyAutoTimer.Stop(); enemyAutoButton.Foreground = new SolidColorBrush(Colors.Lime); }
                else
                {
                    for (int i = 0; i < EnemyList.Count;i++)
                    {
                        DataGridRow row = (DataGridRow)enemyList.ItemContainerGenerator.ContainerFromIndex(i);
                        object item = enemyList.Items[i];
                        enemyList.SelectedItem = enemyList.Items[i];
                        enemyList.ScrollIntoView(item);
                        if (SelectedEnemy.HP == 0) continue;
                        //row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        enemyAutoPlay();
                    }
                }
            }, this.Dispatcher); enemyAutoTimer.Stop();
        }

        private void leaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Update()
        {
            if (SelectedAlly != null)
            {
                allyItems.ItemsSource = SelectedAlly.Inventory;
            }
            if (SelectedEnemy != null)
            {
                enemyItems.ItemsSource = SelectedEnemy.Inventory;
            }
            DataContext = new
            {
                AllyList,
                EnemyList,
                SelectedAlly,
                SelectedEnemy,
                BP
            };
            allyList.Items.Refresh();
            enemyList.Items.Refresh();
        }

        /* Scatter random code:
         * 
            Random r = new Random();
            int result = (int)scatterAverage.Value;
            int scatter = (int)((scatterAverage.Value / 100) * scatterPercantage.Value);
            for (int i = 0;i<scatter;i++) { result += r.Next(-1,2); }
             */

        private void allyList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var ally = allyList.SelectedItem as Unit;
            if (ally != null)
            {
                SelectedAlly = ally;
                allyMoves.ItemsSource = SelectedAlly.MoveList;
                if (mainwindow.Groups.Contains(mainwindow.Groups.Where(x => x.Crew.Contains(ally)).FirstOrDefault()) || mainwindow.World.Contains(ally))
                {
                    addToGuildButton.IsEnabled = false;
                    addAllyToWorldButton.Content = "+ world";
                }
                else {
                    addToGuildButton.IsEnabled = true;
                    addAllyToWorldButton.Content = "to world";
                }
                Update();
            }
        }

        private void enemyList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var enemy = enemyList.SelectedItem as Unit;
            if (enemy != null)
            {
                SelectedEnemy = enemy;
                enemyMoves.ItemsSource = SelectedEnemy.MoveList;
                if (mainwindow.Groups.Contains(mainwindow.Groups.Where(x => x.Crew.Contains(enemy)).FirstOrDefault()) || mainwindow.World.Contains(enemy))
                {
                    addEnemyToWorldButton.Content = "+ world";
                    addPrisonerButton.IsEnabled = false;
                }
                else
                {
                    addEnemyToWorldButton.Content = "to world";
                    addPrisonerButton.IsEnabled = true;
                }
                Update();
            }
        }

        private void addGuildAllyButton_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add guildian ally", $"Type his name");
            if (result != null && mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                AllyList.Add(mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void addCustomAllyButton_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add ally", $"Enter his type (nothing for random person)");
            if (result != null)
            {
                SelectedAlly = new Unit(result);
                AllyList.Add(SelectedAlly);
                Update();
            }
        }

        private void deleteAllyButton_Click(object sender, RoutedEventArgs e)
        {
            var ally = allyList.SelectedItem as Unit;
            if (ally != null)
            {
                AllyList.Remove(ally);
                SelectedAlly = null;
                allyMoves.ItemsSource = null;
                allyItems.ItemsSource = null;
                addAllyToWorldButton.Content = "+ world";
                addToGuildButton.IsEnabled = false;
                Update(); 
            }
        }

        private void addGuildEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add guildian enemy", $"Type his name");
            if (result != null && mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)) != null)
            {
                EnemyList.Add(mainwindow.SelectedGroup.Crew.FirstOrDefault(x => x.Name.Contains(result)));
                Update();
            }
        }

        private void addCustomEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Add enemy", $"Enter his type (nothing for random person)");
            if (result != null)
            {
                SelectedEnemy = new Unit(result);
                EnemyList.Add(SelectedEnemy);
                Update();
            }
        }

        private void deleteEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            var enemy = enemyList.SelectedItem as Unit;
            if (enemy != null)
            {
                EnemyList.Remove(enemy);
                SelectedEnemy = null;
                enemyMoves.ItemsSource = null;
                enemyItems.ItemsSource = null;
                addEnemyToWorldButton.Content = "+ world";
                addPrisonerButton.IsEnabled = false;
                Update();
            }
        }

        void AutoMode()
        {
            /*while(Анализ_состояния_участников) 
             * while (труъ) {
             * Создание списка мувов
             * Отсеивание по невозможным затратам
             * Отсеивание по целесообразности
             * Выбор рандомного из оставшихся
             * Если список пуст (в т.ч. из-за VP), то выход из цикла
             * }
             * Turn();
             * Все то же самое, но для противника и т.п.
             * Желательно с режимами для только противника и т.п.
             */
        }

        void UseMove(bool isAlly)
        {
            if (AllyList.Count == 0 || EnemyList.Count == 0) { UnselectMoves(); return; }
            Unit caster = null, target = null;
            ObservableCollection<Unit> casterArea = new ObservableCollection<Unit> { };
            ObservableCollection<Unit> targetArea = new ObservableCollection<Unit> { };
            string atckType = "", DMGType = "";
            bool missed = true; bool evaded = false; bool resisted = false;
            double casterHPmod, casterMPmod, casterSPmod, casterWPmod, targetHPmod, targetMPmod, targetSPmod, targetWPmod;
            double casterAHPmod, casterAMPmod, casterASPmod, casterAWPmod, targetAHPmod, targetAMPmod, targetASPmod, targetAWPmod;
            double HPdmg = 0, MPdmg = 0, SPdmg = 0, WPdmg = 0;
            double AHPdmg = 0, AMPdmg = 0, ASPdmg = 0, AWPdmg = 0;
            Move selMove;
            if (isAlly && allyMoves.SelectedItem != null)
            {
                caster = SelectedAlly; target = SelectedEnemy;
                casterArea = AllyList; targetArea = EnemyList;
                selMove = allyMoves.SelectedItem as Move;
            }
            else if (!isAlly && enemyMoves.SelectedItem != null)
            {
                caster = SelectedEnemy; target = SelectedAlly;
                casterArea = EnemyList; targetArea = AllyList;
                selMove = enemyMoves.SelectedItem as Move;
            }
            else { return; };
            atckType = selMove.Skill;
            DMGType = selMove.Attr;

            casterHPmod = selMove.cHP;
            casterMPmod = selMove.cMP;
            casterSPmod = selMove.cSP;
            casterWPmod = selMove.cWP;

            casterAHPmod = selMove.caHP;
            casterAMPmod = selMove.caMP;
            casterASPmod = selMove.caSP;
            casterAWPmod = selMove.caWP;

            targetHPmod = selMove.tHP;
            targetMPmod = selMove.tMP;
            targetSPmod = selMove.tSP;
            targetWPmod = selMove.tWP;

            targetAHPmod = selMove.taHP;
            targetAMPmod = selMove.taMP;
            targetASPmod = selMove.taSP;
            targetAWPmod = selMove.taWP;

            if (BufItem != null)
            {
                if (BufItem.Stack > 1) BufItem.Stack--;
                else if (isAlly) { SelectedAlly.Inventory.Remove(BufItem); allyItems.Items.Refresh(); }
                else if (!isAlly) { SelectedEnemy.Inventory.Remove(BufItem); enemyItems.Items.Refresh(); }
            }
            if (BufCraftItem != null)
            {
                caster.Inventory.Add(BufCraftItem);
                Update();
            }
            if (BufSummon != null)
            {
                if (isAlly) AllyList.Add(BufSummon);
                else EnemyList.Add(BufSummon);
                Update();
            }

            if (BufElementType==null)
            {
                if (atckType == "LongBlade" || atckType == "Axe" || atckType == "BluntWeapon" || atckType == "Polearms" || atckType == "ShortBlade")
                    BufElementType = "Steel";
                else BufElementType = "Normal";
            }

            if (targetHPmod == 0 && targetSPmod == 0 && targetSPmod == 0 && targetMPmod == 0 && targetWPmod == 0 && 
                casterHPmod == 0 && casterSPmod == 0 && casterWPmod == 0 && casterMPmod == 0 && 
                targetAHPmod == 0 && targetASPmod == 0 && targetAMPmod == 0 && targetAWPmod == 0 &&
                casterAHPmod == 0 && casterASPmod == 0 && casterAWPmod == 0 && casterAMPmod == 0) return;

            for (int i = 0; i < caster.Stat(atckType); i++)
            {
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterHPmod <= 0)
                        casterHPmod += Math.Abs(casterHPmod) / 100;
                    else casterHPmod += Math.Abs(casterHPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterMPmod <= 0)
                        casterMPmod += Math.Abs(casterMPmod) / 100;
                    else casterMPmod += Math.Abs(casterMPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterSPmod <= 0)
                        casterSPmod += Math.Abs(casterSPmod) / 100;
                    else casterSPmod += Math.Abs(casterSPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterWPmod <= 0)
                        casterWPmod += Math.Abs(casterWPmod) / 100;
                    else casterWPmod += Math.Abs(casterWPmod) / 10;

                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterAHPmod <= 0)
                        casterAHPmod += Math.Abs(casterAHPmod) / 100;
                    else casterAHPmod += Math.Abs(casterAHPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterAMPmod <= 0)
                        casterAMPmod += Math.Abs(casterAMPmod) / 100;
                    else casterAMPmod += Math.Abs(casterAMPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterASPmod <= 0)
                        casterASPmod += Math.Abs(casterASPmod) / 100;
                    else casterASPmod += Math.Abs(casterASPmod) / 10;
                if (caster.Stat(DMGType) * 0.5 > r.Next(0, 100))
                    if (casterAWPmod <= 0)
                        casterAWPmod += Math.Abs(casterAWPmod) / 100;
                    else casterAWPmod += Math.Abs(casterAWPmod) / 10;
            }
            if (caster.Stat(atckType) > r.Next(0, 100))
            {
                missed = false;
                if (target.SP >= 2 * Math.Abs(targetHPmod + targetSPmod) / 5 && ((target.Stat("DEX") * 2.4) + Math.Abs(targetHPmod + targetSPmod) - target.WeightMod*30 > r.Next(0, 100)))
                {
                    target.SP -= (int)(2 * Math.Abs(targetHPmod + targetSPmod) / 5);
                    evaded = true;
                }
                if (target.WP >= 2 * Math.Abs(targetMPmod + targetWPmod) / 5 && (target.Stat("WIS") * 4 - Math.Abs(targetMPmod + targetWPmod) > r.Next(0, 100)))
                {
                    target.WP -= (int)(2 * Math.Abs(targetMPmod + targetWPmod) / 5);
                    resisted = true;
                }
            }
            else if (r.Next(0, 44) == 0) missed = false;

            foreach (Unit u in casterArea)
            {
                double HP_MOD = 1, MP_MOD = 1, SP_MOD = 1, WP_MOD = 1;
                if (BufElementType == "Fire" && u.ElementEffect == "Wet") u.ElementEffect = "";
                else if (BufElementType == "Fire" && u.ElementEffect=="Frozen") u.ElementEffect = "";
                else if (BufElementType == "Fire" && u.ElementType!="Fire") u.ElementEffect = "Burning";
                else if (BufElementType == "Water" && u.ElementEffect == "Burning") u.ElementEffect = "";
                else if (BufElementType == "Water" && u.ElementType != "Water") u.ElementEffect = "Wet";
                else if (BufElementType == "Ice" && u.ElementEffect == "Burning") u.ElementEffect = "";
                else if (BufElementType == "Ice" && u.ElementEffect == "Wet") u.ElementEffect = "Frozen";
                else if (BufElementType == "Ice" && u.ElementType != "Ice") u.ElementEffect = "Frozen";
                else if (BufElementType == "Plant" && u.ElementEffect == "Wet") u.ElementEffect = "";

                if (BufElementType == u.ElementType && u.ElementType != "Steel" && u.ElementType != "Rock" && u.ElementType != "Bug")
                {
                    if (casterAHPmod > 0) HP_MOD *= 2;
                    else HP_MOD *= -1;
                    if (casterAMPmod > 0) MP_MOD *= 2;
                    else MP_MOD *= -1;
                    if (casterAWPmod > 0) WP_MOD *= 2;
                    else WP_MOD *= -1;
                    if (casterASPmod < 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Void" && u.ElementType == "Psy") || (BufElementType == "Light" && u.ElementType == "Void") ||
                (BufElementType == "Fire" && u.ElementType == "Plant") || (BufElementType == "Fire" && u.ElementType == "Ice") ||
                (BufElementType == "Fire" && u.ElementType == "Steel") || (BufElementType == "Fire" && u.ElementType == "Bug") ||
                (BufElementType == "Storm" && u.ElementType == "Water") || (BufElementType == "Water" && u.ElementType == "Fire") ||
                (BufElementType == "Water" && u.ElementType == "Rock") || (BufElementType == "Ice" && u.ElementType == "Plant") ||
                (BufElementType == "Ice" && u.ElementType == "Bug") || (BufElementType == "Earth" && u.ElementType == "Storm") ||
                (BufElementType == "Water" && u.ElementType == "Earth") || (BufElementType == "Earth" && u.ElementType == "Fire") ||
                (BufElementType == "Earth" && u.ElementType == "Rock") || (BufElementType == "Earth" && u.ElementType == "Poison") ||
                (BufElementType == "Plant" && u.ElementType == "Water") || (BufElementType == "Plant" && u.ElementType == "Rock") ||
                (BufElementType == "Rock" && u.ElementType == "Fire") || (BufElementType == "Rock" && u.ElementType == "Ice") ||
                (BufElementType == "Rock" && u.ElementType == "Bug") || (BufElementType == "Steel" && u.ElementType == "Ice") ||
                (BufElementType == "Steel" && u.ElementType == "Rock") || (BufElementType == "Void" && u.ElementType == "Light") ||
                (BufElementType == "Poison" && u.ElementType == "Plant") || (BufElementType == "Poison" && u.ElementType == "Bug") ||
                (BufElementType == "Bug" && u.ElementType == "Plant") || (BufElementType == "Earth" && u.ElementType == "Steel") ||
                (BufElementType == "Plant" && u.ElementType == "Earth") || (BufElementType == "Ice" && u.ElementType == "Earth"))
                {
                    if (casterAHPmod > 0) HP_MOD *= -1;
                    else HP_MOD *= 2;
                    if (casterAMPmod > 0) MP_MOD *= -1;
                    else MP_MOD *= 2;
                    if (casterAWPmod > 0) WP_MOD *= -1;
                    else WP_MOD *= 2;
                    if (casterASPmod > 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Psy" && u.ElementType == "Void") || (BufElementType == "Psy" && u.ElementType == "Light") ||
                   (BufElementType == "Plant" && u.ElementType == "Fire") || (BufElementType == "Ice" && u.ElementType == "Fire") ||
                   (BufElementType == "Steel" && u.ElementType == "Fire") || (BufElementType == "Bug" && u.ElementType == "Fire") ||
                   (BufElementType == "Fire" && u.ElementType == "Water") || (BufElementType == "Storm" && u.ElementType == "Earth") ||
                   (BufElementType == "Rock" && u.ElementType == "Earth") || (BufElementType == "Poison" && u.ElementType == "Earth") ||
                   (BufElementType == "Water" && u.ElementType == "Plant") || (BufElementType == "Fire" && u.ElementType == "Rock") ||
                   (BufElementType == "Ice" && u.ElementType == "Steel") || (BufElementType == "Plant" && u.ElementType == "Bug") ||
                   (BufElementType == "Rock" && u.ElementType == "Steel") || (BufElementType == "Light" && u.ElementType == "Steel") ||
                   (BufElementType == "Plant" && u.ElementType == "Poison") || (BufElementType == "Bug" && u.ElementType == "Poison") ||
                   (BufElementType == "Poison" && u.ElementType == "Steel") || (BufElementType == "Bug" && u.ElementType == "Steel") ||
                   (BufElementType == "Plant" && u.ElementType == "Steel") || (BufElementType == "Psy" && u.ElementType == "Steel") ||
                   (BufElementType == "Steel" && u.ElementType == "Storm") || (BufElementType == "Steel" && u.ElementType == "Steel") ||
                   (BufElementType == "Normal" && u.ElementType == "Steel") || (BufElementType == "Normal" && u.ElementType == "Rock") ||
                   (BufElementType == "Earth" && u.ElementType == "Bug") || (BufElementType == "Earth" && u.ElementType == "Plant") ||
                   (BufElementType == "Poison" && u.ElementType == "Rock") || (BufElementType == "Fire" && u.ElementType == "Light") ||
                   (BufElementType == "Poison" && u.ElementType == "Light") || (BufElementType == "Bug" && u.ElementType == "Light") ||
                   (BufElementType == "Bug" && u.ElementType == "Void") || (BufElementType == "Poison" && u.ElementType == "Void") ||
                   (BufElementType == "Fire" && u.ElementType == "Void") || (BufElementType == "Ice" && u.ElementType == "Void"))
                {
                    HP_MOD *= 0;
                    MP_MOD *= 0;
                    WP_MOD *= 0;
                    SP_MOD *= 0;
                }

                if (casterAHPmod * HP_MOD < 0 && u.Armor("physical") != 0)
                {
                    if (casterAHPmod * HP_MOD + u.Armor("physical") <= 0) casterAHPmod += u.Armor("physical");
                    else casterAHPmod = 0;
                }
                if (casterAMPmod * MP_MOD < 0 && u.Armor("magic") != 0)
                {
                    if (casterAMPmod * MP_MOD + u.Armor("magic") <= 0) casterAMPmod += u.Armor("magic");
                    else casterAMPmod = 0;
                }
                if (casterAWPmod * WP_MOD < 0 && u.Armor("magic") != 0)
                {
                    if (casterAWPmod * WP_MOD + u.Armor("magic") <= 0) casterAWPmod += u.Armor("magic");
                    else casterAWPmod = 0;
                }

                u.HP += (int)(casterAHPmod*HP_MOD);
                if (u.MP + (int)(casterAMPmod * MP_MOD) < 0) u.WP += (int)(casterAMPmod * MP_MOD) + u.MP;
                u.MP += (int)(casterAMPmod*MP_MOD);
                u.SP += (int)(casterASPmod*SP_MOD);
                u.WP += (int)(casterAWPmod*WP_MOD);
            }

            caster.HP += (int)casterHPmod;
            //if (caster.MP + (int)(casterMPmod) < 0) caster.WP += (int)(casterMPmod) + caster.MP;
            caster.MP += (int)casterMPmod;
            //if (caster.SP + (int)(casterSPmod) < 0) caster.HP += (int)(casterSPmod) + caster.SP;
            caster.SP += (int)casterSPmod;
            caster.WP += (int)casterWPmod;

            foreach (Unit u in targetArea)
            {
                AHPdmg = targetAHPmod;
                ASPdmg = targetASPmod;
                AMPdmg = targetAMPmod;
                AWPdmg = targetAWPmod;

                for (int i = 0; i < caster.Stat(DMGType); i++)
                {
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        AHPdmg += targetAHPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        ASPdmg += targetASPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        AMPdmg += targetAMPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        AWPdmg += targetAWPmod / 5;
                }

                double HP_MOD = 1, MP_MOD = 1, SP_MOD = 1, WP_MOD = 1;
                if (BufElementType == "Fire" && u.ElementEffect == "Wet") { u.ElementEffect = ""; HP_MOD *= 0.5; }
                else if (BufElementType == "Fire" && u.ElementEffect == "Frozen") { u.ElementEffect = ""; HP_MOD *= 0.5; }
                else if (BufElementType == "Fire" && u.ElementEffect == "Burning") { u.ElementEffect = ""; HP_MOD *= 1.5; }
                else if (BufElementType == "Fire" && u.ElementType != "Fire") { u.ElementEffect = "Burning"; }
                else if (BufElementType == "Water" && u.ElementEffect == "Burning") { u.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Water" && u.ElementType != "Water") { u.ElementEffect = "Wet"; }
                else if (BufElementType == "Ice" && u.ElementEffect == "Burning") { u.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Ice" && u.ElementType != "Ice") { u.ElementEffect = "Frozen"; }
                else if (BufElementType == "Ice" && u.ElementEffect == "Wet") { u.ElementEffect = "Frozen"; HP_MOD *= 1.5; SP_MOD *= 1.5; WP_MOD *= 1.5; MP_MOD *= 1.5; }
                else if (BufElementType == "Plant" && u.ElementEffect == "Wet") { u.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Storm" && u.ElementEffect == "Wet") { HP_MOD *= 1.5; SP_MOD *= 1.5; WP_MOD *= 1.5; MP_MOD *= 1.5; }

                if (BufElementType == u.ElementType && u.ElementType != "Normal" && u.ElementType != "Steel" && 
                    u.ElementType != "Rock" && u.ElementType != "Bug")
                {
                    if (AHPdmg > 0) HP_MOD *= 2;
                    else HP_MOD *= -1;
                    if (AMPdmg > 0) MP_MOD *= 2;
                    else MP_MOD *= -1;
                    if (AWPdmg > 0) WP_MOD *= 2;
                    else WP_MOD *= -1;
                    if (ASPdmg < 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Void" && u.ElementType == "Psy") || (BufElementType == "Light" && u.ElementType == "Void") ||
                  (BufElementType == "Fire" && u.ElementType == "Plant") || (BufElementType == "Fire" && u.ElementType == "Ice") ||
                  (BufElementType == "Fire" && u.ElementType == "Steel") || (BufElementType == "Fire" && u.ElementType == "Bug") ||
                  (BufElementType == "Storm" && u.ElementType == "Water") || (BufElementType == "Water" && u.ElementType == "Fire") ||
                  (BufElementType == "Water" && u.ElementType == "Rock") || (BufElementType == "Ice" && u.ElementType == "Plant") ||
                  (BufElementType == "Ice" && u.ElementType == "Bug") || (BufElementType == "Earth" && u.ElementType == "Storm") ||
                  (BufElementType == "Water" && u.ElementType == "Earth") || (BufElementType == "Earth" && u.ElementType == "Fire") ||
                  (BufElementType == "Earth" && u.ElementType == "Rock") || (BufElementType == "Earth" && u.ElementType == "Poison") ||
                  (BufElementType == "Plant" && u.ElementType == "Water") || (BufElementType == "Plant" && u.ElementType == "Rock") ||
                  (BufElementType == "Rock" && u.ElementType == "Fire") || (BufElementType == "Rock" && u.ElementType == "Ice") ||
                  (BufElementType == "Rock" && u.ElementType == "Bug") || (BufElementType == "Steel" && u.ElementType == "Ice") ||
                  (BufElementType == "Steel" && u.ElementType == "Rock") || (BufElementType == "Void" && u.ElementType == "Light") ||
                  (BufElementType == "Poison" && u.ElementType == "Plant") || (BufElementType == "Poison" && u.ElementType == "Bug") ||
                  (BufElementType == "Bug" && u.ElementType == "Plant") || (BufElementType == "Earth" && u.ElementType == "Steel") ||
                  (BufElementType == "Plant" && u.ElementType == "Earth") || (BufElementType == "Ice" && u.ElementType == "Earth"))
                {
                    if (AHPdmg > 0) HP_MOD *= -1;
                    else HP_MOD *= 2;
                    if (AMPdmg > 0) MP_MOD *= -1;
                    else MP_MOD *= 2;
                    if (AWPdmg > 0) WP_MOD *= -1;
                    else WP_MOD *= 2;
                    if (ASPdmg > 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Psy" && u.ElementType == "Void") || (BufElementType == "Psy" && u.ElementType == "Light") ||
                   (BufElementType == "Plant" && u.ElementType == "Fire") || (BufElementType == "Ice" && u.ElementType == "Fire") ||
                   (BufElementType == "Steel" && u.ElementType == "Fire") || (BufElementType == "Bug" && u.ElementType == "Fire") ||
                   (BufElementType == "Fire" && u.ElementType == "Water") || (BufElementType == "Storm" && u.ElementType == "Earth") ||
                   (BufElementType == "Rock" && u.ElementType == "Earth") || (BufElementType == "Poison" && u.ElementType == "Earth") ||
                   (BufElementType == "Water" && u.ElementType == "Plant") || (BufElementType == "Fire" && u.ElementType == "Rock") ||
                   (BufElementType == "Ice" && u.ElementType == "Steel") || (BufElementType == "Plant" && u.ElementType == "Bug") ||
                   (BufElementType == "Rock" && u.ElementType == "Steel") || (BufElementType == "Light" && u.ElementType == "Steel") ||
                   (BufElementType == "Plant" && u.ElementType == "Poison") || (BufElementType == "Bug" && u.ElementType == "Poison") ||
                   (BufElementType == "Poison" && u.ElementType == "Steel") || (BufElementType == "Bug" && u.ElementType == "Steel") ||
                   (BufElementType == "Plant" && u.ElementType == "Steel") || (BufElementType == "Psy" && u.ElementType == "Steel") ||
                   (BufElementType == "Steel" && u.ElementType == "Storm") || (BufElementType == "Steel" && u.ElementType == "Steel") ||
                   (BufElementType == "Normal" && u.ElementType == "Steel") || (BufElementType == "Normal" && u.ElementType == "Rock") ||
                   (BufElementType == "Earth" && u.ElementType == "Bug") || (BufElementType == "Earth" && u.ElementType == "Plant") ||
                   (BufElementType == "Poison" && u.ElementType == "Rock") || (BufElementType == "Fire" && u.ElementType == "Light") ||
                   (BufElementType == "Poison" && u.ElementType == "Light") || (BufElementType == "Bug" && u.ElementType == "Light") ||
                   (BufElementType == "Bug" && u.ElementType == "Void") || (BufElementType == "Poison" && u.ElementType == "Void") ||
                   (BufElementType == "Fire" && u.ElementType == "Void") || (BufElementType == "Ice" && u.ElementType == "Void"))
                {
                    HP_MOD *= 0;
                    MP_MOD *= 0;
                    WP_MOD *= 0;
                    SP_MOD *= 0;
                }

                if (AHPdmg * HP_MOD < 0 && u.Armor("physical") != 0)
                {
                    if (AHPdmg * HP_MOD + u.Armor("physical") <= 0) AHPdmg += u.Armor("physical");
                    else AHPdmg = 0;
                }
                if (AMPdmg * MP_MOD < 0 && u.Armor("magic") != 0)
                {
                    if (AMPdmg * MP_MOD + u.Armor("magic") <= 0) AMPdmg += u.Armor("magic");
                    else AMPdmg = 0;
                }
                if (AWPdmg * WP_MOD < 0 && u.Armor("magic") != 0)
                {
                    if (AWPdmg * WP_MOD + u.Armor("magic") <= 0) AWPdmg += u.Armor("magic");
                    else AWPdmg = 0;
                }

                u.HP += (int)(AHPdmg * HP_MOD);
                if (u.MP + (int)(AMPdmg * MP_MOD) < 0) u.WP += (int)(AMPdmg * MP_MOD) + u.MP;
                u.MP += (int)(AMPdmg * MP_MOD);
                u.SP += (int)(ASPdmg * SP_MOD);
                u.WP += (int)(AWPdmg * WP_MOD);
            }

            if (missed) { hitBox.Text = "MISSED!"; if (targetHPmod + targetSPmod + targetMPmod + targetWPmod == 0) hitBox.Text = ""; return; }

                HPdmg = targetHPmod;
                SPdmg = targetSPmod;
                MPdmg = targetMPmod;
                WPdmg = targetWPmod;

                for (int i = 0; i < caster.Stat(DMGType); i++)
                {
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        HPdmg += targetHPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        SPdmg += targetSPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        MPdmg += targetMPmod / 5;
                    if (caster.Stat(atckType) * 0.8 > r.Next(0, 100))
                        WPdmg += targetWPmod / 5;
                }

            {
                double HP_MOD = 1, MP_MOD = 1, SP_MOD = 1, WP_MOD = 1;
                if (BufElementType == "Fire" && target.ElementEffect == "Wet") { target.ElementEffect = ""; HP_MOD *= 0.5; }
                else if (BufElementType == "Fire" && target.ElementEffect == "Frozen") { target.ElementEffect = ""; HP_MOD *= 0.5; }
                else if (BufElementType == "Fire" && target.ElementEffect == "Burning") { target.ElementEffect = ""; HP_MOD *= 1.5; }
                else if (BufElementType == "Fire" && target.ElementType != "Fire") { target.ElementEffect = "Burning"; }
                else if (BufElementType == "Water" && target.ElementEffect == "Burning") { target.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Water" && target.ElementType != "Water") { target.ElementEffect = "Wet"; }
                else if (BufElementType == "Ice" && target.ElementEffect == "Burning") { target.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Ice" && target.ElementType != "Ice") { target.ElementEffect = "Frozen"; }
                else if (BufElementType == "Ice" && target.ElementEffect == "Wet") { target.ElementEffect = "Frozen"; HP_MOD *= 1.5; SP_MOD *= 1.5; WP_MOD *= 1.5; MP_MOD *= 1.5; }
                else if (BufElementType == "Plant" && target.ElementEffect == "Wet") { target.ElementEffect = ""; HP_MOD *= 0.5; SP_MOD *= 0.5; WP_MOD *= 0.5; }
                else if (BufElementType == "Storm" && target.ElementEffect == "Wet") { HP_MOD *= 1.5; SP_MOD *= 1.5; WP_MOD *= 1.5; MP_MOD *= 1.5; }

                if (BufElementType == target.ElementType && target.ElementType != "Normal" && target.ElementType != "Steel" 
                    && target.ElementType != "Rock" && target.ElementType != "Bug")
                {
                    if (HPdmg > 0) HP_MOD *= 2;
                    else HP_MOD *= -1;
                    if (MPdmg > 0) MP_MOD *= 2;
                    else MP_MOD *= -1;
                    if (WPdmg > 0) WP_MOD *= 2;
                    else WP_MOD *= -1;
                    if (SPdmg < 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Void" && target.ElementType == "Psy") || (BufElementType == "Light" && target.ElementType == "Void") ||
                  (BufElementType == "Fire" && target.ElementType == "Plant") || (BufElementType == "Fire" && target.ElementType == "Ice") ||
                  (BufElementType == "Fire" && target.ElementType == "Steel") || (BufElementType == "Fire" && target.ElementType == "Bug") ||
                  (BufElementType == "Storm" && target.ElementType == "Water") || (BufElementType == "Water" && target.ElementType == "Fire") ||
                  (BufElementType == "Water" && target.ElementType == "Rock") || (BufElementType == "Ice" && target.ElementType == "Plant") ||
                  (BufElementType == "Ice" && target.ElementType == "Bug") || (BufElementType == "Earth" && target.ElementType == "Storm") ||
                  (BufElementType == "Water" && target.ElementType == "Earth") || (BufElementType == "Earth" && target.ElementType == "Fire") ||
                  (BufElementType == "Earth" && target.ElementType == "Rock") || (BufElementType == "Earth" && target.ElementType == "Poison") ||
                  (BufElementType == "Plant" && target.ElementType == "Water") || (BufElementType == "Plant" && target.ElementType == "Rock") ||
                  (BufElementType == "Rock" && target.ElementType == "Fire") || (BufElementType == "Rock" && target.ElementType == "Ice") ||
                  (BufElementType == "Rock" && target.ElementType == "Bug") || (BufElementType == "Steel" && target.ElementType == "Ice") ||
                  (BufElementType == "Steel" && target.ElementType == "Rock") || (BufElementType == "Void" && target.ElementType == "Light") ||
                  (BufElementType == "Poison" && target.ElementType == "Plant") || (BufElementType == "Poison" && target.ElementType == "Bug") ||
                  (BufElementType == "Bug" && target.ElementType == "Plant") || (BufElementType == "Earth" && target.ElementType == "Steel") ||
                  (BufElementType == "Plant" && target.ElementType == "Earth") || (BufElementType == "Ice" && target.ElementType == "Earth"))
                {
                    if (HPdmg > 0) HP_MOD *= -1;
                    else HP_MOD *= 2;
                    if (MPdmg > 0) MP_MOD *= -1;
                    else MP_MOD *= 2;
                    if (WPdmg > 0) WP_MOD *= -1;
                    else WP_MOD *= 2;
                    if (SPdmg > 0) SP_MOD *= 0;
                }
                else if ((BufElementType == "Psy" && target.ElementType == "Void") || (BufElementType == "Psy" && target.ElementType == "Light") ||
                   (BufElementType == "Plant" && target.ElementType == "Fire") || (BufElementType == "Ice" && target.ElementType == "Fire") ||
                   (BufElementType == "Steel" && target.ElementType == "Fire") || (BufElementType == "Bug" && target.ElementType == "Fire") ||
                   (BufElementType == "Fire" && target.ElementType == "Water") || (BufElementType == "Storm" && target.ElementType == "Earth") ||
                   (BufElementType == "Rock" && target.ElementType == "Earth") || (BufElementType == "Poison" && target.ElementType == "Earth") ||
                   (BufElementType == "Water" && target.ElementType == "Plant") || (BufElementType == "Fire" && target.ElementType == "Rock") ||
                   (BufElementType == "Ice" && target.ElementType == "Steel") || (BufElementType == "Plant" && target.ElementType == "Bug") ||
                   (BufElementType == "Rock" && target.ElementType == "Steel") || (BufElementType == "Light" && target.ElementType == "Steel") ||
                   (BufElementType == "Plant" && target.ElementType == "Poison") || (BufElementType == "Bug" && target.ElementType == "Poison") ||
                   (BufElementType == "Poison" && target.ElementType == "Steel") || (BufElementType == "Bug" && target.ElementType == "Steel") ||
                   (BufElementType == "Plant" && target.ElementType == "Steel") || (BufElementType == "Psy" && target.ElementType == "Steel") ||
                   (BufElementType == "Steel" && target.ElementType == "Storm") || (BufElementType == "Steel" && target.ElementType == "Steel") ||
                   (BufElementType == "Normal" && target.ElementType == "Steel") || (BufElementType == "Normal" && target.ElementType == "Rock") ||
                   (BufElementType == "Earth" && target.ElementType == "Bug") || (BufElementType == "Earth" && target.ElementType == "Plant") ||
                   (BufElementType == "Poison" && target.ElementType == "Rock") || (BufElementType == "Fire" && target.ElementType == "Light") ||
                   (BufElementType == "Poison" && target.ElementType == "Light") || (BufElementType == "Bug" && target.ElementType == "Light") ||
                   (BufElementType == "Bug" && target.ElementType == "Void") || (BufElementType == "Poison" && target.ElementType == "Void") ||
                   (BufElementType == "Fire" && target.ElementType == "Void") || (BufElementType == "Ice" && target.ElementType == "Void"))
                {
                    HP_MOD *= 0;
                    MP_MOD *= 0;
                    WP_MOD *= 0;
                    SP_MOD *= 0;
                }

                if (evaded && resisted && (targetHPmod != 0 || targetSPmod != 0) && (targetMPmod != 0 || targetWPmod != 0)) { hitBox.Text = "EVADED & RESISTED!"; }
                else if (evaded && (targetHPmod != 0 || targetSPmod != 0)) { hitBox.Text = "EVADED!"; }
                else if (resisted && (targetMPmod != 0 || targetWPmod != 0)) { hitBox.Text = "RESISTED!"; }
                else if (!resisted && !evaded && (HPdmg != 0 || SPdmg != 0 || MPdmg != 0 || WPdmg != 0)) hitBox.Text = "HIT!";
                else hitBox.Text = "";

                if (!evaded && target.Inventory.Contains(target.Inventory.Where(x => x.Description.Contains("[Block]")).FirstOrDefault()) && target.SP > 3 * Math.Abs(targetHPmod + targetSPmod) / 5 && target.Stat("Block") / 2 + Math.Abs(targetHPmod + targetSPmod) > r.Next(0, 100))
                {
                    target.SP -= (int)(3 * Math.Abs(targetHPmod + targetSPmod) / 5);
                    hitBox.Text = "BLOCKED!";
                }
                else if (!evaded)
                {
                    if (HPdmg * HP_MOD < 0 && target.Armor("physical") != 0)
                    {
                        if (HPdmg * HP_MOD + target.Armor("physical") <= 0) HPdmg += target.Armor("physical");
                        else HPdmg = 0;
                    }
                    if (selMove.IsDrain && HPdmg * HP_MOD < 0) caster.HP += (int)Math.Abs(HPdmg * HP_MOD);
                    if (selMove.IsDrain && SPdmg * SP_MOD < 0) caster.SP += (int)Math.Abs(SPdmg * SP_MOD);
                    target.HP += (int)(HPdmg*HP_MOD);
                    target.SP += (int)(SPdmg*SP_MOD);
                }
                if (!resisted)
                {
                    if (MPdmg * MP_MOD < 0 && target.Armor("magic") != 0)
                    {
                        if (MPdmg * MP_MOD + target.Armor("magic") <= 0) MPdmg += target.Armor("magic");
                        else MPdmg = 0;
                    }
                    if (WPdmg * WP_MOD < 0 && target.Armor("magic") != 0)
                    {
                        if (WPdmg * WP_MOD + target.Armor("magic") <= 0) WPdmg += target.Armor("magic");
                        else WPdmg = 0;
                    }
                    if (selMove.IsDrain && MPdmg * MP_MOD < 0) caster.MP += (int)Math.Abs(MPdmg * MP_MOD);
                    if (selMove.IsDrain && WPdmg * WP_MOD < 0) caster.SP += (int)Math.Abs(WPdmg * WP_MOD);
                    if (target.MP + (int)(MPdmg * MP_MOD) < 0) target.WP += (int)(MPdmg * MP_MOD) + target.MP;
                    target.MP += (int)(MPdmg*MP_MOD);
                    target.WP += (int)(WPdmg*WP_MOD);
                }
            }
            if (!evaded||!resisted) caster.EncountStat(atckType, 1);
            if (!evaded||!resisted) caster.Promote((int)Math.Abs(targetHPmod + targetSPmod + targetMPmod + targetWPmod));
            UnselectMoves();
            //for (int i = 0; i < targetArea.Count(); i++) if (targetArea[i].WP == 0) { targetArea[i].WP = 40; betray(targetArea[i], !isAlly); }
        }

        void UnselectMoves()
        {
            enemyMoves.SelectedItem = null;
            allyMoves.SelectedItem = null;
            allyItems.SelectedItem = null;
            enemyItems.SelectedItem = null;
            BufItem = null;
            BufSummon = null;
            BufElementType = null;
            BufCraftItem = null;
        }

        private void allyThrowButton_Click(object sender, RoutedEventArgs e)
        {
            UseMove(true);
            UnselectMoves();
        }

        private void turnDisplay_Click(object sender, RoutedEventArgs e)
        {
            Turn();
            Update();
            UnselectMoves();
        }

        void Turn()
        {
            if (AllyList.Count() > 0 && EnemyList.Count() > 0)
            {
                if (turnDisplay.Content.ToString() != "START BATTLE" && turnDisplay.Content.ToString() == "YOUR TURN") {
                    turnDisplay.Content = "ENEMY TURN";
                    enemyThrowButton.IsEnabled = true;
                    allyThrowButton.IsEnabled = false;
                    foreach (Unit u in AllyList)
                    {
                        int MPR = 0, HPR = 0, SPR = 0, WPR = 0;
                        foreach(Item i in u.Inventory)
                        {
                            string s = i.Description;
                            string[] itemProps = s.Split('\n');
                            for (int j = 0; j<itemProps.Length;j++)
                            {
                                string[] prop = itemProps[j].Split(' ');

                                switch (prop[0])
                                {
                                    case "MPR":
                                        MPR += Int32.Parse(prop[1]);
                                        break;
                                    case "HPR":
                                        HPR += Int32.Parse(prop[1]);
                                        break;
                                    case "SPR":
                                        SPR += Int32.Parse(prop[1]);
                                        break;
                                    case "WPR":
                                        WPR += Int32.Parse(prop[1]);
                                        break;
                                    default: break;
                                }
                            }
                        }

                        u.MP += u.MP / 4 + MPR;
                        u.SP += 10 + SPR + u.Stat("DEX") / 2;
                        u.HP += HPR;
                        u.WP += WPR;
                    }
                }
                else if (turnDisplay.Content.ToString() != "START BATTLE" && turnDisplay.Content.ToString() == "ENEMY TURN") {
                    turnDisplay.Content = "YOUR TURN";
                    enemyThrowButton.IsEnabled = false;
                    allyThrowButton.IsEnabled = true;
                    foreach(Unit u in EnemyList)
                    {
                        int MPR = 0, HPR = 0, SPR = 0, WPR = 0;
                        foreach (Item i in u.Inventory)
                        {
                            string s = i.Description;
                            string[] itemProps = s.Split('\n');
                            for (int j = 0; j < itemProps.Length; j++)
                            {
                                string[] prop = itemProps[j].Split(' ');

                                switch (prop[0])
                                {
                                    case "MPR":
                                        MPR += Int32.Parse(prop[1]);
                                        break;
                                    case "HPR":
                                        HPR += Int32.Parse(prop[1]);
                                        break;
                                    case "SPR":
                                        SPR += Int32.Parse(prop[1]);
                                        break;
                                    case "WPR":
                                        WPR += Int32.Parse(prop[1]);
                                        break;
                                    default: break;
                                }
                            }
                        }

                        u.MP += u.MP / 4 + MPR;
                        u.SP += 10 + SPR + u.Stat("DEX") / 2;
                        u.HP += HPR;
                        u.WP += WPR;
                    }
                }

                if (turnDisplay.Content.ToString() == "START BATTLE")
                {
                    if (AllyList.Sum(x => x.Attributes[1].Value) > EnemyList.Sum(x => x.Attributes[1].Value)) {
                        turnDisplay.Content = "ENEMY TURN";
                        enemyThrowButton.IsEnabled = true;
                        allyThrowButton.IsEnabled = false;
                        foreach(Unit u in AllyList)
                        {
                            u.SP = 15 + (u.Stat("WIS") + u.Stat("DEX")) / 8;
                            u.MP += u.MP / 4;
                        }
                        foreach (Unit u in EnemyList)
                        {
                            u.SP = 15 + (u.Stat("WIS") + u.Stat("DEX")) / 8;
                            u.MP += u.MP / 4;
                        }
                    }
                    else {
                        turnDisplay.Content = "YOUR TURN";
                        enemyThrowButton.IsEnabled = false;
                        allyThrowButton.IsEnabled = true;
                        foreach (Unit u in AllyList)
                        {
                            u.SP = 15 + (u.Stat("WIS") + u.Stat("DEX")) / 8;
                            u.MP += u.MP / 4;
                        }
                        foreach (Unit u in EnemyList)
                        {
                            u.SP = 15 + (u.Stat("WIS") + u.Stat("DEX")) / 8;
                            u.MP += u.MP / 4;
                        }
                    }
                }
            }
        }

        private void allyName_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedAlly != null)
            {
                CharWindow chw = new CharWindow(mainwindow, SelectedAlly);
                chw.Show();
            }
        }

        private void enemyName_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedEnemy != null)
            {
                CharWindow chw = new CharWindow(mainwindow, SelectedEnemy);
                chw.Show();
            }
        }

        void InputMods(Move move, bool isAlly)
        {
            if (move != null && move != null && AllyList.Count > 0 && EnemyList.Count > 0)
            {
                try
                {
                    Unit caster;
                    if (isAlly) caster = SelectedAlly;
                    else caster = SelectedEnemy;
                    if ((caster.HP + move.cHP < 0) || (caster.MP + move.cMP < 0) || (caster.SP + move.cSP < 0) || (caster.WP + move.cWP < 0)) { UnselectMoves(); return; }
                    else { displayHitChance(move); }
                }
                catch { }
            }

        }

        void InputMoveMods(Move move, bool isAlly)
        {
            if (move != null) InputMods(move, isAlly);
        }

        void InputItemMods(Item item, bool isAlly)
        {
            //if (item != null) InputMods(item.Description, isAlly);
        }

        private void enemyThrowButton_Click(object sender, RoutedEventArgs e)
        {
            UseMove(false);
            UnselectMoves();
        }

        private void allyMoves_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (turnDisplay.Content != null && turnDisplay.Content.ToString() == "YOUR TURN")
            {
                allyItems.SelectedItem = null;
                Move selected = allyMoves.SelectedItem as Move;
                InputMoveMods(selected, true);
            }
        }

        private void enemyMoves_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (turnDisplay.Content != null && turnDisplay.Content.ToString() == "ENEMY TURN")
            {
                enemyItems.SelectedItem = null;
                Move selected = enemyMoves.SelectedItem as Move;
                InputMoveMods(selected, false);
            }
        }

        private void addToGuildButton_Click(object sender, RoutedEventArgs e)
        {
            var ally = allyList.SelectedItem as Unit;
            if (ally != null)
            {
                mainwindow.SelectedGroup.Crew.Add(ally);
                mainwindow.shake();
                mainwindow.save();
                addToGuildButton.IsEnabled = false;
                addAllyToWorldButton.Content = "+ world";
            }
        }

        private void addAllyToWorldButton_Click(object sender, RoutedEventArgs e)
        {
            if (addAllyToWorldButton.Content.ToString() == "to world")
            {
                var ally = allyList.SelectedItem as Unit;
                if (ally != null)
                {
                    mainwindow.World.Add(ally);
                    mainwindow.shake();
                    mainwindow.save();
                    addToGuildButton.IsEnabled = false;
                    addAllyToWorldButton.Content = "+ world";
                }
            }
            else
            {
                string result = this.ShowModalInputExternal("Add someone from world", $"Type his name");
                if (result != null && mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)) != null)
                {
                    AllyList.Add(mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)));
                    Update();
                }
            }
        }

        private void betray(Unit u, bool IsAlly)
        {
            if (IsAlly)
            {
                AllyList.Remove(u);
                EnemyList.Add(u);
                if (u == SelectedAlly)
                {
                    SelectedAlly = null;
                    allyList.SelectedItem = null;
                    allyMoves.ItemsSource = null;
                    allyItems.ItemsSource = null;
                    addAllyToWorldButton.Content = "+ world";
                    addToGuildButton.IsEnabled = false;
                }
            } else
            {
                EnemyList.Remove(u);
                AllyList.Add(u);
                if (u == SelectedEnemy)
                {
                    SelectedEnemy = null;
                    enemyList.SelectedItem = null;
                    enemyMoves.ItemsSource = null;
                    enemyItems.ItemsSource = null;
                    addEnemyToWorldButton.Content = "+ world";
                    addPrisonerButton.IsEnabled = false;
                }
            }
            Update();
        }

        private void moveAllyButton_Click(object sender, RoutedEventArgs e)
        {
            betray(SelectedAlly, true);
        }

        private void moveEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            betray(SelectedEnemy, false);
        }

        private void addPrisonerButton_Click(object sender, RoutedEventArgs e)
        {
            var enemy = enemyList.SelectedItem as Unit;
            if (enemy != null)
            {
                mainwindow.SelectedGroup.Prisoners.Add(enemy);
                mainwindow.shake();
                mainwindow.save();
                addPrisonerButton.IsEnabled = false;
                addEnemyToWorldButton.Content = "+ world";
            }
        }

        private void addEnemyToWorldButton_Click(object sender, RoutedEventArgs e)
        {
            if (addEnemyToWorldButton.Content.ToString() == "to world")
            {
                var enemy = enemyList.SelectedItem as Unit;
                if (enemy != null)
                {
                    mainwindow.World.Add(enemy);
                    mainwindow.shake();
                    mainwindow.save();
                    addPrisonerButton.IsEnabled = false;
                    addEnemyToWorldButton.Content = "+ world";
                }
            }
            else
            {
                string result = this.ShowModalInputExternal("Add someone from world", $"Type his name");
                if (result != null && mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)) != null)
                {
                    EnemyList.Add(mainwindow.World.FirstOrDefault(x => x.Name.Contains(result)));
                    Update();
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            UnselectMoves();
        }

        private void enemyItems_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (turnDisplay.Content != null && turnDisplay.Content.ToString() == "ENEMY TURN")
            {
                enemyMoves.SelectedItem = null;
                Item selected = enemyItems.SelectedItem as Item;
                InputItemMods(selected, false);
            }
        }

        private void allyItems_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (turnDisplay.Content != null && turnDisplay.Content.ToString() == "YOUR TURN")
            {
                allyMoves.SelectedItem = null;
                Item selected = allyItems.SelectedItem as Item;
                InputItemMods(selected, true);
            }
        }

        private void endBattleButton_Click(object sender, RoutedEventArgs e)
        {
            turnDisplay.Content = "START BATTLE";
            UnselectMoves();
            foreach (Unit u in AllyList) u.SP = 0;
            foreach (Unit u in EnemyList) u.SP = 0;
            Update();
        }

        private void exchangeButton_Click(object sender, RoutedEventArgs e)
        {
            Trade trade = new Trade(mainwindow, AllyList, EnemyList);
            trade.ShowDialog();
        }

        private void displayHitChance(Move move)
        {
            if (AllyList.Count() > 0 && EnemyList.Count() > 0)
            {
                if (turnDisplay.Content.ToString() == "YOUR TURN")
                {
                    int hitChance = SelectedAlly.Stat(move.Skill);
                    int critDMG = SelectedAlly.Stat(move.Attr);
                    if (hitChance <= 98 && hitChance >= 2) hitBox.Text = $"{hitChance}% hit | {critDMG}% crit";
                    else if (hitChance <= 2) hitBox.Text = $"~2% hit | {critDMG}% crit";
                    else if (hitChance >= 98) hitBox.Text = $"~98% hit | {critDMG}% crit";
                }
                else if (turnDisplay.Content.ToString() == "ENEMY TURN")
                {
                    int hitChance = SelectedEnemy.Stat(move.Skill);
                    int critDMG = SelectedEnemy.Stat(move.Attr);
                    if (hitChance <= 98 && hitChance >= 2) hitBox.Text = $"{hitChance}% hit | {critDMG}% crit";
                    else if (hitChance <= 2) hitBox.Text = $"~2% hit | {critDMG}% crit";
                    else if (hitChance >= 98) hitBox.Text = $"~98% hit | {critDMG}% crit";
                }
                else
                {
                    hitBox.Text = "";
                }
            }
        }

        

        private void enemyAutoButton_Click(object sender, RoutedEventArgs e)
        {
            if (enemyAutoTimer.IsEnabled) { enemyAutoTimer.Stop(); enemyAutoButton.Foreground = new SolidColorBrush(Colors.Lime); }
            else { enemyAutoTimer.Start(); enemyAutoButton.Foreground = new SolidColorBrush(Colors.DarkGoldenrod); }
        }

        private void enemyAutoPlay()
        {
            allyList.SelectedItem = allyList.Items[r.Next(0, allyList.Items.Count)];
            switch (r.Next(0,2))
            {
                case 0:
                    if (enemyItems.Items.Count > 0) enemyItems.SelectedItem = enemyItems.Items[r.Next(0,enemyItems.Items.Count)];
                    break;
                case 1:
                    enemyMoves.SelectedItem = enemyMoves.Items[r.Next(0, enemyMoves.Items.Count)];
                    break;
                default:
                    break;
            }
            if (EnemySPdec!=0)
            {
                UseMove(false);
                UnselectMoves();
            }
            else if (SelectedEnemy.SP < 8)
            {
                Turn();
                Update();
                UnselectMoves();
            }
        }
    }
}
