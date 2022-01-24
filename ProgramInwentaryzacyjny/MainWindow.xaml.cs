using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Magazyn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StorageMainPage();
        }
        private void Wyjście_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
