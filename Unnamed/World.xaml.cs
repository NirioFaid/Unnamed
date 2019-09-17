using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class World : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        MainWindow mainwindow = null;
        [Magic]
        public ObservableCollection<Unit> Party { get; set; } = new ObservableCollection<Unit> { };

        public World(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
            namesList.ItemsSource = mainwindow.SelectedGroup.Crew;
            partyList.ItemsSource = Party;
        }

        private void Polygon_GotFocus(object sender, RoutedEventArgs e)
        {
            var polygon = (Polygon)sender;
            polygon.Stroke = Brushes.Lime;
            placeName.Text = polygon.Name.ToString();
        }

        private void Polygon_LostFocus(object sender, RoutedEventArgs e)
        {
            var polygon = (Polygon)sender;
            polygon.Stroke = Brushes.LimeGreen;
        }

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var polygon = (Polygon)sender;
            polygon.Focus();
        }

        private void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
            {
                keyboardFocus.MoveFocus(tRequest);
            }
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
            {
                keyboardFocus.MoveFocus(tRequest);
            }

        }

        private void ReturnChar_Click(object sender, RoutedEventArgs e)
        {
            Unit unit = partyList.SelectedItem as Unit;
            if (unit!=null)
            {
                mainwindow.SelectedGroup.Crew.Add(unit);
                Party.Remove(unit);
            }
        }

        private void MoveChar_Click(object sender, RoutedEventArgs e)
        {
            Unit unit = namesList.SelectedItem as Unit;
            if (unit!=null)
            {
                Party.Add(unit);
                mainwindow.SelectedGroup.Crew.Remove(unit);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            while (Party.Count != 0)
            {
                mainwindow.SelectedGroup.Crew.Add(Party[0]);
                Party.Remove(Party[0]);
            }
        }
    }
}
