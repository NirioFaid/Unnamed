using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Unnamed
{
    public partial class Log : MetroWindow
    {
        public Log()
        {
            InitializeComponent();
            string t = String.Empty;
            if (File.Exists("log.eq")) { t = File.ReadAllText("log.eq"); t = App.Crypt(t, "KuriGohan & Kamehameha"); }
            logs.Document.Blocks.Clear();
            logs.AppendText(t);
        }

        private void logs_TextChanged(object sender, TextChangedEventArgs e)
        {
            string t = new TextRange(logs.Document.ContentStart, logs.Document.ContentEnd).Text;
            File.WriteAllText("log.eq", App.Crypt(t, "KuriGohan & Kamehameha"));
        }

        private void MetroWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string t = new TextRange(logs.Document.ContentStart, logs.Document.ContentEnd).Text;
            File.WriteAllText("log.eq", App.Crypt(t, "KuriGohan & Kamehameha"));
        }
    }
}
