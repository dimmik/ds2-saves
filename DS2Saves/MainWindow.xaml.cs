using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DS2Saves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IEnumerable<string> Saves()
        {
            return Directory
                //.EnumerateFiles(SavesFolder.Text, "*", new EnumerationOptions() {  })
                .GetFiles(SavesFolder.Text, "*").OrderByDescending(d => new FileInfo(d).CreationTime)
                .Select(s => System.IO.Path.GetFileName(s));
        }
        public MainWindow()
        {
            InitializeComponent();
            PathToGame.Text = File.ReadAllText("path-to-game.txt").Trim();
            SavesFolder.Text = File.ReadAllText("saves-folder.txt").Trim();
            Template.Text = File.ReadAllText("template.txt").Trim(); ;
            AvailableSaves.ItemsSource = Saves();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var savesFn = System.IO.Path.GetFileName(PathToGame.Text);
            var storeFn = Template.Text.Replace("{date}", $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
            var storePath = System.IO.Path.Combine(SavesFolder.Text, storeFn);
            File.Copy(PathToGame.Text, storePath, overwrite: true);
            AvailableSaves.ItemsSource = Saves();
        }

        private void Restore(object sender, RoutedEventArgs e)
        {
            if (AvailableSaves.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a save");
                return;
            }
            var saveToRestore = AvailableSaves.SelectedItems[0]?.ToString();
            if (saveToRestore != null)
            {
                var from = System.IO.Path.Combine(SavesFolder.Text, saveToRestore);
                var to = PathToGame.Text;
                File.Move(to, $"{to}.bak", overwrite: true);
                File.Copy(from, to, overwrite: true);
                MessageBox.Show($"{from} restored to {to}");
            } 
            else
            {
                MessageBox.Show($"NULL selected");
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            AvailableSaves.ItemsSource = Saves();
        }
    }
}
