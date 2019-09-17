using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Unnamed
{
    public partial class Promotion : MetroWindow
    {
        MainWindow mainwindow = null;
        public Promotion(MainWindow mw)
        {
            InitializeComponent();
            mainwindow = mw;
        }

        private void attributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button.Content = $"+{(expText.Value.ToString())} EXP";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (expText.Value != null)
                mainwindow.promote((int)expText.Value);
            Close();
        }
    }
}
