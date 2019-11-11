using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Unnamed
{
    public partial class AddOrChangeItem : MetroWindow
    {
        MainWindow mainwindow = null;
        CharWindow charwindow = null;
        Item item; Move move; Unit unit;
        string Mode;
        public AddOrChangeItem(CharWindow chw, Unit unit, string mode)
        {
            InitializeComponent();
            charwindow = chw;
            Mode = mode;
            this.unit = unit;
            if (Mode == "new_move") hideItemStuff();
            else Mode = "new_item";
        }

        public AddOrChangeItem(CharWindow chw, Item item, Unit unit)
        {
            InitializeComponent();
            charwindow = chw;
            this.item = item;
            this.unit = unit;
            itemName.Text = this.item.Name;
            avgitemCost.Value = item.Cost;
            weight.Value = item.Weight;
            amount.Value = item.Stack;
        }

        public AddOrChangeItem(CharWindow chw, Move move, Unit unit)
        {
            InitializeComponent();
            charwindow = chw;
            this.unit = unit;
            Mode = "edit_move";
            this.move = move;
            itemName.Text = move.Name;
            descr.Document.Blocks.Clear();
            descr.AppendText(move.Descr);
            hideItemStuff();
        }

        public void hideItemStuff()
        {
            l1.Visibility = Visibility.Collapsed;
            l2.Visibility = Visibility.Collapsed;
            l3.Visibility = Visibility.Collapsed;
            l4.Visibility = Visibility.Collapsed;
            avgitemCost.Visibility = Visibility.Collapsed;
            itemCost.Visibility = Visibility.Collapsed;
            weight.Visibility = Visibility.Collapsed;
            amount.Visibility = Visibility.Collapsed;
        }

        public AddOrChangeItem(MainWindow mw, Item olditem, Unit unit)
        {
            InitializeComponent();
            mainwindow = mw;
            item = olditem;
            this.unit = unit;
            avgitemCost.Value = item.Cost;
            weight.Value = item.Weight;
            amount.Value = item.Stack;
            itemName.Text = item.Name;
            descr.Document.Blocks.Clear();
            descr.AppendText(item.Description);
            Mode = "edit_item";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*if (oldItem == null)
                mainwindow.addItem(itemName.Text, (int)itemCost.Value, 1, "");
            else mainwindow.replaceItem(oldItem, new Item (itemName.Text, (int)itemCost.Value, 1, ""), (int)itemCost.Value);
            if (storage != null) storage.updatePersonalItems();*/
            switch (Mode)
            {
                case "edit_move":
                    unit.MoveList.Where(x => x == move).FirstOrDefault().Name = itemName.Text;
                    unit.MoveList.Where(x => x == move).FirstOrDefault().Descr = new TextRange(descr.Document.ContentStart, descr.Document.ContentEnd).Text.Replace("\r", "").TrimEnd('\n');
                    break;
                /*case "new_move":
                    unit.MoveList.Add(new Move(itemName.Text, new TextRange(descr.Document.ContentStart, descr.Document.ContentEnd).Text.Replace("\r", "").TrimEnd('\n')));
                    break;*/
                case "edit_item":
                    unit.Inventory.Where(x => x == item).FirstOrDefault().Name = itemName.Text;
                    unit.Inventory.Where(x => x == item).FirstOrDefault().Description = new TextRange(descr.Document.ContentStart, descr.Document.ContentEnd).Text.Replace("\r", "").TrimEnd('\n');
                    unit.Inventory.Where(x => x == item).FirstOrDefault().Cost = (int)avgitemCost.Value;
                    unit.Inventory.Where(x => x == item).FirstOrDefault().Weight = (double)weight.Value;
                    unit.Inventory.Where(x => x == item).FirstOrDefault().Stack = (int)amount.Value;
                    break;
                case "new_item":
                    unit.Inventory.Add(new Item(itemName.Text, (int)avgitemCost.Value, (double)weight.Value, new TextRange(descr.Document.ContentStart, descr.Document.ContentEnd).Text.Replace("\r","").TrimEnd('\n')));
                    unit.Inventory.Where(x => x.Name == itemName.Text).FirstOrDefault().Stack = (int)amount.Value;
                    break;
                default: break;
            }
            if (charwindow!=null) charwindow.shake();
            Close();
        }
    }
}
