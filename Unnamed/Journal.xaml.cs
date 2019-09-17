using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace Unnamed
{
    public partial class Journal : MetroWindow
    {
        public List<Note> displayedNotes = new List<Note> { };
        public List<Note> notes = new List<Note> { };

        public class Note
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public string Date { get; set; }

            public Note()
            {
                Name = "";
                Text = "";
                Date = DateTime.Now.ToString("dd.MM.yyyy") + " " + DateTime.Now.ToString("HH:mm");
            }
            public Note(string name, string text, string date)
            {
                Name = name;
                Text = text;
                Date = date;
            }
        }

        public class Data
        {
            public List<Note> Notes { get; set; } = new List<Note> { };

            public Data() { }
            public Data(List<Note> notes)
            {
                Notes = notes;
            }
        }

        public void save()
        {
            var saveData = new Data(notes);
            XmlSerializer xs = new XmlSerializer(typeof(Data));
            TextWriter tw = new StreamWriter("journal.eq");
            xs.Serialize(tw, saveData);
            tw.Close();
            string s = File.ReadAllText("journal.eq");
            s = App.Crypt(s, "YEAAAH! MARK RUFFALO, HUH?!");
            File.WriteAllText("journal.eq", s);
        }

        public void shake()
        {
            displayedNotes.Clear();
            foreach (Note i in notes) { if (i.Name.Contains(searchBar.Text)) displayedNotes.Add(i); }
            try { notesList.Items.Refresh(); } catch { notesList.CommitEdit(); notesList.CancelEdit(); } finally { notesList.Items.Refresh(); }
        }

        public Journal()
        {
            InitializeComponent();
            if (File.Exists("journal.eq"))
            {
                string s = String.Empty;
                var loadData = new Data(notes);
                XmlSerializer xs = new XmlSerializer(typeof(Data));
                s = File.ReadAllText("journal.eq");
                s = App.Crypt(s, "YEAAAH! MARK RUFFALO, HUH?!");
                File.WriteAllText("journal.eq", s);
                using (var sr = new StreamReader("journal.eq"))
                {
                    loadData = (Data)xs.Deserialize(sr);
                }
                notes = loadData.Notes;
            }
            save();
            notesList.ItemsSource = displayedNotes;
            shake();
        }

        private void notesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var note = notesList.SelectedItem as Note;
            if (note != null) { textBox.Document.Blocks.Clear(); textBox.AppendText(note.Text); }
            else { textBox.Document.Blocks.Clear(); }
            shake(); save();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var note = notesList.SelectedItem as Note;
            if (note != null && new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text != null && new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text != "")
                notes.Find(x => x == note).Text = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
            save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            notes.Reverse();
            notes.Add(new Note("", "", (DateTime.Now.ToString("dd.MM.yyyy") + " " + DateTime.Now.ToString("HH:mm"))));
            notes.Reverse();
            save();
            shake();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (textBox.IsReadOnly) textBox.IsReadOnly = false;
            else textBox.IsReadOnly = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string result = this.ShowModalInputExternal("Confirm", $"Are you sure about that? Type \"yes\" if so.");
            if (result != null && result.ToLower() == "yes")
            {
                var note = notesList.SelectedItem as Note;
                if (note != null) notes.Remove(note);
                shake(); save();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            shake();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            searchBar.Text = "";
        }
    }
}
